using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TestApp
{
	public class IndexBuilder
	{
		public IndexInfo Index { get; set; }
		public Book CurrentBook { get; set; }
		public Verse CurrentVerse { get; set; }

		public IndexBuilder() {
			Index = new IndexInfo();
		}

		public void ProcessLine(string line)
		{
			if (string.IsNullOrEmpty(line.Trim()))
				return;

			// Look for Book
			if (line.Trim().StartsWith("Book"))
			{
				int bookInt = line.IndexOf("Book");
				int bookIntEnd = line.IndexOf(" ", bookInt);
				int bookNum = int.Parse(line.Substring(bookIntEnd+1, 2));
				string bookName = line.Substring(bookIntEnd+3).Trim();
				CurrentBook = new Book();
				CurrentBook.BookNumber = bookNum;
				CurrentBook.Name = bookName;
				Index.Books.Add(CurrentBook);
				return;
			}

			// Check for verse heading
			int chapter;
			if (int.TryParse(line.Substring(0, 3), out chapter))
			{
				int verse = int.Parse(line.Substring(4, 3));
				string verseStart = line.Substring(8).Trim();
				CurrentVerse = new Verse();
				CurrentVerse.Chapter = chapter;
				CurrentVerse.VerseNum = verse;
				CurrentVerse.Text = verseStart;
				CurrentBook.Verses.Add(CurrentVerse);
				return;
			}

			if (CurrentVerse != null)
				CurrentVerse.Text += " " + line.Trim();
		}
	}

	public class IndexInfo
	{
		public List<Book> Books { get; set; }

		public IndexInfo()
		{
			Books = new List<Book>();
		}
	}

	public class Book 
	{
		public string Name { get; set; }
		public int BookNumber { get; set; }
		public List<Verse> Verses { get; set; }

		public Book()
		{
			Verses = new List<Verse>();
		}
	}

	public class Verse 
	{
		public int Chapter { get; set; }
		public int VerseNum { get; set; }
		public string Text { get; set; }

		public override string ToString() 
		{
			return string.Format("{0}:{1} {2}", Chapter, VerseNum, Text);
		}
	}
}

