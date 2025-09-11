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

# Apply migrations for TenantDbContext FIRST (as per requirement)
echo "🔄 Applying migrations for TenantDbContext..."
dotnet ef database update -c TenantDbContext --verbose

# Apply migrations for ApplicationDbContext SECOND
echo "🔄 Applying migrations for ApplicationDbContext..."
dotnet ef database update -c ApplicationDbContext --verbose

echo "✅ Migration completed successfully!"

echo "🎉 Database setup completed!"
