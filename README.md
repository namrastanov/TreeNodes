## Project Overview: TreeNodes API

### Improvements
- **API**: Refactored API endpoints from dot-notation format (e.g., api.user.tree.get) to RESTful standards using proper HTTP verbs (GET, POST, PUT, DELETE), resource-based paths (e.g., GET /api/trees/{treeName}), and request bodies for POST/PUT operations.

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
  <img width="1210" height="97" alt="Screenshot_2" src="https://github.com/user-attachments/assets/fed8ae4d-f39e-498a-8640-4929e1b9d988" />

### Screenshots from the swagger
  <img width="1485" height="849" alt="image" src="https://github.com/user-attachments/assets/29c85f48-bbf6-4c81-bc1c-65381eae9f0b" />

  <img width="566" height="624" alt="Screenshot_1" src="https://github.com/user-attachments/assets/bc225dcc-48a7-4f21-aab9-a3ae24b45cda" />


