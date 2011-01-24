using CSharpQuery.Index;

namespace CSharpQuery.QueryEngine
{
    public interface ITextIndexReader
    {
        TextIndex GetTextIndex();
    }
}