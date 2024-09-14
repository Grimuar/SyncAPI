using DreamsSyncronizer;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using DreamsSyncronizer.Common;
using System.Text.Json.Serialization;
using System.Reflection;
using DreamsSyncronizer.Controllers;
using DreamsSyncronizer.Middleware;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddTransient<ErrorHandlerMiddleware>();

Log.Logger = new LoggerConfiguration()
             .ReadFrom.Configuration(builder.Configuration)
             .Enrich.FromLogContext()
             .CreateLogger();

builder.Host.UseSerilog();

var connectionString = builder.Configuration.GetValue<string>("DbConnection");
var serverVersion = new MySqlServerVersion(new Version(8, 3, 0));

builder.Services.AddDbContext<SleepTrackerDbContext>(options =>
{
    options.UseMySql(connectionString, serverVersion)
           .EnableDetailedErrors();
});

builder.Services.AddControllers();
builder.Services.AddAuthorization();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
       .AddJwtBearer(options =>
       {
           options.TokenValidationParameters = new TokenValidationParameters
           {
               ValidateIssuer = true,
               ValidIssuer = AuthOptions.ISSUER,
               ValidateAudience = true,
               ValidAudience = AuthOptions.AUDIENCE,
               ValidateLifetime = true,
               IssuerSigningKey = AuthOptions.GetSymmetricSecurityKey(),
               ValidateIssuerSigningKey = true,
           };
       });

builder.Services.AddHttpContextAccessor();
builder.Services.AddControllers()
       .AddJsonOptions(x =>
                           x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);

builder.Services.AddSwaggerGen(config =>
{
    //settings for use XML file
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";

    //here we use mirroring for create path to file
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    config.IncludeXmlComments(xmlPath);
});

var app = builder.Build();

app.UseMiddleware<ErrorHandlerMiddleware>();

using (var scope = app.Services.CreateScope())
{
    var serviceProvider = scope.ServiceProvider;
    try
    {
        var context = serviceProvider.GetRequiredService<SleepTrackerDbContext>();
        context.Database.Migrate();
        DbInitializer.Initialize(context);
    }
    catch (Exception exception)
    {
        //
    }
}

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseDefaultFiles();
app.UseStaticFiles();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});


app.Run();