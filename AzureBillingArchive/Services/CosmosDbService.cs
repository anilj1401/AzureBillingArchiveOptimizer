using Microsoft.Azure.Cosmos;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class CosmosDbService
{
    private readonly Container _container;

    public CosmosDbService(CosmosClient client, string dbName, string containerName)
    {
        _container = client.GetContainer(dbName, containerName);
    }

    public async Task<List<BillingRecord>> GetOldBillingRecordsAsync(DateTime cutoffDate)
    {
        var query = new QueryDefinition("SELECT * FROM c WHERE c.Timestamp < @cutoff")
                        .WithParameter("@cutoff", cutoffDate);

        var results = new List<BillingRecord>();
        using var feed = _container.GetItemQueryIterator<BillingRecord>(query);

        while (feed.HasMoreResults)
        {
            var page = await feed.ReadNextAsync();
            results.AddRange(page);
        }

        return results;
    }

    public async Task DeleteRecordAsync(string id, string partitionKey)
    {
        await _container.DeleteItemAsync<BillingRecord>(id, new PartitionKey(partitionKey));
    }
}
