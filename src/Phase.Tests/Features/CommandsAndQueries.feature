Feature: CommandsAndQueries
	
Scenario: execute command that succesfully applies to aggregate and subscribers
	Given phase is occupied with tenant id "63921ebb-b2b4-44fd-b441-17e730556ac8"
	When phase executes link account command
	| Field         | Value                                |
	| AccountId     | 63921ebb-b2b4-44fd-b441-17e730556ac8 |
	| AccountNumber | 1111                                 |
	| AccountName   | BofA Checking                        |
	And phase executes get accounts query
	Then the query should return the following accounts
	| Id                                   | Number | Name          |
	| 63921ebb-b2b4-44fd-b441-17e730556ac8 | 1111   | BofA Checking |
