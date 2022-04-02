using Quartz;

namespace PLCHost.Jobs;

public class OtpJob : IJob
{
    private readonly IPlcService _plcService;
    private readonly IOtpService _otpService;

    public OtpJob(IPlcService plcService, IOtpService otpService)
    {
        _plcService = plcService;
        _otpService = otpService;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        if (_plcService.GetPlcConnectionStatus() is false)
        {
            return;
        }

        var otpStatus = await _plcService.ReadOtpStatus();
        if (otpStatus is not null && otpStatus.OtpEnabled == 0)
        {
            return;
        }

        var (otpPassword, _) = _otpService.CalculatePassword();
        await _plcService.WriteOtpPassword(otpPassword);
    }
}
