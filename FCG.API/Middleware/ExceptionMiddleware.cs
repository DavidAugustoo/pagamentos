using FCG.API.Errors;
using System.Net;
using System.Text.Json;

namespace FCG.API.Middleware
{
    public class ExceptionMiddleware
    {
        public readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;
        private readonly IHostEnvironment _env;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger, IHostEnvironment env)
        {
            _next = next;
            _logger = logger;
            _env = env;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            _logger.LogInformation("Iniciando requisição: {method} {url}", context.Request.Method, context.Request.Path);

            try
            {
                await _next(context);

                if (context.Response.StatusCode == (int)HttpStatusCode.Unauthorized)
                {
                    _logger.LogWarning("Requisição não autorizada (401): {method} {url}", context.Request.Method, context.Request.Path);
                }

                _logger.LogInformation("Requisição concluída com sucesso: {method} {url} - StatusCode: {statusCode}", 
                    context.Request.Method, context.Request.Path, context.Response.StatusCode);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Argumento inválido: {message}", ex.Message);
                await HandleExceptionAsync(context, ex, HttpStatusCode.BadRequest);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro interno: {message}", ex.Message);
                await HandleExceptionAsync(context, ex, HttpStatusCode.InternalServerError);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception ex, HttpStatusCode statusCode)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)statusCode;

            var response = _env.IsDevelopment() ?
                new ApiException(context.Response.StatusCode.ToString(), ex.Message, ex.StackTrace?.ToString()) :
                new ApiException(context.Response.StatusCode.ToString(), ex.Message, "Internal server error");

            var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
            var json = JsonSerializer.Serialize(response, options);
            await context.Response.WriteAsync(json);
        }
    }
}
