using System.Collections.Generic;
using CSharpQuery.WordBreaker;

namespace CSharpQuery.Index
{
    public class TextIndexFiller
    {
        private string indexFolder;

        public void Initialize(string indexFolder)
        {
            this.indexFolder = indexFolder;
        }

        public void AddPhraseToIndex(TextIndex index, int key, string phrase)
        {
            // break the words
            var words =
                (new DefaultWordBreaker {DatabasePath = indexFolder}).BreakWords(phrase);

            if (words == null)
                return;

            // add the words to the index
            foreach (var wrd in words)
            {
                var reference = new WordRef(wrd.WordText, key, wrd.Index);
                if (!index.ContainsKey(wrd.WordText))
                    index.Add(wrd.WordText, new List<WordRef>(new[] {reference}));
                else
                {
                    var wordRefs = index[wrd.WordText];
                    wordRefs.Add(reference);
                }
            }
        }
    }
}