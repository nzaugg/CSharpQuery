using CSharpQuery.Index;

namespace CSharpQuery.IndexCreation
{
    public interface ITextIndexFiller
    {
        void AddPhraseToIndex(TextIndex index, Phrase phrase);
    }
}