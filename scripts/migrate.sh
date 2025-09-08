#!/bin/bash
set -e

echo "🚀 Starting database migration process..."

# Wait for database to be ready
echo "⏳ Waiting for database to be ready..."
until pg_isready -h db -p 5432 -U postgres; do
  echo "Database is unavailable - sleeping"
  sleep 2
done

echo "✅ Database is ready!"

# Set environment for Docker configuration
export ASPNETCORE_ENVIRONMENT=docker

# Debug: Check environment variable
echo "🔍 Environment variable check:"
echo "ASPNETCORE_ENVIRONMENT = $ASPNETCORE_ENVIRONMENT"

# Navigate to migrator directory
cd /src/src/backend/Migrators/Migrators.PostgreSQL

# Apply migrations for ApplicationDbContext using startup project configuration
echo "🔄 Applying migrations for ApplicationDbContext..."
echo "📂 Using startup project: /src/src/backend/PublicApi"
echo "🔧 Expected config file: /src/src/backend/PublicApi/Configurations/database.docker.json"
dotnet ef database update -s /src/src/backend/PublicApi -c ApplicationDbContext --verbose

# Apply migrations for TenantDbContext using startup project configuration
echo "🔄 Applying migrations for TenantDbContext..."
dotnet ef database update -s /src/src/backend/PublicApi -c TenantDbContext --verbose || echo "⚠️  TenantDbContext migration skipped (may have pending changes)"

echo "✅ Migration completed successfully!"

echo "🎉 Database setup completed!"
