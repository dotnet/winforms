// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel.Design;
using System.ComponentModel;
using System.Collections;
using System.Drawing;
using System.Runtime.InteropServices;

namespace System.Windows.Forms.Design
{
    /// <devdoc>
    ///      This is the designer for the list view control.  It implements hit testing for
    ///      the items in the list view.
    /// </devdoc>
    internal class ListViewDesigner : ControlDesigner
    {
        private DesignerActionListCollection _actionLists;
        private NativeMethods.HDHITTESTINFO hdrhit = new NativeMethods.HDHITTESTINFO();
        private bool inShowErrorDialog;


        /// <include file='doc\ListViewDesigner.uex' path='docs/doc[@for="ListViewDesigner.AssociatedComponents"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Retrieves a list of associated components.  These are components that should be incluced in a cut or copy operation on this component.
        ///    </para>
        /// </devdoc>
        public override ICollection AssociatedComponents
        {
            get
            {
                ListView lv = Control as ListView;
                if (lv != null)
                {
                    return lv.Columns;
                }
                return base.AssociatedComponents;
            }
        }

        private bool OwnerDraw
        {
            get
            {
                return (bool)ShadowProperties["OwnerDraw"];
            }
            set
            {
                ShadowProperties["OwnerDraw"] = value;
            }
        }

        private View View
        {
            get
            {
                return ((ListView)Component).View;
            }
            set
            {
                ((ListView)Component).View = value;
                if (value == View.Details)
                {
                    HookChildHandles(Control.Handle);
                }
            }

        }


        /// <include file='doc\ListViewDesigner.uex' path='docs/doc[@for="ListViewDesigner.GetHitTest"]/*' />
        /// <devdoc>
        ///      We override GetHitTest to make the header in report view UI-active.
        /// </devdoc>
        protected override bool GetHitTest(Point point)
        {
            ListView lv = (ListView)Component;
            if (lv.View == View.Details)
            {
                Point lvPoint = Control.PointToClient(point);
                IntPtr hwndList = lv.Handle;
                IntPtr hwndHit = NativeMethods.ChildWindowFromPointEx(hwndList, lvPoint.X, lvPoint.Y, NativeMethods.CWP_SKIPINVISIBLE);

                if (hwndHit != IntPtr.Zero && hwndHit != hwndList)
                {
                    IntPtr hwndHdr = NativeMethods.SendMessage(hwndList, NativeMethods.LVM_GETHEADER, IntPtr.Zero, IntPtr.Zero);
                    if (hwndHit == hwndHdr)
                    {
                        NativeMethods.POINT ptHdr = new NativeMethods.POINT();
                        ptHdr.x = point.X;
                        ptHdr.y = point.Y;
                        NativeMethods.MapWindowPoints(IntPtr.Zero, hwndHdr, ptHdr, 1);
                        hdrhit.pt_x = ptHdr.x;
                        hdrhit.pt_y = ptHdr.y;
                        NativeMethods.SendMessage(hwndHdr, NativeMethods.HDM_HITTEST, IntPtr.Zero, hdrhit);
                        if (hdrhit.flags == NativeMethods.HHT_ONDIVIDER)
                            return true;
                    }
                }
            }
            return false;
        }

        public override void Initialize(IComponent component)
        {

            ListView lv = (ListView)component;
            this.OwnerDraw = lv.OwnerDraw;
            lv.OwnerDraw = false;
            lv.UseCompatibleStateImageBehavior = false;

            AutoResizeHandles = true;

            base.Initialize(component);
            if (lv.View == View.Details)
            {
                HookChildHandles(Control.Handle);
            }
        }

        protected override void PreFilterProperties(IDictionary properties)
        {
            PropertyDescriptor ownerDrawProp = (PropertyDescriptor)properties["OwnerDraw"];

            if (ownerDrawProp != null)
            {
                properties["OwnerDraw"] = TypeDescriptor.CreateProperty(typeof(ListViewDesigner), ownerDrawProp, new Attribute[0]);
            }

            PropertyDescriptor viewProp = (PropertyDescriptor)properties["View"];

            if (viewProp != null)
            {
                properties["View"] = TypeDescriptor.CreateProperty(typeof(ListViewDesigner), viewProp, new Attribute[0]);
            }

            base.PreFilterProperties(properties);
        }

        protected override void WndProc(ref Message m)
        {

            switch (m.Msg)
            {

                case NativeMethods.WM_NOTIFY:
                case NativeMethods.WM_REFLECT + NativeMethods.WM_NOTIFY:
                    NativeMethods.NMHDR nmhdr = (NativeMethods.NMHDR)Marshal.PtrToStructure(m.LParam, typeof(NativeMethods.NMHDR));
                    if (nmhdr.code == NativeMethods.HDN_ENDTRACK)
                    {

                        // Re-codegen if the columns have been resized
                        //
                        try
                        {
                            IComponentChangeService componentChangeService = (IComponentChangeService)GetService(typeof(IComponentChangeService));
                            componentChangeService.OnComponentChanged(Component, null, null, null);
                        }
                        catch (System.InvalidOperationException ex)
                        {
                            if (this.inShowErrorDialog)
                            {
                                return;
                            }

                            IUIService uiService = (IUIService)this.Component.Site.GetService(typeof(IUIService));
                            this.inShowErrorDialog = true;
                            try
                            {
                                DataGridViewDesigner.ShowErrorDialog(uiService, ex, (ListView)this.Component);
                            }
                            finally
                            {
                                this.inShowErrorDialog = false;
                            }
                            return;
                        }
                    }
                    break;
            }

            base.WndProc(ref m);
        }


        public override DesignerActionListCollection ActionLists
        {
            get
            {
                if (_actionLists == null)
                {
                    _actionLists = new DesignerActionListCollection();
                    _actionLists.Add(new ListViewActionList(this));
                }
                return _actionLists;
            }
        }
    }
}

