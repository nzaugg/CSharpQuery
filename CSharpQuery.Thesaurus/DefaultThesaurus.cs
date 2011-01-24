using System.Collections.Generic;
using System.Linq;
using CSharpQuery.WordBreaker;

namespace CSharpQuery.Thesaurus
{
    public class DefaultThesaurus : IThesaurus
    {
        private readonly IThesaurusDictionaryRetriever thesaurusDictionaryRetriever;

        public DefaultThesaurus(IThesaurusDictionaryRetriever thesaurusDictionaryRetriever)
        {
            this.thesaurusDictionaryRetriever = thesaurusDictionaryRetriever;
        }

        public List<Synonym> Suggest(List<Word> searchWords)
        {
            var thesaurusDictionary = thesaurusDictionaryRetriever.GetThesaurus();

            var results = new List<Synonym>();
            var removeWords = new List<string>();

            // Find each word or words
            foreach (var wrd in searchWords)
            {
                var s = new Synonym {OriginalWord = wrd.WordText, SuggestedWords = new List<string>()};
                results.Add(s);

                if (!thesaurusDictionary.ContainsKey(wrd.WordText))
                    continue;
                var result = thesaurusDictionary[wrd.WordText];
                s.SuggestedWords.AddRange(result.Keys);
            }

            // COMPOUND WORDS
            // Input: Bull Fight =>  Bullfight -> bull fight
            // Input: Bullfight => Bullfight -> bull fight

            // compound words => "Bull Fight" becomes -> "BullFight"
            for (var i = 0; i < searchWords.Count - 1; i++)
            {
                var word = searchWords[i].WordText + " " + searchWords[i + 1].WordText;
                if (!thesaurusDictionary.ContainsKey(word))
                    continue;

                var result = thesaurusDictionary[word];
                if (result != null)
                {
                    // We want to remove the "fight" part of "bull fight" -- bull has both
                    removeWords.Add(searchWords[i + 1].WordText);

                    // Find out which word is the compound word
                    // For now we are assuming the last word is the last word
                    var compoundWord = result.Keys[result.Count - 1];
                    var suggestedword = result.Keys[0];
                    results[i].OriginalWord = compoundWord;
                    results[i].SuggestedWords.AddRange(result.Keys.Where(n => n != compoundWord).ToList());
                    //results[i].SuggestedWords.Add(searchWords[i+1].WordText);
                    if (result.Count < i)
                    {
                        results[i].SuggestedWords.AddRange(results[i + 1].SuggestedWords);
                        if (!results[i].SuggestedWords.Contains(results[i + 1].OriginalWord))
                            results[i].SuggestedWords.Add(results[i + 1].OriginalWord);
                        results.RemoveAt(i + 1);
                    }
                }
            }

            results.RemoveAll(n => removeWords.Contains(n.OriginalWord));
            results.ForEach(n => n.SuggestedWords.RemoveAll(m => m == n.OriginalWord));
            return results;
        }
    }
}