using System.ComponentModel;
using Core;  // Reference the Core namespace

public class CheckerViewModel : INotifyPropertyChanged
{
    private int _position;
    private string _color;
    private double _xPosition;  // Absolute X-coordinate
    private double _yPosition;  // Absolute Y-coordinate
    public CheckerViewModel(Checker coreChecker)
    {
        _position = coreChecker.Position;
        _color = coreChecker.Color.ToString();
        XPosition = 100;  // Placeholder initial position
        YPosition = 100;
    }

    public int Position
    {
        get { return _position; }
        set
        {
            _position = value;
            OnPropertyChanged(nameof(Position));
        }
    }

    public string Color
    {
        get { return _color; }
        set
        {
            _color = value;
            OnPropertyChanged(nameof(Color));
        }
    }

    public double XPosition
    {
        get { return _xPosition; }
        set
        {
            _xPosition = value;
            OnPropertyChanged(nameof(XPosition));  // Notify UI when position changes
        }
    }

    public double YPosition
    {
        get { return _yPosition; }
        set
        {
            _yPosition = value;
            OnPropertyChanged(nameof(YPosition));  // Notify UI when position changes
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;
    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}