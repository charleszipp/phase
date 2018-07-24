Feature: CommandsAndQueries
	
Scenario: execute command that succesfully applies to aggregate and subscribers
	Given phase is occupied with tenant id "63921ebb-b2b4-44fd-b441-17e730556ac8"
	When phase executes create mock command
	| Field    | Value                                |
	| MockId   | 63921ebb-b2b4-44fd-b441-17e730556ac8 |
	| MockName | Mock 1                               |
	And phase executes get mock query
	Then the query should return mock name "Mock 1"
