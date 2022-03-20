using S7.Net;

namespace PLCHost.Extension;

public static class PlcCpuTypeExtension
{
    public static string ToFormattedString(this CpuType cpuType)
    {
        return cpuType switch
        {
            CpuType.S7200 => "SIMATIC S7-200",
            CpuType.Logo0BA8 => "LOGO! 0BA8",
            CpuType.S7200Smart => "SIMATIC S7-200 SMART",
            CpuType.S7300 => "SIMATIC S7-300",
            CpuType.S7400 => "SIMATIC S7-400",
            CpuType.S71200 => "SIMATIC S7-1200",
            CpuType.S71500 => "SIMATIC S7-1500",
            _ => throw new ArgumentOutOfRangeException(nameof(cpuType))
        };
    }

    public static CpuType ToCpuType(this string cpuTypeString)
    {
        return cpuTypeString switch
        {
            "SIMATIC S7-200" => CpuType.S7200,
            "LOGO! 0BA8" => CpuType.Logo0BA8,
            "SIMATIC S7-200 SMART" => CpuType.S7200Smart,
            "SIMATIC S7-300" => CpuType.S7300,
            "SIMATIC S7-400" => CpuType.S7400,
            "SIMATIC S7-1200" => CpuType.S71200,
            "SIMATIC S7-1500" => CpuType.S71500,
            _ => throw new ArgumentOutOfRangeException(nameof(cpuTypeString))
        };
    }
}
