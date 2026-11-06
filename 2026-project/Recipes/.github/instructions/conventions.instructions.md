---
description: "Use when writing Recipe Organizer C# code, shared contracts, Razor code-behind, or automated tests."
applyTo: "**/Recipes/**/*.cs,**/Recipes.Api/**/*.cs,**/Recipes.Shared/**/*.cs"
---

# Coding conventions

- Follow the existing nullable-enabled C# project style: file-scoped namespaces, implicit usings, and namespaces matching project and folder names.
- Use PascalCase for types, public members, and methods; camelCase for parameters and local variables. Suffix asynchronous methods with `Async`.
- Use descriptive entity and DTO names, such as `Recipe`, `RecipeIngredient`, `CreateRecipeRequest`, and `RecipeDetailsDto`.
- Keep nullability accurate. Validate external input rather than suppressing warnings with null-forgiving operators.
- Prefer straightforward, small methods and constructor injection. Reuse shared validation and query helpers instead of copying logic.
- Use decimal values for ingredient quantities, integer values for servings and preparation minutes, and UTC timestamps for temporal boundaries.
- Do not add dependencies, abstractions, or comments unless they solve a concrete problem.
- Add focused tests for changed behavior, especially validation, recipe-ingredient updates, paging boundaries, and history responses.
- Use the project's existing test framework when available. Do not invent build or test commands for projects that have not been created.
