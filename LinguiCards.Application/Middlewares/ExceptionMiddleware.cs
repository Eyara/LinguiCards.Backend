using System.Net;
using System.Text.Json;
using LinguiCards.Application.Common.Exceptions.Base;
using Microsoft.AspNetCore.Http;

namespace LinguiCards.Application.Middlewares;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;

    public ExceptionMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext httpContext)
    {
        try
        {
            await _next(httpContext);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(httpContext, ex);
        }
    }

    private Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        var statusCode = (int)HttpStatusCode.InternalServerError;
        var response = new { message = "An unexpected error occurred." };
        switch (exception)
        {
            case InvalidJwtTokenException jwtEx:
                statusCode = (int)HttpStatusCode.Unauthorized;
                response = new { message = jwtEx.Message };
                break;
            case NotFoundException notFoundEx:
                statusCode = (int)HttpStatusCode.NotFound;
                response = new { message = $"{notFoundEx.Message}" };
                break;
            case EntityOwnershipException ownershipEx:
                statusCode = (int)HttpStatusCode.Forbidden;
                response = new { message = ownershipEx.Message };
                break;
        }

        context.Response.StatusCode = statusCode;
        return context.Response.WriteAsync(JsonSerializer.Serialize(response));
    }
}
