using System;
using Unity;

class Program
{
    static void Main()
    {
        // Create Unity container
        IUnityContainer container = new UnityContainer();

        // Register types
        container.RegisterType<ILogger, ConsoleLogger>();
        container.RegisterType<IMyLogger, MyLogger>();

        // Resolve dependency
        var service = container.Resolve<Service>();
        service.PerformAction();
        
        var myService = container.Resolve<MyService>();
        myService.PerformMyAction();

        Console.ReadKey();
    }
}

// Interfaces and classes
public interface ILogger
{
    void Log(string message);
}
public interface IMyLogger
{
    void Trace(string message);
    void Debug(string message);
    void Info(string message);
    void Warn(string message);
    void Error(string message);
    void Fatal(string message);
}

public class ConsoleLogger : ILogger
{
    public void Log(string message)
    {
        Console.WriteLine($"Logging: {message}");
    }
}

public class MyLogger : IMyLogger
{
    public void Debug(string message)
    {
        var curForeColor = Console.ForegroundColor;
        Console.ForegroundColor = ConsoleColor.Blue;
        Console.WriteLine($"DEBUG: {message}");
        Console.ForegroundColor = curForeColor;
    }

    public void Error(string message)
    {
        var curForeColor = Console.ForegroundColor;
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"ERROR: {message}");
        Console.ForegroundColor = curForeColor;
    }

    public void Fatal(string message)
    {
        var curForeColor = Console.ForegroundColor;
        Console.ForegroundColor = ConsoleColor.Red;
        var curBackColor = Console.BackgroundColor;
        Console.ForegroundColor = ConsoleColor.White;
        Console.BackgroundColor = ConsoleColor.Red;
        Console.WriteLine($"FATAL: {message}");
        Console.ForegroundColor = curForeColor;
        Console.BackgroundColor = curBackColor;
    }

    public void Info(string message)
    {
        var curForeColor = Console.ForegroundColor;
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"INFO: {message}");
        Console.ForegroundColor = curForeColor;
    }

    public void Trace(string message)
    {
        var curForeColor = Console.ForegroundColor;
        Console.ForegroundColor = ConsoleColor.Gray;
        Console.WriteLine($"TRACE: {message}");
        Console.ForegroundColor = curForeColor;
    }

    public void Warn(string message)
    {
        var curForeColor = Console.ForegroundColor;
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine($"WARN: {message}");
        Console.ForegroundColor = curForeColor;
    }
}

public class Service
{
    private readonly ILogger logger;

    // Constructor injection
    public Service(ILogger logger)
    {
        this.logger = logger;
    }

    public void PerformAction()
    {
        logger.Log("Action performed!");
    }
}


public class MyService
{
    private readonly IMyLogger _mylogger;

    // Constructor injection
    public MyService(IMyLogger mylogger)
    {
        _mylogger = mylogger;
    }

    public void PerformMyAction()
    {
        _mylogger.Trace("My TRACE action performed!");
        _mylogger.Debug("My DEBUG action performed!");
        _mylogger.Info("My INFO action performed!");
        _mylogger.Warn("My WARN action performed!");
        _mylogger.Error("My ERROR action performed!");
        _mylogger.Fatal("My FATAL action performed!");
    }
}