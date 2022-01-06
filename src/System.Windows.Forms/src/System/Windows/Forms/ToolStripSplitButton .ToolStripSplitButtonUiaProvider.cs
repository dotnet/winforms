// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using static Interop;

namespace System.Windows.Forms
{
    public partial class ToolStripSplitButton
    {
        internal class ToolStripSplitButtonUiaProvider : ToolStripDropDownItemAccessibleObject
        {
            private readonly ToolStripSplitButtonExAccessibleObject _accessibleObject;

            public ToolStripSplitButtonUiaProvider(ToolStripSplitButton owner) : base(owner)
            {
                _accessibleObject = new ToolStripSplitButtonExAccessibleObject(owner);
            }

            public override void DoDefaultAction()
            {
                _accessibleObject.DoDefaultAction();
            }

            internal override object GetPropertyValue(UiaCore.UIA propertyID)
            {
                return _accessibleObject.GetPropertyValue(propertyID);
            }

            internal override bool IsIAccessibleExSupported() => true;

            internal override bool IsPatternSupported(UiaCore.UIA patternId)
            {
                return _accessibleObject.IsPatternSupported(patternId);
            }

            internal override void Expand()
            {
                DoDefaultAction();
            }

            internal override void Collapse()
            {
                _accessibleObject.Collapse();
            }

            internal override UiaCore.ExpandCollapseState ExpandCollapseState
            {
                get
                {
                    return _accessibleObject.ExpandCollapseState;
                }
            }

            internal override UiaCore.IRawElementProviderFragment FragmentNavigate(UiaCore.NavigateDirection direction)
            {
                return _accessibleObject.FragmentNavigate(direction);
            }
        }
    }
}
