using SecretsScanner.Abstractions;

namespace Secretes_Scanner.GitLeaks;

public class GitleaksOptions : ScanOptions
{
    public string GitleaksExePath { get; set; } = default!;

    // FIXED report file
    public string ReportFilePath { get; set; } = default!;

    public bool Redact { get; set; } = true;
}
