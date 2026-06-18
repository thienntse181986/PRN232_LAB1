using Microsoft.EntityFrameworkCore;
using PRN232.LMS.Repositories.Context;
using PRN232.LMS.Repositories.Implementations;
using PRN232.LMS.Repositories.Interfaces;
using PRN232.LMS.Services.Implementations;
using PRN232.LMS.Services.Interfaces;
using PRN232.LMS.Repositories.Generic;
using System.Reflection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Asp.Versioning;
using Asp.Versioning.ApiExplorer;
using FluentValidation;
using FluentValidation.AspNetCore;
using PRN232.LMS.API.Middleware;
using PRN232.LMS.Services.Helpers;
using PRN232.LMS.Services.Validation;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// ──────────────────────────────
// Database
// ──────────────────────────────
builder.Services.AddDbContext<LmsDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"),
        sql => sql.MigrationsAssembly("PRN232.LMS.Repositories")));

// ──────────────────────────────
// Repositories & UnitOfWork
// ──────────────────────────────
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

builder.Services.AddScoped<IStudentRepository, StudentRepository>();
builder.Services.AddScoped<ICourseRepository, CourseRepository>();
builder.Services.AddScoped<ISubjectRepository, SubjectRepository>();
builder.Services.AddScoped<ISemesterRepository, SemesterRepository>();
builder.Services.AddScoped<IEnrollmentRepository, EnrollmentRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();

// ──────────────────────────────
// Helpers & Core Services
// ──────────────────────────────
builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();
builder.Services.AddScoped<IAuthService, AuthService>();

// ──────────────────────────────
// Business Services
// ──────────────────────────────
builder.Services.AddScoped<IStudentService, StudentService>();
builder.Services.AddScoped<ICourseService, CourseService>();
builder.Services.AddScoped<ISubjectService, SubjectService>();
builder.Services.AddScoped<ISemesterService, SemesterService>();
builder.Services.AddScoped<IEnrollmentService, EnrollmentService>();

// ──────────────────────────────
// Controllers & Content Negotiation
// ──────────────────────────────
builder.Services.AddControllers(options =>
{
    options.Filters.Add<PRN232.LMS.API.Filters.DataShapingFilter>();
    options.RespectBrowserAcceptHeader = true;
    options.ReturnHttpNotAcceptable = true; // Returns 406 Not Acceptable for unsupported formats
})
.AddXmlSerializerFormatters(); // Enable XML support

// ──────────────────────────────
// JWT Authentication
// ──────────────────────────────
var secret = Environment.GetEnvironmentVariable("JWT_SECRET")
             ?? builder.Configuration["Jwt:Secret"]
             ?? "FallbackSuperSecretJWTKey12345678901234567890";

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
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret)),
        ValidateIssuer = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"] ?? "LmsApi",
        ValidateAudience = true,
        ValidAudience = builder.Configuration["Jwt:Audience"] ?? "LmsClient",
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
    };
});

// ──────────────────────────────
// FluentValidation
// ──────────────────────────────
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<StudentCreateRequestValidator>();

// ──────────────────────────────
// API Versioning
// ──────────────────────────────
builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;
    options.ApiVersionReader = new UrlSegmentApiVersionReader();
})
.AddApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV";
    options.SubstituteApiVersionInUrl = true;
});

// ──────────────────────────────
// Swagger / OpenAPI
// ──────────────────────────────
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    // Resolve the ApiVersionDescriptionProvider dynamically
    var provider = builder.Services.BuildServiceProvider().GetRequiredService<IApiVersionDescriptionProvider>();
    
    foreach (var description in provider.ApiVersionDescriptions)
    {
        c.SwaggerDoc(description.GroupName, new OpenApiInfo
        {
            Title = $"PRN232 LMS API {description.GroupName.ToUpperInvariant()}",
            Version = description.ApiVersion.ToString(),
            Description = $"Learning Management System RESTful API built with ASP.NET Core 8 - Version {description.ApiVersion}",
            Contact = new OpenApiContact
            {
                Name = "Nguyen Trong Thien",
                Email = "SE181986@fpt.edu.vn"
            }
        });
    }

    // Configure Swagger JWT Authentication
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Enter 'Bearer' [space] and then your token in the text input below. Example: 'Bearer 12345abcdef'",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                },
                Scheme = "oauth2",
                Name = "Bearer",
                In = ParameterLocation.Header
            },
            new List<string>()
        }
    });

    // Include XML comments for Swagger
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
        c.IncludeXmlComments(xmlPath);
});

// ──────────────────────────────
// CORS
// ──────────────────────────────
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
        policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
});

var app = builder.Build();

// ──────────────────────────────
// Global Middlewares (First in Pipeline)
// ──────────────────────────────
app.UseMiddleware<RequestLoggingMiddleware>();
app.UseMiddleware<ExceptionMiddleware>();

// ──────────────────────────────
// Apply Migrations + Seed on Startup
// ──────────────────────────────
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<LmsDbContext>();
    db.Database.Migrate();
}

// ──────────────────────────────
// Swagger Pipeline
// ──────────────────────────────
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    var provider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();
    foreach (var description in provider.ApiVersionDescriptions)
    {
        c.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json", $"PRN232 LMS API {description.GroupName.ToUpperInvariant()}");
    }
    c.RoutePrefix = string.Empty; // Swagger at root
});

app.UseCors("AllowAll");
app.UseHttpsRedirection();

// UseAuthentication MUST run before UseAuthorization
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
