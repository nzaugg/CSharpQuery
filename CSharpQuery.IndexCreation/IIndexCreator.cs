using System.Collections.Generic;
using CSharpQuery.Index;

namespace CSharpQuery.IndexCreation
{
    public interface IIndexCreator<T>
    {
        TextIndex<T> CreateIndex(IEnumerable<Phrase<T>> phrases);
    }
}