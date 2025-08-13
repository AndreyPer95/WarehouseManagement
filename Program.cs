using Microsoft.EntityFrameworkCore;
using WarehouseManagement.Data;
using WarehouseManagement.Services.Interfaces;
using WarehouseManagement.Services.Implementations;
using WarehouseManagement.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Add Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Warehouse Management API",
        Version = "v1",
        Description = "API для управления складом"
    });
});

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});

// Configure Database
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<WarehouseContext>(options =>
    options.UseNpgsql(connectionString));

// Register Services
builder.Services.AddScoped<IResourceService, ResourceService>();
builder.Services.AddScoped<IUnitService, UnitService>();
builder.Services.AddScoped<IReceiptService, ReceiptService>();
builder.Services.AddScoped<IWarehouseService, WarehouseService>();

var app = builder.Build();

// Initialize Database
app.InitDatabase();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Warehouse Management API v1");
        c.RoutePrefix = string.Empty; // Swagger UI на корневом URL
    });
}

app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseAuthorization();
app.MapControllers();

app.Run();