// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Text;
using System.Text.RegularExpressions;

namespace System.Windows.Forms;

/// <summary>
///  Helper class used by ToolStripManager that implements most of the logic to save out and apply
///  settings for toolstrips on a form.
/// </summary>
internal partial class ToolStripSettingsManager
{
    private readonly Form _form;
    private readonly string _formKey;

    internal ToolStripSettingsManager(Form owner, string formKey)
    {
        _form = owner;
        _formKey = formKey;
    }

    internal void Load()
    {
        List<SettingsStub> savedToolStripSettingsObjects = [];

        List<ToolStrip> toolStripControls = [];
        FindControls(true, _form.Controls, toolStripControls);

        foreach (ToolStrip toolStrip in toolStripControls)
        {
            if (!string.IsNullOrEmpty(toolStrip.Name))
            {
                ToolStripSettings toolStripSettings = new(GetSettingsKey(toolStrip));

                // Check if we have settings saved out for this toolstrip. If so, add it to our apply list.
                if (!toolStripSettings.IsDefault)
                {
                    savedToolStripSettingsObjects.Add(new SettingsStub(toolStripSettings));
                }
            }
        }

        ApplySettings(savedToolStripSettingsObjects);
    }

    internal void Save()
    {
        List<ToolStrip> toolStripControls = [];
        FindControls(true, _form.Controls, toolStripControls);

        foreach (ToolStrip toolStrip in toolStripControls)
        {
            if (!string.IsNullOrEmpty(toolStrip.Name))
            {
                ToolStripSettings toolStripSettings = new(GetSettingsKey(toolStrip));
                SettingsStub stub = new(toolStrip);

                toolStripSettings.ItemOrder = stub.ItemOrder;
                toolStripSettings.Name = stub.Name;
                toolStripSettings.Location = stub.Location;
                toolStripSettings.Size = stub.Size;
                toolStripSettings.ToolStripPanelName = stub.ToolStripPanelName;
                toolStripSettings.Visible = stub.Visible;

                toolStripSettings.Save();
            }
        }
    }

    internal static string GetItemOrder(ToolStrip toolStrip)
    {
        if (toolStrip.Items.Count == 0)
        {
            return string.Empty;
        }

        StringBuilder itemNames = new(toolStrip.Items.Count);

        for (int i = 0; i < toolStrip.Items.Count; i++)
        {
            itemNames.Append(toolStrip.Items[i].Name ?? "null");
            itemNames.Append(',');
        }

        return itemNames.ToString(0, itemNames.Length - 1);
    }

    private void ApplySettings(List<SettingsStub> toolStripSettingsToApply)
    {
        if (toolStripSettingsToApply.Count == 0)
        {
            return;
        }

        SuspendAllLayout(_form);

        // iterate through all the toolstrips and build up a hash of where the items
        // are right now.
        Dictionary<string, ToolStrip> itemLocationHash = BuildItemOriginationHash();

        // build up a hash of where we want the ToolStrips to go
        Dictionary<object, List<SettingsStub>> toolStripPanelDestinationHash = [];

        foreach (SettingsStub toolStripSettings in toolStripSettingsToApply)
        {
            object? destinationPanel = !string.IsNullOrEmpty(toolStripSettings.ToolStripPanelName) ? toolStripSettings.ToolStripPanelName : null;

            if (destinationPanel is null)
            {
                // Not in a panel.
                if (!string.IsNullOrEmpty(toolStripSettings.Name))
                {
                    // apply the toolstrip settings.
                    ToolStrip? toolStrip = ToolStripManager.FindToolStrip(_form, toolStripSettings.Name);
                    ApplyToolStripSettings(toolStrip, toolStripSettings, itemLocationHash);
                }
            }
            else
            {
                // This toolStrip is in a ToolStripPanel. We will process it below.
                if (!toolStripPanelDestinationHash.TryGetValue(destinationPanel, out List<SettingsStub>? value))
                {
                    value = [];
                    toolStripPanelDestinationHash[destinationPanel] = value;
                }

                value.Add(toolStripSettings);
            }
        }

        // Build up a list of the toolstrippanels to party on.
        List<ToolStripPanel> toolStripPanels = [];
        FindControls(true, _form.Controls, toolStripPanels);
        foreach (ToolStripPanel toolStripPanel in toolStripPanels)
        {
            // Set all the controls to visible false.
            foreach (Control c in toolStripPanel.Controls)
            {
                c.Visible = false;
            }

            string toolStripPanelName = toolStripPanel.Name;

            // Handle the ToolStripPanels inside a ToolStripContainer
            if (string.IsNullOrEmpty(toolStripPanelName) && toolStripPanel.Parent is ToolStripContainer && !string.IsNullOrEmpty(toolStripPanel.Parent.Name))
            {
                toolStripPanelName = $"{toolStripPanel.Parent.Name}.{toolStripPanel.Dock}";
            }

            toolStripPanel.BeginInit();
            // get the associated toolstrips for this panel
            if (toolStripPanelDestinationHash.TryGetValue(toolStripPanelName, out List<SettingsStub>? stubSettings))
            {
                foreach (SettingsStub settings in stubSettings)
                {
                    if (!string.IsNullOrEmpty(settings.Name))
                    {
                        // apply the toolstrip settings.
                        ToolStrip? toolStrip = ToolStripManager.FindToolStrip(_form, settings.Name);
                        ApplyToolStripSettings(toolStrip, settings, itemLocationHash);
                        toolStripPanel.Join(toolStrip!, settings.Location);
                    }
                }
            }

            toolStripPanel.EndInit();
        }

        ResumeAllLayout(_form, true);
    }

