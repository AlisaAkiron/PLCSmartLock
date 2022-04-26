namespace PLCHost;

public class HostedService : IHostedService
{
    private readonly IPlcService _plcService;
    private readonly IOtpJob _otpJob;

    public HostedService(IPlcService plcService, IOtpJob otpJob)
    {
        _plcService = plcService;
        _otpJob = otpJob;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _otpJob.Start();
        return Task.CompletedTask;
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        _otpJob.Stop();
        await _plcService.DisConnect();
    }
}
