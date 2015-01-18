using ProtoBuf;
using System.Collections.Generic;

namespace CSharpQuery.Index
{
	[ProtoContract]
	public class WordReference<T>
	{
		[ProtoMember(1)]
		public string Word { get; set; }

		[ProtoMember(2)]
		public T Key { get; set; }

		[ProtoMember(3)]
		public int PhraseIndex { get; set; }

		public override bool Equals(object obj)
		{
			if (obj == null)
				return false;
			if (obj is WordReference<T>)
				return ((WordReference<T>) obj).Key.Equals(Key);
			return false;
		}

		public override int GetHashCode()
		{
			return Key.GetHashCode() + PhraseIndex;
		}

		public override string ToString()
		{
			return string.Format("{0} -> {1}", Word, Key);
		}
	}

	public class WordRefEqualityComparer<T> : IEqualityComparer<WordReference<T>>
	{
		public bool Equals(WordReference<T> x, WordReference<T> y)
		{
			return x.Equals(y);
		}

		public int GetHashCode(WordReference<T> obj)
		{
			return obj.GetHashCode();
		}
	}
}