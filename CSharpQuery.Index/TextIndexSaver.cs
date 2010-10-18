using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace CSharpQuery.Index
{
    public class TextIndexSaver
    {
        private static string BeginFile = (char)01 + "CSharpQuery_Index" + (char)01;
        private static string BeginRecord = (char)02 + "";
        private static string BeginField = (char)03 + "";
        private static string FieldInfoDelimeter = (char)04 + "";
        private static string EndField = (char)05 + "";
        private static string EndRecord = (char)06 + "\r\n";
        private static string EndFile = (char)07 + "";
        
        public void Initialize(string databasePath, string name) {
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

            writer.Write(BeginFile);

            foreach (string key in textIndex.Keys)
            {
                writer.Write(BeginRecord);
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
    }
}