// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using Windows.Win32.System.Variant;
using Windows.Win32.UI.Accessibility;

namespace System.Windows.Forms;

public partial class Form
{
    /// <summary>
    ///  Form control accessible object with UI Automation provider functionality.
    /// </summary>
    internal class FormAccessibleObject : ControlAccessibleObject
    {
        internal FormAccessibleObject(Form owner) : base(owner)
        {
        }

        public override Rectangle Bounds => this.IsOwnerHandleCreated(out Control? owner)
            ? owner.RectangleToScreen(owner.ClientRectangle)
            : Rectangle.Empty;

        internal override Rectangle BoundingRectangle
            => !this.IsOwnerHandleCreated(out Control? owner)
                ? Rectangle.Empty
                : owner.Parent?.RectangleToScreen(owner.Bounds) ?? owner.Bounds;

        internal override VARIANT GetPropertyValue(UIA_PROPERTY_ID propertyID) =>
            propertyID switch
            {
                // Unlike other controls, here the default "ControlType" doesn't correspond the value from the mapping
                // depending on the default Role.
                // In other cases "ControlType" will reflect changes to Form.AccessibleRole (i.e. if it is set to a custom role).
                UIA_PROPERTY_ID.UIA_ControlTypePropertyId when
                    Role == AccessibleRole.Client
                    => (VARIANT)(int)UIA_CONTROLTYPE_ID.UIA_WindowControlTypeId,
                UIA_PROPERTY_ID.UIA_IsDialogPropertyId => (VARIANT)(this.TryGetOwnerAs(out Form? owner) && owner.Modal),
                _ => base.GetPropertyValue(propertyID)
            };

        internal override bool IsIAccessibleExSupported() => true;

        public override AccessibleRole Role => this.GetOwnerAccessibleRole(AccessibleRole.Client);
    }
}
