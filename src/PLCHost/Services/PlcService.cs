using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using S7.Net;

namespace PLCHost.Services;

public class PlcService : IPlcService
{
    private Plc? _plc;
    private readonly IOtpService _otpService;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<PlcService> _logger;
    private bool _isConnecting;

    public PlcService(IOtpService otpService, IServiceScopeFactory scopeFactory, ILogger<PlcService> logger)
    {
        _isConnecting = false;
        _otpService = otpService;
        _scopeFactory = scopeFactory;
        _logger = logger;

        var plcInfoString = ReadDatabase(DbPersistKey.PlcInfo).Result;
        if (plcInfoString is null)
        {
            logger.LogWarning("No PLC connection info stored in database");
            return;
        }

        var plcInfo = JsonSerializer.Deserialize<PlcInfo>(plcInfoString);

        if (plcInfo is null || plcInfo.Ip is null)
        {
            _logger.LogError("PLC info in database can not be deserialized");
            return;
        }

        _plc = new Plc(plcInfo.CpuType.ToCpuType(), plcInfo.Ip, plcInfo.Rack, plcInfo.Slot);
        Connect();
    }

    /// <inheritdoc />
    public void SetPlcConnectionInfo(PlcInfo plcInfo)
    {
        _logger.LogInformation("Set up new PLC info");
        if (GetPlcConnectionStatus())
        {
            _logger.LogInformation("Change PLC info, disconnect from current instance");
            DisConnect();
        }
        _plc = new Plc(plcInfo.CpuType.ToCpuType(), plcInfo.Ip!, plcInfo.Rack, plcInfo.Slot);

        WriteDatabase(DbPersistKey.PlcInfo, JsonSerializer.Serialize(plcInfo)).Wait();
    }

    /// <inheritdoc />
    public PlcInfo? GetPlcConnectionInfo()
    {
        if (_plc is not null)
        {
            return new PlcInfo()
            {
                CpuType = _plc.CPU.ToFormattedString(), Ip = _plc.IP, Rack = _plc.Rack, Slot = _plc.Slot
            };
        }

        _logger.LogWarning("Get PLC info but PLC instance is null");
        return null;
    }

    /// <inheritdoc />
    public bool Connect()
    {
        if (_plc is null)
        {
            return false;
        }
        if (_plc.IsConnected is false)
        {
            _logger.LogInformation("Start a PLC connection");
            try
            {
                _isConnecting = true;
                _plc.OpenAsync().GetAwaiter().GetResult();
            }
            catch (Exception)
            {
                // Ignore
            }
            finally
            {
                _isConnecting = false;
            }
        }

        if (GetPlcConnectionStatus())
        {
            _logger.LogInformation("PLC connected");
            WriteOpenWorkStatus().Wait();

            if (ReadOtpStatus().Result?.OtpEnabled != 1)
            {
                return GetPlcConnectionStatus();
            }

            var key = ReadOtpKey().Result!.OtpKeyArray;
            _otpService.SetOtpKey(key!);
        }
        else
        {
            _logger.LogWarning("Failed to connect to PLC");
        }

        return GetPlcConnectionStatus();
    }

    /// <inheritdoc />
    public bool DisConnect()
    {
        if (_plc?.IsConnected is true)
        {
            _logger.LogInformation("Start to disconnect to PLC");
            WriteCloseWorkStatus().Wait();
            _plc.Close();
        }

        if (GetPlcConnectionStatus())
        {
            _logger.LogWarning("Failed to disconnect");
        }
        else
        {
            _logger.LogInformation("PLC disconnected");
        }

        return !GetPlcConnectionStatus();
    }

    /// <inheritdoc />
    public bool GetPlcConnectionStatus()
    {
        if (_isConnecting)
        {
            return false;
        }
        return _plc is not null && _plc.IsConnected;
    }

    /// <inheritdoc />
    public async Task<bool> WriteOpenWorkStatus()
    {
        if (GetPlcConnectionStatus() is false)
        {
            _logger.LogError("Try to write open work status but PLC is disconnected");
            return false;
        }

        _logger.LogInformation("Write open work status to PLC");

        StaticPassword? staticPassword;
        var staticPasswordString = await ReadDatabase(DbPersistKey.StaticPassword);
        if (staticPasswordString is not null)
        {
            staticPassword = JsonSerializer.Deserialize<StaticPassword>(staticPasswordString);
        }
        else
        {
            var rnd = new Random();
            staticPassword = new StaticPassword
            {
                StaticPassword1 = (ushort)rnd.Next(0, 9),
                StaticPassword2 = (ushort)rnd.Next(0, 9),
                StaticPassword3 = (ushort)rnd.Next(0, 9),
                StaticPassword4 = (ushort)rnd.Next(0, 9),
                StaticPassword5 = (ushort)rnd.Next(0, 9),
                StaticPassword6 = (ushort)rnd.Next(0, 9),
                StaticPasswordAnalogHigh = (ushort)rnd.Next(50000, 60000),
                StaticPasswordAnalogLow = (ushort)rnd.Next(10000, 20000)
            };
        }

        await WriteStaticPassword(staticPassword);
        var workStatus = new PlcWorkStatus { WorkStatus = 1 };
        await _plc!.WriteClassAsync(workStatus, 9);
        return true;
    }

