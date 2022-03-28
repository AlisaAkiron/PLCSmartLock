namespace PLCHost.Services.Interfaces;

public interface IPlcService
{
    /// <summary>
    /// 设置 PLC 连接信息，若已有一个连接，会先断开当前连接
    /// </summary>
    /// <param name="plcInfo"></param>
    void SetPlcConnectionInfo(PlcInfo plcInfo);
    /// <summary>
    /// 获取 PLC 连接信息
    /// </summary>
    /// <returns></returns>
    PlcInfo GetPlcConnectionInfo();

    /// <summary>
    /// 连接，返回 <see cref="GetPlcConnectionStatus"/> 的值
    /// </summary>
    bool Connect();
    /// <summary>
    /// 断开连接，返回 <see cref="GetPlcConnectionStatus"/> 的值取反
    /// </summary>
    bool DisConnect();
    /// <summary>
    /// 获取 PLC 连接状态
    /// </summary>
    /// <returns></returns>
    bool GetPlcConnectionStatus();

    /// <summary>
    /// 写入开始工作状态标志
    /// </summary>
    /// <returns></returns>
    Task<bool> WriteOpenWorkStatus();
    /// <summary>
    /// 写入工作停止状态标志
    /// </summary>
    /// <returns></returns>
    Task<bool> WriteCloseWorkStatus();

    /// <summary>
    /// 写入 OTP 密钥
    /// </summary>
    /// <param name="otpKey"></param>
    /// <returns></returns>
    Task<bool> WriteOtpKey(OtpKey otpKey);
    /// <summary>
    /// 写入 OTP 密码
    /// </summary>
    /// <param name="otpPassword"></param>
    /// <returns></returns>
    Task<bool> WriteOtpPassword(OtpPassword otpPassword);
    /// <summary>
    /// 写入 OTP 状态
    /// </summary>
    /// <param name="otpStatus"></param>
    /// <returns></returns>
    Task<bool> WriteOtpStatus(OtpStatus otpStatus);
    /// <summary>
    /// 写入静态密码
    /// </summary>
    /// <param name="staticPassword"></param>
    /// <returns></returns>
    Task<bool> WriteStaticPassword(StaticPassword staticPassword);

    /// <summary>
    /// 获取 OTP 密钥
    /// </summary>
    /// <returns></returns>
    Task<OtpKey?> ReadOtpKey();
    /// <summary>
    /// 获取 OTP 状态
    /// </summary>
    /// <returns></returns>
    Task<OtpStatus?> ReadOtpStatus();
    /// <summary>
    /// 获取静态密码
    /// </summary>
    /// <returns></returns>
    Task<StaticPassword?> ReadStaticPassword();
}
