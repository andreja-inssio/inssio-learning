using System.ComponentModel.DataAnnotations;

namespace RecipeShared;

public record RecipeIngredientDto
{
    private string? unit;

    [Range(1, int.MaxValue)]
    public int RecipeId { get; init; }

    [Range(1, int.MaxValue)]
    public int IngredientId { get; init; }

    [Range(typeof(decimal), "0", "9999999.999",
        MinimumIsExclusive = true, ParseLimitsInInvariantCulture = true)]
    public decimal Quantity { get; init; }

    [Required]
    [StringLength(RecipeValidationLimits.UnitMaxLength)]
    public required string? Unit
    {
        get => unit;
        init => unit = value?.Trim();
    }
}
