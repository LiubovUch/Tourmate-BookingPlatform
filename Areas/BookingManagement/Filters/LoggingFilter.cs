using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using System;

namespace Assignment1.Areas.BookingManagement.Filters
{
    public class LoggingFilter : IActionFilter
    {
        private readonly ILogger<LoggingFilter> _logger;

        public LoggingFilter(ILogger<LoggingFilter> logger)
        {
            _logger = logger;
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            // Log the details of the user's activity before the action executes
            var controllerName = context.RouteData.Values["controller"];
            var actionName = context.RouteData.Values["action"];
            var ipAddress = context.HttpContext.Connection.RemoteIpAddress;

            _logger.LogInformation($"User with IP address {ipAddress} is accessing {controllerName}/{actionName} at {DateTime.Now}.");
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            // Log the details of the user's activity after the action executes
            var controllerName = context.RouteData.Values["controller"];
            var actionName = context.RouteData.Values["action"];
            var ipAddress = context.HttpContext.Connection.RemoteIpAddress;

            _logger.LogInformation($"User with IP address {ipAddress} finished accessing {controllerName}/{actionName} at {DateTime.Now}.");
        }
    }
}
