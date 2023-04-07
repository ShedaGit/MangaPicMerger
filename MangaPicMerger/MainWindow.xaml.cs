using MangaPicMerger.ViewModels;
using System;
using System.Windows;
using System.Windows.Media.Imaging;

namespace MangaPicMerger
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainWindowViewModel();
                    }

        private void OnWindowDrop(object sender, DragEventArgs e)
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
