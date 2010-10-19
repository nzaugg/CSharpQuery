using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace CSharpQuery.Index
{
    public class TextIndexSaver
    {
        private TextIndexFileInformation textIndexFileInformation;

        public void Initialize(string databasePath, string name)
        {
            textIndexFileInformation = new TextIndexFileInformation();
            IndexFolder = databasePath;
            Name = name;
        }

        public string IndexFolder { get; private set; }
        public string Name { get; private set; }

        public void SaveIndex(TextIndex textIndex)
        {
            string fileName = new IndexFileNameGenerator().GetIndexFileName(Name, IndexFolder);

            if (File.Exists(fileName))
                File.Delete(fileName);

            StreamWriter writer = new StreamWriter(
                File.Open(fileName, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None), Encoding.Unicode);

            writer.Write(textIndexFileInformation.BeginFile);

            foreach (string key in textIndex.Keys)
            {
                writer.Write(textIndexFileInformation.BeginRecord);
                writer.Write(key);
                List<WordRef> wordRecords = textIndex[key];
                wordRecords.Sort((Comparison<WordRef>)delegate(WordRef x, WordRef y)
                                                          {
                                                              if (x.Key == y.Key)
                                                                  return x.PhraseIndex - y.PhraseIndex;
                                                              else
                                                                  return x.Key - y.Key;
                                                          });
                foreach (WordRef wref in wordRecords)
                {
                    writer.Write(textIndexFileInformation.BeginField);
                    writer.Write(wref.Key);
                    writer.Write(textIndexFileInformation.FieldInfoDelimeter);
                    writer.Write(wref.PhraseIndex);
                    writer.Write(textIndexFileInformation.EndField);
                }
                writer.Write(textIndexFileInformation.EndRecord);
            }
            writer.Write(textIndexFileInformation.EndFile);
            writer.Close();
        }
    }
}