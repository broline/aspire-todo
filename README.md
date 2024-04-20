# Aspire Todo App

Sandbox application for testing out .Net [Aspire](https://learn.microsoft.com/en-us/dotnet/aspire/get-started/aspire-overview)

## Running the app

Visual Studio

Set `Todo.Apphost` as the startup project and run

CLI

`dotnet run --project src\Todo.AppHost`

## Provisioning and Deployment

Install the [Azure Developer CLI](https://learn.microsoft.com/en-us/azure/developer/azure-developer-cli/install-azd?tabs=winget-windows%2Cbrew-mac%2Cscript-linux&pivots=os-windows)

> `winget install microsoft.azd` on windows

If you want to run this on your azure subscription you will have to create a new environment with `azd env new`

Run `azd up` to provision and deploy the app resources

> If this is your first time you will be asked to auth first with `azd auth login`

To deploy the resources after changes run `azd deploy`

To un-provision all of the resources run `azd down`

## Setting up your github actions or ADO pipeline

`azd pipeline config` to create a service princiapal and setup secrets

`azd infra synth` to create the infra/ files

## Generating the manifest

`dotnet run --project src\Todo.AppHost\Todo.AppHost.csproj -- --publisher manifest --output-path aspire-manifest.json`

## Migrations

> Using the package manager console

Add a migration

`dotnet tool run dotnet-ef migrations add MigrationName --project Todo.Data`