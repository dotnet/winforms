// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Drawing;

namespace System.Windows.Forms.Design.Behavior
{
    /// <summary>
    ///  The SelectionBehavior is pushed onto the BehaviorStack in response to a positively hit tested SelectionGlyph.  The SelectionBehavior performs  two main tasks: 1) forward messages to the related ControlDesigner, and 2) calls upon the SelectionManager to push a potention DragBehavior.
    /// </summary>
    internal sealed class SelectionManager : IDisposable
    {
        private Adorner selectionAdorner;//used to provide all selection glyphs
        private Adorner bodyAdorner;//used to track all body glyphs for each control
        private BehaviorService behaviorService;//ptr back to our BehaviorService
        private IServiceProvider serviceProvider;//standard service provider
        private readonly Hashtable componentToDesigner;//used for quick look up of designers related to comps
        private readonly Control rootComponent;//root component being designed
        private ISelectionService selSvc;//we cache the selection service for perf.
        private IDesignerHost designerHost;//we cache the designerhost for perf.
        private bool needRefresh;    // do we need to refresh?
        private Rectangle[] prevSelectionBounds;//used to only repaint the changing part of the selection
        private object prevPrimarySelection; //used to check if the primary selection changed
        private Rectangle[] curSelectionBounds;
        private int curCompIndex;
        private DesignerActionUI designerActionUI; // the "container" for all things related to the designer action (smartags) UI
        private bool selectionChanging; //we dont want the OnSelectionChanged to be recursively called.

        /// <summary>
        ///  Constructor.  Here we query for necessary services and cache them for perf. reasons. We also hook to Component Added/Removed/Changed notifications so we can keep in sync when the designers' components change.  Also, we create our custom Adorner and add it to the BehaviorService.
        /// </summary>
        public SelectionManager(IServiceProvider serviceProvider, BehaviorService behaviorService)
        {
            prevSelectionBounds = null;
            prevPrimarySelection = null;
            this.behaviorService = behaviorService;
            this.serviceProvider = serviceProvider;

            selSvc = (ISelectionService)serviceProvider.GetService(typeof(ISelectionService));
            designerHost = (IDesignerHost)serviceProvider.GetService(typeof(IDesignerHost));

            if (designerHost is null || selSvc is null)
            {
                Debug.Fail("SelectionManager - Host or SelSvc is null, can't continue");
            }

            //sync the BehaviorService's begindrag event
            behaviorService.BeginDrag += new BehaviorDragDropEventHandler(OnBeginDrag);
            //sync the BehaviorService's Synchronize event
            behaviorService.Synchronize += new EventHandler(OnSynchronize);
            selSvc.SelectionChanged += new EventHandler(OnSelectionChanged);
            rootComponent = (Control)designerHost.RootComponent;

            // create and add both of our adorners, one for selection, one for bodies
            selectionAdorner = new Adorner();
            bodyAdorner = new Adorner();
            behaviorService.Adorners.Add(bodyAdorner);
            behaviorService.Adorners.Add(selectionAdorner); //adding this will cause the adorner to get setup with a ptr to the beh.svc.

            componentToDesigner = new Hashtable();
            designerHost.TransactionClosed += new DesignerTransactionCloseEventHandler(OnTransactionClosed);
            // designeraction UI
            if (designerHost.GetService(typeof(DesignerOptionService)) is DesignerOptionService options)
            {
                PropertyDescriptor p = options.Options.Properties["UseSmartTags"];
                if (p != null && p.PropertyType == typeof(bool) && (bool)p.GetValue(null))
                {
                    designerActionUI = new DesignerActionUI(serviceProvider, selectionAdorner);
                    behaviorService.DesignerActionUI = designerActionUI;
                }
            }
        }

        /// <summary>
        ///  Returns the Adorner that contains all the BodyGlyphs for the current selection state.
        /// </summary>
        internal Adorner BodyGlyphAdorner
        {
            get => bodyAdorner;
        }

