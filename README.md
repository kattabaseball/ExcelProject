# Excel Template System

A .NET 8 Web API application for managing Excel templates and processing Excel file submissions.

## Project Structure

- **ExcelTemplateSystem.API**: ASP.NET Core Web API project
- **ExcelTemplateSystem.Business**: Business logic layer
- **ExcelTemplateSystem.Data**: Data access layer with Entity Framework Core

## Prerequisites

- .NET 8.0 SDK
- SQL Server (LocalDB is used by default)

## Getting Started

1. Clone the repository
2. Restore NuGet packages
3. Update the connection string in `appsettings.json` if needed
4. Run the application

## API Endpoints

### Document Templates

- `GET /api/documenttemplates`: Get all document templates
- `GET /api/documenttemplates/{id}`: Get a specific template by ID
- `POST /api/documenttemplates`: Create a new template
- `PUT /api/documenttemplates/{id}`: Update a template
- `DELETE /api/documenttemplates/{id}`: Delete a template

### Template Columns

- `POST /api/documenttemplates/{templateId}/columns`: Add a column to a template
- `DELETE /api/documenttemplates/columns/{columnId}`: Remove a column from a template

### Excel Operations

- `GET /api/documenttemplates/{templateId}/download`: Download an Excel template
- `POST /api/documenttemplates/{templateId}/submit?submittedBy=user`: Submit an Excel file for processing

### Submissions

- `GET /api/documenttemplates/submissions/{uniqueIdentifier}`: Get a specific submission
- `GET /api/documenttemplates/{templateId}/submissions`: Get all submissions for a template

## Database Migrations

To apply database migrations, run the following command from the API project directory:

```bash
dotnet ef database update
```

## Testing

You can test the API using tools like Postman or Swagger UI available at `/swagger` when running the application.

## License

MIT
"# ExcelProject" 
