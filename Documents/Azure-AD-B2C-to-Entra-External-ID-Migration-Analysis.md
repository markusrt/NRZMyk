# Azure AD B2C to Microsoft Entra External ID Migration Analysis

## Executive Summary

This document provides a comprehensive analysis of the current Azure AD B2C implementation in the NRZMyk project and outlines the requirements, risks, and migration plan for transitioning to Microsoft Entra External ID.

**Critical Timeline**: Microsoft stopped selling Azure AD B2C on May 1, 2025, with existing P2 tenants retiring on March 15, 2026. SLAs and security updates continue until May 2030.

## Current Azure AD B2C Implementation

### 1. Architecture Overview

The NRZMyk project is a Blazor Server application with WebAssembly client that uses Azure AD B2C for external user authentication. The system implements custom role-based authorization using B2C extension attributes.

**Key Components:**
- **Server (NRZMyk.Server)**: ASP.NET Core API with JWT Bearer authentication
- **Client (NRZMyk.Client)**: Blazor WebAssembly with MSAL authentication
- **Services (NRZMyk.Services)**: Business logic with Microsoft Graph integration

### 2. Authentication Libraries and Dependencies

| Component | Library | Version | Purpose |
|-----------|---------|---------|---------|
| Server | Microsoft.Identity.Web | 3.6.0 | JWT Bearer token validation |
| Server | Microsoft.Graph | 4.11.0 | User management and role queries |
| Server | Azure.Identity | 1.11.4 | Azure authentication |
| Client | Microsoft.Authentication.WebAssembly.Msal | 8.0.12 | Client-side authentication |

**⚠️ Security Alert**: Microsoft.Identity.Web 3.6.0 has a known moderate severity vulnerability (GHSA-rpq8-q44m-2rpg).

### 3. Current Configuration Structure

#### Server Configuration (`appsettings.json`)
```json
{
  "AzureAdB2C": {
    "Instance": "https://nrcmycosis.b2clogin.com",
    "ClientId": "[Server App Client ID]",
    "B2cExtensionAppClientId": "[Extension App Client ID]",
    "Domain": "[tenant.onmicrosoft.com]",
    "SignUpSignInPolicyId": "[B2C_1_SigninPolicy]",
    "ClientSecret": "[Secret for Graph API access]"
  }
}
```

#### Client Configuration (`wwwroot/appsettings.json`)
```json
{
  "AzureAdB2C": {
    "Authority": "https://nrcmycosis.b2clogin.com/nrcmycosis.onmicrosoft.com/B2C_1_SigninAndSignup_2",
    "ClientId": "2962979b-837e-47cf-aa7b-9dfd0acfa750",
    "ValidateAuthority": false,
    "ResetPasswordFlow": "https://nrcmycosis.b2clogin.com/nrcmycosis.onmicrosoft.com/oauth2/v2.0/authorize?p=B2C_1_forgotPassword"
  }
}
```

### 4. Custom Role Management Implementation

**Role Storage**: Uses B2C extension attributes with custom schema
- Attribute Name: `extension_{B2cExtensionAppClientId}_Role`
- Storage Format: Integer representation of enum values
- Supported Roles: Guest, User, Admin (flag-based enum)

**Role Retrieval**: `UserService.cs` implementation
- Queries Microsoft Graph API: `/users/{userId}?$select=id,displayName,{RoleAttributeName}`
- Fallback to Guest role on API failures
- Custom claims transformation in JWT token validation

### 5. User Flows and Policies

**Current B2C User Flows:**
- Sign-up/Sign-in: `B2C_1_SigninAndSignup_2`
- Password Reset: `B2C_1_forgotPassword`
- Authority Domain: `nrcmycosis.b2clogin.com`

## Migration to Microsoft Entra External ID

### 1. Key Differences and Changes Required

#### Authentication Endpoints
| Current (B2C) | Target (Entra External ID) |
|---------------|----------------------------|
| `https://nrcmycosis.b2clogin.com` | `https://tenant.ciamlogin.com` |
| B2C user flows | Entra External ID user flows |
| B2C extension attributes | Entra External ID custom attributes |

#### Library Compatibility
- **Microsoft.Identity.Web**: ✅ Compatible (requires configuration updates)
- **Microsoft.Authentication.WebAssembly.Msal**: ✅ Compatible (requires configuration updates)
- **Microsoft.Graph**: ✅ Compatible (may require endpoint/scope updates)

### 2. Migration Requirements

#### Azure Portal Configuration Tasks
1. **Create Entra External ID Tenant**
   - Set up new tenant with appropriate licensing
   - Configure domain and branding

2. **Application Registration**
   - Register server application (API)
   - Register client application (SPA)
   - Configure redirect URIs and scopes
   - Set up application roles/permissions

3. **User Flow Configuration**
   - Create sign-up/sign-in user flow
   - Configure password reset flow
   - Set up custom attributes for role storage

