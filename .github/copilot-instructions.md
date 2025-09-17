# AI Coding Agent Instructions for EvrenDev Boilerplate

## Project Overview

This is a full-stack application boilerplate based on .NET 9 (backend) and Vue 3 (frontend) designed for building enterprise-grade SaaS applications. The solution follows Clean Architecture principles with Domain-Driven Design.

## Architecture

### Backend Architecture (.NET 9)

- **Core Layer**:
  - `Domain`: Contains business entities, enums, exceptions, interfaces, value objects and domain events (`/src/backend/Core/Domain`).
  - `Application`: Contains business logic, CQRS commands/queries, interfaces, DTOs (`/src/backend/Core/Application`).
  - `Shared`: Contains shared components used across the application (`/src/backend/Core/Shared`).
- **Infrastructure Layer** (`/src/backend/Infrastructure`): Implementation of interfaces defined in the Application layer.
- **API Layer** (`/src/backend/PublicApi`): ASP.NET Core Web API project with controllers and API endpoints.

### Frontend Architecture (Vue 3)

- **Composition API**: All components use the Composition API pattern.
- **Pinia Store**: State management using Pinia stores (`/src/frontend/src/stores/`).
- **Auto-imports**: Vue components and APIs are auto-imported (see `vite.config.ts`).

## Key Development Workflows

### Local Development

```bash
# Start the development environment with Docker
make dev

# Alternative commands
docker compose -f docker-compose.yml -f docker-compose.override.yml up -d

# Manual backend development
dotnet run --project src/backend/PublicApi/PublicApi.csproj

# Manual frontend development
cd src/frontend
npm run dev
```

### Database Migrations

```bash
# Run migrations
dotnet ef database update --project src/backend/PublicApi
```

## Project Conventions

### Backend Conventions

1. **CQRS Pattern**: Commands and Queries are separated in the Application layer.
2. **Domain Events**: Used for cross-domain communication.
3. **Repository Pattern**: Used for data access in the Infrastructure layer.
4. **Multi-tenancy**: The application supports multi-tenancy using Finbuckle.MultiTenant.
5. **Mediator Pattern**: Used for implementing CQRS with MediatR library.
6. **Validation**: FluentValidation is used for request validation.

### Frontend Conventions

1. **Component Structure**: Components are organized in feature-based directories.
2. **Store Pattern**: Use Pinia for state management with dedicated stores for each module.
3. **Typed API Calls**: TypeScript interfaces for API responses.
4. **Auto-imports**: Vue components and functions are auto-imported.

## Common Integration Points

1. **Authentication**: JWT-based authentication flow between frontend and backend.
2. **API Communication**: Frontend uses Axios for API calls to the backend.
3. **Real-time Updates**: SignalR for real-time communication.

## Configuration Management

- Backend: JSON files in `/src/backend/PublicApi/Configurations/`
- Frontend: Environment variables in `.env` files and Vuetify theme settings

## Debugging Workflows

- Backend: Use standard .NET debugging tools
- Frontend: Vue DevTools and browser developer console

## Documentation

- API: Swagger/OpenAPI documentation available at `/swagger` endpoint
- Project: README.md for overall project documentation
