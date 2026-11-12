using RecipeShared;
using Recipes.Services;

namespace Recipes.Pages;

public partial class Reports
{
    private readonly CancellationTokenSource lifetime = new();
    private CancellationToken cancellationToken;
    private readonly HashSet<int> selectedIds = [];
    private IReadOnlyList<RecipeDto> recipes = [];
    private IReadOnlyList<CategoryStatisticsDto> statistics = [];
    private IReadOnlyList<ShoppingListItemDto>? shoppingList;
    private string? loadError;
    private string? shoppingError;
    private bool loading;
    private bool generating;

    protected override Task OnInitializedAsync()
    {
        cancellationToken = lifetime.Token;
        return LoadAsync();
    }

    private async Task LoadAsync()
    {
        loading = true;
        loadError = null;
        try
        {
            var recipesTask = Api.GetRecipesAsync(cancellationToken);
            var statisticsTask = Api.GetCategoryStatisticsAsync(cancellationToken);
            await Task.WhenAll(recipesTask, statisticsTask);
            cancellationToken.ThrowIfCancellationRequested();
            recipes = (await recipesTask).OrderBy(recipe => recipe.Name).ThenBy(recipe => recipe.Id).ToArray();
            statistics = await statisticsTask;
        }
        catch (OperationCanceledException) when (lifetime.IsCancellationRequested)
        {
            // Navigation cancelled the pending request.
        }
        catch (Exception exception) when (exception is RecipeApiException or HttpRequestException)
        {
            loadError = exception.Message;
        }
        finally
        {
            loading = false;
        }
    }

    private void ToggleRecipe(int id)
    {
        if (!selectedIds.Remove(id))
            selectedIds.Add(id);
        shoppingList = null;
        shoppingError = null;
    }

    private async Task GenerateAsync()
    {
        generating = true;
        shoppingList = null;
        shoppingError = null;
        try
        {
            shoppingList = await Api.GetShoppingListAsync(selectedIds, cancellationToken);
        }
        catch (OperationCanceledException) when (lifetime.IsCancellationRequested)
        {
            // Navigation cancelled the pending request.
        }
        catch (Exception exception) when (exception is RecipeApiException or HttpRequestException)
        {
            shoppingError = exception.Message;
        }
        finally
        {
            generating = false;
        }
    }

    public void Dispose()
    {
        lifetime.Cancel();
        lifetime.Dispose();
    }
}