        /// <summary>
        ///  There are certain cases like Adding Item to ToolStrips through InSitu Editor, where there is ParentTransaction that has to be cancelled depending upon the user action When this parent transaction is cancelled, there may be no reason to REFRESH the selectionManager which actually clears all the glyphs and readds them This REFRESH causes a lot of flicker and can be avoided by setting this property to false. Since this property is checked in the TransactionClosed, the SelectionManager won't REFRESH and hence just eat up the refresh thus avoiding unnecessary flicker.
        /// </summary>
        internal bool NeedRefresh
        {
            get => needRefresh;
            set => needRefresh = value;
        }

        /// <summary>
        ///  Returns the Adorner that contains all the BodyGlyphs for the current selection state.
        /// </summary>
        internal Adorner SelectionGlyphAdorner
        {
            get => selectionAdorner;
        }

        /// <summary>
        ///  This method fist calls the recursive AddControlGlyphs() method. When finished, we add the final glyph(s) to the root comp.
        /// </summary>
        private void AddAllControlGlyphs(Control parent, ArrayList selComps, object primarySelection)
        {
            foreach (Control control in parent.Controls)
            {
                AddAllControlGlyphs(control, selComps, primarySelection);
            }

            GlyphSelectionType selType = GlyphSelectionType.NotSelected;
            if (selComps.Contains(parent))
            {
                if (parent.Equals(primarySelection))
                {
                    selType = GlyphSelectionType.SelectedPrimary;
                }
                else
                {
                    selType = GlyphSelectionType.Selected;
                }
            }
            AddControlGlyphs(parent, selType);
        }

        /// <summary>
        ///  Recursive method that goes through and adds all the glyphs of every child to our global Adorner.
        /// </summary>
        private void AddControlGlyphs(Control c, GlyphSelectionType selType)
        {
            ControlDesigner cd = (ControlDesigner)componentToDesigner[c];
            if (cd != null)
            {
                ControlBodyGlyph bodyGlyph = cd.GetControlGlyphInternal(selType);
                if (bodyGlyph != null)
                {
                    bodyAdorner.Glyphs.Add(bodyGlyph);
                    if (selType == GlyphSelectionType.SelectedPrimary ||
                        selType == GlyphSelectionType.Selected)
                    {
                        if (curSelectionBounds[curCompIndex] == Rectangle.Empty)
                        {
                            curSelectionBounds[curCompIndex] = bodyGlyph.Bounds;
                        }
                        else
                        {
                            curSelectionBounds[curCompIndex] = Rectangle.Union(curSelectionBounds[curCompIndex], bodyGlyph.Bounds);
                        }
                    }
                }
                GlyphCollection glyphs = cd.GetGlyphs(selType);
                if (glyphs != null)
                {
                    selectionAdorner.Glyphs.AddRange(glyphs);
                    if (selType == GlyphSelectionType.SelectedPrimary ||
                        selType == GlyphSelectionType.Selected)
                    {
                        foreach (Glyph glyph in glyphs)
                        {
                            curSelectionBounds[curCompIndex] = Rectangle.Union(curSelectionBounds[curCompIndex], glyph.Bounds);
                        }
                    }
                }
            }

            if (selType == GlyphSelectionType.SelectedPrimary || selType == GlyphSelectionType.Selected)
            {
                curCompIndex++;
            }
        }

        /// <summary>
        ///  Unhook all of our event notifications, clear our adorner and remove it from the Beh.Svc.
        /// </summary>
        // We don't need to Dispose rootComponent.
        public void Dispose()
        {
            if (designerHost != null)
            {
                designerHost.TransactionClosed -= new DesignerTransactionCloseEventHandler(OnTransactionClosed);
                designerHost = null;
            }

            if (serviceProvider != null)
            {
                if (selSvc != null)
                {
                    selSvc.SelectionChanged -= new EventHandler(OnSelectionChanged);
                    selSvc = null;
                }
                serviceProvider = null;
            }

            if (behaviorService != null)
            {
                behaviorService.Adorners.Remove(bodyAdorner);
                behaviorService.Adorners.Remove(selectionAdorner);
                behaviorService.BeginDrag -= new BehaviorDragDropEventHandler(OnBeginDrag);
                behaviorService.Synchronize -= new EventHandler(OnSynchronize);
                behaviorService = null;
            }

            if (selectionAdorner != null)
            {
                selectionAdorner.Glyphs.Clear();
                selectionAdorner = null;
            }

            if (bodyAdorner != null)
            {
                bodyAdorner.Glyphs.Clear();
                bodyAdorner = null;
            }

            if (designerActionUI != null)
            {
                designerActionUI.Dispose();
                designerActionUI = null;
            }
        }

