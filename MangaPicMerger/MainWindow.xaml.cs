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
            //dlg.InitialDirectory = "c:\\";
            dlg.Filter = "Images (*.jpg, *.jpeg, *.png)|*.jpg;*.jpeg;*.png" + "|All Files (*.*)|*.*";
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
                    //imageLeft.UriSource.ToString();
                    ImageViewerLeft.Source = imageLeft;

                    imageRight = new BitmapImage();
                    imageRight.BeginInit();
                    imageRight.UriSource = new Uri(dlg.FileNames[1]);
                    imageRight.EndInit();
                    //imageLeft.UriSource.ToString();
                    ImageViewerRight.Source = imageRight;
                }
                else
                {
                    MessageBox.Show("You should choose 2 images.", "Oops!", MessageBoxButton.OK, MessageBoxImage.Information);
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
                MessageBox.Show("You should choose 2 images.", "Oops!", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            Bitmap imageLeft = BitmapImage2Bitmap(this.imageLeft);
            Bitmap imageRight = BitmapImage2Bitmap(this.imageRight);

            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "Images (*.jpg, *.jpeg, *.png)|*.jpg;*.jpeg;*.png";
            ImageFormat format = ImageFormat.Png;
            sfd.FileName = "picture.png";
            if (sfd.ShowDialog() == true)
            {
                //File.WriteAllText(dlg.FileName, txtEditor.Text);

                int height = imageLeft.Height > imageRight.Height ? imageLeft.Height : imageRight.Height;
                int width = imageLeft.Width + imageRight.Width;

                Bitmap resultImage;
                //= new Bitmap(width, height);

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


                //string resultImageDirectory = imagesDirectory + textBox3.Text + ".png";

                string ext = System.IO.Path.GetExtension(sfd.FileName);
                switch (ext)
                {
                    case ".jpg":
                        format = ImageFormat.Jpeg;
                        break;
                    case ".jpeg":
                        format = ImageFormat.Bmp;
                        break;
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
                // Note that you can have more than one file.
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

                if (files.Length >= 2)
                {
                    imageLeft = new BitmapImage();
                    imageLeft.BeginInit();
                    imageLeft.UriSource = new Uri(files[0]);
                    imageLeft.EndInit();
                    //imageLeft.UriSource.ToString();
                    ImageViewerLeft.Source = imageLeft;

                    imageRight = new BitmapImage();
                    imageRight.BeginInit();
                    imageRight.UriSource = new Uri(files[1]);
                    imageRight.EndInit();
                    //imageLeft.UriSource.ToString();
                    ImageViewerRight.Source = imageRight;
                }
                else
                {
                    MessageBox.Show("You should choose 2 images.", "Oops!", MessageBoxButton.OK, MessageBoxImage.Information);
                }


                // Assuming you have one file that you care about, pass it off to whatever
                // handling code you have defined.

                //HandleFileOpen(files[0]);
            }
        }
    }
}
