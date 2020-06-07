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

### Azure AD

This app authenticates with Azure AD. In order to run it locally you need to configure the following environment variables:

- `AzureAd__Domain`: Your domain
- `AzureAd__TenantId`: Your tenant ID
- `AzureAd__ClientId`: Your apps client ID

## Deployment setup

### Fix Azure AD redirect issues

There are cases where RedirectUri is needed, for instance when you use a reverse proxy that transforms HTTPS
URLs (external world) to HTTP URLs (inside the protected area). This can also be useful for Web apps running
in containers (for the same reasons). I.e. in case that AAD tries to redirect to <http://your.host.name/signin-oidc>
you would need to override the following config value:

- `AzureAd__RedirectUri`: `<https://your.host.name/signin-oidc>`