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

        public IndexFileNameGenerator(TextFileAccessContext textFileAccessContext)
        {
            this.textFileAccessContext = textFileAccessContext;
        }

        public string GetIndexFileName()
        {
            return Path.Combine(textFileAccessContext.Directory,
                                string.Format("Index_{0}.{1}.index", textFileAccessContext.Name, textFileAccessContext.Culture == CultureInfo.InvariantCulture ? "invarient" : textFileAccessContext.Culture.ToString()));
        }
    }
}