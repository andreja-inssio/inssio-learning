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

## Running the Blazor app

Start both the `RecipeAPI` and `Recipes` projects in Visual Studio (configure
multiple startup projects), using matching launch profiles. The HTTP profile
uses the API at `http://localhost:5085`; the HTTPS profile uses
`https://localhost:7172`. Trust the local development certificate for HTTPS
using `dotnet dev-certs https --trust` if the browser reports a certificate
error. The API does not redirect HTTP to HTTPS in Development so the HTTP
client profile also works when the API's HTTPS profile is running.

The client URLs are configured in `Recipes/wwwroot/appsettings.json`. For
deployment, set `ApiBaseUrl` to the actual HTTPS API address and configure
`AllowedOrigins` on the API with the deployed client origin. Production keeps
HTTPS redirection enabled.

## CRUD endpoints

The API exposes `GET` collection and item routes, `POST` create routes, `PUT`
replacement routes, and `DELETE` routes for all four entities:

| Entity | Collection route | Item route |
|---|---|---|
| Categories | `/api/categories` | `/api/categories/{id}` |
| Ingredients | `/api/ingredients` | `/api/ingredients/{id}` |
| Recipes | `/api/recipes` | `/api/recipes/{id}` |
| Recipe ingredients | `/api/recipe-ingredients` | `/api/recipe-ingredients/{recipeId}/{ingredientId}` |

Creates return `201 Created` with a `Location` header. Deletes return `204 No
Content`; missing resources return `404`. Deleting a category or ingredient
that is still referenced returns `409 Conflict`. Recipe creation and
replacement include the category ID and ingredient quantities in the request.
See `RecipeAPI.http` for example requests for every operation.

After changing the model or seed data, generate and inspect a new migration:

```powershell
dotnet ef migrations add <MigrationName> --project .\RecipeAPI
dotnet ef database update --project .\RecipeAPI
```
