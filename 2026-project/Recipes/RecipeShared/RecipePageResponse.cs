namespace RecipeShared;

/// <summary>A page of recipes and the total number of recipes.</summary>
public sealed record RecipePageResponse(IReadOnlyList<RecipeDto> Items, int TotalCount);
