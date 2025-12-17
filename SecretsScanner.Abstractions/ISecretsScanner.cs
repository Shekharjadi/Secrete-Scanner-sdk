namespace SecretsScanner.Abstractions;

public interface ISecretsScanner
{
    Task<ScanResult> ScanAsync(ScanOptions options, CancellationToken ct = default);

}
