namespace Metron.Api.Http;

/// <summary>
/// Marks a filter property with the wire name of the API query parameter it maps to.
/// Read by <see cref="QueryStringBuilder"/> via reflection.
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public sealed class QueryParameterAttribute(string name) : Attribute
{
    public string Name { get; } = name;
}
