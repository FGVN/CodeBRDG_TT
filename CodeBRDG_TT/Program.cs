using CodeBRDG_TT.Data.UnitOfWork;
using CodeBRDG_TT.Data;
using CodeBRDG_TT.Middlewares;
using FluentValidation.AspNetCore;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using System.Threading.RateLimiting;
using Microsoft.Extensions.Options;

public class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        var permitLimit = builder.Configuration.GetValue<int>("RateLimiting:PermitLimit");
        var windowInSeconds = builder.Configuration.GetValue<int>("RateLimiting:WindowInSeconds");

        // Register Rate Limiter with the policy
        builder.Services.AddRateLimiter(_ =>
        {
            _.OnRejected = (context, _) =>
            {
                // Set Retry-After header if applicable

                string retryMessage = string.Empty;
                if (context.Lease.TryGetMetadata(MetadataName.RetryAfter, out var retryAfter))
                {
                    context.HttpContext.Response.Headers.RetryAfter =
                        ((int)retryAfter.TotalSeconds).ToString();

                    // Create a retry message
                    retryMessage = $" Please try again in {retryAfter.TotalSeconds} seconds.";
                }


                // Set the response status code and content
                context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                context.HttpContext.Response.ContentType = "application/json";

                var errorMessage = $"{{\"error\": \"Too many requests. (More than {permitLimit} per {windowInSeconds} seconds){retryMessage}\"}}";

                context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                context.HttpContext.Response.WriteAsync(errorMessage);
                return new ValueTask();
            };
            _.AddFixedWindowLimiter("RequestsPerFrame", limiterOptions =>
            {
                limiterOptions.PermitLimit = permitLimit;
                limiterOptions.Window = TimeSpan.FromSeconds(windowInSeconds);
                limiterOptions.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                limiterOptions.QueueLimit = 0;
            });
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

        // Apply Rate Limiting
        app.UseRateLimiter();

        app.MapControllers().RequireRateLimiting("RequestsPerFrame");

        app.MapGet("/ping", () => Results.Text("Dogshouseservice.Version1.0.1")).RequireRateLimiting("RequestsPerFrame");

        app.Run();
    }
}
