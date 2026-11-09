using Microsoft.JSInterop;
using RecipeShared;
using Recipes.Models;
using Recipes.Services;

namespace Recipes.Pages;

public partial class RecipeIngredients
{
    private IReadOnlyList<RecipeDto>? recipes;
    private IReadOnlyList<IngredientDto>? ingredients;
    private IReadOnlyList<RecipeIngredientDto>? links;
    private RecipeIngredientDto? selected;
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
        var loadedRecipes = Api.GetRecipesAsync();
        var loadedIngredients = Api.GetIngredientsAsync();
        var loadedLinks = Api.GetRecipeIngredientsAsync();
        await Task.WhenAll(loadedRecipes, loadedIngredients, loadedLinks);
        recipes = await loadedRecipes;
        ingredients = await loadedIngredients;
        links = await loadedLinks;
    }

    private string RecipeName(int id) =>
        recipes?.FirstOrDefault(recipe => recipe.Id == id)?.Name ?? $"Recipe #{id}";

    private string IngredientName(int id) =>
        ingredients?.FirstOrDefault(ingredient => ingredient.Id == id)?.Name ?? $"Ingredient #{id}";

    private void Edit(RecipeIngredientDto link)
    {
        selected = link;
        error = null;
        success = null;
    }

    private void CancelEdit()
    {
        selected = null;
        error = null;
    }

    private async Task SaveAsync(RecipeIngredientLinkFormModel model)
    {
        try
        {
            if (selected is null)
            {
                await Api.CreateRecipeIngredientAsync(model);
                success = "Ingredient added to the recipe.";
            }
            else
            {
                await Api.UpdateRecipeIngredientAsync(selected.RecipeId, selected.IngredientId,
                    new RecipeIngredientUpdateRequest(model.Quantity, model.Unit.Trim()));
                success = "Recipe ingredient updated.";
            }
            selected = null;
            error = null;
            await LoadAsync();
        }
        catch (Exception exception) when (exception is RecipeApiException or HttpRequestException)
        {
            error = exception.Message;
            success = null;
        }
    }

    private async Task DeleteAsync(RecipeIngredientDto link)
    {
        if (!await JS.InvokeAsync<bool>("confirm",
                $"Remove {IngredientName(link.IngredientId)} from {RecipeName(link.RecipeId)}?"))
            return;

        try
        {
            await Api.DeleteRecipeIngredientAsync(link.RecipeId, link.IngredientId);
            if (selected?.RecipeId == link.RecipeId && selected.IngredientId == link.IngredientId)
                selected = null;
            success = "Ingredient removed from the recipe.";
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
