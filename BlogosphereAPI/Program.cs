using BlogosphereAPI.Data;
using BlogosphereAPI.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<BlogosphereDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("dbcs")));
//builder.Services.AddCors(options =>
//{
//    options.AddPolicy("AllowReactApp", policy =>
//    {
//        policy.WithOrigins("http://localhost:5173") // Allow requests from React app
//              .AllowAnyHeader() // Allow all headers
//              .AllowAnyMethod(); // Allow GET, POST, PUT, DELETE, etc.
//    });

//});
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowGateway", policy =>
    {
        policy.WithOrigins("https://localhost:7400") // Allow requests from the gateway
              .AllowAnyHeader() // Allow all headers
              .AllowAnyMethod() // Allow all HTTP methods
              .AllowCredentials(); // Allow cookies if needed
    });
});
builder.Services.AddControllers();
builder.Services.AddScoped<ITagRepository, TagRepository>();
builder.Services.AddScoped<IBlogPostRepository, BlogPostRepository>();
builder.Services.AddScoped<IBlogPostLikeRepository, BlogPostLikeRepository>();
builder.Services.AddScoped<IimageRepository, ImageRepository>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
//app.UseCors("AllowReactApp");
app.UseCors("AllowGateway");
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
