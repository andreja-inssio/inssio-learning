using Microsoft.EntityFrameworkCore;
using RecipeAPI.Data;
using RecipeShared;

namespace RecipeAPI.Services;

public interface IRecipeHistoryService
{
    Task<IReadOnlyList<RecipeHistoryDto>?> SearchAsync(
        int id, string mode, DateTimeOffset? from, DateTimeOffset? to, CancellationToken cancellationToken);
}

public sealed class RecipeHistoryService(RecipeDbContext db) : IRecipeHistoryService
{
    public async Task<IReadOnlyList<RecipeHistoryDto>?> SearchAsync(
        int id, string mode, DateTimeOffset? from, DateTimeOffset? to, CancellationToken cancellationToken)
    {
        if (!await db.Recipes.TemporalAll().AnyAsync(recipe => recipe.Id == id, cancellationToken))
            return null;

        var query = mode switch
        {
            "all" => db.Recipes.TemporalAll(),
            "asof" => db.Recipes.TemporalAsOf(from!.Value.UtcDateTime),
            "fromto" => db.Recipes.TemporalFromTo(from!.Value.UtcDateTime, to!.Value.UtcDateTime),
            "between" => db.Recipes.TemporalBetween(from!.Value.UtcDateTime, to!.Value.UtcDateTime),
            "containedin" => db.Recipes.TemporalContainedIn(from!.Value.UtcDateTime, to!.Value.UtcDateTime),
            _ => throw new ArgumentOutOfRangeException(nameof(mode), mode, "Unsupported temporal mode.")
        };
        var versions = await query.AsNoTracking().Where(recipe => recipe.Id == id)
            .OrderBy(recipe => EF.Property<DateTime>(recipe, "PeriodStart"))
            .ThenBy(recipe => EF.Property<DateTime>(recipe, "PeriodEnd"))
            .Select(recipe => new
            {
                recipe.Id, recipe.Name, recipe.Description, recipe.Instructions,
                recipe.PreparationMinutes, recipe.Servings, recipe.CategoryId,
                Start = EF.Property<DateTime>(recipe, "PeriodStart"),
                End = EF.Property<DateTime>(recipe, "PeriodEnd")
            })
            .ToListAsync(cancellationToken);

        return versions.Select(version => new RecipeHistoryDto(
            version.Id, version.Name, version.Description, version.Instructions,
            version.PreparationMinutes, version.Servings, version.CategoryId,
            new DateTimeOffset(DateTime.SpecifyKind(version.Start, DateTimeKind.Utc)),
            new DateTimeOffset(DateTime.SpecifyKind(version.End, DateTimeKind.Utc)))).ToArray();
    }
}
