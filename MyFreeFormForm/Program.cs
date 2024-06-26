using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MyFreeFormForm.Core;
using MyFreeFormForm.Core.Repositories;
using MyFreeFormForm.Repositories;
using MyFreeFormForm.Data;
using MyFreeFormForm.Helpers;
using MyFreeFormForm.Services;
using Serilog;
using Serilog.Events;
using System.Text.Json.Serialization;
using MyFreeFormForm.Models;
using Nest;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Options;
using System.Configuration;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog
builder.Host.UseSerilog((context, services, configuration) => configuration
    .ReadFrom.Configuration(context.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {SourceContext} {Message:lj}{NewLine}{Exception}")
    .WriteTo.File("logs/My-FreeForm-Form-.txt", rollingInterval: RollingInterval.Day,
        outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss} {Level:u3}] {SourceContext} {Message:lj}{NewLine}{Exception}"));


// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddScoped<DataProtectorTokenProvider<MyIdentityUsers>, CustomEmailConfirmationTokenProvider<MyIdentityUsers>>();

// Add services to the container.
builder.Services.AddScoped<CustomEmailConfirmationTokenProvider<MyIdentityUsers>>();
builder.Services.Configure<EmailConfirmationTokenProviderOptions>(options =>
{
    options.TokenLifespan = TimeSpan.FromMinutes(15); // Adjust as needed
});

builder.Services.AddDefaultIdentity<MyIdentityUsers>(options =>
{
    options.SignIn.RequireConfirmedAccount = true;
    options.Tokens.ProviderMap.Add("EmailConfirmation", new TokenProviderDescriptor(typeof(CustomEmailConfirmationTokenProvider<MyIdentityUsers>)));
    options.Tokens.EmailConfirmationTokenProvider = "EmailConfirmation";
})
.AddRoles<IdentityRole>()
.AddEntityFrameworkStores<ApplicationDbContext>();



builder.Services.AddScoped<DataProtectorTokenProvider<MyIdentityUsers>, CustomEmailConfirmationTokenProvider<MyIdentityUsers>>();
builder.Services.AddScoped<CustomEmailConfirmationTokenProvider<MyIdentityUsers>>();

builder.Services.Configure<EmailSenderOptions>(builder.Configuration.GetSection("EmailSender"));
builder.Services.AddTransient<IEmailSender, EmailSender>();
builder.Services.Configure<EmailSender>(builder.Configuration);

builder.Services.AddControllersWithViews().AddSessionStateTempDataProvider(); 
builder.Services.AddRazorPages();
builder.Services.AddTransient<FileParser>();

builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(20); // You can adjust the timeout as needed
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true; // Make the session cookie essential
});
builder.Services.AddLogging();

builder.Services.AddScoped<FormsDbc>();

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

builder.Services.AddHostedService<QueueProcessor>();
builder.Services.AddScoped<DataService>();

builder.Services.AddScoped<IFormProcessorService, FormProcessorService>();

builder.Services.AddSingleton<IQueueProcessorMonitor, QueueProcessorMonitor>();

builder.Services.AddHealthChecks()
    .AddCheck<QueueProcessorHealthCheck>("queue_processor_health_check");

// In ConfigureServices
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin",
    builder => builder.WithOrigins("https://localhost:7222/") // Replace with your client's origin
        .AllowAnyMethod()
        .AllowAnyHeader());
});

// Add ElasticsearchService
/*builder.Services.AddSingleton<ElasticsearchService>(provider =>
{
    var uri = "http://localhost:9200";
    return new ElasticsearchService(uri);
});*/

builder.Services.AddScoped<SearchService>();


builder.Services.AddSignalR(hubOptions =>
{
    hubOptions.EnableDetailedErrors = true; // Enable detailed errors
});
// In Configure, before UseRouting or UseEndpoints
AddAuthorizationPolicies(builder.Services);
AddScoped(builder.Services);



var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}
app.UseSession();

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// use top level route registrations instead of UseEndpoints to add endpoint for SignalR /dataHub

app.UseCors("AllowSpecificOrigin");

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapHealthChecks("/health");


app.MapRazorPages();
//app.MapHub<DataHub>("/dataHub");
app.MapHub<DataHub>("/dataHub").RequireCors("AllowSpecificOrigin");


app.Run();

// Define the AddAuthorizationPolicies method
void AddAuthorizationPolicies(IServiceCollection services)
{
    services.AddAuthorization(options =>
    {
        // Define your policies here
        options.AddPolicy("EmployeeOnly", policy => policy.RequireClaim("EmployeeNumber"));

        options.AddPolicy(Constants.Policies.RequireAdmin, policy => policy.RequireRole(Constants.Roles.Administrator));
        options.AddPolicy(Constants.Policies.RequireManager, policy => policy.RequireRole(Constants.Roles.Manager));
    });
}

void AddScoped(IServiceCollection services)
{
    services.AddScoped<IUserRepository, UserRepository>();
    services.AddScoped<IRoleRepository, RoleRepository>();
    services.AddScoped<IUnitOfWork, UnitOfWork>();
}


public class EmailConfirmationTokenProviderOptions : DataProtectionTokenProviderOptions
{
    public EmailConfirmationTokenProviderOptions()
    {
        Name = "EmailDataProtectorTokenProvider";
        TokenLifespan = TimeSpan.FromHours(4);
    }
}

public class CustomEmailConfirmationTokenProvider<TUser>
                              : DataProtectorTokenProvider<TUser> where TUser : class
{
    public CustomEmailConfirmationTokenProvider(
        IDataProtectionProvider dataProtectionProvider,
        IOptions<EmailConfirmationTokenProviderOptions> options,
        ILogger<DataProtectorTokenProvider<TUser>> logger)
                                       : base(dataProtectionProvider, options, logger)
    {

    }
}
