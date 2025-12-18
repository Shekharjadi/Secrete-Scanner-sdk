using System.Threading;
using System.Threading.Tasks;

namespace SecretsScanner.Abstractions;

public interface ISecretsScanner
{
    Task<ScanResult> ScanAsync(ScanOptions options, CancellationToken ct = default);

}
