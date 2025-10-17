
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;
using Periodico.Context;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// CORS Configuration
builder.Services.AddCors(options =>
{
	options.AddPolicy("AllowAll", policyBuilder =>
	{
		policyBuilder.AllowAnyOrigin()
			 .AllowAnyHeader()
			 .AllowAnyMethod();
	});
});

// Database Configuration with retry logic
var connectionString = builder.Configuration.GetConnectionString("AzureSQL");
builder.Services.AddDbContext<AppDbContext>(options =>
	options.UseSqlServer(connectionString, sqlOptions =>
	{
		sqlOptions.EnableRetryOnFailure(
			maxRetryCount: 5,
			maxRetryDelay: TimeSpan.FromSeconds(30),
			errorNumbersToAdd: null);
	}));

// JWT Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
	.AddJwtBearer(options =>
	{
		options.TokenValidationParameters = new TokenValidationParameters
		{
			ValidateIssuerSigningKey = true,
			IssuerSigningKey = new SymmetricSecurityKey(
				Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])),
			ValidateIssuer = false,
			ValidateAudience = false
		};
	});

var app = builder.Build();

// Apply migrations
try
{
	using (var scope = app.Services.CreateScope())
	{
		var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
		if (dbContext.Database.IsRelational())
		{
			dbContext.Database.Migrate();
		}
	}
}
catch (Exception ex)
{
	Console.WriteLine($"Error applying migrations: {ex.Message}");
}

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();
app.UseStaticFiles(); // Esto sirve archivos de wwwroot
var uploadsPath = Path.Combine(builder.Environment.WebRootPath, "uploads");
if (!Directory.Exists(uploadsPath))
{
	Directory.CreateDirectory(uploadsPath);
}

app.UseStaticFiles(new StaticFileOptions
{
	FileProvider = new PhysicalFileProvider(uploadsPath),
	RequestPath = "/uploads"
});
app.MapControllers();

app.Run();