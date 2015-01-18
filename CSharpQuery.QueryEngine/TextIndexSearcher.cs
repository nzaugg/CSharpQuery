using System.Collections.Generic;
using CSharpQuery.Index;

namespace CSharpQuery.QueryEngine
{
    public interface ITextIndexSearcher<T>
    {
        List<WordReference<T>> SearchTheIndex(TextIndex<T> index, string frontString);
    }

    public class TextIndexSearcher<T> : ITextIndexSearcher<T>
    {
        public List<WordReference<T>> SearchTheIndex(TextIndex<T> index, string frontString)
        {
            var binarySearcher = new BinarySearch<T>((str1, str2) => str2.StartsWith(str1) ? 0 : str1.CompareTo(str2));
            var words = binarySearcher.Search(index, frontString);

            var results = new List<WordReference<T>>();
            foreach (var word in words)
                results.AddRange(index[word]);

            return results;
        }
    }
}