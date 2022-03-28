namespace PLCHost.Models.Dtos;

public record PlcInfo
{
    public string Ip { get; set; }
    public string CpuType { get; set; }
    public short Rack { get; set; }
    public short Slot { get; set; }
}
