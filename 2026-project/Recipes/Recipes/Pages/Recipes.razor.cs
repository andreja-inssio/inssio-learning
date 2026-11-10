using Microsoft.AspNetCore.Components.QuickGrid;
using Microsoft.JSInterop;
using RecipeShared;
using Recipes.Models;
using Recipes.Services;

namespace Recipes.Pages;

public partial class Recipes
{
    private IReadOnlyList<CategoryDto>? categories;
    private IReadOnlyList<IngredientDto>? ingredients;
    private readonly PaginationState pagination = new() { ItemsPerPage = 10 };
    private readonly GridSort<RecipeGridRow> recipeNameSort = GridSort<RecipeGridRow>.ByAscending(row => row.Recipe.Name);
    private readonly GridSort<RecipeGridRow> preparationTimeSort = GridSort<RecipeGridRow>.ByAscending(row => row.Recipe.PreparationMinutes);
    private GridItemsProvider<RecipeGridRow>? recipeProvider;
    private QuickGrid<RecipeGridRow>? grid;
    private bool gridLoaded;
    private bool gridFailed;
    private bool gridEmpty;
    private int formVersion;
    private RecipeDto? selected;
    private string? error;
    private string? gridError;
    private string? success;

    protected override async Task OnInitializedAsync()
    {
        recipeProvider = LoadRecipesAsync;
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
        var categoriesTask = Api.GetCategoriesAsync();
        var ingredientsTask = Api.GetIngredientsAsync();
        await Task.WhenAll(categoriesTask, ingredientsTask);
        categories = await categoriesTask;
        ingredients = await ingredientsTask;
    }

    private async ValueTask<GridItemsProviderResult<RecipeGridRow>> LoadRecipesAsync(
        GridItemsProviderRequest<RecipeGridRow> request)
    {
        var sort = request.GetSortByProperties().FirstOrDefault().PropertyName switch
        {
            null => "id",
            "Recipe.Name" => "name",
            "Category" => "category",
            "Ingredients" => "ingredients",
            "Recipe.PreparationMinutes" => "time",
            "Recipe.Servings" => "servings",
            var property => throw new InvalidOperationException($"Unsupported grid sort property: {property}.")
        };
        try
        {
            var page = await Api.GetRecipePageAsync(request.StartIndex, request.Count ?? pagination.ItemsPerPage,
                sort, sort != "id" && !request.SortByAscending, request.CancellationToken);
            request.CancellationToken.ThrowIfCancellationRequested();
            var rows = page.Items.Select(recipe => new RecipeGridRow(
                recipe, CategoryName(recipe.CategoryId), IngredientSummary(recipe))).ToArray();
            var needsRender = !gridLoaded || gridFailed || gridEmpty != (page.TotalCount == 0);
            gridLoaded = true;
            gridFailed = false;
            gridEmpty = page.TotalCount == 0;
            gridError = null;
            if (needsRender)
                await InvokeAsync(StateHasChanged);
            return GridItemsProviderResult.From(rows, page.TotalCount);
        }
        catch (Exception exception) when (exception is RecipeApiException or HttpRequestException)
        {
            gridFailed = true;
            gridError = exception.Message;
            success = null;
            await InvokeAsync(StateHasChanged);
            return GridItemsProviderResult.From(Array.Empty<RecipeGridRow>(), pagination.TotalItemCount ?? 0);
        }
    }

    private Task RefreshGridAsync() => grid?.RefreshDataAsync() ?? Task.CompletedTask;

    private string CategoryName(int id) =>
        categories?.FirstOrDefault(category => category.Id == id)?.Name ?? "Unknown category";

    private string IngredientSummary(RecipeDto recipe) =>
        string.Join(", ", recipe.RecipeIngredients.OrderBy(row => row.IngredientId).Select(row =>
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
                formVersion++;
                success = "Recipe created.";
            }
            else
            {
                await Api.UpdateRecipeAsync(selected.Id, request);
                success = "Recipe updated.";
            }
            selected = null;
            error = null;
            await RefreshGridAsync();
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
            await RefreshGridAsync();
        }
        catch (Exception exception) when (exception is RecipeApiException or HttpRequestException)
        {
            error = exception.Message;
            success = null;
        }
    }

    private sealed record RecipeGridRow(RecipeDto Recipe, string Category, string Ingredients);
}
