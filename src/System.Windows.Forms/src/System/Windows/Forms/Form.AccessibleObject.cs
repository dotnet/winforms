// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;
using static Interop;

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

        public override Rectangle Bounds => this.TryGetOwnerAs(out Control? owner) && owner.IsHandleCreated
            ? owner.RectangleToScreen(owner.ClientRectangle)
            : Rectangle.Empty;

        internal override Rectangle BoundingRectangle
            => !this.TryGetOwnerAs(out Control? owner) || !owner.IsHandleCreated
                ? Rectangle.Empty
                : owner.Parent?.RectangleToScreen(owner.Bounds) ?? owner.Bounds;

        internal override object? GetPropertyValue(UiaCore.UIA propertyID) =>
            propertyID switch
            {
                // Unlike other controls, here the default "ControlType" doesn't correspond the value from the mapping
                // depending on the default Role.
                // In other cases "ControlType" will reflect changes to Form.AccessibleRole (i.e. if it is set to a custom role).
                UiaCore.UIA.ControlTypePropertyId when
                    Role == AccessibleRole.Client
                    => UiaCore.UIA.WindowControlTypeId,
                UiaCore.UIA.IsDialogPropertyId => this.TryGetOwnerAs(out Form? owner) && owner.Modal,
                _ => base.GetPropertyValue(propertyID)
            };

        internal override bool IsIAccessibleExSupported() => true;

        public override AccessibleRole Role => this.GetOwnerAccessibleRole(AccessibleRole.Client);
    }
}
