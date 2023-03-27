namespace Domain.Identity.Entities;

public sealed class ApplicationClaimValue
{
    public string Type { get; set; } = string.Empty;

    public string Value { get; set; } = string.Empty;

    public ApplicationClaimValue()
    {
    }

    public ApplicationClaimValue(string type, string value)
    {
        Type = type;
        Value = value;
    }
}
