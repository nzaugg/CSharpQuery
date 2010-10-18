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

        public List<WordRef> FrontMatch(string frontString)
        {
            var binarySearcher = new BinarySearch((str1, str2) => str2.StartsWith(str1) ? 0 : str1.CompareTo(str2));
            var words = binarySearcher.Search(wordIndex, frontString);

            var results = new List<WordRef>();
            foreach (var word in words)
                results.AddRange(this[word]);
            
            return results;
        }

        public void Initialize(string databasePath, string name)
        {
            IndexFolder = databasePath;
            Name = name;
        }



        public List<WordRef> FindWord(string wordText)
        {
            return this[wordText];
        }
    }
}