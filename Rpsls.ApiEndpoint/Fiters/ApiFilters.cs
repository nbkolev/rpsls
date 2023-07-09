using System.Diagnostics.CodeAnalysis;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Rpsls.ApiEndpoint;

[ExcludeFromCodeCoverage]
public class ApiExceptionFilter : IExceptionFilter
{
    private readonly IHostEnvironment _hostEnvironment;

    public ApiExceptionFilter(IHostEnvironment hostEnvironment) =>
        _hostEnvironment = hostEnvironment;

    /// <summary>
    /// Provide only relevant exception details in production environment.
    /// </summary>
    /// 
    public void OnException(ExceptionContext context)
    {
        if (_hostEnvironment.IsDevelopment())
        {
            context.Result = new ContentResult
            {
                Content = context.Exception.StackTrace
            };
          
        }
        else
        {
            context.Result = new ContentResult
            {
                Content = context.Exception.Message
            };
        }
    }
}
