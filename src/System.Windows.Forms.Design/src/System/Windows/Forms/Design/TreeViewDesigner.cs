// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Diagnostics;
using static Interop;

namespace System.Windows.Forms.Design
{
    /// <summary>
    ///  This is the designer for tree view controls.  It inherits
    ///  from the base control designer and adds live hit testing
    ///  capabilites for the tree view control.
    /// </summary>
    internal class TreeViewDesigner : ControlDesigner
    {
        private ComCtl32.TVHITTESTINFO tvhit;
        private DesignerActionListCollection _actionLists;
        private TreeView treeView;

        public TreeViewDesigner()
        {
            AutoResizeHandles = true;
        }

        /// <summary>
        ///  Disposes of this object.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (treeView != null)
                {
                    treeView.AfterExpand -= new TreeViewEventHandler(TreeViewInvalidate);
                    treeView.AfterCollapse -= new TreeViewEventHandler(TreeViewInvalidate);
                    treeView = null;
                }
            }

            base.Dispose(disposing);
        }

        /// <summary>
        ///  <para>Allows your component to support a design time user interface. A TabStrip
        ///  control, for example, has a design time user interface that allows the user
        ///  to click the tabs to change tabs. To implement this, TabStrip returns
        ///  true whenever the given point is within its tabs.</para>
        /// </summary>
        protected override bool GetHitTest(Point point)
        {
            point = Control.PointToClient(point);
            tvhit.pt = point;
            User32.SendMessageW(Control, (User32.WM)ComCtl32.TVM.HITTEST, IntPtr.Zero, ref tvhit);
            if (tvhit.flags == ComCtl32.TVHT.ONITEMBUTTON)
                return true;
            return false;
        }

        public override void Initialize(IComponent component)
        {
            base.Initialize(component);
            treeView = component as TreeView;
            Debug.Assert(treeView != null, "TreeView is null in TreeViewDesigner");
            if (treeView != null)
            {
                treeView.AfterExpand += new TreeViewEventHandler(TreeViewInvalidate);
                treeView.AfterCollapse += new TreeViewEventHandler(TreeViewInvalidate);
            }
        }

        private void TreeViewInvalidate(object sender, TreeViewEventArgs e)
        {
            if (treeView != null)
            {
                treeView.Invalidate();
            }
        }

        public override DesignerActionListCollection ActionLists
        {
            get
            {
                if (_actionLists == null)
                {
                    _actionLists = new DesignerActionListCollection();
                    _actionLists.Add(new TreeViewActionList(this));
                }

                return _actionLists;
            }
        }
    }
}
