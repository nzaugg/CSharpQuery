using System.Collections.Generic;

namespace CSharpQuery.Index
{
    public class WordReference
    {
        public string Word { get; set; }
        public int Key { get; set; }
        public int PhraseIndex { get; set; }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            if (obj is WordReference)
                return ((WordReference) obj).Key == Key;
            return false;
        }

        public override int GetHashCode()
        {
            return Key + PhraseIndex;
        }

        public override string ToString()
        {
            return string.Format("{0} -> {1}", Word, Key);
        }
    }

    public class WordRefEqualityComparer : IEqualityComparer<WordReference>
    {
        public bool Equals(WordReference x, WordReference y)
        {
            return x.Equals(y);
        }

        public int GetHashCode(WordReference obj)
        {
            return obj.Key;
        }
    }
}