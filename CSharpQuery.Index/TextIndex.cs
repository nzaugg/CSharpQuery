using System.Collections.Generic;
using System.Globalization;

namespace CSharpQuery.Index
{
    public class TextIndex
    {
        protected SortedList<string, List<WordRef>> wordIndex;

        public string Name { get; private set; }
        public string IndexFolder { get; private set; }

        public TextIndex()
        {
            wordIndex = new SortedList<string, List<WordRef>>();
        }

        public SortedList<string, List<WordRef>> WordIndex
        {
            get { return wordIndex; }
        }

        public List<WordRef> this[string word]
        {
            get
            {
                List<WordRef> result = null;
                wordIndex.TryGetValue(word, out result);
                return result;
            }
        }


        public void Initialize(string databasePath, string name)
        {
            IndexFolder = databasePath;
            Name = name;
        }

    }
}