    /// <inheritdoc />
    public async Task<bool> WriteCloseWorkStatus()
    {
        if (GetPlcConnectionStatus())
        {
            _logger.LogInformation("Write close work status to PLC");
            await _plc!.WriteClassAsync(new PlcWorkStatus { WorkStatus = 0 }, 9);
            return true;
        }

        _logger.LogError("Try to write close work status but PLC is disconnected");
        return false;
    }

    /// <inheritdoc />
    public async Task<bool> WriteOtpKey(OtpKey otpKey)
    {
        if (GetPlcConnectionStatus())
        {
            var serialized = JsonSerializer.Serialize(otpKey);
            await WriteDatabase(DbPersistKey.OtpKey, serialized);
            _logger.LogDebug("Write OTP key to PLC");
            await _plc!.WriteBytesAsync(DataType.DataBlock, 6, 0, otpKey.OtpKeyArray!);
            return true;
        }

        _logger.LogError("Try to write OTP key but PLC is disconnected");
        return false;
    }

    /// <inheritdoc />
    public async Task<bool> WriteOtpPassword(OtpPassword otpPassword)
    {
        if (GetPlcConnectionStatus())
        {
            _logger.LogDebug("Write OTP password {Password} to PLC",
                $"{otpPassword.OtpPassword1}{otpPassword.OtpPassword2}{otpPassword.OtpPassword3}" +
                $"{otpPassword.OtpPassword4}{otpPassword.OtpPassword5}{otpPassword.OtpPassword6}");
            await _plc!.WriteClassAsync(otpPassword, 3);
            return true;
        }

        _logger.LogError("Try to write OTP password but PLC is disconnected");
        return false;
    }

    /// <inheritdoc />
    public async Task<bool> WriteOtpStatus(OtpStatus otpStatus)
    {
        if (GetPlcConnectionStatus())
        {
            _logger.LogInformation("Write OTP status {Status} to PLC", otpStatus.OtpEnabled);
            await _plc!.WriteClassAsync(otpStatus, 5);
            return true;
        }

        _logger.LogError("Try to write OTP status but PLC is disconnected");
        return false;
    }

    /// <inheritdoc />
    public async Task<bool> WriteStaticPassword(StaticPassword? staticPassword)
    {
        if (GetPlcConnectionStatus())
        {
            var serialized = JsonSerializer.Serialize(staticPassword);
            await WriteDatabase(DbPersistKey.StaticPassword, serialized);
            _logger.LogDebug("Write static password {Password} to PLC",
                $"{staticPassword!.StaticPassword1}{staticPassword.StaticPassword2}{staticPassword.StaticPassword3}" +
                $"{staticPassword.StaticPassword4}{staticPassword.StaticPassword5}{staticPassword.StaticPassword6}|" +
                $"{staticPassword.StaticPasswordAnalogLow}-{staticPassword.StaticPasswordAnalogHigh}");
            await _plc!.WriteClassAsync(staticPassword, 2);
            return true;
        }

        _logger.LogError("Try to write static password but PLC is disconnected");
        return false;
    }

    /// <inheritdoc />
    public async Task<OtpKey?> ReadOtpKey()
    {
        if (!GetPlcConnectionStatus())
        {
            return null;
        }

        var data = await _plc!.ReadBytesAsync(DataType.DataBlock, 6, 0, 20);
        return new OtpKey { OtpKeyArray = data };
    }

    /// <inheritdoc />
    public async Task<OtpStatus?> ReadOtpStatus()
    {
        if (!GetPlcConnectionStatus())
        {
            return null;
        }

        var data = await _plc!.ReadClassAsync<OtpStatus>(5);
        return data;
    }

    /// <inheritdoc />
    public async Task<InputPassword?> ReadInputPassword()
    {
        if (!GetPlcConnectionStatus())
        {
            return null;
        }

        var data = await _plc!.ReadClassAsync<InputPassword>(1);
        return data;
    }

    /// <inheritdoc />
    public async Task<StaticPassword?> ReadStaticPassword()
    {
        if (!GetPlcConnectionStatus())
        {
            return null;
        }

        var data = await _plc!.ReadClassAsync<StaticPassword>(2);
        return data;
    }

    /// <summary>
    /// 写入数据库
    /// </summary>
    /// <param name="key">Key</param>
    /// <param name="value">序列化后的字符串值</param>
    private async Task WriteDatabase(DbPersistKey key, string? value)
    {
        using var scope = _scopeFactory.CreateScope();
        await using var db = scope.ServiceProvider.GetRequiredService<PlcHostDbContext>();

        var exist = await db.Persistent!.FirstOrDefaultAsync(x => x.Key == key.ToString());
        if (exist is null)
        {
            await db.Persistent!.AddAsync(new Persistent { Id = Guid.NewGuid(), Key = key.ToString(), Value = value });
        }
        else
        {
            exist.Value = value;
            db.Update(exist);
        }

        await db.SaveChangesAsync();
    }

    /// <summary>
    /// 读取数据库
    /// </summary>
    /// <param name="key">Key</param>
    /// <returns>序列化后的字符串值</returns>
    private async Task<string?> ReadDatabase(DbPersistKey key)
    {
        using var scope = _scopeFactory.CreateScope();
        await using var db = scope.ServiceProvider.GetRequiredService<PlcHostDbContext>();

        var exist = await db.Persistent!.FirstOrDefaultAsync(x => x.Key == key.ToString());

        return exist?.Value;
    }
}
