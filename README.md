# CrmSaaS - Multi-Tenant Customer Relationship Management System

A modern, scalable SaaS-based CRM application built with .NET 10.0 featuring multi-tenancy, role-based access control, and comprehensive business logic for managing leads, deals, contacts, and activities.

---

## 📋 Table of Contents
- [Project Overview](#project-overview)
- [Architecture](#architecture)
- [Key Models & Entities](#key-models--entities)
- [Services & Business Logic](#services--business-logic)
- [API Endpoints](#api-endpoints)
- [Prerequisites](#prerequisites)
- [Setup & Installation](#setup--installation)
- [Database Configuration](#database-configuration)
- [Development Guidelines](#development-guidelines)
- [Project Structure](#project-structure)

---

## 🎯 Project Overview

**CrmSaaS** is a comprehensive Customer Relationship Management system designed for multi-tenant environments. It enables organizations to manage their sales pipeline, customer relationships, activities, and business operations through a secure, role-based API.

### Key Features
- **Multi-Tenant Architecture**: Isolated data and settings per tenant
- **JWT Authentication**: Secure token-based authentication with refresh tokens
- **Role-Based Access Control**: Four role levels (SuperAdmin, TenantAdmin, SalesManager, SalesRep)
- **Lead Management**: Track prospects through the sales funnel
- **Deal Pipeline**: Manage sales deals across multiple stages
- **Contact Management**: Maintain detailed customer contact information
- **Activity Tracking**: Log interactions and activities with customers
- **Audit Logging**: Track all significant business actions
- **Notification System**: Keep users informed of important events

---

## 🏗️ Architecture

CrmSaaS follows a **three-tier architecture** pattern:

```
┌─────────────────────────────────────┐
│         API Layer (Presentation)    │
│  - Controllers                      │
│  - Request/Response Handling        │
│  - Routing & Validation             │
└──────────────┬──────────────────────┘
               │
┌──────────────▼──────────────────────┐
│  BLL Layer (Business Logic)         │
│  - Services (AuthService, etc.)     │
│  - DTOs & Data Mapping              │
│  - Business Rules & Validation      │
│  - Constants & Configuration        │
└──────────────┬──────────────────────┘
               │
┌──────────────▼──────────────────────┐
│     DAL Layer (Data Access)         │
│  - Entity Framework Context         │
│  - Repository Pattern               │
│  - Database Models                  │
│  - Migrations                       │
└─────────────────────────────────────┘
```

### Design Patterns Used
- **Repository Pattern**: Abstract data access logic
- **Dependency Injection**: Loose coupling via DI container
- **DTO Pattern**: Transfer objects for API contracts
- **Factory Pattern**: DataAccessFactory for repository creation
- **Mapper Pattern**: AutoMapper for object transformation

---

## 📊 Key Models & Entities

### 1. **AppUser**
Represents system users with authentication and role management.
- **Properties**: Email, PasswordHash, FirstName, LastName, Role, IsActive, RefreshToken
- **Relationships**: Belongs to Tenant; owns Leads, Deals, Activities, Notifications
- **Roles**: SuperAdmin, TenantAdmin, SalesManager, SalesRep

### 2. **Tenant**
Represents an organization using the SaaS platform.
- **Properties**: Name, Email, Subscription Level, Trial End Date
- **Relationships**: Contains multiple AppUsers and CRM data
- **Multi-Tenancy**: Complete data isolation per tenant

### 3. **Lead**
Represents a prospective customer in the sales pipeline.
- **Properties**: FullName, Email, Phone, Company, Source, Status, Notes, LastContactedAt, ConvertedAt
- **Status**: New, Qualified, Negotiating, Lost, Converted
- **Relationships**: Assigned to AppUser; convertible to Contact/Deal

### 4. **Deal**
Represents a sales opportunity with financial value.
- **Properties**: Title, Description, Value, Stage, Probability, ExpectedCloseDate, ActualCloseDate
- **Stages**: Prospect, Qualified, Proposal, Negotiation, Closed Won/Lost
- **Relationships**: Owned by AppUser; linked to Contacts

### 5. **Contact**
Represents a confirmed customer or business contact.
- **Properties**: FirstName, LastName, Email, Phone, Company, Title, Industry
- **Relationships**: Can have multiple Deals, Notes, and Activities

### 6. **ContactNote**
Additional information and notes about contacts.
- **Properties**: Content, NoteType, CreatedBy, CreatedAt

### 7. **Activity**
Logs customer interactions and sales activities.
- **ActivityType**: Call, Email, Meeting, Task, Note
- **Properties**: Type, Description, ActivityDate, Duration, Outcome
- **Relationships**: Linked to Contacts, Deals, and AppUsers

### 8. **AuditLog**
Tracks all significant business transactions for compliance.
- **Properties**: EntityType, Action, OldValue, NewValue, ChangedBy, Timestamp

### 9. **Notification**
System notifications for users on important events.
- **Type**: Deal Update, Lead Assignment, Task Reminder, System Alert
- **Properties**: Title, Message, IsRead, CreatedAt

### 10. **TenantSettings**
Configurable settings per tenant.
- **Properties**: Currency, TimeZone, Lead Conversion Settings, Custom Fields

### Base Entity
All entities inherit from `BaseEntity`:
```csharp
public class BaseEntity
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public bool IsDeleted { get; set; }
}
```

---

## 🔧 Services & Business Logic

### AuthService
Handles authentication, authorization, and token management.

**Key Methods:**
- `Login(LoginDTO)` - Authenticates user and returns JWT token pair
  - Validates email/password
  - Generates access token (30 min) and refresh token (7 days)
  - Updates user's refresh token in database
  
- `RefreshToken(string refreshToken)` - Issues new token pair
  - Validates refresh token expiry
  - Generates new tokens
  - Updates refresh token for security rotation

- `GenerateTokenPair(AppUser)` - Creates JWT tokens
  - Access Token: Contains user claims (ID, email, role, tenant)
  - Refresh Token: Secure token for obtaining new access tokens

**Security Features:**
- BCrypt password hashing
- JWT with HS256 signing
- Token expiry validation
- Refresh token rotation

### TenantManagementService
Manages tenant lifecycle and operations.

**Responsibilities:**
- Tenant registration and onboarding
- Plan management (Trial, Standard, Professional, Enterprise)
- Trial period management (default: 14 days)
- Tenant settings configuration
- Subscription management

---

## 🔌 API Endpoints

### Authentication Routes
Base URL: `https://localhost:5001/api/auth`

#### 1. **POST /api/auth/login**
Authenticate user and obtain tokens.
```http
POST /api/auth/login
Content-Type: application/json

{
  "email": "user@example.com",
  "password": "SecurePassword123!"
}
```
**Response (200 OK):**
```json
{
  "success": true,
  "message": "Login successful.",
  "data": {
    "accessToken": "eyJhbGciOiJIUzI1NiIs...",
    "refreshToken": "secure_refresh_token_here",
    "expiresIn": 1800,
    "tokenType": "Bearer"
  }
}
```

**Error Responses:**
- `401 Unauthorized`: Invalid credentials or deactivated account
- `400 Bad Request`: Missing/invalid input
- `500 Internal Server Error`: Server error

#### 2. **POST /api/auth/refresh-token**
Obtain new access token using refresh token.
```http
POST /api/auth/refresh-token
Content-Type: application/json

{
  "refreshToken": "secure_refresh_token_here"
}
```
**Response (200 OK):**
```json
{
  "success": true,
  "message": "Token refreshed successfully.",
  "data": {
    "accessToken": "eyJhbGciOiJIUzI1NiIs...",
    "refreshToken": "new_refresh_token_here",
    "expiresIn": 1800,
    "tokenType": "Bearer"
  }
}
```

### Additional Endpoints (In Development)
- **Lead Management**: CRUD operations for leads
- **Deal Pipeline**: Manage sales opportunities
- **Contact Management**: Customer information
- **Activity Logging**: Track customer interactions
- **Reporting**: Analytics and insights

---

## 📋 Prerequisites

- **.NET SDK**: 10.0 or later ([Download](https://dotnet.microsoft.com/download))
- **SQL Server**: 2019 or later, or LocalDB
- **Git**: For version control
- **Visual Studio Code** or **Visual Studio 2024** (recommended)
- **Package Manager**: NuGet (included with .NET SDK)

### Required NuGet Packages
- `EntityFrameworkCore` - ORM
- `EntityFrameworkCore.SqlServer` - SQL Server provider
- `AutoMapper` - Object mapping
- `BCrypt.Net-Next` - Password hashing
- `System.IdentityModel.Tokens.Jwt` - JWT handling

---

## ⚙️ Setup & Installation

### 1. Clone the Repository
```bash
git clone https://github.com/your-repo/CrmSaaS.git
cd CrmSaaS
```

### 2. Restore Dependencies
```bash
dotnet restore
```

### 3. Configure Database Connection
Edit `API/appsettings.json`:
```json
{
  "ConnectionStrings": {
    "DbConn": "Server=.\\SQLEXPRESS;Database=CrmSaaSDb;Integrated Security=true;TrustServerCertificate=true;"
  },
  "Jwt": {
    "SecretKey": "your-secret-key-min-32-characters-long",
    "Issuer": "CrmSaaS",
    "Audience": "CrmSaaSUsers"
  }
}
```

### 4. Build the Solution
```bash
dotnet build
```

### 5. Run Migrations
See [Database Configuration](#database-configuration) section below.

### 6. Run the Application
```bash
dotnet run --project API
```

The API will be available at:
- **HTTPS**: https://localhost:5001
- **HTTP**: http://localhost:5000

---

## 🗄️ Database Configuration

### Entity Framework Core Setup

The project uses **Entity Framework Core** with SQL Server for data persistence.

#### Database Context
Location: `DAL/EF/CrmSaaSDbContext.cs`
- Manages 10 DbSets (entities)
- Configures cascade delete behavior
- Default behavior: Restrict (prevents orphaned records)

#### Create Initial Migration
```bash
dotnet ef migrations add InitialCreate --project DAL --startup-project API --context CrmSaaSDbContext --output-dir EF/Migrations
```

#### Apply Migrations to Database
```bash
dotnet ef database update --project DAL --startup-project API --context CrmSaaSDbContext
```

#### Database Schema
The application creates the following tables:

| Table | Purpose |
|-------|---------|
| AppUsers | User accounts and authentication |
| Tenants | Organization/SaaS customer data |
| Leads | Sales prospects |
| Deals | Sales opportunities |
| Contacts | Customer contacts |
| ContactNotes | Notes associated with contacts |
| Activities | Customer interactions (calls, emails, meetings) |
| AuditLogs | Transaction audit trail |
| Notifications | User notifications |
| TenantSettings | Tenant-specific configurations |

#### Connection String Examples

**SQL Server (Default):**
```
Server=.\\SQLEXPRESS;Database=CrmSaaSDb;Integrated Security=true;TrustServerCertificate=true;
```

**LocalDB:**
```
Server=(localdb)\\mssqllocaldb;Database=CrmSaaSDb;Integrated Security=true;TrustServerCertificate=true;
```

**Remote SQL Server:**
```
Server=your-server.com;Database=CrmSaaSDb;User Id=sa;Password=YourPassword;Encrypt=true;TrustServerCertificate=false;
```

#### Reset Database
To drop and recreate the database:
```bash
dotnet ef database drop --project DAL --startup-project API --context CrmSaaSDbContext
dotnet ef database update --project DAL --startup-project API --context CrmSaaSDbContext
```

---

## 📚 Development Guidelines

### Folder Structure & Responsibilities

#### **API Layer** (`API/`)
- **Controllers/**: HTTP endpoint handlers
  - Use `[ApiController]` and `[Route("api/[controller]")]` attributes
  - Implement proper error handling with try-catch
  - Return standardized response objects
  
- **Models/**: API response/request models (DTOs)
  - Keep models lightweight
  - Separate from domain models

- **appsettings.json**: Configuration
  - Database connection strings
  - JWT settings
  - Logging levels
  - Service URLs

#### **BLL Layer** (`BLL/`)
- **Services/**: Business logic implementation
  - Contain core application logic
  - Validate input data
  - Orchestrate repository calls
  - Enforce business rules
  
- **DTOs/**: Data Transfer Objects
  - `Auth/`: Authentication-related DTOs
  - Keep DTOs focused on single concerns
  - Use for API contracts only
  
- **Constants/**: Application constants
  - `AppConstants.cs`: Token expiry, trial days, rates
  - `ClaimNames.cs`: JWT claim identifiers
  
- **MapperConfig.cs**: AutoMapper profiles (for future use)

#### **DAL Layer** (`DAL/`)
- **EF/Models/**: Domain entity classes
  - Inherit from `BaseEntity`
  - Include data annotations for validation
  - Define navigation properties
  - SQL column type specifications
  
- **EF/Migrations/**: Database schema versions
  - Auto-generated by EF migrations
  - Include `[timestamp]_Description.cs` and `.Designer.cs` files
  - Never manually edit generated code
  
- **EF/CrmSaaSDbContext.cs**: Entity Framework context
  
- **Repos/**: Repository implementations
  - CRUD operations
  - Custom queries
  - One repo per entity
  - Inherit from generic `Repository<T>`
  
- **Interfaces/**: Repository contracts
  - `IRepository<T>`: Generic interface
  - Entity-specific interfaces
  
- **Enums/**: Database enumerations
  - `UserRole`: SuperAdmin, TenantAdmin, SalesManager, SalesRep
  - `LeadStatus`: New, Qualified, Negotiating, Lost, Converted
  - `DealStage`: Prospect, Qualified, Proposal, Negotiation, Closed
  - `ActivityType`, `AuditActionType`, `EntityType`, etc.
  
- **DataAccessFactory.cs**: Repository creation factory
  - Returns appropriate repository instances
  - Centralizes repository access

### Coding Standards

#### Naming Conventions
- **Classes**: PascalCase (`AuthService`, `Lead`)
- **Properties**: PascalCase (`FirstName`, `IsActive`)
- **Methods**: PascalCase (`GenerateTokenPair()`, `ValidateLoginInput()`)
- **Parameters**: camelCase (`loginData`, `userId`)
- **Constants**: UPPER_SNAKE_CASE or PascalCase (`AccessTokenExpiryMinutes`)
- **Interfaces**: Prefix with `I` (`IAuthService`, `IRepository`)

#### Code Organization
```csharp
// Order: Usings → Namespace → Class → Properties → Methods → Navigation Properties
using System;
using System.Collections.Generic;

namespace BLL.Services
{
    public class AuthService
    {
        // Fields
        private readonly DataAccessFactory dataAccessFactory;
        
        // Constructor
        public AuthService(DataAccessFactory factory) { }
        
        // Public Methods
        public TokenResponseDTO Login(LoginDTO data, out string msg) { }
        
        // Private Methods
        private void ValidateLoginInput(string email, string password) { }
    }
}
```

#### Error Handling
- Use specific exception types: `UnauthorizedAccessException`, `ArgumentException`, `InvalidOperationException`
- Always include descriptive error messages
- Log errors appropriately
- Return meaningful HTTP status codes

```csharp
try
{
    // Business logic
}
catch (UnauthorizedAccessException ex)
{
    return Unauthorized(new { Success = false, Message = ex.Message });
}
catch (ArgumentException ex)
{
    return BadRequest(new { Success = false, Message = ex.Message });
}
catch (Exception ex)
{
    return StatusCode(500, new { Success = false, Message = ex.Message });
}
```

#### Out Parameters
The project uses `out` parameters for error messages:
```csharp
var user = repository.FindByEmail(email, out string msg);
if (!success)
    throw new InvalidOperationException($"Operation failed: {msg}");
```

### Configuration Management

#### JWT Configuration
Located in `appsettings.json`:
```json
{
  "Jwt": {
    "SecretKey": "your-very-long-secret-key-with-at-least-32-characters",
    "Issuer": "CrmSaaS",
    "Audience": "CrmSaaSUsers",
    "ExpiryMinutes": 30
  }
}
```

**Important**: Use strong, randomly generated secret keys in production.

#### Environment-Specific Configuration
- `appsettings.json` - Base configuration
- `appsettings.Development.json` - Development overrides
- `appsettings.Production.json` - Production settings

### Adding New Features

#### Adding a New Service
1. Create service class in `BLL/Services/`
2. Inject `DataAccessFactory` for data access
3. Implement business logic and validation
4. Register in `API/Program.cs`

#### Adding a New Entity
1. Create model in `DAL/EF/Models/` inheriting from `BaseEntity`
2. Add DbSet to `CrmSaaSDbContext`
3. Create `IRepository<T>` interface in `DAL/Interfaces/`
4. Create repository in `DAL/Repos/` extending `Repository<T>`
5. Create migration: `dotnet ef migrations add [Name]`
6. Apply migration: `dotnet ef database update`

#### Adding a New API Endpoint
1. Create/modify controller in `API/Controllers/`
2. Create DTOs in `BLL/DTOs/`
3. Inject service and implement action
4. Add proper authorization attributes
5. Return standardized response

### Testing Best Practices
- Test business logic in BLL services
- Mock repositories in unit tests
- Test edge cases and error scenarios
- Use integration tests for API endpoints
- Maintain >80% code coverage

### Security Considerations
- Never commit secrets to repository
- Use strong password hashing (BCrypt)
- Validate all user input
- Implement rate limiting (future)
- Use HTTPS in production
- Regularly rotate JWT secrets
- Implement proper CORS policies
- Sanitize data before storage

### Performance Optimization
- Use `.AsNoTracking()` for read-only queries
- Implement pagination for large datasets
- Use database indexes on frequently queried columns
- Cache frequently accessed data
- Minimize N+1 query problems with `.Include()`

---

## 📁 Project Structure

```
CrmSaaS/
├── API/                          # Presentation Layer
│   ├── Controllers/
│   │   └── AuthController.cs     # Authentication endpoints
│   ├── Models/
│   │   └── ErrorViewModel.cs
│   ├── Properties/
│   │   └── launchSettings.json   # Launch configuration
│   ├── Views/
│   ├── wwwroot/                  # Static files
│   ├── appsettings.json          # Configuration
│   ├── appsettings.Development.json
│   ├── Program.cs                # Application startup
│   └── API.csproj
│
├── BLL/                          # Business Logic Layer
│   ├── Services/
│   │   ├── AuthService.cs        # Authentication logic
│   │   └── TenantManagementService.cs
│   ├── DTOs/
│   │   └── Auth/
│   │       ├── LoginDTO.cs
│   │       ├── TokenResponseDTO.cs
│   │       ├── RefreshTokenDTO.cs
│   │       ├── RegisterTenantDTO.cs
│   │       └── UserDTO.cs
│   ├── Constants/
│   │   ├── AppConstants.cs       # App-wide constants
│   │   └── ClaimNames.cs         # JWT claim names
│   ├── MapperConfig.cs           # AutoMapper configuration
│   └── BLL.csproj
│
├── DAL/                          # Data Access Layer
│   ├── EF/
│   │   ├── CrmSaaSDbContext.cs   # Entity Framework context
│   │   ├── Models/               # Domain entities
│   │   │   ├── BaseEntity.cs
│   │   │   ├── AppUser.cs
│   │   │   ├── Tenant.cs
│   │   │   ├── Lead.cs
│   │   │   ├── Deal.cs
│   │   │   ├── Contact.cs
│   │   │   ├── ContactNote.cs
│   │   │   ├── Activity.cs
│   │   │   ├── AuditLog.cs
│   │   │   ├── Notification.cs
│   │   │   └── TenantSettings.cs
│   │   └── Migrations/           # Database migrations
│   │       └── [timestamp]_*.cs
│   ├── Repos/                    # Repository implementations
│   │   ├── Repository.cs         # Base repository
│   │   ├── AppUserRepo.cs
│   │   ├── LeadRepo.cs
│   │   ├── DealRepo.cs
│   │   └── ... (other repos)
│   ├── Interfaces/               # Repository contracts
│   │   ├── IRepository.cs
│   │   ├── IAppUserRepo.cs
│   │   └── ... (other interfaces)
│   ├── Enums/                    # Database enumerations
│   │   ├── UserRole.cs
│   │   ├── LeadStatus.cs
│   │   ├── DealStage.cs
│   │   └── ... (other enums)
│   ├── DataAccessFactory.cs      # Factory for repository creation
│   └── DAL.csproj
│
├── CrmSaaS.slnx                  # Solution file
└── README.md                     # This file
```

---

## 🚀 Running the Application

### Development Mode
```bash
# From project root
dotnet run --project API

# Or with watch mode for auto-reload
dotnet watch --project API
```

### With Docker (Future)
```bash
docker build -t crmsaas .
docker run -p 5001:443 crmsaas
```

### Verify Installation
- Open https://localhost:5001 in browser
- You should see the default page or API swagger (if enabled)
- Try login endpoint: `POST /api/auth/login`

---

## 🤝 Contributing

1. Create a feature branch: `git checkout -b feature/your-feature`
2. Commit changes: `git commit -am 'Add feature'`
3. Push to branch: `git push origin feature/your-feature`
4. Submit pull request with description

### Code Review Checklist
- [ ] Code follows naming conventions
- [ ] Proper error handling implemented
- [ ] Database migrations included (if applicable)
- [ ] DTOs created for API contracts
- [ ] Business logic in services (not controllers)
- [ ] Tests written for new features
- [ ] Documentation updated

---

## 📝 License

This project is proprietary and confidential. All rights reserved.

---

## 📧 Support

For issues and questions, contact the development team or create an issue in the repository.

---

## 🗓️ Changelog

### Version 1.0.0 (Current)
- ✅ Multi-tenant architecture
- ✅ JWT authentication with refresh tokens
- ✅ Role-based access control (RBAC)
- ✅ Lead management system
- ✅ Deal pipeline tracking
- ✅ Contact management
- ✅ Activity logging
- ✅ Audit trail system
- ✅ Notification framework

### Planned Features (v1.1.0+)
- [ ] Real-time notifications via SignalR
- [ ] Advanced reporting and analytics
- [ ] Custom fields support
- [ ] Email integration
- [ ] Mobile API optimization
- [ ] Batch operations
- [ ] API rate limiting
- [ ] File attachment support

---

**Last Updated**: May 2026  
**Framework**: .NET 10.0  
**Target**: Production-ready SaaS CRM platform
