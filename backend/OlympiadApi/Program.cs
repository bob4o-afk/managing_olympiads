using OlympiadApi.Data;
using OlympiadApi.Services;
using OlympiadApi.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using DotNetEnv; 

var builder = WebApplication.CreateBuilder(args);

DotNetEnv.Env.Load();


// Load configuration from appsettings.json
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

if (string.IsNullOrEmpty(connectionString))
{
    throw new InvalidOperationException("The connection string 'DefaultConnection' was not found or is empty. Please check your configuration.");
}

// Add DatabaseHelper as a singleton and pass the connection string
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

builder.Configuration.AddEnvironmentVariables();

var secretKey = builder.Configuration["JWT_SECRET_KEY"];
if (string.IsNullOrWhiteSpace(secretKey))
{
    throw new InvalidOperationException("JWT_SECRET_KEY is missing or empty. Please check your environment variables or appsettings.json.");
}

var issuer = builder.Configuration["JWT_ISSUER"];
if (string.IsNullOrWhiteSpace(issuer))
{
    throw new InvalidOperationException("JWT_ISSUER is missing or empty. Please check your environment variables or appsettings.json.");
}

var audience = builder.Configuration["JWT_AUDIENCE"];
if (string.IsNullOrWhiteSpace(audience))
{
    throw new InvalidOperationException("JWT_AUDIENCE is missing or empty. Please check your environment variables or appsettings.json.");
}

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidIssuer = issuer,
            ValidAudience = audience,
            ValidateLifetime = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
        };
    });

builder.Services.AddAuthorization();



builder.Services.AddScoped<RoleService>();
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<AcademicYearService>();
builder.Services.AddScoped<OlympiadService>(); 
builder.Services.AddScoped<UserRoleAssignmentService>();
builder.Services.AddScoped<StudentOlympiadEnrollmentService>();
builder.Services.AddScoped<JwtHelper>();


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure EmailService
builder.Services.Configure<SmtpSettings>(builder.Configuration.GetSection("SmtpSettings"));
builder.Services.AddTransient<IEmailService, EmailService>();

// Configure CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins",
        builder =>
        {
            builder.AllowAnyOrigin()
                   .AllowAnyMethod()
                   .AllowAnyHeader();
        });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Apply CORS policy
app.UseCors("AllowAllOrigins");

app.UseAuthorization();

app.MapControllers(); // Enable attribute routing for controllers

app.Run("http://0.0.0.0:5138"); // Listen on all network interfaces
