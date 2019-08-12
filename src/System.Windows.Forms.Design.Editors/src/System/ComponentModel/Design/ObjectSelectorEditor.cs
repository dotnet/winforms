// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Drawing.Design;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using static Interop;

namespace System.ComponentModel.Design
{
    public abstract class ObjectSelectorEditor : UITypeEditor
    {
        public bool SubObjectSelector = false;
        protected object prevValue = null;
        protected object currValue = null;
        private Selector _selector = null;

        /// <summary>
        ///  Default constructor for ObjectSelectorEditor
        /// </summary>
        public ObjectSelectorEditor()
        {
        }

        /// <summary>
        ///  Constructor for ObjectSelectorEditor which sets SubObjectSelector equal to parameter subObjectSelector
        /// </summary>
        public ObjectSelectorEditor(bool subObjectSelector)
        {
            SubObjectSelector = subObjectSelector;
        }

        /// <summary>
        ///  Edits the given object value using the editor style provided by ObjectSelectorEditor.GetEditStyle.
        /// </summary>
        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            if (provider == null)
            {
                return value;
            }
            if (!(provider.GetService(typeof(IWindowsFormsEditorService)) is IWindowsFormsEditorService edSvc))
            {
                return value;
            }

            if (_selector == null)
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

            return value;
        }

        /// <summary>
        ///  Modify a WinForms TreeView control to use the new Explorer style theme
        /// </summary>
        /// <param name="treeView">The tree view control to modify</param>
        public static void ApplyTreeViewThemeStyles(TreeView treeView)
        {
            if (treeView == null)
            {
                throw new ArgumentNullException(nameof(treeView));
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

        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.DropDown;
        }

        protected internal bool EqualsToValue(object value) => value == currValue;

        protected virtual void FillTreeWithData(Selector selector, ITypeDescriptorContext context, IServiceProvider provider)
        {
            selector.Clear();
        }

        /// <summary>
        ///  Override this method to add validation code for new value
        /// </summary>
        public virtual void SetValue(object value)
        {
            currValue = value;
        }

        public class Selector : TreeView
        {
            private readonly ObjectSelectorEditor _editor = null;
            private IWindowsFormsEditorService _edSvc = null;
            public bool clickSeen = false;

            /// <summary>
            ///  Constructor for Selector, takes ObjectSelectorEditor
            /// </summary>
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

            /// <summary>
            ///  Adds a Node with given label and value to the parent, provided the parent is not null;
            ///  Otherwise, adds that node to the Nodes TreeNodeCollection. Returns the new node.
            /// </summary>
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

            /// <summary>
            ///  Returns true if the given node was selected; false otherwise.
            /// </summary>
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

            /// <summary>
            ///  Clears the TreeNodeCollection and sets clickSeen to false
            /// </summary>
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

            /// <summary>
            ///  Sets the selection
            /// </summary>
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
                {
                    return false;
                }

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

            /// <summary>
            ///  Sets the internal IWindowsFormsEditorService to the given edSvc, and calls SetSelection on the given value
            /// </summary>
            public void Start(IWindowsFormsEditorService edSvc, object value)
            {
                _edSvc = edSvc;
                clickSeen = false;
                SetSelection(value, Nodes);
            }

            /// <summary>
            ///  Sets the internal IWindowsFormsEditorService to null
            /// </summary>
            public void Stop()
            {
                _edSvc = null;
            }

            protected override void WndProc(ref Message m)
            {
                switch (m.Msg)
                {
                    case WindowMessages.WM_GETDLGCODE:
                        m.Result = (IntPtr)((long)m.Result | NativeMethods.DLGC_WANTALLKEYS);
                        return;
                    case WindowMessages.WM_MOUSEMOVE:
                        if (clickSeen)
                        {
                            clickSeen = false;
                        }
                        break;
                    case WindowMessages.WM_REFLECT + WindowMessages.WM_NOTIFY:
                        NativeMethods.NMTREEVIEW nmtv = Marshal.PtrToStructure<NativeMethods.NMTREEVIEW>(m.LParam);
                        if (nmtv.nmhdr.code == NativeMethods.NM_CLICK)
                        {
                            clickSeen = true;
                        }
                        break;
                }
                base.WndProc(ref m);
            }
        }

        ///  Suppressed because although the type implements ISerializable --its on the base class and this class
        ///  is not modifying the stream to include its local information.  Therefore, we should not publicly advertise this as
        ///  Serializable unless explicitly required.
        public class SelectorNode : TreeNode
        {
            public object value = null;

            /// <summary>
            ///  Sets label and value to given.
            /// </summary>
            public SelectorNode(string label, object value) : base(label)
            {
                this.value = value;
            }
        }
    }
}
