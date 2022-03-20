namespace PLCHost.Models.Entities;

public record Persistent
{
    public Guid Id { get; set; }
    public string? Key { get; set; }
    public string? Value { get; set; }
}
