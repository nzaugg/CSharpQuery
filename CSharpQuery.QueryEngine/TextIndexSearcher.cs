using System.Collections.Generic;
using CSharpQuery.Index;

namespace CSharpQuery.QueryEngine
{
    public interface ITextIndexSearcher
    {
        List<WordReference> SearchTheIndex(TextIndex index, string frontString);
    }

    public class TextIndexSearcher : ITextIndexSearcher
    {
        public List<WordReference> SearchTheIndex(TextIndex index, string frontString)
        {
            var binarySearcher = new BinarySearch((str1, str2) => str2.StartsWith(str1) ? 0 : str1.CompareTo(str2));
            var words = binarySearcher.Search(index, frontString);

            var results = new List<WordReference>();
            foreach (var word in words)
                results.AddRange(index[word]);

            return results;
        }
    }
}