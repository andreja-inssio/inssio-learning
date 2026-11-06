---
description: "Use when planning or changing Recipe Organizer architecture, project boundaries, dependencies, or end-to-end features."
---

# Recipe Organizer architecture

- Build a simple, single-user recipe organizer. Do not introduce authentication, an AI service, or enterprise architecture unless requested.
- The existing `Recipes` project is a standalone Blazor WebAssembly client. The API, shared contracts, and database layers are planned, not yet implemented.
- When adding the backend, use `Recipes.Api` for ASP.NET Core endpoints and EF Core data access, and `Recipes.Shared` for request and response DTOs plus shared input validation.
- Keep dependencies one-way: the client and API may reference shared contracts; shared contracts must not reference the client, API, or EF Core.
- Keep EF Core entities, `DbContext`, migrations, connection strings, and database credentials on the server. Never access the database directly from WASM.
- Use SQL Server for EF Core temporal-table support.
- Model recipes, ingredients, categories, and an explicit recipe-ingredient join entity. A recipe has one category and many ingredients.
- Keep the API and data access in one server project initially. Do not add generic repositories, CQRS, mediators, or additional layers without a concrete need.
- Implement features across DTOs, API, persistence, and UI together; update related validation and tests when behavior changes.
