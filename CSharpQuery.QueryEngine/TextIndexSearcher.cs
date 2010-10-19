using System.Collections.Generic;
using CSharpQuery.Index;

namespace CSharpQuery.QueryEngine
{
    public class TextIndexSearcher
    {
        public List<WordRef> FrontMatch(TextIndex index, string frontString)
        {
            var binarySearcher = new BinarySearch((str1, str2) => str2.StartsWith(str1) ? 0 : str1.CompareTo(str2));
            var words = binarySearcher.Search(index, frontString);

            var results = new List<WordRef>();
            foreach (var word in words)
                results.AddRange(index[word]);

            return results;
        }

        public List<WordRef> FindWord(TextIndex index, string wordText)
        {
            return index[wordText];
        }
    }
}