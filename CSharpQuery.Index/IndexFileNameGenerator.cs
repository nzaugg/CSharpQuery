using System.Globalization;
using System.IO;

namespace CSharpQuery.Index
{
    public interface IIndexFileNameGenerator
    {
        string GetIndexFileName(string name, string indexFolder);
    }

    public class IndexFileNameGenerator : IIndexFileNameGenerator
    {
        private CultureInfo cultureInfo;

        public IndexFileNameGenerator()
        {
            cultureInfo = new CultureInfo("en-US");
        }

        public string GetIndexFileName(string name, string indexFolder)
        {
            return Path.Combine(indexFolder,
                                string.Format("Index_{0}.{1}.index", name, cultureInfo == CultureInfo.InvariantCulture ? "invarient" : cultureInfo.ToString()));
        }
    }
}