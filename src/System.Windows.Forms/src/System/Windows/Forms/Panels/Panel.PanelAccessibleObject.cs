// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using Windows.Win32.System.Variant;
using Windows.Win32.UI.Accessibility;

namespace System.Windows.Forms;

public partial class Panel
{
    internal sealed class PanelAccessibleObject : ControlAccessibleObject
    {
        public PanelAccessibleObject(Panel owner) : base(owner)
        {
        }

        internal override Rectangle BoundingRectangle => this.IsOwnerHandleCreated(out Panel? owner) ?
            owner.GetToolNativeScreenRectangle() : Rectangle.Empty;

        internal override IRawElementProviderFragmentRoot.Interface FragmentRoot => this;

        public override AccessibleObject? GetChild(int index) =>
            !this.IsOwnerHandleCreated(out Panel? owner) || index < 0 || index >= owner.Controls.Count
                ? null
                : owner.Controls[index].AccessibilityObject;

        private protected override bool IsInternal => true;

        public override int GetChildCount()
            => this.IsOwnerHandleCreated(out Panel? owner) ? owner.Controls.Count : -1;

        internal override VARIANT GetPropertyValue(UIA_PROPERTY_ID propertyID)
           => propertyID switch
           {
               UIA_PROPERTY_ID.UIA_IsKeyboardFocusablePropertyId => VARIANT.False,
               _ => base.GetPropertyValue(propertyID)
           };
    }
}
