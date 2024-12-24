using Microsoft.AspNetCore.Http;
using OmsTradeConsumer.Domain.Exceptions;
using OmsTradeConsumer.Domain.Exceptions.ExceptionTypes;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Json;

namespace OmsTradeConsumer.Configuration.Middleware;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;

    public ExceptionMiddleware(RequestDelegate next) => _next = next;
    public async Task InvokeAsync(HttpContext httpContext)
    {
        if (httpContext == null)
            throw new ArgumentNullException(nameof(httpContext), "Parâmetro não configurado");

        string httpRequestBody = await GetBody(httpContext.Request);
        try
        {
            await _next(httpContext);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(httpContext, ex, httpRequestBody);
        }
    }
    private async Task HandleExceptionAsync(HttpContext context, Exception ex, string httpRequestBody)
    {
        int statusCode = StatusCodes.Status500InternalServerError;
        object response;

        if (ex is NotFoundException)
        {
            response = new ErrorViewModel("NotFound", ex.Message);
            statusCode = StatusCodes.Status404NotFound;
        }
        else if (ex is ValidationException)
        {
            response = new ErrorViewModel("BadRequest", ex.Message);
            statusCode = StatusCodes.Status400BadRequest;
        }
        else if (ex is UnprocessableEntityException)
        {
            response = new ErrorViewModel("UnprocessableEntity", ex.Message);
            statusCode = StatusCodes.Status422UnprocessableEntity;
        }
        else if (ex is NoContentException)
        {
            response = new ErrorViewModel("NoContent", ex.Message);
            statusCode = StatusCodes.Status204NoContent;
        }
        else
        {
            response = new ErrorViewModel("InternalServerError", "Ocorreu um erro interno ao processar a requisição.");
        }

        context.Response.StatusCode = statusCode; 
        context.Response.ContentType = "application/json"; 
        
        string result = JsonSerializer.Serialize(response); 
        await context.Response.WriteAsync(result, Encoding.UTF8);
    }
    private async Task<string> GetBody(HttpRequest request)
    {
        request.EnableBuffering();

        Stream body = request.Body;

        byte[] buffer = new byte[Convert.ToInt32(request.ContentLength)];

        await request.Body.ReadAsync(buffer, 0, buffer.Length);

        string requestBody = Encoding.UTF8.GetString(buffer);

        body.Seek(0, SeekOrigin.Begin);

        return requestBody;
    }
}
