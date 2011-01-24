/***** CSharpQuery **** By: Nathan Zaugg **** Created: 3/9/2009 *************
 * This software is licensed under Microsoft Public License (Ms-PL)			*
 * http://www.microsoft.com/opensource/licenses.mspx						*
 *																			*
 * Downloaded From: http://www.InteractiveASP.NET							*
 ****************************************************************************/

using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;
using System.Collections;
using System.IO;
using System.Linq;
using System.Diagnostics;

namespace CSharpQuery.WordBreaker {
    public interface IWordBreaker
    {
        /// <summary>
        /// Breaks up words. See Class Notes
        /// </summary>
        /// <param name="phrase">Original Phrase to be broken</param>
        /// <returns>Dictionary<string, int> string=word, int=Index In Phrase</returns>
        List<Word> BreakWords(string phrase);
    }

    public interface IWordBreakingInformationRetriever
    {
        WordBreakingInformation GetWordBreakingInformation();
    }

    public class WordBreakingInformationRetriever : IWordBreakingInformationRetriever
    {
        private IDictionary<string, string> substitutions = new Dictionary<string, string>();
        private IList<char> whitespace = new List<char>();
        private IDictionary<string, string> noiseWords = new Dictionary<string, string>();
        private string databasePath;
        private readonly CultureInfo cultureInfo;

        public WordBreakingInformationRetriever(string databasePath, CultureInfo cultureInfo)
        {
            this.databasePath = databasePath;
            this.cultureInfo = cultureInfo;
        }

        public WordBreakingInformation GetWordBreakingInformation()
        {
            string folder = databasePath;

            // Load the global Substitutions List
            string filename = Path.Combine(folder, string.Format("Substitutions.global.txt"));
            LoadSubstitutions(filename);

            // Load the regional Substitutions List
            filename = Path.Combine(folder, string.Format("Substitutions.{0}.txt", cultureInfo));
            LoadSubstitutions(filename);

            // Load the global whitespace list
            filename = Path.Combine(folder, string.Format("Whitespace.global.txt"));
            LoadWhitespace(filename);

            // Load the regional whitespace list
            filename = Path.Combine(folder, string.Format("Whitespace.{0}.txt", cultureInfo));
            LoadWhitespace(filename);

            // Load the global noise words list
            filename = Path.Combine(folder, string.Format("NoiseWords.global.txt"));
            LoadNoiseWords(filename);

            // Load the regional noise words list
            filename = Path.Combine(folder, string.Format("NoiseWords.{0}.txt", cultureInfo));
            LoadNoiseWords(filename);

            return null;
        }

        private void LoadNoiseWords(string filename)
        {
            if (noiseWords == null)
                noiseWords = new Dictionary<string, string>();

            if (!File.Exists(filename))
                return;

            TextReader rdr = new StreamReader(File.Open(filename, FileMode.Open, FileAccess.Read, FileShare.Read), Encoding.Unicode);
            string line;
            while ((line = rdr.ReadLine()) != null)
            {
                line = line.Trim();
                if (!noiseWords.ContainsKey(line))
                    noiseWords.Add(line, null);
            }
            rdr.Close();
        }

        private void LoadWhitespace(string filename)
        {
            // Format: {char}\r\n
            if (whitespace == null)
                whitespace = new List<char>();

            if (!File.Exists(filename))
                return;

            TextReader rdr = new StreamReader(File.Open(filename, FileMode.Open, FileAccess.Read, FileShare.Read), Encoding.Unicode);
            string line;
            while ((line = rdr.ReadLine()) != null)
            {
                // there should only be 1 char
                if (line.Length != 1)
                {
                    Trace.WriteLine(string.Format("Invlaid Line in file '{0}'. Line: {1}", filename, line));
                    continue; // ignore this
                }
                char chr = line[0];
                if (!whitespace.Contains(chr))
                    whitespace.Add(chr);
            }
            rdr.Close();
            whitespace.Add('\r');
            whitespace.Add('\n');
        }

        private void LoadSubstitutions(string filename)
        {
            // Format: À=A\r\n  OR À=\r\n
            if (substitutions == null)
                substitutions = new Dictionary<string, string>();

            if (!File.Exists(filename))
                return;

            TextReader rdr = new StreamReader(File.Open(filename, FileMode.Open, FileAccess.Read, FileShare.Read), Encoding.Unicode);
            string line;
            while ((line = rdr.ReadLine()) != null)
            {
                string leftSide = line.Substring(0, line.IndexOf('=', 1));
                string rightSide = line.Substring(line.IndexOf('=', 1)).Replace("=", "");
                if (substitutions.ContainsKey(leftSide))
                    substitutions[leftSide] = rightSide;
                else
                    substitutions.Add(leftSide, rightSide);
            }
            rdr.Close();
        }
    }

    public class WordBreakingInformation
    {
        public IDictionary<string, string> Substitutions { get; set; }
        public IList<char> Whitespace { get; set; }
        public IDictionary<string, string> NoiseWords { get; set; }
    }

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
