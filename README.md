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

#### Azure SQL: minimum TLS version

Azure is retiring the *MinTLS None* (also shown as *No Minimum TLS*)
configuration on Azure SQL Database and SQL Managed Instance. After
**31 July 2026** all Azure SQL servers will require a minimum TLS
version of **1.2** for client connections, and unencrypted connections
will be rejected. Servers that are still configured with
`MinTLS = None` will be auto-updated to TLS 1.2.

This application is unaffected at the code level: it uses
`Microsoft.EntityFrameworkCore.SqlServer` (which depends on
`Microsoft.Data.SqlClient` 5.x), and that driver supports TLS 1.2/1.3
out of the box and defaults `Encrypt=True` for all connections.

To stay on the safe side, please make sure that:

1. The production connection string used in the deployment environment
   uses an encrypted connection. With recent SqlClient versions this is
   the default, but it can be made explicit, e.g.:

   ```
   Server=tcp:<your-server>.database.windows.net,1433;Database=NRZMyk;User Id=nrzmyk;Password=******;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;
   ```

2. On the Azure side, set the **Minimum TLS Version** of the SQL server
   (or Managed Instance) to **1.2** (or higher). For Azure SQL
   Database this is configured under
   *SQL server → Networking → Connectivity → Minimum TLS Version*.
   See the official documentation for step-by-step instructions:

   - [Azure SQL Database – configure minimum TLS version](https://learn.microsoft.com/azure/azure-sql/database/connectivity-settings#configure-minimum-tls-version)
   - [Azure SQL Managed Instance – configure minimum TLS version](https://learn.microsoft.com/azure/azure-sql/managed-instance/minimal-tls-version-configure)

No application restart or redeployment is required after raising the
minimum TLS version on the server, as long as the connection string
already allows encrypted connections (which is the default).

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
dotnet user-secrets set "APPINSIGHTS:INSTRUMENTATIONKEY" "Your application insights key"
dotnet user-secrets set "Application:SendGridSenderEmail" "Address used to send messages to other users"
dotnet user-secrets set "Application:SendGridDynamicTemplateId" "Sendgrid dynamic template id"
dotnet user-secrets set "Application:SendGridRemindOrganizationOnDispatchMonthTemplateId" "Sendgrid dynamic template id"
dotnet user-secrets set "Application:AdministratorEmail" "Address of admin to receive registration requests"
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

```
dotnet tool install --global dotnet-ef
```

## Deployment setup

Make sure above mentioned secrets are also set correctly in your deployment environment. Usually this 
can be done via environment variables:

- `AzureAdB2C__Domain=contoso.onmicrosoft.com`
- `AzureAdB2C__ClientId=acc6f10a-484d-4e56-a0fa-1536d7b2df0b
- *etc...*

## Development tasks

### Entity framework migrations

List migrations in project `NRZMyk.Services` using

```
dotnet ef migrations --startup-project ../NRZMyk.Server/NRZMyk.Server.csproj list 
```

Add a new migration using

```
dotnet ef migrations --startup-project ../NRZMyk.Server/NRZMyk.Server.csproj add Entity_MigrationDetails 
```


## Reference to third party licenses

- Used architecutral patters based on <https://github.com/dotnet-architecture/eShopOnWeb>
  - MIT License: https://github.com/dotnet-architecture/eShopOnWeb/blob/master/LICENSE
  - Last checked 2020-07-25