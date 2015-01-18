using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using CSharpQuery.Index;
using CSharpQuery.Thesaurus;
using CSharpQuery.WordBreaker;

namespace CSharpQuery.QueryEngine
{
    public class FreeTextQuery<T>
    {
        private readonly IWordBreaker wordBreaker;
        private readonly IThesaurus thesaurus;
        private readonly IEqualityComparer<WordReference<T>> wordReferenceEqualityComparer;
        private readonly ITextIndexSearcher<T> textIndexSearcher;

        #region Fields - Query Tuning

        public static decimal WeightWordMatching = 0.60m;
        public static decimal WeightLowPhraseIndex = 0.15m;
        public static decimal WeightSearchTermsProximity = 0.15m;
        public static decimal WeightMultipleOccurance = 0.10m;

        #endregion

        #region Properties

        public static SortedList<string, TextIndex<T>> Indexes { get; set; }
        public static ReaderWriterLock readerLock = new ReaderWriterLock();

        #endregion

        public FreeTextQuery(IWordBreaker wordBreaker, 
            IThesaurus thesaurus, 
            IEqualityComparer<WordReference<T>> wordReferenceEqualityComparer,
            ITextIndexSearcher<T> textIndexSearcher)
        {
            this.wordBreaker = wordBreaker;
            this.thesaurus = thesaurus;
            this.wordReferenceEqualityComparer = wordReferenceEqualityComparer;
            Indexes = new SortedList<string, TextIndex<T>>();
            this.textIndexSearcher = textIndexSearcher;
        }

        #region Public Static Methods

        public static void ExpireCachedIndexes()
        {
            readerLock.AcquireWriterLock(60*1000); // we'll wait 60 seconds for this!
            Indexes.Clear();
            GC.Collect(4); // include large object heap
            readerLock.ReleaseWriterLock();
        }

        #endregion

        public List<QueryResult<T>> SearchFreeTextQuery(TextIndex<T> textIndex, string query)
        {
            return SearchFreeTextQuery(textIndex, query, uint.MaxValue);
        }

        public List<QueryResult<T>> SearchFreeTextQuery(TextIndex<T> textIndex, string query, uint topNbyRank)
        {
            readerLock.AcquireReaderLock(1000*60); // 60 second timeout

            var results = new Dictionary<Synonym, List<WordReference<T>>>();

            foreach (var word in GetTheWordsBeingSearchedFor(query))
                results.Add(word, GetTheSearchResultsForThisWord(textIndex, word));

            readerLock.ReleaseReaderLock();

            var rankedResults = RankTheResults(query, IntersectTheResults(results));

            return rankedResults;
        }

        private List<QueryResult<T>> RankTheResults(string query, SortedList<T, QueryResult<T>> queryResults)
        {
            return RankResults(query, GetTheWordsBeingSearchedFor(query), queryResults);
        }

        private SortedList<T, QueryResult<T>> IntersectTheResults(Dictionary<Synonym, List<WordReference<T>>> results)
        {
            var resultList = new List<T>();
            var firstTime = true;
            foreach (var wrfs in results.Values)
            {
                if (firstTime)
                {
                    resultList = wrfs.Select(n => n.Key).ToList();
                    firstTime = false;
                    continue;
                }
                //resultList = resultList.Intersect(wrfs, comp).ToList();
                resultList = resultList.Intersect(wrfs.Select(n => n.Key)).ToList();
            }

            return PivitQuery(results, resultList);
        }

        private List<WordReference<T>> GetTheSearchResultsForThisWord(TextIndex<T> textIndex, Synonym word)
        {
            var resultsForThisWord = textIndexSearcher.SearchTheIndex(textIndex, word.OriginalWord);

            foreach (var suggestedWord in word.SuggestedWords)
            {
                if (suggestedWord == word.OriginalWord)
                    continue;

                var subResults = GetTheSearchResultsForTheSynonyms(textIndex, suggestedWord);
                resultsForThisWord.AddRange(subResults);
            }
            return resultsForThisWord;
        }

