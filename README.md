## Project Overview: TreeNodes API

### Technologies & Architecture
- **Backend Framework**: ASP.NET Core Web API with .NET 8.0  
- **Architecture**: Clean Architecture with separate layers (Domain, Application, Infrastructure, Web, Auth)  
- **Database**: PostgreSQL with Entity Framework Core and automatic migrations on startup  
- **Design Pattern**: CQRS implementation using MediatR for command/query separation  
- **Authentication**: JWT-based authentication using partner code validation  
- **Validation**: FluentValidation with MediatR pipeline behaviors for request validation  
- **Logging**: Serilog configured for structured logging to console and debug outputs  
- **Exception Handling**: Global exception handling middleware that automatically logs all exceptions to a database journal  

### Features
- **Tree Management**: Hierarchical tree/node structure with full CRUD operations (create, get, rename, delete nodes)  
- **Business Rules**: Enforces unique node names among siblings within the same tree  
- **Exception Journal**: Paginated API endpoint to view and filter logged exceptions with detailed request context  
- **API Documentation**: Swagger/OpenAPI integration for interactive API documentation  
- **Cascade Deletion**: Deleting a node automatically removes all its descendants  

### Testing
- **Test Framework**: xUnit with FluentAssertions for readable assertions and Moq for mocking  
- **Test Database**: EF Core InMemory database for isolated unit and integration tests

### Screenshot from the "journal" table with logged exceptions
  <img width="828" height="137" alt="image" src="https://github.com/user-attachments/assets/ff9325cd-7ebe-4af1-bcd1-16b8036f0b50" />
