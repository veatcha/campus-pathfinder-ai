#r "nuget: Microsoft.Azure.Cosmos, 3.49.0"

using System.Text.Json;
using Microsoft.Azure.Cosmos;

var dataPath = Args.FirstOrDefault() ?? "../data/raw";
var cosmosConnString = Environment.GetEnvironmentVariable("COSMOS_CONN_STRING");

var client = new CosmosClient(cosmosConnString);
var container = client.GetContainer("campusdb", "knowledge");

var response = await client.GetContainer("campusdb","knowledge")
                           .ReadContainerAsync();
Console.WriteLine("PartitionKeyPath: " + response.Resource.PartitionKeyPath);

foreach (var file in Directory.EnumerateFiles(dataPath, "*.json"))
{
    Console.WriteLine($"Ingesting {file}");
    var json = await File.ReadAllTextAsync(file);
    var docs = JsonSerializer.Deserialize<JsonElement>(json);

    foreach (var element in docs.EnumerateArray())
{
    if (!element.TryGetProperty("id", out var idProp) ||
        !element.TryGetProperty("pk", out var pkProp))
    {
        Console.WriteLine("⚠️  Skipping doc missing id or pk: " + element);
        continue;
    }
    var idValue = idProp.GetString();
    var pkValue = pkProp.ValueKind == JsonValueKind.String
        ? pkProp.GetString()
        : pkProp.ToString();

    var rawJson = element.GetRawText();
    using var stream = new MemoryStream(Encoding.UTF8.GetBytes(rawJson));

    try
    {
        var streamResponse = await container.UpsertItemStreamAsync(
            stream,
            new PartitionKey(pkValue)
        );

        if (!streamResponse.IsSuccessStatusCode)
        {
            Console.WriteLine(
                $"❌ Upsert failed for id={idValue} pk={pkValue} " +
                $"Status={streamResponse.StatusCode} " +
                $"Error={streamResponse.ErrorMessage}"
            );
            break;  
        }
        Console.WriteLine($"✔  Upsert succeeded id={idValue} pk={pkValue}");
    }
    catch (CosmosException ex)
    {
        // 4) Log the full exception including SubStatus & server body
        Console.WriteLine($"🚨 CosmosException on id={idValue} pk={pkValue}");
        Console.WriteLine($"   StatusCode: {ex.StatusCode}");
        Console.WriteLine($"   SubStatus:  {ex.SubStatusCode}");
        Console.WriteLine($"   Message:    {ex.Message}");
        Console.WriteLine($"   Body:       {ex.ResponseBody}");
        break;
    }
}

}
Console.WriteLine("✅ Ingestion complete");
