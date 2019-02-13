// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Diagnostics.CodeAnalysis;

using System.Drawing.Design;

namespace System.Windows.Forms.Design.Editors.System.ComponentModel.Design
{
    [SuppressMessage("Microsoft.Design", "CA1012:AbstractTypesShouldNotHaveConstructors")]
    public abstract class ObjectSelectorEditor : UITypeEditor
    {
        public bool SubObjectSelector = false;
        protected object prevValue = null;
        protected object currValue = null;
        private Selector _selector = null;

        public ObjectSelectorEditor()
        {
        }

        public ObjectSelectorEditor(bool subObjectSelector)
        {
            SubObjectSelector = subObjectSelector;
        }
        
        [SuppressMessage("Microsoft.Security", "CA2123:OverrideLinkDemandsShouldBeIdenticalToBase")] // everything in this assembly is full trust.
        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            if (null != provider)
            {
                IWindowsFormsEditorService edSvc = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));
                if (edSvc != null)
                {
                    if (null == _selector)
                    {
                        _selector = new Selector(this);

                        // Enable Vista Explorer treeview style
                        ApplyTreeViewThemeStyles(_selector);
                    }

                    prevValue = value;
                    currValue = value;
                    FillTreeWithData(_selector, context, provider);
                    _selector.Start(edSvc, value);
                    edSvc.DropDownControl(_selector);
                    _selector.Stop();
                    if (prevValue != currValue)
                    {
                        value = currValue;
                    }
                }
            }

            return value;
        }
        
        /// <devdoc>
        /// Modify a WinForms TreeView control to use the new Explorer style theme
        /// </devdoc>
        /// <param name="treeView">The tree view control to modify</param>
        public static void ApplyTreeViewThemeStyles(TreeView treeView)
        {
            if (treeView == null)
            {
                throw new ArgumentNullException("treeView");
            }

            treeView.HotTracking = true;
            treeView.ShowLines = false;

            IntPtr hwnd = treeView.Handle;
            int exstyle = TreeView_GetExtendedStyle(hwnd);
            exstyle |= NativeMethods.TVS_EX_DOUBLEBUFFER | NativeMethods.TVS_EX_FADEINOUTEXPANDOS;
            TreeView_SetExtendedStyle(hwnd, exstyle, 0);
        }
        private static int TreeView_GetExtendedStyle(IntPtr handle)
        {
            IntPtr ptr = NativeMethods.SendMessage(handle, NativeMethods.TVM_GETEXTENDEDSTYLE, IntPtr.Zero, IntPtr.Zero);
            return ptr.ToInt32();
        }

        private static void TreeView_SetExtendedStyle(IntPtr handle, int extendedStyle, int mask)
        {
            NativeMethods.SendMessage(handle, NativeMethods.TVM_SETEXTENDEDSTYLE, new IntPtr(mask), new IntPtr(extendedStyle));
        }
        
        [SuppressMessage("Microsoft.Security", "CA2123:OverrideLinkDemandsShouldBeIdenticalToBase")] // everything in this assembly is full trust.
        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.DropDown;
        }
        
        public bool EqualsToValue(object value)
        {
            if (value == currValue)
                return true;
            else
                return false;
        }
        
        protected virtual void FillTreeWithData(Selector selector, ITypeDescriptorContext context, IServiceProvider provider)
        {
            selector.Clear();
        }

        //
        // override this method to add validation code for new value
        //
        public virtual void SetValue(object value)
        {
            currValue = value;
        }
        
        public class Selector : TreeView
        {
            private readonly ObjectSelectorEditor _editor = null;
            private IWindowsFormsEditorService _edSvc = null;
            public bool clickSeen = false;

            public Selector(ObjectSelectorEditor editor)
            {
                CreateHandle();
                _editor = editor;

                BorderStyle = BorderStyle.None;
                FullRowSelect = !editor.SubObjectSelector;
                Scrollable = true;
                CheckBoxes = false;
                ShowPlusMinus = editor.SubObjectSelector;
                ShowLines = editor.SubObjectSelector;
                ShowRootLines = editor.SubObjectSelector;

                AfterSelect += new TreeViewEventHandler(OnAfterSelect);
            }
            
            public SelectorNode AddNode(string label, object value, SelectorNode parent)
            {
                SelectorNode newNode = new SelectorNode(label, value);

                if (parent != null)
                {
                    parent.Nodes.Add(newNode);
                }
                else
                {
                    Nodes.Add(newNode);
                }
                return newNode;
            }

            private bool ChooseSelectedNodeIfEqual()
            {
                if (_editor != null && _edSvc != null)
                {
                    _editor.SetValue(((SelectorNode)SelectedNode).value);
                    if (_editor.EqualsToValue(((SelectorNode)SelectedNode).value))
                    {
                        _edSvc.CloseDropDown();
                        return true;
                    }
                }
                return false;
            }
            
            public void Clear()
            {
                clickSeen = false;
                Nodes.Clear();
            }
            
            protected void OnAfterSelect(object sender, TreeViewEventArgs e)
            {
                if (clickSeen)
                {
                    ChooseSelectedNodeIfEqual();
                    clickSeen = false;
                }
            }
            
            protected override void OnKeyDown(KeyEventArgs e)
            {
                Keys key = e.KeyCode;
                switch (key)
                {
                    case Keys.Return:
                        if (ChooseSelectedNodeIfEqual())
                        {
                            e.Handled = true;
                        }
                        break;

                    case Keys.Escape:
                        _editor.SetValue(_editor.prevValue);
                        e.Handled = true;
                        _edSvc.CloseDropDown();
                        break;
                }
                base.OnKeyDown(e);
            }
            
            protected override void OnKeyPress(KeyPressEventArgs e)
            {
                switch (e.KeyChar)
                {
                    case '\r':  // Enter key
                        e.Handled = true;
                        break;
                }
                base.OnKeyPress(e);
            }
            
            protected override void OnNodeMouseClick(TreeNodeMouseClickEventArgs e)
            {
                // we won't get an OnAfterSelect if it's already selected, so use this instead
                if (e.Node == SelectedNode)
                {
                    ChooseSelectedNodeIfEqual();
                }
                base.OnNodeMouseClick(e);
            }
            
            public bool SetSelection(object value, TreeNodeCollection nodes)
            {
                TreeNode[] treeNodes;

                if (nodes == null)
                {
                    treeNodes = new TreeNode[Nodes.Count];
                    Nodes.CopyTo(treeNodes, 0);
                }
                else
                {
                    treeNodes = new TreeNode[nodes.Count];
                    nodes.CopyTo(treeNodes, 0);
                }

                int len = treeNodes.Length;
                if (len == 0)
                    return false;

                for (int i = 0; i < len; i++)
                {
                    if (((SelectorNode)treeNodes[i]).value == value)
                    {
                        SelectedNode = treeNodes[i];
                        return true;
                    }
                    if ((treeNodes[i].Nodes != null) && (treeNodes[i].Nodes.Count != 0))
                    {
                        treeNodes[i].Expand();
                        if (SetSelection(value, treeNodes[i].Nodes))
                        {
                            return true;
                        }
                        treeNodes[i].Collapse();
                    }
                }
                return false;
            }
            
            public void Start(IWindowsFormsEditorService edSvc, object value)
            {
                _edSvc = edSvc;
                clickSeen = false;
                SetSelection(value, Nodes);
            }
            
            public void Stop()
            {
                _edSvc = null;
            }
            
            [SuppressMessage("Microsoft.Security", "CA2123:OverrideLinkDemandsShouldBeIdenticalToBase")] // everything in this assembly is full trust.
            protected override void WndProc(ref Message m)
            {
                switch (m.Msg)
                {
                    case NativeMethods.WM_GETDLGCODE:
                        m.Result = (IntPtr)((long)m.Result | NativeMethods.DLGC_WANTALLKEYS);
                        return;
                    case NativeMethods.WM_MOUSEMOVE:
                        if (clickSeen)
                        {
                            clickSeen = false;
                        }
                        break;
                    case NativeMethods.WM_REFLECT + NativeMethods.WM_NOTIFY:
                        NativeMethods.NMTREEVIEW nmtv = (NativeMethods.NMTREEVIEW)Marshal.PtrToStructure(m.LParam, typeof(NativeMethods.NMTREEVIEW));
                        if (nmtv.nmhdr.code == NativeMethods.NM_CLICK)
                        {
                            clickSeen = true;
                        }
                        break;
                }
                base.WndProc(ref m);
            }
        }
        
        /// Suppressed because although the type implements ISerializable --its on the base class and this class
        /// is not modifying the stream to include its local information.  Therefore, we should not publicly advertise this as
        /// Serializable unless explicitly required.
        [SuppressMessage("Microsoft.Usage", "CA2237:MarkISerializableTypesWithSerializable")]
        public class SelectorNode : TreeNode
        {
            public object value = null;
            
            public SelectorNode(string label, object value) : base(label)
            {
                this.value = value;
            }
        }
    }
}
