using System.Collections.Generic;

namespace CSharpQuery.WordBreaker
{
    public interface IWordBreaker
    {
        /// <summary>
        /// Breaks up words. See Class Notes
        /// </summary>
        /// <param name="phrase">Original Phrase to be broken</param>
        /// <returns>Dictionary<string, int> string=word, int=Index In Phrase</returns>
        List<Word> BreakWords(string phrase);
    }
}