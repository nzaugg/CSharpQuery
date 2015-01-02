using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using CSharpQuery.Index;
using ProtoBuf;
using System.Diagnostics;

namespace CSharpQuery.QueryEngine
{
	public class TextIndexLoader
	{
		private TextIndexFileInformation textIndexFileInformation;
		private IIndexFileNameGenerator indexFileNameGenerator;

		public TextIndexLoader(TextFileAccessContext textFileAccessContext)
		{
			textIndexFileInformation = new TextIndexFileInformation();
			indexFileNameGenerator = new IndexFileNameGenerator(textFileAccessContext);
		}

		public TextIndex LoadIndex()
		{
			var filename = indexFileNameGenerator.GetIndexFileName();

			using (var fs = File.Open(filename, FileMode.Open, FileAccess.Read, FileShare.Read))
			{
				var sw = Stopwatch.StartNew();
				var index = Serializer.Deserialize<TextIndex>(fs);
				sw.Stop();
				Trace.WriteLine("Load Index completed in: " + sw.Elapsed);
				return index;
			}
		}

		private List<WordReference> ExtractWordReferences(string record)
		{
			// saxphone5941389259415012559415492594168125
			// ^BR      ^BF    ^FS^EF                                 ^ER
			if (!record.StartsWith(textIndexFileInformation.BeginRecord))
				throw new FormatException("Expected start of record!");
			var word = record.Substring(textIndexFileInformation.BeginRecord.Length,
										record.IndexOf(textIndexFileInformation.BeginField) - 1);

			var fieldStartIdx = record.IndexOf(textIndexFileInformation.BeginField);
			var fieldEndIdx = record.IndexOf(textIndexFileInformation.EndField);

			var results = new List<WordReference>();
			while (fieldStartIdx > 0 && fieldEndIdx > 0)
			{
				// 56166096
				var Field = record.Substring(fieldStartIdx, fieldEndIdx - fieldStartIdx);
				var rsIdx = Field.IndexOf(textIndexFileInformation.FieldInfoDelimeter);
				var key = int.Parse(Field.Substring(1, rsIdx - 1));
				var pos = int.Parse(Field.Substring(rsIdx + 1));
				results.Add(new WordReference {Word = word, Key = key, PhraseIndex = pos});

				fieldStartIdx = record.IndexOf(textIndexFileInformation.BeginField, fieldEndIdx);
				fieldEndIdx = record.IndexOf(textIndexFileInformation.EndField,
											 (fieldStartIdx == -1 ? fieldEndIdx : fieldStartIdx));
			}
			return results;
		}

		private string ReadString(StreamReader reader, int length)
		{
			var buffer = new char[length];
			reader.Read(buffer, 0, length);
			return new string(buffer);
		}
	}
}