using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using RecipeAPI.ApiModels;
using RecipeAPI.Services;
using RecipeShared;

namespace RecipeAPI.Endpoints;

public static class RecipeIngredientEndpoints
{
    public static RouteGroupBuilder MapRecipeIngredientEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/recipe-ingredients").WithTags("Recipe ingredients")
            .WithDescription("Manage recipe-ingredient associations and their quantities and units.");

        group.MapGet("/", async Task<Ok<IReadOnlyList<RecipeIngredientDto>>> (
            IRecipeIngredientService service, CancellationToken cancellationToken) =>
                TypedResults.Ok(await service.GetAllAsync(cancellationToken)))
            .WithName("GetRecipeIngredients").WithSummary("List recipe-ingredient associations");

        group.MapGet("/{recipeId:int}/{ingredientId:int}",
            async Task<Results<Ok<RecipeIngredientDto>, NotFound>> (
                int recipeId, int ingredientId, IRecipeIngredientService service,
                CancellationToken cancellationToken) =>
            {
                var row = await service.GetByIdAsync(recipeId, ingredientId, cancellationToken);
                return row is null ? TypedResults.NotFound() : TypedResults.Ok(row);
            })
            .WithName("GetRecipeIngredientById").WithSummary("Get a recipe-ingredient association")
            .Produces<RecipeIngredientDto>(StatusCodes.Status200OK).Produces(StatusCodes.Status404NotFound);

        group.MapPost("/", async Task<Results<Created<RecipeIngredientDto>,
            BadRequest<ProblemDetails>, Conflict<ProblemDetails>>> (
            CreateRecipeIngredientRequest request, IRecipeIngredientService service,
            CancellationToken cancellationToken) =>
        {
            var result = await service.CreateAsync(request, cancellationToken);
            return result.Status switch
            {
                OperationStatus.Success => TypedResults.Created(
                    $"/api/recipe-ingredients/{result.Value!.RecipeId}/{result.Value.IngredientId}",
                    result.Value),
                OperationStatus.Conflict => TypedResults.Conflict(
                    ApiProblems.Conflict("This ingredient is already assigned to the recipe.")),
                _ => TypedResults.BadRequest(ApiProblems.InvalidReference(
                    "The recipe or ingredient does not exist."))
            };
        })
            .WithName("CreateRecipeIngredient").WithSummary("Add an ingredient to a recipe")
            .Produces<RecipeIngredientDto>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status409Conflict);

        group.MapPut("/{recipeId:int}/{ingredientId:int}",
            async Task<Results<Ok<RecipeIngredientDto>, NotFound>> (
                int recipeId, int ingredientId, UpdateRecipeIngredientRequest request,
                IRecipeIngredientService service, CancellationToken cancellationToken) =>
            {
                var result = await service.UpdateAsync(recipeId, ingredientId, request, cancellationToken);
                return result.Status == OperationStatus.Success
                    ? TypedResults.Ok(result.Value!)
                    : TypedResults.NotFound();
            })
            .WithName("UpdateRecipeIngredient").WithSummary("Update an ingredient in a recipe")
            .Produces<RecipeIngredientDto>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);

        group.MapDelete("/{recipeId:int}/{ingredientId:int}",
            async Task<Results<NoContent, NotFound>> (
                int recipeId, int ingredientId, IRecipeIngredientService service,
                CancellationToken cancellationToken) =>
            {
                var status = await service.DeleteAsync(recipeId, ingredientId, cancellationToken);
                return status == OperationStatus.Success
                    ? TypedResults.NoContent()
                    : TypedResults.NotFound();
            })
            .WithName("DeleteRecipeIngredient").WithSummary("Remove an ingredient from a recipe")
            .Produces(StatusCodes.Status204NoContent).Produces(StatusCodes.Status404NotFound);

        return group;
    }
}
