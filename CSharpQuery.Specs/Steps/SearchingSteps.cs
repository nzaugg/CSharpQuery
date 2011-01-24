using System.Collections.Generic;
using System.Linq;
using CSharpQuery.Index;
using CSharpQuery.QueryEngine;
using CSharpQuery.Thesaurus;
using CSharpQuery.WordBreaker;
using Moq;
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

            var reader = new Mock<ITextIndexReader>();
            reader.Setup(x => x.GetTextIndex())
                .Returns(index);

            var thesaurus = new Mock<IThesaurus>();
            thesaurus.Setup(x => x.Suggest(It.IsAny<List<Word>>()))
                .Returns((List<Word> words) =>
                             {
                                 return words.Select(x=>new Synonym{OriginalWord = x.WordText, SuggestedWords = new List<string>()})
                                     .ToList();
                             });

            var wordBreakingInformationRetriever = new Mock<IWordBreakingInformationRetriever>();
            wordBreakingInformationRetriever.Setup(x => x.GetWordBreakingInformation())
                .Returns(new WordBreakingInformation{NoiseWords = new Dictionary<string, string>(),
                Substitutions = new Dictionary<string, string>(),
                Whitespace = new List<char>()});

            var query = new FreeTextQuery(reader.Object, new DefaultWordBreaker(wordBreakingInformationRetriever.Object), thesaurus.Object);
            var results = query.SearchFreeTextQuery(searchTerm);

            ScenarioContext.Current.Set(results.Select(x=>x));
        }

        [Then(@"my search results should include")]
        public void ThenMySearchResultsShouldInclude(Table table)
        {
            var results = ScenarioContext.Current.Get<IEnumerable<QueryResult>>();

            table.CompareToSet(results);
        }
    }
}