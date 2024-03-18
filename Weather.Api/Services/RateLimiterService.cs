using Azure.Data.Tables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Weather.Api.Services;

public class RateLimiterService : IRateLimiterService
{
    private TableClient tableClient;

    public RateLimiterService(string connection, string tableName)
    {
        tableClient = new TableClient(connection, tableName);
        tableClient.CreateIfNotExists();

    }

    public async Task<bool> IsRateLimited(string client, TimeSpan limit)
    {
        var entity = await tableClient.GetEntityAsync<TableEntity>(partitionKey: "RateLimit", rowKey: client);
        if (entity != null)
        {
            var lastRequest = entity.Value.GetDateTime("LastRequest") ?? DateTime.MinValue;
            if (DateTime.UtcNow - lastRequest < limit)
                return true;
        }

        var newEntity = new TableEntity("RateLimit", client)
        {
            { "LastRequest", DateTime.UtcNow }
        };
        await tableClient.UpsertEntityAsync(newEntity);
        return false;
    }
}
