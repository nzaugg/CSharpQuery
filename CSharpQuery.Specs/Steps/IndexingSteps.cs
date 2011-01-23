using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CSharpQuery.IndexCreation;
using TechTalk.SpecFlow;

namespace CSharpQuery.Specs.Steps
{
    [Binding]
    public class IndexingSteps
    {
        [When(@"I index the people")]
        public void WhenIIndexThePeople()
        {
            var people = ScenarioContext.Current.Get<IEnumerable<Person>>();

            //var indexCreator = new IndexCreator(new Mock);

            //string sql = "SELECT VerseID, VerseText FROM Verse";
            //using (SqlCeConnection conn = new SqlCeConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString))
            //{
            //    conn.Open();
            //    var rdr = new SqlCeCommand(sql, conn).ExecuteReader();

            //    var context = new TextFileAccessContext("Bible", IndexDir, new CultureInfo("en-US"));
            //    var textIndexSaver = new TextIndexSaver(context);
            //    var indexCreator = new IndexCreator(context);

            //    var index = indexCreator.CreateIndex(new BibleVersuses(rdr));

            //    textIndexSaver.SaveIndex(index);

            //    rdr.Close();
            //}
        }
    }
}
