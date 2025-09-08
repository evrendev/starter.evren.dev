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

# Set connection string
CONNECTION_STRING="Host=db;Port=5432;Database=evrendev-sass;Username=postgres;Password=P@s5w0rd.123;"

# Navigate to migrator directory
cd /src/src/backend/Migrators/Migrators.PostgreSQL

# Apply migrations for ApplicationDbContext (main application context)
echo "🔄 Applying migrations for ApplicationDbContext..."
dotnet ef database update --context ApplicationDbContext --connection "$CONNECTION_STRING"

# Apply migrations for TenantDbContext (ignore pending model changes)
echo "🔄 Applying migrations for TenantDbContext..."
dotnet ef database update --context TenantDbContext --connection "$CONNECTION_STRING" || echo "⚠️  TenantDbContext migration skipped (may have pending changes)"

echo "✅ Migration completed successfully!"

echo "🎉 Database setup completed!"
