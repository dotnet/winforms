// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Drawing;
using System.Globalization;

namespace System.Windows.Forms;

/// <summary>
///  Represents a Windows up-down control that displays numeric values.
/// </summary>
[DefaultProperty(nameof(Value))]
[DefaultEvent(nameof(ValueChanged))]
[DefaultBindingProperty(nameof(Value))]
[SRDescription(nameof(SR.DescriptionNumericUpDown))]
public partial class NumericUpDown : UpDownBase, ISupportInitialize
{
    private const decimal DefaultValue = decimal.Zero;
    private const decimal DefaultMinimum = decimal.Zero;
    private const decimal DefaultMaximum = (decimal)100.0;
    private const int DefaultDecimalPlaces = 0;
    private const decimal DefaultIncrement = decimal.One;
    private const bool DefaultThousandsSeparator = false;
    private const bool DefaultHexadecimal = false;
    private const int InvalidValue = -1;

    //////////////////////////////////////////////////////////////
    // Member variables
    //
    //////////////////////////////////////////////////////////////
    /// <summary>
    ///  The number of decimal places to display.
    /// </summary>
    private int _decimalPlaces = DefaultDecimalPlaces;

    /// <summary>
    ///  The amount to increment by.
    /// </summary>
    private decimal _increment = DefaultIncrement;

    // Display the thousands separator?
    private bool _thousandsSeparator = DefaultThousandsSeparator;

    // Minimum and maximum values
    private decimal _minimum = DefaultMinimum;
    private decimal _maximum = DefaultMaximum;

    // Hexadecimal
    private bool _hexadecimal = DefaultHexadecimal;

    // Internal storage of the current value
    private decimal _currentValue = DefaultValue;
    private bool _currentValueChanged;

    // Event handler for the onValueChanged event
    private EventHandler? _onValueChanged;

    // Disable value range checking while initializing the control
    private bool _initializing;

    // Provides for finer acceleration behavior.
    private NumericUpDownAccelerationCollection? _accelerations;

    // the current NumericUpDownAcceleration object.
    private int _accelerationsCurrentIndex;

    // Used to calculate the time elapsed since the up/down button was pressed,
    // to know when to get the next entry in the acceleration table.
    private long _buttonPressedStartTime;

    public NumericUpDown() : base()
    {
        // this class overrides GetPreferredSizeCore, let Control automatically cache the result
        SetExtendedState(ExtendedStates.UserPreferredSizeCache, true);
        Text = "0";
        StopAcceleration();
    }

    /// <summary>
    ///  Specifies the acceleration information.
    /// </summary>
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public NumericUpDownAccelerationCollection Accelerations
    {
        get
        {
            _accelerations ??= [];

            return _accelerations;
        }
    }

    /// <summary>
    ///  Gets or sets the number of decimal places to display in the up-down control.
    /// </summary>
    [SRCategory(nameof(SR.CatData))]
    [DefaultValue(DefaultDecimalPlaces)]
    [SRDescription(nameof(SR.NumericUpDownDecimalPlacesDescr))]
    public int DecimalPlaces
    {
        get
        {
            return _decimalPlaces;
        }

        set
        {
            ArgumentOutOfRangeException.ThrowIfNegative(value);
            ArgumentOutOfRangeException.ThrowIfGreaterThan(value, 99);

            _decimalPlaces = value;
            UpdateEditText();
        }
    }

    /// <summary>
    ///  Gets or
    ///  sets a value indicating whether the up-down control should
    ///  display the value it contains in hexadecimal format.
    /// </summary>
    [SRCategory(nameof(SR.CatAppearance))]
    [DefaultValue(DefaultHexadecimal)]
    [SRDescription(nameof(SR.NumericUpDownHexadecimalDescr))]
    public bool Hexadecimal
    {
        get
        {
            return _hexadecimal;
        }

        set
        {
            _hexadecimal = value;
            UpdateEditText();
        }
    }

    /// <summary>
    ///  Gets or sets the value
    ///  to increment or
    ///  decrement the up-down control when the up or down buttons are clicked.
    /// </summary>
    [SRCategory(nameof(SR.CatData))]
    [SRDescription(nameof(SR.NumericUpDownIncrementDescr))]
    public decimal Increment
    {
        get
        {
            if (_accelerationsCurrentIndex != InvalidValue)
            {
                return Accelerations[_accelerationsCurrentIndex].Increment;
            }

            return _increment;
        }

        set
        {
            ArgumentOutOfRangeException.ThrowIfNegative(value);

            _increment = value;
        }
    }