        /// <summary>
        ///  Refreshes all selection Glyphs.
        /// </summary>
        public void Refresh()
        {
            NeedRefresh = false;
            OnSelectionChanged(this, null);
        }

        /// <summary>
        ///  When a component is added, we get the designer and add it to our hashtable for quick lookup.
        /// </summary>
        private void OnComponentAdded(object source, ComponentEventArgs ce)
        {
            IComponent component = ce.Component;
            IDesigner designer = designerHost.GetDesigner(component);
            if (designer is ControlDesigner)
            {
                componentToDesigner.Add(component, designer);
            }
        }

        /// <summary>
        ///  Before a drag, remove all glyphs that are involved in the drag operation and any that don't allow drops.
        /// </summary>
        private void OnBeginDrag(object source, BehaviorDragDropEventArgs e)
        {
            ArrayList dragComps = new ArrayList(e.DragComponents);
            ArrayList glyphsToRemove = new ArrayList();
            foreach (ControlBodyGlyph g in bodyAdorner.Glyphs)
            {
                if (g.RelatedComponent is Control)
                {
                    if (dragComps.Contains(g.RelatedComponent) ||
                        !((Control)g.RelatedComponent).AllowDrop)
                    {
                        glyphsToRemove.Add(g);
                    }
                }
            }
            foreach (Glyph g in glyphsToRemove)
            {
                bodyAdorner.Glyphs.Remove(g);
            }
        }

        // Called by the DropSourceBehavior when dragging into a new host
        internal void OnBeginDrag(BehaviorDragDropEventArgs e)
        {
            OnBeginDrag(null, e);
        }

        /// <summary>
        ///  When a component is changed - we need to refresh the selection.
        /// </summary>
        private void OnComponentChanged(object source, ComponentChangedEventArgs ce)
        {
            if (selSvc.GetComponentSelected(ce.Component))
            {
                if (!designerHost.InTransaction)
                {
                    Refresh();
                }
                else
                {
                    NeedRefresh = true;
                }
            }
        }

        /// <summary>
        ///  When a component is removed - we remove the key and value from our hashtable.
        /// </summary>
        private void OnComponentRemoved(object source, ComponentEventArgs ce)
        {
            if (componentToDesigner.Contains(ce.Component))
            {
                componentToDesigner.Remove(ce.Component);
            }
            //remove the associated designeractionpanel
            if (designerActionUI != null)
            {
                designerActionUI.RemoveActionGlyph(ce.Component);
            }
        }
        /// <summary>
        ///  Computes the region representing the difference between the old selection and the new selection.
        /// </summary>
        private Region DetermineRegionToRefresh(object primarySelection)
        {
            Region toRefresh = new Region(Rectangle.Empty);
            Rectangle[] larger;
            Rectangle[] smaller;
            if (curSelectionBounds.Length >= prevSelectionBounds.Length)
            {
                larger = curSelectionBounds;
                smaller = prevSelectionBounds;
            }
            else
            {
                larger = prevSelectionBounds;
                smaller = curSelectionBounds;
            }

            // we need to make sure all of the rects in the smaller array are  accounted for.  Any that don't intersect a rect in the larger array need to be included in the region to repaint.
            bool[] intersected = new bool[smaller.Length];
            for (int i = 0; i < smaller.Length; i++)
            {
                intersected[i] = false;
            }

            // determine which rects in the larger array need to be included in the region to invalidate by intersecting with rects in the smaller array.
            for (int l = 0; l < larger.Length; l++)
            {
                bool largeIntersected = false;
                Rectangle large = larger[l];
                for (int s = 0; s < smaller.Length; s++)
                {
                    if (large.IntersectsWith(smaller[s]))
                    {
                        Rectangle small = smaller[s];
                        largeIntersected = true;
                        if (large != small)
                        {
                            toRefresh.Union(large);
                            toRefresh.Union(small);
                        }
                        intersected[s] = true;
                        break;
                    }
                }
                if (!largeIntersected)
                {
                    toRefresh.Union(large);
                }
            }
            // now add any rects from the smaller array that weren't accounted for
            for (int k = 0; k < intersected.Length; k++)
            {
                if (!intersected[k])
                {
                    toRefresh.Union(smaller[k]);
                }
            }

            using (Graphics g = behaviorService.AdornerWindowGraphics)
            {
                //If all that changed was the primary selection, then the refresh region was empty, but we do need to update the 2 controls.
                if (toRefresh.IsEmpty(g) && primarySelection != null && !primarySelection.Equals(prevPrimarySelection))
                {
                    for (int i = 0; i < curSelectionBounds.Length; i++)
                    {
                        toRefresh.Union(curSelectionBounds[i]);
                    }
                }
            }
            return toRefresh;
        }

