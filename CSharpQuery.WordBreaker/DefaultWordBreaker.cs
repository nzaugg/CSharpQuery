/***** CSharpQuery **** By: Nathan Zaugg **** Created: 3/9/2009 *************
 * This software is licensed under Microsoft Public License (Ms-PL)			*
 * http://www.microsoft.com/opensource/licenses.mspx						*
 *																			*
 * Downloaded From: http://www.InteractiveASP.NET							*
 ****************************************************************************/

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Collections;
using System.Linq;

namespace CSharpQuery.WordBreaker {
    /// <summary>
	/// Responsible for the creation of an Index.
	/// This class will break a phrase into words.
	/// It also has the responsibility of:
	/// 1) Keeping track of the position of the words in the phrase
	/// 2) Removing invalid chars & Replacing diacritic and other strings
	///    using a substitution list
	/// 
	/// The word breaker has configuration files that work
	/// in both the gloabl and regional dialect.
	/// </summary>
	public class DefaultWordBreaker : IWordBreaker
    {
        private readonly IWordBreakingInformationRetriever wordBreakingInformationRetriever;

        protected bool initialized = false;

		public CultureInfo Culture { get; set; }
		public string DatabasePath { get; set; }

		public DefaultWordBreaker(IWordBreakingInformationRetriever wordBreakingInformationRetriever)
		{
		    this.wordBreakingInformationRetriever = wordBreakingInformationRetriever;
		    this.Culture = new CultureInfo("en-US");
		}

        public void Initialize() {
			initialized = true;

		}

		


		/// <summary>
		/// Breaks up words. See Class Notes
		/// </summary>
		/// <param name="phrase">Original Phrase to be broken</param>
		/// <returns>Dictionary<string, int> string=word, int=Index In Phrase</returns>
		public List<Word> BreakWords(string phrase) {
			if (string.IsNullOrEmpty(phrase))
				return null;

			if (!initialized)
				Initialize();


		    var info = wordBreakingInformationRetriever.GetWordBreakingInformation();

			// Do a bit of pre-processing
			string lowerPhrase = phrase.ToLower(Culture);
			// Do replacements on multi-char substitutions
            info.Substitutions.Keys.ToList().ForEach(n => lowerPhrase = lowerPhrase.Replace(n, info.Substitutions[n]));

			var results = new List<Word>();
            var words = lowerPhrase.Split(info.Whitespace.ToArray(), StringSplitOptions.RemoveEmptyEntries).ToList();
			words.ForEach(n => n = n.Trim().ToLower(Culture));

			results.AddRange(from n in words where !info.NoiseWords.ContainsKey(n) select new Word(n.Trim(), 0));

			// Remove the empties
			results.RemoveAll(n => string.IsNullOrEmpty(n.WordText));

			// Find the indexes
			int index = 0;
			foreach (Word word in results) {
				index = lowerPhrase.IndexOf(word.WordText, index);
				word.Index = index;
			}

			return results;
		}

		private Word CreateWord(string word, int pos) {
			// check to see if she is a noise word
		    var wordBreakingInformation = wordBreakingInformationRetriever.GetWordBreakingInformation();
		    word = word.Trim().Trim(wordBreakingInformation.Whitespace.ToArray()).ToLower(Culture);
			if ( wordBreakingInformation.NoiseWords.ContainsKey(word) )
				return null;
			if (string.IsNullOrEmpty(word))
				return null;			 

			return new Word(word, pos);
		}
	}

}
