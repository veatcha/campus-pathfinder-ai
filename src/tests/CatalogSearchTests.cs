using Xunit;
using Azure.Search.Documents;
using Azure;
using System.Linq;

public class CatalogSearchTests
{
    private readonly SearchClient _client =
        new SearchClient(new Uri("https://<searchName>.search.windows.net"),
                         "knowledge-index",
                         new AzureKeyCredential("<adminKey>"));

    [Theory]
    [InlineData("pay tuition")]
    [InlineData("CS101")]
    public async Task TopThreeRecall_ShouldContainRelevantDoc(string query)
    {
        var results = await _client.SearchAsync<SearchDocument>(query, new SearchOptions { Size = 3 });
        var found = results.Value.GetResults().Any();
        Assert.True(found, $"Query '{query}' should return at least one doc in top 3.");
    }
}
