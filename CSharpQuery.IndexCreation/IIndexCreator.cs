using System.Collections.Generic;
using CSharpQuery.Index;

namespace CSharpQuery.IndexCreation
{
    public interface IIndexCreator
    {
        TextIndex CreateIndex(IEnumerable<Phrase> phrases);
    }
}