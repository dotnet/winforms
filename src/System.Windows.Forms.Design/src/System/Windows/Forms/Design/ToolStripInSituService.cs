// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.ComponentModel.Design;
using System.Diagnostics;

namespace System.Windows.Forms.Design
{
    /// <summary>
    ///  This class implements the ISupportInSituService which enables some designers to  go into InSitu Editing when Keys are pressed while the Component is Selected.
    /// </summary>
    internal class ToolStripInSituService : ISupportInSituService, IDisposable
    {
        private readonly IServiceProvider _sp;
        private readonly IDesignerHost _designerHost;
        private IComponentChangeService _componentChangeSvc;
        private ToolStripDesigner _toolDesigner;
        private ToolStripItemDesigner _toolItemDesigner;
        private ToolStripKeyboardHandlingService _toolStripKeyBoardService;

        /// <summary>
        ///  The constructor for this class which takes the serviceprovider used to get the selectionservice. This ToolStripInSituService is ToolStrip specific.
        /// </summary>
        public ToolStripInSituService(IServiceProvider provider)
        {
            _sp = provider;
            _designerHost = (IDesignerHost)provider.GetService(typeof(IDesignerHost));
            Debug.Assert(_designerHost != null, "ToolStripKeyboardHandlingService relies on the selection service, which is unavailable.");
            if (_designerHost != null)
            {
                _designerHost.AddService(typeof(ISupportInSituService), this);
            }
            _componentChangeSvc = (IComponentChangeService)_designerHost.GetService(typeof(IComponentChangeService));
            Debug.Assert(_componentChangeSvc != null, "ToolStripKeyboardHandlingService relies on the componentChange service, which is unavailable.");
            if (_componentChangeSvc != null)
            {
                _componentChangeSvc.ComponentRemoved += new ComponentEventHandler(OnComponentRemoved);
            }
        }

        /// <summary>
        ///  Disposes of this object, removing all commands from the menu service.
        /// </summary>
        public void Dispose()
        {
            if (_toolDesigner != null)
            {
                _toolDesigner.Dispose();
                _toolDesigner = null;
            }
            if (_toolItemDesigner != null)
            {
                _toolItemDesigner.Dispose();
                _toolItemDesigner = null;
            }
            if (_componentChangeSvc != null)
            {
                _componentChangeSvc.ComponentRemoved -= new ComponentEventHandler(OnComponentRemoved);
                _componentChangeSvc = null;
            }
        }

        private ToolStripKeyboardHandlingService ToolStripKeyBoardService
        {
            get
            {
                if (_toolStripKeyBoardService is null)
                {
                    _toolStripKeyBoardService = (ToolStripKeyboardHandlingService)_sp.GetService(typeof(ToolStripKeyboardHandlingService));
                }
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
                if (selectionService != null && host != null)
                {
                    if (!(selectionService.PrimarySelection is IComponent comp))
                    {
                        comp = (IComponent)ToolStripKeyBoardService.SelectedDesignerControl;
                    }
                    if (comp != null)
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
                                        if (_toolItemDesigner != null)
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
                                    if (_toolDesigner != null)
                                    {
                                        _toolItemDesigner = null;
                                        return true;
                                    }
                                }
                            }
                        }
                        else if (comp is ToolStripDropDown) //case for ToolStripDropDown..
                        {
                            if (host.GetDesigner(comp) is ToolStripDropDownDesigner designer)
                            {
                                ToolStripMenuItem toolItem = designer.DesignerMenuItem;
                                if (toolItem != null)
                                {
                                    _toolItemDesigner = host.GetDesigner(toolItem) as ToolStripItemDesigner;
                                    if (_toolItemDesigner != null)
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
                            if (_toolDesigner != null)
                            {
                                _toolItemDesigner = null;
                                return true;
                            }
                        }
                        else if (comp is ToolStripMenuItem)
                        {
                            _toolItemDesigner = host.GetDesigner(comp) as ToolStripItemDesigner;
                            if (_toolItemDesigner != null)
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
            if (_toolDesigner != null || _toolItemDesigner != null)
            {
                if (_toolDesigner != null)
                {
                    _toolDesigner.ShowEditNode(false);
                }
                else if (_toolItemDesigner != null)
                {
                    if (_toolItemDesigner is ToolStripMenuItemDesigner menuDesigner)
                    {
                        ISelectionService selService = (ISelectionService)_sp.GetService(typeof(ISelectionService));
                        if (selService != null)
                        {
                            object comp = selService.PrimarySelection;
                            if (comp is null)
                            {
                                comp = ToolStripKeyBoardService.SelectedDesignerControl;
                            }
                            DesignerToolStripControlHost designerItem = comp as DesignerToolStripControlHost;
                            if (designerItem != null || comp is ToolStripDropDown)
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
            if (_toolDesigner != null && _toolDesigner.Editor != null && _toolDesigner.Editor.EditBox != null)
            {
                hWnd = (_toolDesigner.Editor.EditBox.Visible) ? _toolDesigner.Editor.EditBox.Handle : hWnd;
            }
            else if (_toolItemDesigner != null && _toolItemDesigner.Editor != null && _toolItemDesigner.Editor.EditBox != null)
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
                if (inSituService != null)
                {
                    //since we are going away .. restore the old commands.
                    _designerHost.RemoveService(typeof(ISupportInSituService));
                }
            }
        }
    }
}
