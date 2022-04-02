using PLCHost.Jobs;
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

            q.ScheduleJob<OtpJob>(trigger =>
            {
                trigger.WithIdentity("PLC-HOST-OTP-TRIGGER", "PLC-HOST-TRIGGER-GROUP")
                    .WithSimpleSchedule(schedule =>
                    {
                        schedule.WithIntervalInSeconds(2);
                    })
                    .StartAt(DateTimeOffset.Now.AddSeconds(10));
            }, job =>
            {
                job.WithIdentity("PLC-HOST-OTP-JOB", "PLC-HOST-JOB-GROUP");
            });
        });

        service.AddQuartzHostedService(configure =>
        {
            configure.WaitForJobsToComplete = false;
        });
    }
}
