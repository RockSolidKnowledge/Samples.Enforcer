using Rsk.Enforcer.Services.Logging;

namespace JustPDP;

public class ConsoleEnforcerLogger : IEnforcerLogger, IDisposable
{
    public void LogDiagnostic(string message)
    {
        Console.WriteLine(message);
    }

    public void LogInformation(string message)
    {
        Console.WriteLine(message);
    }

    public void LogWarning(string message)
    {
        Console.WriteLine(message);
    }

    public void LogError(string message)
    {
        Console.WriteLine(message);
    }

    public void LogError(string message, Exception error)
    {
        Console.WriteLine(message);
    }

    public void LogCritical(string message)
    {
        Console.WriteLine(message);
    }

    public void LogCritical(string message, Exception error)
    {
        Console.WriteLine($"{message} : {error}");
    }

    public IDisposable BeginScope(object context)
    {
        return this;
    }

    public void Dispose()
    {
        return;
    }
}