using System.Timers;
using Timer = System.Timers.Timer;

namespace PLCHost.Jobs;

public class OtpJob : IOtpJob
{
    private readonly IPlcService _plcService;
    private readonly IOtpService _otpService;
    private readonly ILogger<OtpJob> _logger;

    private readonly Timer _timer;

    public OtpJob(IPlcService plcService, IOtpService otpService, ILogger<OtpJob> logger)
    {
        _plcService = plcService;
        _otpService = otpService;
        _logger = logger;

        _timer = new Timer { Interval = 1000, AutoReset = true, Enabled = false, };
        _timer.Elapsed += Execute;
    }

    private void Execute(object? sender, ElapsedEventArgs args)
    {
        try
        {
            if (_plcService.GetPlcConnectionStatus() is false)
            {
                return;
            }

            var otpStatus = _plcService.ReadOtpStatus().Result;
            if (otpStatus is not null && otpStatus.OtpEnabled == 0)
            {
                return;
            }

            var (otpPassword, _) = _otpService.CalculatePassword();
            _plcService.WriteOtpPassword(otpPassword).GetAwaiter().GetResult();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error");
        }
    }

    public void Start()
    {
        _timer.Start();
        _logger.LogInformation("Otp Job started");
    }

    public void Stop()
    {
        _timer.Stop();
        _logger.LogInformation("Otp Job stopped");
    }
}
