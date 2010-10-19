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
    public interface IIndexCreator
    {
        string Directory { set; }
        void CreateIndex(string name, IEnumerable<Phrase> phrases);
    }

    public class IndexCreator : IIndexCreator
    {
        private ITextIndexFiller textIndexFiller;
        private ITextIndexSaver textIndexSaver;
	    private string directory;

	    public delegate void RowInserted(int rowNum);

		public event RowInserted OnRowInserted;

        public IndexCreator()
        {
            textIndexFiller = new TextIndexFiller();
            textIndexSaver = new TextIndexSaver();
        }

	    public string Directory
	    {
            set { directory = value; }
	    }

	    // 1) Load Table
		// 2) Add items to index
		// 3) Save Index
		public void CreateIndex(string name, IEnumerable<Phrase> phrases) {
			
			var index = CreateAnIndex();

            LoadPhrasesIntoTheIndex(phrases, index);

            textIndexSaver.Initialize(directory, name);
		    textIndexSaver.SaveIndex(index);
		}

	    private void LoadPhrasesIntoTheIndex(IEnumerable<Phrase> phrases, TextIndex index)
	    {
	        var row = 0;
	        foreach(var phrase in phrases)
            {
	            AddPhrase(index, phrase);
	            row++;
	            FireRowInsertedEvent(row);
	        }
	    }

	    private TextIndex CreateAnIndex()
	    {
	        var index = new TextIndex();
            textIndexFiller.Initialize(directory);
	        return index;
	    }

	    private void AddPhrase(TextIndex index, Phrase phrase)
	    {
	        textIndexFiller.AddPhraseToIndex(index, phrase.Key, phrase.Text);
	    }

	    private void FireRowInsertedEvent(int row)
	    {
	        if (OnRowInserted != null)
	            OnRowInserted(row);
	    }
	}
}
