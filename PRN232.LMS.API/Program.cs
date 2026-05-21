using Microsoft.EntityFrameworkCore;
using PRN232.LMS.Repositories.Context;
using PRN232.LMS.Repositories.Implementations;
using PRN232.LMS.Repositories.Interfaces;
using PRN232.LMS.Services.Implementations;
using PRN232.LMS.Services.Interfaces;
using PRN232.LMS.Repositories.Generic;
using System.Reflection;

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

// ──────────────────────────────
// Services
// ──────────────────────────────
builder.Services.AddScoped<IStudentService, StudentService>();
builder.Services.AddScoped<ICourseService, CourseService>();
builder.Services.AddScoped<ISubjectService, SubjectService>();
builder.Services.AddScoped<ISemesterService, SemesterService>();
builder.Services.AddScoped<IEnrollmentService, EnrollmentService>();

// ──────────────────────────────
// Controllers
// ──────────────────────────────
builder.Services.AddControllers(options =>
{
    options.Filters.Add<PRN232.LMS.API.Filters.DataShapingFilter>();
})
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.DefaultIgnoreCondition =
            System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
    });

// ──────────────────────────────
// Swagger / OpenAPI
// ──────────────────────────────
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "PRN232 LMS API",
        Version = "v1",
        Description = "Learning Management System RESTful API built with ASP.NET Core 8",
        Contact = new Microsoft.OpenApi.Models.OpenApiContact
        {
            Name = "Nguyen Trong Thien",
            Email = "SE181986@fpt.edu.vn"
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
// Apply Migrations + Seed on Startup
// ──────────────────────────────
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<LmsDbContext>();
    db.Database.Migrate();
}

// ──────────────────────────────
// Middleware pipeline
// ──────────────────────────────
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "PRN232 LMS API v1");
    c.RoutePrefix = string.Empty; // Swagger at root
});

app.UseCors("AllowAll");
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
