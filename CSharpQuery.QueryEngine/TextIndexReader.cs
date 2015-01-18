using CSharpQuery.Index;

namespace CSharpQuery.QueryEngine
{
    public class TextIndexReader<T> : ITextIndexReader<T>
    {
        private readonly TextFileAccessContext textFileAccessContext;

        public TextIndexReader(TextFileAccessContext textFileAccessContext)
        {
            this.textFileAccessContext = textFileAccessContext;
        }

        public TextIndex<T> GetTextIndex()
        {
            var indexKey = textFileAccessContext.Name + textFileAccessContext.Culture;

            if (FreeTextQuery<T>.Indexes.Keys.Contains(indexKey))
                return FreeTextQuery<T>.Indexes[indexKey];

            var textIndexLoader = new TextIndexLoader<T>(textFileAccessContext);
            var index = textIndexLoader.LoadIndex();

            var lc = FreeTextQuery<T>.readerLock.UpgradeToWriterLock(1000 * 60); // 60 seconds!
            FreeTextQuery<T>.Indexes.Add(indexKey, index);
            FreeTextQuery<T>.readerLock.DowngradeFromWriterLock(ref lc);
            return index;
        }
    }
}