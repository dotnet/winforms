// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using static Interop;

namespace System.Windows.Forms;

public partial class WebBrowser
{
    internal class WebBrowserAccessibleObject : ControlAccessibleObject
    {
        public WebBrowserAccessibleObject(WebBrowser owner) : base(owner)
        { }

        internal override object? GetPropertyValue(UiaCore.UIA propertyID)
            => propertyID switch
            {
                UiaCore.UIA.HasKeyboardFocusPropertyId => this.TryGetOwnerAs(out WebBrowser? owner) && owner.Focused,
                _ => base.GetPropertyValue(propertyID)
            };
    }
}
