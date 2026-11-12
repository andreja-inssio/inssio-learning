using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Recipes.Models;
using RecipeShared;

namespace Recipes.Services;

public sealed class RecipeApiClient(HttpClient http)
{
    public Task<IReadOnlyList<CategoryDto>> GetCategoriesAsync(CancellationToken cancellationToken = default) =>
        GetAsync<IReadOnlyList<CategoryDto>>("api/categories", cancellationToken);

    public Task<CategoryDto> CreateCategoryAsync(NameFormModelRequest request, CancellationToken cancellationToken = default) =>
        SendAsync<CategoryDto>(HttpMethod.Post, "api/categories", request, cancellationToken);

    public Task<CategoryDto> UpdateCategoryAsync(int id, NameFormModelRequest request, CancellationToken cancellationToken = default) =>
        SendAsync<CategoryDto>(HttpMethod.Put, $"api/categories/{id}", request, cancellationToken);

    public Task DeleteCategoryAsync(int id, CancellationToken cancellationToken = default) =>
        DeleteAsync($"api/categories/{id}", cancellationToken);

    public Task<IReadOnlyList<IngredientDto>> GetIngredientsAsync(CancellationToken cancellationToken = default) =>
        GetAsync<IReadOnlyList<IngredientDto>>("api/ingredients", cancellationToken);

    public Task<IngredientDto> CreateIngredientAsync(NameFormModelRequest request, CancellationToken cancellationToken = default) =>
        SendAsync<IngredientDto>(HttpMethod.Post, "api/ingredients", request, cancellationToken);

    public Task<IngredientDto> UpdateIngredientAsync(int id, NameFormModelRequest request, CancellationToken cancellationToken = default) =>
        SendAsync<IngredientDto>(HttpMethod.Put, $"api/ingredients/{id}", request, cancellationToken);

    public Task DeleteIngredientAsync(int id, CancellationToken cancellationToken = default) =>
        DeleteAsync($"api/ingredients/{id}", cancellationToken);

    public Task<IReadOnlyList<RecipeDto>> GetRecipesAsync(CancellationToken cancellationToken = default) =>
        GetAsync<IReadOnlyList<RecipeDto>>("api/recipes", cancellationToken);

    public Task<IReadOnlyList<ShoppingListItemDto>> GetShoppingListAsync(
        IEnumerable<int> recipeIds, CancellationToken cancellationToken = default) =>
        GetAsync<IReadOnlyList<ShoppingListItemDto>>(
            "api/reports/shopping-list?" + string.Join("&", recipeIds.Distinct().Order().Select(id => $"recipeIds={id}")),
            cancellationToken);

    public Task<IReadOnlyList<CategoryStatisticsDto>> GetCategoryStatisticsAsync(CancellationToken cancellationToken = default) =>
        GetAsync<IReadOnlyList<CategoryStatisticsDto>>("api/reports/category-statistics", cancellationToken);

    public Task<IReadOnlyList<IngredientUsageDto>> GetIngredientUsageAsync(CancellationToken cancellationToken = default) =>
        GetAsync<IReadOnlyList<IngredientUsageDto>>("api/reports/ingredient-usage", cancellationToken);

    public Task<RecipePageResponse> GetRecipePageAsync(
        int offset, int limit, string sort, bool descending, CancellationToken cancellationToken = default) =>
        GetAsync<RecipePageResponse>(
            $"api/recipes/page?offset={offset}&limit={limit}&sort={Uri.EscapeDataString(sort)}&descending={descending.ToString().ToLowerInvariant()}",
            cancellationToken);

    public Task<RecipeDto> CreateRecipeAsync(RecipeWriteRequest request, CancellationToken cancellationToken = default) =>
        SendAsync<RecipeDto>(HttpMethod.Post, "api/recipes", request, cancellationToken);

    public Task<IReadOnlyList<RecipeHistoryDto>> GetRecipeHistoryAsync(
        int id, string mode, DateTimeOffset from, DateTimeOffset to, CancellationToken cancellationToken = default) =>
        GetAsync<IReadOnlyList<RecipeHistoryDto>>(
            $"api/recipes/{id}/history?mode={Uri.EscapeDataString(mode)}"
            + $"&from={Uri.EscapeDataString(from.ToString("O", System.Globalization.CultureInfo.InvariantCulture))}"
            + $"&to={Uri.EscapeDataString(to.ToString("O", System.Globalization.CultureInfo.InvariantCulture))}",
            cancellationToken);

