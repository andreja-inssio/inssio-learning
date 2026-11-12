namespace RecipeShared;

/// <summary>A version of recipe fields and its UTC validity period; related entities are not versioned.</summary>
public sealed record RecipeHistoryDto(
    int Id,
    string Name,
    string? Description,
    string Instructions,
    int PreparationMinutes,
    int Servings,
    int CategoryId,
    DateTimeOffset ValidFrom,
    DateTimeOffset ValidTo);
