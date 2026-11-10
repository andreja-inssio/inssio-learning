---
name: recipe-crud-feature
description: "Use when adding or extending a Recipe Organizer CRUD feature across EF Core persistence, ASP.NET Core Minimal API endpoints, shared contracts, and Blazor WebAssembly forms."
argument-hint: "Describe the entity or CRUD behavior to add or change."
---

# Implement a Recipe Organizer CRUD feature

Deliver the requested feature across the existing layers without introducing repositories, CQRS, or extra projects.

## 1. Establish scope

- Read the relevant [instructions](../../instructions/), especially architecture, validation, API, EF Core, and Blazor guidance.
- Inspect the current feature and working-tree changes before editing. Preserve unrelated work.
- Identify required fields, relationships, endpoints, UI actions, and failure cases. Ask about unresolved behavior that materially changes the design.
- Treat stale instructions as a discrepancy to resolve, not evidence that implemented code is missing. Do not silently replace the existing offset/limit paging contract with page-number paging.

## 2. Trace the existing implementation

Use the nearest existing feature as the pattern:

- Persistence: `RecipeAPI/Entities`, `RecipeAPI/Data/RecipeDbContext.cs`, and `RecipeAPI/Migrations`.
- Server contracts and logic: `RecipeAPI/ApiModels/RecipeRequests.cs`, `RecipeAPI/Services/RecipeServices.cs`, and `RecipeAPI/Endpoints`.
- Shared responses and limits: `RecipeShared`, including `RecipeValidationLimits`.
- Client integration: `Recipes/Services/RecipeApiClient.cs`, `Recipes/Pages`, and `Recipes/Components`.

Reuse these locations and existing helpers; do not create a parallel request/validation system. Follow the established request placement unless moving contracts is explicitly part of the task.

## 3. Implement the vertical slice

- Update only the necessary entities, EF configuration, contracts, service logic, endpoints, client methods, and UI.
- Keep EF entities and database access on the server. Apply common field rules consistently and enforce database-dependent rules server-side.
- Preserve foreign keys, duplicate prevention, decimal precision, and deletion rules. Save aggregate changes atomically.
- Keep temporal history limited to recipe fields unless expanding its scope is explicitly requested.
- Use the existing HTTP status and Problem Details patterns; expose useful validation messages without leaking internals.
- Preserve input on form errors, show pending/error/empty states, and prevent duplicate submissions.
- Generate and inspect a migration only for a schema change. Follow the existing local tool manifest and migration conventions; never apply migrations to an existing database without authorization.

## 4. Verify and report

- From the Recipes solution directory, run `dotnet build .\Recipes.slnx`.
- Inspect available test projects before choosing a test command. No automated test project exists at the time this skill was authored; do not report a build as a test.
- Cover the changed success path plus invalid input, missing references, duplicate rows/names, and deletion conflicts as applicable. Add focused automated coverage when test infrastructure is available.
- For database behavior, use SQL Server, not EF InMemory or SQLite, to validate temporal tables and SQL Server constraints. Use an isolated test database and clean up only resources created for the check.
- For UI changes, verify the form/list behavior in a browser when available. Otherwise state exactly what remains unverified.
- Update directly affected documentation. Summarize changed files, validation results, and any migration or setup steps still required.
