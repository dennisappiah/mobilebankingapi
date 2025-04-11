# Mobile Banking API

A robust mobile banking API built using ASP.NET Web API, leveraging Dapper as the ORM, with PostgreSQL stored procedures accessed via the Npgsql client.

## Overview

This API provides a secure and scalable backend for mobile banking applications, offering essential banking functionalities such as account management, transactions, authentication, and more. Built with modern technologies and best practices in mind, it ensures high performance and maintainability.

## Tech Stack

- **Framework**: ASP.NET Web API
- **ORM**: Dapper
- **Database**: PostgreSQL
- **Database Access**: Npgsql
- **Authentication**: JWT Bearer tokens

## Features

- 🔐 **Secure Authentication & Authorization**
  - JWT-based authentication
  - Role-based access control
  - Multi-factor authentication support

- 💼 **Account Management**
  - Account creation and maintenance
  - Balance inquiries
  - Account statements and history

- 💸 **Transaction Processing**
  - Funds transfers (internal and external)
  - Bill payments
  - Scheduled transactions

- 📊 **Reporting**
  - Transaction history
  - Spending analytics
  - Account summaries

- 🔔 **Notifications**
  - Transaction alerts
  - Security notifications
  - Account updates

## Architecture

The API follows a clean architecture approach with the following layers:

1. **API Layer** - Controllers and endpoints
2. **Service Layer** - Business logic implementation
3. **Repository Layer** - Data access using Dapper and stored procedures
4. **Domain Layer** - Core business entities and interfaces

## Getting Started

### Prerequisites

- .NET 8.0 SDK or later
- PostgreSQL 14 or later
- Visual Studio 2022 or preferred IDE

### Installation

1. Clone the repository
   ```
   git clone https://github.com/yourusername/mobile-banking-api.git
   cd mobile-banking-api
   ```

2. Set up the database
   ```
   psql -U postgres -f ./database/schema.sql
   psql -U postgres -f ./database/stored_procedures.sql
   ```

3. Configure connection strings
   - Update `appsettings.json` with your PostgreSQL connection details

4. Build and run the application
   ```
   dotnet restore
   dotnet build
   dotnet run
   ```

5. API will be available at `https://localhost:5001`

## Database Design

The API uses PostgreSQL stored procedures to handle all database operations, providing an additional layer of security and performance optimization. The Npgsql client is used to connect to PostgreSQL, while Dapper maps the results to domain entities.

### Key Database Objects

- **Tables**: Accounts, Transactions, Users, Cards, etc.
- **Stored Procedures**: Used for all CRUD operations and complex business logic
- **Functions**: For calculations and validations
- **Views**: For reporting and analytics

## API Endpoints

### Authentication
- `POST /api/auth/login` - Authenticate user
- `POST /api/auth/register` - Register new user
- `POST /api/auth/refresh-token` - Refresh JWT token
- `POST /api/auth/verify-mfa` - Verify multi-factor authentication

### Accounts
- `GET /api/accounts` - Get all accounts for authenticated user
- `GET /api/accounts/{id}` - Get account by ID
- `GET /api/accounts/{id}/balance` - Get account balance
- `GET /api/accounts/{id}/statements` - Get account statements

### Transactions
- `POST /api/transactions/transfer` - Transfer funds
- `POST /api/transactions/payment` - Make a payment
- `GET /api/transactions/{id}` - Get transaction details
- `GET /api/transactions/history` - Get transaction history

## Security Considerations

- All API endpoints are secured with JWT authentication
- Password hashing using BCrypt
- HTTPS enforced for all communications
- Data encryption for sensitive information
- Rate limiting to prevent abuse
- Comprehensive logging for audit trails
- Input validation and sanitization to prevent SQL injection

## Performance Optimization

- Efficient database access with Dapper
- PostgreSQL stored procedures for complex operations
- Response caching for appropriate endpoints
- Async/await pattern implementation
- Connection pooling

## Development Best Practices

### Coding Standards
- Follow C# coding conventions
- Use meaningful names for methods, properties, and variables
- Document public APIs with XML comments

### Testing
- Unit tests for services and repositories
- Integration tests for API endpoints
- Performance tests for critical operations

### Deployment
- CI/CD pipeline configuration
- Docker support for containerized deployment
- Environment-specific configuration

## Configuration

Configuration can be modified through `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "BankingDatabase": "Host=localhost;Database=banking;Username=user;Password=password;"
  },
  "JwtSettings": {
    "Secret": "your-secret-key-with-at-least-32-characters",
    "ExpiryMinutes": 60,
    "Issuer": "banking-api",
    "Audience": "mobile-app"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft": "Warning"
    }
  }
}
```

## Contributing

1. Fork the repository
2. Create your feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'Add some amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

