using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
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
            throw new NotImplementedException();
        }

        private void Switch(object obj)
        {
            throw new NotImplementedException();
        }

        private void Merge(object obj)
        {
            throw new NotImplementedException();
        }
    }
}
