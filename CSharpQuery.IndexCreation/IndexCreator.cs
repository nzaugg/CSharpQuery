/***** CSharpQuery **** By: Nathan Zaugg **** Created: 3/9/2009 *************
 * This software is licensed under Microsoft Public License (Ms-PL)			*
 * http://www.microsoft.com/opensource/licenses.mspx						*
 *																			*
 * Downloaded From: http://www.InteractiveASP.NET							*
 ****************************************************************************/

using System.Collections.Generic;
using System.Globalization;
using CSharpQuery.Index;

namespace CSharpQuery.IndexCreation
{
	public class IndexCreator
	{
	    private readonly CultureInfo culture;
        private TextIndexFiller textIndexFiller;
	    private string directory;

	    public delegate void RowInserted(int rowNum);

		public event RowInserted OnRowInserted;

        public IndexCreator()
        {
            culture = new CultureInfo("en-US");
            textIndexFiller = new TextIndexFiller();
        }

	    public string Directory
	    {
            set { directory = value; }
	    }

	    // 1) Load Table
		// 2) Add items to index
		// 3) Save Index
		public void CreateIndex(string name, IEnumerable<Phrase> phrases) {
			
			var index = CreateAnIndex(name);

            LoadPhrasesIntoTheIndex(phrases, index);

		    var textIndexSaver = new TextIndexSaver();
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

	    private TextIndex CreateAnIndex(string name)
	    {
	        var index = new TextIndex();
	        index.Initialize(directory, name);
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
