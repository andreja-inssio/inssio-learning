using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using RecipeAPI.ApiModels;
using RecipeAPI.Services;
using RecipeShared;

namespace RecipeAPI.Endpoints;

public static class IngredientEndpoints
{
    public static RouteGroupBuilder MapIngredientEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/ingredients").WithTags("Ingredients")
            .WithDescription("Create, retrieve, replace, and delete recipe ingredients.");

        group.MapGet("/", async Task<Ok<IReadOnlyList<IngredientDto>>> (
            IIngredientService service, CancellationToken cancellationToken) =>
                TypedResults.Ok(await service.GetAllAsync(cancellationToken)))
            .WithName("GetIngredients").WithSummary("List ingredients");

        group.MapGet("/{id:int}", async Task<Results<Ok<IngredientDto>, NotFound>> (
            int id, IIngredientService service, CancellationToken cancellationToken) =>
        {
            var ingredient = await service.GetByIdAsync(id, cancellationToken);
            return ingredient is null ? TypedResults.NotFound() : TypedResults.Ok(ingredient);
        })
            .WithName("GetIngredientById").WithSummary("Get an ingredient")
            .Produces<IngredientDto>(StatusCodes.Status200OK).Produces(StatusCodes.Status404NotFound);

        group.MapPost("/", async Task<Results<Created<IngredientDto>, Conflict<ProblemDetails>>> (
            IngredientRequest request, IIngredientService service, CancellationToken cancellationToken) =>
        {
            var result = await service.CreateAsync(request, cancellationToken);
            return result.Status == OperationStatus.Success
                ? TypedResults.Created($"/api/ingredients/{result.Value!.Id}", result.Value)
                : TypedResults.Conflict(ApiProblems.Conflict("An ingredient with this name already exists."));
        })
            .WithName("CreateIngredient").WithSummary("Create an ingredient")
            .Produces<IngredientDto>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status409Conflict);

        group.MapPut("/{id:int}", async Task<Results<Ok<IngredientDto>, NotFound, Conflict<ProblemDetails>>> (
            int id, IngredientRequest request, IIngredientService service, CancellationToken cancellationToken) =>
        {
            var result = await service.UpdateAsync(id, request, cancellationToken);
            return result.Status switch
            {
                OperationStatus.Success => TypedResults.Ok(result.Value!),
                OperationStatus.NotFound => TypedResults.NotFound(),
                _ => TypedResults.Conflict(ApiProblems.Conflict("An ingredient with this name already exists."))
            };
        })
            .WithName("UpdateIngredient").WithSummary("Replace an ingredient")
            .Produces<IngredientDto>(StatusCodes.Status200OK).Produces(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status409Conflict);

        group.MapDelete("/{id:int}", async Task<Results<NoContent, NotFound, Conflict<ProblemDetails>>> (
            int id, IIngredientService service, CancellationToken cancellationToken) =>
        {
            var status = await service.DeleteAsync(id, cancellationToken);
            return status switch
            {
                OperationStatus.Success => TypedResults.NoContent(),
                OperationStatus.NotFound => TypedResults.NotFound(),
                _ => TypedResults.Conflict(ApiProblems.Conflict("The ingredient is used by one or more recipes."))
            };
        })
            .WithName("DeleteIngredient").WithSummary("Delete an ingredient")
            .Produces(StatusCodes.Status204NoContent).Produces(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status409Conflict);

        return group;
    }
}
