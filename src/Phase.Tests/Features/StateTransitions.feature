Feature: StateTransitions

@CatchException
Scenario: execute command without result against a vacant phase
	Given phase is vacant
	When executing a command without result
	Then an exception should be thrown with message "Phase must be occupied before executing commands and queries"

@CatchException
Scenario: execute query against a vacant phase
	Given phase is vacant
	When executing a query
	Then an exception should be thrown with message "Phase must be occupied before executing commands and queries"

@CatchException
Scenario: vacate a vacant phase
	Given phase is vacant
	When vacate phase
	Then an exception should be thrown with message "Phase is already vacant"

Scenario: occupy vacant phase
	Given phase is vacant
	When occupy phase with tenant id "63921ebb-b2b4-44fd-b441-17e730556ac8"
	Then phase should be occupied with tenant id "63921ebb-b2b4-44fd-b441-17e730556ac8"

@CatchException
Scenario: occupy occupied phase
	Given phase is occupied with tenant id "63921ebb-b2b4-44fd-b441-17e730556ac8"
	When occupy phase with tenant id "63921ebb-b2b4-44fd-b441-17e730556ac8"
	Then an exception should be thrown with message "Phase is already occupied"

Scenario: vacate occupied phase
	Given phase is occupied with tenant id "63921ebb-b2b4-44fd-b441-17e730556ac8"
	When vacate phase
	Then phase should be vacant

Scenario: resuse phase instance for different tenant 
	Given phase is occupied with tenant id "63921ebb-b2b4-44fd-b441-17e730556ac8"
	When phase executes create mock command
	| Field    | Value                                |
	| MockId   | 63921ebb-b2b4-44fd-b441-17e730556ac8 |
	| MockName | Mock 1                               |
	And vacate phase
	And occupy phase with tenant id "0e0c4165-b386-45dc-a278-12bfe46f5921"
	And phase executes create mock command
	| Field    | Value                                |
	| MockId   | 0e0c4165-b386-45dc-a278-12bfe46f5921 |
	| MockName | Mock 2                               |
	And phase executes get mock query
	Then the query should return mock name "Mock 2"

Scenario: reuse phase instance for different tenant then occupy for first tenant
	Given phase is occupied with tenant id "63921ebb-b2b4-44fd-b441-17e730556ac8"
	When phase executes create mock command
	| Field    | Value                                |
	| MockId   | 63921ebb-b2b4-44fd-b441-17e730556ac8 |
	| MockName | Mock 1                               |
	And vacate phase
	And occupy phase with tenant id "0e0c4165-b386-45dc-a278-12bfe46f5921"
	And phase executes create mock command
	| Field    | Value                                |
	| MockId   | 0e0c4165-b386-45dc-a278-12bfe46f5921 |
	| MockName | Mock 2                               |
	And vacate phase
	And occupy phase with tenant id "63921ebb-b2b4-44fd-b441-17e730556ac8"
	And phase executes get mock query
	Then the query should return mock name "Mock 1"