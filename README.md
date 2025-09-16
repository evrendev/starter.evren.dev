# EvrenDev - Full Stack Boilerplate

> An enterprise-grade boilerplate for modern web applications based on .NET 9 and Vue 3

[![.NET](https://img.shields.io/badge/.NET-9.0-purple)](https://dotnet.microsoft.com/)
[![Vue.js](https://img.shields.io/badge/Vue.js-3.5-green)](https://vuejs.org/)
[![PostgreSQL](https://img.shields.io/badge/PostgreSQL-15-blue)](https://postgresql.org/)
[![Docker](https://img.shields.io/badge/Docker-Compose-blue)](https://docker.com/)
[![License](https://img.shields.io/badge/License-MIT-yellow.svg)](LICENSE)

## 📋 Table of Contents

- [About the Project](#-about-the-project)
- [Features](#-features)
- [Technology Stack](#-technology-stack)
- [System Requirements](#-system-requirements)
- [Installation](#-installation)
- [Development](#-development)
- [Deployment](#-deployment)
- [Configuration](#-configuration)
- [API Documentation](#-api-documentation)
- [Contributing](#-contributing)
- [License](#-license)

## 🚀 About the Project

**Full Stack Boilerplate** designed to accelerate the development process of modern web applications. Utilizing a .NET 9 backend and Vue 3 frontend, this platform enables developers to build SaaS applications quickly and securely.

### Key Goals

- 🎯 **Rapid Development**: Quickly start projects with a ready-to-use infrastructure.
- 🔒 **Security**: JWT Authentication, Policy-based authorization.
- 📈 **Scalability**: Multi-tenant architecture and a microservice-ready foundation.
- 🎨 **Modern UI/UX**: Responsive design with Vue 3 + Vuetify.
- 🔧 **DevOps Ready**: Support for Docker and CI/CD pipelines.

## ✨ Features

### Backend (.NET 9)

- **🏗️ Clean Architecture**: Based on Domain-Driven Design principles.
- **🔐 Authentication & Authorization**: JWT Bearer Token, Policy-based access.
- **🏢 Multi-Tenancy**: Tenant management with Finbuckle.MultiTenant.
- **📊 Background Jobs**: Background processing with Hangfire.
- **💾 Database Support**: Support for PostgreSQL & SQL Server.
- **📧 Email Service**: Email sending with MailKit.
- **📄 Export/Import**: Data transfer with Excel files.
- **🔍 API Documentation**: Swagger/OpenAPI integration.
- **📈 Monitoring & Logging**: Structured logging with Serilog.
- **⚡ Caching**: Performance optimization with Redis.
- **🌐 Localization**: Multi-language support.
- **🔄 Real-time Communication**: SignalR integration.

### Frontend (Vue 3)

- **⚡ Modern Framework**: Vue 3 + Composition API.
- **🎨 UI Framework**: Material Design with Vuetify 3.
- **🏗️ Build Tool**: Fast development with Vite.
- **📱 Responsive Design**: Mobile-first approach.
- **🔄 State Management**: Centralized state management with Pinia.
- **🌐 Internationalization**: Multi-language support with Vue I18n.
- **📊 Charts & Visualization**: ApexCharts integration.
- **🔧 TypeScript**: Type-safe development.
- **🎯 Auto-import**: Automatic imports with Unplugin.
- **🧪 Testing**: Playwright + Vue Test Utils.

### DevOps & Infrastructure

- **🐳 Docker**: Multi-stage Dockerfiles.
- **🔄 Docker Compose**: Development and production environments.
- **📦 Database Migrations**: Entity Framework migrations.
- **🔧 Health Checks**: Application health monitoring.
- **📊 Monitoring**: PostgreSQL + Redis health checks.

## 🛠️ Technology Stack

### Backend Technologies

| Technology                | Version | Description                 |
| ------------------------- | ------- | --------------------------- |
| **.NET**                  | 9.0     | Core framework              |
| **ASP.NET Core**          | 9.0     | Web API framework           |
| **Entity Framework Core** | 9.0     | ORM and database migrations |
| **PostgreSQL**            | 15+     | Primary database            |
| **Redis**                 | Alpine  | Cache and session store     |
| **Hangfire**              | 1.8.18  | Background job processing   |
| **SignalR**               | 9.0     | Real-time communication     |
| **MediatR**               | 12.5    | CQRS pattern implementation |
| **FluentValidation**      | 12.0    | Request validation          |
| **Mapster**               | 7.4     | Object mapping              |
| **Serilog**               | Latest  | Structured logging          |
| **JWT Bearer**            | 9.0     | Authentication              |
| **Swagger/NSwag**         | Latest  | API documentation           |

### Frontend Technologies

| Technology      | Version | Description                |
| --------------- | ------- | -------------------------- |
| **Vue.js**      | 3.5.21  | Progressive framework      |
| **Vuetify**     | 3.10    | Material Design components |
| **TypeScript**  | 5.9     | Type-safe JavaScript       |
| **Vite**        | 5.4     | Build tool and dev server  |
| **Pinia**       | 3.0     | State management           |
| **Vue Router**  | 4.5     | SPA routing                |
| **Vue I18n**    | 11.1    | Internationalization       |
| **Axios**       | 1.12    | HTTP client                |
| **ApexCharts**  | 1.8     | Data visualization         |
| **VeeValidate** | 4.15    | Form validation            |

### Development & DevOps

| Technology         | Description                   |
| ------------------ | ----------------------------- |
| **Docker**         | Containerization              |
| **Docker Compose** | Multi-container orchestration |
| **Make**           | Build automation              |
| **ESLint**         | JavaScript/TypeScript linting |
| **Prettier**       | Code formatting               |
| **StyleLint**      | CSS/SCSS linting              |
| **Playwright**     | End-to-end testing            |

## 💻 System Requirements

### Development Environment

- **Operating System**: Windows 10+, macOS 10.15+, Ubuntu 18.04+
- **.NET SDK**: 9.0.300+
- **Node.js**: 22.x LTS
- **Docker**: 20.10+
- **Docker Compose**: 2.0+
- **Git**: 2.30+

### Production Requirements

- **CPU**: 2+ cores
- **RAM**: 4GB+ (8GB recommended)
- **Disk**: 20GB+ available space
- **Network**: HTTP/HTTPS access

## 🚀 Installation

### 1. Clone the Repository

```bash
git clone [https://github.com/evrendev/starter.evren.dev.git](https://github.com/evrendev/starter.evren.dev.git)
cd starter.evren.dev
```

### 2\. Quick Start with Docker (Recommended)

#### Development Environment

```bash
# Start the development environment
make dev

# Alternatively:
docker compose -f docker-compose.yml -f docker-compose.override.yml up -d
```

#### Production Environment

```bash
# Start the production environment
make up

# Alternatively:
docker compose -f docker-compose.yml -f docker-compose.prod.yml up -d
```

### 3\. Manual Installation

#### Backend Setup

```bash
# Check your .NET SDK installation
dotnet --version

# Install dependencies
dotnet restore BackendBoilerplate.sln

# Configure database settings (see Configuration)
# Run the migrations
dotnet ef database update --project src/backend/PublicApi

# Run the backend
dotnet run --project src/backend/PublicApi/PublicApi.csproj
```

#### Frontend Setup

```bash
cd src/frontend

# Install dependencies
npm install

# Start the development server
npm run dev

# Build for production
npm run build
```

## 🔧 Development

### Make Commands

```bash
# Display the help menu
make help

# Start the development environment
make dev

# Build and deploy for production
make build
make up

# View logs
make logs
make logs-backend
make logs-frontend

# Check container status
make status

# Cleanup
make clean
```

### Development Workflow

1.  **Feature Branch**: `git checkout -b feature/new-feature`
2.  **Development**: Start the local environment with `make dev`
3.  **Coding**: Backend: `src/backend/`, Frontend: `src/frontend/`
4.  **Testing**: `npm run test` (frontend), `dotnet test` (backend)
5.  **Commit**: Use Conventional Commits
6.  **Pull Request**: Create a merge request to the main branch

### Hot Reload

- **Backend**: Automatic reload with `dotnet watch`
- **Frontend**: Instant updates with Vite HMR
- **Database**: Schema updates with EF Core migrations

## 🌐 Deployment

### Docker Production Deploy

```bash
# Build production containers
make build

# Start the production environment
make up

# Health check
curl http://localhost:5001/health
```

### Environment Variables

Set the following environment variables for production deployment:

```bash
# Database
ASPNETCORE_ENVIRONMENT=Production
DatabaseSettings__ConnectionString=<your_connection_string>
DatabaseSettings__DbProvider=PostgreSQL

# Cache
CacheSettings__RedisURL=<redis_connection>

# Security
JwtSettings__SecretKey=<your_secret_key>
JwtSettings__Issuer=<your_issuer>
JwtSettings__Audience=<your_audience>

# Email
MailSettings__Server=<smtp_server>
MailSettings__Port=587
MailSettings__UserName=<smtp_username>
MailSettings__Password=<smtp_password>
```

## ⚙️ Configuration

### Backend Configuration

Backend configurations are managed with JSON files in the `src/backend/PublicApi/Configurations/` directory:

#### Database Configuration (`database.json`)

```json
{
  "DatabaseSettings": {
    "ConnectionString": "Host=localhost;Port=5432;Database=evrendev-sass;Username=postgres;Password=P@s5w0rd.123;Include Error Detail=true",
    "DBProvider": "PostgreSQL"
  }
}
```

#### Cache Configuration (`cache.json`)

```json
{
  "CacheSettings": {
    "RedisURL": "localhost:6379",
    "SlidingExpiration": "00:15:00"
  }
}
```

#### CORS Configuration (`cors.json`)

```json
{
  "CorsSettings": {
    "Angular": "http://localhost:4200",
    "Blazor": "https://localhost:5002",
    "React": "http://localhost:3000",
    "Vue": "http://localhost:5173"
  }
}
```

### Frontend Configuration

#### API Base URL

```typescript
// src/frontend/.env.local
VITE_API_URL=http://localhost:5001
```

#### Vuetify Theme

```scss
// src/frontend/src/assets/styles/admin/variables/_vuetify.scss
// Custom theme configurations
```

### Environment-specific Configurations

- **Development**: `*.development.json` files
- **Docker**: `*.Docker.json` files
- **Production**: Overridden by environment variables

## 📚 API Documentation

### Swagger UI

Once the backend is running, you can access the Swagger UI at:

- Development: `http://localhost:5001/swagger`
- Production: `https://your-domain.com/swagger`

### Main Endpoints

#### Authentication

```
POST /api/identity/login
POST /api/identity/register
POST /api/identity/refresh-token
POST /api/identity/logout
```

#### User Management

```
GET    /api/identity/users
POST   /api/identity/users
PUT    /api/identity/users/{id}
DELETE /api/identity/users/{id}
```

#### Catalog Management

```
GET    /api/catalog/products
POST   /api/catalog/products
PUT    /api/catalog/products/{id}
DELETE /api/catalog/products/{id}
```

## 🧪 Testing

### Backend Testing

```bash
# Run unit tests
dotnet test

# Generate coverage report
dotnet test --collect:"XPlat Code Coverage"
```

### Frontend Testing

```bash
cd src/frontend

# Run unit tests
npm run test:unit

# Run E2E tests
npm run test:e2e

# Playwright tests
npx playwright test
```

## 🔒 Security

### Security Headers

The application includes the following security headers by default:

- Content Security Policy (CSP)
- X-Frame-Options
- X-Content-Type-Options
- Referrer-Policy
- Permissions-Policy

### Authentication Flow

1.  **Login**: Obtain a JWT token with Username/Password.
2.  **Authorization**: Access the API with a Bearer token.
3.  **Refresh**: Token refresh mechanism.
4.  **Logout**: Token invalidation.

## 📈 Performance & Monitoring

### Caching Strategy

- **Memory Cache**: For frequently accessed data.
- **Redis Cache**: For distributed caching.
- **HTTP Cache**: For static resources.

### Logging

```bash
# Set log levels
"Logging": {
  "LogLevel": {
    "Default": "Information",
    "Microsoft": "Warning"
  }
}
```

### Health Checks

- Database connectivity
- Redis availability
- External service dependencies

## 🤝 Contributing

### Development Setup

1.  Fork and clone the repository.
2.  Create a feature branch: `git checkout -b feature/amazing-feature`
3.  Commit your changes: `git commit -m 'feat: add amazing feature'`
4.  Push your branch: `git push origin feature/amazing-feature`
5.  Create a Pull Request.

### Coding Standards

- **Backend**: Microsoft C\# coding conventions.
- **Frontend**: ESLint + Prettier.
- **Commits**: Conventional Commits format.
- **Documentation**: In-code documentation is required.

## 📄 License

This project is licensed under the MIT License. See the [LICENSE](https://www.google.com/search?q=LICENSE) file for details.

## 👨‍💻 Author

**Evren Yeniuğur**

- Website: [evren.dev](https://evren.dev)
- LinkedIn: [evrenyeniev](https://linkedin.com/in/evreyeniev)
- GitHub: [@evrendev](https://github.com/evrendev)

---

⭐ If you find this project useful, don't forget to give it a star\!

📞 For questions, please use the [Issues](https://github.com/evrendev/starter.evren.dev/issues) section.
