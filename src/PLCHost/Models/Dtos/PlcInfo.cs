namespace PLCHost.Models.Dtos;

public record PlcInfo
{
    public string Ip { get; set; }
    public string CpuType { get; set; }
    public int Rack { get; set; }
    public int Slot { get; set; }
}
