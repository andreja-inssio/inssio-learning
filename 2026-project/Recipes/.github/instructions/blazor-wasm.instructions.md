---
description: "Use when creating or editing Recipe Organizer Blazor WebAssembly pages, components, forms, navigation, or HTTP integration."
applyTo: "**/Recipes/**/*.razor,**/Recipes/**/*.razor.cs,**/Recipes/**/wwwroot/**/*.css"
---

# Blazor WebAssembly UI

- Keep routable pages in `Recipes/Pages` and layout components in `Recipes/Layout`, following the existing project structure.
- Provide recipe list, details, create/edit, recipe-field history, ingredient management, and category management pages. Use confirmation actions for deletion rather than dedicated pages.
- Use shared DTOs for HTTP communication, not EF Core entities. Keep HTTP calls out of markup; extract a focused client service when calls are reused.
- Use `EditForm`, `DataAnnotationsValidator`, field-level messages, and a validation summary. Display server-side validation errors as well as client-side errors.
- Allow users to add and remove ingredient rows, each with an ingredient selection, decimal quantity, and unit.
- Show loading, empty, not-found, and error states explicitly. Disable duplicate submissions while a save is pending.
- Request filtering and pagination from the server. Reset to the first page when a filter changes and keep filters when navigating between pages.
- Label inputs, use semantic buttons and links, and preserve keyboard accessibility.
- Label history as changes to recipe fields only; ingredient assignments and quantities are not versioned in the initial scope.
- Never put secrets in client configuration or treat client validation as a security boundary.
