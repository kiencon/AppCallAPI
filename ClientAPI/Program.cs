using System.Net.Http.Json;
using Contracts;

var baseUrl = "http://localhost:5209"; // adjust to whatever API prints
var http = new HttpClient { BaseAddress = new Uri(baseUrl) };

// send a correlation id to show propagation across layers
var correlationId = Guid.NewGuid().ToString("N");
http.DefaultRequestHeaders.Add("X-Correlation-Id", correlationId);

var userId = "u123";
var clientVersion = "1.0.0";
var submit = new SubmitLeaveRequestCommand(
    UserId: userId,
    StartDate: new DateOnly(2026, 2, 1),
    EndDate: new DateOnly(2026, 2, 5),
    Language: Lang.FR,
    ClientVersion: clientVersion
);

var submitResp = await http.PostAsJsonAsync("/leave-requests", submit);
submitResp.EnsureSuccessStatusCode();
var submitResult = await submitResp.Content.ReadFromJsonAsync<SubmitLeaveRequestResult>();

Console.WriteLine($"[API] Submitted requestId={submitResult!.RequestId} correlationId={submitResult.CorrelationId}");

var printBody = new PrintLeavePdfCommand(
    RequestId: submitResult.RequestId, // overwritten by route in API
    UserId: userId,
    ClientVersion: clientVersion
);

var printResp = await http.PostAsJsonAsync($"/leave-requests/{submitResult.RequestId}/print", printBody);
printResp.EnsureSuccessStatusCode();
var printResult = await printResp.Content.ReadFromJsonAsync<PrintLeavePdfResult>();

Console.WriteLine($"[API] Printed requestId={printResult!.RequestId} requestLang={printResult.RequestLanguage} pdfLang={printResult.PdfLanguage}");
Console.WriteLine($"[API] pdfPath={printResult.PdfPath} correlationId={printResult.CorrelationId}");
