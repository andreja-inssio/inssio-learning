using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using RecipeAPI.ApiModels;
using RecipeAPI.Services;
using RecipeShared;

namespace RecipeAPI.Endpoints;

public static class CategoryEndpoints
{
    public static RouteGroupBuilder MapCategoryEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/categories").WithTags("Categories")
            .WithDescription("Create, retrieve, replace, and delete recipe categories.");

        group.MapGet("/", async Task<Ok<IReadOnlyList<CategoryDto>>> (
            ICategoryService service, CancellationToken cancellationToken) =>
                TypedResults.Ok(await service.GetAllAsync(cancellationToken)))
            .WithName("GetCategories").WithSummary("List categories");

        group.MapGet("/{id:int}", async Task<Results<Ok<CategoryDto>, NotFound>> (
            int id, ICategoryService service, CancellationToken cancellationToken) =>
        {
            var category = await service.GetByIdAsync(id, cancellationToken);
            return category is null ? TypedResults.NotFound() : TypedResults.Ok(category);
        })
            .WithName("GetCategoryById").WithSummary("Get a category")
            .Produces<CategoryDto>(StatusCodes.Status200OK).Produces(StatusCodes.Status404NotFound);

        group.MapPost("/", async Task<Results<Created<CategoryDto>, Conflict<ProblemDetails>>> (
            CategoryRequest request, ICategoryService service, CancellationToken cancellationToken) =>
        {
            var result = await service.CreateAsync(request, cancellationToken);
            return result.Status == OperationStatus.Success
                ? TypedResults.Created($"/api/categories/{result.Value!.Id}", result.Value)
                : TypedResults.Conflict(ApiProblems.Conflict("A category with this name already exists."));
        })
            .WithName("CreateCategory").WithSummary("Create a category")
            .Produces<CategoryDto>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status409Conflict);

        group.MapPut("/{id:int}", async Task<Results<Ok<CategoryDto>, NotFound, Conflict<ProblemDetails>>> (
            int id, CategoryRequest request, ICategoryService service, CancellationToken cancellationToken) =>
        {
            var result = await service.UpdateAsync(id, request, cancellationToken);
            return result.Status switch
            {
                OperationStatus.Success => TypedResults.Ok(result.Value!),
                OperationStatus.NotFound => TypedResults.NotFound(),
                _ => TypedResults.Conflict(ApiProblems.Conflict("A category with this name already exists."))
            };
        })
            .WithName("UpdateCategory").WithSummary("Replace a category")
            .Produces<CategoryDto>(StatusCodes.Status200OK).Produces(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status409Conflict);

        group.MapDelete("/{id:int}", async Task<Results<NoContent, NotFound, Conflict<ProblemDetails>>> (
            int id, ICategoryService service, CancellationToken cancellationToken) =>
        {
            var status = await service.DeleteAsync(id, cancellationToken);
            return status switch
            {
                OperationStatus.Success => TypedResults.NoContent(),
                OperationStatus.NotFound => TypedResults.NotFound(),
                _ => TypedResults.Conflict(ApiProblems.Conflict("The category is assigned to one or more recipes."))
            };
        })
            .WithName("DeleteCategory").WithSummary("Delete a category")
            .Produces(StatusCodes.Status204NoContent).Produces(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status409Conflict);

        return group;
    }
}

internal static class ApiProblems
{
    public static ProblemDetails Conflict(string detail) => new()
    {
        Status = StatusCodes.Status409Conflict,
        Title = "Conflict",
        Detail = detail
    };

    public static ProblemDetails InvalidReference(string detail) => new()
    {
        Status = StatusCodes.Status400BadRequest,
        Title = "Invalid reference",
        Detail = detail
    };
}
