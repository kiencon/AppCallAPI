using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Logging.Console;

public sealed class CorrelationOnlyConsoleFormatter : ConsoleFormatter
{
    public CorrelationOnlyConsoleFormatter() : base("corr") { }

    public override void Write<TState>(
        in LogEntry<TState> logEntry,
        IExternalScopeProvider? scopeProvider,
        TextWriter textWriter)
    {
        var timestamp = DateTimeOffset.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
        string correlationId = "-";

        scopeProvider?.ForEachScope<object>((scope, _) =>
        {
            if (scope is IEnumerable<KeyValuePair<string, object>> kvps)
            {
                foreach (var kv in kvps)
                {
                    if (kv.Key == "CorrelationId")
                        correlationId = kv.Value?.ToString() ?? "-";
                }
            }
        }, state: null);

        textWriter.Write($"{timestamp} correlationId {correlationId} :");
        
        textWriter.WriteLine(logEntry.Formatter(logEntry.State, logEntry.Exception));

        if (logEntry.Exception is not null)
            textWriter.WriteLine(logEntry.Exception);
    }
}
