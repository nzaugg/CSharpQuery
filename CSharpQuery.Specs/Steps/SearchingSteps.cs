using System.Collections.Generic;
using System.Linq;
using CSharpQuery.Index;
using CSharpQuery.QueryEngine;
using CSharpQuery.Thesaurus;
using CSharpQuery.WordBreaker;
using Moq;
using Should;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;

namespace CSharpQuery.Specs.Steps
{
    [Binding]
    public class SearchingSteps
    {
        [When(@"I search for '(.*)'")]
        public void x(string searchTerm)
        {
            var index = ScenarioContext.Current.Get<TextIndex>();

            var mock = new Mock<IThesaurusDictionaryRetriever>();
            mock.Setup(x => x.GetThesaurus())
                .Returns(new SortedList<string, SortedList<string, int>>());
            var thesaurus = new DefaultThesaurus(mock.Object);

            var wordBreakingInformationRetriever = new Mock<IWordBreakingInformationRetriever>();
            wordBreakingInformationRetriever.Setup(x => x.GetWordBreakingInformation())
                .Returns(new WordBreakingInformation{NoiseWords = new Dictionary<string, string>(),
                Substitutions = new Dictionary<string, string>(),
                Whitespace = new List<char>()});

            var query = new FreeTextQuery(new DefaultWordBreaker(wordBreakingInformationRetriever.Object), thesaurus, new WordRefEqualityComparer(), new TextIndexSearcher());
            var results = query.SearchFreeTextQuery(index, searchTerm);

            ScenarioContext.Current.Set(results.Select(x=>x));
        }

        [Then(@"my search results should include")]
        public void ThenMySearchResultsShouldInclude(Table table)
        {
            var results = ScenarioContext.Current.Get<IEnumerable<QueryResult>>();

            table.CompareToSet(results);
        }

        [Then(@"I should get no search results")]
        public void ThenIShouldGetNoSearchResults()
        {
            var results = ScenarioContext.Current.Get<IEnumerable<QueryResult>>();

            results.Count().ShouldEqual(0);
        }
    }
}