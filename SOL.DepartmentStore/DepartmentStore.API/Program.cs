using AutoMapper;
using DepartmentStore.DataAccess;
using DepartmentStore.DataAccess.Repositories;
using DepartmentStore.Entities;
using DepartmentStore.Service.Implementations;
using DepartmentStore.Service.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.OData;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

// ========================= DATABASE CONTEXT =========================
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(
        configuration.GetConnectionString("DefaultConnection")!,
        new MySqlServerVersion(new Version(8, 0, 32)))
);

// ========================= IDENTITY CONFIG =========================
builder.Services
    .AddIdentity<AppUser, AppRole>(options =>
    {
        options.Password.RequireDigit = true;
        options.Password.RequireLowercase = true;
        options.Password.RequireUppercase = false;
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequiredLength = 6;
    })
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

// ========================= REPOSITORIES & SERVICES =========================
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<IProductService, ProductService>();
// builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();   // <-- uncomment if you have a UnitOfWork

// ========================= AUTOMAPPER =========================
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

// ========================= CONTROLLERS + ODATA + JSON =========================
builder.Services
    .AddControllers(options => options.ReturnHttpNotAcceptable = true)
    .AddXmlSerializerFormatters()                     // XML support
    .AddNewtonsoftJson(opt =>
        opt.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore)
    .AddOData(opt =>
    {
        opt.Select().Filter().OrderBy().Count().Expand().SetMaxTop(100);
    });

// ========================= JWT AUTHENTICATION =========================
var jwtSection = configuration.GetSection("JwtSettings");
var key = Encoding.UTF8.GetBytes(jwtSection["Key"]!);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;   // set true in production
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSection["Issuer"],
        ValidAudience = jwtSection["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(key)
    };
});

// ========================= CORS (optional but recommended) =========================
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", p =>
        p.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
});

// ========================= SWAGGER =========================
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Department Store Management API",
        Version = "v1",
        Description = "API for Department Store Management (DSM) Project"
    });

    // JWT in Swagger UI
    var jwtSecurityScheme = new OpenApiSecurityScheme
    {
        Scheme = "bearer",
        BearerFormat = "JWT",
        Name = "Authorization",
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

// ========================= BUILD APP =========================
var app = builder.Build();

// ========================= SEED DATABASE =========================
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    await SeedDb.InitializeAsync(services);
}

// ========================= MIDDLEWARE PIPELINE =========================
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseExceptionHandler("/error");
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseCors("AllowAll");          // <-- remove or restrict in production

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();