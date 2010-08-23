/***** CSharpQuery **** By: Nathan Zaugg **** Created: 3/9/2009 *************
 * This software is licensed under Microsoft Public License (Ms-PL)			*
 * http://www.microsoft.com/opensource/licenses.mspx						*
 *																			*
 * Downloaded From: http://www.InteractiveASP.NET							*
 ****************************************************************************/

using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Globalization;
using CSharpQuery.Index;

namespace CSharpQuery.IndexCreation
{
	public class SQLIndexCreator
	{

		public delegate void RowInserted(int rowNum);

		public event RowInserted OnRowInserted;

		// 1) Load Table
		// 2) Add items to index
		// 3) Save Index
		public void CreateIndex(string Name, string Directory, IDataReader reader, CultureInfo culture, string keyField, string textField) {
			int row = 0;
			TextIndex index = new TextIndex(Name, culture);
			index.IndexFolder = Directory;
			index.Initialize();
			while (reader.Read()) {
				int key = (int)reader[keyField];
				string text = (string)reader[textField];
				index.AddPhrase(key, text);
				row++;
				if (OnRowInserted != null)
					OnRowInserted(row);
			}
			index.SaveIndex();
		}
	}
}
