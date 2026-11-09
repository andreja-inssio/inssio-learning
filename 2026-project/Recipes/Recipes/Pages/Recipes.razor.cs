using Microsoft.JSInterop;
using RecipeShared;
using Recipes.Models;
using Recipes.Services;

namespace Recipes.Pages;

public partial class Recipes
{
    private IReadOnlyList<RecipeDto>? recipes;
    private IReadOnlyList<CategoryDto>? categories;
    private IReadOnlyList<IngredientDto>? ingredients;
    private RecipeDto? selected;
    private string? error;
    private string? success;

    protected override async Task OnInitializedAsync()
    {
        try
        {
            await LoadAsync();
        }
        catch (Exception exception) when (exception is RecipeApiException or HttpRequestException)
        {
            error = exception.Message;
        }
    }

    private async Task LoadAsync()
    {
        var recipesTask = Api.GetRecipesAsync();
        var categoriesTask = Api.GetCategoriesAsync();
        var ingredientsTask = Api.GetIngredientsAsync();
        await Task.WhenAll(recipesTask, categoriesTask, ingredientsTask);
        recipes = await recipesTask;
        categories = await categoriesTask;
        ingredients = await ingredientsTask;
    }

    private string CategoryName(int id) =>
        categories?.FirstOrDefault(category => category.Id == id)?.Name ?? "Unknown category";

    private string IngredientSummary(RecipeDto recipe) =>
        string.Join(", ", recipe.RecipeIngredients.Select(row =>
        {
            var name = ingredients?.FirstOrDefault(item => item.Id == row.IngredientId)?.Name ?? "Unknown";
            return $"{name} ({row.Quantity} {row.Unit})";
        }));

    private void Edit(RecipeDto recipe)
    {
        selected = recipe;
        success = null;
        error = null;
    }

    private void CancelEdit()
    {
        selected = null;
        error = null;
    }

    private async Task SaveAsync(RecipeFormModel model)
    {
        var request = new RecipeWriteRequest(
            model.Name.Trim(),
            string.IsNullOrWhiteSpace(model.Description) ? null : model.Description.Trim(),
            model.Instructions.Trim(),
            model.PreparationMinutes,
            model.Servings,
            model.CategoryId,
            model.Ingredients.Select(row => new RecipeComponentWriteRequest(
                row.IngredientId, row.Quantity, row.Unit.Trim())).ToArray());
        try
        {
            if (selected is null)
            {
                await Api.CreateRecipeAsync(request);
                success = "Recipe created.";
            }
            else
            {
                await Api.UpdateRecipeAsync(selected.Id, request);
                success = "Recipe updated.";
            }
            selected = null;
            await LoadAsync();
            error = null;
        }
        catch (Exception exception) when (exception is RecipeApiException or HttpRequestException)
        {
            error = exception.Message;
            success = null;
        }
    }

    private async Task DeleteAsync(RecipeDto recipe)
    {
        if (!await JS.InvokeAsync<bool>("confirm", $"Delete recipe '{recipe.Name}'?"))
            return;

        try
        {
            await Api.DeleteRecipeAsync(recipe.Id);
            if (selected?.Id == recipe.Id)
                selected = null;
            success = "Recipe deleted.";
            error = null;
            await LoadAsync();
        }
        catch (Exception exception) when (exception is RecipeApiException or HttpRequestException)
        {
            error = exception.Message;
            success = null;
        }
    }
}
