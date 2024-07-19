using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Register database context
var connString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(connString));

var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.Run();
