/***** CSharpQuery **** By: Nathan Zaugg **** Created: 3/9/2009 *************
 * This software is licensed under Microsoft Public License (Ms-PL)			*
 * http://www.microsoft.com/opensource/licenses.mspx						*
 *																			*
 * Downloaded From: http://www.InteractiveASP.NET							*
 ****************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using CSharpQuery.Index;
using System.Threading;
using System.Globalization;
using CSharpQuery.WordBreaker;
using CSharpQuery.Thesaurus;

namespace CSharpQuery.QueryEngine {
	public class FreeTextQuery {
	    private readonly TextFileAccessContext textFileAccessContext;
	    private readonly TextIndexReader textIndexReader;
	    private TextIndexSearcher textIndexSearcher;

	    #region Fields - Query Tuning
		public static decimal WeightWordMatching = 0.60m;
		public static decimal WeightLowPhraseIndex = 0.15m;
		public static decimal WeightSearchTermsProximity = 0.15m;
		public static decimal WeightMultipleOccurance = 0.10m;
		#endregion

		#region Properties
		public static SortedList<string, TextIndex> Indexes { get; set; }
		public static ReaderWriterLock readerLock = new ReaderWriterLock();
		#endregion

        public FreeTextQuery(TextFileAccessContext textFileAccessContext)
	    {
            this.textFileAccessContext = textFileAccessContext;
            textIndexReader = new TextIndexReader(textFileAccessContext);
            Indexes = new SortedList<string, TextIndex>();
            textIndexSearcher = new TextIndexSearcher();
	    }

		#region Public Static Methods
		public static void ExpireCachedIndexes() {
			readerLock.AcquireWriterLock(60 * 1000); // we'll wait 60 seconds for this!
			Indexes.Clear();
			GC.Collect(4); // include large object heap
			readerLock.ReleaseWriterLock();
		}
		#endregion

		public List<QueryResult> SearchFreeTextQuery(string query) {
			return SearchFreeTextQuery(query, uint.MaxValue);
		}

		public List<QueryResult> SearchFreeTextQuery(string query, uint topNbyRank) {
			readerLock.AcquireReaderLock(1000 * 60); // 60 second timeout

            var index = textIndexReader.GetTextIndex();

		    // Get all of the words (front match)
            List<Word> queryWordsList = (new WordBreaker.DefaultWordBreaker(new WordBreakingInformationRetriever(textFileAccessContext.Directory, textFileAccessContext.Culture)) { DatabasePath = textFileAccessContext.Directory }).BreakWords(query);

			Dictionary<Synonym, List<WordRef>> results = new Dictionary<Synonym, List<WordRef>>();
            List<Synonym> words = (new DefaultThesaurus(textFileAccessContext.Culture) { DatabasePath = textFileAccessContext.Directory }).Suggest(queryWordsList);
			WordRefEqualityComparer comp = new WordRefEqualityComparer();

			foreach (Synonym word in words) {
				List<WordRef> res = textIndexSearcher.FrontMatch(index, word.OriginalWord);

				// Synonyms
				foreach (string syn in word.SuggestedWords) {
					if (syn == word.OriginalWord)
						continue;

                    List<Word> synBreaker = (new WordBreaker.DefaultWordBreaker(new WordBreakingInformationRetriever(textFileAccessContext.Directory,textFileAccessContext.Culture)) { DatabasePath = textFileAccessContext.Directory }).BreakWords(syn);
					List<WordRef> SubResults = new List<WordRef>();
					bool FirstLoop = true;

					foreach (Word w in synBreaker) {
						// maybe intersect the results here?
						List<WordRef> lookup = textIndexSearcher.FindWord(index, w.WordText);
						if (lookup != null) {
							if (FirstLoop) {
								SubResults.AddRange(lookup);
								FirstLoop = false;
							} else
								SubResults = SubResults.Intersect(lookup, comp).ToList();
						}
					}

					// Add the matchie words
					if ( SubResults != null )
						res.AddRange(SubResults);
				}
				results.Add(word, res);
			}
			readerLock.ReleaseReaderLock();
			// Intersect results

			// intersect the results
			List<int> resultList = new List<int>();
			bool firstTime = true;
			foreach (List<WordRef> wrfs in results.Values) {
				if (firstTime) {
					resultList = wrfs.Select(n => n.Key).ToList();
					firstTime = false;
					continue;
				}
				//resultList = resultList.Intersect(wrfs, comp).ToList();
				resultList = resultList.Intersect(wrfs.Select( n => n.Key )).ToList();
			}

			SortedList<int, QueryResult> queryResults = PivitQuery(results, resultList);

			// Rank the results!
			return RankResults(query, words, queryResults);
		}

	    public List<QueryResult> SearchTextQuery(string catalog, CultureInfo culture, string query) {
			readerLock.AcquireReaderLock(1000 * 60); // 60 second timeout
		    var textIndexReader = new TextIndexReader(textFileAccessContext);
		    TextIndex index = textIndexReader.GetTextIndex();

			// Get all of the words (front match)
            List<Word> queryWordsList = (new WordBreaker.DefaultWordBreaker(new WordBreakingInformationRetriever(textFileAccessContext.Directory, textFileAccessContext.Culture)) { DatabasePath = textFileAccessContext.Directory }).BreakWords(query);
			List<Synonym> wordList = queryWordsList.Select(n => new Synonym() { OriginalWord = n.WordText }).ToList();

			Dictionary<Synonym, List<WordRef>> results = new Dictionary<Synonym, List<WordRef>>();
			foreach (Synonym word in wordList) {
				List<WordRef> res = textIndexSearcher.FindWord(index, word.OriginalWord);
				results.Add(word, res);
			}
			readerLock.ReleaseReaderLock();

			// intersect the results -- what word ref's contain all phrases searched for
			List<int> resultList = new List<int>();
			WordRefEqualityComparer comp = new WordRefEqualityComparer();
			bool firstTime = true;
			foreach (List<WordRef> wrfs in results.Values) {
				if (firstTime) {
					resultList = wrfs.Select(n => n.Key).ToList();
					firstTime = false;
					continue;
				}
				resultList = resultList.Intersect(wrfs.Select(n => n.Key)).ToList();
			}

			SortedList<int, QueryResult> queryResult = PivitQuery(results, resultList);
			return RankResults(query, wordList, queryResult);
		}

	    #region Private Methods

	    /// <summary>
		/// This function takes the raw list of words & results and the intersection list and combines the two
		/// </summary>
		/// <param name="results">The word lookup results</param>
		/// <param name="resultList">The intersection of the results lists</param>
		/// <returns>A SortedList(int, QueryResult) int: They [Key] value in the query; QueryResult: The matching words for that key</returns>
		private static SortedList<int, QueryResult> PivitQuery(Dictionary<Synonym, List<WordRef>> results, List<int> resultList) {
			// WordRef - Synonyms?
			SortedList<int, QueryResult> queryResults = new SortedList<int, QueryResult>();
			List<WordRef> sr = new List<WordRef>();
			foreach (Synonym wrd in results.Keys) {
				sr.AddRange(
					results[wrd].Where(n => resultList.Contains(n.Key))
				);
			}

			foreach (WordRef wr in sr) {
				if (queryResults.ContainsKey(wr.Key))
					queryResults[wr.Key].WordIndexes.Add(wr);
				else {
					QueryResult q = new QueryResult() { Key = wr.Key, WordIndexes = new List<WordRef>() };
					q.WordIndexes.Add(wr);
					queryResults.Add(wr.Key, q);
				}
			}
			return queryResults;
		}

		private static List<QueryResult> RankResults(string query, List<Synonym> words, SortedList<int, QueryResult> queryResults) {
			
			foreach (int keyRef in queryResults.Keys) {
				
				// Rank this hit!
				decimal searchTermsProximity = RankSearchTermsProximity(query, words, queryResults[keyRef]);
				decimal wordMatching = RankWordMatching(query, words, queryResults[keyRef]);
				decimal lowPhraseIndex = RankLowPhraseIndex(query, words, queryResults[keyRef]);
				decimal multipleOccurance = RankMultipleOccurance(query, words, queryResults[keyRef]);

				decimal rank = (searchTermsProximity * WeightSearchTermsProximity) +
								(wordMatching * WeightWordMatching) +
								(lowPhraseIndex * WeightLowPhraseIndex) +
								(multipleOccurance * WeightMultipleOccurance);
				QueryResult r = queryResults[keyRef];
				r.Rank = rank;

				r.searchTermsProximity = searchTermsProximity;
				r.wordMatching = wordMatching;
				r.lowPhraseIndex = lowPhraseIndex;
				r.multipleOccurance = multipleOccurance;
			}			

			// Go through each and make sure they only contain
			return queryResults.Values.OrderByDescending(n => n.Rank).ToList();
		}

		private static decimal Normalize(decimal value) {
			if (value < 0)
				return 0;
			if (value > 1)
				return 1;
			return Math.Round(value, 4);
		}

		private static decimal RankMultipleOccurance(string query, List<Synonym> words, QueryResult queryResult) {
			// Only use the original words
			decimal WordCount = 0;
			foreach (Synonym word in words) {
				// Piano Jazz
				// Sum all of the piano's, then all of the "jazz"				
				// Exact match word = 2 points

				WordCount += (from q in queryResult.WordIndexes
						   where q.Word.Equals(word.OriginalWord, StringComparison.CurrentCultureIgnoreCase)
						   select q).Count();
			}

			int totalWords = (words.Count == 0 ? 1 : words.Count);
			decimal rank = ((WordCount + queryResult.WordIndexes.Count) / totalWords) / 10;
			return Normalize(rank);
		}

		private static decimal RankLowPhraseIndex(string query, List<Synonym> words, QueryResult queryResult) {
			// The sum of the pos
			decimal posVal = 0;
			foreach (WordRef wr in queryResult.WordIndexes) {
				int val = (wr.PhraseIndex - wr.Word.Length);
				if (val > 0)
					posVal += val;
			}

			decimal result = (words.Count / ((posVal / words.Count) + 1)) * (words.Count * 2);
			return Normalize(result);
		}

		private static decimal RankWordMatching(string query, List<Synonym> words, QueryResult queryResult) {
			// Original serch terms.  
			// The first search term gets more weight
			// If it contains all of the original search words (exactly) then 1
			// If it contains x / y; return x / y

			// Contains words in right order = .6
			// Contains all words in the search query = .4
			// Contains syninym words in the right order = ?

			// Contains words in right order = .6
			bool rightOrder = true;
			bool containsAllWords = true;
			int lastIndex = -1;
			foreach (Synonym word in words) {
				WordRef wrdIndx = queryResult.WordIndexes.Where(n => n.Word == word.OriginalWord).FirstOrDefault();
				if (wrdIndx != null) {
					if (lastIndex < wrdIndx.PhraseIndex)
						lastIndex = wrdIndx.PhraseIndex;
					else {
						rightOrder = false;
						break;
					}
				} else {
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

			decimal result = (rightOrder ? 0.6m : 0m) + (containsAllWords ? .4m : 0m);
			return Normalize(result);
		}

		private static decimal RankSearchTermsProximity(string query, List<Synonym> words, QueryResult queryResult) {
			// Not counting the thesaurus looked up words, 
			// How close together are the search terms? Are they right next to each other?

			List<WordRef> wr = (from q in queryResult.WordIndexes orderby q.PhraseIndex select q).ToList();

			int distance = 0;
			for (int i = 0; i < wr.Count-1; i++) {
				WordRef w1 = wr[i];
				WordRef w2 = wr[i + 1];

				// Distance between these two?
				int d = ((w1.PhraseIndex - w1.Word.Length) + (w2.PhraseIndex - w2.Word.Length));
				if (d < 0)
					continue;
				else
					distance += d;
			}

			decimal result = (words.Count / ((distance + words.Count) + 1.0m)) * (words.Count * 2);
			return Normalize(result);
		}
		#endregion
	}

    public class TextIndexReader
    {
        private readonly TextFileAccessContext textFileAccessContext;

        public TextIndexReader(TextFileAccessContext textFileAccessContext)
        {
            this.textFileAccessContext = textFileAccessContext;
        }

        public TextIndex GetTextIndex()
        {
            var indexKey = textFileAccessContext.Name + textFileAccessContext.Culture;

            if (FreeTextQuery.Indexes.Keys.Contains(indexKey))
                return FreeTextQuery.Indexes[indexKey];

            var textIndexLoader = new TextIndexLoader(textFileAccessContext);
            var index = textIndexLoader.LoadIndex();

            var lc = FreeTextQuery.readerLock.UpgradeToWriterLock(1000 * 60); // 60 seconds!
            FreeTextQuery.Indexes.Add(indexKey, index);
            FreeTextQuery.readerLock.DowngradeFromWriterLock(ref lc);
            return index;
        }
    }
}
