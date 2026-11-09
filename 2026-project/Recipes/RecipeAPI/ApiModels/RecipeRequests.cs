using System.ComponentModel.DataAnnotations;

namespace RecipeAPI.ApiModels;

/// <summary>Fields required to create or update a category.</summary>
public sealed record CategoryRequest
{
    private string? name;

    [Required, StringLength(200, MinimumLength = 1)]
    public required string Name { get => name!; init => name = value?.Trim(); }
}

/// <summary>Fields required to create or update an ingredient.</summary>
public sealed record IngredientRequest
{
    private string? name;

    [Required, StringLength(200, MinimumLength = 1)]
    public required string Name { get => name!; init => name = value?.Trim(); }
}

/// <summary>Fields required to create or update a recipe.</summary>
public sealed record RecipeRequest : IValidatableObject
{
    private string? name;
    private string? description;
    private string? instructions;

    [Required, StringLength(200, MinimumLength = 1)]
    public required string Name { get => name!; init => name = value?.Trim(); }

    [StringLength(2000)]
    public string? Description { get => description; init => description = value?.Trim(); }

    [Required, StringLength(10000, MinimumLength = 1)]
    public required string Instructions { get => instructions!; init => instructions = value?.Trim(); }

    [Range(0, int.MaxValue)]
    public int PreparationMinutes { get; init; }

    [Range(1, int.MaxValue)]
    public int Servings { get; init; }

    [Range(1, int.MaxValue)]
    public int CategoryId { get; init; }

    [Required, MinLength(1)]
    public required RecipeComponentRequest[] RecipeIngredients { get; init; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (RecipeIngredients is null)
            yield break;

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
                    "This ingredient is already included.",
                    [$"{memberName}.{nameof(RecipeComponentRequest.IngredientId)}"]);
            }
        }
    }
}

/// <summary>Ingredient quantity and unit attached to a recipe.</summary>
public sealed record RecipeComponentRequest
{
    private string? unit;

    [Range(1, int.MaxValue)]
    public int IngredientId { get; init; }

    [Range(typeof(decimal), "0", "9999999.999",
        MinimumIsExclusive = true, ParseLimitsInInvariantCulture = true)]
    public decimal Quantity { get; init; }

    [Required, StringLength(30, MinimumLength = 1)]
    public required string Unit { get => unit!; init => unit = value?.Trim(); }
}

/// <summary>Fields required to create a recipe-ingredient association.</summary>
public sealed record CreateRecipeIngredientRequest
{
    private string? unit;

    [Range(1, int.MaxValue)]
    public int RecipeId { get; init; }

    [Range(1, int.MaxValue)]
    public int IngredientId { get; init; }

    [Range(typeof(decimal), "0", "9999999.999",
        MinimumIsExclusive = true, ParseLimitsInInvariantCulture = true)]
    public decimal Quantity { get; init; }

    [Required, StringLength(30, MinimumLength = 1)]
    public required string Unit { get => unit!; init => unit = value?.Trim(); }
}

/// <summary>Fields that can be changed on a recipe-ingredient association.</summary>
public sealed record UpdateRecipeIngredientRequest
{
    private string? unit;

    [Range(typeof(decimal), "0", "9999999.999",
        MinimumIsExclusive = true, ParseLimitsInInvariantCulture = true)]
    public decimal Quantity { get; init; }

    [Required, StringLength(30, MinimumLength = 1)]
    public required string Unit { get => unit!; init => unit = value?.Trim(); }
}
