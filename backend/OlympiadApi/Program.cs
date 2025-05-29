using OlympiadApi.Data;
using OlympiadApi.DTos;
using OlympiadApi.Services;
using OlympiadApi.Helpers;
using OlympiadApi.Repositories.Implementations;
using OlympiadApi.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using OlympiadApi.Filters;
using OlympiadApi.Services.Interfaces;
using System.Collections;

namespace OlympiadApi
{
    public partial class Program
    {
        public static WebApplication CreateApp(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Configuration.AddEnvironmentVariables();

            DotNetEnv.Env.Load();

            MapEnvironmentVariablesToConfiguration(builder.Configuration);

            // Load configuration from appsettings.json
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
            ValidateConnectionString(connectionString);

            ValidateJwtConfiguration(builder.Configuration);
            var secretKey = builder.Configuration["JWT_SECRET_KEY"];
            var issuer = builder.Configuration["JWT_ISSUER"];
            var audience = builder.Configuration["JWT_AUDIENCE"];

            // DatabaseHelper for the connection string
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

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
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey!))
                    };
                });

            builder.Services.AddAuthorization();

            builder.Services.AddScoped<IJwtHelper, JwtHelper>();
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<IRoleService, RoleService>();
            builder.Services.AddScoped<IAcademicYearService, AcademicYearService>();
            builder.Services.AddScoped<IOlympiadService, OlympiadService>();
            builder.Services.AddScoped<IUserRoleAssignmentService, UserRoleAssignmentService>();
            builder.Services.AddScoped<IStudentOlympiadEnrollmentService, StudentOlympiadEnrollmentService>();
            builder.Services.AddScoped<IAuthService, AuthService>();
            builder.Services.AddScoped<IEmailService, EmailService>();

            builder.Services.AddScoped<IUserRepository, UserRepository>();
            builder.Services.AddScoped<IAcademicYearRepository, AcademicYearRepository>();
            builder.Services.AddScoped<IOlympiadRepository, OlympiadRepository>();
            builder.Services.AddScoped<IRoleRepository, RoleRepository>();
            builder.Services.AddScoped<IStudentOlympiadEnrollmentRepository, StudentOlympiadEnrollmentRepository>();
            builder.Services.AddScoped<IUserRoleAssignmentRepository, UserRoleAssignmentRepository>();
            builder.Services.AddScoped<IAuthRepository, AuthRepository>();

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // Configure EmailService
            builder.Services.Configure<SmtpSettings>(builder.Configuration.GetSection("SmtpSettings"));
            builder.Services.AddScoped<ISmtpClient, SmtpClientWrapper>();
            builder.Services.AddScoped<Func<ISmtpClient>>(sp => () => sp.GetRequiredService<ISmtpClient>());
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

            return builder.Build();
        }

        public static void MapEnvironmentVariablesToConfiguration(ConfigurationManager config, IDictionary? envVars = null)
        {
            envVars ??= Environment.GetEnvironmentVariables();

            foreach (DictionaryEntry pair in envVars)
            {
                config[pair.Key!.ToString()!] = pair.Value?.ToString();
            }
        }

        public static void ValidateConnectionString(string? connectionString)
        {
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException("The connection string 'DefaultConnection' was not found or is empty. Please check your configuration.");
            }
        }

        public static void ValidateJwtConfiguration(IConfiguration config)
        {
            var secretKey = config["JWT_SECRET_KEY"];
            if (string.IsNullOrWhiteSpace(secretKey))
                throw new InvalidOperationException("JWT_SECRET_KEY is missing or empty. Please check your environment variables or appsettings.json.");

            var issuer = config["JWT_ISSUER"];
            if (string.IsNullOrWhiteSpace(issuer))
                throw new InvalidOperationException("JWT_ISSUER is missing or empty. Please check your environment variables or appsettings.json.");

            var audience = config["JWT_AUDIENCE"];
            if (string.IsNullOrWhiteSpace(audience))
                throw new InvalidOperationException("JWT_AUDIENCE is missing or empty. Please check your environment variables or appsettings.json.");
        }
    }

    public static class AppRunner
    {
        public static void Main(string[] args)
        {
            var app = Program.CreateApp(args);

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
        }
    }
}
