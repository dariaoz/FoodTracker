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

- **FoodTracker.Domain** — entities (`Product`, `Recipe`, `FoodLog`, `Serving`, `ServingUnit`), domain interfaces (`IHaveMacronutrients`, `IHaveCalories`), and validators (`IValidator<T>`, `ProductValidator`, `RecipeValidator`, `FoodLogValidator`). No external dependencies.
- **FoodTracker.Application** — service interfaces and implementations (`ProductService`, `RecipeService`, `FoodLogService`), repository interfaces (`IRepository<T>`, `IProductRepository`, `IRecipeRepository`, `IFoodLogRepository`), and `INotionUnitOfWork`. Services call `_validator.Validate(entity)` before any write, then delegate to the repository through the unit of work.
- **FoodTracker.Infrastructure** — Notion API backend. `NotionClient` wraps the Notion REST API. Three repositories (`ProductRepository`, `RecipeRepository`, `FoodLogRepository`) use `INotionClient` + `IDistributedCache` (Redis, 15-min TTL). Mappers in `Notion/Mappers/` translate between Notion page properties and domain entities. `NotionUnitOfWork` exposes all three repositories. Exception handling middleware in `ExceptionHandling/` with a strategy pattern (`IExceptionHandlerStrategy`) — `ValidationExceptionStrategy` maps `ValidationException` to 400.
- **FoodTracker.Api** — ASP.NET Core Web API. Three controllers (`ProductsController`, `RecipesController`, `FoodLogController`) wired to application services via DI. `Models/` contains request models (`ProductRequest`, `RecipeRequest`, `FoodLogRequest`), response models (`ProductResponse`, `RecipeResponse`, `FoodLogResponse`), and `Mappings.cs` with extension methods to convert between them and domain entities.

## Configuration

`appsettings.json` requires:
- `Notion:ApiKey` — Notion integration token
- `Notion:ProductsDatabaseId`, `Notion:RecipesDatabaseId`, `Notion:FoodLogDatabaseId` — Notion database IDs
- `Redis:ConnectionString` — defaults to `localhost:6379`

## Documentation

All documentation and design notes are stored as `.md` files under `docs/`.

## Domain rules

- `FoodLog` links to either a `Product` or a `Recipe`, never both.
- `Product` `ServingUnit` must be `Gram` or `Milliliter` (not `Portion`).
- `Recipe` uses `Portion` as its serving unit.
- `ValidationException` carries a list of error strings and is the only domain exception type.
- "Delete" in this system archives the Notion page (`in_trash: true`) rather than hard-deleting it.
- **Macronutrients in `FoodLog` are a snapshot, intentionally.** Calories/protein/carbs/fat are computed from the linked `Product` or `Recipe` at write time and stored. If a product's macros are later corrected, historical log entries keep the original values — this is by design.