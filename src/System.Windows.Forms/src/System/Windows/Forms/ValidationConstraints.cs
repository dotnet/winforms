// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    /// <summary>
    ///  Determines which child controls in a ContainerControl will be validated
    ///  by the <see cref='ContainerControl.ValidateChildren(ValidationConstraints)'/> method.
    /// </summary>
    [Flags]
    public enum ValidationConstraints
    {
        /// <summary>
        ///  All child controls and their descendants are validated.
        /// </summary>
        None = 0x00,

        /// <summary>
        ///  Child control must be selectable to be validated.
        ///
        ///  Note: This flag allows validation of a control that has the
        ///  ControlStyles.Selectable style, ie. has the ability to be selected,
        ///  even when that control is currently unselectable because it has been
        ///  disabled or hidden. To prevent validation of selectable controls that
        ///  are currently disabled or hidden,
        /// </summary>
        Selectable = 0x01,

        /// <summary>
        ///  Child control must be enabled to be validated (Control.Enabled = true).
        /// </summary>
        Enabled = 0x02,

        /// <summary>
        ///  Child control must be visible to be validated (Control.Visible = true).
        /// </summary>
        Visible = 0x04,

        /// <summary>
        ///  Child control must be a tab stops to be validated (Control.TabStop = true).
        /// </summary>
        TabStop = 0x08,

        /// <summary>
        ///  Only immediate children of container control are validated.
        ///  Descendants are not validated.
        /// </summary>
        ImmediateChildren = 0x10,
    }
}
