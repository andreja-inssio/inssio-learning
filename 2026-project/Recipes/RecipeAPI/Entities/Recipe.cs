namespace RecipeAPI.Entities;

public class Recipe
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }
    public required string Instructions { get; set; }
    public int PreparationMinutes { get; set; }
    public int Servings { get; set; }
    public int CategoryId { get; set; }
    public Category? Category { get; set; }
    public ICollection<RecipeIngredient> RecipeIngredients { get; set; } = [];
}
