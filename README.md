# EmployeeSchedule
Backend Code challenge.

To create a database and endpoints that enable a user to:

  	Add update delete and get details of an employee
		Get: 	/api/employees
		Get 	/api/employees/5
		Put: 	/api/employees/5
		Post: 	/api/employees
		Delete:	/api/employees/5
	
  	Add update delete and get details of shifts - for all shifts or for a specific employee
		Get: 	/api/shifts
		Get:	/api/shifts/5     --This is employeeID instead of shiftID to get all the shifts for a specific employee
		Put: 	/api/shifts/5
		Post: 	/api/shifts
		Delete:	/api/shifts/5
	
  	Swap shifts, an endpoint where the user can swap shifts given two shift IDs
		Put: 	/api/shifts/5/6
  
Limitations:

  	All employees must have a first name last name and unique email address
	
  	An employee can only work one shift at a time
