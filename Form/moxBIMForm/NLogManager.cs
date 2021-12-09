using System;

public class NLogManager : ILogManager
{
    private static NLog.ILogger logger = LogManager.GetCurrentClassLogger();
    public void LogDebug(string message)
    {
        logger.Debug(message);
    }
    public void LogError(string message)
    {
        Logger logger = LogManager.GetLogger("EventLogTarget");
        var logEventInfo = new LogEventInfo(LogLevel.Error,
        logger.Name, message);
        logger.Log(logEventInfo);
    }
    public void LogInformation(string message)
    {
        logger.Info(message);
    }
    public void LogWarning(string message)
    {
        logger.Warn(message);
    }
}

public void LogError(string message)
{
    Logger logger = LogManager.GetLogger("EventLogTarget");
    var logEventInfo = new LogEventInfo(LogLevel.Error,
    logger.Name, message);
    logger.Log(logEventInfo);
}
