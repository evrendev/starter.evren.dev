.PHONY: help build up down migrate dev dev-build clean logs status

# Default target
help: ## Show this help message
	@echo "Available commands:"
	@grep -E '^[a-zA-Z_-]+:.*?## .*$$' $(MAKEFILE_LIST) | sort | awk 'BEGIN {FS = ":.*?## "}; {printf "\033[36m%-15s\033[0m %s\n", $$1, $$2}'

# Production commands
build: ## Build all production containers
	docker compose -f docker-compose.yml -f docker-compose.prod.yml build

up: ## Start production containers with auto-migration
	docker compose -f docker-compose.yml -f docker-compose.prod.yml down -v
	docker compose -f docker-compose.yml -f docker-compose.prod.yml up -d

down: ## Stop and remove containers
	docker compose -f docker-compose.yml -f docker-compose.prod.yml down

clean: ## Stop containers and remove volumes
	docker compose -f docker-compose.yml -f docker-compose.prod.yml down -v
	docker compose -f docker-compose.yml -f docker-compose.override.yml down -v || true
	docker system prune -f

migrate: ## Run migrations manually
	docker compose run --rm backend-migrator

# Development commands
dev: ## Start development environment with file watching
	docker compose -f docker-compose.yml -f docker-compose.override.yml down -v
	docker compose -f docker-compose.yml -f docker-compose.override.yml up -d

dev-build: ## Build and start development environment
	docker compose -f docker-compose.yml -f docker-compose.override.yml build
	docker compose -f docker-compose.yml -f docker-compose.override.yml up -d

# Utility commands
logs: ## Show logs for all containers
	docker compose -f docker-compose.yml -f docker-compose.prod.yml logs -f

logs-%: ## Show logs for specific container (e.g., make logs-backend)
	docker compose -f docker-compose.yml -f docker-compose.prod.yml logs -f $*

status: ## Show status of all containers
	docker compose -f docker-compose.yml -f docker-compose.prod.yml ps

restart: ## Restart all containers
	docker compose -f docker-compose.yml -f docker-compose.prod.yml restart

restart-%: ## Restart specific container (e.g., make restart-backend)
	docker compose -f docker-compose.yml -f docker-compose.prod.yml restart $*

# Git hooks setup
setup-git-hooks: ## Setup git hooks for auto-rebuild
	chmod +x scripts/setup-git-hooks.sh
	./scripts/setup-git-hooks.sh
