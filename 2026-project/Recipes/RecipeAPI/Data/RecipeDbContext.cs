using Microsoft.EntityFrameworkCore;
using RecipeAPI.Entities;
using RecipeShared;

namespace RecipeAPI.Data;

public class RecipeDbContext(DbContextOptions<RecipeDbContext> options) : DbContext(options)
{
    public DbSet<Recipe> Recipes => Set<Recipe>();
    public DbSet<Ingredient> Ingredients => Set<Ingredient>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<RecipeIngredient> RecipeIngredients => Set<RecipeIngredient>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        var recipe = modelBuilder.Entity<Recipe>();
        recipe.ToTable("Recipes", table =>
        {
            table.IsTemporal(temporal => temporal.UseHistoryTable("RecipeHistory"));
            table.HasCheckConstraint("CK_Recipes_Name", "LEN(LTRIM(RTRIM([Name]))) > 0");
            table.HasCheckConstraint("CK_Recipes_Instructions", "LEN(LTRIM(RTRIM([Instructions]))) > 0");
            table.HasCheckConstraint("CK_Recipes_PreparationMinutes", "[PreparationMinutes] >= 0");
            table.HasCheckConstraint("CK_Recipes_Servings", "[Servings] > 0");
        });
        recipe.Property(x => x.Name).IsRequired().HasMaxLength(RecipeValidationLimits.NameMaxLength);
        recipe.Property(x => x.Description).HasMaxLength(RecipeValidationLimits.DescriptionMaxLength);
        recipe.Property(x => x.Instructions).IsRequired().HasMaxLength(RecipeValidationLimits.InstructionsMaxLength);
        recipe.HasOne(x => x.Category)
            .WithMany(x => x.Recipes)
            .HasForeignKey(x => x.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        var ingredient = modelBuilder.Entity<Ingredient>();
        ingredient.ToTable("Ingredients", table =>
            table.HasCheckConstraint("CK_Ingredients_Name",
                "LEN([Name]) > 0 AND DATALENGTH([Name]) = DATALENGTH(LTRIM(RTRIM([Name])))"));
        ingredient.Property(x => x.Name).IsRequired()
            .HasMaxLength(RecipeValidationLimits.NameMaxLength)
            .UseCollation("Latin1_General_100_CI_AS");
        ingredient.HasIndex(x => x.Name).IsUnique();

        var category = modelBuilder.Entity<Category>();
        category.ToTable("Categories", table =>
            table.HasCheckConstraint("CK_Categories_Name",
                "LEN([Name]) > 0 AND DATALENGTH([Name]) = DATALENGTH(LTRIM(RTRIM([Name])))"));
        category.Property(x => x.Name).IsRequired()
            .HasMaxLength(RecipeValidationLimits.NameMaxLength)
            .UseCollation("Latin1_General_100_CI_AS");
        category.HasIndex(x => x.Name).IsUnique();

        var recipeIngredient = modelBuilder.Entity<RecipeIngredient>();
        recipeIngredient.ToTable("RecipeIngredients", table =>
        {
            table.HasCheckConstraint("CK_RecipeIngredients_Quantity", "[Quantity] > 0");
            table.HasCheckConstraint("CK_RecipeIngredients_Unit", "LEN(LTRIM(RTRIM([Unit]))) > 0");
        });
        recipeIngredient.HasKey(x => new { x.RecipeId, x.IngredientId });
        recipeIngredient.Property(x => x.Quantity).HasPrecision(10, 3);
        recipeIngredient.Property(x => x.Unit).IsRequired().HasMaxLength(RecipeValidationLimits.UnitMaxLength);
        recipeIngredient.HasOne(x => x.Recipe)
            .WithMany(x => x.RecipeIngredients)
            .HasForeignKey(x => x.RecipeId)
            .OnDelete(DeleteBehavior.Cascade);
        recipeIngredient.HasOne(x => x.Ingredient)
            .WithMany(x => x.RecipeIngredients)
            .HasForeignKey(x => x.IngredientId)
            .OnDelete(DeleteBehavior.Restrict);

        RecipeSeed.Configure(modelBuilder);
    }
}
