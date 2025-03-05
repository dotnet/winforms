// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Windows.Win32.UI.Accessibility;

namespace System.Windows.Forms;

public partial class ToolStripControlHost
{
    /// <summary>
    ///  Defines the ToolStripControlHost AccessibleObject.
    /// </summary>
    internal sealed class ToolStripControlHostAccessibleObject : ToolStripItemAccessibleObject
    {
        private readonly ToolStripControlHost _ownerItem;

        /// <summary>
        ///  Initializes the new instance of ToolStripControlHostAccessibleObject.
        /// </summary>
        /// <param name="ownerItem">The owning ToolStripControlHost.</param>
        public ToolStripControlHostAccessibleObject(ToolStripControlHost ownerItem) : base(ownerItem)
        {
            _ownerItem = ownerItem;
        }

        /// <summary>
        ///  Gets a description of the default action for an object.
        /// </summary>
        public override string DefaultAction
        {
            get
            {
                // Note: empty value is provided due to this Accessible object
                // represents the control container but not the contained control
                // itself.
                return string.Empty;
            }
        }

        private protected override bool IsInternal => true;

        internal override bool CanGetDefaultActionInternal => false;

        /// <summary>
        ///  Performs the default action associated with this accessible object.
        /// </summary>
        public override void DoDefaultAction()
        {
            // Do nothing.
        }

        /// <summary>
        ///  Gets the role of this accessible object.
        /// </summary>
        public override AccessibleRole Role
        {
            get
            {
                AccessibleObject? controlAccessibleObject = _ownerItem.ControlAccessibilityObject;
                if (controlAccessibleObject is not null)
                {
                    return controlAccessibleObject.Role;
                }

                return AccessibleRole.Default;
            }
        }

        internal override IRawElementProviderFragment.Interface? FragmentNavigate(NavigateDirection direction)
        {
            if (direction is NavigateDirection.NavigateDirection_FirstChild
                or NavigateDirection.NavigateDirection_LastChild)
            {
                return _ownerItem.Control.AccessibilityObject;
            }

            // Handle Parent and other directions in base ToolStripItem.FragmentNavigate() method.
            return base.FragmentNavigate(direction);
        }

        internal override IRawElementProviderFragmentRoot.Interface? FragmentRoot =>
            _ownerItem.RootToolStrip?.AccessibilityObject;
    }
}
