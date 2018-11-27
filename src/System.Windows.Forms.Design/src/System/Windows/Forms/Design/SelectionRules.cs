// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics.CodeAnalysis;

namespace System.Windows.Forms.Design
{
    /// <summary>
    ///     <para>
    ///         Specifies a set of selection rule identifiers that
    ///         can be used to indicate attributes for a selected component.
    ///     </para>
    /// </summary>
    // We need to combine the SelectionRules.    
    [SuppressMessage("Microsoft.Usage", "CA2217:DoNotMarkEnumsWithFlags")]
    [Flags]
    public enum SelectionRules
    {
        /// <summary>
        ///     <para>
        ///         Indicates
        ///         no special selection attributes.
        ///     </para>
        /// </summary>
        None = 0x00000000,

        /// <summary>
        ///     <para>
        ///         Indicates
        ///         the given component supports a location
        ///         property that allows it to be moved on the screen, and
        ///         that the selection service is not currently locked.
        ///     </para>
        /// </summary>
        Moveable = 0x10000000,

        /// <summary>
        ///     <para>
        ///         Indicates
        ///         the given component has some form of visible user
        ///         interface and the selection service is drawing a selection border around
        ///         this user interface. If a selected component has this rule set, you can assume
        ///         that the component implements <see cref='System.ComponentModel.IComponent' />
        ///         and that it
        ///         is associated with a corresponding design instance.
        ///     </para>
        /// </summary>
        Visible = 0x40000000,

        /// <summary>
        ///     <para>
        ///         Indicates
        ///         the given component is locked to
        ///         its container. Overrides the moveable and sizeable
        ///         properties of this enum.
        ///     </para>
        /// </summary>
        Locked = unchecked((int)0x80000000),

        /// <summary>
        ///     <para>
        ///         Indicates
        ///         the given component supports resize from
        ///         the top. This bit will be ignored unless the Sizeable
        ///         bit is also set.
        ///     </para>
        /// </summary>
        TopSizeable = 0x00000001,

        /// <summary>
        ///     <para>
        ///         Indicates
        ///         the given component supports resize from
        ///         the bottom. This bit will be ignored unless the Sizeable
        ///         bit is also set.
        ///     </para>
        /// </summary>
        BottomSizeable = 0x00000002,

        /// <summary>
        ///     <para>
        ///         Indicates
        ///         the given component supports resize from
        ///         the left. This bit will be ignored unless the Sizeable
        ///         bit is also set.
        ///     </para>
        /// </summary>
        LeftSizeable = 0x00000004,

        /// <summary>
        ///     <para>
        ///         Indicates
        ///         the given component supports resize from
        ///         the right. This bit will be ignored unless the Sizeable
        ///         bit is also set.
        ///     </para>
        /// </summary>
        RightSizeable = 0x00000008,

        /// <summary>
        ///     <para>
        ///         Indicates
        ///         the given component supports sizing
        ///         in all directions, and the selection service is not currently locked.
        ///     </para>
        /// </summary>
        AllSizeable = TopSizeable | BottomSizeable | LeftSizeable | RightSizeable
    }
}
