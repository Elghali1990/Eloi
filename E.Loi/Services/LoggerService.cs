namespace E.Loi.Services;

public class LoggerService<T>(ILogger<T> _logger) where T : class
{
    public void HandlLogger(string ActionName, Exception ex) => _logger.LogError(ex.Message, $"Error on {nameof(ActionName)}", nameof(T));
}
