# Migration Plan: Azure AD B2C to Microsoft Entra External ID

## Overview

This document provides a detailed, step-by-step migration plan for transitioning the NRZMyk application from Azure AD B2C to Microsoft Entra External ID. The plan is designed to minimize downtime and ensure a smooth transition.

## Prerequisites

- [ ] Complete analysis document review
- [ ] Stakeholder approval for migration timeline
- [ ] Development team training on Entra External ID
- [ ] Access to Azure portal with appropriate permissions
- [ ] Backup of current B2C configuration and user data

## Migration Timeline

**Total Estimated Duration**: 8-10 weeks
**Recommended Start Date**: April 2025
**Target Completion**: June 2025 (9 months before deadline)

## Phase 1: Preparation and Setup (Week 1-3)

### Task 1.1: Create Entra External ID Development Tenant
**Assignee**: Azure Administrator  
**Duration**: 2 days  
**Prerequisites**: Azure subscription with Entra External ID licensing

**Steps:**
1. Navigate to Azure portal → Microsoft Entra External ID
2. Create new tenant: `nrzmyk-dev-external.onmicrosoft.com`
3. Configure tenant settings and branding
4. Set up billing and licensing
5. Document tenant configuration

**Deliverables:**
- Development tenant URL and configuration
- Tenant administrator accounts
- Basic tenant configuration documentation

### Task 1.2: Application Registration in Entra External ID
**Assignee**: Azure Administrator + Lead Developer  
**Duration**: 3 days  
**Prerequisites**: Entra External ID tenant created

**Steps:**
1. **Register Server Application (API)**
   - Name: `NRZMyk-Server-Dev`
   - Application type: Web API
   - Configure API scopes: `API.Access`
   - Set up application roles if needed
   - Generate client secret

2. **Register Client Application (SPA)**
   - Name: `NRZMyk-Client-Dev`
   - Application type: Single Page Application
   - Set redirect URIs: `https://localhost:5001/authentication/login-callback`
   - Configure API permissions to server app
   - Enable ID tokens and access tokens

**Deliverables:**
- Application IDs and configurations
- API scope definitions
- Redirect URI configurations

### Task 1.3: User Flow Configuration
**Assignee**: Azure Administrator  
**Duration**: 2 days  
**Prerequisites**: Applications registered

**Steps:**
1. **Create Sign-up/Sign-in User Flow**
   - Name: `B2C_1A_SignupSignin`
   - Configure identity providers (local accounts)
   - Set up custom attributes for role storage
   - Configure token claims

2. **Create Password Reset User Flow**
   - Name: `B2C_1A_PasswordReset`
   - Configure reset flow settings
   - Set up email templates

**Deliverables:**
- User flow configurations
- Custom attribute definitions
- Token claim mappings

### Task 1.4: Custom Attributes Setup
**Assignee**: Lead Developer + Azure Administrator  
**Duration**: 2 days  
**Prerequisites**: User flows created

**Steps:**
1. Define custom attribute schema for roles
2. Create Graph API application with appropriate permissions
3. Test attribute creation and retrieval via Graph API
4. Document attribute naming conventions

**Deliverables:**
- Custom attribute schema
- Graph API application setup
- Attribute access testing results

## Phase 2: Code Migration (Week 4-5)

### Task 2.1: Update Server Configuration
**Assignee**: Backend Developer  
**Duration**: 2 days  
**Prerequisites**: Entra External ID tenant configured

**Steps:**
1. Update `appsettings.json` configuration
2. Update `AzureAdB2C.cs` configuration class
3. Test JWT token validation with new issuer
4. Verify Graph API connectivity

**Configuration Changes:**
```json
{
  "AzureAdB2C": {
    "Instance": "https://nrzmyk-dev-external.ciamlogin.com",
    "ClientId": "[New Client ID]",
    "B2cExtensionAppClientId": "[New Extension App Client ID]",
    "Domain": "nrzmyk-dev-external.onmicrosoft.com",
    "SignUpSignInPolicyId": "B2C_1A_SignupSignin",
    "ClientSecret": "[New Client Secret]"
  }
}
```

**Deliverables:**
- Updated server configuration
- JWT validation testing results
- Graph API connectivity verification

### Task 2.2: Update Client Configuration
**Assignee**: Frontend Developer  
**Duration**: 1 day  
**Prerequisites**: Server configuration updated

**Steps:**
1. Update client `appsettings.json`
2. Update MSAL configuration in `Program.cs`
3. Test client-side authentication flow
4. Verify token acquisition and API calls

