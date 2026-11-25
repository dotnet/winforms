// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable disable

using System.Collections;
using System.Windows.Forms.Design;
using Moq;
using Moq.Protected;

namespace System.ComponentModel.Design.Tests;

/// <summary>
///  Drives the default collection editor dialog end to end (add, remove, OK, Cancel) through
///  <see cref="CollectionEditor.EditValue"/> to verify transactions, change notifications and
///  which instances are created or destroyed.
/// </summary>
public class CollectionEditorEditValueTests
{
    // Scenario: Adding two items and pressing OK appends them to the collection in the order they were added,
    // leaves existing items alive, returns the same collection and commits the design transaction.
    [WinFormsFact]
    public void EditValue_AddItemsThenOk_AppendsInOrderAndCommitsTransaction()
    {
        using Harness harness = new();
        TrackedItem first = new() { Name = "first" };
        TrackedItem second = new() { Name = "second" };
        List<TrackedItem> list = [first, second];
        TestEditor editor = new(typeof(List<TrackedItem>));

        object result = harness.Edit(editor, list, form =>
        {
            Click(form, "addButton");
            Click(form, "addButton");
            Assert.Equal(4, GetListBox(form).Items.Count);
            Click(form, "okButton");
        });

        Assert.Same(list, result);
        Assert.Equal(4, list.Count);
        Assert.Same(first, list[0]);
        Assert.Same(second, list[1]);
        Assert.Equal(2, editor.CreatedInstances.Count);
        Assert.Same(editor.CreatedInstances[0], list[2]);
        Assert.Same(editor.CreatedInstances[1], list[3]);
        Assert.NotSame(list[2], list[3]);
        Assert.All(list, item => Assert.False(item.IsDisposed));
        harness.Transaction.Protected().Verify("OnCommit", Times.Once());
        harness.Transaction.Protected().Verify("OnCancel", Times.Never());
        harness.Context.Verify(c => c.OnComponentChanging(), Times.AtLeastOnce());
        harness.Context.Verify(c => c.OnComponentChanged(), Times.AtLeastOnce());
    }

    // Scenario: Adding an item and then pressing Cancel destroys only the newly created item, restores the
    // original collection contents, keeps pre-existing items alive and cancels the design transaction.
    [WinFormsFact]
    public void EditValue_AddItemThenCancel_DestroysOnlyNewItemAndRestoresCollection()
    {
        using Harness harness = new();
        TrackedItem first = new() { Name = "first" };
        TrackedItem second = new() { Name = "second" };
        List<TrackedItem> list = [first, second];
        TestEditor editor = new(typeof(List<TrackedItem>));

        object result = harness.Edit(editor, list, form =>
        {
            Click(form, "addButton");
            Click(form, "cancelButton");
        });

        Assert.Same(list, result);
        Assert.Equal([first, second], list);
        Assert.False(first.IsDisposed);
        Assert.False(second.IsDisposed);
        TrackedItem created = Assert.Single(editor.CreatedInstances);
        Assert.True(created.IsDisposed);
        Assert.Single(editor.DestroyedInstances);
        Assert.Same(created, editor.DestroyedInstances[0]);
        harness.Transaction.Protected().Verify("OnCancel", Times.Once());
        harness.Transaction.Protected().Verify("OnCommit", Times.Never());
    }

    // Scenario: Removing a pre-existing item and pressing OK removes it from the collection and destroys it.
    [WinFormsFact]
    public void EditValue_RemoveExistingItemThenOk_RemovesAndDestroysItem()
    {
        using Harness harness = new();
        TrackedItem first = new() { Name = "first" };
        TrackedItem second = new() { Name = "second" };
        List<TrackedItem> list = [first, second];
        TestEditor editor = new(typeof(List<TrackedItem>));

        harness.Edit(editor, list, form =>
        {
            // The first item is selected when the dialog opens.
            Click(form, "removeButton");
            Assert.Single(GetListBox(form).Items);
            Assert.False(first.IsDisposed);
            Click(form, "okButton");
        });

        Assert.Equal([second], list);
        Assert.True(first.IsDisposed);
        Assert.False(second.IsDisposed);
        harness.Transaction.Protected().Verify("OnCommit", Times.Once());
    }

