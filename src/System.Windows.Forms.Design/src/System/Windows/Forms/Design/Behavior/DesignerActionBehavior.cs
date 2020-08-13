// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Drawing;

namespace System.Windows.Forms.Design.Behavior
{
    /// <summary>
    ///  This is the Behavior that represents DesignerActions for a particular control.  The DesignerActionBehavior is responsible for responding to the MouseDown message and either 1) selecting the control and changing the  DesignerActionGlyph's image or 2) building up a chrome menu  and requesting it to be shown. Also, this Behavior acts as a proxy between "clicked" context menu items and the actual DesignerActions that they represent.
    /// </summary>
    internal sealed class DesignerActionBehavior : Behavior
    {
        private readonly IComponent _relatedComponent; //The component we are bound to
        private readonly DesignerActionUI _parentUI; //ptr to the parenting UI, used for showing menus and setting selection
        private DesignerActionListCollection _actionLists; //all the shortcuts!
        private readonly IServiceProvider _serviceProvider; // we need to cache the service provider here to be able to create the panel with the proper arguments
        private bool _ignoreNextMouseUp;

        /// <summary>
        ///  Constructor that calls base and caches off the action lists.
        /// </summary>
        internal DesignerActionBehavior(IServiceProvider serviceProvider, IComponent relatedComponent, DesignerActionListCollection actionLists, DesignerActionUI parentUI)
        {
            _actionLists = actionLists;
            _serviceProvider = serviceProvider;
            _relatedComponent = relatedComponent;
            _parentUI = parentUI;
        }

        /// <summary>
        ///  Returns the collection of DesignerActionLists this Behavior is managing. These will be dynamically updated (some can be removed, new ones can be added, etc...).
        /// </summary>
        internal DesignerActionListCollection ActionLists
        {
            get => _actionLists;
            set => _actionLists = value;
        }

        /// <summary>
        ///  Returns the parenting UI (a DesignerActionUI)
        /// </summary>
        internal DesignerActionUI ParentUI
        {
            get => _parentUI;
        }

        /// <summary>
        ///  Returns the Component that this glyph is attached to.
        /// </summary>
        internal IComponent RelatedComponent
        {
            get => _relatedComponent;
        }

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
            DesignerActionListCollection lists = new DesignerActionListCollection();
            lists.AddRange(ActionLists);
            DesignerActionPanel dap = new DesignerActionPanel(_serviceProvider);
            dap.UpdateTasks(lists, new DesignerActionListCollection(), string.Format(SR.DesignerActionPanel_DefaultPanelTitle, relatedComponent.GetType().Name), null);
            return dap;
        }

        /// <summary>
        ///  Shows the designer action panel UI associated with this glyph.
        /// </summary>
        internal void ShowUI(Glyph g)
        {
            if (!(g is DesignerActionGlyph glyph))
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

        public override bool OnMouseDoubleClick(Glyph g, MouseButtons button, Point mouseLoc)
        {
            _ignoreNextMouseUp = true;
            return true;
        }

        public override bool OnMouseDown(Glyph g, MouseButtons button, Point mouseLoc)
        { // we take the msg
            return (!ParentUI.IsDesignerActionPanelVisible);
        }

        /// <summary>
        ///  In response to a MouseUp, we will either 1) select the Glyph and control if not selected, or 2) Build up our context menu representing our DesignerActions and show it.
        /// </summary>
        public override bool OnMouseUp(Glyph g, MouseButtons button)
        {
            if (button != MouseButtons.Left || ParentUI is null)
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
                if (_serviceProvider != null)
                {
                    ISelectionService selectionService = (ISelectionService)_serviceProvider.GetService(typeof(ISelectionService));
                    if (selectionService != null)
                    {
                        if (selectionService.PrimarySelection != RelatedComponent)
                        {
                            List<IComponent> componentList = new List<IComponent>
                            {
                                RelatedComponent
                            };
                            selectionService.SetSelectedComponents(componentList, SelectionTypes.Primary);
                        }
                    }
                }
                ShowUI(g);
            }
            else
            {
                returnValue = false;
            }
            _ignoreNextMouseUp = false;
            return returnValue;
        }
    }
}