        private List<WordReference<T>> GetTheSearchResultsForTheSynonyms(TextIndex<T> textIndex, string suggestedWord)
        {
            var synonyms = wordBreaker.BreakWords(suggestedWord);

            var subResults = new List<WordReference<T>>();
            foreach (var synonym in synonyms)
            {
                var searchResults = textIndex[synonym.WordText];
                if (subResults.Count() == 0)
                    subResults.AddRange(searchResults);
                else
                    subResults = subResults.Intersect(searchResults, wordReferenceEqualityComparer).ToList();
            }
            return subResults;
        }

        private List<Synonym> GetTheWordsBeingSearchedFor(string query)
        {
            var queryWords = wordBreaker.BreakWords(query);
            return thesaurus.Suggest(queryWords);
        }

        public List<QueryResult<T>> SearchTextQuery(TextIndex<T> textIndex, string catalog, CultureInfo culture, string query)
        {
            readerLock.AcquireReaderLock(1000*60); // 60 second timeout

            var queryWordsList = wordBreaker.BreakWords(query);
            var wordList = queryWordsList.Select(n => new Synonym {OriginalWord = n.WordText}).ToList();

            var results = new Dictionary<Synonym, List<WordReference<T>>>();
            foreach (var word in wordList)
                results.Add(word, textIndex[word.OriginalWord]);

            readerLock.ReleaseReaderLock();

            // intersect the results -- what word ref's contain all phrases searched for
            var resultList = new List<T>();
            var firstTime = true;
            foreach (var wrfs in results.Values)
            {
                if (firstTime)
                {
                    resultList = wrfs.Select(n => n.Key).ToList();
                    firstTime = false;
                    continue;
                }
                resultList = resultList.Intersect(wrfs.Select(n => n.Key)).ToList();
            }

            var queryResult = PivitQuery(results, resultList);
            return RankResults(query, wordList, queryResult);
        }

        #region Private Methods

        /// <summary>
        ///   This function takes the raw list of words & results and the intersection list and combines the two
        /// </summary>
        /// <param name = "results">The word lookup results</param>
        /// <param name = "resultList">The intersection of the results lists</param>
        /// <returns>A SortedList(int, QueryResult) int: They [Key] value in the query; QueryResult: The matching words for that key</returns>
        private static SortedList<T, QueryResult<T>> PivitQuery(Dictionary<Synonym, List<WordReference<T>>> results, List<T> resultList)
        {
            // WordRef - Synonyms?
            var queryResults = new SortedList<T, QueryResult<T>>();
            var sr = new List<WordReference<T>>();
            foreach (var wrd in results.Keys)
            {
                sr.AddRange(
                    results[wrd].Where(n => resultList.Contains(n.Key))
                    );
            }

            foreach (var wr in sr)
            {
                if (queryResults.ContainsKey(wr.Key))
                    queryResults[wr.Key].WordIndexes.Add(wr);
                else
                {
                    var q = new QueryResult<T> {Key = wr.Key, WordIndexes = new List<WordReference<T>>()};
                    q.WordIndexes.Add(wr);
                    queryResults.Add(wr.Key, q);
                }
            }
            return queryResults;
        }

        private static List<QueryResult<T>> RankResults(string query, List<Synonym> words, SortedList<T, QueryResult<T>> queryResults)
        {
            foreach (var keyRef in queryResults.Keys)
            {
                // Rank this hit!
                var searchTermsProximity = RankSearchTermsProximity(query, words, queryResults[keyRef]);
                var wordMatching = RankWordMatching(query, words, queryResults[keyRef]);
                var lowPhraseIndex = RankLowPhraseIndex(query, words, queryResults[keyRef]);
                var multipleOccurance = RankMultipleOccurance(query, words, queryResults[keyRef]);

                var rank = (searchTermsProximity*WeightSearchTermsProximity) +
                           (wordMatching*WeightWordMatching) +
                           (lowPhraseIndex*WeightLowPhraseIndex) +
                           (multipleOccurance*WeightMultipleOccurance);
                var r = queryResults[keyRef];
                r.Rank = rank;

                r.searchTermsProximity = searchTermsProximity;
                r.wordMatching = wordMatching;
                r.lowPhraseIndex = lowPhraseIndex;
                r.multipleOccurance = multipleOccurance;
            }

            // Go through each and make sure they only contain
            return queryResults.Values.OrderByDescending(n => n.Rank).ToList();
        }

