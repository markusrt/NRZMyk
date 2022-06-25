# NRZMyk

## Build and code quality

### GitHub

[![CodeQL](https://github.com/markusrt/NRZMyk/actions/workflows/codeql-analysis.yml/badge.svg?branch=master)](https://github.com/markusrt/NRZMyk/actions/workflows/codeql-analysis.yml) [![Build and Publish Docker](https://github.com/markusrt/NRZMyk/actions/workflows/docker-build-and-publish.yml/badge.svg?branch=master)](https://github.com/markusrt/NRZMyk/actions/workflows/docker-build-and-publish.yml)

### Sonarcloud

[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=markusrt_NRZMyk&metric=alert_status)](https://sonarcloud.io/summary/new_code?id=markusrt_NRZMyk) [![Maintainability Rating](https://sonarcloud.io/api/project_badges/measure?project=markusrt_NRZMyk&metric=sqale_rating)](https://sonarcloud.io/summary/new_code?id=markusrt_NRZMyk) [![Coverage](https://sonarcloud.io/api/project_badges/measure?project=markusrt_NRZMyk&metric=coverage)](https://sonarcloud.io/summary/new_code?id=markusrt_NRZMyk) [![Code Smells](https://sonarcloud.io/api/project_badges/measure?project=markusrt_NRZMyk&metric=code_smells)](https://sonarcloud.io/summary/new_code?id=markusrt_NRZMyk)

## Infrastructure setup

### Database

First you need to create an empty database named, e.g. `NRZMyk` and then
a corresponding user:

```sql
-- To be executed as admin on master database
CREATE LOGIN nrzmyk WITH password='******';

-- To be executed as admin on NRZMyk database
CREATE USER nrzmyk FROM LOGIN nrzmyk;
EXEC sp_addrolemember 'db_owner', 'nrzmyk';
```

After changing the connection string in `appsettings.json` it should
be possible to apply entity framework migrations.

## Development setup

### Azure ADB2C

This app authenticates with Azure ADB2C. In order to run it locally you need
to setup client and server apps according to this documentation:

<https://docs.microsoft.com/en-us/azure/active-directory-b2c/tutorial-create-tenant>

### Server App

Configure the following secrets for the server project:

```shell
cd NRZMyk.Server
dotnet user-secrets set "AzureAdB2C:Domain" "Your ADB2C domain"
dotnet user-secrets set "AzureAdB2C:ClientId" "Your server app client ID"
dotnet user-secrets set "AzureAdB2C:SignUpSignInPolicyId" "Your signin policy name"
dotnet user-secrets set "APPINSIGHTS:INSTRUMENTATIONKEY": "Your application insights key"
dotnet user-secrets set "Application:SendGridSenderEmail": "Address used to send messages to other users"
dotnet user-secrets set "Application:SendGridDynamicTemplateId": "Sendgrid dynamic template id"
dotnet user-secrets set "Application:AdministratorEmail": "Address of admin to receive registration requests"
dotnet user-secrets set "SendGrid:ApiKey" "Sendgrid API key"
dotnet user-secrets set "AzureAdB2C:ClientSecret" "Client secret for GraphAPI access"
dotnet user-secrets set "AzureAdB2C:B2cExtensionAppClientId" "B2C extensions app client id"
```

See also `appsettings.json` in server project.

### Client App

- Configure `wwwroot/appsettings.json` with corresponding values for the client app.
- Configure `Program.cs` with corresponding API access key.

### Role support

Roles are currently supported by adding a custom attribute to the ADB2C users.

- Attribute name: `Role`
- Type: `int`
- Value: Flag enum integer representation for `Role` enum in server project

Flag based authentication on client side is still WIP.

### Base tools to install

```shell
dotnet tool install --global dotnet-ef
```

## Deployment setup

Make sure above mentioned secrets are also set correctly in your deployment environment. Usually this can be done via environment variables:

- `AzureAdB2C__Domain=contoso.onmicrosoft.com`
- `AzureAdB2C__ClientId=acc6f10a-484d-4e56-a0fa-1536d7b2df0b
- *etc...*

## Development tasks

### Entity framework migrations

List migrations in project `NRZMyk.Services` using

```shell
dotnet ef migrations --startup-project ../NRZMyk.Server/NRZMyk.Server.csproj list 
```

Add a new migration using

```shell
dotnet ef migrations --startup-project ../NRZMyk.Server/NRZMyk.Server.csproj add Entity_MigrationDetails 
```

### Clean registry

```shell
az acr run -r <yourregistry> --cmd="acr purge --ago 40d --dry-run --untagged --filter 'nrzmyk:xxxx.*'" /dev/null
```

## Reference to third party licenses

- Used architecutral patters based on <https://github.com/dotnet-architecture/eShopOnWeb>
  - MIT License: <https://github.com/dotnet-architecture/eShopOnWeb/blob/master/LICENSE>
  - Last checked 2020-07-25
