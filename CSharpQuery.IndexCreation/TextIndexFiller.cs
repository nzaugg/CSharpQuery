using System.Collections.Generic;
using CSharpQuery.Index;
using CSharpQuery.WordBreaker;

namespace CSharpQuery.IndexCreation
{
    public interface ITextIndexFiller
    {
        void AddPhraseToIndex(TextIndex index, int key, string phrase);
    }

    public class TextIndexFiller : ITextIndexFiller
    {
        private readonly IWordBreaker wordBreaker;

        public TextIndexFiller(IWordBreaker wordBreaker)
        {
            this.wordBreaker = wordBreaker;
        }

        public void AddPhraseToIndex(TextIndex index, int key, string phrase)
        {
            var words = wordBreaker.BreakWords(phrase);

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