using Microsoft.EntityFrameworkCore;
using RecipeAPI.ApiModels;
using RecipeAPI.Data;
using RecipeAPI.Entities;
using RecipeShared;

namespace RecipeAPI.Services;

public enum OperationStatus
{
    Success,
    NotFound,
    Conflict,
    InvalidReference
}

public sealed record OperationResult<T>(OperationStatus Status, T? Value = null) where T : class;

public interface ICategoryService
{
    Task<IReadOnlyList<CategoryDto>> GetAllAsync(CancellationToken cancellationToken);
    Task<CategoryDto?> GetByIdAsync(int id, CancellationToken cancellationToken);
    Task<OperationResult<CategoryDto>> CreateAsync(CategoryRequest request, CancellationToken cancellationToken);
    Task<OperationResult<CategoryDto>> UpdateAsync(int id, CategoryRequest request, CancellationToken cancellationToken);
    Task<OperationStatus> DeleteAsync(int id, CancellationToken cancellationToken);
}

public interface IIngredientService
{
    Task<IReadOnlyList<IngredientDto>> GetAllAsync(CancellationToken cancellationToken);
    Task<IngredientDto?> GetByIdAsync(int id, CancellationToken cancellationToken);
    Task<OperationResult<IngredientDto>> CreateAsync(IngredientRequest request, CancellationToken cancellationToken);
    Task<OperationResult<IngredientDto>> UpdateAsync(int id, IngredientRequest request, CancellationToken cancellationToken);
    Task<OperationStatus> DeleteAsync(int id, CancellationToken cancellationToken);
}

public interface IRecipeService
{
    Task<IReadOnlyList<RecipeDto>> GetAllAsync(CancellationToken cancellationToken);
    Task<RecipeDto?> GetByIdAsync(int id, CancellationToken cancellationToken);
    Task<OperationResult<RecipeDto>> CreateAsync(RecipeRequest request, CancellationToken cancellationToken);
    Task<OperationResult<RecipeDto>> UpdateAsync(int id, RecipeRequest request, CancellationToken cancellationToken);
    Task<OperationStatus> DeleteAsync(int id, CancellationToken cancellationToken);
}

public interface IRecipeIngredientService
{
    Task<IReadOnlyList<RecipeIngredientDto>> GetAllAsync(CancellationToken cancellationToken);
    Task<RecipeIngredientDto?> GetByIdAsync(int recipeId, int ingredientId, CancellationToken cancellationToken);
    Task<OperationResult<RecipeIngredientDto>> CreateAsync(CreateRecipeIngredientRequest request, CancellationToken cancellationToken);
    Task<OperationResult<RecipeIngredientDto>> UpdateAsync(int recipeId, int ingredientId, UpdateRecipeIngredientRequest request, CancellationToken cancellationToken);
    Task<OperationStatus> DeleteAsync(int recipeId, int ingredientId, CancellationToken cancellationToken);
}

public sealed class CategoryService(RecipeDbContext db) : ICategoryService
{
    public async Task<IReadOnlyList<CategoryDto>> GetAllAsync(CancellationToken cancellationToken) =>
        await db.Categories.AsNoTracking().OrderBy(category => category.Id)
            .Select(category => new CategoryDto { Id = category.Id, Name = category.Name })
            .ToListAsync(cancellationToken);

    public async Task<CategoryDto?> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        var category = await db.Categories.AsNoTracking()
            .SingleOrDefaultAsync(item => item.Id == id, cancellationToken);
        return category is null ? null : ToDto(category);
    }

    public async Task<OperationResult<CategoryDto>> CreateAsync(
        CategoryRequest request, CancellationToken cancellationToken)
    {
        var name = request.Name.Trim();
        if (await db.Categories.AnyAsync(category => category.Name == name, cancellationToken))
            return new(OperationStatus.Conflict);

        var category = new Category { Name = name };
        db.Categories.Add(category);
        await db.SaveChangesAsync(cancellationToken);
        return new(OperationStatus.Success, ToDto(category));
    }

    public async Task<OperationResult<CategoryDto>> UpdateAsync(
        int id, CategoryRequest request, CancellationToken cancellationToken)
    {
        var category = await db.Categories.FindAsync([id], cancellationToken);
        if (category is null)
            return new(OperationStatus.NotFound);

        var name = request.Name.Trim();
        if (await db.Categories.AnyAsync(item => item.Id != id && item.Name == name, cancellationToken))
            return new(OperationStatus.Conflict);

        category.Name = name;
        await db.SaveChangesAsync(cancellationToken);
        return new(OperationStatus.Success, ToDto(category));
    }

    public async Task<OperationStatus> DeleteAsync(int id, CancellationToken cancellationToken)
    {
        var category = await db.Categories.FindAsync([id], cancellationToken);
        if (category is null)
            return OperationStatus.NotFound;
        if (await db.Recipes.AnyAsync(recipe => recipe.CategoryId == id, cancellationToken))
            return OperationStatus.Conflict;

        db.Categories.Remove(category);
        await db.SaveChangesAsync(cancellationToken);
        return OperationStatus.Success;
    }

    private static CategoryDto ToDto(Category category) =>
        new() { Id = category.Id, Name = category.Name };
}

