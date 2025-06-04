// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace System.ComponentModel.Design;

public abstract partial class ObjectSelectorEditor
{
    public class Selector : TreeView
    {
        private readonly ObjectSelectorEditor _editor;
        private IWindowsFormsEditorService? _editorService;
        public bool clickSeen;

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

            AfterSelect += OnAfterSelect;
        }

        /// <summary>
        ///  Adds a Node with given label and value to the parent, provided the parent is not null;
        ///  Otherwise, adds that node to the Nodes TreeNodeCollection. Returns the new node.
        /// </summary>
        public SelectorNode AddNode(string? label, object? value, SelectorNode? parent)
        {
            SelectorNode newNode = new(label, value);

            if (parent is not null)
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
            if (_editor is not null && _editorService is not null)
            {
                _editor.SetValue(((SelectorNode)SelectedNode!).value);
                if (_editor.EqualsToValue(((SelectorNode)SelectedNode).value))
                {
                    _editorService.CloseDropDown();
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        ///  Clears the TreeNodeCollection and sets clickSeen to false.
        /// </summary>
        public void Clear()
        {
            clickSeen = false;
            Nodes.Clear();
        }

        protected void OnAfterSelect(object? sender, TreeViewEventArgs e)
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
                    _editorService!.CloseDropDown();
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
            // We won't get an OnAfterSelect if it's already selected, so use this instead.
            if (e.Node == SelectedNode)
            {
                ChooseSelectedNodeIfEqual();
            }

            base.OnNodeMouseClick(e);
        }

        public bool SetSelection(object? value, TreeNodeCollection? nodes)
        {
            nodes ??= Nodes;

            int length = nodes.Count;
            if (length == 0)
            {
                return false;
            }

            TreeNode[] treeNodes = new TreeNode[length];
            nodes.CopyTo(treeNodes, 0);

            for (int i = 0; i < length; i++)
            {
                if (((SelectorNode)treeNodes[i]).value == value)
                {
                    SelectedNode = treeNodes[i];
                    return true;
                }

                if ((treeNodes[i].Nodes is not null) && (treeNodes[i].Nodes.Count != 0))
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
        public void Start(IWindowsFormsEditorService edSvc, object? value)
        {
            _editorService = edSvc;
            clickSeen = false;
            SetSelection(value, Nodes);
        }

        /// <summary>
        ///  Sets the internal IWindowsFormsEditorService to null
        /// </summary>
        public void Stop() => _editorService = null;

        protected override unsafe void WndProc(ref Message m)
        {
            switch (m.MsgInternal)
            {
                case PInvokeCore.WM_GETDLGCODE:
                    m.ResultInternal = (LRESULT)(m.ResultInternal | (int)PInvoke.DLGC_WANTALLKEYS);
                    return;
                case PInvokeCore.WM_MOUSEMOVE:
                    if (clickSeen)
                    {
                        clickSeen = false;
                    }

                    break;
                case MessageId.WM_REFLECT_NOTIFY:
                    NMHDR* nmtv = (NMHDR*)(nint)m.LParamInternal;
                    if (nmtv->code == PInvoke.NM_CLICK)
                    {
                        clickSeen = true;
                    }

                    break;
            }

            base.WndProc(ref m);
        }
    }
}
