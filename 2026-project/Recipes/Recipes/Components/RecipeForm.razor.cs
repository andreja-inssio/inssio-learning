using Microsoft.AspNetCore.Components;
using RecipeShared;
using Recipes.Models;

namespace Recipes.Components;

public partial class RecipeForm
{
    [Parameter, EditorRequired] public IReadOnlyList<CategoryDto> Categories { get; set; } = [];
    [Parameter, EditorRequired] public IReadOnlyList<IngredientDto> Ingredients { get; set; } = [];
    [Parameter] public RecipeDto? InitialValue { get; set; }
    [Parameter] public EventCallback<RecipeFormModel> OnSave { get; set; }
    [Parameter] public EventCallback OnCancel { get; set; }

    private RecipeFormModel Model { get; set; } = new();
    private string SubmitText => InitialValue is null ? "Create recipe" : "Save recipe";

    protected override void OnParametersSet()
    {
        Model = InitialValue is null
            ? new RecipeFormModel()
            : new RecipeFormModel
            {
                Name = InitialValue.Name ?? "",
                Description = InitialValue.Description,
                Instructions = InitialValue.Instructions ?? "",
                PreparationMinutes = InitialValue.PreparationMinutes,
                Servings = InitialValue.Servings,
                CategoryId = InitialValue.CategoryId,
                Ingredients = InitialValue.RecipeIngredients.Select(row => new RecipeIngredientFormModel
                {
                    IngredientId = row.IngredientId,
                    Quantity = row.Quantity,
                    Unit = row.Unit ?? ""
                }).ToList()
            };
    }

    private void AddIngredient() => Model.Ingredients.Add(new());
    private void RemoveIngredient(RecipeIngredientFormModel ingredient) => Model.Ingredients.Remove(ingredient);
    private Task SaveAsync() => OnSave.InvokeAsync(Model);
    private Task CancelAsync() => OnCancel.InvokeAsync();
}
