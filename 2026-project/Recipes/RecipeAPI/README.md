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

The Recipes page requests ten recipes at a time. Sorting and pagination are
performed by the API and database, not on the full list in the browser.
`GET /api/recipes/page` accepts zero-based `offset`, `limit` (1-100, default
10), `sort` (`id`, `name`, `category`, `ingredients`, `time`, or `servings`)
and `descending` (boolean, default `false`). It returns `items` and
`totalCount`, and uses recipe ID to break sort ties. The existing
`GET /api/recipes` remains available for clients needing the complete list.

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

## Recipe history

Open `/recipes/{id}/history` in the Blazor app, or use the History link in the
recipe list. The page shows recipe fields and UTC validity periods, including
current and deleted versions. Ingredient rows and category names are not
historical; only the category ID stored on the recipe is shown.

`GET /api/recipes/{id}/history?mode=all` supports:

| Mode | EF Core operator | Parameters and behavior |
|---|---|---|
| `all` (default) | `TemporalAll` | All versions, oldest first |
| `asof` | `TemporalAsOf` | `from`: version valid at that instant |
| `fromto` | `TemporalFromTo` | `from`, `to`: overlapping versions, excluding versions starting exactly at `to` |
| `between` | `TemporalBetween` | Like FromTo, but includes versions starting exactly at `to` |
| `containedin` | `TemporalContainedIn` | Versions starting and ending inside the inclusive bounds |

Use ISO 8601 timestamps with `Z` or an explicit offset (URL-encode `+` as `%2B`).
The page's InputDate date/time fields are interpreted as UTC, not browser-local
time. AsOf uses the start field; range searches require start before end.
Invalid modes/ranges return `400`; a recipe with no temporal records returns
`404`; a valid search with no matching versions returns `200` and `[]`.
Malformed date values return `400`. SQL Server excludes zero-duration versions.

To deliberately generate twenty additional history versions for one existing
recipe, start the API and run from the solution directory:

```powershell
.\RecipeAPI\Scripts\Add-RecipeHistory.ps1 -RecipeId 1
```

This makes twenty separate PUT requests, incrementing preparation minutes by
one each time and preserving the other recipe data. It changes the selected
recipe permanently and is never run automatically on startup. Re-running adds
another twenty versions; do not run it while someone else is editing that recipe.
The script checks the version count increased by exactly twenty.
