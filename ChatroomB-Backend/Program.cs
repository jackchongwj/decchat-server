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

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ChatroomB_BackendContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("ChatroomB_BackendContext") ?? throw new InvalidOperationException("Connection string 'ChatroomB_BackendContext' not found.")));

//dapper
builder.Services.AddTransient<IDbConnection>((sp) =>
           new SqlConnection(builder.Configuration.GetConnectionString("ChatroomB_BackendContext")));


//redis set up
builder.Services.AddSingleton<IConnectionMultiplexer>(provider =>
{
    ConfigurationOptions configuration = ConfigurationOptions.Parse(builder.Configuration.GetSection("RedisConnection")["RedisConnectionString"]);
    return ConnectionMultiplexer.Connect(configuration);
});

//MongoDB set up
builder.Services.AddSingleton<IMongoClient>(provider =>
{
    string connectionString = builder.Configuration.GetSection("MongoDBConnection")["MongoDBConnectionString"];
    return new MongoClient(connectionString);
});

builder.Services.AddSingleton(provider =>
{
    var mongoClient = provider.GetRequiredService<IMongoClient>();
    var defaultDatabaseName = builder.Configuration.GetSection("MongoDBConnection")["DatabaseName"];
    var defaultCollectionName = builder.Configuration.GetSection("MongoDBConnection")["CollectionName"];

    var database = mongoClient.GetDatabase(defaultDatabaseName);
    var collection = database.GetCollection<ErrorHandle>(defaultCollectionName);
    return collection;
});



// Add Cookie Policy
builder.Services.Configure<CookiePolicyOptions>(options =>
{
    options.CheckConsentNeeded = context => true;
    options.MinimumSameSitePolicy = SameSiteMode.None; 
    options.HttpOnly = HttpOnlyPolicy.Always;
    options.Secure = CookieSecurePolicy.SameAsRequest; 
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
        
    }) ;


// service repository utils
builder.Services.AddScoped<IUserRepo, UsersRepo>();
builder.Services.AddScoped<IUserService, UsersServices>();
builder.Services.AddScoped<IFriendService, FriendsServices>();
builder.Services.AddScoped<IAuthService, AuthServices>();
builder.Services.AddScoped<ITokenService, TokenServices>();
builder.Services.AddScoped<IMessageService, MessagesServices>();
builder.Services.AddScoped<IErrorHandleService, ErrorHanldeServices>();

builder.Services.AddScoped<IChatRoomRepo, ChatRoomRepo>();
builder.Services.AddScoped<IUserRepo, UsersRepo>();
builder.Services.AddScoped<IFriendRepo, FriendsRepo>();
builder.Services.AddScoped<IAuthRepo, AuthRepo>();  
builder.Services.AddScoped<ITokenRepo, TokenRepo>();
builder.Services.AddScoped<IMessageRepo, MessagesRepo>();
builder.Services.AddScoped<IErrorHandleRepo, ErrorHandleRepo>();


builder.Services.AddScoped<IAuthUtils, AuthUtils>();
builder.Services.AddScoped<ITokenUtils, TokenUtils>();

// RabbitMQ-Related Services
builder.Services.AddSingleton<RabbitMQServices>();
builder.Services.AddScoped<ApplicationServices>();
builder.Services.AddScoped<IBlobService,BlobServices>();
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
            ValidateLifetime = false,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration.GetSection("JwtSettings:Issuer").Get<string>(),
            ValidAudience = builder.Configuration.GetSection("JwtSettings:Audience").Get<string>(),
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration.GetSection("JwtSettings:SecretKey").Get<string>()))
        };
    });


var app = builder.Build();

app.MapHub<ChatHub>("/chatHub");

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

app.UseCors("AngularApp");

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

app.UseMiddleware<ExecptionHandlingMiddleware>();

app.UseMiddleware<TokenValidationMiddleware>();

app.MapControllers();

app.Run();


