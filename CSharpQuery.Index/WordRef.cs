/***** CSharpQuery **** By: Nathan Zaugg **** Created: 3/9/2009 *************
 * This software is licensed under Microsoft Public License (Ms-PL)			*
 * http://www.microsoft.com/opensource/licenses.mspx						*
 *																			*
 * Downloaded From: http://www.InteractiveASP.NET							*
 ****************************************************************************/

using System.Collections.Generic;

namespace CSharpQuery.Index {
	public class WordRef {
		public string Word { get; set; }
		public int Key { get; set; }
		public int PhraseIndex { get; set; }

		public WordRef(string word, int key, int phraseIndex) {
			Word = word;
			Key = key;
			PhraseIndex = phraseIndex;
		}

		public override bool Equals(object obj) {
			if (obj == null)
				return false;
			if ( obj is WordRef )
				return ((WordRef)obj).Key == this.Key;
			return false;
		}

		public override int GetHashCode() {
			return Key + PhraseIndex;
		}

		public override string ToString() {
			return string.Format("{0} -> {1}", Word, Key);
		}
	}

	public class WordRefEqualityComparer : IEqualityComparer<WordRef> {
		#region IEqualityComparer<WordRef> Members

		public bool Equals(WordRef x, WordRef y) {
			return x.Equals(y);
		}

		public int GetHashCode(WordRef obj) {
			return obj.Key;
		}

		#endregion
	}
}
