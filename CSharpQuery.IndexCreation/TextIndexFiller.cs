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
        private readonly TextFileAccessContext textFileAccessContext;

        public TextIndexFiller(TextFileAccessContext textFileAccessContext)
        {
            this.textFileAccessContext = textFileAccessContext;
        }

        public void AddPhraseToIndex(TextIndex index, int key, string phrase)
        {
            // break the words
            var words =
                (new DefaultWordBreaker { DatabasePath = textFileAccessContext.Directory}).BreakWords(phrase);

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