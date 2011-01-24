Feature: Search
	In order to allow someone to find things
	As the system
	I want to create a search index items use it to produce good search results

Scenario Outline: Good search
	Given I have the following people
	| Key | FirstName | LastName |
	| 4   | Darren    | Cauthon  |
	When I index the people 
	And I search for '<search term>'
	Then my search results should include
	| Key |
	| 4   |

Examples:
	| search term    |
	| darren         |
	| DARREN         |
	| Darren         |
	| Cauthon        |
	| Darren Cauthon |

Scenario Outline: Bad search
	Given I have the following people
	| Key | FirstName | LastName |
	| 4   | Darren    | Cauthon  |
	When I index the people 
	And I search for '<search term>'
	Then I should get no search results

Examples:
	| x          |
	| 1234       |
	| lskdflksjf |
	| Evan       |
