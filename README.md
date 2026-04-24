# FoodTracker

REST API for tracking daily food intake. Products and recipes are stored in Notion databases; food log entries link to them and snapshot macronutrients at write time.

## Stack

- **ASP.NET Core 9** — Web API
- **Notion API** (`2026-03-11`) — persistence backend
- **Redis** — response caching (15 min TTL for entities, 7 days for Notion data-source ID mappings)
- **Docker Compose** — local development

## Prerequisites

- .NET 9 SDK
- Redis (or Docker)
- Notion integration token and three Notion databases: **Products**, **Recipes**, **Food Log**

## Getting started

### Local (without Docker)

1. Start Redis:
   ```bash
   docker run -p 6379:6379 redis:7-alpine
   ```

2. Set user secrets:
   ```bash
   cd src/FoodTracker.Api
   dotnet user-secrets set "Notion:ApiKey"             "<token>"
   dotnet user-secrets set "Notion:ProductsDatabaseId" "<id>"
   dotnet user-secrets set "Notion:RecipesDatabaseId"  "<id>"
   dotnet user-secrets set "Notion:FoodLogDatabaseId"  "<id>"
   ```

3. Run:
   ```bash
   dotnet run --project src/FoodTracker.Api
   ```

Swagger UI is available at `http://localhost:5000/swagger`.

### Docker Compose

Create a `.env` file in the repo root:

```env
NOTION_API_KEY=<token>
NOTION_PRODUCTS_DB_ID=<id>
NOTION_RECIPES_DB_ID=<id>
NOTION_FOODLOG_DB_ID=<id>
```

Then:

```bash
docker compose up --build
```

API is exposed on port `8080`.

## API

All endpoints return `400` for validation errors and `404` when an entity is not found. `ServingUnit` is returned as a string (`"Gram"`, `"Milliliter"`, `"Portion"`).

### Products — `GET|POST|DELETE /products`

| Method | Path | Description |
|--------|------|-------------|
| GET | `/products` | List all products |
| GET | `/products/{id}` | Get product by ID |
| POST | `/products` | Create product |
| DELETE | `/products/{id}` | Delete product |

`ServingUnit` must be `Gram` or `Milliliter` (not `Portion`).

### Recipes — `GET|POST|DELETE /recipes`

| Method | Path | Description |
|--------|------|-------------|
| GET | `/recipes` | List all recipes |
| GET | `/recipes/{id}` | Get recipe by ID |
| POST | `/recipes` | Create recipe |
| DELETE | `/recipes/{id}` | Delete recipe |

### Food Log — `GET|POST|DELETE /foodlog`

| Method | Path | Description |
|--------|------|-------------|
| GET | `/foodlog` | List all entries (optional `?date=yyyy-MM-dd` filter) |
| GET | `/foodlog/{id}` | Get entry by ID |
| POST | `/foodlog` | Create entry |
| DELETE | `/foodlog/{id}` | Delete entry |

A food log entry links to either a product **or** a recipe (never both). Macronutrients are computed from the linked source at write time and stored as a snapshot — editing a product's macros later does not affect existing log entries.

## Project structure

```
src/
  FoodTracker.Domain          # Entities, validators, value types — no dependencies
  FoodTracker.Application     # Services, repository interfaces
  FoodTracker.Infrastructure  # Notion client, repositories, Redis cache, mappers
  FoodTracker.Api             # Controllers, request/response models
tests/                        # Test projects
```

## Development

```bash
dotnet build        # Build
dotnet test         # Run tests
```
