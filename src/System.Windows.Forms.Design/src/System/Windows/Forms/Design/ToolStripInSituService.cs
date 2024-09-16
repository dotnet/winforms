// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable disable

using System.ComponentModel;
using System.ComponentModel.Design;

namespace System.Windows.Forms.Design;

/// <summary>
///  This class implements the ISupportInSituService which enables some designers to go into
///  InSitu Editing when Keys are pressed while the Component is Selected.
/// </summary>
internal class ToolStripInSituService : ISupportInSituService, IDisposable
{
    private readonly IServiceProvider _sp;
    private readonly IDesignerHost _designerHost;
    private IComponentChangeService _componentChangeService;
    private ToolStripDesigner _toolDesigner;
    private ToolStripItemDesigner _toolItemDesigner;
    private ToolStripKeyboardHandlingService _toolStripKeyBoardService;

    /// <summary>
    ///  The constructor for this class which takes the serviceprovider used to get the selectionservice.
    ///  This ToolStripInSituService is ToolStrip specific.
    /// </summary>
    public ToolStripInSituService(IServiceProvider provider)
    {
        _sp = provider;
        _designerHost = (IDesignerHost)provider.GetService(typeof(IDesignerHost));
        Debug.Assert(_designerHost is not null, "ToolStripKeyboardHandlingService relies on the selection service, which is unavailable.");
        _designerHost?.AddService<ISupportInSituService>(this);

        _componentChangeService = (IComponentChangeService)_designerHost.GetService(typeof(IComponentChangeService));
        Debug.Assert(_componentChangeService is not null, "ToolStripKeyboardHandlingService relies on the componentChange service, which is unavailable.");
        if (_componentChangeService is not null)
        {
            _componentChangeService.ComponentRemoved += OnComponentRemoved;
        }
    }

    /// <summary>
    ///  Disposes of this object, removing all commands from the menu service.
    /// </summary>
    public void Dispose()
    {
        if (_toolDesigner is not null)
        {
            _toolDesigner.Dispose();
            _toolDesigner = null;
        }

        if (_toolItemDesigner is not null)
        {
            _toolItemDesigner.Dispose();
            _toolItemDesigner = null;
        }

        if (_componentChangeService is not null)
        {
            _componentChangeService.ComponentRemoved -= OnComponentRemoved;
            _componentChangeService = null;
        }
    }

    private ToolStripKeyboardHandlingService ToolStripKeyBoardService
    {
        get
        {
            _toolStripKeyBoardService ??= (ToolStripKeyboardHandlingService)_sp.GetService(typeof(ToolStripKeyboardHandlingService));

            return _toolStripKeyBoardService;
        }
    }

