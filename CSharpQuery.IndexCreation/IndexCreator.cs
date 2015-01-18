/***** CSharpQuery **** By: Nathan Zaugg **** Created: 3/9/2009 *************
 * This software is licensed under Microsoft Public License (Ms-PL)			*
 * http://www.microsoft.com/opensource/licenses.mspx						*
 *																			*
 * Downloaded From: http://www.InteractiveASP.NET							*
 ****************************************************************************/

using System.Collections.Generic;
using CSharpQuery.Index;

namespace CSharpQuery.IndexCreation
{
	public class IndexCreator<T> : IIndexCreator<T>
	{
		private readonly ITextIndexFiller<T> textIndexFiller;

		public delegate void RowInserted(int rowNum);
		public event RowInserted OnRowInserted;

		public IndexCreator(ITextIndexFiller<T> textIndexFiller)
		{
			this.textIndexFiller = textIndexFiller;
		}

		public TextIndex<T> CreateIndex(IEnumerable<Phrase<T>> phrases)
		{

			var index = new TextIndex<T>();

			LoadPhrasesIntoTheIndex(phrases, index);

			return index;
		}

		private void LoadPhrasesIntoTheIndex(IEnumerable<Phrase<T>> phrases, TextIndex<T> index)
		{
			var row = 0;
			foreach (var phrase in phrases)
			{
				AddPhrase(index, phrase);
				row++;
				FireRowInsertedEvent(row);
			}
		}

		private void AddPhrase(TextIndex<T> index, Phrase<T> phrase)
		{
			textIndexFiller.AddPhraseToIndex(index, phrase);
		}

		private void FireRowInsertedEvent(int row)
		{
			if (OnRowInserted != null)
				OnRowInserted(row);
		}
	}
}
