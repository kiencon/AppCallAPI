using Contracts;
using Infrastructure;
using Microsoft.Extensions.Logging;
//using Observability;

namespace Services;

public interface ILeaveService
{
    Task<SubmitLeaveRequestResult> SubmitAsync(SubmitLeaveRequestCommand cmd, CancellationToken ct);
    Task<PrintLeavePdfResult> PrintPdfAsync(PrintLeavePdfCommand cmd, CancellationToken ct);
}

/// <summary>
/// This is the core of your POC: centralized workflow + centralized tracing + audit.
/// </summary>
public sealed class LeaveService : ILeaveService
{
    private readonly ILogger<LeaveService> _logger;
    //private readonly ICorrelationContext _corr;
    //private readonly IAuditLogger _audit;
    private readonly ILeaveRepository _repo;
    private readonly IPdfGenerator _pdf;

    /// <summary>
    /// Toggle this to demonstrate the bug.
    /// - true: BUG: PDF language derived from default/system language instead of request language.
    /// - false: FIX: PDF language must match leave request language.
    /// </summary>
    public static bool Bug_UseSystemDefaultPdfLanguage = true;

    public LeaveService(
        ILogger<LeaveService> logger,
        //ICorrelationContext corr,
        //IAuditLogger audit,
        ILeaveRepository repo,
        IPdfGenerator pdf)
    {
        _logger = logger;
        // _corr = corr;
        // _audit = audit;
        _repo = repo;
        _pdf = pdf;
    }

    public async Task<SubmitLeaveRequestResult> SubmitAsync(SubmitLeaveRequestCommand cmd, CancellationToken ct)
    {
        //using var _ = _logger.BeginCorrelationScope(_corr.CorrelationId);

        _logger.LogInformation("SubmitLeaveRequest start userId={userId} lang={lang} clientVersion={clientVersion}",
            cmd.UserId, cmd.Language, cmd.ClientVersion);

        var req = new LeaveRequest
        {
            Id = Guid.NewGuid(),
            UserId = cmd.UserId,
            StartDate = cmd.StartDate,
            EndDate = cmd.EndDate,
            Language = cmd.Language
        };

        await _repo.CreateAsync(req, ct);

        // _audit.Write("LeaveRequestSubmitted", new
        // {
        //     req.Id,
        //     req.UserId,
        //     req.Language,
        //     cmd.ClientVersion
        // });

        _logger.LogInformation("SubmitLeaveRequest done requestId={requestId}", req.Id);

        return new SubmitLeaveRequestResult(req.Id);
    }

    public async Task<PrintLeavePdfResult> PrintPdfAsync(PrintLeavePdfCommand cmd, CancellationToken ct)
    {
        //using var _ = _logger.BeginCorrelationScope(_corr.CorrelationId);

        _logger.LogInformation("PrintLeavePdf start requestId={requestId} userId={userId} clientVersion={clientVersion}",
            cmd.RequestId, cmd.UserId, cmd.ClientVersion);

        var req = await _repo.GetAsync(cmd.RequestId, ct);
        if (req is null)
        {
            //_audit.Write("LeavePdfPrintFailed_NotFound", new { cmd.RequestId, cmd.UserId, cmd.ClientVersion });
            throw new InvalidOperationException($"Leave request not found: {cmd.RequestId}");
        }

        // --- This is the “language inconsistency” bug you want to demonstrate ---
        var pdfLang = Bug_UseSystemDefaultPdfLanguage
            ? Lang.EN // imagine: Environment/user profile default, thread culture, etc.
            : req.Language;

        _logger.LogInformation("ResolveLanguage requestLang={requestLang} resolvedPdfLang={pdfLang} bugMode={bugMode}",
            req.Language, pdfLang, Bug_UseSystemDefaultPdfLanguage);

        var pdfPath = await _pdf.GenerateLeavePdfAsync(req.Id, pdfLang, ct);

        // _audit.Write("LeavePdfPrinted", new
        // {
        //     req.Id,
        //     req.UserId,
        //     RequestLanguage = req.Language,
        //     PdfLanguage = pdfLang,
        //     pdfPath,
        //     cmd.ClientVersion
        // });

        _logger.LogInformation("PrintLeavePdf done pdfPath={pdfPath}", pdfPath);

        return new PrintLeavePdfResult(req.Id, req.Language, pdfLang, pdfPath);
    }
}
