namespace PLCHost.Services.Interfaces;

public interface IOtpService
{
    /// <summary>
    /// 获取新的 OTP 密钥
    /// </summary>
    /// <returns></returns>
    byte[] GenerateOtpKey();
    /// <summary>
    /// 设置 OTP 密钥
    /// <param name="key"></param>
    /// </summary>
    void SetOtpKey(byte[] key);
    /// <summary>
    /// 计算密码和剩余时间
    /// </summary>
    /// <returns></returns>
    (OtpPassword, int) CalculatePassword();
}
