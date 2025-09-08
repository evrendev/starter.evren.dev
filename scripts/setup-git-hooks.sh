#!/bin/bash

# Git hook setup script
echo "🔧 Setting up git hooks..."

# Create git hooks directory if it doesn't exist
mkdir -p .git/hooks

# Create post-commit hook
cat > .git/hooks/post-commit << 'EOF'
#!/bin/bash

echo "🚀 Git commit detected, updating development containers..."

# Check if docker-compose is available
if ! command -v docker &> /dev/null || ! command -v docker-compose &> /dev/null; then
    echo "❌ Docker or docker-compose not found. Skipping container update."
    exit 0
fi

# Check if development containers are running
if docker compose -f docker-compose.yml -f docker-compose.override.yml ps --services --filter "status=running" | grep -q .; then
    echo "🔄 Rebuilding and restarting development containers..."
    
    # Rebuild only changed services
    docker compose -f docker-compose.yml -f docker-compose.override.yml build
    
    # Restart containers
    docker compose -f docker-compose.yml -f docker-compose.override.yml up -d
    
    echo "✅ Development containers updated successfully!"
else
    echo "ℹ️  No development containers running. Skipping update."
fi
EOF

# Make the hook executable
chmod +x .git/hooks/post-commit

echo "✅ Git hooks setup completed!"
echo "📝 Now when you commit changes, development containers will automatically rebuild and restart."
echo "🎯 To start development mode: make dev"
echo "🎯 To start production mode: make up"
