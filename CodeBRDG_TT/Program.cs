using CodeBRDG_TT.Data;
using Microsoft.EntityFrameworkCore;
using MediatR;
using System.Reflection;
using CodeBRDG_TT.Data.UnitOfWork;
using CodeBRDG_TT.Validators;
using FluentValidation;
using FluentValidation.AspNetCore;
using System.Threading.RateLimiting;
using CodeBRDG_TT.Middlewares;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRateLimiter(options =>
{
    options.AddPolicy("TenRequestsPerSecond", context =>
        RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: context.Connection.RemoteIpAddress?.ToString() ?? "global",
            factory: partition => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 10,               
                Window = TimeSpan.FromSeconds(1), 
                QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                QueueLimit = 0                  
            }));
});

builder.Services.AddControllers();
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddFluentValidationClientsideAdapters();

builder.Services.AddValidatorsFromAssemblyContaining<DogCommandValidator>();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));  

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationPipelineBehavior<,>));

builder.Services.AddMediatR(Assembly.GetExecutingAssembly());

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();
app.UseMiddleware<JsonValidationMiddleware>();

app.Use(async (context, next) =>
{
    await next();

    if (context.Response.StatusCode == 429)
    {
        context.Response.ContentType = "application/json";
        await context.Response.WriteAsync("{\"error\": \"Too many requests. (More than 10 per second)\"}");
    }
});


app.MapControllers().RequireRateLimiting("TenRequestsPerSecond");

app.MapGet("/ping", () => Results.Text("Dogshouseservice.Version1.0.1"));

app.Run();
