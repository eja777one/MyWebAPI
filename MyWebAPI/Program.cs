using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using MyWebAPI.Attributes;
using MyWebAPI.Data;
using MyWebAPI.Repositories;
using MyWebAPI.Repositories.Interfaces;
using MyWebAPI.Services;
using MyWebAPI.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

#region DB

var connection = builder.Configuration["MainDb:ConnectionString"]
    + $";Username={builder.Configuration["MainDb:User"]}"
    + $";Password={builder.Configuration["MainDb:Pw"]}";

builder.Services.AddDbContext<MainDbContext>(options =>
options.UseNpgsql(connection), ServiceLifetime.Transient);

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true); // dateTime fix

#endregion

builder.Services.AddControllers(config =>
{
    config.Filters.Add<ValidateModelAttribute>();
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "ToDo API",
        Description = "An ASP.NET Core Web API for managing ToDo items",
        TermsOfService = new Uri("https://example.com/terms"),
        Contact = new OpenApiContact
        {
            Name = "Example Contact",
            Url = new Uri("https://example.com/contact")
        },
        License = new OpenApiLicense
        {
            Name = "Example License",
            Url = new Uri("https://example.com/license")
        }
    });

    // using System.Reflection;
    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
});

builder.Services.AddTransient<IVideoService, VideoService>();
builder.Services.AddTransient<IVideoRepository, VideoRepository>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

public partial class Program { }