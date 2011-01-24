using CSharpQuery.Index;

namespace CSharpQuery.QueryEngine
{
    public class TextIndexReader : ITextIndexReader
    {
        private readonly TextFileAccessContext textFileAccessContext;

        public TextIndexReader(TextFileAccessContext textFileAccessContext)
        {
            this.textFileAccessContext = textFileAccessContext;
        }

        public TextIndex GetTextIndex()
        {
            var indexKey = textFileAccessContext.Name + textFileAccessContext.Culture;

            if (FreeTextQuery.Indexes.Keys.Contains(indexKey))
                return FreeTextQuery.Indexes[indexKey];

            var textIndexLoader = new TextIndexLoader(textFileAccessContext);
            var index = textIndexLoader.LoadIndex();

            var lc = FreeTextQuery.readerLock.UpgradeToWriterLock(1000 * 60); // 60 seconds!
            FreeTextQuery.Indexes.Add(indexKey, index);
            FreeTextQuery.readerLock.DowngradeFromWriterLock(ref lc);
            return index;
        }
    }
}