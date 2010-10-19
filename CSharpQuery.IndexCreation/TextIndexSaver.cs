using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using CSharpQuery.Index;

namespace CSharpQuery.IndexCreation
{
    public interface ITextIndexSaver
    {
        void SaveIndex(TextIndex textIndex);
    }

    public class TextIndexSaver : ITextIndexSaver
    {
        private TextIndexFileInformation textIndexFileInformation;
        private IIndexFileNameGenerator indexFileNameGenerator;
        private IndexCreationContext indexCreationContext;

        public TextIndexSaver(IndexCreationContext indexCreationContext)
        {
            this.indexCreationContext = indexCreationContext;
            indexFileNameGenerator = new IndexFileNameGenerator();
            textIndexFileInformation = new TextIndexFileInformation();
        }

        public void SaveIndex(TextIndex textIndex)
        {
            string fileName = indexFileNameGenerator.GetIndexFileName(indexCreationContext.Name, indexCreationContext.Directory);

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