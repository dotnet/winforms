// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;

namespace System.Windows.Forms;

internal partial class ToolStripSettingsManager
{
    /// <summary>
    ///  Light weight structure that captures the properties we want to save as settings.
    /// </summary>
    private struct SettingsStub
    {
        public bool Visible;
        public string? ToolStripPanelName;
        public Point Location;
        public Size Size;
        public string? ItemOrder;
        public string? Name;

        public SettingsStub(ToolStrip toolStrip)
        {
            ToolStripPanelName = string.Empty;

            if (toolStrip.Parent is ToolStripPanel parentPanel)
            {
                if (!string.IsNullOrEmpty(parentPanel.Name))
                {
                    ToolStripPanelName = parentPanel.Name;
                }
                else if (parentPanel.Parent is ToolStripContainer && !string.IsNullOrEmpty(parentPanel.Parent.Name))
                {
                    // Handle the case when the ToolStripPanel belongs to a ToolStripContainer.
                    ToolStripPanelName = $"{parentPanel.Parent.Name}.{parentPanel.Dock}";
                }

                Debug.Assert(!string.IsNullOrEmpty(ToolStripPanelName), "ToolStrip was parented to a panel, but we couldn't figure out its name.");
            }

            Visible = toolStrip.Visible;
            Size = toolStrip.Size;
            Location = toolStrip.Location;
            Name = toolStrip.Name;
            ItemOrder = GetItemOrder(toolStrip);
        }

        public SettingsStub(ToolStripSettings toolStripSettings)
        {
            ToolStripPanelName = toolStripSettings.ToolStripPanelName;
            Visible = toolStripSettings.Visible;
            Size = toolStripSettings.Size;
            Location = toolStripSettings.Location;
            Name = toolStripSettings.Name;
            ItemOrder = toolStripSettings.ItemOrder;
        }
    }
}
