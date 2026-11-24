// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable disable

using System.Collections;
using System.Reflection;
using System.Windows.Forms.Design;
using Moq;
using Moq.Protected;

namespace System.ComponentModel.Design.Tests;

/// <summary>
///  Verifies the contracts between <see cref="CollectionEditor"/>, its overridable factories and hooks,
///  and the real collection dialog or a custom <see cref="CollectionEditor.CollectionForm"/>.
///  The test class derives from <see cref="CollectionEditor"/> to reach the protected nested form type.
/// </summary>
public class CollectionEditorSubclassContractTests : CollectionEditor
{
    public CollectionEditorSubclassContractTests() : base(typeof(List<int>))
    {
    }

    // Scenario: When the editor offers several item types, the Add button gets a drop-down menu that
    // lists each type in order, and choosing an entry creates exactly that type.
    [WinFormsFact]
    public void NewItemTypes_MultipleTypes_AddMenuCreatesChosenType()
    {
        using Harness harness = new();
        List<Animal> list = [];
        ContractEditor editor = new(typeof(List<Animal>)) { NewTypes = [typeof(Dog), typeof(Cat)] };

        harness.Edit(editor, list, form =>
        {
            SplitButton add = Assert.IsType<SplitButton>(GetButton(form, "addButton"));
            ContextMenuStrip menu = add.ContextMenuStrip;
            Assert.NotNull(menu);
            Assert.Equal(["Dog", "Cat"], menu.Items.Cast<ToolStripItem>().Select(i => i.Text));

            ((ToolStripMenuItem)menu.Items[1]).PerformClick();
            ((ToolStripMenuItem)menu.Items[0]).PerformClick();
            Assert.Equal(2, GetListBox(form).Items.Count);
            Click(form, "okButton");
        });

        Assert.Collection(
            list,
            item => Assert.IsType<Cat>(item),
            item => Assert.IsType<Dog>(item));
        Assert.Equal([typeof(Cat), typeof(Dog)], editor.RequestedItemTypes);
    }

    // Scenario: When the editor offers a single item type, the Add button has no
    // drop-down menu, and clicking it creates that type.
    [WinFormsFact]
    public void NewItemTypes_SingleType_AddButtonHasNoMenuAndCreatesThatType()
    {
        using Harness harness = new();
        List<Animal> list = [];
        ContractEditor editor = new(typeof(List<Animal>)) { NewTypes = [typeof(Cat)] };

        harness.Edit(editor, list, form =>
        {
            SplitButton add = Assert.IsType<SplitButton>(GetButton(form, "addButton"));
            Assert.Null(add.ContextMenuStrip);
            Click(form, "addButton");
            Click(form, "okButton");
        });

        Assert.IsType<Cat>(Assert.Single(list));
    }

    // Scenario: When the editor lists several item types, clicking the main part of the Add button creates
    // the first listed type rather than the last or the collection's declared item type.
    [WinFormsFact]
    public void NewItemTypes_MultipleTypes_AddButtonCreatesFirstType()
    {
        using Harness harness = new();
        List<Animal> list = [];
        ContractEditor editor = new(typeof(List<Animal>)) { NewTypes = [typeof(Cat), typeof(Dog)] };

        harness.Edit(editor, list, form =>
        {
            Click(form, "addButton");
            Click(form, "okButton");
        });

        Assert.IsType<Cat>(Assert.Single(list));
    }

    // Scenario: Overriding the collection item type changes the dialog caption and is the type the default
    // Add button creates, and the editor reports it through the protected properties.
    [WinFormsFact]
    public void CreateCollectionItemType_Override_DrivesCaptionAndDefaultNewItemType()
    {
        using Harness harness = new();
        List<Animal> list = [];
        ContractEditor editor = new(typeof(List<Animal>)) { ItemType = typeof(Dog) };

        harness.Edit(editor, list, form =>
        {
            Assert.Contains("Dog", form.Text);
            Click(form, "addButton");
            Click(form, "okButton");
        });

        Assert.Equal(typeof(Dog), editor.ExposedItemType);
        Assert.Equal([typeof(Dog)], editor.ExposedNewItemTypes);
        Assert.IsType<Dog>(Assert.Single(list));
    }