    /// <summary>
    ///  Returning true for IgnoreMessages means that this service is interested in getting the KeyBoard characters.
    /// </summary>
    public bool IgnoreMessages
    {
        get
        {
            ISelectionService selectionService = (ISelectionService)_sp.GetService(typeof(ISelectionService));
            IDesignerHost host = (IDesignerHost)_sp.GetService(typeof(IDesignerHost));
            if (selectionService is not null && host is not null)
            {
                if (selectionService.PrimarySelection is not IComponent comp)
                {
                    comp = (IComponent)ToolStripKeyBoardService.SelectedDesignerControl;
                }

                if (comp is not null)
                {
                    if (comp is DesignerToolStripControlHost c)
                    {
                        if (c.GetCurrentParent() is ToolStripDropDown dropDown)
                        {
                            if (dropDown.OwnerItem is ToolStripDropDownItem parentItem)
                            {
                                if (parentItem is ToolStripOverflowButton)
                                {
                                    return false;
                                }
                                else
                                {
                                    _toolItemDesigner = host.GetDesigner(parentItem) as ToolStripMenuItemDesigner;
                                    if (_toolItemDesigner is not null)
                                    {
                                        _toolDesigner = null;
                                        return true;
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (c.GetCurrentParent() is MenuStrip tool)
                            {
                                _toolDesigner = host.GetDesigner(tool) as ToolStripDesigner;
                                if (_toolDesigner is not null)
                                {
                                    _toolItemDesigner = null;
                                    return true;
                                }
                            }
                        }
                    }
                    else if (comp is ToolStripDropDown) // case for ToolStripDropDown..
                    {
                        if (host.GetDesigner(comp) is ToolStripDropDownDesigner designer)
                        {
                            ToolStripMenuItem toolItem = designer.DesignerMenuItem;
                            if (toolItem is not null)
                            {
                                _toolItemDesigner = host.GetDesigner(toolItem) as ToolStripItemDesigner;
                                if (_toolItemDesigner is not null)
                                {
                                    _toolDesigner = null;
                                    return true;
                                }
                            }
                        }
                    }
                    else if (comp is MenuStrip)
                    {
                        _toolDesigner = host.GetDesigner(comp) as ToolStripDesigner;
                        if (_toolDesigner is not null)
                        {
                            _toolItemDesigner = null;
                            return true;
                        }
                    }
                    else if (comp is ToolStripMenuItem)
                    {
                        _toolItemDesigner = host.GetDesigner(comp) as ToolStripItemDesigner;
                        if (_toolItemDesigner is not null)
                        {
                            _toolDesigner = null;
                            return true;
                        }
                    }
                }
            }

            return false;
        }
    }

    /// <summary>
    ///  This function is called on the service when the PBRSFORWARD gets the first WM_CHAR message.
    /// </summary>
    public void HandleKeyChar()
    {
        if (_toolDesigner is not null || _toolItemDesigner is not null)
        {
            if (_toolDesigner is not null)
            {
                _toolDesigner.ShowEditNode(false);
            }
            else if (_toolItemDesigner is not null)
            {
                if (_toolItemDesigner is ToolStripMenuItemDesigner menuDesigner)
                {
                    ISelectionService selService = (ISelectionService)_sp.GetService(typeof(ISelectionService));
                    if (selService is not null)
                    {
                        object comp = selService.PrimarySelection;
                        comp ??= ToolStripKeyBoardService.SelectedDesignerControl;

                        DesignerToolStripControlHost designerItem = comp as DesignerToolStripControlHost;
                        if (designerItem is not null || comp is ToolStripDropDown)
                        {
                            menuDesigner.EditTemplateNode(false);
                        }
                        else
                        {
                            menuDesigner.ShowEditNode(false);
                        }
                    }
                }
                else
                {
                    _toolItemDesigner.ShowEditNode(false);
                }
            }
        }
    }

    /// <summary>
    ///  This function returns the Window handle that should get all the Keyboard messages.
    /// </summary>
    public IntPtr GetEditWindow()
    {
        IntPtr hWnd = IntPtr.Zero;
        if (_toolDesigner is not null && _toolDesigner.Editor is not null && _toolDesigner.Editor.EditBox is not null)
        {
            hWnd = (_toolDesigner.Editor.EditBox.Visible) ? _toolDesigner.Editor.EditBox.Handle : hWnd;
        }
        else if (_toolItemDesigner is not null && _toolItemDesigner.Editor is not null && _toolItemDesigner.Editor.EditBox is not null)
        {
            hWnd = (_toolItemDesigner.Editor.EditBox.Visible) ? _toolItemDesigner.Editor.EditBox.Handle : hWnd;
        }

        return hWnd;
    }

    // Remove the Service when the last toolStrip is removed.
    private void OnComponentRemoved(object sender, ComponentEventArgs e)
    {
        bool toolStripPresent = false;
        ComponentCollection comps = _designerHost.Container.Components;
        foreach (IComponent comp in comps)
        {
            if (comp is ToolStrip)
            {
                toolStripPresent = true;
                break;
            }
        }

        if (!toolStripPresent)
        {
            ToolStripInSituService inSituService = (ToolStripInSituService)_sp.GetService(typeof(ISupportInSituService));
            if (inSituService is not null)
            {
                // since we are going away .. restore the old commands.
                _designerHost.RemoveService<ISupportInSituService>();
            }
        }
    }
}