public sealed class IngredientService(RecipeDbContext db) : IIngredientService
{
    public async Task<IReadOnlyList<IngredientDto>> GetAllAsync(CancellationToken cancellationToken) =>
        await db.Ingredients.AsNoTracking().OrderBy(ingredient => ingredient.Id)
            .Select(ingredient => new IngredientDto { Id = ingredient.Id, Name = ingredient.Name })
            .ToListAsync(cancellationToken);

    public async Task<IngredientDto?> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        var ingredient = await db.Ingredients.AsNoTracking()
            .SingleOrDefaultAsync(item => item.Id == id, cancellationToken);
        return ingredient is null ? null : ToDto(ingredient);
    }

    public async Task<OperationResult<IngredientDto>> CreateAsync(
        IngredientRequest request, CancellationToken cancellationToken)
    {
        var name = request.Name.Trim();
        if (await db.Ingredients.AnyAsync(ingredient => ingredient.Name == name, cancellationToken))
            return new(OperationStatus.Conflict);

        var ingredient = new Ingredient { Name = name };
        db.Ingredients.Add(ingredient);
        await db.SaveChangesAsync(cancellationToken);
        return new(OperationStatus.Success, ToDto(ingredient));
    }

    public async Task<OperationResult<IngredientDto>> UpdateAsync(
        int id, IngredientRequest request, CancellationToken cancellationToken)
    {
        var ingredient = await db.Ingredients.FindAsync([id], cancellationToken);
        if (ingredient is null)
            return new(OperationStatus.NotFound);

        var name = request.Name.Trim();
        if (await db.Ingredients.AnyAsync(item => item.Id != id && item.Name == name, cancellationToken))
            return new(OperationStatus.Conflict);

        ingredient.Name = name;
        await db.SaveChangesAsync(cancellationToken);
        return new(OperationStatus.Success, ToDto(ingredient));
    }

    public async Task<OperationStatus> DeleteAsync(int id, CancellationToken cancellationToken)
    {
        var ingredient = await db.Ingredients.FindAsync([id], cancellationToken);
        if (ingredient is null)
            return OperationStatus.NotFound;
        if (await db.RecipeIngredients.AnyAsync(row => row.IngredientId == id, cancellationToken))
            return OperationStatus.Conflict;

        db.Ingredients.Remove(ingredient);
        await db.SaveChangesAsync(cancellationToken);
        return OperationStatus.Success;
    }

    private static IngredientDto ToDto(Ingredient ingredient) =>
        new() { Id = ingredient.Id, Name = ingredient.Name };
}

public sealed class RecipeService(RecipeDbContext db) : IRecipeService
{
    public async Task<IReadOnlyList<RecipeDto>> GetAllAsync(CancellationToken cancellationToken)
    {
        var recipes = await db.Recipes.AsNoTracking()
            .Include(recipe => recipe.RecipeIngredients)
            .OrderBy(recipe => recipe.Id)
            .ToListAsync(cancellationToken);
        return recipes.Select(ToDto).ToArray();
    }

    public async Task<RecipeDto?> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        var recipe = await db.Recipes.AsNoTracking()
            .Include(item => item.RecipeIngredients)
            .SingleOrDefaultAsync(item => item.Id == id, cancellationToken);
        return recipe is null ? null : ToDto(recipe);
    }

    public async Task<OperationResult<RecipeDto>> CreateAsync(
        RecipeRequest request, CancellationToken cancellationToken)
    {
        if (!await ReferencesExistAsync(request, cancellationToken))
            return new(OperationStatus.InvalidReference);

        var recipe = new Recipe
        {
            Name = request.Name.Trim(),
            Description = request.Description?.Trim(),
            Instructions = request.Instructions.Trim(),
            PreparationMinutes = request.PreparationMinutes,
            Servings = request.Servings,
            CategoryId = request.CategoryId,
            RecipeIngredients = request.RecipeIngredients.Select(row => new RecipeIngredient
            {
                IngredientId = row.IngredientId,
                Quantity = row.Quantity,
                Unit = row.Unit.Trim()
            }).ToList()
        };
        db.Recipes.Add(recipe);
        await db.SaveChangesAsync(cancellationToken);
        return new(OperationStatus.Success, ToDto(recipe));
    }

    public async Task<OperationResult<RecipeDto>> UpdateAsync(
        int id, RecipeRequest request, CancellationToken cancellationToken)
    {
        var recipe = await db.Recipes.Include(item => item.RecipeIngredients)
            .SingleOrDefaultAsync(item => item.Id == id, cancellationToken);
        if (recipe is null)
            return new(OperationStatus.NotFound);
        if (!await ReferencesExistAsync(request, cancellationToken))
            return new(OperationStatus.InvalidReference);

        recipe.Name = request.Name.Trim();
        recipe.Description = request.Description?.Trim();
        recipe.Instructions = request.Instructions.Trim();
        recipe.PreparationMinutes = request.PreparationMinutes;
        recipe.Servings = request.Servings;
        recipe.CategoryId = request.CategoryId;
        db.RecipeIngredients.RemoveRange(recipe.RecipeIngredients);
        recipe.RecipeIngredients = request.RecipeIngredients.Select(row => new RecipeIngredient
        {
            RecipeId = id,
            IngredientId = row.IngredientId,
            Quantity = row.Quantity,
            Unit = row.Unit.Trim()
        }).ToList();
        await db.SaveChangesAsync(cancellationToken);
        return new(OperationStatus.Success, ToDto(recipe));
    }

    public async Task<OperationStatus> DeleteAsync(int id, CancellationToken cancellationToken)
    {
        var recipe = await db.Recipes.FindAsync([id], cancellationToken);
        if (recipe is null)
            return OperationStatus.NotFound;

        db.Recipes.Remove(recipe);
        await db.SaveChangesAsync(cancellationToken);
        return OperationStatus.Success;
    }

    private async Task<bool> ReferencesExistAsync(
        RecipeRequest request, CancellationToken cancellationToken)
    {
        if (!await db.Categories.AnyAsync(category => category.Id == request.CategoryId, cancellationToken))
            return false;

        var ingredientIds = request.RecipeIngredients.Select(row => row.IngredientId).Distinct().ToArray();
        return await db.Ingredients.CountAsync(ingredient => ingredientIds.Contains(ingredient.Id),
            cancellationToken) == ingredientIds.Length;
    }

    private static RecipeDto ToDto(Recipe recipe) =>
        new()
        {
            Id = recipe.Id,
            Name = recipe.Name,
            Description = recipe.Description,
            Instructions = recipe.Instructions,
            PreparationMinutes = recipe.PreparationMinutes,
            Servings = recipe.Servings,
            CategoryId = recipe.CategoryId,
            RecipeIngredients = recipe.RecipeIngredients.Select(row => new RecipeIngredientDto
            {
                RecipeId = row.RecipeId,
                IngredientId = row.IngredientId,
                Quantity = row.Quantity,
                Unit = row.Unit
            }).ToArray()
        };
}

