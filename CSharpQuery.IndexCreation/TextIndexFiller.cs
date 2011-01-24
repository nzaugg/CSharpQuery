using System.Collections.Generic;
using CSharpQuery.Index;
using CSharpQuery.WordBreaker;

namespace CSharpQuery.IndexCreation
{
    public class TextIndexFiller : ITextIndexFiller
    {
        private readonly IWordBreaker wordBreaker;

        public TextIndexFiller(IWordBreaker wordBreaker)
        {
            this.wordBreaker = wordBreaker;
        }

        public void AddPhraseToIndex(TextIndex index, Phrase phrase)
        {
            var words = GetTheWordsToIndex(phrase);

            AddTheWordsToTheIndex(words, phrase.Key, index);
        }

        private IEnumerable<Word> GetTheWordsToIndex(Phrase phrase)
        {
            return wordBreaker.BreakWords(phrase.Text);
        }

        private static void AddTheWordsToTheIndex(IEnumerable<Word> words, int key, TextIndex index)
        {
            foreach (var word in words)
            {
                var wordReference = new WordReference {Word = word.WordText, Key = key, PhraseIndex = word.Index};

                if (ThisWordIsNotInTheIndex(word, index))
                    AddTheNewWordToTheIndex(word, index, wordReference);
                else
                    AddTheNewUseOfThisWordToTheIndex(word, index, wordReference);
            }
        }

        private static void AddTheNewUseOfThisWordToTheIndex(Word word, TextIndex index, WordReference wordReference)
        {
            index[word.WordText].Add(wordReference);
        }

        private static void AddTheNewWordToTheIndex(Word word, TextIndex index, WordReference wordReference)
        {
            var wordReferences = new List<WordReference>(new[] {wordReference});
            index.Add(word.WordText, wordReferences);
        }

        private static bool ThisWordIsNotInTheIndex(Word word, TextIndex index)
        {
            return !index.ContainsKey(word.WordText);
        }
    }
}