4. **Custom Attributes Setup**
   - Define role attribute schema
   - Configure attribute collection in user flows
   - Set up Graph API permissions for attribute access

#### Code Migration Tasks
1. **Configuration Updates**
   - Update server `appsettings.json` with new endpoints
   - Update client `appsettings.json` with new authority
   - Update Graph API endpoints if needed

2. **Authentication Flow Updates**
   - Verify JWT token validation with new issuer
   - Test custom claims transformation
   - Update MSAL configuration parameters

3. **User Service Updates**
   - Update Graph API queries for custom attributes
   - Verify role retrieval logic
   - Test fallback scenarios

#### Data Migration Tasks
1. **User Account Migration**
   - Export existing B2C users
   - Import users to Entra External ID
   - Preserve user roles and custom attributes

2. **Role Data Migration**
   - Verify role attribute schema compatibility
   - Migrate existing role assignments
   - Test role-based authorization

### 3. Risk Assessment

#### High-Risk Areas
1. **Service Interruption**
   - **Risk**: Authentication failures during migration
   - **Impact**: Complete service unavailability
   - **Mitigation**: Blue-green deployment strategy

2. **Data Loss**
   - **Risk**: User accounts or roles not migrated correctly
   - **Impact**: Users locked out, permission issues
   - **Mitigation**: Comprehensive backup and validation procedures

3. **Custom Attribute Schema Changes**
   - **Risk**: Role storage mechanism incompatibility
   - **Impact**: Authorization system failure
   - **Mitigation**: Thorough testing of attribute APIs

#### Medium-Risk Areas
1. **API Breaking Changes**
   - **Risk**: Microsoft Graph API differences
   - **Impact**: Role queries failing
   - **Mitigation**: API compatibility testing

2. **Configuration Complexity**
   - **Risk**: Incorrect endpoint/scope configuration
   - **Impact**: Authentication failures
   - **Mitigation**: Step-by-step configuration validation

3. **User Experience Changes**
   - **Risk**: Different authentication flows
   - **Impact**: User confusion, support requests
   - **Mitigation**: User communication and documentation

#### Low-Risk Areas
1. **Library Updates**
   - **Risk**: Package compatibility issues
   - **Impact**: Build failures
   - **Mitigation**: Dependency analysis and testing

## Migration Plan Phases

### Phase 1: Preparation and Planning (2-3 weeks)
- [ ] Set up separate Entra External ID development tenant
- [ ] Create detailed configuration documentation
- [ ] Develop migration scripts and procedures
- [ ] Set up monitoring and rollback procedures

### Phase 2: Development Environment Migration (1-2 weeks)
- [ ] Configure dev environment with Entra External ID
- [ ] Update application code and configuration
- [ ] Perform comprehensive testing
- [ ] Validate role-based authorization

### Phase 3: Staging Environment Migration (1 week)
- [ ] Deploy changes to staging environment
- [ ] Perform user acceptance testing
- [ ] Load testing with realistic user scenarios
- [ ] Final validation of all features

### Phase 4: Production Migration (1 week)
- [ ] Schedule maintenance window
- [ ] Execute production migration
- [ ] Monitor system health and user authentication
- [ ] Address any immediate issues

### Phase 5: Post-Migration Validation (1 week)
- [ ] Comprehensive system testing
- [ ] User feedback collection
- [ ] Performance monitoring
- [ ] Documentation updates

## Recommendations

### Immediate Actions (Next 30 Days)
1. **Security Update**: Upgrade Microsoft.Identity.Web to address vulnerability
2. **Development Tenant Setup**: Create Entra External ID development environment
3. **Migration Planning**: Finalize detailed migration timeline

### Medium-Term Actions (Next 90 Days)
1. **Code Migration**: Complete development and staging migrations
2. **Testing**: Comprehensive testing of all authentication scenarios
3. **User Communication**: Prepare user documentation and communication plan

### Critical Deadlines
- **Migration Completion**: Must be completed before March 15, 2026
- **Recommended Timeline**: Complete by December 2025 to allow buffer time

## Cost Considerations

### Entra External ID Pricing
- **Free Tier**: Up to 50,000 monthly active users
- **Premium Features**: Additional costs for advanced features
- **Migration Costs**: Development time and potential consultancy fees

### Risk of Delay
- **Limited Time**: Only ~14 months until P2 tenant retirement
- **Increasing Complexity**: Later migration may face additional challenges
- **Support Limitations**: Reduced Microsoft support as retirement approaches

## Conclusion

The migration from Azure AD B2C to Microsoft Entra External ID is feasible but requires careful planning and execution. The current implementation uses standard Microsoft Identity libraries, which should facilitate the migration. However, the custom role management system and specific configuration requirements need thorough testing.

**Key Success Factors:**
1. Early start on migration planning
2. Comprehensive testing in non-production environments
3. Careful data migration procedures
4. Clear rollback plans
5. User communication and support

**Recommendation**: Begin migration preparation immediately with a target completion date of Q4 2025 to ensure adequate time for testing and issue resolution.