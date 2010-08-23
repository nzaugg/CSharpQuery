/***** CSharpQuery **** By: Nathan Zaugg **** Created: 3/9/2009 *************
 * This software is licensed under Microsoft Public License (Ms-PL)			*
 * http://www.microsoft.com/opensource/licenses.mspx						*
 *																			*
 * Downloaded From: http://www.InteractiveASP.NET							*
 ****************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using System.IO;
using System.Xml.Linq;
using CSharpQuery.WordBreaker;

namespace CSharpQuery.Thesaurus {
	public class DefaultThesaurus {
		protected SortedList<string, SortedList<string, int>> thesaurusDictionary;

		protected bool initialized = false;

		public CultureInfo Culture { get; set; }
		public string DatabasePath { get; set; }

		public DefaultThesaurus(CultureInfo culture) {
			this.thesaurusDictionary = new SortedList<string, SortedList<string, int>>();
			this.Culture = culture;
		}

		public void Initialize() {
			initialized = true;

			// Thesaurus.global.xml
			string filename = Path.Combine(DatabasePath, "Thesaurus.global.xml");
			LoadThesaurus(filename);

			// Thesaurus.en-US.xml
			filename = Path.Combine(DatabasePath, "Thesaurus.global.xml");
			LoadThesaurus(filename);
		}

		private void LoadThesaurus(string filename) {

			if (!File.Exists(filename))
				return;
			// Sample XML
			//<XML ID="Microsoft Search Thesaurus">
			//    <thesaurus xmlns="x-schema:tsSchema.xml">
			//    <diacritics_sensitive>0</diacritics_sensitive>
			//        <expansion>
			//            <sub>abduct ion</sub>
			//            <sub>abduction</sub>
			//        </expansion>
			//        <expansion>
			//            <sub>abe lard</sub>
			//            <sub>abelard</sub>
			//        </expansion>
			//    </thesaurus>
			//</XML>

			// Read the XML
			XDocument xmlDoc = XDocument.Load(new StreamReader(File.Open(filename, FileMode.Open, FileAccess.Read, FileShare.Read)));

			var query = from e in xmlDoc.Descendants("expansion")
						select e;

			foreach (XElement element in query) {
				List<string> subs = (from s in element.Descendants("sub")
									 select s.Value).ToList();
				SortedList<string, int> syns = new SortedList<string,int>();
				foreach ( string syn in subs)
					syns.Add(syn, 0);

				foreach (string wrd in syns.Keys) {
					if (thesaurusDictionary.ContainsKey(wrd)) {
						foreach (string w in syns.Keys) {
							if (thesaurusDictionary.ContainsKey(w))
								continue;
							thesaurusDictionary[wrd].Add(w, 0);
						}
					} else
						thesaurusDictionary.Add(wrd, syns);
				}
			}
		}

		public List<Synonym> Suggest(List<Word> searchWords) {
			if (!initialized)
				Initialize();

			List<Synonym> results = new List<Synonym>();
			List<string> removeWords = new List<string>();

			// Find each word or words
			foreach (Word wrd in searchWords) {
				Synonym s = new Synonym() { OriginalWord = wrd.WordText, SuggestedWords=new List<string>() };
				results.Add(s);

				if (!thesaurusDictionary.ContainsKey(wrd.WordText))
					continue;
				SortedList<string, int> result = thesaurusDictionary[wrd.WordText];
				s.SuggestedWords.AddRange(result.Keys);
			}


			// COMPOUND WORDS
			// Input: Bull Fight =>  Bullfight -> bull fight
			// Input: Bullfight => Bullfight -> bull fight

			// compound words => "Bull Fight" becomes -> "BullFight"
			for (int i = 0; i < searchWords.Count - 1; i++) {
				string word = searchWords[i].WordText + " " + searchWords[i + 1].WordText;
				if (!thesaurusDictionary.ContainsKey(word))
					continue;
				
				SortedList<string, int> result = thesaurusDictionary[word];
				if (result != null) {
					// We want to remove the "fight" part of "bull fight" -- bull has both
					removeWords.Add(searchWords[i + 1].WordText);

					// Find out which word is the compound word
					// For now we are assuming the last word is the last word
					string compoundWord = result.Keys[result.Count - 1];
					string suggestedword = result.Keys[0];
					results[i].OriginalWord = compoundWord;
					results[i].SuggestedWords.AddRange(result.Keys.Where(n => n != compoundWord).ToList());
					//results[i].SuggestedWords.Add(searchWords[i+1].WordText);
					if (result.Count < i) {
						results[i].SuggestedWords.AddRange(results[i + 1].SuggestedWords);
						if (!results[i].SuggestedWords.Contains(results[i + 1].OriginalWord))
							results[i].SuggestedWords.Add(results[i + 1].OriginalWord);
						results.RemoveAt(i + 1);
					}
				}
			}

			results.RemoveAll(n => removeWords.Contains(n.OriginalWord));
			results.ForEach(n => n.SuggestedWords.RemoveAll(m => m == n.OriginalWord));
			return results;
		}
	}
}
