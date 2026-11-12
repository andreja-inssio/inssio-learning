using Microsoft.EntityFrameworkCore;
using RecipeAPI.Data;
using RecipeShared;

namespace RecipeAPI.Services;

public interface IRecipeReportService
{
    Task<IReadOnlyList<ShoppingListItemDto>?> GetShoppingListAsync(
        IReadOnlyCollection<int> recipeIds, CancellationToken cancellationToken);
    Task<IReadOnlyList<CategoryStatisticsDto>> GetCategoryStatisticsAsync(CancellationToken cancellationToken);
    Task<IReadOnlyList<IngredientUsageDto>> GetIngredientUsageAsync(CancellationToken cancellationToken);
}

public sealed class RecipeReportService(RecipeDbContext db) : IRecipeReportService
{
    public async Task<IReadOnlyList<ShoppingListItemDto>?> GetShoppingListAsync(
        IReadOnlyCollection<int> recipeIds, CancellationToken cancellationToken)
    {
        var ids = recipeIds.Distinct().ToArray();
        if (await db.Recipes.CountAsync(recipe => ids.Contains(recipe.Id), cancellationToken) != ids.Length)
            return null;

        return await db.Recipes.AsNoTracking()
            .Where(recipe => ids.Contains(recipe.Id))
            .SelectMany(recipe => recipe.RecipeIngredients)
            .GroupBy(row => new { row.IngredientId, row.Ingredient!.Name, row.Unit })
            .OrderBy(group => group.Key.Name).ThenBy(group => group.Key.IngredientId).ThenBy(group => group.Key.Unit)
            .Select(group => new ShoppingListItemDto(
                group.Key.IngredientId, group.Key.Name, group.Sum(row => row.Quantity), group.Key.Unit))
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<CategoryStatisticsDto>> GetCategoryStatisticsAsync(
        CancellationToken cancellationToken) =>
        await db.Recipes.AsNoTracking()
            .GroupBy(recipe => new { recipe.CategoryId, recipe.Category!.Name })
            .OrderBy(group => group.Key.Name).ThenBy(group => group.Key.CategoryId)
            .Select(group => new CategoryStatisticsDto(
                group.Key.CategoryId, group.Key.Name, group.Count(),
                group.Average(recipe => recipe.PreparationMinutes)))
            .ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<IngredientUsageDto>> GetIngredientUsageAsync(
        CancellationToken cancellationToken) =>
        await db.Ingredients.AsNoTracking()
            .GroupJoin(db.RecipeIngredients,
                ingredient => ingredient.Id,
                row => row.IngredientId,
                (ingredient, rows) => new { ingredient, rows })
            .SelectMany(match => match.rows.DefaultIfEmpty(),
                (match, row) => new
                {
                    IngredientId = match.ingredient.Id,
                    IngredientName = match.ingredient.Name,
                    RecipeId = row == null ? (int?)null : row.RecipeId,
                    RecipeName = row == null ? null : row.Recipe!.Name
                })
            .OrderBy(item => item.IngredientName).ThenBy(item => item.IngredientId).ThenBy(item => item.RecipeId)
            .Select(item => new IngredientUsageDto(
                item.IngredientId, item.IngredientName, item.RecipeId, item.RecipeName))
            .ToListAsync(cancellationToken);
}
