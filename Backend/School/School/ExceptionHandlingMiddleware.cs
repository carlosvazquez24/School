using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Text.Json;
using ApplicationServices.Exceptions; // Asegúrate de que el namespace coincida con tus excepciones
using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch.Exceptions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace YourNamespace.Middleware
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;
        private readonly IHostEnvironment _env;

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger, IHostEnvironment env)
        {
            _next = next;
            _logger = logger;
            _env = env;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                // Continuar con la ejecución del siguiente middleware
                await _next(context);
            }
            catch (Exception ex)
            {
                // Manejo de la excepción capturada
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            HttpStatusCode statusCode;
            string message;
            string? stackTrace = null;

            // Mapeo de excepciones personalizadas y generales a códigos de estado HTTP
            switch (exception)
            {
                case BadRequestException:
                case ValidationException:  // Agregamos Validación
                case JsonPatchException:   // Agregamos errores de Patch
                case DbUpdateException:    // Agregamos errores de BD
                    statusCode = HttpStatusCode.BadRequest;
                    break;
                case NotFoundException:
                    statusCode = HttpStatusCode.NotFound;
                    break;
                case AutoMapperMappingException: // Errores de mapeo
                    statusCode = HttpStatusCode.InternalServerError;
                    break;
                default:
                    statusCode = HttpStatusCode.InternalServerError;
                    break;
            }

            _logger.LogError(exception, exception.Message);

            if (_env.IsDevelopment())
            {
                // Incluye la inner exception si está presente
                message = exception.InnerException != null
                    ? $"{exception.Message} - Inner: {exception.InnerException.Message}"
                    : exception.Message;
                stackTrace = exception.StackTrace;
            }
            else
            {
                message = "Ocurrió un error interno en el servidor.";
            }

            var errorResponse = new { error = message, stackTrace };
            var errorJson = JsonSerializer.Serialize(errorResponse);

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)statusCode;
            await context.Response.WriteAsync(errorJson);
        }

    }

    // Extensión para facilitar la incorporación del middleware en el pipeline
    public static class ExceptionHandlingMiddlewareExtensions
    {
        public static IApplicationBuilder UseExceptionHandlingMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ExceptionHandlingMiddleware>();
        }
    }
}
