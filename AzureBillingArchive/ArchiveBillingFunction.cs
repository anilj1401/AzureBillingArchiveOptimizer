using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

public class ArchiveBillingFunction
{
    private readonly CosmosDbService _cosmosDb;
    private readonly BlobStorageService _blobService;

    public ArchiveBillingFunction(CosmosDbService cosmosDb, BlobStorageService blobService)
    {
        _cosmosDb = cosmosDb;
        _blobService = blobService;
    }

    [FunctionName("ArchiveBillingRecords")]
    public async Task Run([TimerTrigger("0 0 2 * * *")] TimerInfo myTimer, ILogger log)
    {
        log.LogInformation($"Archival started at: {DateTime.UtcNow}");

        var cutoffDate = DateTime.UtcNow.AddMonths(-3);
        var oldRecords = await _cosmosDb.GetOldBillingRecordsAsync(cutoffDate);

        foreach (var record in oldRecords)
        {
            await _blobService.UploadAsync(record);
            await _cosmosDb.DeleteRecordAsync(record.Id, record.PartitionKey);
        }

        log.LogInformation($"Archival completed at: {DateTime.UtcNow}, Total archived: {oldRecords.Count}");
    }
}
