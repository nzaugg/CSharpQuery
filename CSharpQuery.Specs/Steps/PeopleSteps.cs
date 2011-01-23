using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;

namespace CSharpQuery.Specs.Steps
{
    [Binding]
    public class PeopleSteps
    {
        [Given(@"I have the following people")]
        public void GivenIHaveTheFollowingPeople(Table table)
        {
            var people = table.CreateSet<Person>();
            ScenarioContext.Current.Set(people);
        }
    }

    public class Person
    {
        public int Key { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}