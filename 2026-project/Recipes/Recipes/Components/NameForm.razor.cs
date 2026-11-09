using Microsoft.AspNetCore.Components;
using Recipes.Models;

namespace Recipes.Components;

public partial class NameForm
{
    [Parameter] public string EntityName { get; set; } = "item";
    [Parameter] public NameFormModel? InitialValue { get; set; }
    [Parameter] public EventCallback<NameFormModel> OnSave { get; set; }
    [Parameter] public EventCallback OnCancel { get; set; }

    private NameFormModel Model { get; set; } = new();
    private bool IsEditing => InitialValue is not null;
    private string SubmitText => IsEditing ? $"Save {EntityName}" : $"Create {EntityName}";

    protected override void OnParametersSet() =>
        Model = new NameFormModel { Name = InitialValue?.Name ?? "" };

    private Task SaveAsync() => OnSave.InvokeAsync(Model);
    private Task CancelAsync() => OnCancel.InvokeAsync();
}
