using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
// Add CORS policy for the gateway
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp", policy =>
    {
        policy.WithOrigins("http://localhost:5173") // Allow requests from your React app
              .AllowAnyHeader() // Allow all headers
              .AllowAnyMethod() // Allow all HTTP methods
              .AllowCredentials(); // Allow cookies if needed
    });
});
// Add authentication for JWT tokens
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["JWT:ValidIssuer"], 
            ValidAudience = builder.Configuration["JWT:ValidAudience"], 
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:Secret"])) 
        };
    });

builder.Configuration.AddJsonFile("ocelot.json", optional: false, reloadOnChange: true);
builder.Services.AddOcelot(builder.Configuration);
//builder.Services.AddOcelot();
var app = builder.Build();
// Apply the CORS policy
app.UseCors("AllowReactApp");
//app.MapControllers();
//Use Ocelot middleware
await app.UseOcelot();

// Use authentication middleware
app.UseAuthentication();
app.UseAuthorization();
app.Run();