    // Scenario: Removing a pre-existing item and pressing Cancel leaves the collection intact and never
    // destroys the item that was only removed from the dialog list.
    [WinFormsFact]
    public void EditValue_RemoveExistingItemThenCancel_KeepsCollectionAndDoesNotDestroyItem()
    {
        using Harness harness = new();
        TrackedItem first = new() { Name = "first" };
        TrackedItem second = new() { Name = "second" };
        List<TrackedItem> list = [first, second];
        TestEditor editor = new(typeof(List<TrackedItem>));

        harness.Edit(editor, list, form =>
        {
            Click(form, "removeButton");
            Click(form, "cancelButton");
        });

        Assert.Equal([first, second], list);
        Assert.False(first.IsDisposed);
        Assert.False(second.IsDisposed);
        Assert.Empty(editor.DestroyedInstances);
        harness.Transaction.Protected().Verify("OnCancel", Times.Once());
    }

    // Scenario: An item added during the session and then removed in the same session is destroyed immediately,
    // and pressing OK leaves the original collection contents unchanged.
    [WinFormsFact]
    public void EditValue_AddThenRemoveNewItemThenOk_DestroysNewItemAndLeavesCollectionUnchanged()
    {
        using Harness harness = new();
        TrackedItem first = new() { Name = "first" };
        List<TrackedItem> list = [first];
        TestEditor editor = new(typeof(List<TrackedItem>));

        harness.Edit(editor, list, form =>
        {
            Click(form, "addButton");
            TrackedItem created = Assert.Single(editor.CreatedInstances);
            Assert.False(created.IsDisposed);

            // The newly added item is selected, so Remove targets it.
            Click(form, "removeButton");
            Assert.True(created.IsDisposed);
            Assert.Single(GetListBox(form).Items);
            Click(form, "okButton");
        });

        Assert.Equal([first], list);
        Assert.False(first.IsDisposed);
        Assert.Single(editor.DestroyedInstances);
    }

    // Scenario: Pressing OK without having changed anything is reported as a cancellation, so the transaction is
    // rolled back and the collection and its items are left untouched.
    [WinFormsFact]
    public void EditValue_OkWithoutChanges_CancelsTransactionAndLeavesCollection()
    {
        using Harness harness = new();
        TrackedItem first = new() { Name = "first" };
        List<TrackedItem> list = [first];
        TestEditor editor = new(typeof(List<TrackedItem>));

        object result = harness.Edit(editor, list, form => Click(form, "okButton"));

        Assert.Same(list, result);
        Assert.Equal([first], list);
        Assert.False(first.IsDisposed);
        Assert.Empty(editor.CreatedInstances);
        Assert.Empty(editor.DestroyedInstances);
        harness.Transaction.Protected().Verify("OnCancel", Times.Once());
        harness.Transaction.Protected().Verify("OnCommit", Times.Never());
    }

    // Scenario: When the editor reports that an item cannot be removed, the Remove button is disabled for it and
    // the item stays in the collection and is not destroyed.
    [WinFormsFact]
    public void EditValue_ItemThatCannotBeRemoved_DisablesRemoveAndKeepsItem()
    {
        using Harness harness = new();
        TrackedItem pinned = new() { Name = "pinned", IsPinned = true };
        List<TrackedItem> list = [pinned];
        TestEditor editor = new(typeof(List<TrackedItem>));

        harness.Edit(editor, list, form =>
        {
            Assert.False(GetButton(form, "removeButton").Enabled);
            Assert.True(GetButton(form, "addButton").Enabled);
            Click(form, "okButton");
        });

        Assert.Equal([pinned], list);
        Assert.False(pinned.IsDisposed);
        Assert.Empty(editor.DestroyedInstances);
    }

    // Scenario: A read-only collection opens in the dialog with both Add and Remove disabled and is not modified.
    [WinFormsFact]
    public void EditValue_ReadOnlyCollection_DisablesAddAndRemove()
    {
        using Harness harness = new();
        ArrayList inner = [new TrackedItem { Name = "first" }];
        ArrayList readOnly = ArrayList.ReadOnly(inner);
        TestEditor editor = new(typeof(ArrayList));

        object result = harness.Edit(editor, readOnly, form =>
        {
            Assert.False(GetButton(form, "addButton").Enabled);
            Assert.False(GetButton(form, "removeButton").Enabled);
            Click(form, "okButton");
        });

        Assert.Same(readOnly, result);
        Assert.Single(inner);
        Assert.Empty(editor.CreatedInstances);
    }

