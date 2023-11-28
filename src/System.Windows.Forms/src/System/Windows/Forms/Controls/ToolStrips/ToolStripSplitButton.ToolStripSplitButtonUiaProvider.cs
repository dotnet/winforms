// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Windows.Win32.System.Variant;
using Windows.Win32.UI.Accessibility;

namespace System.Windows.Forms;

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
            => _accessibleObject.DoDefaultAction();

        internal override VARIANT GetPropertyValue(UIA_PROPERTY_ID propertyID)
            => _accessibleObject.GetPropertyValue(propertyID);

        internal override bool IsIAccessibleExSupported() => true;

        internal override bool IsPatternSupported(UIA_PATTERN_ID patternId)
            => _accessibleObject.IsPatternSupported(patternId);

        internal override void Expand()
            => DoDefaultAction();

        internal override void Collapse()
            => _accessibleObject.Collapse();

        internal override ExpandCollapseState ExpandCollapseState
            => _accessibleObject.ExpandCollapseState;

        internal override IRawElementProviderFragment.Interface? FragmentNavigate(NavigateDirection direction)
            => direction switch
            {
                NavigateDirection.NavigateDirection_FirstChild => base.FragmentNavigate(direction),
                NavigateDirection.NavigateDirection_LastChild => base.FragmentNavigate(direction),
                _ => _accessibleObject.FragmentNavigate(direction)
            };
    }
}