    // Scenario: The editor's multi-select decision is applied to the dialog's list box, so a subclass that
    // allows it gets extended selection and one that forbids it gets single selection.
    [WinFormsTheory]
    [InlineData(true, SelectionMode.MultiExtended)]
    [InlineData(false, SelectionMode.One)]
    public void CanSelectMultipleInstances_Override_SetsListBoxSelectionMode(bool allowMultiple, SelectionMode expected)
    {
        using Harness harness = new();
        ContractEditor editor = new(typeof(List<Animal>)) { AllowMultiple = allowMultiple };
        SelectionMode actual = default;

        harness.Edit(editor, new List<Animal>(), form =>
        {
            actual = GetListBox(form).SelectionMode;
            Click(form, "cancelButton");
        });

        Assert.Equal(expected, actual);
    }

    // Scenario: With multi-select allowed, selecting two items and pressing Remove removes both from the
    // dialog and OK commits the change while destroying both removed items.
    [WinFormsFact]
    public void CanSelectMultipleInstances_True_RemoveRemovesAllSelectedItems()
    {
        using Harness harness = new();
        Dog a = new() { Name = "a" };
        Dog b = new() { Name = "b" };
        Dog c = new() { Name = "c" };
        List<Animal> list = [a, b, c];
        ContractEditor editor = new(typeof(List<Animal>)) { AllowMultiple = true };

        harness.Edit(editor, list, form =>
        {
            ListBox listBox = GetListBox(form);
            listBox.ClearSelected();
            listBox.SetSelected(0, true);
            listBox.SetSelected(2, true);
            Assert.Equal(2, listBox.SelectedItems.Count);
            Click(form, "removeButton");
            Assert.Single(listBox.Items);
            Click(form, "okButton");
        });

        Assert.Equal([b], list);
        Assert.Equal([a, c], editor.DestroyedInstances);
    }

    // Scenario: The display text produced by an overridden GetDisplayText is what the list shows for each
    // item and what the properties caption uses for the selected item.
    [WinFormsFact]
    public void GetDisplayText_Override_UsedForListEntriesAndPropertiesLabel()
    {
        using Harness harness = new();
        List<Animal> list = [new Dog { Name = "rex" }, new Cat { Name = "tom" }];
        ContractEditor editor = new(typeof(List<Animal>)) { DisplayText = value => $"<{((Animal)value).Name}>" };
        List<string> entries = [];
        string label = null;

        harness.Edit(editor, list, form =>
        {
            entries.AddRange(GetListBox(form).Items.Cast<object>().Select(i => i.ToString()));
            label = form.Controls.Find("propertiesLabel", searchAllChildren: true).Single().Text;
            Click(form, "cancelButton");
        });

        Assert.Equal(["<rex>", "<tom>"], entries);
        Assert.Contains("<rex>", label);
    }

    // Scenario: An empty string from the display text override does not break the dialog; the list entry is
    // simply shown as empty text.
    [WinFormsFact]
    public void GetDisplayText_OverrideReturnsEmpty_DialogStillOpens()
    {
        using Harness harness = new();
        ContractEditor editor = new(typeof(List<Animal>)) { DisplayText = _ => string.Empty };
        string entry = null;

        harness.Edit(editor, new List<Animal> { new Dog() }, form =>
        {
            entry = GetListBox(form).Items[0].ToString();
            Click(form, "cancelButton");
        });

        Assert.Equal(string.Empty, entry);
    }

    // Scenario: The dialog shows a help button, and both the help button and the F1 help request ask the
    // help service to display the topic returned by the subclass's overridden help topic.
    [WinFormsTheory]
    [InlineData(false)]
    [InlineData(true)]
    public void HelpTopic_Override_HelpRequestsShowOverriddenTopic(bool useHelpButton)
    {
        using Harness harness = new();
        ContractEditor editor = new(typeof(List<Animal>)) { Topic = "custom.topic" };
        bool helpButtonVisible = false;

        harness.Edit(editor, new List<Animal>(), form =>
        {
            helpButtonVisible = form.HelpButton;
            RaiseHelp(form, useHelpButton);
            Click(form, "cancelButton");
        });

        Assert.True(helpButtonVisible);
        harness.HelpService.Verify(h => h.ShowHelpFromKeyword("custom.topic"), Times.Once());
        harness.HelpService.Verify(h => h.ShowHelpFromKeyword(It.IsAny<string>()), Times.Once());
    }

