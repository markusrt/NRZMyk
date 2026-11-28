# NRZMyk

## Build and code quality

### GitHub

[![CodeQL](https://github.com/markusrt/NRZMyk/actions/workflows/codeql-analysis.yml/badge.svg?branch=master)](https://github.com/markusrt/NRZMyk/actions/workflows/codeql-analysis.yml)
[![Build and Publish Docker](https://github.com/markusrt/NRZMyk/actions/workflows/docker-build-and-publish.yml/badge.svg?branch=master)](https://github.com/markusrt/NRZMyk/actions/workflows/docker-build-and-publish.yml)
[![Lint](https://github.com/markusrt/NRZMyk/actions/workflows/super-linter.yml/badge.svg)](https://github.com/markusrt/NRZMyk/actions/workflows/super-linter.yml)

### Sonarcloud

[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=markusrt_NRZMyk&metric=alert_status)](https://sonarcloud.io/summary/new_code?id=markusrt_NRZMyk)
[![Maintainability Rating](https://sonarcloud.io/api/project_badges/measure?project=markusrt_NRZMyk&metric=sqale_rating)](https://sonarcloud.io/summary/new_code?id=markusrt_NRZMyk)
[![Coverage](https://sonarcloud.io/api/project_badges/measure?project=markusrt_NRZMyk&metric=coverage)](https://sonarcloud.io/summary/new_code?id=markusrt_NRZMyk)
[![Code Smells](https://sonarcloud.io/api/project_badges/measure?project=markusrt_NRZMyk&metric=code_smells)](https://sonarcloud.io/summary/new_code?id=markusrt_NRZMyk)

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

Azure is retiring the _MinTLS None_ (also shown as _No Minimum TLS_)
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

   ```text
   Server=tcp:<your-server>.database.windows.net,1433;Database=NRZMyk;User Id=nrzmyk;Password=******;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;
   ```

2. On the Azure side, set the **Minimum TLS Version** of the SQL server
   (or Managed Instance) to **1.2** (or higher). For Azure SQL
   Database this is configured under
   _SQL server → Security → Networking → Connectivity → Minimum TLS Version_.
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
dotnet user-secrets set "AutoMapper:LicenseKey" "AutoMapper License Key"
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

Flag based authentication on client-side is still WIP.

### Base tools to install

```bash
dotnet tool install --global dotnet-ef
```

## Deployment setup

Make sure above mentioned secrets are also set correctly in your deployment environment. Usually this
can be done via environment variables:

- `AzureAdB2C__Domain=contoso.onmicrosoft.com`
- `AzureAdB2C__ClientId=acc6f10a-484d-4e56-a0fa-1536d7b2df0b`
- _etc..._

## Development tasks

### Entity framework migrations

List migrations in project `NRZMyk.Services` using

```bash
dotnet ef migrations --startup-project ../NRZMyk.Server/NRZMyk.Server.csproj list
```

Add a new migration using

```bash
dotnet ef migrations --startup-project ../NRZMyk.Server/NRZMyk.Server.csproj add Entity_MigrationDetails
```

### Super-linter

- Workflow: `.github/workflows/super-linter.yml`
- Scope: only changed files in PRs (`VALIDATE_ALL_CODEBASE: false`)
- Some validators are currently disabled to reduce baseline noise (BIOME/CHECKOV/JSCPD/TRIVY/ZIZMOR and .NET solution-format validators).

`FIX_CSS_PRETTIER` / `FIX_CSS` run fixes only inside the CI workspace. They do not
create a new Git commit automatically, so fixes are not persisted back to the PR
branch unless a separate commit/push step is added. For the current PR check these
flags are set to `false`.

### Fix linting issues locally

When Super-Linter reports a linting issue, run the same container locally from
the repository root:

```bash
docker run --rm \
  -e RUN_LOCAL=true \
  -e DEFAULT_WORKSPACE=/tmp/lint \
  -e VALIDATE_ALL_CODEBASE=false \
  -e FIX_MARKDOWN_PRETTIER=true \
  -e FIX_MARKDOWN=true \
  -v "$PWD":/tmp/lint \
  ghcr.io/super-linter/super-linter:v8.6.0
```

Then review and verify:

```bash
git --no-pager diff -- README.md
dotnet test NRZMyk.sln -v minimal --no-restore
```

Notes:

- This runs the same Super-Linter image as CI and applies available fixes directly
  to your local files via the bind mount.
- `FIX_MARKDOWN_PRETTIER=true` and `FIX_MARKDOWN=true` autofix formatting and
  Markdown rules that support fixing.
- Some checks (for example spelling suggestions from `codespell`) are not always
  auto-fixable and may require a manual edit.

### Browser tests

`NRZMyk.BrowserTests` contains Gherkin scenarios (German, [Reqnroll](https://reqnroll.net/)) that drive the
app in Chromium with [Playwright](https://playwright.dev/dotnet/), including the real B2C sign-in. They run
locally only: the project is not part of `NRZMyk.sln`, so `dotnet test NRZMyk.sln` and CI skip it.

The app runs as a container built from `NRZMyk.Server/Dockerfile` on `http://localhost:8080`, next to a
SQL Server container. In the environments `BrowserTests` and `Development`, the client uses a separate
SPA app registration for local use (`NRZMyk.Client/wwwroot/appsettings.{Environment}.json`), so the
production registration needs no localhost redirect URI. That registration lists
`http://localhost:8080/authentication/login-callback`. Users of the tenant can sign in locally with their
normal accounts as well. The container reads the Azure values from the user secrets of the server project (see
[Server App](#server-app)), mounted read-only. Docker Compose overrides the email key, the reminder job
and Application Insights, so a test run sends no emails and no telemetry.

Before every scenario the test database is reset to fixed test data, so scenarios do not depend on each
other: three organizations, the accounts of the four test users and sentinel entries for search, paging
and the cryo view (`NRZMyk.BrowserTests/Support/TestDatabase.cs`). Clinical breakpoints come from the
app's own seeding. Each role signs in through the real sign-in page once per run; later scenarios of that
role reuse the session.

**One-time setup in Azure** (B2C tenant `nrcmycosis`, switch to it with "Switch directory" in the portal)

1. User flow `B2C_1_SigninAndSignup_2`: check that **Role** is ticked under _Application claims_ and that
   MFA is off. The data type of `Role` is `int`.
2. Four test users (local accounts with email sign-in, not real people). The portal's "Create new user"
   creates internal users that cannot sign in through B2C; create them in
   [Graph Explorer](https://developer.microsoft.com/graph/graph-explorer?tenant=nrcmycosis.onmicrosoft.com)
   instead, signed in as a B2C admin. `<ID>` is `AzureAdB2C:B2cExtensionAppClientId` from the server's user
   secrets without dashes. `extension_<ID>_Role` is only set for SuperUser (`4`) and Admin (`8`); Guest and
   User get no role, the User role comes from the organization in the test data.

   ```http
   POST https://graph.microsoft.com/v1.0/users
   {
     "displayName": "Test Admin",
     "accountEnabled": true,
     "identities": [
       { "signInType": "emailAddress", "issuer": "nrcmycosis.onmicrosoft.com", "issuerAssignedId": "<email of the test user>" }
     ],
     "passwordProfile": { "password": "<password>", "forceChangePasswordNextSignIn": false },
     "passwordPolicies": "DisablePasswordExpiration",
     "extension_<ID>_Role": 8
   }
   ```

   The `id` in the response is the object ID needed below.

3. Separate SPA app registration for local use (client ID in
   `NRZMyk.Client/wwwroot/appsettings.BrowserTests.json` and `appsettings.Development.json`):
   - Account types "Any Entra ID Tenant + Personal Microsoft accounts" (the B2C option for user flows)
   - Platform _Single-page application_, redirect URI `http://localhost:8080/authentication/login-callback`
     plus the URIs used when starting the app from the IDE, e.g. `https://localhost:5001/authentication/login-callback`
   - API permissions: `openid`, `offline_access` and `API.Access` of the API registration (App ID URI ending
     in `/nrzmyk-client`), then _Grant admin consent_
4. Microsoft Graph for the role column of the admin page: on the server registration ("NRZMyk Server 5.0"),
   create a client secret for local use under _Certificates & secrets_ and check that the application
   permission `User.Read.All` is granted. Without a valid secret, the admin page silently shows "Guest" for
   everyone.

**One-time setup on the machine** (PowerShell, from the repository root)

```powershell
# Azure values of the app container: user secrets of the server project (see "Server App")
dotnet user-secrets set "AzureAdB2C:ClientSecret" (Read-Host "Graph client secret") --project NRZMyk.Server

# Test users of the browser tests
foreach ($role in 'Guest', 'User', 'SuperUser', 'Admin') {
  dotnet user-secrets set "BrowserTests:Users:${role}:Email" (Read-Host "$role email") --project NRZMyk.BrowserTests
  dotnet user-secrets set "BrowserTests:Users:${role}:Password" (Read-Host "$role password") --project NRZMyk.BrowserTests
  dotnet user-secrets set "BrowserTests:Users:${role}:ObjectId" (Read-Host "$role object ID") --project NRZMyk.BrowserTests
}

# Build once and install the browser
cd NRZMyk.BrowserTests
dotnet build
pwsh bin/Debug/net8.0/playwright.ps1 install chromium
```

Then start the environment (see below), open `http://localhost:8080` and sign in once with each test user
by hand. Each must land in the app without an extra page (password change, consent).

**Run**

```bash
cd NRZMyk.BrowserTests
docker compose up -d --build
dotnet test
docker compose down   # frees port 8080 again
```

Run a single role with `dotnet test --filter Category=admin` (tags: `anonym`, `guest`, `user`, `superuser`,
`admin`). For a visible browser, set `BrowserTests__Headed=true`. For a failed scenario, a screenshot and
a Playwright trace (open with `pwsh bin/Debug/net8.0/playwright.ps1 show-trace <path>/trace.zip`) are kept
in `NRZMyk.BrowserTests/TestResults`. The server's user secrets are found via `%APPDATA%` on Windows; on
other systems set `SERVER_USER_SECRETS` to the path of its `secrets.json`.

While the environment is running, `http://localhost:8080` can also be used to try the app with login
manually. Changes made there are discarded by the next test run.

## Reference to third party licenses

- Used architectural patterns based on <https://github.com/dotnet-architecture/eShopOnWeb>
  - MIT License: <https://github.com/dotnet-architecture/eShopOnWeb/blob/master/LICENSE>
  - Last checked 2020-07-25
