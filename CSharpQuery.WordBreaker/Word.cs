/***** CSharpQuery **** By: Nathan Zaugg **** Created: 3/9/2009 *************
 * This software is licensed under Microsoft Public License (Ms-PL)			*
 * http://www.microsoft.com/opensource/licenses.mspx						*
 *																			*
 * Downloaded From: http://www.InteractiveASP.NET							*
 ****************************************************************************/

using System;
using System.Collections.Generic;
using System.Text;

namespace CSharpQuery.WordBreaker {
	public class Word {
		public string WordText { get; set; }
		public int Index { get; set; }

		public Word(string word, int index) {
			this.WordText = word;
			this.Index = index;
		}

		public override string ToString() {
			return string.Format("{0}:{1}", Index, WordText);
		}
	}
}
