// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable enable

using System.ComponentModel;
using System.Reflection;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;
using Xunit;
using static Interop;

namespace System.Windows.Forms.PropertyGridInternal.TestUtilities;

public class SubPropertyGrid<TSelected> : PropertyGrid where TSelected : new()
{
    private static readonly User32.WM WM_DELAYEDEXECUTION = User32.RegisterWindowMessageW("WinFormsSubPropertyGridDelayedExecution");

    internal PropertyGridView GridView => this.TestAccessor().Dynamic._gridView;

    internal GridEntry SelectedEntry
    {
        get => GridView.SelectedGridEntry;
        set => SelectedGridItem = value;
    }

    internal GridEntry this[string propertyName]
    {
        get
        {
            string categoryName = SelectedObject.GetType().GetProperty(propertyName)!
                .GetCustomAttribute<CategoryAttribute>()!.Category;
            return GetPropEntries().Cast<GridEntry>().Single(entry => entry.PropertyName == categoryName)
                .Children.Cast<GridEntry>().Single(entry => entry.PropertyName == propertyName);
        }
    }

    public SubPropertyGrid() => SelectedObject = new TSelected();

    public void PopupEditorAndClose(Action? onClosingAction = null)
    {
        Exception? exception = null;
        bool called = false;

        var callbackHandle = GCHandle.Alloc(() =>
        {
            try
            {
                onClosingAction?.Invoke();
            }
            catch (Exception ex)
            {
                exception = ex;
            }

            GridView.CloseDropDown();

            called = true;
        });

        try
        {
            User32.PostMessageW(this, WM_DELAYEDEXECUTION, lParam: GCHandle.ToIntPtr(callbackHandle));
            GridView.PopupDialog(GridView.TestAccessor().Dynamic._selectedRow);
        }
        finally
        {
            callbackHandle.Free();
        }

        Assert.True(called);

        if (exception is not null)
        {
            ExceptionDispatchInfo.Capture(exception).Throw();
        }
    }

    protected override void Dispose(bool disposing)
    {
        object selectedObject = SelectedObject;

        base.Dispose(disposing);

        if (disposing && selectedObject is IDisposable disposable)
        {
            disposable.Dispose();
        }
    }

    protected override void WndProc(ref Message m)
    {
        if (m.Msg != (int)WM_DELAYEDEXECUTION)
        {
            base.WndProc(ref m);
            return;
        }

        ((Action)GCHandle.FromIntPtr(m.LParam).Target!)();
    }

    protected override void OnGotFocus(EventArgs e)
    {
        base.OnGotFocus(e);

        GridView.Focus();
    }
}
