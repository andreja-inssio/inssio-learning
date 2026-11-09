using System.ComponentModel.DataAnnotations;

namespace RecipeShared;

/// <summary>Represents a category returned by the recipes API.</summary>
public sealed record CategoryDto
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
}
