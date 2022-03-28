using Quartz;

namespace PLCHost.Extension;

public static class QuartzJobExtension
{
    public static void AddPlcHostJobs(this IServiceCollection service)
    {
        service.AddQuartz(q =>
        {
            q.SchedulerName = "PLC Host Job Scheduler";
            q.SchedulerId = "PLC-Host-Job-Scheduler";
            q.UseMicrosoftDependencyInjectionJobFactory();
            q.UseSimpleTypeLoader();
            q.UseInMemoryStore();
            q.UseDefaultThreadPool(10);
        });

        service.AddQuartzHostedService(configure =>
        {
            configure.WaitForJobsToComplete = false;
        });
    }
}
