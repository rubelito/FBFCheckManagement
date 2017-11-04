using System.Drawing;
using System.IO;
using System.Windows.Media.Imaging;
namespace FBFCheckManagement.WPF.HelperClass
{
    public class ImageToBitmap
    {
        public static BitmapImage ConvertToBitmapImage(Image img)
        {
            var bitmap = new BitmapImage();
            bitmap.BeginInit();
            var memoryStream = new MemoryStream();
            img.Save(memoryStream, img.RawFormat);
            memoryStream.Seek(0, SeekOrigin.Begin);
            bitmap.StreamSource = memoryStream;
            bitmap.EndInit();
            return bitmap;
        }
    }
}