    // Scenario: Without a help topic override, help requests use the default collection editor topic.
    [WinFormsFact]
    public void HelpTopic_Default_HelpRequestShowsDefaultTopic()
    {
        using Harness harness = new();
        ContractEditor editor = new(typeof(List<Animal>));

        harness.Edit(editor, new List<Animal>(), form =>
        {
            RaiseHelp(form, useHelpButton: false);
            Click(form, "cancelButton");
        });

        harness.HelpService.Verify(h => h.ShowHelpFromKeyword("net.ComponentModel.CollectionEditor"), Times.Once());
    }

    // Scenario: When the context offers no help service, a help request is ignored without throwing and the
    // dialog can still be closed normally.
    [WinFormsFact]
    public void HelpTopic_NoHelpService_HelpRequestIsIgnored()
    {
        using Harness harness = new(includeHelpService: false);
        ContractEditor editor = new(typeof(List<Animal>));

        object result = harness.Edit(editor, new List<Animal>(), form =>
        {
            RaiseHelp(form, useHelpButton: true);
            Click(form, "cancelButton");
        });

        Assert.NotNull(result);
        harness.HelpService.Verify(h => h.ShowHelpFromKeyword(It.IsAny<string>()), Times.Never());
    }

    // Scenario: When GetObjectsFromInstance expands one created instance into several, every expanded object
    // is added to the collection in order, and null entries are skipped.
    [WinFormsFact]
    public void GetObjectsFromInstance_ExpandsInstance_AddsEachNonNullObject()
    {
        using Harness harness = new();
        List<Animal> list = [];
        Dog extra = new() { Name = "extra" };
        ContractEditor editor = new(typeof(List<Animal>))
        {
            NewTypes = [typeof(Dog)],
            ExpandInstance = created => new ArrayList { created, null, extra }
        };

        harness.Edit(editor, list, form =>
        {
            Click(form, "addButton");
            Assert.Equal(2, GetListBox(form).Items.Count);
            Click(form, "okButton");
        });

        Assert.Equal(2, list.Count);
        Assert.Same(editor.CreatedInstances[0], list[0]);
        Assert.Same(extra, list[1]);
    }

    // Scenario: When GetObjectsFromInstance returns null, nothing is added and the collection stays unchanged
    // even after OK.
    [WinFormsFact]
    public void GetObjectsFromInstance_ReturnsNull_AddsNothing()
    {
        using Harness harness = new();
        List<Animal> list = [];
        ContractEditor editor = new(typeof(List<Animal>)) { ExpandInstance = _ => null };

        harness.Edit(editor, list, form =>
        {
            Click(form, "addButton");
            Assert.Empty(GetListBox(form).Items);
            Click(form, "okButton");
        });

        Assert.Empty(list);
        harness.UIService.Verify(u => u.ShowError(It.IsAny<Exception>()), Times.Never());
    }

    // Scenario: Pressing Cancel invokes the subclass's CancelChanges hook exactly once, whereas pressing OK
    // after a change never invokes it.
    [WinFormsTheory]
    [InlineData(true, 1)]
    [InlineData(false, 0)]
    public void CancelChanges_Hook_InvokedOnlyOnCancel(bool cancel, int expectedCalls)
    {
        using Harness harness = new();
        ContractEditor editor = new(typeof(List<Animal>));

        harness.Edit(editor, new List<Animal>(), form =>
        {
            Click(form, "addButton");
            Click(form, cancel ? "cancelButton" : "okButton");
        });

        Assert.Equal(expectedCalls, editor.CancelChangesCalls);
    }