        /// <summary>
        ///  Event handler for the behaviorService's Synchronize event
        /// </summary>
        private void OnSynchronize(object sender, EventArgs e)
        {
            Refresh();
        }

        /// <summary>
        ///  On every selectionchange, we remove all glyphs, get the newly selected components, and re-add all glyphs back to the Adorner.
        /// </summary>
        private void OnSelectionChanged(object sender, EventArgs e)
        {
            // Note: selectionChanging would guard against a re-entrant code... Since we dont want to be in messed up state when adding new Glyphs.
            if (!selectionChanging)
            {
                selectionChanging = true;
                selectionAdorner.Glyphs.Clear();
                bodyAdorner.Glyphs.Clear();
                ArrayList selComps = new ArrayList(selSvc.GetSelectedComponents());
                object primarySelection = selSvc.PrimarySelection;
                //add all control glyphs to all controls on rootComp
                curCompIndex = 0;
                curSelectionBounds = new Rectangle[selComps.Count];
                AddAllControlGlyphs(rootComponent, selComps, primarySelection);
                if (prevSelectionBounds != null)
                {
                    Region toUpdate = DetermineRegionToRefresh(primarySelection);
                    using (Graphics g = behaviorService.AdornerWindowGraphics)
                    {
                        if (!toUpdate.IsEmpty(g))
                        {
                            selectionAdorner.Invalidate(toUpdate);
                        }
                    }
                }
                else
                {
                    // There was no previous selection, so just invalidate the current selection
                    if (curSelectionBounds.Length > 0)
                    {
                        Rectangle toUpdate = curSelectionBounds[0];
                        for (int i = 1; i < curSelectionBounds.Length; i++)
                        {
                            toUpdate = Rectangle.Union(toUpdate, curSelectionBounds[i]);
                        }
                        if (toUpdate != Rectangle.Empty)
                        {
                            selectionAdorner.Invalidate(toUpdate);
                        }
                    }
                    else
                    {
                        selectionAdorner.Invalidate();
                    }
                }

                prevPrimarySelection = primarySelection;
                if (curSelectionBounds.Length > 0)
                {
                    prevSelectionBounds = new Rectangle[curSelectionBounds.Length];
                    Array.Copy(curSelectionBounds, prevSelectionBounds, curSelectionBounds.Length);
                }
                else
                {
                    prevSelectionBounds = null;
                }
                selectionChanging = false;
            }
        }

        /// <summary>
        ///  When a transaction that involves one of our components closes,  refresh to reflect any changes.
        /// </summary>
        private void OnTransactionClosed(object sender, DesignerTransactionCloseEventArgs e)
        {
            if (e.LastTransaction && NeedRefresh)
            {
                Refresh();
            }
        }
    }
}
