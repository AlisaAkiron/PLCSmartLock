using OtpNet;

namespace PLCHost.Services;

public class OtpService : IOtpService
{
    private Totp _totp;

    public OtpService()
    {
        var key = GenerateOtpKey();
        _totp = new Totp(key);
    }

    /// <inheritdoc />
    public byte[] GenerateOtpKey()
    {
        var key = KeyGeneration.GenerateRandomKey(20);
        _totp = new Totp(key);
        return key;
    }

    /// <inheritdoc />
    public void SetOtpKey(byte[] key)
    {
        _totp = new Totp(key);
    }

    /// <inheritdoc />
    public (OtpPassword, int) CalculatePassword()
    {
        var current = DateTime.Now;
        var code = _totp.ComputeTotp(current).ToCharArray();
        var remainingTime = _totp.RemainingSeconds(current);

        var otpPassword = new OtpPassword
        {
            OtpPassword1 = (ushort)(code[0] - '0'),
            OtpPassword2 = (ushort)(code[1] - '0'),
            OtpPassword3 = (ushort)(code[2] - '0'),
            OtpPassword4 = (ushort)(code[3] - '0'),
            OtpPassword5 = (ushort)(code[4] - '0'),
            OtpPassword6 = (ushort)(code[5] - '0'),
        };

        return (otpPassword, remainingTime);
    }
}
