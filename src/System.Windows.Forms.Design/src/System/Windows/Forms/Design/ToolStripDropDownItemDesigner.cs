﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;

namespace System.Windows.Forms.Design;

/// <summary>
///  Designer for ToolStripDropDownItems. This is here so only the dropdown items get the "Edit Items..." verb.
/// </summary>
internal class ToolStripDropDownItemDesigner : ToolStripItemDesigner
{
    /// <summary>
    ///  Initialize the item.
    /// </summary>
    public override void Initialize(IComponent component)
    {
        base.Initialize(component);
    }

    /// <summary>
    ///  The ToolStripItems are the associated components. We want those to come with in any cut, copy opreations.
    /// </summary>
    public override Collections.ICollection AssociatedComponents
    {
        get
        {
            if (Component is ToolStripDropDownItem item && item.DropDown.IsAutoGenerated)
            {
                return item.DropDownItems;
            }

            return base.AssociatedComponents;
        }
    }
}
