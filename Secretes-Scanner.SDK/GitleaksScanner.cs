using Secretes_Scanner.GitLeaks;
using SecretsScanner.Abstractions;
using System;
using System.Diagnostics;
using System.IO;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace SecretsScanner.Gitleaks;

public class GitleaksScanner : ISecretsScanner
{
    public async Task<ScanResult> ScanAsync(
        ScanOptions options,
        CancellationToken cancellationToken = default)
    {
        var gitleaksOptions = options as GitleaksOptions
            ?? throw new ArgumentException("Invalid options type");

        Validate(gitleaksOptions);

        var arguments =
            $"detect --source \"{gitleaksOptions.SourceFolder}\" " +
            $"--report-path \"{gitleaksOptions.ReportFilePath}\" " +
            $"--report-format json";

        if (gitleaksOptions.Redact)
            arguments += " --redact";

        using var process = CreateProcess(
            gitleaksOptions.GitleaksExePath,
            arguments);

        process.Start();

        var stdOutTask = process.StandardOutput.ReadToEndAsync();
        var stdErrTask = process.StandardError.ReadToEndAsync();

        await process.WaitForExitAsync(cancellationToken);

        var stdout = await stdOutTask;
        var stderr = await stdErrTask;

        // IMPORTANT:
        // ExitCode 0 = no findings
        // ExitCode 1 = findings found
        // ExitCode >1 = real error
        if (process.ExitCode > 1)
            throw new Exception($"Gitleaks execution failed: {stderr}");

        if (!File.Exists(gitleaksOptions.ReportFilePath))
            throw new Exception("Gitleaks report file was not generated");

        var findingsCount = CountFindings(gitleaksOptions.ReportFilePath);

        return new ScanResult
        {
            FindingsCount = findingsCount,
            ReportPath = gitleaksOptions.ReportFilePath
        };
    }

    // =========================
    // PRIVATE HELPERS
    // =========================

    private static Process CreateProcess(string exePath, string arguments)
    {
        return new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = exePath,
                Arguments = arguments,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            }
        };
    }

    private static int CountFindings(string reportPath)
    {
        var json = File.ReadAllText(reportPath);

        using var doc = JsonDocument.Parse(json);

        // Gitleaks JSON is an array
        return doc.RootElement.ValueKind == JsonValueKind.Array
            ? doc.RootElement.GetArrayLength()
            : 0;
    }

    private static void Validate(GitleaksOptions options)
    {
        if (!File.Exists(options.GitleaksExePath))
            throw new FileNotFoundException("Gitleaks executable not found");

        if (!Directory.Exists(options.SourceFolder))
            throw new DirectoryNotFoundException("Source folder not found");

        var directory = Path.GetDirectoryName(options.ReportFilePath);
        if (!string.IsNullOrWhiteSpace(directory))
            Directory.CreateDirectory(directory);
    }
}
