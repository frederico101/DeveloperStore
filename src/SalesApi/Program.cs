using MediatR;
using SalesApi.Application.Handlers;
using Microsoft.EntityFrameworkCore;
using Structure;
using SalesApi.Application.Mappings;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.WebHost.UseUrls("http://+:8090");

// Register MediatR
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(CreateSaleCommandHandler).Assembly));

builder.Services.AddAutoMapper(typeof(MappingProfile));

// Configure Entity Framework with SQL Server
builder.Services.AddDbContext<SalesDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));


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
