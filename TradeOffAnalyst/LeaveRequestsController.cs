using Contracts;
using Microsoft.AspNetCore.Mvc;
//using Observability;
using Services;

namespace TradeOffAnalyst;

[ApiController]
[Route("leave-requests")]
public sealed class LeaveRequestsController : ControllerBase
{
    private readonly ILogger<LeaveRequestsController> _logger;
    //private readonly ICorrelationContext _corr;
    private readonly ILeaveService _svc;

    public LeaveRequestsController(
        ILogger<LeaveRequestsController> logger,
        //ICorrelationContext corr,
        ILeaveService svc)
    {
        _logger = logger;
        //_corr = corr;
        _svc = svc;
    }

    [HttpPost]
    public async Task<ActionResult<SubmitLeaveRequestResult>> Submit([FromBody] SubmitLeaveRequestCommand cmd, CancellationToken ct)
    {
        //_logger.LogInformation("HTTP Submit correlationId={correlationId}", _corr.CorrelationId);
        return Ok(await _svc.SubmitAsync(cmd, ct));
    }

    [HttpPost("{id:guid}/print")]
    public async Task<ActionResult<PrintLeavePdfResult>> Print(Guid id, [FromBody] PrintLeavePdfCommand body, CancellationToken ct)
    {
        // Ensure route id is used
        var cmd = body with { RequestId = id };

        //_logger.LogInformation("HTTP Print correlationId={correlationId}", _corr.CorrelationId);
        return Ok(await _svc.PrintPdfAsync(cmd, ct));
    }
}
