// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms {
    using System;
    using System.Configuration;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis; 
    using System.Drawing;
    using System.Text;
    using System.Text.RegularExpressions;

    /// <include file='doc\ToolStripSettings.uex' path='docs/doc[@for="ToolStripSettings"]/*' />
    /// <devdoc> 
    ///     A settings class used by the ToolStripManager to save toolstrip settings.
    /// </devdoc>
    internal class ToolStripSettings : ApplicationSettingsBase {

        internal ToolStripSettings(string settingsKey) : base(settingsKey) {}
        
        [UserScopedSetting]
        [DefaultSettingValue("true")]
        public bool IsDefault {
            get {
                return (bool) this["IsDefault"]; 
            }
            set {
                this["IsDefault"] = value;
            }
        }
        
        [UserScopedSetting]
        public string ItemOrder {
            get {
                return this["ItemOrder"] as string;
            }
            set {
                this["ItemOrder"] = value;
            }
        }
        
        [UserScopedSetting]
        public string Name {
            get {
                return this["Name"] as string;
            }
            set {
                this["Name"] = value;
            }
        }

        [UserScopedSetting]
        [DefaultSettingValue("0,0")]
        public Point Location {
            get {
                return (Point) this["Location"];
            }
            set {
                this["Location"] = value;
            }
        }

        [UserScopedSetting]
        [DefaultSettingValue("0,0")]
        public Size Size {
            get {
                return (Size) this["Size"];
            }
            set {
                this["Size"] = value;
            }
        }

        [UserScopedSetting]
        public string ToolStripPanelName {
            get {
                return this["ToolStripPanelName"] as string;
            }
            set {
                this["ToolStripPanelName"] = value;
            }
        }
        
        [UserScopedSetting]
        [DefaultSettingValue("true")]
        public bool Visible {
            get {
                return (bool) this["Visible"];
            }
            set {
                this["Visible"] = value;
            }
        }
        
        
        public override void Save() {
            this.IsDefault = false;
            base.Save();
        }
    }

    /// <include file='doc\ToolStripSettings.uex' path='docs/doc[@for="ToolStripSettings"]/*' />
    /// <devdoc> 
    ///     Helper class used by ToolStripManager that implements most of the logic to save out and apply
    ///     settings for toolstrips on a form.
    /// </devdoc>
    internal class ToolStripSettingsManager {
        private Form form;
        private string formKey;
        
        internal ToolStripSettingsManager(Form owner, string formKey) {
            this.form = owner;
            this.formKey = formKey;
        }

        internal void Load() {
            ArrayList savedToolStripSettingsObjects = new ArrayList();

            foreach (ToolStrip toolStrip in FindToolStrips(true, form.Controls)) {
                if (toolStrip != null && !string.IsNullOrEmpty(toolStrip.Name)) {
                    ToolStripSettings toolStripSettings = new ToolStripSettings(GetSettingsKey(toolStrip));
                    
                    // Check if we have settings saved out for this toolstrip. If so, add it to our apply list.
                    if (!toolStripSettings.IsDefault) {
                        savedToolStripSettingsObjects.Add(new SettingsStub(toolStripSettings));
                    }
                }
            }

            ApplySettings(savedToolStripSettingsObjects);
        }
        
        internal void Save() {
            foreach (ToolStrip toolStrip in FindToolStrips(true, form.Controls)) {
                if (toolStrip != null && !string.IsNullOrEmpty(toolStrip.Name)) {
                    ToolStripSettings toolStripSettings = new ToolStripSettings(GetSettingsKey(toolStrip));
                    SettingsStub stub = new SettingsStub(toolStrip);

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

        internal static string GetItemOrder(ToolStrip toolStrip) {
            StringBuilder itemNames = new StringBuilder(toolStrip.Items.Count);

            for (int i = 0; i < toolStrip.Items.Count; i++) {
                itemNames.Append((toolStrip.Items[i].Name == null) ? "null" : toolStrip.Items[i].Name);
                if (i != toolStrip.Items.Count - 1) {
                    itemNames.Append(",");
                }
            }
            return itemNames.ToString();
        }

        private void ApplySettings(ArrayList toolStripSettingsToApply) {
            if (toolStripSettingsToApply.Count == 0) {
                return;
            }

            SuspendAllLayout(form);
        
        
            // iterate through all the toolstrips and build up a hash of where the items
            // are right now.
            Dictionary<string, ToolStrip> itemLocationHash = BuildItemOriginationHash();
           
            // build up a hash of where we want the ToolStrips to go
            Dictionary<object, List<SettingsStub>> toolStripPanelDestinationHash = new Dictionary<object, List<SettingsStub>>();
            
            foreach (SettingsStub toolStripSettings in toolStripSettingsToApply) {
                object destinationPanel = !string.IsNullOrEmpty(toolStripSettings.ToolStripPanelName) ? toolStripSettings.ToolStripPanelName : null;

                if (destinationPanel == null) {
                    // Not in a panel.
                    if (!string.IsNullOrEmpty(toolStripSettings.Name)) {
                        // apply the toolstrip settings.
                        ToolStrip toolStrip = ToolStripManager.FindToolStrip(form, toolStripSettings.Name);
                        ApplyToolStripSettings(toolStrip, toolStripSettings, itemLocationHash);
                    }
                }
                else {
                    // This toolStrip is in a ToolStripPanel. We will process it below.
                    if (!toolStripPanelDestinationHash.ContainsKey(destinationPanel)) {
                        toolStripPanelDestinationHash[destinationPanel] = new List<SettingsStub>();
                    }
            
                    toolStripPanelDestinationHash[destinationPanel].Add(toolStripSettings);
                }
            }
            
            // build up a list of the toolstrippanels to party on
            ArrayList toolStripPanels = FindToolStripPanels(true, form.Controls);
        
            foreach (ToolStripPanel toolStripPanel in toolStripPanels) {
                // set all the controls to visible false/
                foreach (Control c in toolStripPanel.Controls) {
                    c.Visible = false;
                }
                
                string toolStripPanelName = toolStripPanel.Name;

                // Handle the ToolStripPanels inside a ToolStripContainer
                if (String.IsNullOrEmpty(toolStripPanelName) && toolStripPanel.Parent is ToolStripContainer && !String.IsNullOrEmpty(toolStripPanel.Parent.Name)) {
                    toolStripPanelName = toolStripPanel.Parent.Name + "." + toolStripPanel.Dock.ToString();
                }
                toolStripPanel.BeginInit();
                // get the associated toolstrips for this panel
                if (toolStripPanelDestinationHash.ContainsKey(toolStripPanelName)) {
                    List<SettingsStub> stubSettings = toolStripPanelDestinationHash[toolStripPanelName];
        
                    if (stubSettings != null) {
                        foreach (SettingsStub settings in stubSettings) {
                            if (!string.IsNullOrEmpty(settings.Name)) {
                                // apply the toolstrip settings.
                                ToolStrip toolStrip = ToolStripManager.FindToolStrip(form, settings.Name);
                                ApplyToolStripSettings(toolStrip, settings, itemLocationHash);
                                toolStripPanel.Join(toolStrip, settings.Location);
                            }
                        }
                    }
                }
                toolStripPanel.EndInit();
            }

            ResumeAllLayout(form, true);
        }

        private void ApplyToolStripSettings(ToolStrip toolStrip, SettingsStub settings, Dictionary<string, ToolStrip> itemLocationHash) {
            if (toolStrip != null) {
                toolStrip.Visible = settings.Visible;
                toolStrip.Size = settings.Size;
                
                // Apply the item order changes.
                string itemNames = settings.ItemOrder;
                if (!string.IsNullOrEmpty(itemNames)) {
                    string[] keys = itemNames.Split(',');
                    Regex r = new Regex("(\\S+)");
            
                    // Shuffle items according to string.
                    for (int i = 0; ((i < toolStrip.Items.Count) && (i < keys.Length)); i++) {
                        Match match = r.Match(keys[i]);
                        if (match != null && match.Success) {
                            string key = match.Value;
                            if (!string.IsNullOrEmpty(key) && itemLocationHash.ContainsKey(key)) {
                                toolStrip.Items.Insert(i, itemLocationHash[key].Items[key]);
                            }
                        }
                    }
                }
            }
        }

        private Dictionary<string, ToolStrip> BuildItemOriginationHash() {
           ArrayList toolStrips = FindToolStrips(true, form.Controls);
           Dictionary<string, ToolStrip> itemLocationHash = new Dictionary<string, ToolStrip>();
        
           if (toolStrips != null) {
               foreach (ToolStrip toolStrip in toolStrips) {
                   foreach (ToolStripItem item in toolStrip.Items) {
                       if (!string.IsNullOrEmpty(item.Name)) {
                           Debug.Assert(!itemLocationHash.ContainsKey(item.Name), "WARNING: ToolStripItem name not unique.");
                           itemLocationHash[item.Name] = toolStrip;
                       }
                   }
               }
           }

           return itemLocationHash;
        }

        [
            SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")
        ]
        private ArrayList FindControls(Type baseType, bool searchAllChildren, Control.ControlCollection controlsToLookIn, ArrayList foundControls) {
            if ((controlsToLookIn == null) || (foundControls == null)) {
                return null;
            }
        
            try {
                // Perform breadth first search - as it's likely people will want controls belonging
                // to the same parent close to each other.
        
                for (int i = 0; i < controlsToLookIn.Count; i++) {
                    if (controlsToLookIn[i] == null) {
                        continue;
                    }
        
                    if (baseType.IsAssignableFrom(controlsToLookIn[i].GetType())) {
                        foundControls.Add(controlsToLookIn[i]);
                    }
                }
        
                // Optional recurive search for controls in child collections.
        
                if (searchAllChildren) {
                    for (int i = 0; i < controlsToLookIn.Count; i++) {
                        if (controlsToLookIn[i] == null || controlsToLookIn[i] is Form) {
                            continue;
                        }
                        if ((controlsToLookIn[i].Controls != null) && controlsToLookIn[i].Controls.Count > 0) {
                            // if it has a valid child collecion, append those results to our collection
                            foundControls = FindControls(baseType, searchAllChildren, controlsToLookIn[i].Controls, foundControls);
                        }
                    }
                }
            }
            catch (Exception e) {
                if (ClientUtils.IsCriticalException(e)) {
                    throw;
                }
            }
            return foundControls;
        }

        private ArrayList FindToolStripPanels(bool searchAllChildren, Control.ControlCollection controlsToLookIn) {
            return FindControls(typeof(ToolStripPanel), true, form.Controls, new ArrayList());
        }
        
        private ArrayList FindToolStrips(bool searchAllChildren, Control.ControlCollection controlsToLookIn) {
            return FindControls(typeof(ToolStrip), true, form.Controls, new ArrayList());
        }
        
        private string GetSettingsKey(ToolStrip toolStrip) {
            if (toolStrip != null) {
                return formKey + "." + toolStrip.Name;
            }

            return String.Empty;
        }

        private void ResumeAllLayout(Control start, bool performLayout) {
            Control.ControlCollection controlsCollection = start.Controls;
        
            for (int i = 0; i < controlsCollection.Count; i++) {
                ResumeAllLayout(controlsCollection[i], performLayout);
            }
        
            start.ResumeLayout(performLayout);
        }
        
        private void SuspendAllLayout(Control start) {
            start.SuspendLayout();
        
            Control.ControlCollection controlsCollection = start.Controls;
            for (int i = 0; i < controlsCollection.Count; i++) {
                SuspendAllLayout(controlsCollection[i]);
            }
        }

        /// <devdoc> 
        ///     Light weight structure that captures the properties we want to save as settings.
        /// </devdoc>
        private struct SettingsStub {
            public bool     Visible;
            public string   ToolStripPanelName;
            public Point    Location;
            public Size     Size;
            public string   ItemOrder;
            public string   Name;

            public SettingsStub(ToolStrip toolStrip) {
                this.ToolStripPanelName = String.Empty;
                ToolStripPanel parentPanel = toolStrip.Parent as ToolStripPanel;

                if (parentPanel != null) {
                    if (!String.IsNullOrEmpty(parentPanel.Name)) {
                        this.ToolStripPanelName = parentPanel.Name;
                    }
                    else if (parentPanel.Parent is ToolStripContainer && !String.IsNullOrEmpty(parentPanel.Parent.Name)) {
                        // Handle the case when the ToolStripPanel belongs to a ToolStripContainer.
                        this.ToolStripPanelName = parentPanel.Parent.Name + "." + parentPanel.Dock.ToString();
                    }

                    Debug.Assert(!String.IsNullOrEmpty(this.ToolStripPanelName), "ToolStrip was parented to a panel, but we couldn't figure out its name.");
                }

                this.Visible = toolStrip.Visible;
                this.Size = toolStrip.Size;
                this.Location = toolStrip.Location;
                this.Name = toolStrip.Name;
                this.ItemOrder = GetItemOrder(toolStrip);
               
            }
            public SettingsStub(ToolStripSettings toolStripSettings) {
                this.ToolStripPanelName = toolStripSettings.ToolStripPanelName;
                this.Visible = toolStripSettings.Visible;
                this.Size = toolStripSettings.Size;
                this.Location = toolStripSettings.Location;
                this.Name = toolStripSettings.Name;
                this.ItemOrder = toolStripSettings.ItemOrder;
            }
        }
    }
}
