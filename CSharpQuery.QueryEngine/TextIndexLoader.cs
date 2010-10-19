using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using CSharpQuery.Index;

namespace CSharpQuery.QueryEngine
{
    public class TextIndexLoader
    {
        private string indexFolder;
        private string name;
        private TextIndexFileInformation textIndexFileInformation;

        public void Initialize(string databasePath, string name)
        {
            textIndexFileInformation = new TextIndexFileInformation();
            indexFolder = databasePath;
            this.name = name;
        }

        public TextIndex LoadIndex()
        {
            var filename = new IndexFileNameGenerator().GetIndexFileName(name, indexFolder);

            var textIndex = new TextIndex();

            var reader = new StreamReader(
                File.Open(filename, FileMode.Open, FileAccess.Read, FileShare.Read), Encoding.Unicode);

            var header = ReadString(reader, textIndexFileInformation.BeginFile.Length);
            if (header != textIndexFileInformation.BeginFile)
                throw new FormatException("The file header is not in the expected format!");

            string record;
            while ((record = reader.ReadLine()) != null)
            {
                if (record == textIndexFileInformation.EndFile)
                    break;

                var value = ExtractWordReferences(record);
                textIndex.Add(value[0].Word, value);
            }
            reader.Close();

            return textIndex;
        }

        private List<WordRef> ExtractWordReferences(string record)
        {
            // saxphone5941389259415012559415492594168125
            // ^BR      ^BF    ^FS^EF                                 ^ER
            if (!record.StartsWith(textIndexFileInformation.BeginRecord))
                throw new FormatException("Expected start of record!");
            var word = record.Substring(textIndexFileInformation.BeginRecord.Length,
                                        record.IndexOf(textIndexFileInformation.BeginField) - 1);

            var fieldStartIdx = record.IndexOf(textIndexFileInformation.BeginField);
            var fieldEndIdx = record.IndexOf(textIndexFileInformation.EndField);

            var results = new List<WordRef>();
            while (fieldStartIdx > 0 && fieldEndIdx > 0)
            {
                // 56166096
                var Field = record.Substring(fieldStartIdx, fieldEndIdx - fieldStartIdx);
                var rsIdx = Field.IndexOf(textIndexFileInformation.FieldInfoDelimeter);
                var key = int.Parse(Field.Substring(1, rsIdx - 1));
                var pos = int.Parse(Field.Substring(rsIdx + 1));
                results.Add(new WordRef(word, key, pos));

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