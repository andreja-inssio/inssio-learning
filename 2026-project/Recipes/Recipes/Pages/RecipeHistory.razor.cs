using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Recipes.Services;
using RecipeShared;

namespace Recipes.Pages;

public partial class RecipeHistory
{
    [Parameter] public int Id { get; set; }

    private readonly HistorySearch search = new();
    private CancellationTokenSource? pending;
    private IReadOnlyList<RecipeHistoryDto>? versions;
    private string? error;
    private bool busy;

    protected override Task OnParametersSetAsync() => LoadAsync();

    private Task SearchAsync(EditContext context) =>
        context.Validate() ? LoadAsync() : Task.CompletedTask;

    private async Task LoadAsync()
    {
        pending?.Cancel();
        pending?.Dispose();
        var request = new CancellationTokenSource();
        var cancellationToken = request.Token;
        pending = request;
        busy = true;
        error = null;
        versions = null;
        try
        {
            if (search.Mode is "fromto" or "between" or "containedin" && search.From >= search.To)
            {
                error = "End must be later than start.";
                return;
            }
            var result = await Api.GetRecipeHistoryAsync(Id, search.Mode,
                new DateTimeOffset(DateTime.SpecifyKind(search.From, DateTimeKind.Utc)),
                new DateTimeOffset(DateTime.SpecifyKind(search.To, DateTimeKind.Utc)), cancellationToken);
            cancellationToken.ThrowIfCancellationRequested();
            versions = result;
        }
        catch (OperationCanceledException) when (request.IsCancellationRequested)
        {
            // Navigation or a newer search superseded this request.
        }
        catch (Exception exception) when (exception is RecipeApiException or HttpRequestException)
        {
            if (!request.IsCancellationRequested)
                error = exception.Message;
        }
        finally
        {
            if (ReferenceEquals(pending, request))
                busy = false;
        }
    }

    public void Dispose()
    {
        pending?.Cancel();
        pending?.Dispose();
    }

    private sealed class HistorySearch
    {
        public string Mode { get; set; } = "all";
        public DateTime From { get; set; } = DateTime.UtcNow.AddDays(-1);
        public DateTime To { get; set; } = DateTime.UtcNow.AddDays(1);
    }
}
