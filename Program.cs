using Microsoft.EntityFrameworkCore;
using WarehouseManagement.Data;
using WarehouseManagement.Services.Interfaces;
using WarehouseManagement.Services.Implementations;
using WarehouseManagement.Extensions;

var builder = WebApplication.CreateBuilder(args);
ConfigureServices(builder.Services, builder.Configuration);
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

var app = builder.Build();
ConfigureInitialData(app);
ConfigureApplication(app);

app.Run();

void ConfigureServices(IServiceCollection services, IConfiguration configuration)
{
    services.AddControllersWithViews();
  
    var connectionString = configuration.GetConnectionString("DefaultConnection");
    services.AddDbContext<WarehouseContext>(options =>
        options.UseNpgsql(connectionString));

    services.AddScoped<IResourceService, ResourceService>();
    services.AddScoped<IUnitService, UnitService>();
    services.AddScoped<IReceiptService, ReceiptService>();
    services.AddScoped<IWarehouseService, WarehouseService>();
}

void ConfigureInitialData(WebApplication app)
{
    app.InitDatabase();
}

void ConfigureApplication(WebApplication app)
{
    app.UseHttpsRedirection();
    app.UseStaticFiles();
    app.UseRouting();
    app.UseAuthorization();
    
    app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Warehouse}/{action=Index}/{id?}");
}