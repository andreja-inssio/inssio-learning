namespace RecipeShared;

/// <summary>A combined ingredient quantity for selected recipes, kept separate by unit.</summary>
public sealed record ShoppingListItemDto(int IngredientId, string IngredientName, decimal Quantity, string Unit);

/// <summary>Recipe count and average preparation time for a category containing recipes.</summary>
public sealed record CategoryStatisticsDto(int CategoryId, string CategoryName, int RecipeCount, double AverageMinutes);

/// <summary>An ingredient's recipe assignment, or null recipe fields when the ingredient is unused.</summary>
public sealed record IngredientUsageDto(int IngredientId, string IngredientName, int? RecipeId, string? RecipeName);
