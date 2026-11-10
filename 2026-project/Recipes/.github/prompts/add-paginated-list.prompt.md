---
name: add-paginated-list
description: "Add or extend a Recipe Organizer server-paginated, filtered list using EF Core, Minimal APIs, and Blazor QuickGrid."
agent: agent
argument-hint: "Name the list and desired filters; defaults to recipes filtered by name, category, and ingredient."
---

# Add a paginated, filtered list

Implement the list requested in the chat. Default to extending the existing recipe list with name, category, and ingredient filters, not creating a second recipe list.

Follow the [API](../instructions/api.instructions.md), [EF Core](../instructions/ef-core.instructions.md), [Blazor](../instructions/blazor-wasm.instructions.md), and [validation](../instructions/validation.instructions.md) instructions.

Inspect the existing endpoint, service, shared response, `RecipeApiClient`, and QuickGrid page before editing. The recipe list currently uses `GET /api/recipes/page` with `offset`, `limit`, `sort`, and `descending`, plus `RecipePageResponse` containing `Items` and `TotalCount`. Confirm the current code rather than relying on this description alone.

The instructions prescribe a different page-number contract. Preserve the current public contract for this task; if satisfying another requirement requires changing it, explain the conflict and obtain approval first.

## Acceptance criteria

- Filter on the server before computing `TotalCount` and fetching a page. Ingredient filtering must not duplicate recipes.
- Keep the existing server-side sorting and QuickGrid integration. Allow only supported sort fields and use an ID tie-breaker for stable paging.
- Validate offset/limit and supplied filter IDs. Retain the current paging defaults and maximum, reject invalid values, and guard arithmetic overflow.
- Return the filtered total and an empty item list when the requested offset is beyond the result set.
- Use asynchronous EF Core operations, cancellation, read-only queries, and DTO projection.
- Reset paging when filters change; retain filters during paging and sorting. Prevent stale responses from replacing newer results.
- Show loading, empty, and error states without disguising failures as empty results.
- Verify first/last/out-of-range pages, invalid input, combined filters, tied sort values, and filter changes from a later page. Use existing test infrastructure if present; clearly identify manual checks and unverified cases.

Build `Recipes.slnx` from its directory, update the affected API documentation/request examples, and summarize the changes and verification results. Do not add a new entity, migration, or dependency unless the requested list actually needs one.
