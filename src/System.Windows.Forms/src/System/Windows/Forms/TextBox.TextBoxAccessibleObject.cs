// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Windows.Win32.System.Variant;
using Windows.Win32.UI.Accessibility;

namespace System.Windows.Forms;

public partial class TextBox
{
    internal class TextBoxAccessibleObject : TextBoxBaseAccessibleObject
    {
        public TextBoxAccessibleObject(TextBox owner) : base(owner)
        { }

        internal override VARIANT GetPropertyValue(UIA_PROPERTY_ID propertyID)
        {
            switch (propertyID)
            {
                case UIA_PROPERTY_ID.UIA_HelpTextPropertyId:
                    string? placeholderText = this.TryGetOwnerAs(out TextBox? owner) ? owner.PlaceholderText : null;
                    return string.IsNullOrEmpty(placeholderText) ? base.GetPropertyValue(propertyID) : (VARIANT)placeholderText;
                default:
                    return base.GetPropertyValue(propertyID);
            }
        }
    }
}
