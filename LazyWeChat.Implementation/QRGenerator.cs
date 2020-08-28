using LazyWeChat.Abstract;
using QRCoder;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace LazyWeChat.Implementation
{
    public class QRGenerator : IQRGenerator
    {
        public virtual byte[] Generate(string content)
        {
            QRCodeGenerator qrGenerator = new QRCodeGenerator();
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(content, QRCodeGenerator.ECCLevel.Q);
            QRCode qrcode = new QRCode(qrCodeData);

            Bitmap bmp = qrcode.GetGraphic(5, Color.Black, Color.White, null, 15, 6, false);

            //保存为PNG到内存流  
            MemoryStream ms = new MemoryStream();
            bmp.Save(ms, ImageFormat.Png);

            //输出二维码图片
            return ms.GetBuffer();
        }
    }
}
