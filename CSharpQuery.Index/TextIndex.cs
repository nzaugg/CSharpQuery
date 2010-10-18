/***** CSharpQuery **** By: Nathan Zaugg **** Created: 3/9/2009 *************
 * This software is licensed under Microsoft Public License (Ms-PL)			*
 * http://www.microsoft.com/opensource/licenses.mspx						*
 *																			*
 * Downloaded From: http://www.InteractiveASP.NET							*
 ****************************************************************************/

using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Globalization;
using CSharpQuery.WordBreaker;
using System.IO;
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

		public DefaultThesaurus Thesaurus { get; set; }

		public CultureInfo Culture { get; set; }
		public string Name { get; set; }
		public string IndexFolder { get; set; }
		public string IndexFileName {
			get {
				return Path.Combine(IndexFolder,
					string.Format("Index_{0}.{1}.index", Name, Culture == CultureInfo.InvariantCulture ? "invarient" : Culture.ToString()));
			}
		}

		public TextIndex() {
            var culture = new CultureInfo("en-US");

			this.wordIndex = new SortedList<string, List<WordRef>>();
		    this.Culture = culture;
			this.Thesaurus = new DefaultThesaurus(culture);
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

		public void Initialize() {
			string databasePath = IndexFolder;
			IndexFolder = databasePath;
			Thesaurus.DatabasePath = databasePath;
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

		public void SaveIndex() {
			string fileName = IndexFileName;

			if (File.Exists(fileName))
				File.Delete(fileName);

			StreamWriter writer = new StreamWriter(
				File.Open(fileName, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None), Encoding.Unicode);

			writer.Write(BeginFile);

			foreach (string key in wordIndex.Keys) {
				writer.Write(BeginRecord);
				writer.Write(key);
				List<WordRef> wordRecords = (List<WordRef>)wordIndex[key];
				wordRecords.Sort((Comparison<WordRef>)delegate(WordRef x, WordRef y) {
					if (x.Key == y.Key)
						return x.PhraseIndex - y.PhraseIndex;
					else
						return x.Key - y.Key; 
				});
				foreach (WordRef wref in wordRecords) {
					writer.Write(BeginField);
					writer.Write(wref.Key);
					writer.Write(FieldInfoDelimeter);
					writer.Write(wref.PhraseIndex);
					writer.Write(EndField);
				}
				writer.Write(EndRecord);
			}
			writer.Write(EndFile);
			writer.Close();
		}

		public void LoadIndex() {
			string filename = IndexFileName;
			StreamReader reader = new StreamReader(
				File.Open(filename, FileMode.Open, FileAccess.Read, FileShare.Read), Encoding.Unicode);

			string header = ReadString(reader, BeginFile.Length);
			if (header != BeginFile)
				throw new FormatException("The file header is not in the expected format!");

			string record;
			while ((record = reader.ReadLine()) != null) {
				if (record == EndFile)
					break;
				
				List<WordRef> value = ExtractWordReferences(record);
				wordIndex.Add(value[0].Word, value);
			}
			reader.Close();
		}

		private List<WordRef> ExtractWordReferences(string record) {
			// saxphone5941389259415012559415492594168125
			// ^BR      ^BF    ^FS^EF                                 ^ER
			if (!record.StartsWith(BeginRecord))
				throw new FormatException("Expected start of record!");
			string word = record.Substring(BeginRecord.Length, record.IndexOf(BeginField) - 1);

			int fieldStartIdx = record.IndexOf(BeginField);
			int fieldEndIdx = record.IndexOf(EndField);

			List<WordRef> results = new List<WordRef>();
			while (fieldStartIdx > 0 && fieldEndIdx > 0) {
				// 56166096
				string Field = record.Substring(fieldStartIdx, fieldEndIdx- fieldStartIdx);
				int rsIdx = Field.IndexOf(FieldInfoDelimeter);
				int key = int.Parse(Field.Substring(1, rsIdx - 1));
				int pos = int.Parse(Field.Substring(rsIdx + 1));
				results.Add(new WordRef(word, key, pos));

				fieldStartIdx = record.IndexOf(BeginField, fieldEndIdx);
				fieldEndIdx = record.IndexOf(EndField, (fieldStartIdx == -1 ? fieldEndIdx : fieldStartIdx));
			}
			return results;
		}

		private string ReadString(StreamReader reader, int length) {
			char[] buffer = new char[length];
			reader.Read(buffer, 0, length);
			return new string(buffer);
		}

		public List<WordRef> FindWord(string wordText) {
			return this[wordText];
		}
	}
}