    /// <summary>
    ///  Gets or sets the maximum value for the up-down control.
    /// </summary>
    [SRCategory(nameof(SR.CatData))]
    [RefreshProperties(RefreshProperties.All)]
    [SRDescription(nameof(SR.NumericUpDownMaximumDescr))]
    public decimal Maximum
    {
        get
        {
            return _maximum;
        }

        set
        {
            _maximum = value;
            if (_minimum > _maximum)
            {
                _minimum = _maximum;
            }

            Value = Constrain(_currentValue);

            Debug.Assert(_maximum == value, "Maximum != what we just set it to!");
        }
    }

    /// <summary>
    ///  Gets or sets the minimum allowed value for the up-down control.
    /// </summary>
    [SRCategory(nameof(SR.CatData))]
    [RefreshProperties(RefreshProperties.All)]
    [SRDescription(nameof(SR.NumericUpDownMinimumDescr))]
    public decimal Minimum
    {
        get
        {
            return _minimum;
        }

        set
        {
            _minimum = value;
            if (_minimum > _maximum)
            {
                _maximum = value;
            }

            Value = Constrain(_currentValue);

            Debug.Assert(_minimum.Equals(value), "Minimum != what we just set it to!");
        }
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public new Padding Padding
    {
        get => base.Padding;
        set => base.Padding = value;
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new event EventHandler? PaddingChanged
    {
        add => base.PaddingChanged += value;
        remove => base.PaddingChanged -= value;
    }

    /// <summary>
    ///  Determines whether the UpDownButtons have been pressed for enough time to activate acceleration.
    /// </summary>
    [MemberNotNullWhen(true, nameof(_accelerations))]
    private bool Spinning
    {
        get
        {
            return _accelerations is not null && _buttonPressedStartTime != InvalidValue;
        }
    }

    /// <summary>
    ///  The text displayed in the control.
    /// </summary>
    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    [Bindable(false)]
    [AllowNull]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    // We're just overriding this to make it non-browsable.
    public override string Text
    {
        get => base.Text;
        set => base.Text = value;
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new event EventHandler? TextChanged
    {
        add => base.TextChanged += value;
        remove => base.TextChanged -= value;
    }

    /// <summary>
    ///  Gets or sets a value indicating whether a thousands
    ///  separator is displayed in the up-down control when appropriate.
    /// </summary>
    [SRCategory(nameof(SR.CatData))]
    [DefaultValue(DefaultThousandsSeparator)]
    [Localizable(true)]
    [SRDescription(nameof(SR.NumericUpDownThousandsSeparatorDescr))]
    public bool ThousandsSeparator
    {
        get
        {
            return _thousandsSeparator;
        }

        set
        {
            _thousandsSeparator = value;
            UpdateEditText();
        }
    }

    /// <summary>
    ///  Gets or sets the value
    ///  assigned to the up-down control.
    /// </summary>
    [SRCategory(nameof(SR.CatAppearance))]
    [Bindable(true)]
    [SRDescription(nameof(SR.NumericUpDownValueDescr))]
    public decimal Value
    {
        get
        {
            if (UserEdit)
            {
                ValidateEditText();
            }

            return _currentValue;
        }

        set
        {
            if (value != _currentValue)
            {
                if (!_initializing && ((value < _minimum) || (value > _maximum)))
                {
                    throw new ArgumentOutOfRangeException(nameof(value), value, string.Format(SR.InvalidBoundArgument, nameof(Value), value, $"'{nameof(Minimum)}'", $"'{nameof(Maximum)}'"));
                }
                else
                {
                    _currentValue = value;

                    OnValueChanged(EventArgs.Empty);
                    _currentValueChanged = true;
                    UpdateEditText();
                }
            }
        }
    }

    /// <summary>
    ///  Occurs when the <see cref="Value"/> property has been changed in some way.
    /// </summary>
    [SRCategory(nameof(SR.CatAction))]
    [SRDescription(nameof(SR.NumericUpDownOnValueChangedDescr))]
    public event EventHandler? ValueChanged
    {
        add => _onValueChanged += value;
        remove => _onValueChanged -= value;
    }

    /// <summary>
    ///  Handles tasks required when the control is being initialized.
    /// </summary>
    public void BeginInit()
    {
        _initializing = true;
    }

    //
    // Returns the provided value constrained to be within the min and max.
    //
    private decimal Constrain(decimal value)
    {
        Debug.Assert(_minimum <= _maximum,
                     "minimum > maximum");

        if (value < _minimum)
        {
            value = _minimum;
        }

        if (value > _maximum)
        {
            value = _maximum;
        }

        return value;
    }

    /// <summary>
    ///  Decrements the value of the up-down control.
    /// </summary>
    public override void DownButton()
    {
        SetNextAcceleration();

        if (UserEdit)
        {
            ParseEditText();
        }

        decimal newValue = _currentValue;

        // Operations on Decimals can throw OverflowException.
        try
        {
            newValue -= Increment;

            if (newValue < _minimum)
            {
                newValue = _minimum;
                if (Spinning)
                {
                    StopAcceleration();
                }
            }
        }
        catch (OverflowException)
        {
            newValue = _minimum;
        }

        Value = newValue;
    }

    /// <summary>
    ///  Called when initialization of the control is complete.
    /// </summary>
    public void EndInit()
    {
        _initializing = false;
        Value = Constrain(_currentValue);
        UpdateEditText();
    }

    /// <summary>
    ///  Overridden to set/reset acceleration variables.
    /// </summary>
    protected override void OnKeyDown(KeyEventArgs e)
    {
        if (InterceptArrowKeys && (e.KeyCode == Keys.Up || e.KeyCode == Keys.Down) && !Spinning)
        {
            StartAcceleration();
        }

        base.OnKeyDown(e);
    }

    /// <summary>
    ///  Overridden to set/reset acceleration variables.
    /// </summary>
    protected override void OnKeyUp(KeyEventArgs e)
    {
        if (InterceptArrowKeys && (e.KeyCode == Keys.Up || e.KeyCode == Keys.Down))
        {
            StopAcceleration();
        }

        base.OnKeyUp(e);
    }

    /// <summary>
    ///  Restricts the entry of characters to digits (including hex), the negative sign,
    ///  the decimal point, and editing keystrokes (backspace).
    /// </summary>
    protected override void OnTextBoxKeyPress(object? source, KeyPressEventArgs e)
    {
        base.OnTextBoxKeyPress(source, e);

        NumberFormatInfo numberFormatInfo = CultureInfo.CurrentCulture.NumberFormat;
        string decimalSeparator = numberFormatInfo.NumberDecimalSeparator;
        string groupSeparator = numberFormatInfo.NumberGroupSeparator;
        string negativeSign = numberFormatInfo.NegativeSign;

        string keyInput = e.KeyChar.ToString();

        if (char.IsDigit(e.KeyChar))
        {
            // Digits are OK
        }
        else if (keyInput.Equals(decimalSeparator) || keyInput.Equals(groupSeparator) || keyInput.Equals(negativeSign))
        {
            // Decimal separator is OK
        }
        else if (e.KeyChar == '\b')
        {
            // Backspace key is OK
        }
        else if (Hexadecimal && ((e.KeyChar >= 'a' && e.KeyChar <= 'f') || (e.KeyChar >= 'A' && e.KeyChar <= 'F')))
        {
            // Hexadecimal digits are OK
        }
        else if ((ModifierKeys & (Keys.Control | Keys.Alt)) != 0)
        {
            // Let the edit control handle control and alt key combinations
        }
        else
        {
            // Eat this invalid key and beep
            e.Handled = true;
            PInvoke.MessageBeep(MESSAGEBOX_STYLE.MB_OK);
        }
    }

    /// <summary>
    ///  Raises the <see cref="OnValueChanged"/> event.
    /// </summary>
    protected virtual void OnValueChanged(EventArgs e)
    {
        // Call the event handler
        _onValueChanged?.Invoke(this, e);
    }

    protected override void OnLostFocus(EventArgs e)
    {
        base.OnLostFocus(e);
        if (UserEdit)
        {
            UpdateEditText();
        }
    }

    /// <summary>
    ///  Overridden to start/end acceleration.
    /// </summary>
    internal override void OnStartTimer()
    {
        StartAcceleration();
    }

    /// <summary>
    ///  Overridden to start/end acceleration.
    /// </summary>
    internal override void OnStopTimer()
    {
        StopAcceleration();
    }

    /// <summary>
    ///  Converts the text displayed in the up-down control to a
    ///  numeric value and evaluates it.
    /// </summary>
    protected void ParseEditText()
    {
        Debug.Assert(UserEdit, "ParseEditText() - UserEdit == false");

        try
        {
            // Verify that the user is not starting the string with a "-"
            // before attempting to set the Value property since a "-" is a valid character with
            // which to start a string representing a negative number.
            if (!string.IsNullOrEmpty(Text) &&
                !(Text.Length == 1 && Text == "-"))
            {
                if (Hexadecimal)
                {
                    Value = Constrain(Convert.ToDecimal(Convert.ToInt32(Text, 16)));
                }
                else
                {
                    Value = Constrain(decimal.Parse(Text, CultureInfo.CurrentCulture));
                }
            }
        }
        catch
        {
            // Leave value as it is
        }
        finally
        {
            UserEdit = false;
        }
    }

    /// <summary>
    ///  Updates the index of the UpDownNumericAcceleration entry to use (if needed).
    /// </summary>
    private void SetNextAcceleration()
    {
        // Spinning will check if accelerations is null.
        if (Spinning && _accelerationsCurrentIndex < (_accelerations.Count - 1))
        {
            // if index not the last entry ...
            // Ticks are in 100-nanoseconds (1E-7 seconds).
            long nowTicks = DateTime.Now.Ticks;
            long buttonPressedElapsedTime = nowTicks - _buttonPressedStartTime;
            long accelerationInterval = 10000000L * _accelerations[_accelerationsCurrentIndex + 1].Seconds;  // next entry.

            // If Up/Down button pressed for more than the current acceleration entry interval, get next entry in the accel table.
            if (buttonPressedElapsedTime > accelerationInterval)
            {
                _buttonPressedStartTime = nowTicks;
                _accelerationsCurrentIndex++;
            }
        }
    }

    private void ResetIncrement()
    {
        Increment = DefaultIncrement;
    }

    private void ResetMaximum()
    {
        Maximum = DefaultMaximum;
    }

    private void ResetMinimum()
    {
        Minimum = DefaultMinimum;
    }

    private void ResetValue()
    {
        Value = DefaultValue;
    }

    /// <summary>
    ///  Indicates whether the <see cref="Increment"/> property should be
    ///  persisted.
    /// </summary>
    private bool ShouldSerializeIncrement()
    {
        return !Increment.Equals(DefaultIncrement);
    }

    /// <summary>
    ///  Indicates whether the <see cref="Maximum"/> property should be persisted.
    /// </summary>
    private bool ShouldSerializeMaximum()
    {
        return !Maximum.Equals(DefaultMaximum);
    }

    /// <summary>
    ///  Indicates whether the <see cref="Minimum"/> property should be persisted.
    /// </summary>
    private bool ShouldSerializeMinimum()
    {
        return !Minimum.Equals(DefaultMinimum);
    }

    /// <summary>
    ///  Indicates whether the <see cref="Value"/> property should be persisted.
    /// </summary>
    private bool ShouldSerializeValue()
    {
        return !Value.Equals(DefaultValue);
    }

    /// <summary>
    ///  Records when UpDownButtons are pressed to enable acceleration.
    /// </summary>
    private void StartAcceleration()
    {
        _buttonPressedStartTime = DateTime.Now.Ticks;
    }

    /// <summary>
    ///  Reset when UpDownButtons are pressed.
    /// </summary>
    private void StopAcceleration()
    {
        _accelerationsCurrentIndex = InvalidValue;
        _buttonPressedStartTime = InvalidValue;
    }

    internal override bool SupportsUiaProviders => true;

    /// <summary>
    ///  Provides some interesting info about this control in String form.
    /// </summary>
    public override string ToString()
    {
        string s = base.ToString();
        return $"{s}, Minimum = {Minimum}, Maximum = {Maximum}";
    }

    /// <summary>
    ///  Increments the value of the up-down control.
    /// </summary>
    public override void UpButton()
    {
        SetNextAcceleration();

        if (UserEdit)
        {
            ParseEditText();
        }

        decimal newValue = _currentValue;

        // Operations on Decimals can throw OverflowException.
        try
        {
            newValue += Increment;

            if (newValue > _maximum)
            {
                newValue = _maximum;
                if (Spinning)
                {
                    StopAcceleration();
                }
            }
        }
        catch (OverflowException)
        {
            newValue = _maximum;
        }

        Value = newValue;
    }

    private string GetNumberText(decimal num)
    {
        string text;

        if (Hexadecimal)
        {
            text = ((long)num).ToString("X", CultureInfo.InvariantCulture);
            Debug.Assert(text == text.ToUpper(CultureInfo.InvariantCulture), "GetPreferredSize assumes hex digits to be uppercase.");
        }
        else
        {
            text = num.ToString($"{(ThousandsSeparator ? "N" : "F")}{DecimalPlaces}", CultureInfo.CurrentCulture);
        }

        return text;
    }

    /// <summary>
    ///  Displays the current value of the up-down control in the appropriate format.
    /// </summary>
    protected override void UpdateEditText()
    {
        // If we're initializing, we don't want to update the edit text yet,
        // just in case the value is invalid.
        if (_initializing)
        {
            return;
        }

        // If the current value is user-edited, then parse this value before reformatting
        if (UserEdit)
        {
            ParseEditText();
        }

        // Verify that the user is not starting the string with a "-"
        // before attempting to set the Value property since a "-" is a valid character with
        // which to start a string representing a negative number.
        if (_currentValueChanged || (!string.IsNullOrEmpty(Text) &&
            !(Text.Length == 1 && Text == "-")))
        {
            _currentValueChanged = false;
            ChangingText = true;

            // Make sure the current value is within the min/max
            Debug.Assert(_minimum <= _currentValue && _currentValue <= _maximum,
                         "DecimalValue lies outside of [minimum, maximum]");

            Text = GetNumberText(_currentValue);
            Debug.Assert(!ChangingText, "ChangingText should have been set to false");
        }
    }

    /// <summary>
    ///  Validates and updates
    ///  the text displayed in the up-down control.
    /// </summary>
    protected override void ValidateEditText()
    {
        // See if the edit text parses to a valid decimal
        ParseEditText();
        UpdateEditText();
    }

    // This is not a breaking change -- Even though this control previously autosized to height,
    // it didn't actually have an AutoSize property. The new AutoSize property enables the
    // smarter behavior.
    internal override Size GetPreferredSizeCore(Size proposedConstraints)
    {
        int height = PreferredHeight;

        int baseSize = Hexadecimal ? 16 : 10;
        int digit = GetLargestDigit(0, baseSize);
        // The floor of log is intentionally 1 less than the number of digits. We initialize
        // testNumber to account for the missing digit.
        int numDigits = (int)Math.Floor(Math.Log(Math.Max(-(double)Minimum, (double)Maximum), baseSize));
        int maxDigits;
        if (Hexadecimal)
        {
            maxDigits = (int)Math.Floor(Math.Log(long.MaxValue, baseSize));
        }
        else
        {
            maxDigits = (int)Math.Floor(Math.Log((double)decimal.MaxValue, baseSize));
        }

        bool maxDigitsReached = numDigits >= maxDigits;
        decimal testNumber;

        // preinitialize testNumber with the leading digit
        if (digit != 0 || numDigits == 1)
        {
            testNumber = digit;
        }
        else
        {
            // zero can not be the leading digit if we need more than
            // one digit. (0*baseSize = 0 in the loop below)
            testNumber = GetLargestDigit(1, baseSize);
        }

        if (maxDigitsReached)
        {
            // Prevent

            numDigits = maxDigits - 1;
        }

        // e.g., if the largest digit is 7, and we can have 3 digits, the widest string would be "777"
        for (int i = 0; i < numDigits; i++)
        {
            testNumber = testNumber * baseSize + digit;
        }

        int textWidth = TextRenderer.MeasureText(GetNumberText(testNumber), Font).Width;

        if (maxDigitsReached)
        {
            string shortText = Hexadecimal
                ? ((long)testNumber).ToString("X", CultureInfo.InvariantCulture)
                : testNumber.ToString(CultureInfo.CurrentCulture);

            int shortTextWidth = TextRenderer.MeasureText(shortText, Font).Width;

            // Adding the width of the one digit that was dropped earlier.
            // This assumes that no additional thousand separator is added by that digit which is correct.
            textWidth += shortTextWidth / (numDigits + 1);
        }

        // Call AdjustWindowRect to add space for the borders
        int width = SizeFromClientSizeInternal(new(textWidth, height)).Width + _upDownButtons.Width;
        return new Size(width, height) + Padding.Size;
    }

    private int GetLargestDigit(int start, int end)
    {
        int largestDigit = -1;
        int digitWidth = -1;

        Span<char> charSpan = stackalloc char[1];
        for (int i = start; i < end; i++)
        {
            if (i < 10)
            {
                i.TryFormat(charSpan, out _);
            }
            else
            {
                charSpan[0] = (char)('A' + (i - 10));
            }

            Size digitSize = TextRenderer.MeasureText(charSpan, Font);

            if (digitSize.Width >= digitWidth)
            {
                digitWidth = digitSize.Width;
                largestDigit = i;
            }
        }

        Debug.Assert(largestDigit != -1 && digitWidth != -1, "Failed to find largest digit.");
        return largestDigit;
    }
}
