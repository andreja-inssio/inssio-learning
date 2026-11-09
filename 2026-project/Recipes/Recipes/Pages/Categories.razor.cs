using Microsoft.JSInterop;
using RecipeShared;
using Recipes.Models;
using Recipes.Services;

namespace Recipes.Pages;

public partial class Categories
{
    private IReadOnlyList<CategoryDto>? categories;
    private CategoryDto? selected;
    private string? error;
    private string? success;

    protected override Task OnInitializedAsync() => LoadAsync();

    private async Task LoadAsync()
    {
        try
        {
            categories = await Api.GetCategoriesAsync();
            error = null;
        }
        catch (Exception exception) when (exception is RecipeApiException or HttpRequestException)
        {
            error = exception.Message;
        }
    }

    private void Edit(CategoryDto category)
    {
        selected = category;
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
                await Api.CreateCategoryAsync(new(model.Name.Trim()));
                success = "Category created.";
            }
            else
            {
                await Api.UpdateCategoryAsync(selected.Id, new(model.Name.Trim()));
                success = "Category updated.";
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

    private async Task DeleteAsync(CategoryDto category)
    {
        if (!await JS.InvokeAsync<bool>("confirm", $"Delete category '{category.Name}'?"))
            return;

        try
        {
            await Api.DeleteCategoryAsync(category.Id);
            success = "Category deleted.";
            error = null;
            if (selected?.Id == category.Id)
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
