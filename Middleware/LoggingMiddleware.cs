using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

public class LoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<LoggingMiddleware> _logger;

    public LoggingMiddleware(RequestDelegate next, ILogger<LoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        await LogRequestAsync(context.Request);

        await _next(context);
    }

    private async Task LogRequestAsync(HttpRequest request)
    {
        request.EnableBuffering();
        var requestBodyStream = new MemoryStream();
        await request.Body.CopyToAsync(requestBodyStream);
        requestBodyStream.Seek(0, SeekOrigin.Begin);
        var requestBodyText = await new StreamReader(requestBodyStream).ReadToEndAsync();

        _logger.LogInformation($"Request: {request.Method} {request.Path} \n Body: {requestBodyText}");
        request.Body.Seek(0, SeekOrigin.Begin);
    }
}
