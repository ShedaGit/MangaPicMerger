using Microsoft.Win32;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
                        var imageLeft = new BitmapImage();
                        imageLeft.BeginInit();
                        imageLeft.UriSource = new Uri(dlg.FileNames[0]);
                        imageLeft.EndInit();
                        ImageLeft = imageLeft;

                        var imageRight = new BitmapImage();
                        imageRight.BeginInit();
                        imageRight.UriSource = new Uri(dlg.FileNames[1]);
                        imageRight.EndInit();
                        ImageRight = imageRight;
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
            throw new NotImplementedException();
        }
    }
}
