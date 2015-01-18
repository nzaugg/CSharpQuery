using System.Collections.Generic;
using CSharpQuery.Index;
using CSharpQuery.WordBreaker;

namespace CSharpQuery.IndexCreation
{
	public class TextIndexFiller<T> : ITextIndexFiller<T>
	{
		private readonly IWordBreaker wordBreaker;

		public TextIndexFiller(IWordBreaker wordBreaker)
		{
			this.wordBreaker = wordBreaker;
		}

		public void AddPhraseToIndex(TextIndex<T> index, Phrase<T> phrase)
		{
			var words = GetTheWordsToIndex(phrase);

			AddTheWordsToTheIndex(words, phrase.Key, index);
		}

		private IEnumerable<Word> GetTheWordsToIndex(Phrase<T> phrase)
		{
			return wordBreaker.BreakWords(phrase.Text);
		}

		private static void AddTheWordsToTheIndex(IEnumerable<Word> words, T key, TextIndex<T> index)
		{
			foreach (var word in words)
			{
				var wordReference = new WordReference<T> {Word = word.WordText, Key = key, PhraseIndex = word.Index};

				if (ThisWordIsNotInTheIndex(word, index))
					AddTheNewWordToTheIndex(word, index, wordReference);
				else
					AddTheNewUseOfThisWordToTheIndex(word, index, wordReference);
			}
		}

		private static void AddTheNewUseOfThisWordToTheIndex(Word word, TextIndex<T> index, WordReference<T> wordReference)
		{
			index[word.WordText].Add(wordReference);
		}

		private static void AddTheNewWordToTheIndex(Word word, TextIndex<T> index, WordReference<T> wordReference)
		{
			var wordReferences = new List<WordReference<T>>(new[] { wordReference });
			index.Add(word.WordText, wordReferences);
		}

		private static bool ThisWordIsNotInTheIndex(Word word, TextIndex<T> index)
		{
			return !index.ContainsKey(word.WordText);
		}
	}
}