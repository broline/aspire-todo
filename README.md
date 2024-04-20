# Aspire Todo App

Sandbox application for testing out .Net [Aspire](https://learn.microsoft.com/en-us/dotnet/aspire/get-started/aspire-overview)

## Running the app

Visual Studio

Set `Todo.Apphost` as the startup project and run

CLI

`dotnet run --project src\Todo.AppHost`

## Generating the manifest

`dotnet run --project src\Todo.AppHost\Todo.AppHost.csproj -- --publisher manifest --output-path aspire-manifest.json`

## Migrations

> Using the package manager console

Add a migration

`dotnet tool run dotnet-ef migrations add MigrationName --project Todo.Data`