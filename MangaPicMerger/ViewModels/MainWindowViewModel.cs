using MangaPicMerger.Helpers;
using Microsoft.Win32;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing.Imaging;
using System.Drawing;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace MangaPicMerger.ViewModels
{
    class MainWindowViewModel : INotifyPropertyChanged
    {
        private BitmapImage _imageLeft;
        private BitmapImage _imageRight;
        private int _selectedBarIndex;
        private int _barSize;
        private string _mergedImageName;

        private readonly ReadOnlyObservableCollection<string> _barBetweenImagesOptions = new ReadOnlyObservableCollection<string>(new ObservableCollection<string> { "None", "White", "Black" });
        public ReadOnlyObservableCollection<string> BarBetweenImagesOptions => _barBetweenImagesOptions;

        private string _selectedBarBetweenImagesOption;

        public string SelectedBarBetweenImagesOption
        {
            get => _selectedBarBetweenImagesOption;
            set
            {
                _selectedBarBetweenImagesOption = value;
                OnPropertyChanged(nameof(SelectedBarBetweenImagesOption));

                if (SelectedBarBetweenImagesOption == "None")
                {
                    BarBetweenImagesVisibility = Visibility.Collapsed;
                }
                else
                {
                    BarBetweenImagesVisibility = Visibility.Visible;
                }
            }
        }

        private Visibility _barBetweenImagesVisibility = Visibility.Collapsed;
        public Visibility BarBetweenImagesVisibility
        {
            get => _barBetweenImagesVisibility;
            set
            {
                _barBetweenImagesVisibility = value;
                OnPropertyChanged(nameof(BarBetweenImagesVisibility));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
            }
        }

        public ICommand BrowseCommand { get; private set; }
        public ICommand SwitchCommand { get; private set; }
        public ICommand MergeCommand { get; private set; }

        public MainWindowViewModel()
        {
            BrowseCommand = new RelayCommand(Browse);
            SwitchCommand = new RelayCommand(Switch);
            MergeCommand = new RelayCommand(Merge);
        }

        public BitmapImage ImageLeft
        {
            get => _imageLeft;
            set
            {
                _imageLeft = value;
                OnPropertyChanged(nameof(ImageLeft));
            }
        }

        public BitmapImage ImageRight
        {
            get => _imageRight;
            set
            {
                _imageRight = value;
                OnPropertyChanged(nameof(ImageRight));
            }
        }

        public int SelectedBarIndex
        {
            get => _selectedBarIndex;
            set
            {
                _selectedBarIndex = value;
                OnPropertyChanged(nameof(SelectedBarIndex));
            }
        }

        public int BarSize
        {
            get => _barSize;
            set
            {
                _barSize = value;
                OnPropertyChanged(nameof(BarSize));
            }
        }

        public string MergedImageName
        {
            get => _mergedImageName;
            set
            {
                _mergedImageName = value;
                OnPropertyChanged(nameof(MergedImageName));
            }
        }

        private void Browse(object obj)
        {
            try
            {
                OpenFileDialog dlg = new OpenFileDialog();
                dlg.Filter = "PNG Files (*.png)|*.png|JPEG Files (*.jpg)|*.jpg|All Files (*.*)|*.*";
                dlg.RestoreDirectory = true;
                dlg.Multiselect = true;

                if (dlg.ShowDialog() == true)
                {
                    if (dlg.FileNames.Length == 2)
                    {
                        ImageLeft = ImageHelper.LoadBitmapImage(dlg.FileNames[0]);
                        ImageRight = ImageHelper.LoadBitmapImage(dlg.FileNames[1]);
                    }
                    else
                    {
                        MessageBox.Show("You should choose 2 images.", "Warning!", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                if (ex is InvalidOperationException || ex is IOException)
                {
                    MessageBox.Show("Operation cancelled by user.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void Switch(object obj)
        {
            var bitmap = ImageRight;
            ImageRight = ImageLeft;
            ImageLeft = bitmap;
        }

        private void Merge(object obj)
        {
            if (ImageLeft == null | ImageRight == null)
            {
                MessageBox.Show("You should choose 2 images.", "Warning!", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            try
            {
                Bitmap imageLeft = ImageHelper.BitmapImage2Bitmap(ImageLeft);
                Bitmap imageRight = ImageHelper.BitmapImage2Bitmap(ImageLeft);

                SaveFileDialog sfd = new SaveFileDialog();
                sfd.Filter = "PNG Files (*.png)|*.png|JPEG Files (*.jpg)|*.jpg";
                sfd.FileName = string.IsNullOrWhiteSpace(MergedImageName) ? "picture" : MergedImageName;
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
                    if (BarBetweenImagesOptions.IndexOf(SelectedBarBetweenImagesOption) != 0)
                    {
                        int sizeOfLine = BarSize;

                        Bitmap line = new Bitmap(sizeOfLine, Math.Max(imageLeft.Height, imageRight.Height));

                        resultImage = new Bitmap(width + sizeOfLine, height);

                        if (BarBetweenImagesOptions.IndexOf(SelectedBarBetweenImagesOption) == 1) // White
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
                        else if (BarBetweenImagesOptions.IndexOf(SelectedBarBetweenImagesOption) == 2) // Black
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
            catch (Exception ex)
            {
                if (ex is InvalidOperationException || ex is IOException)
                {
                    MessageBox.Show("Operation cancelled by user.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}
