using Secretes_Scanner.GitLeaks;
using SecretsScanner.Gitleaks;

// ✅ This is the PROJECT folder (SecretsScanner.Runner)
var projectRoot = Directory.GetCurrentDirectory();

// Folder to scan
var sourceFolder = @"C:\Tools\Mongo";

// Store report INSIDE the Runner project
var reportPath = Path.Combine(
    projectRoot,
    "scan-results",
    "gitleaks-report.json");

Console.WriteLine($"Project root resolved to: {projectRoot}");
Console.WriteLine($"Report will be stored at: {reportPath}");

var scanner = new GitleaksScanner();

var options = new GitleaksOptions
{
    GitleaksExePath = @"C:\Tools\gitleaks_8.30.0_windows_x64\gitleaks.exe",
    SourceFolder = sourceFolder,
    ReportFilePath = reportPath,
    Redact = true
};

await scanner.ScanAsync(options);