    // Scenario: An item type that cannot be instantiated (an abstract class) makes the dialog report an
    // error through the UI service, adds nothing, and leaves the dialog usable for OK.
    [WinFormsFact]
    public void CreateInstance_NonCreatableItemType_ReportsErrorAndAddsNothing()
    {
        using Harness harness = new();
        List<Animal> list = [];
        ContractEditor editor = new(typeof(List<Animal>)) { ItemType = typeof(Shape), UseBaseCreateInstance = true };
        Exception shown = null;
        harness.UIService.Setup(u => u.ShowError(It.IsAny<Exception>())).Callback<Exception>(e => shown = e);

        harness.Edit(editor, list, form =>
        {
            Click(form, "addButton");
            Assert.Empty(GetListBox(form).Items);
            Click(form, "okButton");
        });

        Assert.Empty(list);
        harness.UIService.Verify(u => u.ShowError(It.IsAny<Exception>()), Times.Once());
        Assert.IsAssignableFrom<MemberAccessException>(shown);
    }

    // Scenario: CreateInstance rejects a null item type, and a string item type produces an empty string
    // without needing a designer host.
    [WinFormsFact]
    public void CreateInstance_NullAndStringItemTypes_ThrowsOrReturnsEmpty()
    {
        ContractEditor editor = new(typeof(List<string>)) { UseBaseCreateInstance = true };

        Assert.Throws<ArgumentNullException>(() => editor.InvokeCreateInstance(null));
        Assert.Equal(string.Empty, editor.InvokeCreateInstance(typeof(string)));
    }

    // Scenario: A collection type that is not an IList (a plain ICollection) is displayed read-only: Add is disabled, the existing
    // items are listed, and OK returns the original value without committing the design transaction.
    [WinFormsFact]
    public void CollectionType_NotIList_DialogIsReadOnlyAndOkReturnsOriginalValue()
    {
        using Harness harness = new();
        Stack set = new(new[] { "one", "two" });
        ContractEditor editor = new(typeof(Stack));
        int count = -1;

        object result = harness.Edit(editor, set, form =>
        {
            Assert.False(GetButton(form, "addButton").Enabled);
            Assert.False(GetButton(form, "removeButton").Enabled);
            count = GetListBox(form).Items.Count;
            GetButton(form, "okButton").PerformClick();
        });

        Assert.Same(set, result);
        Assert.Equal(2, count);
        Assert.Equal(2, set.Count);
        harness.Transaction.Protected().Verify("OnCancel", Times.Once());
        harness.Transaction.Protected().Verify("OnCommit", Times.Never());
    }

    // Scenario: A custom collection form returned from CreateCollectionForm is the one shown, receives the
    // edit value before display and a cleared edit value afterwards, sees the editor's context, and is disposed.
    [WinFormsFact]
    public void CreateCollectionForm_Custom_ReceivesValueContextAndIsDisposed()
    {
        using Harness harness = new();
        List<Animal> list = [new Dog { Name = "rex" }];
        ContractEditor editor = new(typeof(List<Animal>)) { UseCustomForm = true };

        object result = harness.EditWithCustomForm(editor, list, form =>
        {
            Assert.Same(list, form.EditValue);
            Assert.Same(harness.Context.Object, form.ExposedContext);
            Assert.Equal(typeof(List<Animal>), form.ExposedCollectionType);
            Assert.Equal(typeof(Animal), form.ExposedCollectionItemType);
            Assert.Same(harness.Host.Object, form.ExposedGetService(typeof(IDesignerHost)));
            return DialogResult.OK;
        });

        RecordingForm shown = editor.LastCustomForm;
        Assert.Same(list, result);
        Assert.Equal(1, shown.ShowEditorDialogCalls);
        Assert.Null(shown.EditValue);
        Assert.True(shown.IsDisposed);
        Assert.Equal([list, null], shown.EditValueNotifications);
    }

    // Scenario: Setting a custom form's Items reads and writes through the editor's GetItems and SetItems,
    // wraps the write in component changing and changed notifications, and a replacement collection returned
    // by SetItems becomes the value returned when the dialog is accepted.
    [WinFormsFact]
    public void CreateCollectionForm_Custom_ItemsRoundTripThroughEditorAndReplacementIsReturnedOnOk()
    {
        using Harness harness = new();
        string[] original = ["a", "b"];
        ContractEditor editor = new(typeof(string[]))
        {
            UseCustomForm = true,
            ReplaceOnSetItems = values => values.Cast<string>().ToArray()
        };

        object result = harness.EditWithCustomForm(editor, original, form =>
        {
            Assert.Equal(["a", "b"], form.ExposedItems);
            form.ExposedItems = ["x", "y", "z"];
            Assert.Equal(["x", "y", "z"], (string[])form.EditValue);
            return DialogResult.OK;
        });

        Assert.Equal(["x", "y", "z"], Assert.IsType<string[]>(result));
        Assert.NotSame(original, result);
        Assert.Equal(["a", "b"], original);
        Assert.Equal(1, editor.SetItemsCalls);
        harness.Context.Verify(c => c.OnComponentChanging(), Times.Once());
        harness.Context.Verify(c => c.OnComponentChanged(), Times.Once());
        harness.Transaction.Protected().Verify("OnCommit", Times.Once());
    }

