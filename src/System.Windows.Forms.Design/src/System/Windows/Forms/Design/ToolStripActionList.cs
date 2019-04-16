﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace System.Windows.Forms.Design
{
    internal class ToolStripActionList : DesignerActionList
    {
        private readonly ToolStrip _toolStrip;
        private bool _autoShow = false;
        private readonly ToolStripDesigner _designer;

        private ChangeToolStripParentVerb _changeParentVerb = null;
        private StandardMenuStripVerb _standardItemsVerb = null;

        public ToolStripActionList(ToolStripDesigner designer) : base(designer.Component)
        {
            _toolStrip = (ToolStrip)designer.Component;
            this._designer = designer;

            _changeParentVerb = new ChangeToolStripParentVerb(string.Format(SR.ToolStripDesignerEmbedVerb), designer);
            if (!(_toolStrip is StatusStrip))
            {
                _standardItemsVerb = new StandardMenuStripVerb(designer);
            }
        }

        /// <summary>
        /// False if were inherited and can't be modified.
        /// </summary>
        private bool CanAddItems
        {
            get
            {
                // Make sure the component is not being inherited -- we can't delete these!
                InheritanceAttribute ia = (InheritanceAttribute)TypeDescriptor.GetAttributes(_toolStrip)[typeof(InheritanceAttribute)];
                if (ia == null || ia.InheritanceLevel == InheritanceLevel.NotInherited)
                {
                    return true;
                }
                return false;
            }
        }

        private bool IsReadOnly
        {
            get
            {
                // Make sure the component is not being inherited -- we can't delete these!
                InheritanceAttribute ia = (InheritanceAttribute)TypeDescriptor.GetAttributes(_toolStrip)[typeof(InheritanceAttribute)];
                if (ia == null || ia.InheritanceLevel == InheritanceLevel.InheritedReadOnly)
                {
                    return true;
                }
                return false;
            }
        }

        //helper function to get the property on the actual Control
        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        private object GetProperty(string propertyName)
        {
            PropertyDescriptor getProperty = TypeDescriptor.GetProperties(_toolStrip)[propertyName];
            Debug.Assert(getProperty != null, "Could not find given property in control.");
            if (getProperty != null)
            {
                return getProperty.GetValue(_toolStrip);
            }
            return null;
        }

        //helper function to change the property on the actual Control
        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        private void ChangeProperty(string propertyName, object value)
        {
            PropertyDescriptor changingProperty = TypeDescriptor.GetProperties(_toolStrip)[propertyName];
            Debug.Assert(changingProperty != null, "Could not find given property in control.");
            if (changingProperty != null)
            {
                changingProperty.SetValue(_toolStrip, value);
            }
        }

        /// <summary>
        /// Controls whether the Chrome is Automatically shown on selection
        /// </summary>
        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        public override bool AutoShow
        {
            get => _autoShow;
            set
            {
                if (_autoShow != value)
                {
                    _autoShow = value;
                }
            }
        }

        public DockStyle Dock
        {
            [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
            get => (DockStyle)GetProperty("Dock");
            [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
            set
            {
                if (value != Dock)
                {
                    ChangeProperty("Dock", (object)value);
                }
            }
        }

        public ToolStripRenderMode RenderMode
        {
            [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
            get => (ToolStripRenderMode)GetProperty("RenderMode");
            [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
            set
            {
                if (value != RenderMode)
                {
                    ChangeProperty("RenderMode", (object)value);
                }
            }
        }

        public ToolStripGripStyle GripStyle
        {
            [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
            get => (ToolStripGripStyle)GetProperty("GripStyle");
            [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
            set
            {
                if (value != GripStyle)
                {
                    ChangeProperty("GripStyle", (object)value);
                }
            }
        }

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        private void InvokeEmbedVerb()
        {
            // Hide the Panel...
            DesignerActionUIService actionUIService = (DesignerActionUIService)_toolStrip.Site.GetService(typeof(DesignerActionUIService));
            if (actionUIService != null)
            {
                actionUIService.HideUI(_toolStrip);
            }
            _changeParentVerb.ChangeParent();
        }

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        private void InvokeInsertStandardItemsVerb()
        {
            _standardItemsVerb.InsertItems();
        }

        /// <summary>
        /// The Main method to group the ActionItems and pass it to the Panel.
        /// </summary>
        public override DesignerActionItemCollection GetSortedActionItems()
        {
            DesignerActionItemCollection items = new DesignerActionItemCollection();
            if (!IsReadOnly)
            {
                items.Add(new DesignerActionMethodItem(this, "InvokeEmbedVerb", SR.ToolStripDesignerEmbedVerb, "", SR.ToolStripDesignerEmbedVerbDesc, true));
            }

            if (CanAddItems)
            {
                if (!(_toolStrip is StatusStrip))
                {
                    items.Add(new DesignerActionMethodItem(this, "InvokeInsertStandardItemsVerb", SR.ToolStripDesignerStandardItemsVerb, "", SR.ToolStripDesignerStandardItemsVerbDesc, true));
                }
                items.Add(new DesignerActionPropertyItem("RenderMode", SR.ToolStripActionList_RenderMode, SR.ToolStripActionList_Layout, SR.ToolStripActionList_RenderModeDesc));
            }

            if (!(_toolStrip.Parent is ToolStripPanel))
            {
                items.Add(new DesignerActionPropertyItem("Dock", SR.ToolStripActionList_Dock, SR.ToolStripActionList_Layout, SR.ToolStripActionList_DockDesc));
            }
            if (!(_toolStrip is StatusStrip))
            {
                items.Add(new DesignerActionPropertyItem("GripStyle", SR.ToolStripActionList_GripStyle, SR.ToolStripActionList_Layout, SR.ToolStripActionList_GripStyleDesc));
            }
            return items;
        }
    }
}
