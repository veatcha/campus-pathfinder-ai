#r "nuget: Azure.Search.Documents, 11.6.0"

using Azure;
using Azure.Search.Documents.Indexes;
using Azure.Search.Documents.Indexes.Models;

var endpoint = Environment.GetEnvironmentVariable("SEARCH_ENDPOINT");
var key = Environment.GetEnvironmentVariable("SEARCH_ADMIN_KEY");
var cosmos = Environment.GetEnvironmentVariable("COSMOS_CONN_STRING")+"database=campusdb;";

if (string.IsNullOrEmpty(endpoint) || string.IsNullOrEmpty(key) || string.IsNullOrEmpty(cosmos))
{
    Console.Error.WriteLine("Missing env vars (SEARCH_ENDPOINT, SEARCH_ADMIN_KEY, COSMOS_CONN_STRING)");
    return;
}

Console.WriteLine(cosmos);

var service = new Uri(endpoint);
var cred = new AzureKeyCredential(key);

var indexClient = new SearchIndexClient(service, cred);
var indexerClient = new SearchIndexerClient(service, cred);

const string ds = "knowledge-cosmos";
const string ix = "knowledge-index";
const string ixr = "knowledge-indexer";

// Data source
var source = new SearchIndexerDataSourceConnection(ds,
    SearchIndexerDataSourceType.CosmosDb, cosmos, new SearchIndexerDataContainer("knowledge"))
{
    DataChangeDetectionPolicy = new HighWaterMarkChangeDetectionPolicy("_ts")
};
await indexerClient.CreateOrUpdateDataSourceConnectionAsync(source);
Console.WriteLine("✅ Data source");

// Index
var index = new SearchIndex(ix)
{
    Fields =
    {
        new SimpleField("pk",   SearchFieldDataType.String) { IsKey = true,  IsFilterable = true },
        new SimpleField("type", SearchFieldDataType.String) { IsFilterable = true },
        new SearchableField("question"),
        new SearchableField("answer"),
        new SimpleField("courseId", SearchFieldDataType.String) { IsFilterable = true },
        new SearchableField("title"),
        new SearchableField("description")
    }
};
await indexClient.CreateOrUpdateIndexAsync(index);
Console.WriteLine("✅ Index");

// Indexer
var indexer = new SearchIndexer(ixr, ds, ix) { Schedule = new IndexingSchedule(TimeSpan.FromHours(1)) };
await indexerClient.CreateOrUpdateIndexerAsync(indexer);
await indexerClient.RunIndexerAsync(ixr);
Console.WriteLine("▶️ Indexer run");
