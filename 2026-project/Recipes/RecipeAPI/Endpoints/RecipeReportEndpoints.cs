using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using RecipeAPI.Services;
using RecipeShared;

namespace RecipeAPI.Endpoints;

public static class RecipeReportEndpoints
{
    public static RouteGroupBuilder MapRecipeReportEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/reports").WithTags("Reports");

        group.MapGet("/shopping-list",
            async Task<Results<Ok<IReadOnlyList<ShoppingListItemDto>>, ValidationProblem>> (
                [FromQuery] string[]? recipeIds, IRecipeReportService service, CancellationToken cancellationToken) =>
            {
                var ids = new HashSet<int>();
                foreach (var value in recipeIds ?? [])
                {
                    if (!int.TryParse(value, out var id) || id < 1)
                        return TypedResults.ValidationProblem(new Dictionary<string, string[]>
                        {
                            ["recipeIds"] = ["Recipe IDs must be positive integers."]
                        });
                    ids.Add(id);
                }
                if (ids.Count == 0)
                    return TypedResults.ValidationProblem(new Dictionary<string, string[]>
                    {
                        ["recipeIds"] = ["Select at least one recipe."]
                    });

                var items = await service.GetShoppingListAsync(ids, cancellationToken);
                return items is null
                    ? TypedResults.ValidationProblem(new Dictionary<string, string[]>
                    {
                        ["recipeIds"] = ["One or more selected recipes no longer exist."]
                    })
                    : TypedResults.Ok(items);
            })
            .WithName("GetShoppingList").WithSummary("Combine ingredients for selected recipes")
            .WithDescription("Repeat recipeIds for each selection. Duplicate IDs count once. "
                + "SelectMany flattens ingredient rows; GroupBy sums quantities by ingredient and unit without unit conversion.")
            .ProducesValidationProblem();

        group.MapGet("/category-statistics", async Task<Ok<IReadOnlyList<CategoryStatisticsDto>>> (
            IRecipeReportService service, CancellationToken cancellationToken) =>
                TypedResults.Ok(await service.GetCategoryStatisticsAsync(cancellationToken)))
            .WithName("GetCategoryStatistics").WithSummary("Count recipes and average preparation time by category")
            .WithDescription("Uses GroupBy. Categories without recipes are omitted.");

        group.MapGet("/ingredient-usage", async Task<Ok<IReadOnlyList<IngredientUsageDto>>> (
            IRecipeReportService service, CancellationToken cancellationToken) =>
                TypedResults.Ok(await service.GetIngredientUsageAsync(cancellationToken)))
            .WithName("GetIngredientUsage").WithSummary("List recipe assignments including unused ingredients")
            .WithDescription("Uses GroupJoin with SelectMany and DefaultIfEmpty for a left join. "
                + "Unused ingredients have null recipeId and recipeName.");

        return group;
    }
}
