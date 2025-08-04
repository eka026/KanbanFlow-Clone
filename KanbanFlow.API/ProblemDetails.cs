using Microsoft.AspNetCore.Diagnostics;
using System.Net;
using System.Text.Json;

namespace KanbanFlow.API
{
    public static class ProblemDetailsExtensions
    {
        public static void UseProblemDetailsExceptionHandler(this IApplicationBuilder app)
        {
            app.UseExceptionHandler(builder =>
            {
                builder.Run(async context =>
                {
                    var exceptionHandlerPathFeature = context.Features.Get<IExceptionHandlerPathFeature>();
                    var exception = exceptionHandlerPathFeature?.Error;

                    var problemDetails = new Microsoft.AspNetCore.Mvc.ProblemDetails
                    {
                        Instance = context.Request.Path,
                        Status = (int)HttpStatusCode.InternalServerError,
                        Detail = exception?.Message
                    };

                    problemDetails.Extensions["traceId"] = System.Diagnostics.Activity.Current?.Id ?? context.TraceIdentifier;
                    if (exception != null)
                    {
                        problemDetails.Extensions["errors"] = new { messages = new[] { exception.Message } };
                    }

                    context.Response.ContentType = "application/problem+json";
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

                    var json = JsonSerializer.Serialize(problemDetails);
                    await context.Response.WriteAsync(json);
                });
            });
        }
    }
}