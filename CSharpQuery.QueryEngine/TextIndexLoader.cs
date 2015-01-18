using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using CSharpQuery.Index;
using ProtoBuf;
using System.Diagnostics;

namespace CSharpQuery.QueryEngine
{
	public class TextIndexLoader<T>
	{
		private TextIndexFileInformation textIndexFileInformation;
		private IIndexFileNameGenerator indexFileNameGenerator;

		public TextIndexLoader(TextFileAccessContext textFileAccessContext)
		{
			textIndexFileInformation = new TextIndexFileInformation();
			indexFileNameGenerator = new IndexFileNameGenerator(textFileAccessContext);
		}

		public TextIndex<T> LoadIndex()
		{
			var filename = indexFileNameGenerator.GetIndexFileName();

			using (var fs = File.Open(filename, FileMode.Open, FileAccess.Read, FileShare.Read))
			{
				var sw = Stopwatch.StartNew();
				var index = Serializer.Deserialize<TextIndex<T>>(fs);
				sw.Stop();
				Trace.WriteLine("Load Index completed in: " + sw.Elapsed);
				return index;
			}
		}

		private string ReadString(StreamReader reader, int length)
		{
			var buffer = new char[length];
			reader.Read(buffer, 0, length);
			return new string(buffer);
		}
	}
}