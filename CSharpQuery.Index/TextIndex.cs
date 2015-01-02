using ProtoBuf;
using System.Collections.Generic;

namespace CSharpQuery.Index
{
	[ProtoContract]
	public class TextIndex : SortedList<string, List<WordReference>>
	{
	}
}