// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.Diagnostics;

namespace System.Windows.Forms.Design
{
    internal class ToolStripCodeDomSerializer : ControlCodeDomSerializer
    {
        protected override bool HasSitedNonReadonlyChildren(Control parent)
        {
            ToolStrip toolStrip = parent as ToolStrip;
            
            if (toolStrip == null)
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
                if (item.Site != null && toolStrip.Site != null && item.Site.Container == toolStrip.Site.Container)
                {
                    // We only emit Size/Location information for controls that are sited and not inherrited readonly.
                    InheritanceAttribute ia = (InheritanceAttribute)TypeDescriptor.GetAttributes(item)[typeof(InheritanceAttribute)];

                    if (ia != null && ia.InheritanceLevel != InheritanceLevel.InheritedReadOnly)
                    {
                        return true;
                    }
                }
            }

            return false;
        }
    }
}
