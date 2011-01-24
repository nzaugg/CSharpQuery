using System.Collections.Generic;

namespace CSharpQuery.Thesaurus
{
    public interface IThesaurusDictionaryRetriever
    {
        SortedList<string, SortedList<string, int>> GetThesaurus();
    }
}