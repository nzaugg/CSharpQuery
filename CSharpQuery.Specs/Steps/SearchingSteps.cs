using CSharpQuery.Index;
using CSharpQuery.QueryEngine;
using TechTalk.SpecFlow;

namespace CSharpQuery.Specs.Steps
{
    [Binding]
    public class SearchingSteps
    {
        [When(@"I search for '(.*)'")]
        public void x(string searchTerm)
        {
            var index = ScenarioContext.Current.Get<TextIndex>();

            var query = new FreeTextQuery(null);
        }
    }
}