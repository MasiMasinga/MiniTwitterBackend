using Tweet.Data;
using Tweet.Services;
using Tweet.Interfaces;
using Comment.Services;
using Comment.Interfaces;
using User.Interfaces;
using User.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddControllers();

var corsOrigins =
  builder.Configuration.GetSection("Cors:Origins").Get<string[]>() ??
  new[] { "http://localhost:3000", "http://localhost:5173" };

builder.Services.AddCors(options =>
{
  options.AddPolicy("CorsPolicy", policy =>
  {
    policy
      .WithOrigins(corsOrigins)
      .AllowAnyHeader()
      .AllowAnyMethod();
  });
});

builder.Services.AddScoped<ITweetService, TweetService>();
builder.Services.AddScoped<ICommentService, CommentService>();
builder.Services.AddScoped<IUserService, UserService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGet("/", () => "🚀 Mini Tweeter Backend is running!");

app.UseHttpsRedirection();
app.UseCors("CorsPolicy");
app.MapControllers();
app.Run();
 