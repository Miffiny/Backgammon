using System.Collections.ObjectModel;
using System.ComponentModel;

namespace UI.ViewModels;

public class PointViewModel : INotifyPropertyChanged
{
    public int Index { get; set; }
    public ObservableCollection<CheckerViewModel> Checkers { get; set; }

    public PointViewModel(int index)
    {
        Index = index;
        Checkers = new ObservableCollection<CheckerViewModel>();
        // You can initialize checkers here based on game state
    }

    public void AddChecker(CheckerViewModel checker)
    {
        Checkers.Add(checker);
        OnPropertyChanged(nameof(Checkers));
    }

    public event PropertyChangedEventHandler PropertyChanged;
    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
