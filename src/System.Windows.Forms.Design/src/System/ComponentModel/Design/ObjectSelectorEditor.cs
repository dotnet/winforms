// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing.Design;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace System.ComponentModel.Design;

public abstract partial class ObjectSelectorEditor : UITypeEditor
{
    public bool SubObjectSelector;
    protected object? prevValue;
    protected object? currValue;
    private Selector? _selector;

    public ObjectSelectorEditor()
    {
    }

    public ObjectSelectorEditor(bool subObjectSelector)
    {
        SubObjectSelector = subObjectSelector;
    }

    /// <inheritdoc />
    public override object? EditValue(ITypeDescriptorContext? context, IServiceProvider provider, object? value)
    {
        if (!provider.TryGetService(out IWindowsFormsEditorService? editorService))
        {
            return value;
        }

        if (_selector is null)
        {
            _selector = new Selector(this);

            // Enable Vista Explorer treeview style
            ApplyTreeViewThemeStyles(_selector);
        }

        prevValue = value;
        currValue = value;
        FillTreeWithData(_selector, context, provider);
        _selector.Start(editorService, value);
        editorService.DropDownControl(_selector);
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
        ArgumentNullException.ThrowIfNull(treeView);

        treeView.HotTracking = true;
        treeView.ShowLines = false;

        HWND hwnd = (HWND)treeView.Handle;
        uint exstyle = (uint)PInvokeCore.SendMessage(hwnd, PInvoke.TVM_GETEXTENDEDSTYLE);
        exstyle |= PInvoke.TVS_EX_DOUBLEBUFFER | PInvoke.TVS_EX_FADEINOUTEXPANDOS;
        PInvokeCore.SendMessage(hwnd, PInvoke.TVM_SETEXTENDEDSTYLE, (WPARAM)0, (LPARAM)exstyle);
    }

    public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext? context) => UITypeEditorEditStyle.DropDown;

    protected internal bool EqualsToValue(object? value) => value == currValue;

    protected virtual void FillTreeWithData(Selector selector, ITypeDescriptorContext? context, IServiceProvider provider)
        => selector.Clear();

    /// <inheritdoc />
    public virtual void SetValue(object? value) => currValue = value;
}
