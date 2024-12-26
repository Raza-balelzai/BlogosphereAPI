using BlogosphereUserAPI.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add DbContext with Identity
builder.Services.AddDbContext<AuthDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("AuthDbcs")));

// Add Identity services with Entity Framework stores
builder.Services.AddIdentity<IdentityUser, IdentityRole>().AddEntityFrameworkStores<AuthDbContext>();

// Add JWT authentication
//builder.Services.AddAuthentication(options =>
//{
//    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
//    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
//})
//.AddJwtBearer(options =>
//{
//    options.RequireHttpsMetadata = false; // Set to true in production
//    options.SaveToken = true;
//    options.TokenValidationParameters = new TokenValidationParameters
//    {
//        ValidateIssuer = true,
//        ValidateAudience = false,
//        ValidateLifetime = true,
//        ValidateIssuerSigningKey = true,
//        ValidIssuer = builder.Configuration["JWT:ValidIssuer"],
//        //ValidAudience = builder.Configuration["JWT:ValidAudience"],
//        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:Secret"]))
//    };
//});
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

// Add controllers
builder.Services.AddControllers();

// Add Swagger for API documentation (if in development)
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

// Add authentication and authorization middleware
/*app.UseAuthentication()*/; // This enables JWT authentication
/*app.UseAuthorization()*/;  // This enables authorization based on roles and claims

app.MapControllers(); // Map the controllers

app.Run();
