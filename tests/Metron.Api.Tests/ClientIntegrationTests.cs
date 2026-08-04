using System.Net;
using Metron.Api.Exceptions;
using Metron.Api.Filters;

namespace Metron.Api.Tests;

public class ClientIntegrationTests
{
    private static MetronClient CreateClient(FakeHttpMessageHandler fake) =>
        new(new MetronClientOptions
        {
            ApiToken = "test-token",
            TransportHandler = fake,
        });

    [Fact]
    public async Task Arc_GetAsync_DeserializesResponse_AndSendsBearerAuth()
    {
        var fake = new FakeHttpMessageHandler();
        fake.Enqueue(FakeHttpMessageHandler.Json(HttpStatusCode.OK, """
            {"id": 42, "name": "Infinity Gauntlet", "desc": "", "resource_url": "https://metron.cloud/arc/42/", "modified": "2024-01-01T00:00:00Z"}
            """));

        using var client = CreateClient(fake);
        var arc = await client.Arc.GetAsync(42);

        Assert.Equal(42, arc.Id);
        Assert.Equal("Infinity Gauntlet", arc.Name);

        var request = Assert.Single(fake.Requests);
        Assert.Equal("Bearer", request.Headers.Authorization?.Scheme);
        Assert.Equal("test-token", request.Headers.Authorization?.Parameter);
        Assert.Equal("https://metron.cloud/api/arc/42/", request.RequestUri!.ToString());
    }

    [Fact]
    public async Task Issue_ListAsync_BuildsQueryStringFromFilter()
    {
        var fake = new FakeHttpMessageHandler();
        fake.Enqueue(FakeHttpMessageHandler.Json(HttpStatusCode.OK, """
            {"count": 0, "next": null, "previous": null, "results": []}
            """));

        using var client = CreateClient(fake);
        await client.Issue.ListAsync(new IssueFilter { SeriesId = 7, CvId = 99 });

        var request = Assert.Single(fake.Requests);
        Assert.Contains("series_id=7", request.RequestUri!.Query);
        Assert.Contains("cv_id=99", request.RequestUri!.Query);
    }

    [Fact]
    public async Task Arc_ListAllAsync_FollowsNextLinkAcrossPages()
    {
        var fake = new FakeHttpMessageHandler();
        fake.Enqueue(FakeHttpMessageHandler.Json(HttpStatusCode.OK, """
            {"count": 2, "next": "https://metron.cloud/api/arc/?page=2", "previous": null,
             "results": [{"id": 1, "name": "Arc One", "modified": "2024-01-01T00:00:00Z"}]}
            """));
        fake.Enqueue(FakeHttpMessageHandler.Json(HttpStatusCode.OK, """
            {"count": 2, "next": null, "previous": "https://metron.cloud/api/arc/?page=1",
             "results": [{"id": 2, "name": "Arc Two", "modified": "2024-01-01T00:00:00Z"}]}
            """));

        using var client = CreateClient(fake);
        var names = new List<string?>();
        await foreach (var arc in client.Arc.ListAllAsync())
        {
            names.Add(arc.Name);
        }

        Assert.Equal(["Arc One", "Arc Two"], names);
        Assert.Equal(2, fake.Requests.Count);
        Assert.Equal("https://metron.cloud/api/arc/?page=2", fake.Requests[1].RequestUri!.ToString());
    }

    [Fact]
    public async Task GetAsync_ThrowsMetronApiException_WithDetailFromBody_OnError()
    {
        var fake = new FakeHttpMessageHandler();
        fake.Enqueue(FakeHttpMessageHandler.Json(HttpStatusCode.NotFound, """{"detail": "Not found."}"""));

        using var client = CreateClient(fake);
        var exception = await Assert.ThrowsAsync<MetronApiException>(() => client.Arc.GetAsync(999));

        Assert.Equal(HttpStatusCode.NotFound, exception.StatusCode);
        Assert.Equal("Not found.", exception.Detail);
    }

    [Fact]
    public async Task Arc_CreateAsync_SendsMultipartFormData()
    {
        var fake = new FakeHttpMessageHandler();
        fake.Enqueue(FakeHttpMessageHandler.Json(HttpStatusCode.Created, """
            {"id": 1, "name": "New Arc", "modified": "2024-01-01T00:00:00Z", "resource_url": "https://metron.cloud/arc/1/"}
            """));

        using var client = CreateClient(fake);
        var created = await client.Arc.CreateAsync(new Models.Arc { Name = "New Arc" });

        Assert.Equal("New Arc", created.Name);
        var request = Assert.Single(fake.Requests);
        Assert.Equal(HttpMethod.Post, request.Method);
        Assert.IsType<MultipartFormDataContent>(request.Content);
    }

    [Fact]
    public async Task Collection_AddAsync_SendsMultipartFormData_AndDeserializesResponse()
    {
        var fake = new FakeHttpMessageHandler();
        fake.Enqueue(FakeHttpMessageHandler.Json(HttpStatusCode.Created, """
            {"id": 99, "quantity": 2, "book_format": "PRINT"}
            """));

        using var client = CreateClient(fake);
        var added = await client.Collection.AddAsync(new Models.CollectionAddItem
        {
            IssueId = 123,
            Quantity = 2,
            BookFormat = Models.BookFormatEnum.PRINT,
        });

        Assert.Equal(99, added.Id);
        Assert.Equal(2, added.Quantity);

        var request = Assert.Single(fake.Requests);
        Assert.Equal(HttpMethod.Post, request.Method);
        Assert.IsType<MultipartFormDataContent>(request.Content);
        Assert.Equal("https://metron.cloud/api/collection/add/", request.RequestUri!.ToString());
    }

    [Fact]
    public async Task Collection_RemoveAsync_SendsDeleteRequest()
    {
        var fake = new FakeHttpMessageHandler();
        fake.Enqueue(new HttpResponseMessage(HttpStatusCode.NoContent));

        using var client = CreateClient(fake);
        await client.Collection.RemoveAsync(7);

        var request = Assert.Single(fake.Requests);
        Assert.Equal(HttpMethod.Delete, request.Method);
        Assert.Equal("https://metron.cloud/api/collection/7/", request.RequestUri!.ToString());
    }
}
