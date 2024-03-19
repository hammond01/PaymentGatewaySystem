using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using PaymentGateway.Domain.Repositories;
using PaymentGateway.Domain.Request;
using Serilog;
using System.Net.Sockets;

namespace PaymentGateway.Ultils.Loggers;

public class LoggingActionFilter : IActionFilter
{
    private readonly IHttpContextAccessor _httpcontextAccessor;

    public LoggingActionFilter(ILogger<LoggingActionFilter> logger, IHttpContextAccessor httpContextAccessor,
        IAuditServices auditServices)
    {
        _httpcontextAccessor = httpContextAccessor;
    }

    public void OnActionExecuting(ActionExecutingContext context)
    {
        var auditModel = new AuditRequest();
        var controllerName = ((ControllerBase)context.Controller).ControllerContext.ActionDescriptor.ControllerName;
        var actionName = ((ControllerBase)context.Controller).ControllerContext.ActionDescriptor.RouteValues["action"];
        var parameters = context.ActionArguments;
        var data = parameters.Values.FirstOrDefault();
        if (data != null) auditModel.PaymentStatus = data.ToString()!;
        var remoteIpAddress = _httpcontextAccessor.HttpContext!.Connection.RemoteIpAddress;
        if (remoteIpAddress != null)
        {
            if (remoteIpAddress.AddressFamily == AddressFamily.InterNetworkV6)
                remoteIpAddress = remoteIpAddress.MapToIPv4();
            auditModel.ActionIp = remoteIpAddress.ToString();
        }

        auditModel.Action = actionName!;
        auditModel.UserId = context.HttpContext.User.Identity!.Name!;
        auditModel.ControllerName = controllerName;
        //_auditServices.InsertAuditLogs(auditModel);
        Log.Information($"Start service {controllerName}: {actionName}");
    }

    public void OnActionExecuted(ActionExecutedContext context)
    {
        var controller = context.Controller.GetType().Name;
        var action = context.ActionDescriptor.RouteValues["action"];
        Log.Information($"End service {controller}: {action}");
    }
}