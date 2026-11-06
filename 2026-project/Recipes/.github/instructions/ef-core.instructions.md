---
description: "Use when designing Recipe Organizer EF Core entities, relationships, SQL Server temporal tables, migrations, queries, or pagination."
---

# EF Core persistence

- Keep the EF Core model and configuration in the server project. Use a scoped `DbContext` and asynchronous database operations.
- Define `Recipe` with `Id`, `Name`, `Description`, `Instructions`, `PreparationMinutes`, `Servings`, and `CategoryId`.
- Define `Ingredient` and `Category` with `Id` and `Name`.
- Define `RecipeIngredient` with `RecipeId`, `IngredientId`, decimal `Quantity`, and `Unit`. Use `(RecipeId, IngredientId)` as its composite primary key and configure decimal precision explicitly.
- Configure category-to-recipes as one-to-many and recipes-to-ingredients as many-to-many through `RecipeIngredient`.
- Cascade deletion of a recipe to its join rows. Restrict deletion of ingredients and categories that are referenced by recipes.
- Configure only `Recipe` as a SQL Server temporal table initially. Use temporal queries for historical recipe fields, not for a purported complete historical recipe aggregate.
- Project temporal queries to scalar recipe-field DTOs; do not expand non-temporal category or ingredient navigation properties.
- Preserve UTC temporal boundaries in history responses and order versions deterministically. History is read-only; do not add restore functionality unless requested.
- Use `AsNoTracking()` and DTO projections for read-only queries. Do not introduce lazy loading or unnecessary `Include` calls.
- Apply filters first, count matching records, then order by `Name` and `Id` before `Skip` and `Take`. Validate pagination and guard offset arithmetic against overflow.
- Enforce required fields, string lengths, decimal precision, foreign keys, and name uniqueness through database configuration consistent with the validation rules.
- Use parameterized queries. Generate and inspect migrations for schema changes; do not substitute `EnsureCreated()` for migration management.
- Verify temporal queries and database constraints against SQL Server. EF Core InMemory and SQLite cannot validate SQL Server temporal behavior.