**Configuration Changes:**
```json
{
  "AzureAdB2C": {
    "Authority": "https://nrzmyk-dev-external.ciamlogin.com/nrzmyk-dev-external.onmicrosoft.com/B2C_1A_SignupSignin",
    "ClientId": "[New Client ID]",
    "ValidateAuthority": false,
    "ResetPasswordFlow": "https://nrzmyk-dev-external.ciamlogin.com/nrzmyk-dev-external.onmicrosoft.com/B2C_1A_PasswordReset"
  }
}
```

**Deliverables:**
- Updated client configuration
- Authentication flow testing results
- API connectivity verification

### Task 2.3: Update UserService Implementation
**Assignee**: Backend Developer  
**Duration**: 2 days  
**Prerequisites**: Graph API setup completed

**Steps:**
1. Update Graph API endpoints if needed
2. Test custom attribute queries
3. Verify role retrieval logic
4. Update error handling for new API responses

**Code Changes:**
- Update `UserService.cs` attribute name construction
- Test Graph API queries with new tenant
- Verify role parsing and assignment logic

**Deliverables:**
- Updated UserService implementation
- Role retrieval testing results
- Error handling verification

### Task 2.4: Security Updates
**Assignee**: Backend Developer  
**Duration**: 1 day  
**Prerequisites**: Code migration completed

**Steps:**
1. Upgrade Microsoft.Identity.Web to latest version
2. Update other authentication-related packages
3. Address any breaking changes
4. Run security vulnerability scans

**Deliverables:**
- Updated package references
- Security scan results
- Breaking change resolution documentation

## Phase 3: Testing and Validation (Week 6-7)

### Task 3.1: Unit and Integration Testing
**Assignee**: QA Engineer + Developers  
**Duration**: 3 days  
**Prerequisites**: Code migration completed

**Test Scenarios:**
1. User authentication (sign-up, sign-in, sign-out)
2. Password reset functionality
3. Role-based authorization
4. API access with JWT tokens
5. Graph API role queries
6. Error handling and fallback scenarios

**Deliverables:**
- Test execution results
- Bug reports and fixes
- Test coverage metrics

### Task 3.2: End-to-End Testing
**Assignee**: QA Engineer  
**Duration**: 2 days  
**Prerequisites**: Unit testing completed

**Test Scenarios:**
1. Complete user journey (registration to data access)
2. Role assignment and permission verification
3. Cross-browser compatibility
4. Mobile device testing
5. Performance testing with realistic load

**Deliverables:**
- E2E test results
- Performance metrics
- Compatibility matrix

### Task 3.3: Security Testing
**Assignee**: Security Specialist  
**Duration**: 2 days  
**Prerequisites**: Functional testing completed

**Test Areas:**
1. Token validation and expiry
2. Authorization bypass attempts
3. Custom attribute access security
4. API endpoint security
5. OWASP security checklist

**Deliverables:**
- Security test results
- Vulnerability assessment
- Security recommendations

## Phase 4: Staging Migration (Week 8)

### Task 4.1: Staging Environment Setup
**Assignee**: DevOps Engineer  
**Duration**: 1 day  
**Prerequisites**: Development testing completed

**Steps:**
1. Create staging Entra External ID tenant
2. Configure applications and user flows
3. Deploy updated code to staging
4. Configure monitoring and logging

**Deliverables:**
- Staging environment setup
- Deployment scripts
- Monitoring configuration

### Task 4.2: User Acceptance Testing
**Assignee**: Business Users + QA  
**Duration**: 2 days  
**Prerequisites**: Staging environment ready

**Test Scenarios:**
1. Business workflow testing
2. User experience validation
3. Performance acceptance
4. Integration with external systems

**Deliverables:**
- UAT results
- User feedback
- Performance benchmarks

### Task 4.3: Load Testing
**Assignee**: Performance Engineer  
**Duration**: 1 day  
**Prerequisites**: UAT completed

**Test Scenarios:**
1. Concurrent user authentication
2. API throughput testing
3. Database performance under load
4. Graph API rate limiting

**Deliverables:**
- Load test results
- Performance bottleneck analysis
- Scalability recommendations

## Phase 5: Production Migration (Week 9)

### Task 5.1: Production Environment Preparation
**Assignee**: DevOps Engineer + Azure Administrator  
**Duration**: 1 day  
**Prerequisites**: Staging validation completed

**Steps:**
1. Create production Entra External ID tenant
2. Configure production applications
3. Set up production user flows and attributes
4. Configure production monitoring

