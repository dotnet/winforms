// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;

namespace System.Windows.Forms.Design;

internal class ToolStripCodeDomSerializer : ControlCodeDomSerializer
{
    protected override bool HasSitedNonReadonlyChildren(Control parent)
    {
        if (parent is not ToolStrip toolStrip)
        {
            Debug.Fail("why were we passed a non winbar?");
            return false;
        }

        if (toolStrip.Items.Count == 0)
        {
            return false;
        }

        foreach (ToolStripItem item in toolStrip.Items)
        {
            if (item.Site is not null && toolStrip.Site is not null && item.Site.Container == toolStrip.Site.Container)
            {
                // We only emit Size/Location information for controls that are sited and not inherited readonly.
                if (TypeDescriptorHelper.TryGetAttribute(item, out InheritanceAttribute? ia) && ia.InheritanceLevel != InheritanceLevel.InheritedReadOnly)
                {
                    return true;
                }
            }
        }

        return false;
    }
}
