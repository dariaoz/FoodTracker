# FoodTracker

REST API for tracking daily food intake. Products and recipes are stored in Notion databases; food log entries link to them and snapshot macronutrients at write time.

## Stack

- **ASP.NET Core 9** — Web API
- **Notion API** (`2026-03-11`) — persistence backend (source of truth)
- **Elasticsearch** — read backend (all GET requests served from ES)
- **Redis** — caches Notion data-source ID mappings (7-day TTL)
- **Docker Compose** — local development

## Prerequisites

- .NET 9 SDK
- Redis (or Docker)
- Elasticsearch (or Docker)
- Notion integration token and three Notion databases: **Products**, **Recipes**, **Food Log**

## Getting started

### Local (without Docker)

1. Start Redis and Elasticsearch:
   ```bash
   docker run -p 6379:6379 redis:7-alpine
   docker run -p 9200:9200 -e "discovery.type=single-node" elasticsearch:8.13.0
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

### Elasticsearch index

All reads go to Elasticsearch; Notion is the write-only source of truth. On startup (non-Development environments), the app automatically triggers a full reindex from Notion into ES — no manual step needed after deploy.

In Development (local debugging) the startup reindex is skipped. If the ES index is empty locally, populate it once with:

```bash
curl -X POST http://localhost:5000/admin/reindex
```

The same endpoint can be called manually at any time to force a full resync.

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
| GET | `/foodlog` | List all entries (optional `?from=yyyy-MM-dd&to=yyyy-MM-dd` filter) |
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
