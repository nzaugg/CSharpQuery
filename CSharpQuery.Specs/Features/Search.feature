Feature: Search
	In order to allow someone to find things
	As the system
	I want to create a search index items use it to produce good search results

Scenario: Search for an item
	Given I have the following people
	| Key | FirstName | LastName |
	| 4   | Darren    | Cauthon  |
	When I index the people 
	And I search for 'Darren Cauthon'
	Then my search results should include
	| Key |
	| 4   |
