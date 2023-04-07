using System.ComponentModel;
using System.Runtime.CompilerServices;
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

        public MainWindowViewModel()
        {
            
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
    }
}
