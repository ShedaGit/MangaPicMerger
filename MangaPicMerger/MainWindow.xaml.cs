using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;
namespace MangaPicMerger
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private List<string> pathsToImages = new List<string>();
        private BitmapImage imageLeft;
        private BitmapImage imageRight;

        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = this;
        }

        private void BrowseButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "PNG Files (*.png)|*.png|JPEG Files (*.jpg)|*.jpg|All Files (*.*)|*.*";
            dlg.RestoreDirectory = true;
            dlg.Multiselect = true;

            if (dlg.ShowDialog() == true)
            {
                if (dlg.FileNames.Length > 1)
                {
                    imageLeft = new BitmapImage();
                    imageLeft.BeginInit();
                    imageLeft.UriSource = new Uri(dlg.FileNames[0]);
                    imageLeft.EndInit();
                    ImageViewerLeft.Source = imageLeft;

                    imageRight = new BitmapImage();
                    imageRight.BeginInit();
                    imageRight.UriSource = new Uri(dlg.FileNames[1]);
                    imageRight.EndInit();
                    ImageViewerRight.Source = imageRight;
                }
                else
                {
                    MessageBox.Show("You should choose 2 images.", "Warning!", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }

        private void SwitchButton_Click(object sender, RoutedEventArgs e)
        {
            BitmapImage bitmap = new BitmapImage();
            bitmap = imageRight;
            imageRight = imageLeft;
            imageLeft = bitmap;

            ImageViewerLeft.Source = imageLeft;
            ImageViewerRight.Source = imageRight;
        }

        private void MergeButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.imageLeft == null | this.imageRight == null)
            {
                MessageBox.Show("You should choose 2 images.", "Warning!", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            Bitmap imageLeft = BitmapImage2Bitmap(this.imageLeft);
            Bitmap imageRight = BitmapImage2Bitmap(this.imageRight);

            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "PNG Files (*.png)|*.png|JPEG Files (*.jpg)|*.jpg";
            sfd.FileName = "picture";
            if (sfd.ShowDialog() == true)
            {
                ImageFormat format = ImageFormat.Png;
                string ext = Path.GetExtension(sfd.FileName);

                switch (ext.ToLower())
                {
                    case ".jpg":
                        format = ImageFormat.Jpeg;
                        break;
                    case ".png":
                        format = ImageFormat.Png;
                        break;
                    default:
                        MessageBox.Show("Invalid format selected.", "Warning!", MessageBoxButton.OK, MessageBoxImage.Information);
                        return;
                }

                int height = imageLeft.Height > imageRight.Height ? imageLeft.Height : imageRight.Height;
                int width = imageLeft.Width + imageRight.Width;

                Bitmap resultImage;
                if (cbWhite.IsChecked == true || cbBlack.IsChecked == true)
                {
                    int sizeOfLine = int.Parse(tbSize.Text);

                    Bitmap line = new Bitmap(sizeOfLine, Math.Max(imageLeft.Height, imageRight.Height));

                    resultImage = new Bitmap(width + sizeOfLine, height);


                    if (cbWhite.IsChecked == true)
                    {
                        using (var g = Graphics.FromImage(line))
                        {
                            using (SolidBrush brush = new SolidBrush(System.Drawing.Color.White))
                            {
                                g.FillRectangle(brush, 0, 0, line.Width, line.Height);
                            }
                        }

                        using (var g = Graphics.FromImage(resultImage))
                        {
                            g.DrawImage(imageLeft, 0, 0, imageLeft.Width, imageLeft.Height);
                            g.DrawImage(line, imageLeft.Width, 0);
                            g.DrawImage(imageRight, imageLeft.Width + line.Width, 0, imageRight.Width, imageRight.Height);
                        }
                    }
                    else if (cbBlack.IsChecked == true)
                    {
                        using (var g = Graphics.FromImage(line))
                        {
                            using (SolidBrush brush = new SolidBrush(System.Drawing.Color.Black))
                            {
                                g.FillRectangle(brush, 0, 0, line.Width, line.Height);
                            }
                        }

                        using (var g = Graphics.FromImage(resultImage))
                        {
                            g.DrawImage(imageLeft, 0, 0, imageLeft.Width, imageLeft.Height);
                            g.DrawImage(line, imageLeft.Width, 0);
                            g.DrawImage(imageRight, imageLeft.Width + line.Width, 0, imageRight.Width, imageRight.Height);
                        }
                    }
                }
                else
                {
                    resultImage = new Bitmap(width, height);

                    using (var g = Graphics.FromImage(resultImage))
                    {
                        g.DrawImage(imageLeft, 0, 0, imageLeft.Width, imageLeft.Height);
                        g.DrawImage(imageRight, imageLeft.Width, 0, imageRight.Width, imageRight.Height);
                    }
                }

                resultImage.Save(sfd.FileName, format);
                MessageBox.Show("Done!");
            }
        }

        private Bitmap BitmapImage2Bitmap(BitmapImage bitmapImage)
        {
            // BitmapImage bitmapImage = new BitmapImage(new Uri("../Images/test.png", UriKind.Relative));

            using (MemoryStream outStream = new MemoryStream())
            {
                BitmapEncoder enc = new BmpBitmapEncoder();
                enc.Frames.Add(BitmapFrame.Create(bitmapImage));
                enc.Save(outStream);
                System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap(outStream);

                return new Bitmap(bitmap);
            }
        }

        private void Window_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

                if (files.Length == 1)
                {
                    var position = e.GetPosition(this);

                    if (position.X < ImageViewerLeft.ActualWidth && position.Y < ImageViewerLeft.ActualHeight)
                    {
                        imageLeft = new BitmapImage();
                        imageLeft.BeginInit();
                        imageLeft.UriSource = new Uri(files[0]);
                        imageLeft.EndInit();
                        ImageViewerLeft.Source = imageLeft;
                    }
                    else if (position.X > (this.ActualWidth - ImageViewerRight.ActualWidth) && position.Y < ImageViewerRight.ActualHeight)
                    {
                        imageRight = new BitmapImage();
                        imageRight.BeginInit();
                        imageRight.UriSource = new Uri(files[0]);
                        imageRight.EndInit();
                        ImageViewerRight.Source = imageRight;
                    }
                }
                else if (files.Length == 2)
                {
                    imageLeft = new BitmapImage();
                    imageLeft.BeginInit();
                    imageLeft.UriSource = new Uri(files[0]);
                    imageLeft.EndInit();
                    ImageViewerLeft.Source = imageLeft;

                    imageRight = new BitmapImage();
                    imageRight.BeginInit();
                    imageRight.UriSource = new Uri(files[1]);
                    imageRight.EndInit();
                    ImageViewerRight.Source = imageRight;
                }
            }
        }
    }
}
