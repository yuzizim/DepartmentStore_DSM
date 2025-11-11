// src/DepartmentStore.API/Program.cs

using AutoMapper;
using DepartmentStore.API.OData;
using DepartmentStore.DataAccess;
using DepartmentStore.DataAccess.Entities;
using DepartmentStore.DataAccess.Repositories;
using DepartmentStore.Service.Implementations;
using DepartmentStore.Service.Interfaces;
using DepartmentStore.Utilities.Helpers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.OData;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

// ========================= 1. DATABASE CONTEXT =========================
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(
        configuration.GetConnectionString("DefaultConnection")!,
        new MySqlServerVersion(new Version(8, 0, 32)))
        .EnableDetailedErrors()
        .EnableSensitiveDataLogging());

// ========================= 2. IDENTITY CONFIG =========================
builder.Services
    .AddIdentity<AppUser, AppRole>(options =>
    {
        options.Password.RequireDigit = true;
        options.Password.RequireLowercase = true;
        options.Password.RequireUppercase = false;
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequiredLength = 6;
        options.User.RequireUniqueEmail = true;
        options.SignIn.RequireConfirmedEmail = false;
    })
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

// ========================= 3. REPOSITORIES & SERVICES =========================
builder.Services.AddScoped(typeof(IBaseRepository<>), typeof(BaseRepository<>));
builder.Services.AddScoped<IUserRepository, UserRepository>();

builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<ISupplierService, SupplierService>();
builder.Services.AddScoped<IInventoryService, InventoryService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IPaymentService, PaymentService>();

// ========================= 4. AUTOMAPPER =========================
builder.Services.AddAutoMapper(typeof(MapperProfile));

// ========================= 5. CONTROLLERS + ODATA + RAZOR PAGES =========================
builder.Services
    .AddControllers(options => options.ReturnHttpNotAcceptable = true)
    .AddXmlSerializerFormatters()
    .AddNewtonsoftJson(opt =>
        opt.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore)
    .AddOData(opt =>
    {
        opt.Select().Filter().OrderBy().Count().Expand().SetMaxTop(100);
        opt.AddRouteComponents("odata", EdmModelBuilder.GetEdmModel());
    });

// Nếu API có Razor Pages (không cần thiết)
// builder.Services.AddRazorPages(); // XÓA nếu không dùng

// ========================= 6. JWT AUTHENTICATION =========================
var jwtSection = configuration.GetSection("JwtSettings");
var key = Encoding.UTF8.GetBytes(jwtSection["Key"]!);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSection["Issuer"],
        ValidAudience = jwtSection["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddAuthorization();

// ========================= 7. CORS =========================
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader());
});

// ========================= 8. SWAGGER =========================
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Department Store API",
        Version = "v1",
        Description = "PRN232 - Department Store Management System"
    });

    var jwtSecurityScheme = new OpenApiSecurityScheme
    {
        Scheme = "bearer",
        BearerFormat = "JWT",
        Name = "JWT Authentication",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Description = "Enter **Bearer** [space] then your token.",
        Reference = new OpenApiReference
        {
            Id = "Bearer",
            Type = ReferenceType.SecurityScheme
        }
    };

    c.AddSecurityDefinition("Bearer", jwtSecurityScheme);
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        { jwtSecurityScheme, Array.Empty<string>() }
    });
});

// ========================= 9. BUILD APP =========================
var app = builder.Build();

// ========================= 10. SEED DATABASE =========================
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var logger = services.GetRequiredService<ILogger<Program>>();
    try
    {
        await SeedDb.InitializeAsync(services);
        logger.LogInformation("Database seeded successfully.");
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Error seeding database.");
    }
}

// ========================= 11. MIDDLEWARE =========================
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "DSM API v1"));
}
else
{
    app.UseExceptionHandler("/error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();