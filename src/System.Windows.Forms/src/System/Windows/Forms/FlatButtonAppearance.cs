// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms.Layout;

namespace System.Windows.Forms;

[TypeConverter(typeof(FlatButtonAppearanceConverter))]
public class FlatButtonAppearance
{
    private readonly ButtonBase _owner;

    private int _borderSize = 1;
    private Color _borderColor = Color.Empty;
    private Color _checkedBackColor = Color.Empty;
    private Color _mouseDownBackColor = Color.Empty;
    private Color _mouseOverBackColor = Color.Empty;

    internal FlatButtonAppearance(ButtonBase owner)
    {
        _owner = owner;
    }

    /// <summary>
    ///  For buttons whose FlatStyle is FlatStyle.Flat, this property specifies the size, in pixels of the border around the button.
    /// </summary>
    [Browsable(true)]
    [ApplicableToButton]
    [NotifyParentProperty(true)]
    [SRCategory(nameof(SR.CatAppearance))]
    [SRDescription(nameof(SR.ButtonBorderSizeDescr))]
    [EditorBrowsable(EditorBrowsableState.Always)]
    [DefaultValue(1)]
    public int BorderSize
    {
        get
        {
            return _borderSize;
        }
        set
        {
            if (value < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(value), value, string.Format(SR.InvalidLowBoundArgumentEx, nameof(BorderSize), value, 0));
            }

            if (_borderSize != value)
            {
                _borderSize = value;
                if (_owner.ParentInternal is not null)
                {
                    LayoutTransaction.DoLayoutIf(_owner.AutoSize, _owner.ParentInternal, _owner, PropertyNames.FlatAppearanceBorderSize);
                }

                _owner.Invalidate();
            }
        }
    }

    /// <summary>
    ///  For buttons whose FlatStyle is FlatStyle.Flat, this property specifies the color of the border around the button.
    /// </summary>
    [Browsable(true)]
    [ApplicableToButton]
    [NotifyParentProperty(true)]
    [SRCategory(nameof(SR.CatAppearance))]
    [SRDescription(nameof(SR.ButtonBorderColorDescr))]
    [EditorBrowsable(EditorBrowsableState.Always)]
    [DefaultValue(typeof(Color), "")]
    public Color BorderColor
    {
        get
        {
            return _borderColor;
        }
        set
        {
            if (value.Equals(Color.Transparent))
            {
                throw new NotSupportedException(SR.ButtonFlatAppearanceInvalidBorderColor);
            }

            if (_borderColor != value)
            {
                _borderColor = value;
                _owner.Invalidate();
            }
        }
    }

    /// <summary>
    ///  For buttons whose FlatStyle is FlatStyle.Flat, this property specifies the color of the client area
    ///  of the button when the button state is checked and the mouse cursor is NOT within the bounds of the control.
    /// </summary>
    [Browsable(true)]
    [NotifyParentProperty(true)]
    [SRCategory(nameof(SR.CatAppearance))]
    [SRDescription(nameof(SR.ButtonCheckedBackColorDescr))]
    [EditorBrowsable(EditorBrowsableState.Always)]
    [DefaultValue(typeof(Color), "")]
    public Color CheckedBackColor
    {
        get
        {
            return _checkedBackColor;
        }
        set
        {
            if (_checkedBackColor != value)
            {
                _checkedBackColor = value;
                _owner.Invalidate();
            }
        }
    }

    /// <summary>
    ///  For buttons whose FlatStyle is FlatStyle.Flat, this property specifies the color of the client area
    ///  of the button when the mouse cursor is within the bounds of the control and the left button is pressed.
    /// </summary>
    [Browsable(true)]
    [ApplicableToButton]
    [NotifyParentProperty(true)]
    [SRCategory(nameof(SR.CatAppearance))]
    [SRDescription(nameof(SR.ButtonMouseDownBackColorDescr))]
    [EditorBrowsable(EditorBrowsableState.Always)]
    [DefaultValue(typeof(Color), "")]
    public Color MouseDownBackColor
    {
        get
        {
            return _mouseDownBackColor;
        }
        set
        {
            if (_mouseDownBackColor != value)
            {
                _mouseDownBackColor = value;
                _owner.Invalidate();
            }
        }
    }

    /// <summary>
    ///  For buttons whose FlatStyle is FlatStyle.Flat, this property specifies the color of the client
    ///  area of the button when the mouse cursor is within the bounds of the control.
    /// </summary>
    [Browsable(true)]
    [ApplicableToButton]
    [NotifyParentProperty(true)]
    [SRCategory(nameof(SR.CatAppearance))]
    [SRDescription(nameof(SR.ButtonMouseOverBackColorDescr))]
    [EditorBrowsable(EditorBrowsableState.Always)]
    [DefaultValue(typeof(Color), "")]
    public Color MouseOverBackColor
    {
        get
        {
            return _mouseOverBackColor;
        }
        set
        {
            if (_mouseOverBackColor != value)
            {
                _mouseOverBackColor = value;
                _owner.Invalidate();
            }
        }
    }
}
