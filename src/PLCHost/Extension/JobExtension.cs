namespace PLCHost.Extension;

public static class JobExtension
{
    public static void AddPlcHostJobs(this IServiceCollection service)
    {
        service.AddSingleton<IOtpJob, OtpJob>();
    }
}
