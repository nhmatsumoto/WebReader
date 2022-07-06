var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/", () => new Reader(Guid.NewGuid(), "Reader1 Reader2 Reader3"));


app.Run();
