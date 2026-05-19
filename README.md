# CrmSaaS

## Overview
CrmSaaS is a multi-tier .NET solution with API, BLL, and DAL projects.

## Prerequisites
- .NET SDK (net10.0)
- SQL Server (or LocalDB)

## Setup
1. Configure the connection string in API/appsettings.json under ConnectionStrings:DbConn.
2. Restore and build:
   - dotnet restore
   - dotnet build

## Database Migrations
Use the following commands from the solution root:

```bash
dotnet ef migrations add InitialCreate --project DAL --startup-project API --context CrmSaaSDbContext --output-dir EF/Migrations
dotnet ef database update --project DAL --startup-project API --context CrmSaaSDbContext
```

## Run
```bash
dotnet run --project API
```
