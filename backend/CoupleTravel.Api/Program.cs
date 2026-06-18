using CoupleTravel.Api.Features.Auth;
using CoupleTravel.Api.Features.Cities;
using CoupleTravel.Api.Features.Photos;
using CoupleTravel.Api.Features.Trips;
using CoupleTravel.Api.Infrastructure.Data;
using CoupleTravel.Api.Infrastructure.External;
using CoupleTravel.Api.Infrastructure.Middleware;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Database
builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseNpgsql(
        builder.Configuration.GetConnectionString("Default"),
        o => o.UseNetTopologySuite()
    )
);

// Session-based auth (cookie)
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(opt =>
{
    opt.Cookie.HttpOnly = true;
    opt.Cookie.SameSite = SameSiteMode.Lax;
    opt.Cookie.SecurePolicy = builder.Environment.IsDevelopment()
        ? CookieSecurePolicy.None
        : CookieSecurePolicy.Always;
    opt.IdleTimeout = TimeSpan.FromDays(30);
    opt.Cookie.Name = "ct.session";
});

// HTTP client for Nominatim (rate-limit: 1 req/s)
builder.Services.AddHttpClient<NominatimClient>();

// Blob storage
builder.Services.AddSingleton<IBlobStorage, LocalBlobStorage>();

// Feature services
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<CitiesService>();
builder.Services.AddScoped<TripsService>();
builder.Services.AddScoped<PhotosService>();
builder.Services.AddScoped<RequireAuthFilter>();

builder.Services.AddControllers();
builder.Services.AddOpenApi();

// CORS for local Vite dev server
builder.Services.AddCors(opt => opt.AddDefaultPolicy(p =>
    p.WithOrigins("http://localhost:5173")
     .AllowAnyHeader()
     .AllowAnyMethod()
     .AllowCredentials()
));

var app = builder.Build();

// Run migrations + seed on startup
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await db.Database.MigrateAsync();
    await DbSeeder.SeedAsync(db);
}

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi(); // OpenAPI JSON at /openapi/v1.json
}

app.UseCors();
app.UseSession();
app.MapControllers();

app.Run();
