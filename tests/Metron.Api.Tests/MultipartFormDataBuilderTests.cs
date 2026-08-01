using Metron.Api.Http;
using Metron.Api.Models;

namespace Metron.Api.Tests;

public class MultipartFormDataBuilderTests
{
    [Fact]
    public async Task Build_OmitsNullProperties()
    {
        var arc = new Arc { Name = "Infinity Gauntlet" };

        using var content = MultipartFormDataBuilder.Build(arc);
        var parts = await ReadPartsAsync(content);

        Assert.Equal(new[] { ("name", "Infinity Gauntlet") }, parts);
    }

    [Fact]
    public async Task Build_SerializesArraysAsRepeatedFields()
    {
        var character = new Character { Name = "Spider-Man", Creators = [1, 2, 3] };

        using var content = MultipartFormDataBuilder.Build(character);
        var parts = await ReadPartsAsync(content);

        Assert.Contains(("name", "Spider-Man"), parts);
        Assert.Equal(3, parts.Count(p => p.Name == "creators"));
        Assert.Contains(("creators", "1"), parts);
        Assert.Contains(("creators", "2"), parts);
        Assert.Contains(("creators", "3"), parts);
    }

    [Fact]
    public async Task Build_SerializesIntBackedEnumAsNumber()
    {
        var series = new Series { Name = "Amazing Spider-Man", Status = StatusEnum.Ongoing };

        using var content = MultipartFormDataBuilder.Build(series);
        var parts = await ReadPartsAsync(content);

        Assert.Contains(("status", "4"), parts);
    }

    [Fact]
    public async Task Build_SerializesBoolAsLowercaseString()
    {
        using var content = MultipartFormDataBuilder.Build(new CollectionRead { IsRead = true });
        var parts = await ReadPartsAsync(content);

        Assert.Contains(("is_read", "true"), parts);
    }

    [Fact]
    public async Task Build_FormatsDateOnlyAsIso()
    {
        var issue = new Issue { Number = "1", CoverDate = new DateOnly(2024, 3, 15) };

        using var content = MultipartFormDataBuilder.Build(issue);
        var parts = await ReadPartsAsync(content);

        Assert.Contains(("cover_date", "2024-03-15"), parts);
    }

    private static async Task<List<(string Name, string Value)>> ReadPartsAsync(MultipartFormDataContent content)
    {
        var results = new List<(string, string)>();
        foreach (var part in content)
        {
            var name = part.Headers.ContentDisposition?.Name?.Trim('"') ?? string.Empty;
            var value = await part.ReadAsStringAsync();
            results.Add((name, value));
        }

        return results;
    }
}
