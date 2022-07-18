using Data;
using Microsoft.EntityFrameworkCore;
using WebReader.Models;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authorization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using WebReader;

//BUILDER
var builder = WebApplication.CreateBuilder(args);

//ADD SERVICES
Bootstrap.AddApplicationServices(builder);

//APPLICATION
var app = builder.Build();

//ENABLE SWAGGER WHEN APP RUNNING IN DEVELOPMENT ENVIROMENT
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();
app.UseDefaultFiles();
app.UseStaticFiles();
app.UseHttpsRedirection();


//GET TOKEN
app.MapPost("/webreader/security/login", [AllowAnonymous] (UserDto user) =>
{
    if (user.email == "admin" && user.password == "adminpassword")
    {
        var issuer = builder.Configuration["Jwt:Issuer"];
        var audience = builder.Configuration["Jwt:Audience"];
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
        var jwtTokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(builder.Configuration["Jwt:Key"]);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim("Id", "1"),
                new Claim(JwtRegisteredClaimNames.Sub, user.email),
                new Claim(JwtRegisteredClaimNames.Email, user.email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            }),

            Expires = DateTime.UtcNow.AddHours(6),
            Audience = audience,
            Issuer = issuer,
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha512Signature)
        };

        var token = jwtTokenHandler.CreateToken(tokenDescriptor);

        var jwtToken = jwtTokenHandler.WriteToken(token);

        return Results.Ok(jwtToken);
    }
    else
    {
        return Results.Unauthorized();
    }
});

//GET ALL CLIMATES
app.MapGet("/climate/", [Authorize] async (WebReaderDataContext db) =>
{
    var climates = await db.Temperatures.ToListAsync();
    return climates == null ? Results.NotFound() : Results.Ok(climates);
});

//GET CLIMATE BY ID
app.MapGet("/climates/{id}", [Authorize] async (WebReaderDataContext db, Guid id) =>
{
    var climate = await db.Temperatures.FirstOrDefaultAsync(x => x.Id == id);
    return climate == null ? Results.NotFound() : Results.Ok(climate);
});

//INSERT NEW CLIMATE
app.MapPost("/climates", [Authorize] async (WebReaderDataContext db, Temperature climate) =>
{
    if (await db.Temperatures.FirstOrDefaultAsync(x => x.Id == climate.Id) != null)
    {
        return Results.BadRequest();
    }

    db.Temperatures.Add(climate);
    await db.SaveChangesAsync();
    return Results.Created($"/climates/{climate.Id}", climate);
});

//UPDATE
app.MapPut("/climates/{id}", [Authorize] async (WebReaderDataContext db, Guid id, Temperature climate) =>
{
    var existItem = await db.Temperatures.FirstOrDefaultAsync(x => x.Id == id);
    if (existItem == null) return Results.BadRequest();

    existItem.Data = climate.Data;

    await db.SaveChangesAsync();
    return Results.Ok(climate);
});

app.MapDelete("/climates/{id}", [Authorize] async (WebReaderDataContext db, Guid id) =>
{
    var existItem = await db.Temperatures.FirstOrDefaultAsync(x => x.Id == id);
    if (existItem == null) return Results.BadRequest();

    db.Temperatures.Remove(existItem);
    await db.SaveChangesAsync();
    return Results.NoContent();
});

app.MapGet("Activity/Updates", [Authorize] async (WebReaderDataContext db) =>
{
    return Results.Ok();
});

app.Run();