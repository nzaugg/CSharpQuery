using ProtoBuf;
using System.Collections.Generic;

namespace CSharpQuery.Index
{
	[ProtoContract]
	public class TextIndex<T> : SortedList<string, List<WordReference<T>>>
	{
	}
}