// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;

namespace System.Windows.Forms;

public partial class ErrorProvider
{
    /// <summary>
    ///  There is one ControlItem for each control that the ErrorProvider is tracking state for.
    ///  It contains the values of all the extender properties.
    /// </summary>
    internal partial class ControlItem
    {
        private string _error;
        private readonly Control _control;
        private ErrorWindow? _window;
        private readonly ErrorProvider _provider;
        private int _iconPadding;
        private ErrorIconAlignment _iconAlignment;
        private const int StartingBlinkPhase = 10; // We want to blink 5 times
        private AccessibleObject? _accessibleObject;

        /// <summary>
        ///  Construct the item with its associated control, provider, and a unique ID. The ID is
        ///  used for the tooltip ID.
        /// </summary>
        public ControlItem(ErrorProvider provider, Control control, IntPtr id)
        {
            _iconAlignment = DefaultIconAlignment;
            _error = string.Empty;
            Id = id;
            _control = control;
            _provider = provider;
            _control.HandleCreated += OnCreateHandle;
            _control.HandleDestroyed += OnDestroyHandle;
            _control.LocationChanged += OnBoundsChanged;
            _control.SizeChanged += OnBoundsChanged;
            _control.VisibleChanged += OnParentVisibleChanged;
            _control.ParentChanged += OnParentVisibleChanged;
        }

        /// <summary>
        ///  The Accessibility Object for this ErrorProvider
        /// </summary>
        internal AccessibleObject AccessibilityObject => _accessibleObject ??= CreateAccessibilityInstance();

        /// <summary>
        ///  Constructs the new instance of the accessibility object for this ErrorProvider. Subclasses
        ///  should not call base.CreateAccessibilityObject.
        /// </summary>
        private ControlItemAccessibleObject CreateAccessibilityInstance() => new(this, _window, _control, _provider);

        public void Dispose()
        {
            if (_control is not null)
            {
                _control.HandleCreated -= OnCreateHandle;
                _control.HandleDestroyed -= OnDestroyHandle;
                _control.LocationChanged -= OnBoundsChanged;
                _control.SizeChanged -= OnBoundsChanged;
                _control.VisibleChanged -= OnParentVisibleChanged;
                _control.ParentChanged -= OnParentVisibleChanged;
            }

            _error = string.Empty;
        }

        /// <summary>
        ///  Returns the unique ID for this control. The ID used as the tooltip ID.
        /// </summary>
        public IntPtr Id { get; }

        /// <summary>
        ///  Returns or set the phase of blinking that this control is currently
        ///  in. If zero, the control is not blinking. If odd, then the control
        ///  is blinking, but invisible. If even, the control is blinking and
        ///  currently visible. Each time the blink timer fires, this value is
        ///  reduced by one (until zero), thus causing the error icon to appear
        ///  or disappear.
        /// </summary>
        public int BlinkPhase { get; set; }

        /// <summary>
        ///  Returns or sets the icon padding for the control.
        /// </summary>
        public int IconPadding
        {
            get => _iconPadding;
            set
            {
                if (_iconPadding == value)
                {
                    return;
                }

                _iconPadding = value;
                UpdateWindow();
            }
        }

        /// <summary>
        ///  Returns or sets the error description string for the control.
        /// </summary>
        [AllowNull]
        public string Error
        {
            get => _error;
            set
            {
                value ??= string.Empty;

                // If the error is the same and the blinkStyle is not AlwaysBlink, then
                // we should not add the error and not start blinking.
                if (_error.Equals(value) && _provider.BlinkStyle != ErrorBlinkStyle.AlwaysBlink)
                {
                    return;
                }

                bool adding = _error.Length == 0;
                _error = value;
                if (value.Length == 0)
                {
                    RemoveFromWindow();
                }
                else
                {
                    if (adding)
                    {
                        AddToWindow();
                    }
                    else
                    {
                        if (_provider.BlinkStyle != ErrorBlinkStyle.NeverBlink)
                        {
                            StartBlinking();
                        }
                        else
                        {
                            UpdateWindow();
                        }
                    }
                }
            }
        }

        /// <summary>
        ///  Returns or sets the location of the error icon for the control.
        /// </summary>
        public ErrorIconAlignment IconAlignment
        {
            get => _iconAlignment;
            set
            {
                SourceGenerated.EnumValidator.Validate(value);

                if (_iconAlignment == value)
                {
                    return;
                }

                _iconAlignment = value;
                UpdateWindow();
            }
        }

