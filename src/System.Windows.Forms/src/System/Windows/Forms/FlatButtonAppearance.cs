// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms.Layout;

namespace System.Windows.Forms
{
    [TypeConverter(typeof(FlatButtonAppearanceConverter))]
    public class FlatButtonAppearance
    {
        private readonly ButtonBase owner;

        private int borderSize = 1;
        private Color borderColor = Color.Empty;
        private Color checkedBackColor = Color.Empty;
        private Color mouseDownBackColor = Color.Empty;
        private Color mouseOverBackColor = Color.Empty;

        internal FlatButtonAppearance(ButtonBase owner)
        {
            this.owner = owner;
        }

        /// <summary>
        ///  For buttons whose FlatStyle is FlatStyle.Flat, this property specifies the size, in pixels of the border around the button.
        /// </summary>
        [
        Browsable(true),
        ApplicableToButton(),
        NotifyParentProperty(true),
        SRCategory(nameof(SR.CatAppearance)),
        SRDescription(nameof(SR.ButtonBorderSizeDescr)),
        EditorBrowsable(EditorBrowsableState.Always),
        DefaultValue(1),
        ]
        public int BorderSize
        {
            get
            {
                return borderSize;
            }
            set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), value, string.Format(SR.InvalidLowBoundArgumentEx, nameof(BorderSize), value, 0));
                }

                if (borderSize != value)
                {
                    borderSize = value;
                    if (owner != null && owner.ParentInternal != null)
                    {
                        LayoutTransaction.DoLayoutIf(owner.AutoSize, owner.ParentInternal, owner, PropertyNames.FlatAppearanceBorderSize);
                    }
                    owner.Invalidate();
                }
            }
        }

        /// <summary>
        ///  For buttons whose FlatStyle is FlatStyle.Flat, this property specifies the color of the border around the button.
        /// </summary>
        [
        Browsable(true),
        ApplicableToButton(),
        NotifyParentProperty(true),
        SRCategory(nameof(SR.CatAppearance)),
        SRDescription(nameof(SR.ButtonBorderColorDescr)),
        EditorBrowsable(EditorBrowsableState.Always),
        DefaultValue(typeof(Color), ""),
        ]
        public Color BorderColor
        {
            get
            {
                return borderColor;
            }
            set
            {
                if (value.Equals(Color.Transparent))
                {
                    throw new NotSupportedException(SR.ButtonFlatAppearanceInvalidBorderColor);
                }

                if (borderColor != value)
                {
                    borderColor = value;
                    owner.Invalidate();
                }
            }
        }

        /// <summary>
        ///  For buttons whose FlatStyle is FlatStyle.Flat, this property specifies the color of the client area
        ///  of the button when the button state is checked and the mouse cursor is NOT within the bounds of the control.
        /// </summary>
        [
        Browsable(true),
        NotifyParentProperty(true),
        SRCategory(nameof(SR.CatAppearance)),
        SRDescription(nameof(SR.ButtonCheckedBackColorDescr)),
        EditorBrowsable(EditorBrowsableState.Always),
        DefaultValue(typeof(Color), ""),
        ]
        public Color CheckedBackColor
        {
            get
            {
                return checkedBackColor;
            }
            set
            {
                if (checkedBackColor != value)
                {
                    checkedBackColor = value;
                    owner.Invalidate();
                }
            }
        }

        /// <summary>
        ///  For buttons whose FlatStyle is FlatStyle.Flat, this property specifies the color of the client area
        ///  of the button when the mouse cursor is within the bounds of the control and the left button is pressed.
        /// </summary>
        [
        Browsable(true),
        ApplicableToButton(),
        NotifyParentProperty(true),
        SRCategory(nameof(SR.CatAppearance)),
        SRDescription(nameof(SR.ButtonMouseDownBackColorDescr)),
        EditorBrowsable(EditorBrowsableState.Always),
        DefaultValue(typeof(Color), ""),
        ]
        public Color MouseDownBackColor
        {
            get
            {
                return mouseDownBackColor;
            }
            set
            {
                if (mouseDownBackColor != value)
                {
                    mouseDownBackColor = value;
                    owner.Invalidate();
                }
            }
        }

        /// <summary>
        ///  For buttons whose FlatStyle is FlatStyle.Flat, this property specifies the color of the client
        ///  area of the button when the mouse cursor is within the bounds of the control.
        /// </summary>
        [
        Browsable(true),
        ApplicableToButton(),
        NotifyParentProperty(true),
        SRCategory(nameof(SR.CatAppearance)),
        SRDescription(nameof(SR.ButtonMouseOverBackColorDescr)),
        EditorBrowsable(EditorBrowsableState.Always),
        DefaultValue(typeof(Color), ""),
        ]
        public Color MouseOverBackColor
        {
            get
            {
                return mouseOverBackColor;
            }
            set
            {
                if (mouseOverBackColor != value)
                {
                    mouseOverBackColor = value;
                    owner.Invalidate();
                }
            }
        }

    }

    internal sealed class ApplicableToButtonAttribute : Attribute
    {
        public ApplicableToButtonAttribute()
        {
        }
    }

    internal class FlatButtonAppearanceConverter : ExpandableObjectConverter
    {
        // Don't let the property grid display the full type name in the value cell
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(string))
            {
                return "";
            }

            return base.ConvertTo(context, culture, value, destinationType);
        }

        // Don't let the property grid display the CheckedBackColor property for Button controls
        public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes)
        {
            if (context != null && context.Instance is Button)
            {
                Attribute[] attributes2 = new Attribute[attributes.Length + 1];
                attributes.CopyTo(attributes2, 0);
                attributes2[attributes.Length] = new ApplicableToButtonAttribute();
                attributes = attributes2;
            }

            return TypeDescriptor.GetProperties(value, attributes);
        }
    }
}
