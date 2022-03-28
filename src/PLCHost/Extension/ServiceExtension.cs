namespace PLCHost.Extension;

public static class ServiceExtension
{
    public static void AddPlcHostServices(this IServiceCollection service)
    {
        service.AddSingleton<IPlcService, PlcService>();
        service.AddSingleton<IOtpService, OtpService>();
    }
}
