using Metron.Api.Filters;
using Metron.Api.Http;

namespace Metron.Api.Tests;

public class QueryStringBuilderTests
{
    [Fact]
    public void Build_ReturnsEmptyString_WhenFilterIsNull()
    {
        Assert.Equal(string.Empty, QueryStringBuilder.Build(null));
    }

    [Fact]
    public void Build_ReturnsEmptyString_WhenAllPropertiesAreNull()
    {
        Assert.Equal(string.Empty, QueryStringBuilder.Build(new ArcFilter()));
    }

    [Fact]
    public void Build_JoinsScalarAndDateFilters()
    {
        var filter = new ArcFilter
        {
            Name = "Infinity",
            CvId = 123,
            ModifiedGt = new DateTimeOffset(2024, 1, 2, 3, 4, 5, TimeSpan.Zero),
        };

        var query = QueryStringBuilder.Build(filter);

        Assert.StartsWith("?", query);
        Assert.Contains("name=Infinity", query);
        Assert.Contains("cv_id=123", query);
        Assert.Contains("modified_gt=2024-01-02T03%3A04%3A05.0000000%2B00%3A00", query);
    }

    [Fact]
    public void Build_JoinsArrayFiltersWithCommas()
    {
        var filter = new IssueFilter { RoleId = [1, 2, 3] };

        var query = QueryStringBuilder.Build(filter);

        Assert.Equal("?role_id=1%2C2%2C3", query);
    }

    [Fact]
    public void Build_EscapesSpecialCharacters()
    {
        var filter = new ArcFilter { Name = "Spider-Man & Friends" };

        var query = QueryStringBuilder.Build(filter);

        Assert.Equal("?name=Spider-Man%20%26%20Friends", query);
    }
}
