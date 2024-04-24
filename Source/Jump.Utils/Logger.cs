using Console = System.Console;
using Godot;
using System;
using System.Drawing.Text;

public class Logger
{
    public static Action<string, LogType> OnLog;
    public Logger(string className)
    {
        _className = className;
    }
    public void Info(object what)
    {
        LogType logType = LogType.Info;
        string message = $"({DateTime.Now.ToString(Constants.LOG_TIME_FORMAT)}) [{logType.ToString().ToUpper()}/{_className}] {what}";
        LogGd(message);
        LogConsole(message);
        OnLog?.Invoke(message, logType);
    }

    public void Warn(object what)
    {
        LogType logType = LogType.Warn;
        string message = $"({DateTime.Now.ToString(Constants.LOG_TIME_FORMAT)}) [{logType.ToString().ToUpper()}/{_className}] {what}";
        LogGd(message);
        LogConsole(message);
        OnLog?.Invoke(message, logType);
    }

    public void Error(object what)
    {
        LogType logType = LogType.Error;
        string message = $"({DateTime.Now.ToString(Constants.LOG_TIME_FORMAT)}) [{logType.ToString().ToUpper()}/{_className}] {what}";
        LogGd(message);
        LogConsole(message);
        OnLog?.Invoke(message, logType);
    }

    private void LogGd(string what)
    {
        GD.Print(what);
    }

    private void LogConsole(string what)
    {
        Console.WriteLine(what);
    }

    private string _className;
}

public enum LogType
{
    Info,
    Warn,
    Error
}