        /// <summary>
        ///  Returns true if the tooltip for this control item is currently shown.
        /// </summary>
        public bool ToolTipShown { get; set; }

        internal ErrorIconAlignment RTLTranslateIconAlignment(ErrorIconAlignment align)
        {
            if (_provider.RightToLeft)
            {
                switch (align)
                {
                    case ErrorIconAlignment.TopLeft:
                        return ErrorIconAlignment.TopRight;
                    case ErrorIconAlignment.MiddleLeft:
                        return ErrorIconAlignment.MiddleRight;
                    case ErrorIconAlignment.BottomLeft:
                        return ErrorIconAlignment.BottomRight;
                    case ErrorIconAlignment.TopRight:
                        return ErrorIconAlignment.TopLeft;
                    case ErrorIconAlignment.MiddleRight:
                        return ErrorIconAlignment.MiddleLeft;
                    case ErrorIconAlignment.BottomRight:
                        return ErrorIconAlignment.BottomLeft;
                    default:
                        Debug.Fail("Unknown ErrorIconAlignment value");
                        return align;
                }
            }
            else
            {
                return align;
            }
        }

        /// <summary>
        ///  Returns the location of the icon in the same coordinate system as the control being
        ///  extended. The size passed in is the size of the icon.
        /// </summary>
        internal Rectangle GetIconBounds(Size size)
        {
            int x = 0;
            int y = 0;

            switch (RTLTranslateIconAlignment(IconAlignment))
            {
                case ErrorIconAlignment.TopLeft:
                case ErrorIconAlignment.MiddleLeft:
                case ErrorIconAlignment.BottomLeft:
                    x = _control.Left - size.Width - _iconPadding;
                    break;
                case ErrorIconAlignment.TopRight:
                case ErrorIconAlignment.MiddleRight:
                case ErrorIconAlignment.BottomRight:
                    x = _control.Right + _iconPadding;
                    break;
            }

            switch (IconAlignment)
            {
                case ErrorIconAlignment.TopLeft:
                case ErrorIconAlignment.TopRight:
                    y = _control.Top;
                    break;
                case ErrorIconAlignment.MiddleLeft:
                case ErrorIconAlignment.MiddleRight:
                    y = _control.Top + (_control.Height - size.Height) / 2;
                    break;
                case ErrorIconAlignment.BottomLeft:
                case ErrorIconAlignment.BottomRight:
                    y = _control.Bottom - size.Height;
                    break;
            }

            return new Rectangle(x, y, size.Width, size.Height);
        }

        /// <summary>
        ///  If this control's error icon has been added to the error window, then update the
        ///  window state because some property has changed.
        /// </summary>
        private void UpdateWindow() => _window?.Update(timerCaused: false);

        /// <summary>
        ///  If this control's error icon has been added to the error window, then start blinking
        ///  the error window.
        /// </summary>
        private void StartBlinking()
        {
            if (_window is not null)
            {
                BlinkPhase = StartingBlinkPhase;
                _window.StartBlinking();
            }
        }

        /// <summary>
        ///  Add this control's error icon to the error window.
        /// </summary>
        private void AddToWindow()
        {
            // if we are recreating the control, then add the control.
            if (_window is null &&
                (_control.Created || _control.RecreatingHandle) &&
                _control.Visible && _control.ParentInternal is not null &&
                _error.Length > 0)
            {
                _window = _provider.EnsureErrorWindow(_control.ParentInternal);
                _window.Add(this);
                // Make sure that we blink if the style is set to AlwaysBlink or BlinkIfDifferentError
                if (_provider.BlinkStyle != ErrorBlinkStyle.NeverBlink)
                {
                    StartBlinking();
                }
            }
        }

        /// <summary>
        ///  Remove this control's error icon from the error window.
        /// </summary>
        private void RemoveFromWindow()
        {
            if (_window is not null)
            {
                _window.Remove(this);
                _window = null;
            }
        }

        /// <summary>
        ///  This is called when a property on the control is changed.
        /// </summary>
        private void OnBoundsChanged(object? sender, EventArgs e) => UpdateWindow();

        private void OnParentVisibleChanged(object? sender, EventArgs e)
        {
            BlinkPhase = 0;
            RemoveFromWindow();
            AddToWindow();
        }

        /// <summary>
        ///  This is called when the control's handle is created.
        /// </summary>
        private void OnCreateHandle(object? sender, EventArgs e) => AddToWindow();

        /// <summary>
        ///  This is called when the control's handle is destroyed.
        /// </summary>
        private void OnDestroyHandle(object? sender, EventArgs e) => RemoveFromWindow();
    }
}
