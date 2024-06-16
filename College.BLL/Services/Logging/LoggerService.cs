using Serilog;

namespace College.BLL.Interfaces.Logging;

public class LoggerService : ILoggerService
{
    private readonly ILogger _logger;

    public LoggerService(ILogger logger)
    {
        _logger = logger;
    }

    public void LogInformation(string msg)
    {
        _logger.Information($"{msg}");
    }

    public void LogWarning(string msg)
    {
        _logger.Warning($"{msg}");
    }

    public void LogTrace(string msg)
    {
        _logger.Information($"{msg}");
    }

    public void LogDebug(string msg)
    {
        _logger.Debug($"{msg}");
    }

    public void LogError(object request, string erroMsg)
    {
        string requestType = request.GetType().ToString();
        string requestClass = requestType[(requestType.LastIndexOf('.') + 1)..];
        _logger.Error($"{requestClass} handled with the error: {erroMsg}");
    }

    public void LogGlobalError(string erroMsg)
    {
        _logger.Error(erroMsg);
    }
}
