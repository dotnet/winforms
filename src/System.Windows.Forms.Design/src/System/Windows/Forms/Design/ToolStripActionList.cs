// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.ComponentModel.Design;

namespace System.Windows.Forms.Design;

internal class ToolStripActionList : DesignerActionList
{
    private readonly ToolStrip _toolStrip;
    private bool _autoShow;

    private readonly ChangeToolStripParentVerb _changeParentVerb;
    private readonly StandardMenuStripVerb? _standardItemsVerb;

    public ToolStripActionList(ToolStripDesigner designer)
        : base(designer.Component)
    {
        _toolStrip = (ToolStrip)designer.Component;

        _changeParentVerb = new ChangeToolStripParentVerb(designer);
        if (_toolStrip is not StatusStrip)
        {
            _standardItemsVerb = new StandardMenuStripVerb(designer);
        }
    }

    /// <summary>
    ///  False if were inherited and can't be modified.
    /// </summary>
    private bool CanAddItems
    {
        get
        {
            // Make sure the component is not being inherited -- we can't delete these!
            if (!TypeDescriptorHelper.TryGetAttribute(_toolStrip, out InheritanceAttribute? ia) || ia.InheritanceLevel == InheritanceLevel.NotInherited)
            {
                return true;
            }

            return false;
        }
    }

    private bool IsReadOnly =>
        // Make sure the component is not being inherited -- we can't delete these!
        !TypeDescriptorHelper.TryGetAttribute(_toolStrip, out InheritanceAttribute? ia)
        || ia.InheritanceLevel == InheritanceLevel.InheritedReadOnly;

    // helper function to get the property on the actual Control
    private object? GetProperty(string propertyName)
    {
        PropertyDescriptor? getProperty = TypeDescriptor.GetProperties(_toolStrip)[propertyName];
        Debug.Assert(getProperty is not null, "Could not find given property in control.");
        return getProperty?.GetValue(_toolStrip);
    }

    // helper function to change the property on the actual Control
    private void ChangeProperty(string propertyName, object value)
    {
        PropertyDescriptor? changingProperty = TypeDescriptor.GetProperties(_toolStrip)[propertyName];
        Debug.Assert(changingProperty is not null, "Could not find given property in control.");
        changingProperty?.SetValue(_toolStrip, value);
    }

    /// <summary>
    ///  Controls whether the Chrome is Automatically shown on selection
    /// </summary>
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
        get => (DockStyle)GetProperty(nameof(Dock))!;
        set
        {
            if (value != Dock)
            {
                ChangeProperty(nameof(Dock), value);
            }
        }
    }

    public ToolStripRenderMode RenderMode
    {
        get => (ToolStripRenderMode)GetProperty(nameof(RenderMode))!;
        set
        {
            if (value != RenderMode)
            {
                ChangeProperty(nameof(RenderMode), value);
            }
        }
    }

    public ToolStripGripStyle GripStyle
    {
        get => (ToolStripGripStyle)GetProperty(nameof(GripStyle))!;
        set
        {
            if (value != GripStyle)
            {
                ChangeProperty(nameof(GripStyle), value);
            }
        }
    }

    private void InvokeEmbedVerb()
    {
        // Hide the Panel.
        DesignerActionUIService? actionUIService = (DesignerActionUIService?)_toolStrip.Site?.GetService(typeof(DesignerActionUIService));
        actionUIService?.HideUI(_toolStrip);
        _changeParentVerb.ChangeParent();
    }

    private void InvokeInsertStandardItemsVerb()
    {
        _standardItemsVerb?.InsertItems();
    }

    /// <summary>
    ///  The Main method to group the ActionItems and pass it to the Panel.
    /// </summary>
    public override DesignerActionItemCollection GetSortedActionItems()
    {
        DesignerActionItemCollection items = [];
        if (!IsReadOnly)
        {
            items.Add(new DesignerActionMethodItem(
                this,
                nameof(InvokeEmbedVerb),
                SR.ToolStripDesignerEmbedVerb,
                "",
                SR.ToolStripDesignerEmbedVerbDesc,
                includeAsDesignerVerb: true));
        }

        if (CanAddItems)
        {
            if (_toolStrip is not StatusStrip)
            {
                items.Add(new DesignerActionMethodItem(
                    this,
                    nameof(InvokeInsertStandardItemsVerb),
                    SR.ToolStripDesignerStandardItemsVerb,
                    "",
                    SR.ToolStripDesignerStandardItemsVerbDesc,
                    true));
            }

            items.Add(new DesignerActionPropertyItem(
                nameof(RenderMode),
                SR.ToolStripActionList_RenderMode,
                SR.ToolStripActionList_Layout,
                SR.ToolStripActionList_RenderModeDesc));
        }

        if (_toolStrip.Parent is not ToolStripPanel)
        {
            items.Add(new DesignerActionPropertyItem(
                nameof(Dock),
                SR.ToolStripActionList_Dock,
                SR.ToolStripActionList_Layout,
                SR.ToolStripActionList_DockDesc));
        }

        if (_toolStrip is not StatusStrip)
        {
            items.Add(new DesignerActionPropertyItem(
                nameof(GripStyle),
                SR.ToolStripActionList_GripStyle,
                SR.ToolStripActionList_Layout,
                SR.ToolStripActionList_GripStyleDesc));
        }

        return items;
    }
}