    // Scenario: If creating a new item fails, the error is shown through the UI service, nothing is added to the
    // collection, and cancelling destroys nothing.
    [WinFormsFact]
    public void EditValue_CreateInstanceThrows_ShowsErrorAndLeavesCollection()
    {
        using Harness harness = new();
        TrackedItem first = new() { Name = "first" };
        List<TrackedItem> list = [first];
        InvalidOperationException exception = new("cannot create");
        TestEditor editor = new(typeof(List<TrackedItem>)) { CreateInstanceException = exception };

        harness.Edit(editor, list, form =>
        {
            Click(form, "addButton");
            Assert.Single(GetListBox(form).Items);
            Click(form, "cancelButton");
        });

        harness.UIService.Verify(s => s.ShowError(exception), Times.Once());
        Assert.Equal([first], list);
        Assert.False(first.IsDisposed);
        Assert.Empty(editor.DestroyedInstances);
    }

    // Scenario: Without an IWindowsFormsEditorService the original value is returned, no dialog is created or
    // shown, and no design transaction is opened.
    [WinFormsFact]
    public void EditValue_NoEditorService_ReturnsValueWithoutCreatingForm()
    {
        using Harness harness = new();
        List<TrackedItem> list = [new TrackedItem()];
        TestEditor editor = new(typeof(List<TrackedItem>));
        Mock<IServiceProvider> provider = new();

        object result = editor.EditValue(harness.Context.Object, provider.Object, list);

        Assert.Same(list, result);
        Assert.Equal(0, editor.CreateCollectionFormCalls);
        harness.Host.Verify(h => h.CreateTransaction(It.IsAny<string>()), Times.Never());
        Assert.Null(editor.GetContext());
    }

    // Scenario: If the host refuses the transaction with a canceled checkout, the original value is returned,
    // the dialog is never shown, and the dialog form is disposed.
    [WinFormsFact]
    public void EditValue_TransactionCheckoutCanceled_ReturnsValueWithoutShowingDialog()
    {
        using Harness harness = new();
        harness.Host
            .Setup(h => h.CreateTransaction(It.IsAny<string>()))
            .Throws(CheckoutException.Canceled);
        List<TrackedItem> list = [new TrackedItem()];
        TestEditor editor = new(typeof(List<TrackedItem>));

        object result = harness.Edit(editor, list, _ => Assert.Fail("The dialog must not be shown."));

        Assert.Same(list, result);
        Assert.Equal(0, harness.Driver.ShowDialogCalls);
        Assert.True(editor.LastForm.IsDisposed);
    }

    // Scenario: A checkout failure that is not the canceled sentinel propagates to the caller, and the dialog form
    // is still disposed.
    [WinFormsFact]
    public void EditValue_TransactionThrowsOtherCheckoutException_PropagatesAndDisposesForm()
    {
        using Harness harness = new();
        harness.Host
            .Setup(h => h.CreateTransaction(It.IsAny<string>()))
            .Throws(new CheckoutException("denied"));
        TestEditor editor = new(typeof(List<TrackedItem>));

        Assert.Throws<CheckoutException>(() => harness.Edit(editor, new List<TrackedItem>(), _ => Assert.Fail("The dialog must not be shown.")));

        Assert.Equal(0, harness.Driver.ShowDialogCalls);
        Assert.True(editor.LastForm.IsDisposed);
    }

    // Scenario: An existing item changed through a child editor (reported by the change service) counts as an edit,
    // so pressing OK commits the transaction instead of cancelling it.
    [WinFormsFact]
    public void EditValue_ExistingItemChangedByChildEditorThenOk_CommitsTransaction()
    {
        using Harness harness = new();
        Mock<IComponentChangeService> changeService = new();
        harness.Host
            .Setup(h => h.GetService(typeof(IComponentChangeService)))
            .Returns(changeService.Object);
        harness.Context
            .Setup(c => c.GetService(typeof(IComponentChangeService)))
            .Returns(changeService.Object);
        TrackedItem existing = new();
        List<TrackedItem> list = [existing];
        TestEditor editor = new(typeof(List<TrackedItem>));

        harness.Edit(editor, list, form =>
        {
            changeService.Raise(s => s.ComponentChanged += null, new ComponentChangedEventArgs(existing, null, null, null));
            Click(form, "okButton");
        });

        Assert.Equal([existing], list);
        harness.Transaction.Protected().Verify("OnCommit", Times.Once());
        harness.Transaction.Protected().Verify("OnCancel", Times.Never());
    }