    public Task<RecipeDto> UpdateRecipeAsync(int id, RecipeWriteRequest request, CancellationToken cancellationToken = default) =>
        SendAsync<RecipeDto>(HttpMethod.Put, $"api/recipes/{id}", request, cancellationToken);

    public Task DeleteRecipeAsync(int id, CancellationToken cancellationToken = default) =>
        DeleteAsync($"api/recipes/{id}", cancellationToken);

    public Task<IReadOnlyList<RecipeIngredientDto>> GetRecipeIngredientsAsync(CancellationToken cancellationToken = default) =>
        GetAsync<IReadOnlyList<RecipeIngredientDto>>("api/recipe-ingredients", cancellationToken);

    public Task<RecipeIngredientDto> CreateRecipeIngredientAsync(
        RecipeIngredientLinkFormModel request, CancellationToken cancellationToken = default) =>
        SendAsync<RecipeIngredientDto>(HttpMethod.Post, "api/recipe-ingredients", request, cancellationToken);

    public Task<RecipeIngredientDto> UpdateRecipeIngredientAsync(
        int recipeId, int ingredientId, RecipeIngredientUpdateRequest request,
        CancellationToken cancellationToken = default) =>
        SendAsync<RecipeIngredientDto>(HttpMethod.Put,
            $"api/recipe-ingredients/{recipeId}/{ingredientId}", request, cancellationToken);

    public Task DeleteRecipeIngredientAsync(
        int recipeId, int ingredientId, CancellationToken cancellationToken = default) =>
        DeleteAsync($"api/recipe-ingredients/{recipeId}/{ingredientId}", cancellationToken);

    private async Task<T> GetAsync<T>(string uri, CancellationToken cancellationToken)
    {
        using var response = await http.GetAsync(uri, cancellationToken);
        await EnsureSuccessAsync(response, cancellationToken);
        return await response.Content.ReadFromJsonAsync<T>(cancellationToken)
            ?? throw new InvalidOperationException($"The API returned an empty response for {uri}.");
    }

    private async Task<T> SendAsync<T>(
        HttpMethod method, string uri, object request, CancellationToken cancellationToken)
    {
        using var message = new HttpRequestMessage(method, uri)
        {
            Content = JsonContent.Create(request)
        };
        using var response = await http.SendAsync(message, cancellationToken);
        await EnsureSuccessAsync(response, cancellationToken);
        return await response.Content.ReadFromJsonAsync<T>(cancellationToken)
            ?? throw new InvalidOperationException($"The API returned an empty response for {uri}.");
    }

    private async Task DeleteAsync(string uri, CancellationToken cancellationToken)
    {
        using var response = await http.DeleteAsync(uri, cancellationToken);
        await EnsureSuccessAsync(response, cancellationToken);
    }

    private static async Task EnsureSuccessAsync(
        HttpResponseMessage response, CancellationToken cancellationToken)
    {
        if (response.IsSuccessStatusCode)
            return;

        var message = $"API request failed with {(int)response.StatusCode} {response.ReasonPhrase}.";
        var body = await response.Content.ReadAsStringAsync(cancellationToken);
        if (!string.IsNullOrWhiteSpace(body))
        {
            try
            {
                using var document = JsonDocument.Parse(body);
                var root = document.RootElement;
                var detail = root.TryGetProperty("detail", out var detailValue)
                    ? detailValue.GetString()
                    : null;
                var title = root.TryGetProperty("title", out var titleValue)
                    ? titleValue.GetString()
                    : null;
                if (!string.IsNullOrWhiteSpace(detail))
                    message = detail;
                else if (!string.IsNullOrWhiteSpace(title))
                    message = title;
            }
            catch (JsonException)
            {
                message = $"API request failed with {(int)response.StatusCode} {response.ReasonPhrase}.";
            }
        }

        throw new RecipeApiException(response.StatusCode, message);
    }
}

public sealed class RecipeApiException(HttpStatusCode statusCode, string message) : Exception(message)
{
    public HttpStatusCode StatusCode { get; } = statusCode;
}

public sealed record NameFormModelRequest(string Name);

public sealed record RecipeWriteRequest(
    string Name,
    string? Description,
    string Instructions,
    int PreparationMinutes,
    int Servings,
    int CategoryId,
    IReadOnlyList<RecipeComponentWriteRequest> RecipeIngredients);

public sealed record RecipeComponentWriteRequest(int IngredientId, decimal Quantity, string Unit);

public sealed record RecipeIngredientUpdateRequest(decimal Quantity, string Unit);
