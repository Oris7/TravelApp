using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using TravelRecommendations.Auth;
using TravelRecommendations.Auth.Model;
using TravelRecommendations.Data;
using TravelRecommendations.Data.Repositories;

//var builder = WebApplication.CreateBuilder(args);
//var app = builder.Build();

//app.MapGet("/", () => "Hello World!");

var builder = WebApplication.CreateBuilder(args);

/*builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("http://localhost:3000").AllowAnyHeader().AllowAnyMethod();
    });
});*/

// what used to be ConfigureServices


//builder.Services.AddDbContext<RestContext>();

builder.Services.AddDbContext<RestContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("PostgreSQL")));



builder.Services.AddAutoMapper(typeof(Program));
builder.Services.AddTransient<ICountriesRepository, CountriesRepository>();
builder.Services.AddTransient<ICitiesRepository, CitiesRepository>();
builder.Services.AddTransient<IActivitiesRepository, ActivitiesRepository>();
builder.Services.AddControllers();

builder.Services.AddTransient<JwtTokenService>();
builder.Services.AddTransient<SessionService>();
builder.Services.AddScoped<AuthSeeder>();

builder.Services.AddEndpointsApiExplorer(); // leidžia generuoti OpenAPI JSON
builder.Services.AddSwaggerGen();           // įjungia Swagger


builder.Services.AddIdentity<User, IdentityRole>()
    .AddEntityFrameworkStores<RestContext>()
    .AddDefaultTokenProviders();


builder.Services.AddAuthentication(configureOptions: options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.MapInboundClaims = false;
    options.TokenValidationParameters.ValidAudience = builder.Configuration["Jwt:ValidAudience"];
    options.TokenValidationParameters.ValidIssuer = builder.Configuration["Jwt:ValidIssuer"];
    options.TokenValidationParameters.IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:Secret"]));
});

builder.Services.AddAuthorization();

var app = builder.Build();

using var scope = app.Services.CreateScope();

var dbContext = scope.ServiceProvider.GetRequiredService<RestContext>();
dbContext.Database.Migrate();

var dbSeeder = scope.ServiceProvider.GetRequiredService<AuthSeeder>();
await dbSeeder.SeedAsync();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();        // sugeneruoja OpenAPI JSON
    app.UseSwaggerUI();      // atidaro Swagger UI naršyklėje
}

app.AddAuthApi();

app.MapGet("/", () => "Hello World!");

// what used to be Configure
app.MapControllers();
app.UseAuthentication();
app.UseAuthorization();
app.Run();