    private static void ApplyToolStripSettings(ToolStrip? toolStrip, SettingsStub settings, Dictionary<string, ToolStrip> itemLocationHash)
    {
        if (toolStrip is null)
        {
            return;
        }

        toolStrip.Visible = settings.Visible;
        toolStrip.Size = settings.Size;

        // Apply the item order changes.
        string? itemNames = settings.ItemOrder;
        if (!string.IsNullOrEmpty(itemNames))
        {
            string[] keys = itemNames.Split(',');
            Regex r = ContiguousNonWhitespace();

            // Shuffle items according to string.
            for (int i = 0; ((i < toolStrip.Items.Count) && (i < keys.Length)); i++)
            {
                Match match = r.Match(keys[i]);
                if (match.Success)
                {
                    string key = match.Value;
                    if (!string.IsNullOrEmpty(key) && itemLocationHash.TryGetValue(key, out ToolStrip? value))
                    {
                        toolStrip.Items.Insert(i, value.Items[key]!);
                    }
                }
            }
        }
    }

    [GeneratedRegex("\\S+")]
    private static partial Regex ContiguousNonWhitespace();

    private Dictionary<string, ToolStrip> BuildItemOriginationHash()
    {
        Dictionary<string, ToolStrip> itemLocationHash = [];

        List<ToolStrip> toolStripControls = [];
        FindControls(true, _form.Controls, toolStripControls);

        foreach (ToolStrip toolStrip in toolStripControls)
        {
            foreach (ToolStripItem item in toolStrip.Items)
            {
                if (!string.IsNullOrEmpty(item.Name))
                {
                    Debug.Assert(!itemLocationHash.ContainsKey(item.Name), "WARNING: ToolStripItem name not unique.");
                    itemLocationHash[item.Name] = toolStrip;
                }
            }
        }

        return itemLocationHash;
    }

    private void FindControls<T>(bool searchAllChildren, Control.ControlCollection controlsToLookIn, List<T> foundControls)
        where T : Control
    {
        try
        {
            // Perform breadth first search - as it's likely people will want controls belonging
            // to the same parent close to each other.
            for (int i = 0; i < controlsToLookIn.Count; i++)
            {
                if (controlsToLookIn[i] is null)
                {
                    continue;
                }

                if (controlsToLookIn[i] is T control)
                {
                    foundControls.Add(control);
                }
            }

            // Optional recursive search for controls in child collections.
            if (searchAllChildren)
            {
                for (int i = 0; i < controlsToLookIn.Count; i++)
                {
                    if (controlsToLookIn[i] is null or Form)
                    {
                        continue;
                    }

                    if ((controlsToLookIn[i].Controls is not null) && controlsToLookIn[i].Controls.Count > 0)
                    {
                        // If it has a valid child collection, append those results to our collection.
                        FindControls(searchAllChildren, controlsToLookIn[i].Controls, foundControls);
                    }
                }
            }
        }
        catch (Exception e) when (!e.IsCriticalException())
        {
        }
    }

    private string GetSettingsKey(ToolStrip toolStrip)
    {
        if (toolStrip is not null)
        {
            return $"{_formKey}.{toolStrip.Name}";
        }

        return string.Empty;
    }

    private static void ResumeAllLayout(Control start, bool performLayout)
    {
        Control.ControlCollection controlsCollection = start.Controls;

        for (int i = 0; i < controlsCollection.Count; i++)
        {
            ResumeAllLayout(controlsCollection[i], performLayout);
        }

        start.ResumeLayout(performLayout);
    }

    private static void SuspendAllLayout(Control start)
    {
        start.SuspendLayout();

        Control.ControlCollection controlsCollection = start.Controls;
        for (int i = 0; i < controlsCollection.Count; i++)
        {
            SuspendAllLayout(controlsCollection[i]);
        }
    }
}
