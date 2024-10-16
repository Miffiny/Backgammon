namespace UI.ViewModels;
using System.ComponentModel;
using Core;
public class DiceViewModel : INotifyPropertyChanged
{
    private Dice _coreDice;  // Reference to the Core Dice class

    private int _value1;
    private int _value2;
    private bool _isDouble;

    public int Value1
    {
        get { return _value1; }
        set
        {
            _value1 = value;
            OnPropertyChanged(nameof(Value1));
        }
    }

    public int Value2
    {
        get { return _value2; }
        set
        {
            _value2 = value;
            OnPropertyChanged(nameof(Value2));
        }
    }

    public bool IsDouble
    {
        get { return _isDouble; }
        set
        {
            _isDouble = value;
            OnPropertyChanged(nameof(IsDouble));
        }
    }

    // Constructor that takes the Core Dice object as a parameter
    public DiceViewModel(Dice coreDice)
    {
        _coreDice = coreDice;
        // Initialize ViewModel with current state of the Core dice
        UpdateValuesFromCore();
    }

    // This method is called when rolling the dice from the UI
    public void Roll()
    {
        // Call the Core logic to roll the dice
        _coreDice.Roll();
        // Update the ViewModel properties based on the new state of the Core dice
        UpdateValuesFromCore();
    }

    private void UpdateValuesFromCore()
    {
        // Update ViewModel properties from the Core Dice object
        Value1 = _coreDice.Value1;
        Value2 = _coreDice.Value2;
        IsDouble = _coreDice.IsDouble;
    }

    public event PropertyChangedEventHandler PropertyChanged;
    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}

