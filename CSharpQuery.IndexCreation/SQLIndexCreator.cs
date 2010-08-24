/***** CSharpQuery **** By: Nathan Zaugg **** Created: 3/9/2009 *************
 * This software is licensed under Microsoft Public License (Ms-PL)			*
 * http://www.microsoft.com/opensource/licenses.mspx						*
 *																			*
 * Downloaded From: http://www.InteractiveASP.NET							*
 ****************************************************************************/

using System.Data;
using System.Globalization;
using CSharpQuery.Index;

namespace CSharpQuery.IndexCreation
{
	public class SQLIndexCreator
	{
	    private readonly string keyField;
	    private readonly string textField;

	    public delegate void RowInserted(int rowNum);

		public event RowInserted OnRowInserted;

        public SQLIndexCreator(string keyField, string textField)
        {
            this.keyField = keyField;
            this.textField = textField;
        }

	    // 1) Load Table
		// 2) Add items to index
		// 3) Save Index
		public void CreateIndex(string Name, string Directory, IDataReader reader, CultureInfo culture) {
			
			var index = CreateAnIndex(Name, culture, Directory);

            LoadPhrasesIntoTheIndex(reader, index);

		    index.SaveIndex();
		}

	    private void LoadPhrasesIntoTheIndex(IDataReader reader, TextIndex index)
	    {
	        var row = 0;
	        while (reader.Read()) {
	            AddPhrase(index, reader);
	            row++;
	            FireRowInsertedEvent(row);
	        }
	    }

	    private static TextIndex CreateAnIndex(string Name, CultureInfo culture, string Directory)
	    {
	        var index = new TextIndex(Name, culture) {IndexFolder = Directory};
	        index.Initialize();
	        return index;
	    }

	    private void AddPhrase(TextIndex index, IDataReader reader)
	    {
	        var key = (int)reader[keyField];
	        var text = (string)reader[textField];
	        index.AddPhrase(key, text);
	    }

	    private void FireRowInsertedEvent(int row)
	    {
	        if (OnRowInserted != null)
	            OnRowInserted(row);
	    }
	}
}
