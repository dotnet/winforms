// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.ComponentModel.Design;

namespace System.Windows.Forms.Design;

internal class ContextMenuStripActionList : DesignerActionList
{
    private readonly ToolStripDropDown _toolStripDropDown;
    private bool _autoShow;

    public ContextMenuStripActionList(ToolStripDropDownDesigner designer)
        : base(designer.Component)
    {
        _toolStripDropDown = (ToolStripDropDown)designer.Component;
    }

    // helper function to get the property on the actual Control
    private object? GetProperty(string propertyName)
    {
        PropertyDescriptor? getProperty = TypeDescriptor.GetProperties(_toolStripDropDown)[propertyName];
        Debug.Assert(getProperty is not null, "Could not find given property in control.");
        if (getProperty is not null)
        {
            return getProperty.GetValue(_toolStripDropDown);
        }

        return null;
    }

    // helper function to change the property on the actual Control
    private void ChangeProperty(string propertyName, object value)
    {
        PropertyDescriptor? changingProperty = TypeDescriptor.GetProperties(_toolStripDropDown)[propertyName];
        Debug.Assert(changingProperty is not null, "Could not find given property in control.");
        changingProperty?.SetValue(_toolStripDropDown, value);
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

    public bool ShowImageMargin
    {
        get => (bool)GetProperty(nameof(ShowImageMargin))!;
        set
        {
            if (value != ShowImageMargin)
            {
                ChangeProperty(nameof(ShowImageMargin), value);
            }
        }
    }

    public bool ShowCheckMargin
    {
        get => (bool)GetProperty(nameof(ShowCheckMargin))!;
        set
        {
            if (value != ShowCheckMargin)
            {
                ChangeProperty(nameof(ShowCheckMargin), value);
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

    /// <summary>
    ///  The Main method to group the ActionItems and pass it to the Panel.
    /// </summary>
    public override DesignerActionItemCollection GetSortedActionItems()
    {
        DesignerActionItemCollection items =
        [
            new DesignerActionPropertyItem(
                nameof(RenderMode),
                SR.ToolStripActionList_RenderMode,
                SR.ToolStripActionList_Layout,
                SR.ToolStripActionList_RenderModeDesc)
        ];

        if (_toolStripDropDown is ToolStripDropDownMenu)
        {
            items.Add(new DesignerActionPropertyItem(
                nameof(ShowImageMargin),
                SR.ContextMenuStripActionList_ShowImageMargin,
                SR.ToolStripActionList_Layout,
                SR.ContextMenuStripActionList_ShowImageMarginDesc));

            items.Add(new DesignerActionPropertyItem(
                nameof(ShowCheckMargin),
                SR.ContextMenuStripActionList_ShowCheckMargin,
                SR.ToolStripActionList_Layout,
                SR.ContextMenuStripActionList_ShowCheckMarginDesc));
        }

        return items;
    }
}
