/***** CSharpQuery **** By: Nathan Zaugg **** Created: 3/9/2009 *************
 * This software is licensed under Microsoft Public License (Ms-PL)			*
 * http://www.microsoft.com/opensource/licenses.mspx						*
 *																			*
 * Downloaded From: http://www.InteractiveASP.NET							*
 ****************************************************************************/

using System.Collections.Generic;
using System.Collections;
using System.Globalization;
using CSharpQuery.WordBreaker;
using CSharpQuery.Thesaurus;

namespace CSharpQuery.Index
{
    public class TextIndex
	{
		private static string BeginFile = (char)01 + "CSharpQuery_Index" + (char)01;
		private static string BeginRecord = (char)02 + "";
		private static string BeginField = (char)03 + "";
		private static string FieldInfoDelimeter = (char)04 + "";
		private static string EndField = (char)05 + "";
		private static string EndRecord = (char)06 + "\r\n";
		private static string EndFile = (char)07 + "";

		protected SortedList<string, List<WordRef>> wordIndex;

		public CultureInfo Culture { get; set; }
		public string Name { get; private set; }
        public string IndexFolder { get; private set; }

	    public TextIndex() {
            var culture = new CultureInfo("en-US");

			this.wordIndex = new SortedList<string, List<WordRef>>();
		    this.Culture = culture;
		}

        public SortedList<string, List<WordRef>> WordIndex
        {
            get { return wordIndex; }
        }

		public List<WordRef> this[string word] {
			get {
				List<WordRef> result = null;
				wordIndex.TryGetValue(word, out result);
				return result;
			}
		}

		public List<WordRef> FrontMatch(string frontString)
		{
            var binarySearcher = new BinarySearch((str1, str2) => str2.StartsWith(str1) ? 0 : str1.CompareTo(str2));
			var words = binarySearcher.Search(wordIndex, frontString);

			List<WordRef> results = new List<WordRef>();
			foreach (string word in words) {
				results.AddRange(this[word]);
			}
			return results;
		}

		public void Initialize(string databasePath, string name) {
			IndexFolder = databasePath;
		    Name = name;
		}

		public void AddPhrase(int key, string phrase)
		{
			// break the words
            List<Word> words = (new DefaultWordBreaker(new CultureInfo("en-US")){DatabasePath = IndexFolder}).BreakWords(phrase);

			if (words == null)
				return;

			// add the words to the index
			foreach (Word wrd in words) {
				WordRef reference = new WordRef(wrd.WordText, key, wrd.Index);
				if (!wordIndex.ContainsKey(wrd.WordText))
					wordIndex.Add(wrd.WordText, new List<WordRef>(new WordRef[] { reference }));
				else {
					List<WordRef> wordRefs = wordIndex[wrd.WordText] as List<WordRef>;
					wordRefs.Add(reference);
				}
			}
		}



		public List<WordRef> FindWord(string wordText) {
			return this[wordText];
		}
	}
}
