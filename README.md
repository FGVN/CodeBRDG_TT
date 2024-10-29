# CodeBRDG_TT
This .NET API provides endpoints for adding and retrieving data, allowing users to manage and view records efficiently in a structured database environment
## Requirements
- .NET 8 SDK 
- MS SQL Server 
## Installation
```bash
git clone https://github.com/FGVN/CodeBRDG_TT
cd CodeBRDG_TT
cd CodeBRDG_TT
```
If you don`t have the MsSQL database runnning - start it
Set your connection string in the appsettings.json here:
```json
{
...
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=localhost;Initial Catalog=MessengerDb;User Id=USERNAME_HERE;Password=PASSWORD_HERE;Persist Security Info=True;Pooling=False;Multiple Active Result Sets=False;Connect Timeout=60;Encrypt=True;Trust Server Certificate=True;Command Timeout=0;MultipleActiveResultSets=true"
  },
...
}
```
Then run the following command to apply migration
```
dotnet ef database update
```
After that, you can simply start the application
```
dotnet run
```
Additionally, you can set the rate limiter yourself
```json
{
 ...
  "RateLimiting": {
    "PermitLimit": 10,
    "WindowInSeconds": 1
  },
 ...
}
```
## Endpoints
### Ping - GET
Use it to check whether or not the application is running
```
/ping
```
#### Response
200 OK: Returns the string "Dogshouseservice.Version1.0.1".
### Dog - POST
Use this endpoint to register a new dog in the database.
```
/dog
```
#### Request body
```
{
  "name" : "string",
  "color" : "string",
  "tail_length" : (number >= 0),
  "weight" : (number > 0)
}
```
#### Response
201 Created: Returns the registered dog details on success.

400 Bad Request: If the registration fails, returns an error message

### Dogs - GET
Use this endpoint to get data about existing dogs.
```
/dogs
```
#### Request body
```
{
  "attribute": "name of the attribute to sort by (e.g., 'name', 'age')", // Optional
  "order": "asc or desc for sorting order", // Optional
  "pageNumber": 1, // Page number for pagination (must be >= 1), optional
  "pageSize": 10 // Number of records per page (must be > 0), optional
}
```
#### Response
200 OK: Returns a paginated list of dog records sorted according to the specified attribute and order.

400 Bad Request: Returns an error message if the request body contains invalid values or if there are issues with pagination parameters.

## Tests
To run test simply change directory to the test project (CodeBRDG_TT.Tests) and run
```
dotnet test
```
