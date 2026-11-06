using System.ComponentModel.DataAnnotations;

namespace RecipeShared;

public record RecipeDto : IValidatableObject
{
    private string? name;

    [Range(1, int.MaxValue)]
    public int Id { get; init; }

    [Required]
    [StringLength(RecipeValidationLimits.NameMaxLength)]
    public required string? Name
    {
        get => name;
        init => name = value?.Trim();
    }

    [StringLength(RecipeValidationLimits.DescriptionMaxLength)]
    public string? Description { get; init; }

    [Required]
    [StringLength(RecipeValidationLimits.InstructionsMaxLength)]
    public required string? Instructions { get; init; }

    [Range(0, int.MaxValue)]
    public int PreparationMinutes { get; init; }

    [Range(1, int.MaxValue)]
    public int Servings { get; init; }

    [Range(1, int.MaxValue)]
    public int CategoryId { get; init; }

    [Required]
    [MinLength(1, ErrorMessage = "A recipe must contain at least one ingredient.")]
    public RecipeIngredientDto[] RecipeIngredients { get; init; } = [];

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (RecipeIngredients is null)
        {
            yield break;
        }

        var ingredientIds = new HashSet<int>();

        for (var index = 0; index < RecipeIngredients.Length; index++)
        {
            var ingredient = RecipeIngredients[index];
            var memberName = $"{nameof(RecipeIngredients)}[{index}]";

            if (ingredient is null)
            {
                yield return new ValidationResult("An ingredient row is required.", [memberName]);
                continue;
            }

            var results = new List<ValidationResult>();
            Validator.TryValidateObject(ingredient,
                new ValidationContext(ingredient, validationContext, validationContext.Items),
                results, validateAllProperties: true);

            foreach (var result in results)
            {
                yield return new ValidationResult(result.ErrorMessage,
                    result.MemberNames.Select(member => $"{memberName}.{member}"));
            }

            if (!ingredientIds.Add(ingredient.IngredientId))
            {
                yield return new ValidationResult(
                    "This ingredient is already included. Remove the duplicate ingredient row.",
                    [$"{memberName}.{nameof(RecipeIngredientDto.IngredientId)}"]);
            }
        }
    }
}
