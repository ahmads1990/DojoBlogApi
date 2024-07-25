using DojoBlogApi.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args); // Register database context
var connString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(connString));
builder.Services.AddCors(options =>
{
    options.AddPolicy("Allow",
           builder =>
           {
               builder.AllowAnyOrigin()
                      .AllowAnyHeader()
                      .AllowAnyMethod();
           });
});
var app = builder.Build();

app.MapGet("/", () => "Hello World!");

// Blog CRUD
// get all
app.MapGet("/blog", async (AppDbContext context) =>
    await context.Blogs
    .Include(b => b.Author)
    .AsNoTracking()
    .ToListAsync());
// find by id
app.MapGet("/blog/{id}", async (int id, AppDbContext context) =>
    await context.Blogs
    .Include(b => b.Author)
    .AsNoTracking()
    .FirstOrDefaultAsync(b => b.Id == id)
        is Blog blog
        ? Results.Ok(blog)
        : Results.NotFound()
);
// create
app.MapPost("/blog", async (AddNewBlogDto dto, AppDbContext context) =>
{
    var newBlog = new Blog
    {
        Title = dto.Title,
        Description = dto.Description,
        AuthorId = dto.AuthorId,
    };
    var result = await context.Blogs.AddAsync(newBlog);
    await context.SaveChangesAsync();
    return result.Entity;
    // return Results.Created($"/blog/{result.Entity.Id}", result.Entity);
});
// update
app.MapPut("/blog", async (int id, AddNewBlogDto dto, AppDbContext context) =>
{
    var blog = await context.Blogs.FirstOrDefaultAsync(b => b.Id == id);
    if (blog is null) return Results.NotFound();

    blog.Title = dto.Title;
    blog.Description = dto.Description;
    blog.AuthorId = dto.AuthorId;

    await context.SaveChangesAsync();
    return Results.Ok(blog);
});
// delete 
app.MapDelete("/blog", async (int id, AppDbContext context) =>
{
    if (await context.Blogs.FindAsync(id) is Blog blog)
    {
        context.Blogs.Remove(blog);
        await context.SaveChangesAsync();
        return Results.NoContent();
    }
    return Results.NotFound();
});

// Author CRUD
// get all
app.MapGet("/author", async (AppDbContext context) =>
    await context.Authors.AsNoTracking().ToListAsync());
// create
app.MapPost("/author", async (string name, AppDbContext context) =>
{
    var author = new Author { Name = name };
    var result = await context.AddAsync(author);
    await context.SaveChangesAsync();
    return result.Entity;
    // return Results.Created($"/author/{result.Entity.Id}", result.Entity);
});
// update
app.MapPut("/author", async (int id, string name, AppDbContext context) =>
{
    var author = await context.Authors.FirstOrDefaultAsync(b => b.Id == id);
    if (author is null) return Results.NotFound();
    author.Name = name;
    await context.SaveChangesAsync();
    return Results.Ok(author);
});
// delete
app.MapDelete("/author", async (int id, AppDbContext context) =>
{
    if (await context.Authors.FindAsync(id) is Author author)
    {
        context.Authors.Remove(author);
        await context.SaveChangesAsync();
        return Results.NoContent();
    }
    return Results.NotFound();
});

app.UseHttpsRedirection();
app.UseCors("Allow");
app.Run();