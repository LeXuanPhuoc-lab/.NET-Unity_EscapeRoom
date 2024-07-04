using System.Text.Json.Serialization;
using AutoMapper;
using EscapeRoomAPI.Data;
using EscapeRoomAPI.Entities;
using EscapeRoomAPI.Hubs;
using EscapeRoomAPI.Mappings;
using EscapeRoomAPI.Payloads.Requests;
using EscapeRoomAPI.Services;
using EscapeRoomAPI.Utils;
using EscapeRoomAPI.Validations;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


// Add DbContext
builder.Services.AddDbContext<EscapeRoomUnityContext>(opt =>
{
    opt.UseSqlServer(builder.Configuration.GetConnectionString("DefaultDB"));
});

// Add FluentValidation
builder.Services.AddScoped<IValidator<SignInRequest>, SignInValidator>();
builder.Services.AddScoped<IValidator<RegisterRequest>, RegisterValidator>();
builder.Services.AddScoped<IValidator<CreateRoomRequest>, CreateRoomValidator>();
builder.Services.AddScoped<IValidator<IFormFile>, ImageFileValidator>();

// Add Database Initializer
builder.Services.AddScoped<DatabaseInitializer>();

// Add AutoMapper
var mapperConfig = new MapperConfiguration(mc =>
{
    mc.AddProfile<MappingProfile>();
});
builder.Services.AddSingleton(mapperConfig.CreateMapper());

//SignalR configuration
builder.Services.AddSignalR()
    .AddJsonProtocol(options => {
        // Serialize not change the casing of property name, instead of (Camel Case)
        options.PayloadSerializerOptions.PropertyNamingPolicy = null;
    });

// Config AppSettings
builder.Services.Configure<FirebaseCredentials>(builder.Configuration.GetSection("FirebaseCredentials"));
builder.Services.Configure<AppSettings>(builder.Configuration.GetSection("AppSettings"));

// Firebase 
builder.Services.AddSingleton<IFirebaseService, FirebaseService>();

// Add CORS
builder.Services.AddCors(p => p.AddPolicy("Cors", policy =>
{
    policy.WithOrigins("*")
          .AllowAnyHeader()
          .SetIsOriginAllowed(_ => true)
          .AllowAnyMethod();
}));


// Configure HttpClient
builder.Services.AddHttpClient("Unity", httpClient =>
{
    //httpClient.BaseAddress = new Uri("https://escaperoom.ddnsking.com");
    httpClient.BaseAddress = new Uri("https://localhost:7000");
});

var app = builder.Build();

// Hook into application lifetime events and trigger only application fully started
app.Lifetime.ApplicationStarted.Register(() =>
{
    // Database Initializer
    Task.Run(async () =>
    {
        await app.InitializeDatabaseAsync();
    });
});

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("Cors");

app.UseRouting();

app.UseAuthorization();

app.MapHub<StartRoomHub>("/start-room");
app.MapControllers(); 

app.Run();
