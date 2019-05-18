﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms {
    using Microsoft.Win32;
    using System;
    using System.ComponentModel;
    using System.ComponentModel.Design;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Drawing;
    using System.Windows.Forms.Internal;
    using System.Globalization;
    using System.Runtime.InteropServices;
    using System.Windows.Forms.Layout;

    /// <summary>
    ///    <para>Represents a Windows up-down control that displays numeric values.</para>
    /// </devdoc>
    [
    ComVisible(true),
    ClassInterface(ClassInterfaceType.AutoDispatch),
    DefaultProperty(nameof(Value)),
    DefaultEvent(nameof(ValueChanged)),
    DefaultBindingProperty(nameof(Value)),
    SRDescription(nameof(SR.DescriptionNumericUpDown))
    ]
    public class NumericUpDown : UpDownBase, ISupportInitialize {

        private static readonly decimal DefaultValue         = decimal.Zero;
        private static readonly decimal DefaultMinimum       = decimal.Zero;
        private static readonly decimal DefaultMaximum       = (decimal)100.0;
        private const           int        DefaultDecimalPlaces = 0;
        private static readonly decimal DefaultIncrement     = decimal.One;
        private const bool       DefaultThousandsSeparator      = false;
        private const bool       DefaultHexadecimal             = false;
        private const int        InvalidValue                   = -1;

        //////////////////////////////////////////////////////////////
        // Member variables
        //
        //////////////////////////////////////////////////////////////

        /// <summary>
        ///     The number of decimal places to display.
        /// </devdoc>
        private int decimalPlaces = DefaultDecimalPlaces;

        /// <summary>
        ///     The amount to increment by.
        /// </devdoc>
        private decimal increment = DefaultIncrement;

        // Display the thousands separator?
        private bool thousandsSeparator = DefaultThousandsSeparator;

        // Minimum and maximum values
        private decimal minimum = DefaultMinimum;
        private decimal maximum = DefaultMaximum;

        // Hexadecimal
        private bool hexadecimal = DefaultHexadecimal;

        // Internal storage of the current value
        private decimal currentValue = DefaultValue;
        private bool    currentValueChanged;

        // Event handler for the onValueChanged event
        private EventHandler onValueChanged = null;

        // Disable value range checking while initializing the control
        private bool initializing = false;

        // Provides for finer acceleration behavior.
        private NumericUpDownAccelerationCollection accelerations;

        // the current NumericUpDownAcceleration object.
        private int accelerationsCurrentIndex;

        // Used to calculate the time elapsed since the up/down button was pressed, 
        // to know when to get the next entry in the accelaration table.
        private long buttonPressedStartTime;

        [
            SuppressMessage("Microsoft.Globalization", "CA1303:DoNotPassLiteralsAsLocalizedParameters") // "0" is the default value for numeric up down.
                                                                                                        // So we don't have to localize it.
        ]
        public NumericUpDown() : base() {
            // this class overrides GetPreferredSizeCore, let Control automatically cache the result
            SetState2(STATE2_USEPREFERREDSIZECACHE, true);  
            Text = "0";
            StopAcceleration();
        }

        //////////////////////////////////////////////////////////////
        // Properties
        //
        //////////////////////////////////////////////////////////////

		
        /// <summary>
        ///     Specifies the acceleration information.
        /// </devdoc>
        [
        Browsable(false),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
        ]
        public NumericUpDownAccelerationCollection Accelerations {
            get  {
                if( this.accelerations == null ){
                    this.accelerations = new NumericUpDownAccelerationCollection();
                }
                return this.accelerations;
            }
        }

        /// <summary>
        ///    <para>Gets or sets the number of decimal places to display in the up-down control.</para>
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatData)),
        DefaultValue(NumericUpDown.DefaultDecimalPlaces),
        SRDescription(nameof(SR.NumericUpDownDecimalPlacesDescr))
        ]
        public int DecimalPlaces {

            get {
                return decimalPlaces;
            }

            set {
                if (value < 0 || value > 99) {
                    throw new ArgumentOutOfRangeException(nameof(value), value, string.Format(SR.InvalidBoundArgument, nameof(DecimalPlaces), value, 0, 99));
                }
                decimalPlaces = value;
                UpdateEditText();
            }
        }

        /// <summary>
        ///    <para>Gets or
        ///       sets a value indicating whether the up-down control should
        ///       display the value it contains in hexadecimal format.</para>
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatAppearance)),
        DefaultValue(NumericUpDown.DefaultHexadecimal),
        SRDescription(nameof(SR.NumericUpDownHexadecimalDescr))
        ]
        public bool Hexadecimal {

            get {
                return hexadecimal;
            }

            set {
                hexadecimal = value;
                UpdateEditText();
            }
        }

        /// <summary>
        ///    <para>Gets or sets the value
        ///       to increment or
        ///       decrement the up-down control when the up or down buttons are clicked.</para>
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatData)),
        SRDescription(nameof(SR.NumericUpDownIncrementDescr))
        ]
        public decimal Increment {

            get {
                if (this.accelerationsCurrentIndex != InvalidValue) {
                    return this.Accelerations[this.accelerationsCurrentIndex].Increment;
                }

                return this.increment;
            }

            set {
                if (value < 0.0m) {
                    throw new ArgumentOutOfRangeException(nameof(value), value, string.Format(SR.InvalidArgument, nameof(Increment), value));
                }
                else {
                    this.increment = value;
                }
            }
        }


        /// <summary>
        ///    <para>Gets or sets the maximum value for the up-down control.</para>
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatData)),
        RefreshProperties(RefreshProperties.All),
        SRDescription(nameof(SR.NumericUpDownMaximumDescr))
        ]
        public decimal Maximum {

            get {
                return maximum;
            }

            set {
                maximum = value;
                if (minimum > maximum) {
                    minimum = maximum;
                }
                
                Value = Constrain(currentValue);
                
                Debug.Assert(maximum == value, "Maximum != what we just set it to!");
            }
        }

        /// <summary>
        ///    <para>Gets or sets the minimum allowed value for the up-down control.</para>
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatData)),
        RefreshProperties(RefreshProperties.All),
        SRDescription(nameof(SR.NumericUpDownMinimumDescr))
        ]
        public decimal Minimum {

            get {
                return minimum;
            }

            set {
                minimum = value;
                if (minimum > maximum) {
                    maximum = value;
                }
                
                Value = Constrain(currentValue);

                Debug.Assert(minimum.Equals(value), "Minimum != what we just set it to!");
            }
        }

        [
        Browsable(false),
        EditorBrowsable(EditorBrowsableState.Never),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
        ]
        public new Padding Padding {
            get { return base.Padding; }
            set { base.Padding = value;}
        }

        [
        Browsable(false),
        EditorBrowsable(EditorBrowsableState.Never)
        ]
        new public event EventHandler PaddingChanged {
            add => base.PaddingChanged += value; 
            remove => base.PaddingChanged -= value; }

        /// <summary>
        ///     Determines whether the UpDownButtons have been pressed for enough time to activate acceleration.
        /// </devdoc>
        private bool Spinning {
            get{
                return this.accelerations != null && this.buttonPressedStartTime != InvalidValue;
            }
        }

        /// <summary>
        ///    <para>
        ///       The text displayed in the control.
        ///    </para>
        /// </devdoc>
        [
        Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
        Bindable(false), 
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)        
        ]
        // We're just overriding this to make it non-browsable.
        public override string Text {
            get {
                return base.Text;
            }
            set {
                base.Text = value;
            }
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        new public event EventHandler TextChanged {
            add => base.TextChanged += value;
            remove => base.TextChanged -= value;
        }
        
        /// <summary>
        ///    <para>Gets or sets a value indicating whether a thousands
        ///       separator is displayed in the up-down control when appropriate.</para>
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatData)),
        DefaultValue(NumericUpDown.DefaultThousandsSeparator),
        Localizable(true),
        SRDescription(nameof(SR.NumericUpDownThousandsSeparatorDescr))
        ]
        public bool ThousandsSeparator {

            get {
                return thousandsSeparator;
            }

            set {
                thousandsSeparator = value;
                UpdateEditText();
            }
        }

        /*
         * The current value of the control
         */
        /// <summary>
        ///    <para>Gets or sets the value
        ///       assigned to the up-down control.</para>
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatAppearance)),
        Bindable(true),
        SRDescription(nameof(SR.NumericUpDownValueDescr))
        ]
        public decimal Value {

            get {
                if (UserEdit) {
                    ValidateEditText();
                }
                return currentValue;
            }

            set {
                if (value != currentValue) {
                
                    if (!initializing && ((value < minimum) || (value > maximum))) {
                        throw new ArgumentOutOfRangeException(nameof(value), value, string.Format(SR.InvalidBoundArgument, nameof(Value), value, $"'{nameof(Minimum)}'", $"'{nameof(Maximum)}'"));
                    }
                    else {
                        currentValue = value;                       
                        
                        OnValueChanged(EventArgs.Empty);
                        currentValueChanged = true;    
                        UpdateEditText();
                    }
                }
            }
        }


        //////////////////////////////////////////////////////////////
        // Methods
        //
        //////////////////////////////////////////////////////////////

        /// <summary>
        ///    <para>
        ///       Occurs when the <see cref='System.Windows.Forms.NumericUpDown.Value'/> property has been changed in some way.
        ///    </para>
        /// </devdoc>
        [SRCategory(nameof(SR.CatAction)), SRDescription(nameof(SR.NumericUpDownOnValueChangedDescr))]
        public event EventHandler ValueChanged {
            add => onValueChanged += value;
            remove => onValueChanged -= value;
        }

        /// <summary>
        ///    Handles tasks required when the control is being initialized.
        /// </devdoc>
        public void BeginInit() {
            initializing = true;
        }

        //
        // Returns the provided value constrained to be within the min and max.
        //
        private decimal Constrain(decimal value) {

            Debug.Assert(minimum <= maximum,
                         "minimum > maximum");

            if (value < minimum) {
                value = minimum;
            }

            if (value > maximum) {
                value = maximum;
            }
            
            return value;
        }
        
        protected override AccessibleObject CreateAccessibilityInstance() {
            return new NumericUpDownAccessibleObject(this);
        }

        /// <summary>
        ///    <para>
        ///       Decrements the value of the up-down control.
        ///    </para>
        /// </devdoc>
        public override void DownButton() {
            SetNextAcceleration();

            if (UserEdit) {
                ParseEditText();
            }

            decimal newValue = currentValue;

            // Operations on Decimals can throw OverflowException.
            //
            try{
                newValue -= this.Increment;

                if (newValue < minimum){
                    newValue = minimum;
                    if( this.Spinning){
                        StopAcceleration();
                    }
                }
            }
            catch( OverflowException ){
                newValue = minimum;
            }
            
            Value = newValue;
        }

        /// <summary>
        ///    <para>
        ///       Called when initialization of the control is complete.
        ///    </para>
        /// </devdoc>
        public void EndInit() {
            initializing = false;
            Value = Constrain(currentValue);
            UpdateEditText();
        }

        /// <summary>
        ///     Overridden to set/reset acceleration variables.
        /// </devdoc>
        protected override void OnKeyDown(KeyEventArgs e) {
            if (base.InterceptArrowKeys && (e.KeyCode == Keys.Up || e.KeyCode == Keys.Down) && !this.Spinning) {
                StartAcceleration();  
            }
            
            base.OnKeyDown(e);
        }
        
        /// <summary>
        ///     Overridden to set/reset acceleration variables.
        /// </devdoc>
        protected override void OnKeyUp(KeyEventArgs e) {
            if (base.InterceptArrowKeys && (e.KeyCode == Keys.Up || e.KeyCode == Keys.Down)) {
                StopAcceleration();  
            }
            
            base.OnKeyUp(e);
        }

        /// <summary>
        ///    <para>
        ///       Restricts the entry of characters to digits (including hex), the negative sign,
        ///       the decimal point, and editing keystrokes (backspace).
        ///    </para>
        /// </devdoc>
        protected override void OnTextBoxKeyPress(object source, KeyPressEventArgs e) {

            base.OnTextBoxKeyPress(source, e);
            
            NumberFormatInfo numberFormatInfo = System.Globalization.CultureInfo.CurrentCulture.NumberFormat;                                
            string decimalSeparator = numberFormatInfo.NumberDecimalSeparator;
            string groupSeparator = numberFormatInfo.NumberGroupSeparator;
            string negativeSign = numberFormatInfo.NegativeSign;
                
            string keyInput = e.KeyChar.ToString();
                
            if (char.IsDigit(e.KeyChar)) {
                // Digits are OK
            }
            else if (keyInput.Equals(decimalSeparator) || keyInput.Equals(groupSeparator) || 
                     keyInput.Equals(negativeSign)) {
                // Decimal separator is OK
            }
            else if (e.KeyChar == '\b') {
                // Backspace key is OK
            }
            else if (Hexadecimal && ((e.KeyChar >= 'a' && e.KeyChar <= 'f') || e.KeyChar >= 'A' && e.KeyChar <= 'F')) {
                // Hexadecimal digits are OK
            }
            else if ((ModifierKeys & (Keys.Control | Keys.Alt)) != 0) {
                // Let the edit control handle control and alt key combinations
            }
            else {
                // Eat this invalid key and beep
                e.Handled = true;
                SafeNativeMethods.MessageBeep(0);
            }
        }
                                  
        /// <summary>
        /// <para>Raises the <see cref='System.Windows.Forms.NumericUpDown.OnValueChanged'/> event.</para>
        /// </devdoc>        
        protected virtual void OnValueChanged(EventArgs e) {

            // Call the event handler
            if (onValueChanged != null) {
                onValueChanged(this, e);
            }
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
        ///   Overridden to start/end acceleration.
        /// </devdoc>
        internal override void OnStartTimer() {
            StartAcceleration();
        }

        /// <summary>
        ///   Overridden to start/end acceleration.
        /// </devdoc>
        internal override void OnStopTimer() {
            StopAcceleration();
        }

        /// <summary>
        ///    <para>
        ///       Converts the text displayed in the up-down control to a
        ///       numeric value and evaluates it.
        ///    </para>
        /// </devdoc>
        protected void ParseEditText() {

            Debug.Assert(UserEdit == true, "ParseEditText() - UserEdit == false");

            try {
                // Verify that the user is not starting the string with a "-"
                // before attempting to set the Value property since a "-" is a valid character with
                // which to start a string representing a negative number.
                if (!string.IsNullOrEmpty(Text) &&
                    !(Text.Length == 1 && Text == "-")) {
                    if (Hexadecimal) {                    
                        Value = Constrain(Convert.ToDecimal(Convert.ToInt32(Text, 16)));
                    }
                    else {
                        Value = Constrain(decimal.Parse(Text, CultureInfo.CurrentCulture));                
                    }
                }
            }
            catch {
                // Leave value as it is
            }
            finally {
               UserEdit = false;
            }
        }

        /// <summary>
        ///     Updates the index of the UpDownNumericAcceleration entry to use (if needed).
        /// </devdoc>
        private void SetNextAcceleration() {
            // Spinning will check if accelerations is null.
            if(this.Spinning && this.accelerationsCurrentIndex < (this.accelerations.Count - 1))  { // if index not the last entry ...
                // Ticks are in 100-nanoseconds (1E-7 seconds).
                long nowTicks                 = DateTime.Now.Ticks;
                long buttonPressedElapsedTime = nowTicks - this.buttonPressedStartTime;
                long accelerationInterval     = 10000000L * this.accelerations[this.accelerationsCurrentIndex + 1].Seconds;  // next entry.
            
                // If Up/Down button pressed for more than the current acceleration entry interval, get next entry in the accel table.
                if( buttonPressedElapsedTime > accelerationInterval ) 
                {
                    this.buttonPressedStartTime = nowTicks;
                    this.accelerationsCurrentIndex++;
                }
            }
        }

        private void ResetIncrement() {
            Increment = DefaultIncrement;
        }

        private void ResetMaximum() {
            Maximum = DefaultMaximum;            
        }

        private void ResetMinimum() {
            Minimum = DefaultMinimum;
        }

        private void ResetValue() {
            Value = DefaultValue;
        }
        
        /// <summary>
        /// <para>Indicates whether the <see cref='System.Windows.Forms.NumericUpDown.Increment'/> property should be
        ///    persisted.</para>
        /// </devdoc>
        private bool ShouldSerializeIncrement() {
            return !Increment.Equals(NumericUpDown.DefaultIncrement);
        }

        /// <summary>
        /// <para>Indicates whether the <see cref='System.Windows.Forms.NumericUpDown.Maximum'/> property should be persisted.</para>
        /// </devdoc>
        private bool ShouldSerializeMaximum() {
            return !Maximum.Equals(NumericUpDown.DefaultMaximum);
        }

        /// <summary>
        /// <para>Indicates whether the <see cref='System.Windows.Forms.NumericUpDown.Minimum'/> property should be persisted.</para>
        /// </devdoc>
        private bool ShouldSerializeMinimum() {
            return !Minimum.Equals(NumericUpDown.DefaultMinimum);
        }

        /// <summary>
        /// <para>Indicates whether the <see cref='System.Windows.Forms.NumericUpDown.Value'/> property should be persisted.</para>
        /// </devdoc>
        private bool ShouldSerializeValue() {
            return !Value.Equals(NumericUpDown.DefaultValue);
        }

        
        /// <summary>
        ///     Records when UpDownButtons are pressed to enable acceleration.
        /// </devdoc>
        private void StartAcceleration() {
            this.buttonPressedStartTime = DateTime.Now.Ticks;
        }

        /// <summary>
        ///     Reset when UpDownButtons are pressed.
        /// </devdoc>
        private void StopAcceleration() {
            this.accelerationsCurrentIndex = InvalidValue;
            this.buttonPressedStartTime        = InvalidValue;
        }

        /// <summary>
        ///     Provides some interesting info about this control in String form.
        /// </devdoc>
        public override string ToString() {

            string s = base.ToString();
            s += ", Minimum = " + Minimum.ToString(CultureInfo.CurrentCulture) + ", Maximum = " + Maximum.ToString(CultureInfo.CurrentCulture);
            return s;
        }
        
        /// <summary>
        ///    <para>
        ///       Increments the value of the up-down control.
        ///    </para>
        /// </devdoc>
        public override void UpButton() {
            SetNextAcceleration();

            if (UserEdit) {
                ParseEditText();
            }

            decimal newValue = currentValue;

            // Operations on Decimals can throw OverflowException.
            //
            try{
                newValue += this.Increment;

                if (newValue > maximum){
                    newValue = maximum;
                    if( this.Spinning){
                        StopAcceleration();
                    }
                }
            }
            catch( OverflowException ){
                newValue = maximum;
            }

            Value = newValue;            
        }

        private string GetNumberText(decimal num) {
            string text;
            
            if (Hexadecimal) {
                text = ((long)num).ToString("X", CultureInfo.InvariantCulture);
                Debug.Assert(text == text.ToUpper(CultureInfo.InvariantCulture), "GetPreferredSize assumes hex digits to be uppercase.");
            }
            else {
                text = num.ToString((ThousandsSeparator ? "N" : "F") + DecimalPlaces.ToString(CultureInfo.CurrentCulture), CultureInfo.CurrentCulture);
            }
            return text;
        }

        /// <summary>
        ///    <para>
        ///       Displays the current value of the up-down control in the appropriate format.
        ///    </para>
        /// </devdoc>
        protected override void UpdateEditText() {
            // If we're initializing, we don't want to update the edit text yet,
            // just in case the value is invalid.
            if (initializing) {
                return;
            }

            // If the current value is user-edited, then parse this value before reformatting
            if (UserEdit) {
                ParseEditText();
            }

            // Verify that the user is not starting the string with a "-"
            // before attempting to set the Value property since a "-" is a valid character with
            // which to start a string representing a negative number.
            if (currentValueChanged || (!string.IsNullOrEmpty(Text) &&
                !(Text.Length == 1 && Text == "-"))) {

                currentValueChanged = false;
                ChangingText = true;

                // Make sure the current value is within the min/max
                Debug.Assert(minimum <= currentValue && currentValue <= maximum,
                             "DecimalValue lies outside of [minimum, maximum]");

                Text = GetNumberText(currentValue);
                Debug.Assert(ChangingText == false, "ChangingText should have been set to false");
            }
        }

        /// <summary>
        ///    <para>
        ///       Validates and updates
        ///       the text displayed in the up-down control.
        ///    </para>
        /// </devdoc>
        protected override void ValidateEditText() {

            // See if the edit text parses to a valid decimal
            ParseEditText();
            UpdateEditText();
        }

        // This is not a breaking change -- Even though this control previously autosized to hieght,
        // it didn't actually have an AutoSize property.  The new AutoSize property enables the
        // smarter behavior.
        internal override Size GetPreferredSizeCore(Size proposedConstraints) {
            int height = PreferredHeight;
            
            int baseSize = Hexadecimal ? 16 : 10;
            int digit = GetLargestDigit(0, baseSize);
            // The floor of log is intentionally 1 less than the number of digits.  We initialize
            // testNumber to account for the missing digit.
            int numDigits = (int)Math.Floor(Math.Log(Math.Max(-(double)Minimum, (double)Maximum), baseSize));
            int maxDigits;
            if (this.Hexadecimal) {
                maxDigits = (int)Math.Floor(Math.Log(long.MaxValue, baseSize));
            }
            else {
                maxDigits = (int)Math.Floor(Math.Log((double)decimal.MaxValue, baseSize));
            }
            bool maxDigitsReached = numDigits >= maxDigits;
            decimal testNumber;
            
            // preinitialize testNumber with the leading digit
            if(digit != 0 || numDigits == 1) {
                testNumber = digit;
            } else {
                // zero can not be the leading digit if we need more than
                // one digit.  (0*baseSize = 0 in the loop below)
                testNumber = GetLargestDigit(1, baseSize);
            }

            if (maxDigitsReached) {
                // Prevent 


                numDigits = maxDigits - 1;
            }

            // e.g., if the lagest digit is 7, and we can have 3 digits, the widest string would be "777"
            for(int i = 0; i < numDigits; i++) {
                testNumber = testNumber * baseSize + digit;
            }

            int textWidth = TextRenderer.MeasureText(GetNumberText(testNumber), this.Font).Width;

            if (maxDigitsReached) {
                string shortText;
                if (this.Hexadecimal) {
                    shortText = ((long) testNumber).ToString("X", CultureInfo.InvariantCulture);
                }
                else {
                    shortText = testNumber.ToString(CultureInfo.CurrentCulture);
                }
                int shortTextWidth = TextRenderer.MeasureText(shortText, this.Font).Width;
                // Adding the width of the one digit that was dropped earlier. 
                // This assumes that no additional thousand separator is added by that digit which is correct.
                textWidth += shortTextWidth / (numDigits+1);
            }
            
            // Call AdjuctWindowRect to add space for the borders
            int width = SizeFromClientSize(textWidth, height).Width + upDownButtons.Width;
            return new Size(width, height) + Padding.Size;
        }

        private int GetLargestDigit(int start, int end) {
            int largestDigit = -1;
            int digitWidth = -1;

            for(int i = start; i < end; i++) {
                char ch;
                if(i < 10) {
                    ch = i.ToString(CultureInfo.InvariantCulture)[0];
                } else {
                    ch = (char)('A' + (i - 10));
                }

                Size digitSize = TextRenderer.MeasureText(ch.ToString(), this.Font);

                if(digitSize.Width >= digitWidth) {
                    digitWidth = digitSize.Width;
                    largestDigit = i;
                }
            }
            Debug.Assert(largestDigit != -1 && digitWidth != -1, "Failed to find largest digit.");
            return largestDigit;
        }
        
        [System.Runtime.InteropServices.ComVisible(true)]        
        internal class NumericUpDownAccessibleObject : ControlAccessibleObject {

            public NumericUpDownAccessibleObject(NumericUpDown owner) : base(owner) {
            }

            /// <summary>
            /// Gets or sets the accessible name.
            /// </summary>
            public override string Name {
                get {
                    string baseName = base.Name;
                    return ((NumericUpDown)Owner).GetAccessibleName(baseName);
                }
                set {
                    base.Name = value;
                }
            }
            
            public override AccessibleRole Role {
                get {
                    AccessibleRole role = Owner.AccessibleRole;
                    if (role != AccessibleRole.Default) {
                        return role;
                    }
                    else {
                        return AccessibleRole.SpinButton;
                    }
                }
            }
            
            public override AccessibleObject GetChild(int index) {
                if (index >= 0 && index < GetChildCount()) {
                    
                    // TextBox child
                    //
                    if (index == 0) {
                        return ((UpDownBase)Owner).TextBox.AccessibilityObject.Parent;
                    }
                    
                    // Up/down buttons
                    //
                    if (index == 1) {
                        return ((UpDownBase)Owner).UpDownButtonsInternal.AccessibilityObject.Parent;
                    }
                }
                
                return null;
            }

            public override int GetChildCount() {
                return 2;
            }            
        }
    }

}
