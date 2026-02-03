// using Microsoft.Extensions.Logging;

// namespace Observability;
// public interface ICorrelationContext
// {
//     string CorrelationId { get; }
//     void Set(string correlationId);
// }

// public sealed class CorrelationContext : ICorrelationContext
// {
//     public string CorrelationId { get; private set; } = "n/a";
//     public void Set(string correlationId) => CorrelationId = correlationId;
// }

// public static class LogScopes
// {
//     public static IDisposable? BeginCorrelationScope(this ILogger logger, string correlationId)
//     {
//         return logger.BeginScope(
//             new Dictionary<string, object> { ["correlationId"] = correlationId }
//         );
//     }
// }