public sealed class RecipeIngredientService(RecipeDbContext db) : IRecipeIngredientService
{
    public async Task<IReadOnlyList<RecipeIngredientDto>> GetAllAsync(CancellationToken cancellationToken)
    {
        var rows = await db.RecipeIngredients.AsNoTracking()
            .OrderBy(row => row.RecipeId).ThenBy(row => row.IngredientId)
            .ToListAsync(cancellationToken);
        return rows.Select(ToDto).ToArray();
    }

    public async Task<RecipeIngredientDto?> GetByIdAsync(
        int recipeId, int ingredientId, CancellationToken cancellationToken)
    {
        var row = await db.RecipeIngredients.AsNoTracking()
            .SingleOrDefaultAsync(item => item.RecipeId == recipeId && item.IngredientId == ingredientId,
                cancellationToken);
        return row is null ? null : ToDto(row);
    }

    public async Task<OperationResult<RecipeIngredientDto>> CreateAsync(
        CreateRecipeIngredientRequest request, CancellationToken cancellationToken)
    {
        if (!await db.Recipes.AnyAsync(recipe => recipe.Id == request.RecipeId, cancellationToken)
            || !await db.Ingredients.AnyAsync(ingredient => ingredient.Id == request.IngredientId, cancellationToken))
            return new(OperationStatus.InvalidReference);
        if (await db.RecipeIngredients.AnyAsync(row =>
                row.RecipeId == request.RecipeId && row.IngredientId == request.IngredientId,
                cancellationToken))
            return new(OperationStatus.Conflict);

        var row = new RecipeIngredient
        {
            RecipeId = request.RecipeId,
            IngredientId = request.IngredientId,
            Quantity = request.Quantity,
            Unit = request.Unit.Trim()
        };
        db.RecipeIngredients.Add(row);
        await db.SaveChangesAsync(cancellationToken);
        return new(OperationStatus.Success, ToDto(row));
    }

    public async Task<OperationResult<RecipeIngredientDto>> UpdateAsync(
        int recipeId, int ingredientId, UpdateRecipeIngredientRequest request,
        CancellationToken cancellationToken)
    {
        var row = await db.RecipeIngredients.FindAsync([recipeId, ingredientId], cancellationToken);
        if (row is null)
            return new(OperationStatus.NotFound);

        row.Quantity = request.Quantity;
        row.Unit = request.Unit.Trim();
        await db.SaveChangesAsync(cancellationToken);
        return new(OperationStatus.Success, ToDto(row));
    }

    public async Task<OperationStatus> DeleteAsync(
        int recipeId, int ingredientId, CancellationToken cancellationToken)
    {
        var row = await db.RecipeIngredients.FindAsync([recipeId, ingredientId], cancellationToken);
        if (row is null)
            return OperationStatus.NotFound;

        db.RecipeIngredients.Remove(row);
        await db.SaveChangesAsync(cancellationToken);
        return OperationStatus.Success;
    }

    private static RecipeIngredientDto ToDto(RecipeIngredient row) =>
        new()
        {
            RecipeId = row.RecipeId,
            IngredientId = row.IngredientId,
            Quantity = row.Quantity,
            Unit = row.Unit
        };
}
