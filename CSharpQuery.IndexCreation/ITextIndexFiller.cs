using CSharpQuery.Index;

namespace CSharpQuery.IndexCreation
{
    public interface ITextIndexFiller<T>
    {
        void AddPhraseToIndex(TextIndex<T> index, Phrase<T> phrase);
    }
}