        private static decimal Normalize(decimal value)
        {
            if (value < 0)
                return 0;
            if (value > 1)
                return 1;
            return Math.Round(value, 4);
        }

        private static decimal RankMultipleOccurance(string query, List<Synonym> words, QueryResult<T> queryResult)
        {
            // Only use the original words
            decimal WordCount = 0;
            foreach (var word in words)
            {
                // Piano Jazz
                // Sum all of the piano's, then all of the "jazz"				
                // Exact match word = 2 points

                WordCount += (from q in queryResult.WordIndexes
                              where q.Word.Equals(word.OriginalWord, StringComparison.CurrentCultureIgnoreCase)
                              select q).Count();
            }

            var totalWords = (words.Count == 0 ? 1 : words.Count);
            var rank = ((WordCount + queryResult.WordIndexes.Count)/totalWords)/10;
            return Normalize(rank);
        }

        private static decimal RankLowPhraseIndex(string query, List<Synonym> words, QueryResult<T> queryResult)
        {
            // The sum of the pos
            decimal posVal = 0;
            foreach (var wr in queryResult.WordIndexes)
            {
                var val = (wr.PhraseIndex - wr.Word.Length);
                if (val > 0)
                    posVal += val;
            }

            var result = (words.Count/((posVal/words.Count) + 1))*(words.Count*2);
            return Normalize(result);
        }

        private static decimal RankWordMatching(string query, List<Synonym> words, QueryResult<T> queryResult)
        {
            // Original serch terms.  
            // The first search term gets more weight
            // If it contains all of the original search words (exactly) then 1
            // If it contains x / y; return x / y

            // Contains words in right order = .6
            // Contains all words in the search query = .4
            // Contains syninym words in the right order = ?

            // Contains words in right order = .6
            var rightOrder = true;
            var containsAllWords = true;
            var lastIndex = -1;
            foreach (var word in words)
            {
                var wrdIndx = queryResult.WordIndexes.Where(n => n.Word == word.OriginalWord).FirstOrDefault();
                if (wrdIndx != null)
                {
                    if (lastIndex < wrdIndx.PhraseIndex)
                        lastIndex = wrdIndx.PhraseIndex;
                    else
                    {
                        rightOrder = false;
                        break;
                    }
                }
                else
                {
                    containsAllWords = false;
                    break;
                }
            }

            // Contains all words in the search query
            //int wordCount = 0;
            //foreach (Synonym wrd in words) {
            //    if ((from q in queryResult.WordIndexes
            //         where q.Word.Equals(wrd.OriginalWord, StringComparison.CurrentCultureIgnoreCase)
            //         select q).Count() >= 1)
            //        wordCount++;
            //}

            //decimal result = ((wordCount + 1) / (words.Count + 1));

            var result = (rightOrder ? 0.6m : 0m) + (containsAllWords ? .4m : 0m);
            return Normalize(result);
        }

        private static decimal RankSearchTermsProximity(string query, List<Synonym> words, QueryResult<T> queryResult)
        {
            // Not counting the thesaurus looked up words, 
            // How close together are the search terms? Are they right next to each other?

            var wr = (from q in queryResult.WordIndexes orderby q.PhraseIndex select q).ToList();

            var distance = 0;
            for (var i = 0; i < wr.Count - 1; i++)
            {
                var w1 = wr[i];
                var w2 = wr[i + 1];

                // Distance between these two?
                var d = ((w1.PhraseIndex - w1.Word.Length) + (w2.PhraseIndex - w2.Word.Length));
                if (d < 0)
                    continue;
                else
                    distance += d;
            }

            var result = (words.Count/((distance + words.Count) + 1.0m))*(words.Count*2);
            return Normalize(result);
        }

        #endregion
    }
}