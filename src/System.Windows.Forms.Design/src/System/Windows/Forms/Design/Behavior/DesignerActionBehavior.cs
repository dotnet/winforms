// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;

namespace System.Windows.Forms.Design.Behavior;

/// <summary>
///  This is the Behavior that represents DesignerActions for a particular control.
///  The DesignerActionBehavior is responsible for responding to the MouseDown message and either
///  1) Selecting the control and changing the DesignerActionGlyph's image or
///  2) Building up a chrome menu and requesting it to be shown.
///  Also, this Behavior acts as a proxy between "clicked" context menu items and the actual
///  DesignerActions that they represent.
/// </summary>
internal sealed class DesignerActionBehavior : Behavior
{
    private readonly IServiceProvider _serviceProvider; // we need to cache the service provider here to be able to create the panel with the proper arguments
    private bool _ignoreNextMouseUp;

    /// <summary>
    ///  Constructor that calls base and caches off the action lists.
    /// </summary>
    internal DesignerActionBehavior(IServiceProvider serviceProvider, IComponent relatedComponent, DesignerActionListCollection actionLists, DesignerActionUI parentUI)
    {
        ActionLists = actionLists;
        _serviceProvider = serviceProvider;
        RelatedComponent = relatedComponent;
        ParentUI = parentUI;
    }

    /// <summary>
    ///  Returns the collection of DesignerActionLists this Behavior is managing.
    ///  These will be dynamically updated (some can be removed, new ones can be added, etc...).
    /// </summary>
    internal DesignerActionListCollection ActionLists { get; set; }

    /// <summary>
    ///  Returns the parenting UI (a DesignerActionUI)
    /// </summary>
    internal DesignerActionUI ParentUI { get; }

    /// <summary>
    ///  Returns the Component that this glyph is attached to.
    /// </summary>
    internal IComponent RelatedComponent { get; }

    /// <summary>
    ///  Hides the designer action panel UI.
    /// </summary>
    internal void HideUI()
    {
        ParentUI.HideDesignerActionPanel();
    }

    internal DesignerActionPanel CreateDesignerActionPanel(IComponent relatedComponent)
    {
        // BUILD AND SHOW THE CHROME UI
        DesignerActionListCollection lists = new();
        lists.AddRange(ActionLists);
        DesignerActionPanel dap = new(_serviceProvider);
        dap.UpdateTasks(lists, [], string.Format(SR.DesignerActionPanel_DefaultPanelTitle, relatedComponent.GetType().Name), null);
        return dap;
    }

    /// <summary>
    ///  Shows the designer action panel UI associated with this glyph.
    /// </summary>
    internal void ShowUI(Glyph g)
    {
        if (g is not DesignerActionGlyph glyph)
        {
            Debug.Fail("Why are we trying to 'showui' on a glyph that's not a DesignerActionGlyph?");
            return;
        }

        DesignerActionPanel dap = CreateDesignerActionPanel(RelatedComponent);
        ParentUI.ShowDesignerActionPanel(RelatedComponent, dap, glyph);
    }

    internal bool IgnoreNextMouseUp
    {
        set
        {
            _ignoreNextMouseUp = value;
        }
    }

    public override bool OnMouseDoubleClick(Glyph? g, MouseButtons button, Point mouseLoc)
    {
        _ignoreNextMouseUp = true;
        return true;
    }

    public override bool OnMouseDown(Glyph? g, MouseButtons button, Point mouseLoc)
    {
        // we take the msg
        return (!ParentUI.IsDesignerActionPanelVisible);
    }

    /// <summary>
    ///  In response to a MouseUp, we will either
    ///  1) select the Glyph and control if not selected, or
    ///  2) Build up our context menu representing our DesignerActions and show it.
    /// </summary>
    public override bool OnMouseUp(Glyph? g, MouseButtons button)
    {
        if (button != MouseButtons.Left)
        {
            return true;
        }

        bool returnValue = true;
        if (ParentUI.IsDesignerActionPanelVisible)
        {
            HideUI();
        }
        else if (!_ignoreNextMouseUp)
        {
            if (_serviceProvider.TryGetService(out ISelectionService? selectionService) &&
                selectionService.PrimarySelection != RelatedComponent)
            {
                List<IComponent> componentList = [RelatedComponent];
                selectionService.SetSelectedComponents(componentList, SelectionTypes.Primary);
            }

            if (g is not null)
            {
                ShowUI(g);
            }
        }
        else
        {
            returnValue = false;
        }

        _ignoreNextMouseUp = false;
        return returnValue;
    }
}
