using System;
using System.Drawing;
using System.IO;
using System.Windows.Media.Imaging;

namespace MangaPicMerger.Helpers
{
    public static class ImageHelper
    {
        public static Bitmap BitmapImage2Bitmap(BitmapImage bitmapImage)
        {
            using (MemoryStream outStream = new MemoryStream())
            {
                BitmapEncoder enc = new BmpBitmapEncoder();
                enc.Frames.Add(BitmapFrame.Create(bitmapImage));
                enc.Save(outStream);

                return new Bitmap(outStream);
            }
        }

        public static BitmapImage LoadBitmapImage(string filePath)
        {
            var image = new BitmapImage();
            image.BeginInit();
            image.UriSource = new Uri(filePath);
            image.EndInit();
            return image;
        }
    }
}
