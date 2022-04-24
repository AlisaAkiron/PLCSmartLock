namespace PLCHost.Services.Interfaces;

public interface IQrCodeService
{
    string GetQrCodeBase64(string text, int size);
}
