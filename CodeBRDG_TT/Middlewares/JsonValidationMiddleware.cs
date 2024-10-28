using System.Text;
using System.Text.Json;

namespace CodeBRDG_TT.Middlewares;
public class JsonValidationMiddleware
{
    private readonly RequestDelegate _next;

    public JsonValidationMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (context.Request.ContentType?.Contains("application/json") == true)
        {
            context.Request.EnableBuffering(); 

            using (var reader = new StreamReader(context.Request.Body, Encoding.UTF8, leaveOpen: true))
            {
                var body = await reader.ReadToEndAsync();
                context.Request.Body.Position = 0; 

                try
                {
                    JsonDocument.Parse(body);
                }
                catch (JsonException jsonEx)
                {
                    context.Response.StatusCode = StatusCodes.Status400BadRequest;
                    var errorResponse = new
                    {
                        error = "Invalid JSON format.",
                        message = jsonEx.Message,
                        lineNumber = jsonEx.LineNumber,
                        bytePositionInLine = jsonEx.BytePositionInLine
                    };

                    context.Response.ContentType = "application/json";
                    await context.Response.WriteAsync(JsonSerializer.Serialize(errorResponse));
                    return;
                }
            }
        }

        await _next(context);
    }
}
