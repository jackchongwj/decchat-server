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

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ChatroomB_BackendContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("ChatroomB_BackendContext") ?? throw new InvalidOperationException("Connection string 'ChatroomB_BackendContext' not found.")));

builder.Services.AddTransient<IDbConnection>((sp) =>
           new SqlConnection(builder.Configuration.GetConnectionString("ChatroomB_BackendContext")));

builder.Services.AddControllers();

// SignalR service
builder.Services.AddSignalR();

// service repository utils
builder.Services.AddScoped<IUserRepo, UsersRepo>();
builder.Services.AddScoped<IUserService, UsersServices>();
builder.Services.AddScoped<IFriendService, FriendsServices>();
builder.Services.AddScoped<IAuthService, AuthServices>();
builder.Services.AddScoped<ITokenService, TokenServices>();
builder.Services.AddScoped<IMessageService, MessagesServices>();

builder.Services.AddScoped<IUserRepo, UsersRepo>();
builder.Services.AddScoped<IFriendRepo, FriendsRepo>();
builder.Services.AddScoped<IAuthRepo, AuthRepo>();  
builder.Services.AddScoped<ITokenRepo, TokenRepo>();
builder.Services.AddScoped<IMessageRepo, MessagesRepo>();

builder.Services.AddScoped<IAuthUtils, AuthUtils>();
builder.Services.AddScoped<ITokenUtils, TokenUtils>();

// RabbitMQ-Related Services
builder.Services.AddSingleton<RabbitMQServices>();
builder.Services.AddScoped<ApplicationServices>();
builder.Services.AddScoped<BlobServices>();
builder.Services.AddScoped<IBlobRepo, BlobsRepo>();

builder.Services.AddScoped<IChatRoomService, ChatRoomServices>();
builder.Services.AddScoped<IChatRoomRepo, ChatRoomRepo>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// add policy
builder.Services.AddCors(options => {
    options.AddPolicy("AngularApp",
            builder => builder.WithOrigins("http://localhost:4200")
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
            ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
            ValidAudience = builder.Configuration["JwtSettings:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:SecretKey"])),
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = false,
            ValidateIssuerSigningKey = true
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

app.UseCors("AngularApp");

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

//app.UseEndpoints(endpoints =>
//{
//    endpoints.MapHub<ChatHub>("/chatHub");
//});

app.MapControllers();

app.MapHub<ChatHub>("/chatHub");

app.Run();


