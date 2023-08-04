﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using static Interop;

namespace System.Windows.Forms;

public partial class Panel
{
    internal class PanelAccessibleObject : ControlAccessibleObject
    {
        public PanelAccessibleObject(Panel owner) : base(owner)
        {
        }

        internal override UiaCore.IRawElementProviderFragmentRoot FragmentRoot => this;

        public override AccessibleObject? GetChild(int index)
        {
            if (!this.IsOwnerHandleCreated(out Panel? owner) || index < 0 || index >= owner.Controls.Count)
            {
                return null;
            }

            return owner.Controls[index].AccessibilityObject;
        }

        public override int GetChildCount()
            => this.IsOwnerHandleCreated(out Panel? owner) ? owner.Controls.Count : -1;

        internal override object? GetPropertyValue(UiaCore.UIA propertyID)
           => propertyID switch
           {
               UiaCore.UIA.IsKeyboardFocusablePropertyId => false,
               _ => base.GetPropertyValue(propertyID)
           };
    }
}
