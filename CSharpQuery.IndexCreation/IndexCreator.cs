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
	    private readonly string directory;

	    public delegate void RowInserted(int rowNum);

		public event RowInserted OnRowInserted;

        public IndexCreator(string directory)
        {
            culture = new CultureInfo("en-US");
            this.directory = directory;
        }

	    // 1) Load Table
		// 2) Add items to index
		// 3) Save Index
		public void CreateIndex(string name, IEnumerable<Phrase> phrases) {
			
			var index = CreateAnIndex(name);

            LoadPhrasesIntoTheIndex(phrases, index);

		    index.SaveIndex();
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
	        var index = new TextIndex(name) {IndexFolder = directory};
	        index.Initialize();
	        return index;
	    }

	    private static void AddPhrase(TextIndex index, Phrase phrase)
	    {
	        index.AddPhrase(phrase.Key, phrase.Text);
	    }

	    private void FireRowInsertedEvent(int row)
	    {
	        if (OnRowInserted != null)
	            OnRowInserted(row);
	    }
	}
}
