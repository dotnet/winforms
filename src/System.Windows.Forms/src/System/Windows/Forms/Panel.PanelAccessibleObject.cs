// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using static Interop;

namespace System.Windows.Forms
{
    public partial class Panel
    {
        internal class PanelAccessibleObject : ControlAccessibleObject
        {
            private readonly Panel _owningPanel;

            public PanelAccessibleObject(Panel owner) : base(owner)
            {
                _owningPanel = owner;
            }

            internal override UiaCore.IRawElementProviderFragmentRoot FragmentRoot => this;

            public override AccessibleObject? GetChild(int index)
            {
                if (!_owningPanel.IsHandleCreated || index < 0 || index >= _owningPanel.Controls.Count)
                {
                    return null;
                }

                return _owningPanel.Controls[index].AccessibilityObject;
            }

            public override int GetChildCount()
                => _owningPanel.IsHandleCreated
                    ? _owningPanel.Controls.Count
                    : -1;

            internal override object? GetPropertyValue(UiaCore.UIA propertyID)
               => propertyID switch
               {
                   UiaCore.UIA.IsKeyboardFocusablePropertyId
                       => false,
                   _ => base.GetPropertyValue(propertyID)
               };
        }
    }
}
