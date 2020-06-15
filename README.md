# nocrast

A simple timer tracker. Visit [my blog](https://devinstance.net/blog/introduction-to-blazor) more informartion.

Live demo: http://nocrast.com

## Database Migrations
Run `dotnet tool restore` in src/Server folder first time.

 - To create/update database: `dotnet ef update database`
 - To add a new migration: `dotnet ef migrations add <name>`
 