---
description: "Use when implementing Recipe Organizer ASP.NET Core API endpoints, CRUD operations, filtering, pagination, or HTTP error handling."
---

# API layer

- These rules describe the planned server API; do not assume endpoints already exist.
- Keep endpoint handlers focused: bind DTOs, validate requests, perform the operation, and return a documented response. Follow the endpoint style chosen when the API is created.
- Expose recipe CRUD and read-only recipe-field history, plus ingredient and category management.
- Use request and response DTOs to avoid overposting and serialization of EF Core navigation properties.
- Return `201 Created` with a resource location for creation, `200 OK` for reads and updates with a body, and `204 No Content` for successful deletion.
- Return `400 Bad Request` with field-level validation details for invalid input, `404 Not Found` for missing target resources, and `409 Conflict` for duplicate names or deletion of referenced records.
- Return consistent Problem Details errors. Log unexpected server errors without exposing stack traces or connection details to clients.
- For recipe lists, accept name search, category and ingredient filters, and one-based `pageNumber` and `pageSize`. Default to page 1 and 10 items; reject page numbers below 1 or page sizes outside 1-100.
- Return a page DTO containing `items`, `totalCount`, `pageNumber`, and `pageSize`. Count after filtering but before pagination; an out-of-range page returns an empty item list with the actual total.
- Pass cancellation tokens to asynchronous EF Core operations. Save a recipe and its ingredient rows atomically.
- Translate known database constraint violations into the relevant client error; do not hide unrelated database failures.
