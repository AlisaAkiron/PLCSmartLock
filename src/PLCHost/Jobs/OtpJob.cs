using Quartz;

namespace PLCHost.Jobs;

public class OtpJob : IJob
{
    private readonly IPlcService _plcService;
    private readonly IOtpService _otpService;
    private readonly ILogger<OtpJob> _logger;

    public OtpJob(IPlcService plcService, IOtpService otpService, ILogger<OtpJob> logger)
    {
        _plcService = plcService;
        _otpService = otpService;
        _logger = logger;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        try
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
        catch (Exception e)
        {
            _logger.LogError(e, "Error");
        }
    }
}
