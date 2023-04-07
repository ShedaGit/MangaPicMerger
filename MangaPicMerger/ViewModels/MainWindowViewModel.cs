using MangaPicMerger.Commands;
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
using System.Linq;

namespace MangaPicMerger.ViewModels
{
    class MainWindowViewModel : INotifyPropertyChanged
    {
        #region Properties
        private BitmapImage _imageLeft;
        public BitmapImage ImageLeft
        {
            get => _imageLeft;
            set
            {
                _imageLeft = value;
                OnPropertyChanged(nameof(ImageLeft));
            }
        }

        private BitmapImage _imageRight;
        public BitmapImage ImageRight
        {
            get => _imageRight;
            set
            {
                _imageRight = value;
                OnPropertyChanged(nameof(ImageRight));
            }
        }

        private int _selectedBarIndex;
        public int SelectedBarIndex
        {
            get => _selectedBarIndex;
            set
            {
                _selectedBarIndex = value;
                OnPropertyChanged(nameof(SelectedBarIndex));
            }
        }

        private int _barSize;
        public int BarSize
        {
            get => _barSize;
            set
            {
                _barSize = value;
                OnPropertyChanged(nameof(BarSize));
            }
        }

        private string _mergedImageName;
        public string MergedImageName
        {
            get => _mergedImageName;
            set
            {
                _mergedImageName = value;
                OnPropertyChanged(nameof(MergedImageName));
            }
        }

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

                BarBetweenImagesVisibility = SelectedBarBetweenImagesOption == "None" ? Visibility.Collapsed : Visibility.Visible;
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

        #endregion

        #region Commands

        public ICommand BrowseCommand { get; private set; }
        public ICommand SwitchCommand { get; private set; }
        public ICommand MergeCommand { get; private set; }

        #endregion

        #region Constructor

        public MainWindowViewModel()
        {
            BrowseCommand = new RelayCommand(Browse);
            SwitchCommand = new RelayCommand(Switch);
            MergeCommand = new RelayCommand(Merge);

            SelectedBarBetweenImagesOption = BarBetweenImagesOptions.FirstOrDefault();
        }

        #endregion

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }

        #endregion

        #region Command Methods

        #region Browse

        private void Browse(object obj)
        {
            try
            {
                OpenFileDialog dlg = new OpenFileDialog();
                dlg.Filter = "Image Files (*.jpg;*.jpeg;*.png;*.webp)|*.jpg;*.jpeg;*.png;*.webp|All Files (*.*)|*.*";
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

        #endregion

        #region Switch

        private void Switch(object obj)
        {
            var bitmapImage = ImageRight;
            ImageRight = ImageLeft;
            ImageLeft = bitmapImage;
        }

        #endregion

        #region Merge

        private void Merge(object obj)
        {
            if (ImageLeft == null | ImageRight == null)
            {
                MessageBox.Show("You should choose 2 images.", "Warning!", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            Bitmap resultImage = null;

            try
            {
                using (Bitmap imageLeft = ImageHelper.BitmapImageToBitmap(ImageLeft))
                using (Bitmap imageRight = ImageHelper.BitmapImageToBitmap(ImageRight))
                {

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
            finally
            {
                if (resultImage != null)
                {
                    resultImage.Dispose();
                    resultImage = null;
                }
            }
        }

        #endregion

        #endregion
    }
}
