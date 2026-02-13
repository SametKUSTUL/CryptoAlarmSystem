using System.Diagnostics;
using System.Text;

namespace CryptoAlarmSystem.Api.Middlewares;

public class RequestResponseLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestResponseLoggingMiddleware> _logger;

    public RequestResponseLoggingMiddleware(RequestDelegate next, ILogger<RequestResponseLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var traceId = Activity.Current?.TraceId.ToString() ?? context.TraceIdentifier;
        var userId = context.Request.Headers["X-User-Id"].FirstOrDefault() ?? "anonymous";
        
        context.Request.EnableBuffering();
        var requestBody = await ReadRequestBodyAsync(context.Request);
        
        _logger.LogInformation(
            "HTTP Request: {Method} {Path} | UserId: {UserId} | TraceId: {TraceId} | Body: {Body}",
            context.Request.Method,
            context.Request.Path,
            userId,
            traceId,
            requestBody);

        var originalBodyStream = context.Response.Body;
        using var responseBody = new MemoryStream();
        context.Response.Body = responseBody;

        var sw = Stopwatch.StartNew();
        await _next(context);
        sw.Stop();

        var responseBodyText = await ReadResponseBodyAsync(context.Response);
        
        _logger.LogInformation(
            "HTTP Response: {Method} {Path} | StatusCode: {StatusCode} | Duration: {Duration}ms | UserId: {UserId} | TraceId: {TraceId} | Body: {Body}",
            context.Request.Method,
            context.Request.Path,
            context.Response.StatusCode,
            sw.ElapsedMilliseconds,
            userId,
            traceId,
            responseBodyText);

        await responseBody.CopyToAsync(originalBodyStream);
    }

    private static async Task<string> ReadRequestBodyAsync(HttpRequest request)
    {
        request.Body.Position = 0;
        using var reader = new StreamReader(request.Body, Encoding.UTF8, leaveOpen: true);
        var body = await reader.ReadToEndAsync();
        request.Body.Position = 0;
        return string.IsNullOrWhiteSpace(body) ? "-" : body;
    }

    private static async Task<string> ReadResponseBodyAsync(HttpResponse response)
    {
        response.Body.Position = 0;
        using var reader = new StreamReader(response.Body, Encoding.UTF8, leaveOpen: true);
        var body = await reader.ReadToEndAsync();
        response.Body.Position = 0;
        return string.IsNullOrWhiteSpace(body) ? "-" : body;
    }
}
