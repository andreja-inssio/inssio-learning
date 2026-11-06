using System.ComponentModel.DataAnnotations;

namespace RecipeShared;

public record CategoryDto
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
