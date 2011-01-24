using System.Collections.Generic;
using CSharpQuery.WordBreaker;

namespace CSharpQuery.Thesaurus
{
    public interface IThesaurus
    {
        List<Synonym> Suggest(List<Word> searchWords);
    }
}