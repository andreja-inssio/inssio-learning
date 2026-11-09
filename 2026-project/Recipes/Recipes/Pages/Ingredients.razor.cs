using Microsoft.JSInterop;
using RecipeShared;
using Recipes.Models;
using Recipes.Services;

namespace Recipes.Pages;

public partial class Ingredients
{
    private IReadOnlyList<IngredientDto>? ingredients;
    private IngredientDto? selected;
    private string? error;
    private string? success;

    protected override Task OnInitializedAsync() => LoadAsync();

    private async Task LoadAsync()
    {
        try
        {
            ingredients = await Api.GetIngredientsAsync();
            error = null;
        }
        catch (Exception exception) when (exception is RecipeApiException or HttpRequestException)
        {
            error = exception.Message;
        }
    }

    private void Edit(IngredientDto ingredient)
    {
        selected = ingredient;
        success = null;
        error = null;
    }

    private void CancelEdit()
    {
        selected = null;
        error = null;
    }

    private async Task SaveAsync(NameFormModel model)
    {
        try
        {
            if (selected is null)
            {
                await Api.CreateIngredientAsync(new(model.Name.Trim()));
                success = "Ingredient created.";
            }
            else
            {
                await Api.UpdateIngredientAsync(selected.Id, new(model.Name.Trim()));
                success = "Ingredient updated.";
            }
            selected = null;
            await LoadAsync();
        }
        catch (Exception exception) when (exception is RecipeApiException or HttpRequestException)
        {
            error = exception.Message;
            success = null;
        }
    }

    private async Task DeleteAsync(IngredientDto ingredient)
    {
        if (!await JS.InvokeAsync<bool>("confirm", $"Delete ingredient '{ingredient.Name}'?"))
            return;

        try
        {
            await Api.DeleteIngredientAsync(ingredient.Id);
            success = "Ingredient deleted.";
            error = null;
            if (selected?.Id == ingredient.Id)
                selected = null;
            await LoadAsync();
        }
        catch (Exception exception) when (exception is RecipeApiException or HttpRequestException)
        {
            error = exception.Message;
            success = null;
        }
    }
}
