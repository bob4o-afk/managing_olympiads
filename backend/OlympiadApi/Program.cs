using OlympiadApi.Data;
using OlympiadApi.Services;
using Microsoft.EntityFrameworkCore;
using MySQLRandomNumberApp.Data;

var builder = WebApplication.CreateBuilder(args);

// Load configuration from appsettings.json
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// Add DatabaseHelper as a singleton and pass the connection string
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

builder.Services.AddSingleton(new DatabaseHelper(connectionString));

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
