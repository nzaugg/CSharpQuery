using ProtoBuf;
using System.Collections.Generic;

namespace CSharpQuery.Index
{
	[ProtoContract]
	public class WordReference
	{
		[ProtoMember(1)]
		public string Word { get; set; }

		[ProtoMember(2)]
		public int Key { get; set; }

		[ProtoMember(3)]
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