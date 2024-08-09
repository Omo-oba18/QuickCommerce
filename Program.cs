using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using QuickCommerce.Models;
using QuickCommerce.Services;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Load configuration from appsettings.json
var configuration = builder.Configuration;

// Retrieve MongoDB configuration values
var connectionString = configuration.GetSection("QuickCommerceStoreDatabase:ConnectionString").Value;
var databaseName = configuration.GetSection("QuickCommerceStoreDatabase:DatabaseName").Value;
var userCollectionName = configuration.GetSection("QuickCommerceStoreDatabase:Collections:UserCollectionName").Value;
var orderCollectionName = configuration.GetSection("QuickCommerceStoreDatabase:Collections:OrderCollectionName").Value;

// Configure JWT Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = configuration["Jwt:Issuer"],
            ValidAudience = configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]))
        };
    });

// Register MongoDB services
builder.Services.AddSingleton<IMongoClient, MongoClient>(sp =>
{
    return new MongoClient(connectionString);
});
builder.Services.AddSingleton<JwtService>();


builder.Services.AddScoped<IMongoDatabase>(sp =>
{
    var client = sp.GetRequiredService<IMongoClient>();
    return client.GetDatabase(databaseName);
});

builder.Services.AddScoped(sp =>
{
    var database = sp.GetRequiredService<IMongoDatabase>();
    return database.GetCollection<User>(userCollectionName);
});

builder.Services.AddScoped(sp =>
{
    var database = sp.GetRequiredService<IMongoDatabase>();
    return database.GetCollection<Order>(orderCollectionName);
});

// Register user service
builder.Services.AddScoped<UserService>();

// Add controllers and services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
