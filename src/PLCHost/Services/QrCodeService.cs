using QRCoder;

namespace PLCHost.Services;

public class QrCodeService : IQrCodeService
{
    private QRCodeGenerator _qrCodeGenerator;

    public QrCodeService()
    {
        _qrCodeGenerator = new QRCodeGenerator();
    }

    public string GetQrCodeBase64(string text, int size)
    {
        var qrCodeData = _qrCodeGenerator.CreateQrCode(text, QRCodeGenerator.ECCLevel.Q);
        var pngQrCode = new PngByteQRCode(qrCodeData);
        var pngBytes = pngQrCode.GetGraphic(size);

        var base64 = Convert.ToBase64String(pngBytes);

        return base64;
    }
}
