// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Windows.Win32.System.Variant;
using Windows.Win32.UI.Accessibility;

namespace System.Windows.Forms;

internal partial class ToolStripGrip
{
    internal sealed class ToolStripGripAccessibleObject : ToolStripButtonAccessibleObject
    {
        public ToolStripGripAccessibleObject(ToolStripGrip owner) : base(owner)
        {
        }

        [AllowNull]
        public override string Name => Owner.AccessibleName ?? SR.ToolStripGripAccessibleName;

        private protected override bool IsInternal => true;

        public override AccessibleRole Role
        {
            get
            {
                AccessibleRole role = Owner.AccessibleRole;
                if (role != AccessibleRole.Default)
                {
                    return role;
                }

                return AccessibleRole.Grip;
            }
        }

        internal override VARIANT GetPropertyValue(UIA_PROPERTY_ID propertyID) =>
            propertyID switch
            {
                UIA_PROPERTY_ID.UIA_IsOffscreenPropertyId => VARIANT.False,
                _ => base.GetPropertyValue(propertyID)
            };
    }
}
