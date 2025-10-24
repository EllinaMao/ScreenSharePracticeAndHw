using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using static System.Windows.Forms.DataFormats;

/*to learn
 https://learn.microsoft.com/ru-ru/dotnet/api/system.drawing.bitmap?view=windowsdesktop-9.0&viewFallbackFrom=dotnet-plat-ext-8.0

https://learn.microsoft.com/ru-ru/dotnet/api/system.drawing.graphics?view=windowsdesktop-9.0
Инкапсулирует поверхность рисования GDI+. Этот класс не наследуется.
Класс Graphics предоставляет методы для рисования объектов на устройстве отображения. объект Graphics связан с определенным контекстом устройства.

FromImage(Image)	
Создает новый объект Graphics из указанного объекта Image.



https://learn.microsoft.com/ru-ru/dotnet/api/system.drawing.imaging.imagecodecinfo?view=windowsdesktop-9.0
GetImageDecoders()	
Возвращает массив объектов ImageCodecInfo, содержащий информацию о декодерах изображений, встроенных в GDI+.

https://learn.microsoft.com/ru-ru/dotnet/api/system.bitconverter?view=net-9.0
Преобразует базовые типы данных в массив байтов и массив байтов в базовые типы данных.

https://learn.microsoft.com/ru-ru/dotnet/api/system.drawing.imaging.imagecodecinfo?view=windowsdesktop-9.0
GDI+ использует кодировщики изображений для преобразования изображений, хранящихся в объектах, в Bitmap различные форматы файлов. Кодировщики изображений встроены в GDI+ для форматов BMP, JPEG, GIF, TIFF и PNG. Кодировщик вызывается при вызове Save метода Image или SaveAdd объекта .


не помогло вообще, но раз уж нашла сохраню хоть
 https://learn.microsoft.com/ru-ru/dotnet/desktop/winforms/advanced/how-to-crop-and-scale-images

 */
namespace ScreenSharePracticeAndHw
{
    internal class ControlsClient : IDisposable
    {
        private UdpClient udpClient;
        private SynchronizationContext ctx;
        private CancellationTokenSource StopShareToken;

        static readonly IPAddress SERVER_IP = IPAddress.Parse("127.0.0.1");
        private const int SERVER_PORT = 8888;
        private const int MAX_PACKET_SIZE = 60000;

        public event Action<string> LogServer;
        public event Action<Bitmap> ScreenshotCaptured;
        public ControlsClient(SynchronizationContext ctx = null)
        {
            this.ctx = ctx;
        }
        public void Log(string message)
        {
            if (ctx != null)
            {
                ctx.Post(d => LogServer?.Invoke(message), null);
                return;
            }

            LogServer?.Invoke(message);
        }
        public void LogPicture(Bitmap pict)
        {
            if (ctx != null)
            {
                Bitmap clone = (Bitmap)pict.Clone();

                ctx.Post(d => ScreenshotCaptured?.Invoke(clone), null);
                return;
            }

            ScreenshotCaptured?.Invoke((Bitmap)pict.Clone());
        }

        public void Connect()
        {
            try
            {
                udpClient = new UdpClient();
                IPEndPoint serverEndpoint = new IPEndPoint(SERVER_IP, SERVER_PORT);
                udpClient.Connect(serverEndpoint);          
                Log("We started!");
            }
            catch (Exception ex)
            {
                Log($"error {ex.Message}");
            }
        }

        public void StartSharing()
        {
            if (udpClient == null)
            {
                Log("Please connect before");
                return;
            }
            StopShareToken = new CancellationTokenSource();
            Log("Starting sharing");
            Task.Run(() => SharingLoopAsync(StopShareToken.Token));
        }

        private async Task SharingLoopAsync(CancellationToken token)
        {
            try
            {
                while (!token.IsCancellationRequested)
                {
                    using (Bitmap screenshot = CaptureScreen())
                    {
                        LogPicture(screenshot);
                        byte[] data = ImageToBytes(screenshot, 50L);
                        await SendImageAsync(data, token);
                    }
                    await Task.Delay(1000, token);
                }
            }
            catch (OperationCanceledException op)
            {
                Log(op.Message);
            }
            catch (Exception ex)
            {
                Log(ex.Message);
            }
        }
        //фрагментируем отправку
        private async Task SendImageAsync(byte[] data, CancellationToken token)
        {
            byte[] frameId = Guid.NewGuid().ToByteArray(); //16
            int totalPackets = (data.Length + MAX_PACKET_SIZE - 1) / MAX_PACKET_SIZE;
            byte[] totalPacketsBytes = BitConverter.GetBytes(totalPackets); // 4

            for (int i = 0; i < totalPackets; i++)
            {
                if (token.IsCancellationRequested)
                {
                    break;
                }
                int offset = i * MAX_PACKET_SIZE;
                int size = Math.Min(data.Length - offset, MAX_PACKET_SIZE);
                byte[] packetIndexBytes = BitConverter.GetBytes(i); // 4

                using (MemoryStream pStream = new MemoryStream())
                {
                    //заголовок
                    pStream.Write(frameId, 0, frameId.Length);         // 16 
                    pStream.Write(packetIndexBytes, 0, 4);             // 4
                    pStream.Write(totalPacketsBytes, 0, 4);            // 4 

                    // кусок данных 
                    pStream.Write(data, offset, size);

                    //собранный пакет
                    byte[] packetData = pStream.ToArray();

                    //отправка
                    await udpClient.SendAsync(packetData, packetData.Length);
                }
            }
        }

        private byte[] ImageToBytes(Bitmap screenshot, long jpegQuality = 80L)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                ImageCodecInfo jpjEncoder = GetEncoder(ImageFormat.Jpeg);
                EncoderParameters encoderParam = new EncoderParameters(1);
                encoderParam.Param[0] = new EncoderParameter(Encoder.Quality, jpegQuality);

                screenshot.Save(stream, jpjEncoder, encoderParam);
                return stream.ToArray();
            }
        }

        private ImageCodecInfo? GetEncoder(ImageFormat jpeg)
        {
            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageDecoders();
            foreach (ImageCodecInfo codec in codecs)
            {
                if (codec.FormatID == jpeg.Guid)
                {
                    return codec;
                }
            }
            return null;
        }

        private Bitmap CaptureScreen()
        {
            var bounds = System.Windows.Forms.Screen.PrimaryScreen.Bounds;
            Bitmap screenshot = new Bitmap(bounds.Width, bounds.Height, PixelFormat.Format32bppArgb);
            using (Graphics g = Graphics.FromImage(screenshot))
            {
                g.CopyFromScreen(bounds.X, bounds.Y, 0, 0, bounds.Size, CopyPixelOperation.SourceCopy);
            }
            return screenshot;
        }

        public void StopSharing()
        {
            StopShareToken?.Cancel();
            Log("Stop share.");
        }

        public void Dispose()
        {
            StopSharing();
            udpClient?.Close();
            StopShareToken?.Dispose();
        }
    }
}

