namespace SecretsScanner.Abstractions;

public class ScanResult
{
    public int FindingsCount { get; set; }
    public string ReportPath { get; set; } = default!;
    public bool HasFindings => FindingsCount > 0;
}
