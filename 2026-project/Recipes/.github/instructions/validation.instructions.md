---
description: "Use when implementing Recipe Organizer form validation, shared request DTOs, API validation, or database constraints."
---

# Validation rules

- Apply the same input rules to create and update operations. Use shared DTO validation for field rules, server-side checks for database-dependent rules, and database constraints for integrity.
- Trim names and units before validating or persisting them. Reject whitespace-only required text.
- Require a recipe name, instructions, an existing category, and at least one ingredient row. Description is optional.
- Require preparation minutes to be zero or greater and servings to be greater than zero.
- Require every ingredient row to reference an existing ingredient, have a quantity greater than zero, and include a nonblank unit.
- Reject duplicate ingredient IDs within a recipe; return an actionable error instead of silently merging rows or dropping duplicates.
- Require ingredient and category names to be unique within their respective lists, ignoring case and surrounding whitespace. Recipe names need not be unique.
- Align SQL Server collation or normalized unique-key configuration with the case-insensitive name rule. Application prechecks alone cannot prevent concurrent duplicates.
- Define explicit string length limits in shared validation and mirror them in EF Core configuration before implementing forms and schema. Do not use inconsistent client and server limits.
- Reject deletion of referenced ingredients or categories with an explanation of what must be changed first.
- Validate pagination and filters on the server, including positive supplied IDs. A valid filter with no matches should return an empty result, not an error.
- Associate errors with the relevant fields or ingredient rows and preserve entered values after validation failures.
- Test boundary values, whitespace input, duplicate rows and names, missing references, and attempts to bypass browser validation.