    // Scenario: When a custom form is cancelled, the original value is returned even though SetItems produced
    // a replacement, and the design transaction is cancelled instead of committed.
    [WinFormsFact]
    public void CreateCollectionForm_Custom_CancelReturnsOriginalValueAndCancelsTransaction()
    {
        using Harness harness = new();
        string[] original = ["a", "b"];
        ContractEditor editor = new(typeof(string[]))
        {
            UseCustomForm = true,
            ReplaceOnSetItems = values => values.Cast<string>().ToArray()
        };

        object result = harness.EditWithCustomForm(editor, original, form =>
        {
            form.ExposedItems = ["x"];
            return DialogResult.Cancel;
        });

        Assert.Same(original, result);
        harness.Transaction.Protected().Verify("OnCancel", Times.Once());
        harness.Transaction.Protected().Verify("OnCommit", Times.Never());
    }

    // Scenario: A custom form's protected helpers route to the editor's overridable factories: instances are
    // created and destroyed through the editor, removal rules and item types come from the editor, and the
    // IUIService receives errors raised through DisplayError.
    [WinFormsFact]
    public void CreateCollectionForm_Custom_HelpersRouteToEditorOverrides()
    {
        using Harness harness = new();
        Dog pinned = new() { Name = "pinned", IsPinned = true };
        ContractEditor editor = new(typeof(List<Animal>))
        {
            UseCustomForm = true,
            NewTypes = [typeof(Dog), typeof(Cat)],
            AllowMultiple = false
        };
        InvalidOperationException error = new("boom");

        harness.EditWithCustomForm(editor, new List<Animal> { pinned }, form =>
        {
            Assert.Equal([typeof(Dog), typeof(Cat)], form.ExposedNewItemTypes);
            Assert.False(form.ExposedCanSelectMultipleInstances());
            Assert.False(form.ExposedCanRemoveInstance(pinned));
            Assert.True(form.ExposedCanRemoveInstance(new Dog()));

            object created = form.ExposedCreateInstance(typeof(Cat));
            Assert.IsType<Cat>(created);
            form.ExposedDestroyInstance(created);
            form.ExposedDisplayError(error);
            return DialogResult.Cancel;
        });

        Assert.Equal([typeof(Cat)], editor.RequestedItemTypes);
        Assert.Single(editor.DestroyedInstances);
        harness.UIService.Verify(u => u.ShowError(error), Times.Once());
    }

    // Scenario: Items assigned on a custom form when the context vetoes the change are not written to the
    // collection, and no changed notification is raised.
    [WinFormsFact]
    public void CreateCollectionForm_Custom_ComponentChangingVeto_DoesNotWriteItems()
    {
        using Harness harness = new();
        harness.Context.Setup(c => c.OnComponentChanging()).Returns(false);
        List<Animal> list = [new Dog { Name = "keep" }];
        ContractEditor editor = new(typeof(List<Animal>)) { UseCustomForm = true };

        harness.EditWithCustomForm(editor, list, form =>
        {
            form.ExposedItems = [];
            return DialogResult.OK;
        });

        Assert.Single(list);
        Assert.Equal(0, editor.SetItemsCalls);
        harness.Context.Verify(c => c.OnComponentChanged(), Times.Never());
    }