**Deliverables:**
- Production tenant configuration
- Application registrations
- Monitoring setup

### Task 5.2: User Data Migration
**Assignee**: Azure Administrator + Data Engineer  
**Duration**: 1 day  
**Prerequisites**: Production environment ready

**Steps:**
1. Export users from existing B2C tenant
2. Transform user data for Entra External ID
3. Import users to new tenant
4. Migrate custom attributes (roles)
5. Validate data migration

**Deliverables:**
- User migration scripts
- Data validation results
- Migration rollback procedures

### Task 5.3: Production Deployment
**Assignee**: DevOps Engineer  
**Duration**: 0.5 days  
**Prerequisites**: User migration completed

**Steps:**
1. Schedule maintenance window
2. Deploy updated application code
3. Switch DNS/configuration to new tenant
4. Monitor system health
5. Verify user authentication

**Deliverables:**
- Deployment execution log
- System health monitoring
- User authentication verification

### Task 5.4: Go-Live Verification
**Assignee**: All Team Members  
**Duration**: 0.5 days  
**Prerequisites**: Production deployment completed

**Steps:**
1. Smoke testing of critical functions
2. User authentication verification
3. API functionality testing
4. Monitor error logs and metrics
5. User communication and support

**Deliverables:**
- Go-live checklist completion
- System health report
- User communication materials

## Phase 6: Post-Migration (Week 10)

### Task 6.1: Monitoring and Support
**Assignee**: Support Team + DevOps  
**Duration**: 5 days  
**Prerequisites**: Production deployment successful

**Activities:**
1. 24/7 monitoring for authentication issues
2. User support for login problems
3. Performance monitoring and optimization
4. Bug fix deployments if needed

**Deliverables:**
- Support metrics
- Issue resolution reports
- Performance optimization results

### Task 6.2: Documentation Updates
**Assignee**: Technical Writer + Developers  
**Duration**: 2 days  
**Prerequisites**: System stabilized

**Updates:**
1. Development setup documentation
2. Deployment procedures
3. Troubleshooting guides
4. User documentation

**Deliverables:**
- Updated documentation
- Knowledge base articles
- Training materials

### Task 6.3: Cleanup and Decommission
**Assignee**: Azure Administrator  
**Duration**: 1 day  
**Prerequisites**: Migration validated successful

**Steps:**
1. Archive B2C tenant configuration
2. Schedule B2C tenant decommission
3. Update monitoring and alerts
4. Clean up temporary resources

**Deliverables:**
- Decommission plan
- Resource cleanup report
- Cost optimization analysis

## Risk Mitigation Strategies

### Rollback Procedures
1. **Configuration Rollback**: Restore previous application configuration
2. **DNS Rollback**: Switch back to B2C tenant
3. **Data Restore**: Restore user data if migration fails
4. **Communication Plan**: User notification procedures

### Monitoring and Alerts
1. **Authentication Failure Rates**: Alert on high failure rates
2. **API Error Rates**: Monitor Graph API errors
3. **Performance Degradation**: Alert on slow response times
4. **User Complaints**: Social media and support ticket monitoring

### Contingency Plans
1. **Extended Maintenance Window**: Additional time if needed
2. **Expert Support**: Microsoft Premier Support engagement
3. **Phased Rollout**: Gradual user migration if issues arise
4. **Communication Templates**: Pre-written user communications

## Success Criteria

### Technical Criteria
- [ ] All users can successfully authenticate
- [ ] Role-based authorization functions correctly
- [ ] API performance meets SLA requirements
- [ ] No security vulnerabilities introduced
- [ ] Zero data loss during migration

### Business Criteria
- [ ] User satisfaction maintained
- [ ] No business workflow disruptions
- [ ] Support ticket volume within normal range
- [ ] Migration completed within budget
- [ ] Timeline objectives met

## Contact Information

| Role | Name | Email | Phone |
|------|------|-------|-------|
| Project Manager | [TBD] | [TBD] | [TBD] |
| Lead Developer | [TBD] | [TBD] | [TBD] |
| Azure Administrator | [TBD] | [TBD] | [TBD] |
| QA Lead | [TBD] | [TBD] | [TBD] |

## Appendix

### A. Configuration Templates
[Detailed configuration templates for each environment]

### B. Testing Checklists
[Comprehensive testing checklists for each phase]

### C. Troubleshooting Guide
[Common issues and resolution procedures]

### D. Communication Templates
[User communication templates for each phase]