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

            var writer = new StreamWriter(
                File.Open(fileName, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None), Encoding.Unicode);

            writer.Write(textIndexFileInformation.BeginFile);

            foreach (var key in textIndex.Keys)
            {
                writer.Write(textIndexFileInformation.BeginRecord);
                writer.Write(key);
                var wordRecords = textIndex[key];
                wordRecords.Sort(delegate(WordReference x, WordReference y)
                                     {
                                         if (x.Key == y.Key)
                                             return x.PhraseIndex - y.PhraseIndex;
                                         else
                                             return x.Key - y.Key;
                                     });
                foreach (var wref in wordRecords)
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