    // Scenario: The editor listens to component change notifications only while the dialog is open, on both the
    // OK and Cancel paths, and then unsubscribes.
    [WinFormsTheory]
    [InlineData(true)]
    [InlineData(false)]
    public void EditValue_ChangeServiceSubscription_OnlyWhileDialogIsOpen(bool accept)
    {
        using Harness harness = new();
        Mock<IComponentChangeService> changeService = new();
        harness.Host
            .Setup(h => h.GetService(typeof(IComponentChangeService)))
            .Returns(changeService.Object);
        harness.Context
            .Setup(c => c.GetService(typeof(IComponentChangeService)))
            .Returns(changeService.Object);
        List<TrackedItem> list = [new TrackedItem()];
        TestEditor editor = new(typeof(List<TrackedItem>));

        // The editor, the dialog form and the property grid site all subscribe; require at least the first two.
        int changedAddCount = 0;
        harness.Edit(editor, list, form =>
        {
            changedAddCount = changeService.Invocations.Count(i => i.Method.Name == "add_ComponentChanged");
            Assert.True(changedAddCount >= 2);
            changeService.VerifyAdd(s => s.ComponentChanging += It.IsAny<ComponentChangingEventHandler>(), Times.Once());
            changeService.VerifyRemove(s => s.ComponentChanged -= It.IsAny<ComponentChangedEventHandler>(), Times.Never());
            changeService.VerifyRemove(s => s.ComponentChanging -= It.IsAny<ComponentChangingEventHandler>(), Times.Never());
            Click(form, "addButton");
            Click(form, accept ? "okButton" : "cancelButton");
        });

        changeService.VerifyRemove(s => s.ComponentChanged -= It.IsAny<ComponentChangedEventHandler>(), Times.Exactly(changedAddCount));
        changeService.VerifyRemove(s => s.ComponentChanging -= It.IsAny<ComponentChangingEventHandler>(), Times.Once());
        harness.Transaction.Protected().Verify(accept ? "OnCommit" : "OnCancel", Times.Once());
    }

    // Scenario: Cancelling after adding a component that is already sited leaves the rollback to the transaction,
    // so the editor does not destroy the component itself and the collection is unchanged.
    [WinFormsFact]
    public void EditValue_AddSitedComponentThenCancel_LeavesRollbackToTransaction()
    {
        using Harness harness = new();
        Mock<ISite> site = new();
        Component created = new() { Site = site.Object };
        harness.Host
            .Setup(h => h.CreateComponent(typeof(Component)))
            .Returns(created);
        Component existing = new();
        List<Component> list = [existing];
        TestEditor editor = new(typeof(List<Component>));

        harness.Edit(editor, list, form =>
        {
            Click(form, "addButton");
            Click(form, "cancelButton");
        });

        harness.Host.Verify(h => h.CreateComponent(typeof(Component)), Times.Once());
        harness.Host.Verify(h => h.DestroyComponent(It.IsAny<IComponent>()), Times.Never());
        // The component is parented into the collection when it is added; undoing that is the transaction's job.
        Assert.Equal([existing, created], list);
        harness.Transaction.Protected().Verify("OnCancel", Times.Once());
    }

    // Scenario: Removing a pre-existing component and pressing OK destroys it through the designer host instead of
    // disposing it directly.
    [WinFormsFact]
    public void EditValue_RemoveExistingComponentThenOk_DestroysThroughHost()
    {
        using Harness harness = new();
        Component first = new();
        Component second = new();
        List<Component> list = [first, second];
        TestEditor editor = new(typeof(List<Component>));

        harness.Edit(editor, list, form =>
        {
            Click(form, "removeButton");
            Click(form, "okButton");
        });

        Assert.Equal([second], list);
        harness.Host.Verify(h => h.DestroyComponent(first), Times.Once());
        harness.Host.Verify(h => h.DestroyComponent(second), Times.Never());
        harness.Transaction.Protected().Verify("OnCommit", Times.Once());
    }

    private static Button GetButton(Form form, string name) => Assert.IsAssignableFrom<Button>(Assert.Single(form.Controls.Find(name, searchAllChildren: true)));

