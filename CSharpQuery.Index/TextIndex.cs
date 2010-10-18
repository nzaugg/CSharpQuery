using System.Collections.Generic;

namespace CSharpQuery.Index
{
    public class TextIndex : SortedList<string, List<WordRef>>
    {
        public string Name { get; private set; }
        public string IndexFolder { get; private set; }

        public void Initialize(string databasePath, string name)
        {
            IndexFolder = databasePath;
            Name = name;
        }
    }
}