    // Scenario: Component changing and changed notifications raised by other components while the dialog is
    // open are forwarded to the edited instance exactly once, and every subscription made for the dialog is
    // released afterwards.
    [WinFormsFact]
    public void EditValue_ComponentChangeNotifications_ForwardedOnceAndUnsubscribed()
    {
        using Harness harness = new();
        FakeChangeService changeService = harness.UseChangeService();
        harness.Context.SetupGet(c => c.Instance).Returns(new object());
        ContractEditor editor = new(typeof(List<Animal>));
        Component other = new();

        harness.Edit(editor, new List<Animal>(), form =>
        {
            Assert.True(changeService.ChangingHandlerCount > 0);
            Assert.True(changeService.ChangedHandlerCount > 0);
            changeService.RaiseChanging(other);
            changeService.RaiseChanging(other);
            changeService.RaiseChanged(other);
            changeService.RaiseChanged(other);
            Click(form, "cancelButton");
        });

        harness.Context.Verify(c => c.OnComponentChanging(), Times.Once());
        harness.Context.Verify(c => c.OnComponentChanged(), Times.Once());
        Assert.Equal(0, changeService.ChangingHandlerCount);
        Assert.Equal(0, changeService.ChangedHandlerCount);
    }

    // Scenario: Notifications whose sender is the edited instance itself are not echoed back to the context,
    // which prevents a feedback loop.
    [WinFormsFact]
    public void EditValue_ComponentChangeNotificationsFromEditedInstance_AreNotForwarded()
    {
        using Harness harness = new();
        FakeChangeService changeService = harness.UseChangeService();
        Component instance = new();
        harness.Context.SetupGet(c => c.Instance).Returns(instance);
        ContractEditor editor = new(typeof(List<Animal>));

        harness.Edit(editor, new List<Animal>(), form =>
        {
            changeService.RaiseChanging(instance, sender: instance);
            changeService.RaiseChanged(instance, sender: instance);
            Click(form, "cancelButton");
        });

        harness.Context.Verify(c => c.OnComponentChanging(), Times.Never());
        harness.Context.Verify(c => c.OnComponentChanged(), Times.Never());
    }

    private static Button GetButton(Form form, string name) =>
        Assert.IsAssignableFrom<Button>(Assert.Single(form.Controls.Find(name, searchAllChildren: true)));

    private static ListBox GetListBox(Form form) =>
        Assert.IsAssignableFrom<ListBox>(Assert.Single(form.Controls.Find("listbox", searchAllChildren: true)));

    private static void Click(Form form, string buttonName)
    {
        Button button = GetButton(form, buttonName);
        Assert.True(button.Enabled);
        button.PerformClick();
    }

    private static void RaiseHelp(Form form, bool useHelpButton)
    {
        if (useHelpButton)
        {
            typeof(Form)
                .GetMethod("OnHelpButtonClicked", BindingFlags.Instance | BindingFlags.NonPublic)
                .Invoke(form, [new CancelEventArgs()]);
        }
        else
        {
            typeof(Control)
                .GetMethod("OnHelpRequested", BindingFlags.Instance | BindingFlags.NonPublic)
                .Invoke(form, [new HelpEventArgs(default)]);
        }
    }

    public class Animal
    {
        public string Name { get; set; }

        public bool IsPinned { get; set; }
    }

    public class Dog : Animal, IDisposable
    {
        public void Dispose()
        {
        }
    }

    public class Cat : Animal
    {
    }

    public abstract class Shape : Animal
    {
    }

    private sealed class FakeChangeService : IComponentChangeService
    {
        public event ComponentEventHandler ComponentAdded
        {
            add { }
            remove { }
        }

        public event ComponentEventHandler ComponentAdding
        {
            add { }
            remove { }
        }

        public event ComponentChangedEventHandler ComponentChanged
        {
            add
            {
                _changed += value;
            }
            remove
            {
                _changed -= value;
            }
        }

        public event ComponentChangingEventHandler ComponentChanging
        {
            add
            {
                _changing += value;
            }
            remove
            {
                _changing -= value;
            }
        }

        public event ComponentEventHandler ComponentRemoved
        {
            add { }
            remove { }
        }

        public event ComponentEventHandler ComponentRemoving
        {
            add { }
            remove { }
        }

        public event ComponentRenameEventHandler ComponentRename
        {
            add { }
            remove { }
        }

        private ComponentChangedEventHandler _changed;
        private ComponentChangingEventHandler _changing;

        public int ChangedHandlerCount => _changed?.GetInvocationList().Length ?? 0;

        public int ChangingHandlerCount => _changing?.GetInvocationList().Length ?? 0;

