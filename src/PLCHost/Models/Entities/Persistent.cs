using System.ComponentModel.DataAnnotations.Schema;

namespace PLCHost.Models.Entities;

[Table("persistent")]
public record Persistent
{
    [Column("id")]
    public Guid Id { get; set; }

    [Column("key")]
    public string Key { get; set; }

    [Column("value")]
    public string Value { get; set; }
}
