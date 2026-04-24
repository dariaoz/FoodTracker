# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Commands

```bash
# Build the solution
dotnet build

# Run the API
dotnet run --project src/FoodTracker.Api

# Run all tests
dotnet test
```

## Architecture

Clean Architecture with four projects:

Each project is organized by feature folder (`Products/`, `Recipes/`, `FoodLogs/`) plus a `Shared/` folder for cross-cutting types. Within each feature folder, all related files live together (entity + validator, service + interface + repository, controller + request/response/mappings).

- **FoodTracker.Domain** — feature folders: `Products/` (`Product`, `ProductValidator`), `Recipes/` (`Recipe`, `RecipeValidator`), `FoodLogs/` (`FoodLog`, `FoodLogValidator`). `Shared/` holds value types (`Serving`, `ServingUnit`, `MacroSnapshot`), interfaces (`IHaveMacronutrients`, `IHaveCalories`, `IHaveId`, `IMacroSource`), and validation primitives (`IValidator<T>`, `ValidationException`). No external dependencies.
- **FoodTracker.Application** — feature folders: `Products/` (`ProductService`, `Interfaces/IProductRepository`, `Interfaces/IProductService`), `Recipes/` (`RecipeService`, `Interfaces/IRecipeRepository`, `Interfaces/IRecipeService`), `FoodLogs/` (`FoodLogService`, `Interfaces/IFoodLogRepository`, `Interfaces/IFoodLogService`). `Shared/` holds `IRepository<T>` and `INotionContext`. Services call `_validator.Validate(entity)` before any write, then delegate to repositories through the context.
- **FoodTracker.Infrastructure** — Notion API backend. Feature folders: `Products/`, `Recipes/`, `FoodLogs/` each contain a base repository, cached repository decorator, and Notion mapper. `Notion/` holds `INotionClient`, `NotionClient`, `NotionModels`, `NotionPropertyHelper`, and `NotionContext`. `Shared/` holds `CachedRepository<T>`. Exception handling middleware in `ExceptionHandling/` with a strategy pattern (`IExceptionHandlerStrategy`) — `ValidationExceptionStrategy` maps `ValidationException` to 400.
- **FoodTracker.Api** — ASP.NET Core Web API. Controllers live in `Controllers/` (one per feature). Models live in `Models/Products/`, `Models/Recipes/`, `Models/FoodLogs/` — each containing request, response, and mapping extension methods. Controllers expose only `GET`, `POST`, and `DELETE` — no `PUT`. All controller action methods use the `Async` suffix; `SuppressAsyncSuffixInActionNames = false` is set so `CreatedAtAction(nameof(...))` resolves correctly.

## Configuration

`appsettings.json` holds non-sensitive defaults:
- `Notion:BaseAddress` — Notion API base URL (`https://api.notion.com/v1/`)
- `Notion:NotionVersion` — Notion API version header value (`2026-03-11`)
- `Cache:TtlMinutes` — Redis cache TTL in minutes, defaults to 15

Sensitive values are stored in user secrets (`dotnet user-secrets`):
- `Notion:ApiKey` — Notion integration token
- `Notion:ProductsDatabaseId`, `Notion:RecipesDatabaseId`, `Notion:FoodLogDatabaseId` — Notion database IDs
- `Redis:ConnectionString` — defaults to `localhost:6379`

## Notion API integration

`NotionClient` uses a **facade pattern** for database operations. Callers pass a `databaseId`; internally two steps run:
1. `GET /databases/{databaseId}` — resolves the `data_source_id` from the `data_sources` array. Result is cached in Redis for **7 days** under key `datasource:{databaseId}`.
2. `POST /data_sources/{dataSourceId}/query` — actual row retrieval.

Create uses `POST /pages` with `parent: { type: "data_source_id", data_source_id: "…" }`. Delete uses `PATCH /pages/{pageId}` with `in_trash: true`. Both also resolve the `data_source_id` first (cache-warmed).

### Notion property name mapping

| Field | Products column | Recipes column | Food Log column |
|-------|----------------|----------------|-----------------|
| Name | `Name` (title) | `Name` (title) | `Name` (title) |
| ServingUnit | `Serving Unit` (select, options `gram`/`milliliter` lowercase) | `Serving Unit` (select, `Gram`/`Milliliter`/`Portion`) | `Serving Unit` (select, `Gram`/`Milliliter`/`Portion`) |
| Protein | `Protein` | `Protein (g)` | `Proteins (g)` |
| Carbs | `Carbs` | `Carbs (g)` | `Carbonates (g)` |
| Fat | `Fat` | `Fat (g)` | `Fat (g)` |
| Quantity | — | — | `Serving` |
| RecipeId | — | — | `Recipe` (**relation**) |
| ProductId | — | — | `Product` (**relation**) |

`FoodLog.Product` and `FoodLog.Recipe` are Notion **relation** properties (not rich text). Macros are written to the Food Log row at create time and read back directly — no linked-page fetches on read.

`ServingUnit` is annotated with `[JsonConverter(typeof(JsonStringEnumConverter))]` so it serializes as a string in all API responses.

## Documentation

All documentation and design notes are stored as `.md` files under `docs/`.

## Domain rules

- `FoodLog` links to either a `Product` or a `Recipe`, never both.
- `Product` `ServingUnit` must be `Gram` or `Milliliter` (not `Portion`).
- `Recipe` `ServingUnit` can be `Gram`, `Milliliter`, or `Portion`. Its `Serving` property returns a default quantity of 100 for g/ml and 1 for portion.
- `FoodLog.Quantity` is always supplied by the caller — no default is applied at the domain level.
- `FoodLog.ServingUnit` must match the `ServingUnit` of the linked `Product` or `Recipe` — enforced in `FoodLogService.CreateAsync`.
- `FoodLog` macronutrients are computed on the backend at write time via `IMacroSource.ComputeMacros(quantity)` — callers do not supply macro values. `FoodLogRequest` has no macro fields.
- `ValidationException` carries a list of error strings and is the only domain exception type.
- "Delete" in this system moves the Notion page to trash (`in_trash: true`) rather than hard-deleting it.
- **Macronutrients in `FoodLog` are a snapshot, intentionally.** Calories/protein/carbs/fat are computed from the linked `Product` or `Recipe` at write time and stored. If a product's macros are later corrected, historical log entries keep the original values — this is by design.
