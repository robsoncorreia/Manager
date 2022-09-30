using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Media.Imaging;

namespace QRCodeGenerator.Repository
{
    public interface IQRCodeRepository
    {
        void ExportImage(string text);

        BitmapImage GerarQRCode(int width, int height, string text);

        void Print(string text);
    }

    public class QRCodeRepository : IQRCodeRepository
    {
        public void Print(string text)
        {
            JpegBitmapEncoder encoder = new JpegBitmapEncoder();

            Guid photoID = Guid.NewGuid();

            string photolocation = $"{Path.GetTempPath()}/{photoID}.jpg";

            encoder.Frames.Add(BitmapFrame.Create(GerarQRCode(178, 178, text)));

            using (FileStream filestream = new FileStream(photolocation, FileMode.Create))
            {
                encoder.Save(filestream);
            }

            ProcessStartInfo info = new ProcessStartInfo
            {
                Verb = "print",
                FileName = photolocation,
                CreateNoWindow = true,
                WindowStyle = ProcessWindowStyle.Hidden
            };

            Process p = new Process
            {
                StartInfo = info
            };

            _ = p.Start();
        }

        public BitmapImage GerarQRCode(int width, int height, string text)
        {
            ZXing.BarcodeWriter bw = new ZXing.BarcodeWriter();
            ZXing.Common.EncodingOptions encOptions = new ZXing.Common.EncodingOptions() { Width = width, Height = height, Margin = 0 };
            bw.Options = encOptions;
            bw.Format = ZXing.BarcodeFormat.QR_CODE;
            Bitmap resultado = new Bitmap(bw.Write(text));
            return BitmapToImageSource(resultado);
        }

        public void ExportImage(string text)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog()
            {
                Filter = "Image Files ( *.jpg, *.bmp, *.png)|*.jpg ; *.bmp;*.png;",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                FileName = $"QRCode {DateTime.Now.Ticks}"
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                JpegBitmapEncoder encoder = new JpegBitmapEncoder();

                string photolocation = $"{saveFileDialog.FileName}";

                encoder.Frames.Add(BitmapFrame.Create(GerarQRCode(178, 178, text)));

                using FileStream filestream = new FileStream(photolocation, FileMode.Create);
                encoder.Save(filestream);
            }
        }

        private BitmapImage BitmapToImageSource(Bitmap bitmap)
        {
            using MemoryStream memory = new MemoryStream();
            bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Bmp);
            memory.Position = 0;
            BitmapImage bitmapimage = new BitmapImage();
            bitmapimage.BeginInit();
            bitmapimage.StreamSource = memory;
            bitmapimage.CacheOption = BitmapCacheOption.OnLoad;
            bitmapimage.EndInit();

            return bitmapimage;
        }
    }
}