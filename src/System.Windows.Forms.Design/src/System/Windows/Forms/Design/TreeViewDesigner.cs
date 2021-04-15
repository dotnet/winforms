// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Diagnostics;

namespace System.Windows.Forms.Design
{
    /// <devdoc>
    ///      This is the designer for tree view controls.  It inherits
    ///      from the base control designer and adds live hit testing
    ///      capabilites for the tree view control.
    /// </devdoc>
    internal class TreeViewDesigner : ControlDesigner
    {
        private NativeMethods.TV_HITTESTINFO tvhit = new NativeMethods.TV_HITTESTINFO();
        private DesignerActionListCollection _actionLists;
        private TreeView treeView = null;

        public TreeViewDesigner()
        {
            AutoResizeHandles = true;
        }


        /// <devdoc>
        ///      Disposes of this object.
        /// </devdoc>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (treeView != null)
                {
                    treeView.AfterExpand -= new System.Windows.Forms.TreeViewEventHandler(TreeViewInvalidate);
                    treeView.AfterCollapse -= new System.Windows.Forms.TreeViewEventHandler(TreeViewInvalidate);
                    treeView = null;
                }
            }

            base.Dispose(disposing);
        }

        /// <include file='doc\TreeViewDesigner.uex' path='docs/doc[@for="TreeViewDesigner.GetHitTest"]/*' />
        /// <devdoc>
        ///    <para>Allows your component to support a design time user interface. A TabStrip
        ///       control, for example, has a design time user interface that allows the user
        ///       to click the tabs to change tabs. To implement this, TabStrip returns
        ///       true whenever the given point is within its tabs.</para>
        /// </devdoc>
        protected override bool GetHitTest(Point point)
        {
            point = Control.PointToClient(point);
            tvhit.pt_x = point.X;
            tvhit.pt_y = point.Y;
            NativeMethods.SendMessage(Control.Handle, NativeMethods.TVM_HITTEST, 0, tvhit);
            if (tvhit.flags == NativeMethods.TVHT_ONITEMBUTTON)
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
                treeView.AfterExpand += new System.Windows.Forms.TreeViewEventHandler(TreeViewInvalidate);
                treeView.AfterCollapse += new System.Windows.Forms.TreeViewEventHandler(TreeViewInvalidate);
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
