namespace UI.ViewModels;
using System.Collections.ObjectModel;
using System.ComponentModel;

public class PlayerViewModel : INotifyPropertyChanged
{
    private string _color;
    public string Color
    {
        get { return _color; }
        set
        {
            _color = value;
            OnPropertyChanged(nameof(Color));
        }
    }

    public ObservableCollection<CheckerViewModel> Bar { get; set; }
    public ObservableCollection<CheckerViewModel> BearOff { get; set; }

    public PlayerViewModel()
    {
        Bar = new ObservableCollection<CheckerViewModel>();
        BearOff = new ObservableCollection<CheckerViewModel>();
    }

    public event PropertyChangedEventHandler PropertyChanged;
    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
