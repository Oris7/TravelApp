using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using TravelRecommendations.Auth;
using TravelRecommendations.Auth.Model;
using TravelRecommendations.Data;
using TravelRecommendations.Data.Repositories;
using Npgsql.EntityFrameworkCore.PostgreSQL;

var builder = WebApplication.CreateBuilder(args);

// Database
builder.Services.AddDbContext<RestContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("PostgreSQL")));

// AutoMapper & Repositories
builder.Services.AddAutoMapper(typeof(Program));
builder.Services.AddTransient<ICountriesRepository, CountriesRepository>();
builder.Services.AddTransient<ICitiesRepository, CitiesRepository>();
builder.Services.AddTransient<IActivitiesRepository, ActivitiesRepository>();
builder.Services.AddControllers();

// CORS for frontend
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:5173", "https://sea-lion-app-ccqwa.ondigitalocean.app") // local frontend dev
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// JWT Auth
builder.Services.AddTransient<JwtTokenService>();
builder.Services.AddTransient<SessionService>();
builder.Services.AddScoped<AuthSeeder>();
builder.Services.AddIdentity<User, IdentityRole>()
    .AddEntityFrameworkStores<RestContext>()
    .AddDefaultTokenProviders();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.MapInboundClaims = false;
    options.TokenValidationParameters.ValidAudience = builder.Configuration["Jwt:ValidAudience"];
    options.TokenValidationParameters.ValidIssuer = builder.Configuration["Jwt:ValidIssuer"];
    options.TokenValidationParameters.IssuerSigningKey =
        new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:Secret"]));
});

builder.Services.AddAuthorization();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Database Migrations & Seeding
using var scope = app.Services.CreateScope();
var dbContext = scope.ServiceProvider.GetRequiredService<RestContext>();
dbContext.Database.Migrate();

var dbSeeder = scope.ServiceProvider.GetRequiredService<AuthSeeder>();
await dbSeeder.SeedAsync();

// Swagger for dev
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Auth API
app.AddAuthApi();

// Routing & Middleware
app.UseRouting();
app.UseCors("AllowFrontend");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

// Default endpoint
app.MapGet("/", () => "Hello World!");

// ✅ Make the app listen on the correct port for DO
var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
app.Urls.Add($"http://*:{port}");

app.Run();
