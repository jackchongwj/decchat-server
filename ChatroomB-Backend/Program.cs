﻿using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ChatroomB_Backend.Data;
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ChatroomB_BackendContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("ChatroomB_BackendContext") ?? throw new InvalidOperationException("Connection string 'ChatroomB_BackendContext' not found.")));

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//add policy
builder.Services.AddCors(options => {
    options.AddPolicy("AngularApp",
            builder => builder.WithOrigins("http://localhost:4200/")
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

app.Run();
