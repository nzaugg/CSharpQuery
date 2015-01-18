using CSharpQuery.Index;

namespace CSharpQuery.QueryEngine
{
    public interface ITextIndexReader<T>
    {
        TextIndex<T> GetTextIndex();
    }
}