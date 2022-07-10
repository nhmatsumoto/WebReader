using Services.WebReader;
using Data;
using Microsoft.EntityFrameworkCore;
using WebReader.Models;

var builder = WebApplication.CreateBuilder(args);
// Configura serviços
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<WebReaderDataContext>(options =>
    options.UseSqlServer("Server=localhost;Database=sa;Trusted_Connection=True;")
);

var app = builder.Build();

// Configura pipeline HTTP request
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseDefaultFiles();
app.UseStaticFiles();

app.UseHttpsRedirection();

app.MapGet("/climate/", async (WebReaderDataContext db) => await db.Climates.ToListAsync());

app.MapPost("/climate/add/data", async (WebReaderDataContext db, Climate data) =>
{
    if (data != null)
    {
        await db.Climates.AddAsync(data);
        await db.SaveChangesAsync();
        return Results.Created($"/climate/{data.Id}", data);
    }

    return Results.BadRequest();
});

app.MapPost("/add-timer/", (TimeSpan time) =>
{
    DateTime.Now.Add(time);
});


app.MapGet("/read-text/", (String text) =>
{
    var reader = new Reader(text);
    reader.Print(700);
});



app.Run();