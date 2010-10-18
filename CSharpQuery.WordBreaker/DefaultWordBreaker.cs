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
	public class DefaultWordBreaker {
		protected Dictionary<string, string> substitutions;
		protected List<char> whitespace;
		protected Dictionary<string, string> noiseWords;

		protected bool initialized = false;

		public CultureInfo Culture { get; set; }
		public string DatabasePath { get; set; }

		public DefaultWordBreaker() {
            this.Culture = new CultureInfo("en-US");
		}

		public void Initialize() {
			initialized = true;
			string folder = DatabasePath;

			// Load the global Substitutions List
			string filename = Path.Combine(folder, string.Format("Substitutions.global.txt"));
			LoadSubstitutions(filename);

			// Load the regional Substitutions List
			filename = Path.Combine(folder, string.Format("Substitutions.{0}.txt", Culture));
			LoadSubstitutions(filename);

			// Load the global whitespace list
			filename = Path.Combine(folder, string.Format("Whitespace.global.txt"));
			LoadWhitespace(filename);

			// Load the regional whitespace list
			filename = Path.Combine(folder, string.Format("Whitespace.{0}.txt", Culture));
			LoadWhitespace(filename);

			// Load the global noise words list
			filename = Path.Combine(folder, string.Format("NoiseWords.global.txt"));
			LoadNoiseWords(filename);

			// Load the regional noise words list
			filename = Path.Combine(folder, string.Format("NoiseWords.{0}.txt", Culture));
			LoadNoiseWords(filename);
		}

		private void LoadNoiseWords(string filename) {
			if (noiseWords == null)
				noiseWords = new Dictionary<string,string>();

			if (!File.Exists(filename))
				return;

			TextReader rdr = new StreamReader(File.Open(filename, FileMode.Open, FileAccess.Read, FileShare.Read), Encoding.Unicode);
			string line;
			while ((line = rdr.ReadLine()) != null) {
				line = line.Trim();
				if (!noiseWords.ContainsKey(line))
					noiseWords.Add(line, null);
			}
			rdr.Close();
		}

		private void LoadWhitespace(string filename) {
			// Format: {char}\r\n
			if (whitespace == null)
				whitespace = new List<char>();

			if (!File.Exists(filename))
				return;

			TextReader rdr = new StreamReader(File.Open(filename, FileMode.Open, FileAccess.Read, FileShare.Read), Encoding.Unicode);
			string line;
			while ((line = rdr.ReadLine()) != null) {
				// there should only be 1 char
				if (line.Length != 1) {
					Trace.WriteLine(string.Format("Invlaid Line in file '{0}'. Line: {1}", filename, line));
					continue; // ignore this
				}
				char chr = line[0];
				if ( !whitespace.Contains(chr) )
					whitespace.Add(chr);
			}
			rdr.Close();
			whitespace.Add('\r');
			whitespace.Add('\n');
		}

		private void LoadSubstitutions(string filename) {
			// Format: À=A\r\n  OR À=\r\n
			if (substitutions == null)
				substitutions = new Dictionary<string,string>();
			
			if (!File.Exists(filename))
				return;

			TextReader rdr = new StreamReader(File.Open(filename, FileMode.Open, FileAccess.Read, FileShare.Read), Encoding.Unicode);
			string line;
			while ((line = rdr.ReadLine()) != null) {
				string leftSide = line.Substring(0, line.IndexOf('=', 1));
				string rightSide = line.Substring(line.IndexOf('=', 1)).Replace("=", "");
				if (substitutions.ContainsKey(leftSide))
					substitutions[leftSide] = rightSide;
				else
					substitutions.Add(leftSide, rightSide);
			}
			rdr.Close();
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

			// Do a bit of pre-processing
			string lowerPhrase = phrase.ToLower(Culture);
			// Do replacements on multi-char substitutions
			substitutions.Keys.ToList().ForEach(n => lowerPhrase = lowerPhrase.Replace(n, substitutions[n]));

			List<Word> results = new List<Word>();
			List<string> words = lowerPhrase.Split(whitespace.ToArray(), StringSplitOptions.RemoveEmptyEntries).ToList();
			words.ForEach(n => n = n.Trim().ToLower(Culture));

			results.AddRange(from n in words where !noiseWords.ContainsKey(n) select new Word(n.Trim(), 0));

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
			word = word.Trim().Trim(whitespace.ToArray()).ToLower(Culture);
			if ( noiseWords.ContainsKey(word) )
				return null;
			if (string.IsNullOrEmpty(word))
				return null;			 

			return new Word(word, pos);
		}
	}
}
