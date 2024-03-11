using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ChatroomB_Backend.Data;
using ChatroomB_Backend.Service;
using ChatroomB_Backend.Repository;
using ChatroomB_Backend.Utils;
using Microsoft.Data.SqlClient;
using System.Configuration;
using System.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using ChatroomB_Backend.Hubs;
using StackExchange.Redis;
using SixLabors.ImageSharp;
using Microsoft.AspNetCore.Diagnostics;
using System.Net;
using ChatroomB_Backend.Middleware;
using Microsoft.AspNetCore.CookiePolicy;
using Microsoft.AspNetCore.SignalR;
using MongoDB.Driver;
using ChatroomB_Backend.Models;
using System;
using AspNetCoreRateLimit;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ChatroomB_BackendContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("ChatroomB_BackendContext") ?? throw new InvalidOperationException("Connection string 'ChatroomB_BackendContext' not found.")));

//dapper
builder.Services.AddTransient<IDbConnection>((sp) =>
           new SqlConnection(builder.Configuration.GetConnectionString("ChatroomB_BackendContext")));

// Add Cache
builder.Services.AddOptions();
builder.Services.AddMemoryCache();

// Add Rate Limiting
builder.Services.Configure<IpRateLimitOptions>(builder.Configuration.GetSection("IpRateLimiting"));
builder.Services.AddInMemoryRateLimiting();
builder.Services.AddSingleton<IIpPolicyStore, MemoryCacheIpPolicyStore>();
builder.Services.AddSingleton<IRateLimitCounterStore, MemoryCacheRateLimitCounterStore>();
builder.Services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();

//redis set up
builder.Services.AddSingleton<IConnectionMultiplexer>(provider =>
{
    ConfigurationOptions configuration = ConfigurationOptions.Parse(builder.Configuration.GetSection("RedisConnection")["RedisConnectionString"]!);
    return ConnectionMultiplexer.Connect(configuration);
});

//MongoDB set up
builder.Services.AddSingleton<IMongoClient>(provider =>
{
    string connectionString = builder.Configuration.GetSection("MongoDBConnection")["MongoDBConnectionString"]!;
    return new MongoClient(connectionString);
});

builder.Services.AddSingleton(provider =>
{
    IMongoClient mongoClient = provider.GetRequiredService<IMongoClient>();
    string defaultDatabaseName = builder.Configuration.GetSection("MongoDBConnection")["DatabaseName"]!;
    string defaultCollectionName = builder.Configuration.GetSection("MongoDBConnection")["CollectionName"]!;

    IMongoDatabase database = mongoClient.GetDatabase(defaultDatabaseName);
    IMongoCollection<ErrorHandle> collection = database.GetCollection<ErrorHandle>(defaultCollectionName);
    return collection;
});

// Add Cookie Policy
IWebHostEnvironment environment = builder.Environment;
builder.Services.Configure<CookiePolicyOptions>(options =>
{
    if (environment.IsProduction())
    {
        // Production cookie policy
        options.CheckConsentNeeded = context => true;
        options.MinimumSameSitePolicy = SameSiteMode.None;
        options.HttpOnly = HttpOnlyPolicy.Always;
        options.Secure = CookieSecurePolicy.SameAsRequest;
    }
    else
    {
        // Development cookie policy
        options.CheckConsentNeeded = context => false;
        options.MinimumSameSitePolicy = SameSiteMode.None;
        options.HttpOnly = HttpOnlyPolicy.None;
        options.Secure = CookieSecurePolicy.SameAsRequest;
    }
});

// Add services to the container.
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = null;
    });

// SignalR service
builder.Services.AddSignalR()
    .AddJsonProtocol(options =>
    {
        options.PayloadSerializerOptions.PropertyNamingPolicy = null;

    });


// service repository utils
builder.Services.AddScoped<IUserRepo, UsersRepo>();
builder.Services.AddScoped<IUserService, UsersServices>();
builder.Services.AddScoped<IFriendService, FriendsServices>();
builder.Services.AddScoped<IAuthService, AuthServices>();
builder.Services.AddSingleton<ITokenService, TokenServices>();
builder.Services.AddScoped<IMessageService, MessagesServices>();
builder.Services.AddScoped<IErrorHandleService, ErrorHandleServices>();

builder.Services.AddScoped<IChatRoomRepo, ChatRoomRepo>();
builder.Services.AddScoped<IUserRepo, UsersRepo>();
builder.Services.AddScoped<IFriendRepo, FriendsRepo>();
builder.Services.AddScoped<IAuthRepo, AuthRepo>();
builder.Services.AddSingleton<ITokenRepo, TokenRepo>();
builder.Services.AddScoped<IMessageRepo, MessagesRepo>();
builder.Services.AddScoped<IErrorHandleRepo, ErrorHandleRepo>();


builder.Services.AddScoped<IAuthUtils, AuthUtils>();
builder.Services.AddSingleton<ITokenUtils, TokenUtils>();

// RabbitMQ-Related Services
builder.Services.AddSingleton<RabbitMQServices>();
builder.Services.AddScoped<ApplicationServices>();
builder.Services.AddScoped<IBlobService, BlobServices>();
builder.Services.AddScoped<IBlobRepo, BlobsRepo>();

builder.Services.AddScoped<IChatRoomService, ChatRoomServices>();
builder.Services.AddScoped<IChatRoomRepo, ChatRoomRepo>();

//Redis
builder.Services.AddScoped<IRedisServcie, RedisService>();
builder.Services.AddScoped<IRedisRepo, RedisRepo>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();



// add policy
builder.Services.AddCors(options =>
{
    options.AddPolicy("AngularApp", policy =>
            policy.WithOrigins("http://localhost:4200", "https://chatroomfe-dec.azurewebsites.net")
                  .AllowAnyMethod()
                  .AllowAnyHeader()
                  .AllowCredentials());
});

// add jwt bearer authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,

            ValidIssuer = builder.Configuration.GetSection("JwtSettings:Issuer").Get<string>(),
            ValidAudience = builder.Configuration.GetSection("JwtSettings:Audience").Get<string>(),
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration.GetSection("JwtSettings:SecretKey").Get<string>()!))
        };
    });


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "API");
    });
}

app.UseHttpsRedirection();

// Apply cookie policy.
app.UseCookiePolicy();

// Placement of UseCors is crucial to ensure it's applied correctly.
app.UseCors("AngularApp");

app.UseRouting();

// Ensure that authentication and authorization come after UseRouting and UseCors.
app.UseAuthentication();
app.UseAuthorization();

// SignalR hubs registration.
app.MapHub<ChatHub>("/chatHub");

// IP rate limiting middleware can be used after authorization.
app.UseIpRateLimiting();

// Custom middleware for token validation and exception handling.
app.UseMiddleware<TokenValidationMiddleware>();
app.UseMiddleware<ExceptionHandlingMiddleware>();

// Mapping controllers should come after all middleware are configured.
app.MapControllers();

app.Run();
