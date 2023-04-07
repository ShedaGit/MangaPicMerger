using MangaPicMerger.Helpers;
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

                try
                {
                    if (files.Length == 1)
                    {
                        var position = e.GetPosition(this);

                        // If we need to load only in the picture field,
                        // we can use this to find the center of the element and,
                        // based on the actual width and height of the element,
                        // find its position in the application.
                        //
                        // var leftImageCenter = ImageViewerLeft.TransformToVisual(this).Transform(new Point(0, 0));
                        // var rightImageCenter = ImageViewerRight.TransformToVisual(this).Transform(new Point(0, 0));

                        if (position.X < this.ActualWidth / 2 && position.Y < this.ActualHeight)
                        {
                            ImageViewerLeft.Source = ImageHelper.LoadBitmapImage(files[0]);
                        }
                        else if (position.X > this.ActualWidth / 2 && position.Y < this.ActualHeight)
                        {
                            ImageViewerRight.Source = ImageHelper.LoadBitmapImage(files[0]);
                        }
                    }
                    else if (files.Length == 2)
                    {
                        ImageViewerLeft.Source = ImageHelper.LoadBitmapImage(files[0]);
                        ImageViewerRight.Source = ImageHelper.LoadBitmapImage(files[1]);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error occurred: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}
