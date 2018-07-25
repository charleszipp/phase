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
	When phase executes link account command
	| Field         | Value                                 |
	| AccountId     | cda49d33-00f6-45f8-99e7-bfc30f08e4a08 |
	| AccountNumber | 1111                                  |
	| AccountName   | Checking                              |
	And vacate phase
	And occupy phase with tenant id "0e0c4165-b386-45dc-a278-12bfe46f5921"
	When phase executes link account command
	| Field         | Value                                |
	| AccountId     | 505ed83d-56cf-471b-bc3f-5d862e622628 |
	| AccountNumber | 2222                                 |
	| AccountName   | Savings                              |
	And phase executes get accounts query
	Then the query should return the following accounts
	| Id                                   | Number | Name    |
	| 505ed83d-56cf-471b-bc3f-5d862e622628 | 2222   | Savings |

Scenario: reuse phase instance for different tenant then occupy for first tenant
	Given phase is occupied with tenant id "63921ebb-b2b4-44fd-b441-17e730556ac8"
	When phase executes link account command
	| Field         | Value                                 |
	| AccountId     | cda49d33-00f6-45f8-99e7-bfc30f08e4a08 |
	| AccountNumber | 1111                                  |
	| AccountName   | Checking                              |
	And vacate phase
	And occupy phase with tenant id "0e0c4165-b386-45dc-a278-12bfe46f5921"
	When phase executes link account command
	| Field         | Value                                |
	| AccountId     | 505ed83d-56cf-471b-bc3f-5d862e622628 |
	| AccountNumber | 2222                                 |
	| AccountName   | Savings                              |
	And vacate phase
	And occupy phase with tenant id "63921ebb-b2b4-44fd-b441-17e730556ac8"
	And phase executes get accounts query
	Then the query should return the following accounts
	| Id                                    | Number | Name     |
	| cda49d33-00f6-45f8-99e7-bfc30f08e4a08 | 1111   | Checking |