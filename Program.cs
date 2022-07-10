using Services.WebReader;
using Data;
using Microsoft.EntityFrameworkCore;
using WebReader.Models;
using Microsoft.OpenApi.Models;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

var securityScheme = new OpenApiSecurityScheme()
{
    Name = "Authorization",
    Type = SecuritySchemeType.ApiKey,
    Scheme = "Bearer",
    BearerFormat = "JWT",
    In = ParameterLocation.Header,
    Description = "JSON Web Token based security",
};

var contact = new OpenApiContact()
{
    Name = "Nilton H. Matsumoto",
    Email = "hiroyukims@hotmail.com",
    Url = new Uri("https://nhmatsumoto.github.io/")
};

var info = new OpenApiInfo()
{
    Version = "v1",
    Title = "WebReader demo",
    Description = "WebReader is a software for making support to farmers",
    //TermsOfService = new Uri(""),
    Contact = contact
};

var securityReq = new OpenApiSecurityRequirement()
{
    {
        new OpenApiSecurityScheme
        {
            Reference = new OpenApiReference
            {
                Type = ReferenceType.SecurityScheme,
                Id = "Bearer"
            }
        },
        new string[] {}
    }
};

// Configura serviços
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(o =>
{
    o.SwaggerDoc("v1", info);
    o.AddSecurityDefinition("Bearer", securityScheme);
    o.AddSecurityRequirement(securityReq);
});
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<WebReaderDataContext>(options =>
    options.UseSqlServer("Server=localhost;Database=sa;Trusted_Connection=True;")
);

// Add JWT configuration
builder.Services.AddAuthentication(o =>
{
    o.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    o.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    o.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(o =>
{
    o.TokenValidationParameters = new TokenValidationParameters
    {
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey
            (Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])),
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = false,
        ValidateIssuerSigningKey = true
    };
});

builder.Services.AddAuthorization();
builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();

// Configura pipeline HTTP request
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

//Endpoints
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

app.MapPost("/security/getToken", [AllowAnonymous] (UserDto user) =>
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

app.MapGet("/climates", [Authorize] async (WebReaderDataContext db) =>
{
    return await db.Climates.ToListAsync();
});

app.MapPost("/climates", [Authorize] async (WebReaderDataContext db, Climate climate) =>
{
    if (await db.Climates.FirstOrDefaultAsync(x => x.Id == climate.Id) != null)
    {
        return Results.BadRequest();
    }

    db.Climates.Add(climate);
    await db.SaveChangesAsync();
    return Results.Created($"/Items/{climate.Id}", climate);
});

app.MapGet("/climates/{id}", [Authorize] async (WebReaderDataContext db, Guid id) =>
{
    var climate = await db.Climates.FirstOrDefaultAsync(x => x.Id == id);
    return climate == null ? Results.NotFound() : Results.Ok(climate);
});

app.MapPut("/climates/{id}", [Authorize] async (WebReaderDataContext db, Guid id, Climate climate) =>
{
    var existItem = await db.Climates.FirstOrDefaultAsync(x => x.Id == id);
    if (existItem == null) return Results.BadRequest();

    existItem.Temparature = climate.Temparature;

    await db.SaveChangesAsync();
    return Results.Ok(climate);
});

app.MapDelete("/climates/{id}", [Authorize] async (WebReaderDataContext db, Guid id) =>
{
    var existItem = await db.Climates.FirstOrDefaultAsync(x => x.Id == id);
    if (existItem == null)
    {
        return Results.BadRequest();
    }

    db.Climates.Remove(existItem);
    await db.SaveChangesAsync();
    return Results.NoContent();
});

app.Run();