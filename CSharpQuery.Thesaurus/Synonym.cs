/***** CSharpQuery **** By: Nathan Zaugg **** Created: 3/9/2009 *************
 * This software is licensed under Microsoft Public License (Ms-PL)			*
 * http://www.microsoft.com/opensource/licenses.mspx						*
 *																			*
 * Downloaded From: http://www.InteractiveASP.NET							*
 ****************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSharpQuery.Thesaurus {
	public class Synonym {
		public string OriginalWord { get; set; }
		public List<string> SuggestedWords { get; set; }
		
		public Synonym() { }
		public Synonym(string originalWord, List<string> suggestedWords) {
			this.OriginalWord = originalWord;
			this.SuggestedWords = suggestedWords;
		}

		public override string ToString() {
			return string.Format("{0} -> {1}", OriginalWord, string.Join(", ", SuggestedWords.ToArray()));
		}
	}
}
