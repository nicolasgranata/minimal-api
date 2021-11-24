using Microsoft.EntityFrameworkCore;
using MinimalApi.Models;
using MinimalApi.Persistence;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<TodoSampleDbContext>(options => {
    options.UseInMemoryDatabase("TodoItemInMemoryDB");
});
// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("/TodoItem", async (TodoSampleDbContext dbContext) =>
{
    return Results.Ok(await dbContext.TodoItems.ToListAsync());
})
.Produces<IEnumerable<TodoItem>>(StatusCodes.Status200OK)
.Produces(StatusCodes.Status500InternalServerError)
.WithName("GetTodoItem");

app.MapGet("/TodoItem/{id}", async (TodoSampleDbContext dbContext, long id) =>
{
    var item = await dbContext.TodoItems.FindAsync(id);
    return item is not null ? Results.Ok(item) : Results.NotFound();

})
.Produces<TodoItem>(StatusCodes.Status200OK)
.Produces(StatusCodes.Status404NotFound)
.Produces(StatusCodes.Status500InternalServerError)
.WithName("GetTodoItemById");

app.MapPost("/TodoItem", async (TodoSampleDbContext dbContext, TodoItem todoItem) =>
{
    dbContext.Add(todoItem);
    await dbContext.SaveChangesAsync();

    return Results.Created($"/TodoItem/{todoItem.Id}", todoItem);
})
.Produces<TodoItem>(StatusCodes.Status201Created)
.Produces(StatusCodes.Status500InternalServerError)
.WithName("CreateTodoItem");

app.MapPut("/TodoItem", async (TodoSampleDbContext dbContext, long id, TodoItem todoItem) =>
{
    if (await dbContext.TodoItems.AsNoTracking().
        FirstOrDefaultAsync(t => t.Id == id) is not null)
    {
        todoItem.Id = id;
        dbContext.TodoItems.Update(todoItem);
        await dbContext.SaveChangesAsync();
        return Results.Ok(todoItem);
    }

    return Results.NotFound();
})
.Produces<TodoItem>(StatusCodes.Status200OK)
.Produces(StatusCodes.Status404NotFound)
.Produces(StatusCodes.Status500InternalServerError)
.WithName("UpdateTodoItem");

app.MapDelete("/TodoItem", async (TodoSampleDbContext dbContext, long id) =>
{
    var todoItem = await dbContext.TodoItems.FindAsync(id);

    if (todoItem is not null)
    {
        dbContext.TodoItems.Remove(todoItem);
        await dbContext.SaveChangesAsync();
        return Results.NoContent();
    }

    return Results.NotFound();
})
.Produces(StatusCodes.Status204NoContent)
.Produces(StatusCodes.Status404NotFound)
.Produces(StatusCodes.Status500InternalServerError)
.WithName("DeleteTodoItem");

app.Run();