using Microsoft.AspNetCore.Components;
using RecipeShared;
using Recipes.Models;

namespace Recipes.Components;

public partial class RecipeIngredientForm
{
    [Parameter, EditorRequired] public IReadOnlyList<RecipeDto> Recipes { get; set; } = [];
    [Parameter, EditorRequired] public IReadOnlyList<IngredientDto> Ingredients { get; set; } = [];
    [Parameter] public RecipeIngredientDto? InitialValue { get; set; }
    [Parameter] public EventCallback<RecipeIngredientLinkFormModel> OnSave { get; set; }
    [Parameter] public EventCallback OnCancel { get; set; }

    private RecipeIngredientLinkFormModel Model { get; set; } = new();

    protected override void OnParametersSet() =>
        Model = InitialValue is null
            ? new RecipeIngredientLinkFormModel()
            : new RecipeIngredientLinkFormModel
            {
                RecipeId = InitialValue.RecipeId,
                IngredientId = InitialValue.IngredientId,
                Quantity = InitialValue.Quantity,
                Unit = InitialValue.Unit ?? ""
            };

    private Task SaveAsync() => OnSave.InvokeAsync(Model);
    private Task CancelAsync() => OnCancel.InvokeAsync();
}
