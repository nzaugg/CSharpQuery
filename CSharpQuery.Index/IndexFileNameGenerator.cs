using System.Globalization;
using System.IO;

namespace CSharpQuery.Index
{
    public interface IIndexFileNameGenerator
    {
        string GetIndexFileName();
    }

    public class IndexFileNameGenerator : IIndexFileNameGenerator
    {
        private readonly TextFileAccessContext textFileAccessContext;
        private CultureInfo cultureInfo;

        public IndexFileNameGenerator(TextFileAccessContext textFileAccessContext)
        {
            this.textFileAccessContext = textFileAccessContext;
            cultureInfo = new CultureInfo("en-US");
        }

        public string GetIndexFileName()
        {
            return Path.Combine(textFileAccessContext.Directory,
                                string.Format("Index_{0}.{1}.index", textFileAccessContext.Name, cultureInfo == CultureInfo.InvariantCulture ? "invarient" : cultureInfo.ToString()));
        }
    }
}