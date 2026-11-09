using System.ComponentModel.DataAnnotations;

namespace Recipes.Models;

public sealed class NameFormModel
{
    [Required, StringLength(200, MinimumLength = 1)]
    public string Name { get; set; } = "";
}

public sealed class RecipeFormModel : IValidatableObject
{
    [Required, StringLength(200, MinimumLength = 1)]
    public string Name { get; set; } = "";

    [StringLength(2000)]
    public string? Description { get; set; }

    [Required, StringLength(10000, MinimumLength = 1)]
    public string Instructions { get; set; } = "";

    [Range(0, int.MaxValue)]
    public int PreparationMinutes { get; set; }

    [Range(1, int.MaxValue)]
    public int Servings { get; set; } = 1;

    [Range(1, int.MaxValue)]
    public int CategoryId { get; set; }

    [MinLength(1, ErrorMessage = "Add at least one ingredient.")]
    public List<RecipeIngredientFormModel> Ingredients { get; set; } = [new()];

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        var ingredientIds = new HashSet<int>();
        for (var index = 0; index < Ingredients.Count; index++)
        {
            var ingredient = Ingredients[index];
            var member = $"{nameof(Ingredients)}[{index}]";

            if (ingredient.IngredientId < 1)
                yield return new ValidationResult("Select an ingredient.", [$"{member}.{nameof(ingredient.IngredientId)}"]);
            if (ingredient.Quantity <= 0)
                yield return new ValidationResult("Quantity must be greater than zero.", [$"{member}.{nameof(ingredient.Quantity)}"]);
            if (string.IsNullOrWhiteSpace(ingredient.Unit) || ingredient.Unit.Trim().Length > 30)
                yield return new ValidationResult("Enter a unit (up to 30 characters).", [$"{member}.{nameof(ingredient.Unit)}"]);
            if (!ingredientIds.Add(ingredient.IngredientId))
                yield return new ValidationResult("Each ingredient can only be added once.", [$"{member}.{nameof(ingredient.IngredientId)}"]);
        }
    }
}

public sealed class RecipeIngredientFormModel
{
    public int IngredientId { get; set; }

    [Range(typeof(decimal), "0", "9999999.999",
        MinimumIsExclusive = true, ParseLimitsInInvariantCulture = true)]
    public decimal Quantity { get; set; } = 1;

    [Required, StringLength(30, MinimumLength = 1)]
    public string Unit { get; set; } = "";
}

public sealed class RecipeIngredientLinkFormModel
{
    [Range(1, int.MaxValue, ErrorMessage = "Select a recipe.")]
    public int RecipeId { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Select an ingredient.")]
    public int IngredientId { get; set; }

    [Range(typeof(decimal), "0", "9999999.999",
        MinimumIsExclusive = true, ParseLimitsInInvariantCulture = true)]
    public decimal Quantity { get; set; } = 1;

    [Required, StringLength(30, MinimumLength = 1)]
    public string Unit { get; set; } = "";
}
