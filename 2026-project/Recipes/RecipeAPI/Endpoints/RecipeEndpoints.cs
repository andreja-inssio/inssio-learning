using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using RecipeAPI.ApiModels;
using RecipeAPI.Services;
using RecipeShared;

namespace RecipeAPI.Endpoints;

public static class RecipeEndpoints
{
    public static RouteGroupBuilder MapRecipeEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/recipes").WithTags("Recipes")
            .WithDescription("Create, retrieve, replace, and delete recipes and their ingredient lists.");

        group.MapGet("/", async Task<Ok<IReadOnlyList<RecipeDto>>> (
            IRecipeService service, CancellationToken cancellationToken) =>
                TypedResults.Ok(await service.GetAllAsync(cancellationToken)))
            .WithName("GetRecipes").WithSummary("List recipes");

        group.MapGet("/{id:int}", async Task<Results<Ok<RecipeDto>, NotFound>> (
            int id, IRecipeService service, CancellationToken cancellationToken) =>
        {
            var recipe = await service.GetByIdAsync(id, cancellationToken);
            return recipe is null ? TypedResults.NotFound() : TypedResults.Ok(recipe);
        })
            .WithName("GetRecipeById").WithSummary("Get a recipe including its ingredients")
            .Produces<RecipeDto>(StatusCodes.Status200OK).Produces(StatusCodes.Status404NotFound);

        group.MapPost("/", async Task<Results<Created<RecipeDto>, BadRequest<ProblemDetails>>> (
            RecipeRequest request, IRecipeService service, CancellationToken cancellationToken) =>
        {
            var result = await service.CreateAsync(request, cancellationToken);
            return result.Status == OperationStatus.Success
                ? TypedResults.Created($"/api/recipes/{result.Value!.Id}", result.Value)
                : TypedResults.BadRequest(ApiProblems.InvalidReference(
                    "The category or one or more ingredients do not exist."));
        })
            .WithName("CreateRecipe").WithSummary("Create a recipe")
            .Produces<RecipeDto>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest);

        group.MapPut("/{id:int}", async Task<Results<Ok<RecipeDto>, NotFound, BadRequest<ProblemDetails>>> (
            int id, RecipeRequest request, IRecipeService service, CancellationToken cancellationToken) =>
        {
            var result = await service.UpdateAsync(id, request, cancellationToken);
            return result.Status switch
            {
                OperationStatus.Success => TypedResults.Ok(result.Value!),
                OperationStatus.NotFound => TypedResults.NotFound(),
                _ => TypedResults.BadRequest(ApiProblems.InvalidReference(
                    "The category or one or more ingredients do not exist."))
            };
        })
            .WithName("UpdateRecipe").WithSummary("Replace a recipe and its ingredients")
            .Produces<RecipeDto>(StatusCodes.Status200OK).Produces(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status400BadRequest);

        group.MapDelete("/{id:int}", async Task<Results<NoContent, NotFound>> (
            int id, IRecipeService service, CancellationToken cancellationToken) =>
        {
            var status = await service.DeleteAsync(id, cancellationToken);
            return status == OperationStatus.Success
                ? TypedResults.NoContent()
                : TypedResults.NotFound();
        })
            .WithName("DeleteRecipe").WithSummary("Delete a recipe")
            .Produces(StatusCodes.Status204NoContent).Produces(StatusCodes.Status404NotFound);

        return group;
    }
}
