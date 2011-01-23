using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TechTalk.SpecFlow;

namespace CSharpQuery.Specs.Steps
{
    [Binding]
    public class IndexingSteps
    {
        [When(@"I index the people")]
        public void WhenIIndexThePeople()
        {
            ScenarioContext.Current.Pending();
        }
    }
}
