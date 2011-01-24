using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CSharpQuery.Index;
using CSharpQuery.IndexCreation;
using CSharpQuery.WordBreaker;
using Moq;
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

            var mock = new Mock<IWordBreakingInformationRetriever>();
            mock.Setup(x => x.GetWordBreakingInformation())
                .Returns(new WordBreakingInformation{NoiseWords = new Dictionary<string, string>(),
                Substitutions = new Dictionary<string, string>(),
                Whitespace = new List<char>()});

            var indexCreator = new IndexCreator(new TextIndexFiller(new DefaultWordBreaker(mock.Object)));

            var phrases = people.Select(x => new Phrase {Key = x.Key, Text = x.FirstName + " " + x.LastName});

            var index = indexCreator.CreateIndex(phrases);

            ScenarioContext.Current.Set(index);
        }
    }
}
