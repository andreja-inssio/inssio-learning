# RecipeAPI database

The API uses SQL Server LocalDB by default, with the `Recipes` connection string
in `appsettings.json`. Override `ConnectionStrings__Recipes` for another SQL
Server instance.

From the parent `Recipes` solution directory:

```powershell
dotnet tool restore
dotnet ef database update --project .\RecipeAPI
```

The `InitialCreate` migration creates the schema, including the temporal
`Recipes` table and its `RecipeHistory` table, and inserts 100 recipes,
30 ingredients, 10 categories, and 463 recipe-ingredient rows with quantities
and units (10 recipes per category).
Seed values and IDs are deterministic and managed with `HasData` in
`Data/RecipeSeed.cs`. Applying migrations again does not duplicate the data.
The API does not automatically create or migrate the database on startup.

After changing the model or seed data, generate and inspect a new migration:

```powershell
dotnet ef migrations add <MigrationName> --project .\RecipeAPI
dotnet ef database update --project .\RecipeAPI
```
