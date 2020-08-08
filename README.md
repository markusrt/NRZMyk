# NRZMyk

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

Configure the following environment variables for the server project:

- `AzureAdB2C__Domain`: Your ADB2C domain
- `AzureAd__TenantId`: Your tenant ID
- `AzureAdB2C__ClientId`: Your server app client ID
- `AzureAdB2C__SignUpSignInPolicyId`: Your signin policy name

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

### Deployment setup

Make sure above mentioned environment variables are also set correctly in your deployment environment

### Base tools to install

```
dotnet tool install --global dotnet-ef
```

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