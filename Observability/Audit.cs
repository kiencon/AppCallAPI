using System;
using Microsoft.Extensions.Logging;


namespace Observability;

public interface IAuditLogger
{
    void Write(string eventName, object data);
}

public sealed class ConsoleAuditLogger : IAuditLogger
{
    private readonly ILogger<ConsoleAuditLogger> _logger;
    private readonly ICorrelationContext _corr;

    public ConsoleAuditLogger(ILogger<ConsoleAuditLogger> logger, ICorrelationContext corr)
    {
        _logger = logger;
        _corr = corr;
    }

    public void Write(string eventName, object data)
    {
        // Keep it simple for POC: log as structured object
        _logger.LogInformation("AUDIT {eventName} correlationId={correlationId} data={@data}",
            eventName, _corr.CorrelationId, data);
    }
}
