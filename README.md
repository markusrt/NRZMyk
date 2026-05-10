# NRZMyk

## Build and code quality

### GitHub

[![CodeQL](https://github.com/markusrt/NRZMyk/actions/workflows/codeql-analysis.yml/badge.svg?branch=master)](https://github.com/markusrt/NRZMyk/actions/workflows/codeql-analysis.yml)
[![Build and Publish Docker](https://github.com/markusrt/NRZMyk/actions/workflows/docker-build-and-publish.yml/badge.svg?branch=master)](https://github.com/markusrt/NRZMyk/actions/workflows/docker-build-and-publish.yml)

### Super-linter (current PR setup)

- Workflow: `.github/workflows/super-linter.yml`
- Scope: only changed files in PRs (`VALIDATE_ALL_CODEBASE: false`)
- Action pin: `super-linter/super-linter@9e863354e3ff62e0727d37183162c4a88873df41` (v8.6.0)
- Some validators are currently disabled to reduce baseline noise (BIOME/CHECKOV/JSCPD/TRIVY/ZIZMOR and .NET solution-format validators).

`FIX_CSS_PRETTIER` / `FIX_CSS` run fixes only inside the CI workspace. They do not
create a new Git commit automatically, so fixes are not persisted back to the PR
branch unless a separate commit/push step is added. For the current PR check these
flags are set to `false`.

### Fix linting issues locally

When Super-Linter reports a linting issue, run the same container locally from
the repo root:

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
- `FIX_MARKDOWN_PRETTIER=true` and `FIX_MARKDOWN=true` auto-fix formatting and
  Markdown rules that support fixing.
- Some checks (for example spelling suggestions from `codespell`) are not always
  auto-fixable and may require a manual edit.

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

Flag based authentication on client-side is still WIP.

### Base tools to install

```bash
dotnet tool install --global dotnet-ef
```

## Deployment setup

Make sure above mentioned secrets are also set correctly in your deployment environment. Usually this
can be done via environment variables:

- `AzureAdB2C__Domain=contoso.onmicrosoft.com`
- `AzureAdB2C\_\_ClientId=acc6f10a-484d-4e56-a0fa-1536d7b2df0b
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

## Reference to third party licenses

- Used architectural patterns based on <https://github.com/dotnet-architecture/eShopOnWeb>
  - MIT License: <https://github.com/dotnet-architecture/eShopOnWeb/blob/master/LICENSE>
  - Last checked 2020-07-25
