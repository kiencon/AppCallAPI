using Contracts;

namespace Infrastructure;

public interface IPdfGenerator
{
    Task<string> GenerateLeavePdfAsync(Guid requestId, Lang pdfLanguage, CancellationToken ct);
}

public sealed class FakePdfGenerator : IPdfGenerator
{
    public Task<string> GenerateLeavePdfAsync(Guid requestId, Lang pdfLanguage, CancellationToken ct)
    {
        // For POC: write a text file and call it “pdf”
        var dir = Path.Combine(Path.GetTempPath(), "LeaveDemoPdfs");
        Directory.CreateDirectory(dir);

        var path = Path.Combine(dir, $"{requestId}-{pdfLanguage}.pdf.txt");
        File.WriteAllText(path, $"Leave Request {requestId}\nPDF Language: {pdfLanguage}\n");

        return Task.FromResult(path);
    }
}