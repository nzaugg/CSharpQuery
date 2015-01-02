﻿using System.IO;
using System.Text;
using CSharpQuery.Index;
using ProtoBuf;
using System.Diagnostics;
using System;

namespace CSharpQuery.IndexCreation
{
	public interface ITextIndexSaver
	{
		void SaveIndex(TextIndex textIndex);
	}

	public class TextIndexSaver : ITextIndexSaver
	{
		private readonly TextIndexFileInformation textIndexFileInformation;
		private readonly IIndexFileNameGenerator indexFileNameGenerator;

		public TextIndexSaver(TextFileAccessContext textFileAccessContext)
		{
			indexFileNameGenerator = new IndexFileNameGenerator(textFileAccessContext);
			textIndexFileInformation = new TextIndexFileInformation();
		}

		public void SaveIndex(TextIndex textIndex)
		{
			var fileName = indexFileNameGenerator.GetIndexFileName();

			if (File.Exists(fileName))
				File.Delete(fileName);

			using (var fs = File.Open(fileName, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None))
			{
				var sw = Stopwatch.StartNew();
				Serializer.Serialize(fs, textIndex);
				sw.Stop();
				Trace.WriteLine("Index saved in: " + sw.Elapsed);
			}
		}
	}
}