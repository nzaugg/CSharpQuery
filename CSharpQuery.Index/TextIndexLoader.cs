using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace CSharpQuery.Index
{
    public class TextIndexLoader
    {
        private static string BeginFile = (char)01 + "CSharpQuery_Index" + (char)01;
        private static string BeginRecord = (char)02 + "";
        private static string BeginField = (char)03 + "";
        private static string FieldInfoDelimeter = (char)04 + "";
        private static string EndField = (char)05 + "";
        private static string EndRecord = (char)06 + "\r\n";
        private static string EndFile = (char)07 + "";

        private string indexFolder;
        private string name;

        public void Initialize(string databasePath, string name)
        {
            indexFolder = databasePath;
            this.name = name;
        }

        public TextIndex LoadIndex()
        {
            string filename = new IndexFileNameGenerator().GetIndexFileName(name, indexFolder);

            var textIndex = new TextIndex();
            textIndex.Initialize(indexFolder, name);

            StreamReader reader = new StreamReader(
                File.Open(filename, FileMode.Open, FileAccess.Read, FileShare.Read), Encoding.Unicode);

            string header = ReadString(reader, BeginFile.Length);
            if (header != BeginFile)
                throw new FormatException("The file header is not in the expected format!");

            string record;
            while ((record = reader.ReadLine()) != null)
            {
                if (record == EndFile)
                    break;

                List<WordRef> value = ExtractWordReferences(record);
                textIndex.Add(value[0].Word, value);
            }
            reader.Close();

            return textIndex;
        }

        private List<WordRef> ExtractWordReferences(string record)
        {
            // saxphone5941389259415012559415492594168125
            // ^BR      ^BF    ^FS^EF                                 ^ER
            if (!record.StartsWith(BeginRecord))
                throw new FormatException("Expected start of record!");
            string word = record.Substring(BeginRecord.Length, record.IndexOf(BeginField) - 1);

            int fieldStartIdx = record.IndexOf(BeginField);
            int fieldEndIdx = record.IndexOf(EndField);

            List<WordRef> results = new List<WordRef>();
            while (fieldStartIdx > 0 && fieldEndIdx > 0)
            {
                // 56166096
                string Field = record.Substring(fieldStartIdx, fieldEndIdx - fieldStartIdx);
                int rsIdx = Field.IndexOf(FieldInfoDelimeter);
                int key = int.Parse(Field.Substring(1, rsIdx - 1));
                int pos = int.Parse(Field.Substring(rsIdx + 1));
                results.Add(new WordRef(word, key, pos));

                fieldStartIdx = record.IndexOf(BeginField, fieldEndIdx);
                fieldEndIdx = record.IndexOf(EndField, (fieldStartIdx == -1 ? fieldEndIdx : fieldStartIdx));
            }
            return results;
        }

        private string ReadString(StreamReader reader, int length)
        {
            char[] buffer = new char[length];
            reader.Read(buffer, 0, length);
            return new string(buffer);
        }
    }
}