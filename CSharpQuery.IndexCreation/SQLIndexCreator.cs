/***** CSharpQuery **** By: Nathan Zaugg **** Created: 3/9/2009 *************
 * This software is licensed under Microsoft Public License (Ms-PL)			*
 * http://www.microsoft.com/opensource/licenses.mspx						*
 *																			*
 * Downloaded From: http://www.InteractiveASP.NET							*
 ****************************************************************************/

using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using CSharpQuery.Index;

namespace CSharpQuery.IndexCreation
{
	public class SQLIndexCreator
	{
	    private readonly string keyField;
	    private readonly string textField;
	    private readonly CultureInfo culture;
	    private readonly string directory;

	    public delegate void RowInserted(int rowNum);

		public event RowInserted OnRowInserted;

        public SQLIndexCreator(string keyField, string textField, 
            CultureInfo culture, string directory)
        {
            this.keyField = keyField;
            this.textField = textField;
            this.culture = culture;
            this.directory = directory;
        }

	    // 1) Load Table
		// 2) Add items to index
		// 3) Save Index
		public void CreateIndex(string Name, IEnumerable<Phrase> phrases) {
			
			var index = CreateAnIndex(Name);

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

	    private TextIndex CreateAnIndex(string Name)
	    {
	        var index = new TextIndex(Name, culture) {IndexFolder = directory};
	        index.Initialize();
	        return index;
	    }

	    private void AddPhrase(TextIndex index, Phrase phrase)
	    {
            //var key = (int)reader[keyField];
            //var text = (string)reader[textField];
	        index.AddPhrase(phrase.Key, phrase.Text);
	    }

	    private void FireRowInsertedEvent(int row)
	    {
	        if (OnRowInserted != null)
	            OnRowInserted(row);
	    }
	}
}
