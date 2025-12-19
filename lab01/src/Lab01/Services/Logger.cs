using System.Text;

namespace Lab01;
public static class Logger
{
    private static readonly object _lock = new();
    private static readonly string _logFilePath = Path.Combine(Directory.GetCurrentDirectory(), "errors.log");

    public static void LogError(string message, Exception? ex = null)
    {
        try
        {
            var sb = new StringBuilder();
            sb.AppendLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] ERROR: {message}");

            if (ex != null)
            {
                sb.AppendLine(ex.ToString());
            }

            lock (_lock)
            {
                File.AppendAllText(_logFilePath, sb.ToString(), Encoding.UTF8);
            }
        }
        catch
        {
        }
    }
}


