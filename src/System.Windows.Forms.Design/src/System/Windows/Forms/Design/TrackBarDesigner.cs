// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;

namespace System.Windows.Forms.Design;

internal class TrackBarDesigner : ControlDesigner
{
    public TrackBarDesigner()
    {
        AutoResizeHandles = true;
    }

    public override SelectionRules SelectionRules
    {
        get
        {
            SelectionRules rules = base.SelectionRules;
            rules |= SelectionRules.AllSizeable;

            if (GetPropertyValue<bool>(Component, nameof(TrackBar.AutoSize)))
            {
                var orientation = GetPropertyValue<Orientation?>(Component, nameof(TrackBar.Orientation)) ?? Orientation.Horizontal;

                switch (orientation)
                {
                    case Orientation.Horizontal:
                        rules &= ~(SelectionRules.TopSizeable | SelectionRules.BottomSizeable);
                        break;
                    case Orientation.Vertical:
                        rules &= ~(SelectionRules.LeftSizeable | SelectionRules.RightSizeable);
                        break;
                }
            }

            return rules;
        }
    }

    private static T? GetPropertyValue<T>(object component, string propertyName)
    {
        PropertyDescriptor? prop = TypeDescriptor.GetProperties(component)[propertyName];
        return prop is null ? default : (T?)prop.GetValue(component);
    }
}