        public void OnComponentChanged(object component, MemberDescriptor member, object oldValue, object newValue)
        {
        }

        public void OnComponentChanging(object component, MemberDescriptor member)
        {
        }

        // Like the designer host, the service itself is the sender unless a sender is given explicitly.
        public void RaiseChanged(object component, object sender = null) =>
            _changed?.Invoke(sender ?? this, new ComponentChangedEventArgs(component, null, null, null));

        public void RaiseChanging(object component, object sender = null) =>
            _changing?.Invoke(sender ?? this, new ComponentChangingEventArgs(component, null));
    }

    private sealed class ContractEditor : CollectionEditor
    {
        public ContractEditor(Type type) : base(type)
        {
        }

        public Type[] NewTypes { get; init; }

        public Type ItemType { get; init; }

        public bool AllowMultiple { get; init; } = true;

        public Func<object, string> DisplayText { get; init; }

        public string Topic { get; init; }

        public Func<object, IList> ExpandInstance { get; init; }

        public Func<object[], object> ReplaceOnSetItems { get; init; }

        public bool UseCustomForm { get; init; }

        public Func<RecordingForm, DialogResult> CustomScript { get; set; }

        public bool UseBaseCreateInstance { get; init; }

        public List<Type> RequestedItemTypes { get; } = [];

        public List<object> CreatedInstances { get; } = [];

        public List<object> DestroyedInstances { get; } = [];

        public int CancelChangesCalls { get; private set; }

        public int SetItemsCalls { get; private set; }

        public RecordingForm LastCustomForm { get; private set; }

        public Type ExposedItemType => CollectionItemType;

        public Type[] ExposedNewItemTypes => NewItemTypes;

        public ITypeDescriptorContext ExposedContext => Context;

        public object InvokeCreateInstance(Type type) => CreateInstance(type);

        protected override Type CreateCollectionItemType() => ItemType ?? base.CreateCollectionItemType();

        protected override Type[] CreateNewItemTypes() => NewTypes ?? base.CreateNewItemTypes();

        protected override bool CanSelectMultipleInstances() => AllowMultiple;

        protected override string GetDisplayText(object value) => DisplayText is null ? base.GetDisplayText(value) : DisplayText(value);

        protected override string HelpTopic => Topic ?? base.HelpTopic;

        protected override IList GetObjectsFromInstance(object instance) =>
            ExpandInstance is null ? base.GetObjectsFromInstance(instance) : ExpandInstance(instance);

        protected override void CancelChanges() => CancelChangesCalls++;

        protected override bool CanRemoveInstance(object value) =>
            value is not Animal { IsPinned: true } && base.CanRemoveInstance(value);

        protected override object SetItems(object editValue, object[] value)
        {
            SetItemsCalls++;
            return ReplaceOnSetItems is null ? base.SetItems(editValue, value) : ReplaceOnSetItems(value);
        }

        protected override object CreateInstance(Type itemType)
        {
            RequestedItemTypes.Add(itemType);
            if (UseBaseCreateInstance)
            {
                return base.CreateInstance(itemType);
            }

            object instance = Activator.CreateInstance(itemType);
            CreatedInstances.Add(instance);
            return instance;
        }

        protected override void DestroyInstance(object instance)
        {
            DestroyedInstances.Add(instance);
            base.DestroyInstance(instance);
        }

        protected override CollectionForm CreateCollectionForm()
        {
            if (!UseCustomForm)
            {
                return base.CreateCollectionForm();
            }

            LastCustomForm = new RecordingForm(this, CustomScript);
            return LastCustomForm;
        }
    }

    private sealed class RecordingForm : CollectionForm
    {
        public RecordingForm(CollectionEditor editor, Func<RecordingForm, DialogResult> script) : base(editor)
        {
            Script = script;
        }

        public Func<RecordingForm, DialogResult> Script { get; init; }

        public int ShowEditorDialogCalls { get; private set; }

        public List<object> EditValueNotifications { get; } = [];

        public ITypeDescriptorContext ExposedContext => Context;

        public Type ExposedCollectionType => CollectionType;

        public Type ExposedCollectionItemType => CollectionItemType;

        public Type[] ExposedNewItemTypes => NewItemTypes;

