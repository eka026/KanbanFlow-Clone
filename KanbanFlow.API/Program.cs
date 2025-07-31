using Microsoft.EntityFrameworkCore;
using KanbanFlow.API.Data;

using KanbanFlow.Core.Interfaces;
using KanbanFlow.API.Data.Repositories;

var builder = WebApplication.CreateBuilder(args);

// --- Service Configuration ---

// Add DbContext for Entity Framework
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString(builder.Configuration["Database:ConnectionStringName"] ?? "DefaultConnection")));

// Add Unit of Work
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// Add support for controllers
builder.Services.AddControllers();

// Add AutoMapper
builder.Services.AddAutoMapper(typeof(Program));

// Add Swagger/OpenAPI for API documentation and testing
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// -- Application Pipeline Configuration --

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();