using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace TodoApi;

internal static class TodoApi
{
    public static RouteGroupBuilder MapTodos(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/todos");

        group.WithTags("Todos");

        // Add security requirements, all incoming requests to this API *must*
        // be authenticated with a valid user.
        //group.RequireAuthorization(pb => pb.RequireCurrentUser())
        //     .AddOpenApiSecurityRequirement();

        // Rate limit all of the APIs
        //group.RequirePerUserRateLimit();

        // Validate the parameters
        //group.WithParameterValidation(typeof(TodoItem));

        group.MapGet("/lists", async (IGoogleTasksApi googleApi) => await googleApi.GetListsAsync());
        group.MapGet("/lists/{listId}", async (IGoogleTasksApi googleApi, string listId) => await googleApi.GetListAsync(listId));
        group.MapPost("/lists/{listId}", async (IGoogleTasksApi googleApi, string listName) => await googleApi.CreateListAsync(listName));
        group.MapDelete("/lists/{listId}", async (IGoogleTasksApi googleApi, string listId) => await googleApi.DeleteListAsync(listId));
        group.MapGet("/{listId}/tasks", async (IGoogleTasksApi googleApi, string listId) => await googleApi.GetTasksAsync(listId));
        group.MapGet("/{listId}/tasks/{taskId}", async (IGoogleTasksApi googleApi, string listId, string taskId) => await googleApi.GetTaskAsync(listId, taskId));
        group.MapPost("/{listId}/tasks/", async (IGoogleTasksApi googleApi, string listId, [FromQuery(Name = "title")] string taskTitle, [FromQuery(Name = "parent")] string? parent) => await googleApi.CreateTaskAsync(listId, taskTitle, parent));


        return group;
    }
}