        public object[] ExposedItems
        {
            get => Items;
            set => Items = value;
        }

        public object ExposedGetService(Type type) => GetService(type);

        public bool ExposedCanSelectMultipleInstances() => CanSelectMultipleInstances();

        public bool ExposedCanRemoveInstance(object value) => CanRemoveInstance(value);

        public object ExposedCreateInstance(Type type) => CreateInstance(type);

        public void ExposedDestroyInstance(object value) => DestroyInstance(value);

        public void ExposedDisplayError(Exception e) => DisplayError(e);

        protected override void OnEditValueChanged() => EditValueNotifications.Add(EditValue);

        protected internal override DialogResult ShowEditorDialog(IWindowsFormsEditorService edSvc)
        {
            ShowEditorDialogCalls++;
            return Script(this);
        }
    }

    private sealed class DialogDriver : IWindowsFormsEditorService
    {
        private readonly Action<Form> _script;

        public DialogDriver(Action<Form> script) => _script = script;

        public Form Dialog { get; private set; }

        public void CloseDropDown()
        {
        }

        public void DropDownControl(Control control)
        {
        }

        public DialogResult ShowDialog(Form dialog)
        {
            Dialog = dialog;
            dialog.Show();

            // The dialog fills its list when shown; mimic the edit value assignment the modal loop would trigger.
            CollectionForm form = (CollectionForm)dialog;
            form.EditValue = form.EditValue;
            _script(dialog);
            return dialog.DialogResult;
        }
    }

    private sealed class Harness : IDisposable
    {
        private DialogDriver _driver;
        private ContractEditor _editor;

        public Harness(bool includeHelpService = true)
        {
            Transaction = new Mock<DesignerTransaction>();
            Host = new Mock<IDesignerHost>();
            Host.Setup(h => h.CreateTransaction(It.IsAny<string>())).Returns(Transaction.Object);
            UIService = new Mock<IUIService>();
            HelpService = new Mock<IHelpService>();
            Context = new Mock<ITypeDescriptorContext>();
            Context.Setup(c => c.OnComponentChanging()).Returns(true);
            Context.Setup(c => c.GetService(typeof(IDesignerHost))).Returns(Host.Object);
            Context.Setup(c => c.GetService(typeof(IUIService))).Returns(UIService.Object);
            if (includeHelpService)
            {
                Context.Setup(c => c.GetService(typeof(IHelpService))).Returns(HelpService.Object);
            }
        }

        public Mock<DesignerTransaction> Transaction { get; }

        public Mock<IDesignerHost> Host { get; }

        public Mock<IUIService> UIService { get; }

        public Mock<IHelpService> HelpService { get; }

        public Mock<ITypeDescriptorContext> Context { get; }

        // Offers the change service through both the host and the context, as a designer would.
        public FakeChangeService UseChangeService()
        {
            FakeChangeService service = new();
            Host.Setup(h => h.GetService(typeof(IComponentChangeService))).Returns(service);
            Context.Setup(c => c.GetService(typeof(IComponentChangeService))).Returns(service);
            return service;
        }

        public object Edit(ContractEditor editor, object value, Action<Form> script)
        {
            _editor = editor;
            _driver = new DialogDriver(script);
            return editor.EditValue(Context.Object, ProviderFor(_driver), value);
        }

        public object EditWithCustomForm(ContractEditor editor, object value, Func<RecordingForm, DialogResult> script)
        {
            _editor = editor;
            editor.CustomScript = script;

            // The custom form decides the outcome itself and never asks the service to show it.
            return editor.EditValue(Context.Object, ProviderFor(new DialogDriver(_ => { })), value);
        }

        // Whatever the outcome of the edit, the editor must dispose the dialog form it created.
        public void Dispose()
        {
            if (_driver?.Dialog is { } dialog)
            {
                Assert.True(dialog.IsDisposed);
            }

            if (_editor?.LastCustomForm is { } custom)
            {
                Assert.True(custom.IsDisposed);
            }
        }

        private static IServiceProvider ProviderFor(IWindowsFormsEditorService service)
        {
            Mock<IServiceProvider> provider = new();
            provider.Setup(p => p.GetService(typeof(IWindowsFormsEditorService))).Returns(service);
            return provider.Object;
        }
    }
}
