using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ChatroomB_Backend.Data;
using ChatroomB_Backend.Service;
using ChatroomB_Backend.Repository;
using Microsoft.Data.SqlClient;
using System.Configuration;
using System.Data;
using ChatroomB_Backend.Hubs;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ChatroomB_BackendContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("ChatroomB_BackendContext") ?? throw new InvalidOperationException("Connection string 'ChatroomB_BackendContext' not found.")));

builder.Services.AddTransient<IDbConnection>((sp) =>
           new SqlConnection(builder.Configuration.GetConnectionString("ChatroomB_BackendContext")));

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddSignalR();

//build service and repository
builder.Services.AddScoped<IUserService, UsersService>();
builder.Services.AddScoped<IUserRepo, UsersRepo>();

builder.Services.AddScoped<IFriendService, FriendsServices>();
builder.Services.AddScoped<IFriendRepo, FriendsRepo>();

builder.Services.AddScoped<IMessageService, MessagesServices>();
builder.Services.AddScoped<IMessageRepo, MessagesRepo>();

// RabbitMQ-Related Services
builder.Services.AddSingleton<RabbitMQServices>();
builder.Services.AddScoped<ApplicationServices>();
builder.Services.AddScoped<BlobServices>();
builder.Services.AddScoped<IBlobRepo, BlobsRepo>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//add policy
builder.Services.AddCors(options => {
    options.AddPolicy("AngularApp",
            builder => builder.WithOrigins("http://localhost:4200")
                              .AllowAnyMethod()
                              .AllowAnyHeader()
                              .AllowCredentials());
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

///use cors
app.UseCors("AngularApp");

app.UseAuthorization();

app.MapControllers();

app.MapHub<ChatHub>("/chatHub");

app.Run();
