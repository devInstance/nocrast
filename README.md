# Nocrast

A simple timer tracker. Visit [my blog](https://devinstance.net/blog/introduction-to-blazor) more information.

Live demo: http://nocrast.com

## Hot reload
Run `dotnet watch run` in src/Server

## Database Migrations
Run `dotnet tool restore` in src/Server folder first time.

 - To create/update database: `dotnet ef database update`
 - To add a new migration: `dotnet ef migrations add <name>`
 
 Create a deployment script:
  `dotnet ef migrations script <Last performed migrartion> > <destination file>`
Example:
  `dotnet ef migrations script ProjectAndMore > deployment.sql`

## Test code coverage
First time install: `dotnet tool install -g dotnet-reportgenerator-globaltool`

Run code coverage tool in test/Server: `dotnet test --collect:"XPlat Code Coverage"`
View report: `reportgenerator -reports:<report file name> -targetdir:<target dir>`
Example: `reportgenerator -reports:coverage.cobertura.xml -targetdir:TestResults\3220a3bc-bc80-4270-b8e6-fe03cf24f2b3`
