using System;
using System.Drawing;
using System.IO;
using System.Windows.Media.Imaging;

namespace MangaPicMerger.Helpers
{
    public static class ImageHelper
    {
        public static Bitmap BitmapImageToBitmap(BitmapImage bitmapImage)
        {
            if (bitmapImage == null)
            {
                throw new ArgumentNullException(nameof(bitmapImage));
            }

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
            if (string.IsNullOrEmpty(filePath))
            {
                throw new ArgumentNullException(nameof(filePath));
            }

            try
            {
                var image = new BitmapImage();
                image.BeginInit();
                image.UriSource = new Uri(filePath);
                image.EndInit();
                return image;
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to load bitmap image from file '{filePath}'", ex);
            }
        }
    }
}
