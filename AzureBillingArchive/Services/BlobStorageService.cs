using Azure.Storage.Blobs;
using System.Text.Json;
using System.Threading.Tasks;

public class BlobStorageService
{
    private readonly BlobContainerClient _container;

    public BlobStorageService(string connectionString, string containerName)
    {
        var client = new BlobServiceClient(connectionString);
        _container = client.GetBlobContainerClient(containerName);
        _container.CreateIfNotExists();
    }

    public async Task UploadAsync(BillingRecord record)
    {
        var blobName = $"billing/{record.Id}.json";
        var blob = _container.GetBlobClient(blobName);
        var json = JsonSerializer.Serialize(record);
        using var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(json));
        await blob.UploadAsync(stream, overwrite: true);
    }
}
