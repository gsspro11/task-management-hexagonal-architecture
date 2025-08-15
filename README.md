# Task Management System
This repository implements a task management system using Hexagonal Architecture in .NET for the backend and Angular for the frontend.
## Prerequisites
- .NET SDK 8.0 or later
- Node.js 18+ and npm
- Visual Studio 2022 or later (optional, for development)
- SQL Server LocalDB (included with Visual Studio or SQL Server Express)
## Cloning the Repository
1. Clone the repository:
  ```
  git clone https://github.com/gsspro11/task-management-hexagonal-architecture.git
  ```
2. Navigate to the project root:
  ```
  cd task-management-hexagonal-architecture
  ```
## Backend Setup and Execution
The backend is located in the `backend` directory and follows Hexagonal Architecture with projects for Domain, Application, Database (Infrastructure), and API.
### 1. Restore Dependencies
Navigate to the backend directory:
```
cd backend
```
Restore NuGet packages:
```
dotnet restore TaskManagement.HexagonalArchitecture.sln
```
### 2. Database Configuration
The backend uses Entity Framework Core with SQL Server LocalDB by default. The connection string is defined in `backend/Adapters/Driving/Apis/TaskManagement.HexagonalArchitecture.Api/appsettings.Development.json`:
```json
"ConnectionStrings": {
 "SqlServerConnection": "Server=(localdb)\\mssqllocaldb;Database=TaskManagement;Trusted_Connection=True;MultipleActiveResultSets=true"
}
```
Update the connection string if using a different SQL Server instance.
### 3. Apply Database Migrations
Ensure the Database project (Infrastructure) and API project are referenced correctly. Run the following commands from the `backend` directory to create and apply migrations:
- Add an initial migration (if not already present):
 ```
 dotnet ef migrations add InitialMigration --project Adapters\Driven\TaskManagement.HexagonalArchitecture.Database --startup-project Adapters\Driving\Apis\TaskManagement.HexagonalArchitecture.Api
 ```
- Update the database:
 ```
 dotnet ef database update --project Adapters\Driven\TaskManagement.HexagonalArchitecture.Database --startup-project Adapters\Driving\Apis\TaskManagement.HexagonalArchitecture.Api
 ```
This will create the `TaskManagement` database in LocalDB and apply schema changes.
### 4. Run the Backend
Build the solution:
```
dotnet build TaskManagement.HexagonalArchitecture.sln
```
Run the API project:
```
dotnet run --project Adapters\Driving\Apis\TaskManagement.HexagonalArchitecture.Api\TaskManagement.HexagonalArchitecture.Api.csproj --environment Development
```
The API will start on default ports (e.g., http://localhost:5000 and https://localhost:5001). Authentication uses JWT; configure the secret in `appsettings.Development.json` if needed.
### 5. Testing the Backend (Optional)
Run unit/integration tests:
```
dotnet test TaskManagement.HexagonalArchitecture.sln
```
## Frontend Setup and Execution
The frontend is an Angular application located in the `frontend` directory.
### 1. Install Dependencies
Navigate to the frontend directory:
```
cd ../frontend
```
Install npm packages:
```
npm install
```
### 2. Run the Frontend
Start the Angular development server:
```
ng serve
```
The frontend will be available at http://localhost:4200. It communicates with the backend API (ensure the backend is running). Update API endpoints in the frontend code (e.g., environment.ts) if the backend URL changes.
## Additional Notes
- **Authentication**: The system uses JWT for authentication. Register/login endpoints are available in the API controllers.
- **Configuration**: For production, update `appsettings.json` and deploy accordingly (e.g., via IIS or Docker, though no Dockerfiles are present).
- **Development**: Open `backend/TaskManagement.HexagonalArchitecture.sln` in Visual Studio to debug or develop further. Use VS Code for the frontend.