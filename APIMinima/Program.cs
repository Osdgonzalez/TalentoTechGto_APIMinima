using APIMinima;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<TodoDb>(opt => opt.UseInMemoryDatabase("TodoList"));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

var app = builder.Build();

//var todoItems = app.MapGroup("/todoitems");

//todoItems.MapGet("/" , GetAllTodos);
//todoItems.MapGet("/complete", GetCompleteTodos);
//todoItems.MapGet("/{id}", GetTodo);
//todoItems.MapPost("/", CreatedTodo);
//todoItems.MapPut("/{id}", UpdateTodo);
//todoItems.MapDelete("/{id}", DeleteTodo);


RouteGroupBuilder todoItems = app.MapGroup("/todoitems");

todoItems.MapGet("/", GetAllTodos);
todoItems.MapGet("/complete", GetCompleteTodos);
todoItems.MapGet("/{id}", GetTodo);
todoItems.MapPost("/", CreateTodo);
todoItems.MapPut("/{id}", UpdateTodo);
todoItems.MapDelete("/{id}", DeleteTodo);


//todoItems.MapGet("/" , async (TodoDb db) => 
//                            await db.Todos.ToListAsync());

//app.MapGet("/todoitems/complete", async (TodoDb db) =>
//                            await db.Todos.Where(t => t.IsComplete).ToListAsync());

//todoItems.MapGet("/{id}", async (int id, TodoDb db) => 
//                                    await db.Todos.FindAsync(id)
//                                    is Todo todo
//                                    ? Results.Ok(todo)
//                                    : Results.NotFound()
//                                    );

//app.MapPost("/todositems", async (Todo todo, TodoDb db) =>
//{
//    db.Todos.Add(todo);
//    await db.SaveChangesAsync();

//    return Results.Created($"/todoitems/{todo.Id}", todo);
//});


//app.MapPut("/todoitems/{id}", async (int id, Todo inputTodo, TodoDb db) =>
//{

//    var todo = await db.Todos.FindAsync(id);

//    if (todo is null) return Results.NotFound();

//    todo.Name = inputTodo.Name;
//    todo.IsComplete = inputTodo.IsComplete;

//    await db.SaveChangesAsync();

//    return Results.NoContent();
//});

//app.MapDelete("/todoitems/{id}", async (int id, TodoDb db) =>
//{
//    if (await db.Todos.FindAsync(id) is Todo todo)
//    {
//        db.Todos.Remove(todo);
//        await db.SaveChangesAsync();
//        return Results.Ok(todo);
//    }

//    return Results.NotFound();
//});




static async Task<IResult> GetAllTodos(TodoDb db)
{
    return TypedResults.Ok(await db.Todos.Select(x => new TodoItemDTO (x)).ToArrayAsync());
}

static async Task<IResult> GetCompleteTodos(TodoDb db)
{
    return TypedResults.Ok(await db.Todos.Where(t => t.IsComplete).Select(x => new TodoItemDTO(x)).ToListAsync());
}

static async Task<IResult> GetTodo(int id, TodoDb db)
{
    return await db.Todos.FindAsync(id)
                                    is Todo todo
                                    ? TypedResults.Ok(new TodoItemDTO(todo))
                                    : TypedResults.NotFound();
}

static async Task <IResult> CreateTodo(TodoItemDTO todoItemDTO , TodoDb db)
{
    var todoItem = new Todo
    {
        IsComplete = todoItemDTO.IsComplete,
        Name = todoItemDTO.Name
    };

    db.Todos.Add(todoItem);
    await db.SaveChangesAsync();

    todoItemDTO = new TodoItemDTO(todoItem);

    return TypedResults.Created($"/todoitems/{todoItem.Id}" , todoItemDTO);
}

static async Task<IResult> UpdateTodo(int id , TodoItemDTO todoItemDTO , TodoDb db)
{
    var todo = await db.Todos.FindAsync(id);

    if(todo is null) return TypedResults.NotFound();

    todo.Name = todoItemDTO.Name;
    todo.IsComplete = todoItemDTO.IsComplete;

    await db.SaveChangesAsync();

    return TypedResults.NoContent();
}

static async Task<IResult> DeleteTodo( int id , TodoDb db)
{
    if(await db.Todos.FindAsync(id) is Todo todo)
    {
        db.Todos.Remove(todo);
        await db.SaveChangesAsync();

        TodoItemDTO todoItemDTO = new TodoItemDTO(todo);

        return TypedResults.Ok(todoItemDTO);
    }
    return TypedResults.NotFound();

}

app.Run();