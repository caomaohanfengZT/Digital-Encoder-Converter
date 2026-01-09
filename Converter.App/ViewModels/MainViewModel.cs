using System.Collections.ObjectModel;
using Converter.App.Models;
using Converter.App.Services;

namespace Converter.App.ViewModels;

public sealed class MainViewModel : ObservableObject
{
    private readonly EncodingConverter _converter = new();
    private string _inputText = string.Empty;
    private InputTypeOption? _selectedInputType;
    private string _originalBinary = string.Empty;
    private string _originalHex = string.Empty;
    private string _onesBinary = string.Empty;
    private string _onesHex = string.Empty;
    private string _twosBinary = string.Empty;
    private string _twosHex = string.Empty;
    private string _decimalValue = string.Empty;
    private string _errorMessage = string.Empty;
    private bool _hasError;

    public MainViewModel()
    {
        InputTypes = new ObservableCollection<InputTypeOption>
        {
            new("原码（2 进制）", InputType.OriginalBinary),
            new("原码（10 进制）", InputType.OriginalDecimal),
            new("原码（16 进制）", InputType.OriginalHex),
            new("反码（2 进制）", InputType.OnesBinary),
            new("反码（16 进制）", InputType.OnesHex),
            new("补码（2 进制）", InputType.TwosBinary),
            new("补码（16 进制）", InputType.TwosHex)
        };

        SelectedInputType = InputTypes[0];
    }

    public ObservableCollection<InputTypeOption> InputTypes { get; }

    public string InputText
    {
        get => _inputText;
        set
        {
            if (SetProperty(ref _inputText, value))
            {
                UpdateOutputs();
            }
        }
    }

    public InputTypeOption? SelectedInputType
    {
        get => _selectedInputType;
        set
        {
            if (SetProperty(ref _selectedInputType, value))
            {
                UpdateOutputs();
            }
        }
    }

    public string OriginalBinary
    {
        get => _originalBinary;
        private set => SetProperty(ref _originalBinary, value);
    }

    public string DecimalValue
    {
        get => _decimalValue;
        private set => SetProperty(ref _decimalValue, value);
    }

    public string OriginalHex
    {
        get => _originalHex;
        private set => SetProperty(ref _originalHex, value);
    }

    public string OnesBinary
    {
        get => _onesBinary;
        private set => SetProperty(ref _onesBinary, value);
    }

    public string OnesHex
    {
        get => _onesHex;
        private set => SetProperty(ref _onesHex, value);
    }

    public string TwosBinary
    {
        get => _twosBinary;
        private set => SetProperty(ref _twosBinary, value);
    }

    public string TwosHex
    {
        get => _twosHex;
        private set => SetProperty(ref _twosHex, value);
    }

    public string ErrorMessage
    {
        get => _errorMessage;
        private set => SetProperty(ref _errorMessage, value);
    }

    public bool HasError
    {
        get => _hasError;
        private set => SetProperty(ref _hasError, value);
    }

    private void UpdateOutputs()
    {
        if (SelectedInputType is null)
        {
            return;
        }

        ConversionResult result = _converter.Convert(InputText, SelectedInputType.Type);
        HasError = result.HasError;
        ErrorMessage = result.ErrorMessage;
        DecimalValue = result.DecimalValue;
        OriginalBinary = result.OriginalBinary;
        OriginalHex = result.OriginalHex;
        OnesBinary = result.OnesBinary;
        OnesHex = result.OnesHex;
        TwosBinary = result.TwosBinary;
        TwosHex = result.TwosHex;
    }
}
