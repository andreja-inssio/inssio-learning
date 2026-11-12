using System.Globalization;
using Microsoft.AspNetCore.Http.HttpResults;
using RecipeAPI.Services;
using RecipeShared;

namespace RecipeAPI.Endpoints;

public static class RecipeHistoryEndpoints
{
    public static IEndpointRouteBuilder MapRecipeHistoryEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("/api/recipes/{id:int}/history",
            async Task<Results<Ok<IReadOnlyList<RecipeHistoryDto>>, NotFound, ValidationProblem>> (
                int id, string? mode, string? from, string? to,
                IRecipeHistoryService service, CancellationToken cancellationToken) =>
            {
                mode ??= "all";
                var errors = new Dictionary<string, string[]>();
                DateTimeOffset? start = null;
                DateTimeOffset? end = null;
                if (from is not null)
                {
                    if (DateTimeOffset.TryParse(from, CultureInfo.InvariantCulture,
                        DateTimeStyles.AssumeUniversal, out var parsed))
                        start = parsed;
                    else
                        errors["from"] = ["Start must be a valid ISO 8601 timestamp."];
                }
                if (to is not null)
                {
                    if (DateTimeOffset.TryParse(to, CultureInfo.InvariantCulture,
                        DateTimeStyles.AssumeUniversal, out var parsed))
                        end = parsed;
                    else
                        errors["to"] = ["End must be a valid ISO 8601 timestamp."];
                }
                if (id < 1)
                    errors["id"] = ["Recipe ID must be positive."];
                if (mode is not ("all" or "asof" or "fromto" or "between" or "containedin"))
                    errors["mode"] = ["Mode must be all, asof, fromto, between, or containedin."];
                if (mode != "all" && from is null)
                    errors["from"] = ["A start or as-of timestamp is required."];
                if (mode is "fromto" or "between" or "containedin")
                {
                    if (to is null)
                        errors["to"] = ["An end timestamp is required."];
                    else if (start is not null && end is not null && start >= end)
                        errors["to"] = ["End must be later than start."];
                }
                if (errors.Count > 0)
                    return TypedResults.ValidationProblem(errors);

                var versions = await service.SearchAsync(id, mode, start, end, cancellationToken);
                return versions is null ? TypedResults.NotFound() : TypedResults.Ok(versions);
            })
            .WithTags("Recipes")
            .WithName("GetRecipeHistory")
            .WithSummary("Search recipe-field history")
            .WithDescription("Modes: all, asof (from), fromto, between, containedin (from/to). "
                + "Timestamps use ISO 8601 offsets and are converted to UTC. Includes deleted recipes. "
                + "FromTo overlaps [from,to); Between also includes versions starting at to; "
                + "ContainedIn returns versions wholly inside the range. Related entities are not versioned.")
            .ProducesValidationProblem();
        return endpoints;
    }
}
