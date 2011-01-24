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
using CSharpQuery.Index;
using System.ComponentModel;
using CSharpQuery.Thesaurus;

namespace CSharpQuery.QueryEngine {
	public class QueryResult {
		public int Key { get; set; }
		public decimal Rank { get; set; }

		public decimal searchTermsProximity { get; set; }
		public decimal wordMatching  { get; set; }
		public decimal lowPhraseIndex  { get; set; }
		public decimal multipleOccurance { get; set; }

		public List<WordReference> WordIndexes { get; set; }
	}
}
