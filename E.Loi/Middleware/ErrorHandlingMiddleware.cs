using System.Net;

namespace E.Loi.Middleware;

public class ErrorHandlingMiddleware : IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            Console.WriteLine("+++++++++++" + context.Request.Method + "+++++++++++");
            await next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
        var result = System.Text.Json.JsonSerializer.Serialize(new { error = exception.Message });
        Console.WriteLine("------------" + context.Response.WriteAsync(result) + "---------------------");
        return context.Response.WriteAsync(result);
    }
}