    private static ListBox GetListBox(Form form) => Assert.IsAssignableFrom<ListBox>(Assert.Single(form.Controls.Find("listbox", searchAllChildren: true)));

    private static void Click(Form form, string buttonName)
    {
        Button button = GetButton(form, buttonName);
        Assert.True(button.Enabled);
        button.PerformClick();
    }

    public class TrackedItem : IDisposable
    {
        public string Name { get; set; }

        public bool IsPinned { get; set; }

        public bool IsDisposed { get; private set; }

        public void Dispose() => IsDisposed = true;
    }

    private sealed class TestEditor : CollectionEditor
    {
        public TestEditor(Type type) : base(type)
        {
        }

        public List<TrackedItem> CreatedInstances { get; } = [];

        public List<object> DestroyedInstances { get; } = [];

        public Exception CreateInstanceException { get; init; }

        public int CreateCollectionFormCalls { get; private set; }

        public Form LastForm { get; private set; }

        public ITypeDescriptorContext GetContext() => Context;

        public void RefreshForm(Form form)
        {
            // Mimics the modal dialog's first display, which populates the list from the edit value.
            CollectionForm collectionForm = (CollectionForm)form;
            collectionForm.EditValue = collectionForm.EditValue;
        }

        protected override CollectionForm CreateCollectionForm()
        {
            CreateCollectionFormCalls++;
            CollectionForm form = base.CreateCollectionForm();
            LastForm = form;
            return form;
        }

        protected override bool CanRemoveInstance(object value) =>
            value is not TrackedItem { IsPinned: true } && base.CanRemoveInstance(value);

        protected override object CreateInstance(Type itemType)
        {
            if (CreateInstanceException is not null)
            {
                throw CreateInstanceException;
            }

            object instance = base.CreateInstance(itemType);
            if (instance is TrackedItem tracked)
            {
                CreatedInstances.Add(tracked);
            }

            return instance;
        }

        protected override void DestroyInstance(object instance)
        {
            DestroyedInstances.Add(instance);
            base.DestroyInstance(instance);
        }
    }

    private sealed class DialogDriver : IWindowsFormsEditorService
    {
        private readonly TestEditor _editor;
        private readonly Action<Form> _script;

        public DialogDriver(TestEditor editor, Action<Form> script)
        {
            _editor = editor;
            _script = script;
        }

        public int ShowDialogCalls { get; private set; }

        public void CloseDropDown()
        {
        }

        public void DropDownControl(Control control)
        {
        }

        public DialogResult ShowDialog(Form dialog)
        {
            ShowDialogCalls++;
            dialog.Show();
            _editor.RefreshForm(dialog);
            _script(dialog);
            return dialog.DialogResult;
        }
    }

    private sealed class Harness : IDisposable
    {
        private TestEditor _editor;

        public Harness()
        {
            Transaction = new Mock<DesignerTransaction>();
            Host = new Mock<IDesignerHost>();
            Host.Setup(h => h.CreateTransaction(It.IsAny<string>())).Returns(Transaction.Object);
            UIService = new Mock<IUIService>();
            Context = new Mock<ITypeDescriptorContext>();
            Context.Setup(c => c.OnComponentChanging()).Returns(true);
            Context.Setup(c => c.GetService(typeof(IDesignerHost))).Returns(Host.Object);
            Context.Setup(c => c.GetService(typeof(IUIService))).Returns(UIService.Object);
        }

        public Mock<DesignerTransaction> Transaction { get; }

        public Mock<IDesignerHost> Host { get; }

        public Mock<IUIService> UIService { get; }

        public Mock<ITypeDescriptorContext> Context { get; }

        public DialogDriver Driver { get; private set; }

        public object Edit(TestEditor editor, object value, Action<Form> script)
        {
            _editor = editor;
            Driver = new DialogDriver(editor, script);
            Mock<IServiceProvider> provider = new();
            provider.Setup(p => p.GetService(typeof(IWindowsFormsEditorService))).Returns(Driver);
            return editor.EditValue(Context.Object, provider.Object, value);
        }

        // Whatever the outcome of the edit, the editor must dispose the dialog form it created.
        public void Dispose() => Assert.True(_editor?.LastForm is null || _editor.LastForm.IsDisposed);
    }
}
