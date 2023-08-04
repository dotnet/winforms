// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections;

namespace System.Windows.Forms.Design;

internal class PropertyGridDesigner : ControlDesigner
{
    protected override void PreFilterProperties(IDictionary properties)
    {
        // Remove the ScrollableControl properties...
        properties.Remove(nameof(PropertyGrid.AutoScroll));
        properties.Remove(nameof(PropertyGrid.AutoScrollMargin));
        properties.Remove(nameof(PropertyGrid.AutoScrollMinSize));
        properties.Remove(nameof(PropertyGrid.DockPadding));

        base.PreFilterProperties(properties);
    }
}
