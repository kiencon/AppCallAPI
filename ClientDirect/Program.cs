using Contracts;
using Infrastructure;

// Direct mode: client reaches repository + pdf generator directly

var repo = new InMemoryLeaveRepository();
var pdf = new FakePdfGenerator();

var userId = "u123";
var lang = Lang.FR;

var req = new LeaveRequest
{
    Id = Guid.NewGuid(),
    UserId = userId,
    StartDate = new DateOnly(2026, 2, 1),
    EndDate = new DateOnly(2026, 2, 5),
    Language = lang
};

await repo.CreateAsync(req, CancellationToken.None);

Console.WriteLine($"[DIRECT] Created leave request {req.Id} language={req.Language}");

// BUG: PDF generator uses default/system language (EN) — mismatch
var pdfLang = Lang.EN;
var pdfPath = await pdf.GenerateLeavePdfAsync(req.Id, pdfLang, CancellationToken.None);

Console.WriteLine($"[DIRECT] Printed PDF for {req.Id} requestLang={req.Language} pdfLang={pdfLang} path={pdfPath}");
Console.WriteLine("[DIRECT] Where did pdfLang=EN come from? Depends on each client / config / culture.");

