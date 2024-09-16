// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections;
using System.Drawing.Design;
using System.Reflection;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace System.ComponentModel.Design;

/// <summary>
///  Provides a generic editor for most any collection.
/// </summary>
public partial class CollectionEditor : UITypeEditor
{
    private Type? _collectionItemType;
    private Type[]? _newItemTypes;

    private bool _ignoreChangedEvents;
    private bool _ignoreChangingEvents;

    /// <summary>
    ///  Initializes a new instance of the <see cref="CollectionEditor"/> class using the specified collection type.
    /// </summary>
    public CollectionEditor(Type type) => CollectionType = type;

    /// <summary>
    ///  Gets the data type of each item in the collection.
    /// </summary>
    protected Type CollectionItemType => _collectionItemType ??= CreateCollectionItemType();

    /// <summary>
    ///  Gets the type of the collection.
    /// </summary>
    protected Type CollectionType { get; }

    /// <summary>
    ///  Gets or sets a type descriptor that indicates the current context.
    /// </summary>
    protected ITypeDescriptorContext? Context { get; private set; }

    /// <summary>
    ///  Gets or sets the available item types that can be created for this collection.
    /// </summary>
    protected Type[] NewItemTypes => _newItemTypes ??= CreateNewItemTypes();

    /// <summary>
    ///  Gets the help topic to display for the dialog help button or pressing F1. Override to display a different help topic.
    /// </summary>
    protected virtual string HelpTopic => "net.ComponentModel.CollectionEditor";

    /// <summary>
    ///  Gets a value indicating whether original members of the collection can be removed.
    /// </summary>
    protected virtual bool CanRemoveInstance(object value)
    {
        if (value is IComponent component)
        {
            // Make sure the component is not being inherited -- we can't delete these!
            if (TypeDescriptorHelper.TryGetAttribute(component, out InheritanceAttribute? attribute)
                && attribute.InheritanceLevel != InheritanceLevel.NotInherited)
            {
                return false;
            }
        }

        return true;
    }

    /// <summary>
    ///  Useful for derived classes to do processing when cancelling changes
    /// </summary>
    protected virtual void CancelChanges()
    {
    }

    /// <summary>
    ///  Gets or sets a value indicating whether multiple collection members can be selected.
    /// </summary>
    protected virtual bool CanSelectMultipleInstances() => true;

    /// <summary>
    ///  Creates a new form to show the current collection.
    /// </summary>
    protected virtual CollectionForm CreateCollectionForm() => new CollectionEditorCollectionForm(this);

    /// <summary>
    ///  Creates a new instance of the specified collection item type.
    /// </summary>
    protected virtual object CreateInstance(Type itemType)
    {
        ArgumentNullException.ThrowIfNull(itemType);

        if (Context.TryGetService(out IDesignerHost? host) && typeof(IComponent).IsAssignableFrom(itemType))
        {
            IComponent instance = host.CreateComponent(itemType);

            // Set component defaults
            if (host.GetDesigner(instance) is IComponentInitializer initializer)
            {
                initializer.InitializeNewComponent(null);
            }

            if (instance is not null)
            {
                return instance;
            }
        }

        if (itemType.UnderlyingSystemType == typeof(string))
        {
            return string.Empty;
        }

        if (TypeDescriptor.CreateInstance(host, itemType, argTypes: null, args: null) is { } obj)
        {
            return obj;
        }

        throw new InvalidOperationException(
            string.Format(
            SR.CollectionEditorCreateInstanceError,
            nameof(IDesignerHost),
            nameof(TypeDescriptor),
            itemType.FullName));
    }

    /// <summary>
    ///  This method gets the object from the given object. The input is an arrayList returned as an Object.
    ///  The output is a arraylist which contains the individual objects that need to be created.
    /// </summary>
    protected virtual IList GetObjectsFromInstance(object? instance) => new ArrayList { instance };

    /// <summary>
    ///  Retrieves the display text for the given list item.
    /// </summary>
    protected virtual string GetDisplayText(object? value)
    {
        if (value is null)
        {
            return string.Empty;
        }

        if (TypeDescriptorHelper.TryGetPropertyValue(value, "Name", out string? text))
        {
            if (!string.IsNullOrEmpty(text))
            {
                return text;
            }
        }

        PropertyDescriptor? property = TypeDescriptor.GetDefaultProperty(CollectionType);
        if (property is not null && property.TryGetValue(value, out text))
        {
            if (!string.IsNullOrEmpty(text))
            {
                return text;
            }
        }

        text = TypeDescriptor.GetConverter(value).ConvertToString(value);
        if (string.IsNullOrEmpty(text))
        {
            text = value.GetType().Name;
        }

        return text;
    }

    /// <summary>
    ///  Gets an instance of the data type this collection contains.
    /// </summary>
    protected virtual Type CreateCollectionItemType()
    {
        PropertyInfo[] properties = TypeDescriptor.GetReflectionType(CollectionType).GetProperties(BindingFlags.Public | BindingFlags.Instance);

        foreach (var property in properties)
        {
            if (property.Name is "Item" or "Items")
            {
                return property.PropertyType;
            }
        }

        return typeof(object);
    }

    /// <summary>
    ///  Gets the data types this collection editor can create.
    /// </summary>
    protected virtual Type[] CreateNewItemTypes() => [CollectionItemType];

    /// <summary>
    ///  Destroys the specified instance of the object.
    /// </summary>
    protected virtual void DestroyInstance(object instance)
    {
        if (instance is IComponent component)
        {
            if (Context.TryGetService(out IDesignerHost? host))
            {
                host.DestroyComponent(component);
            }
            else
            {
                component.Dispose();
            }
        }
        else if (instance is IDisposable disposable)
        {
            disposable.Dispose();
        }
    }

    /// <summary>
    ///  Edits the specified object value using the editor style provided by <see cref="GetEditStyle"/>.
    /// </summary>
    public override object? EditValue(ITypeDescriptorContext? context, IServiceProvider provider, object? value)
    {
        if (!provider.TryGetService(out IWindowsFormsEditorService? editorService))
        {
            return value;
        }

        Context = context;

        // Child modal dialog - launching in SystemAware mode.
        CollectionForm localCollectionForm = ScaleHelper.InvokeInSystemAwareContext(CreateCollectionForm);
        ITypeDescriptorContext? lastContext = Context;
        localCollectionForm.EditValue = value;
        _ignoreChangingEvents = false;
        _ignoreChangedEvents = false;
        DesignerTransaction? transaction = null;

        bool commitChange = true;
        IComponentChangeService? changeService = null;
        IDesignerHost? host = Context?.GetService<IDesignerHost>();

        try
        {
            try
            {
                transaction = host?.CreateTransaction(string.Format(SR.CollectionEditorUndoBatchDesc, CollectionItemType.Name));
            }
            catch (CheckoutException cxe) when (cxe == CheckoutException.Canceled)
            {
                return value;
            }

            if (host.TryGetService(out changeService))
            {
                changeService.ComponentChanged += OnComponentChanged;
                changeService.ComponentChanging += OnComponentChanging;
            }

            if (localCollectionForm.ShowEditorDialog(editorService) == DialogResult.OK)
            {
                value = localCollectionForm.EditValue;
            }
            else
            {
                commitChange = false;
            }
        }
        finally
        {
            localCollectionForm.EditValue = null;
            Context = lastContext;

            if (commitChange)
            {
                transaction?.Commit();
            }
            else
            {
                transaction?.Cancel();
            }

            if (changeService is not null)
            {
                changeService.ComponentChanged -= OnComponentChanged;
                changeService.ComponentChanging -= OnComponentChanging;
            }

            localCollectionForm.Dispose();
        }

        return value;
    }

    /// <inheritdoc />
    public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext? context) => UITypeEditorEditStyle.Modal;

    private bool IsAnyObjectInheritedReadOnly(object[] items)
    {
        // If the object implements IComponent, and is not sited, check with the inheritance service (if it exists)
        // to see if this is a component that is being inherited from another class. If it is, then we do not want
        // to place it in the collection editor. If the inheritance service chose not to site the component, that
        // indicates it should be hidden from the user.

        IInheritanceService? inheritanceService = null;
        bool isInheritanceServiceInitialized = false;

        foreach (object item in items)
        {
            if (item is not IComponent { Site: null } component)
            {
                continue;
            }

            if (!isInheritanceServiceInitialized)
            {
                isInheritanceServiceInitialized = true;
                inheritanceService = Context?.GetService<IInheritanceService>();
            }

            if (inheritanceService is not null
                && inheritanceService.GetInheritanceAttribute(component).Equals(InheritanceAttribute.InheritedReadOnly))
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    ///  Converts the specified collection into an array of objects.
    /// </summary>
    protected virtual object[] GetItems(object? editValue)
    {
        if (editValue is ICollection collection)
        {
            object[] values = new object[collection.Count];
            collection.CopyTo(values, 0);
            return values;
        }

        return [];
    }

    /// <summary>
    ///  Gets the requested service, if it is available.
    /// </summary>
    protected object? GetService(Type serviceType) => Context?.GetService(serviceType);

    /// <summary>
    ///  Reflect any change events to the instance object
    /// </summary>
    private void OnComponentChanged(object? sender, ComponentChangedEventArgs e)
    {
        Debug.Assert(Context is not null);
        if (!_ignoreChangedEvents && sender != Context.Instance)
        {
            _ignoreChangedEvents = true;
            Context.OnComponentChanged();
        }
    }

    /// <summary>
    ///  Reflect any changed events to the instance object
    /// </summary>
    private void OnComponentChanging(object? sender, ComponentChangingEventArgs e)
    {
        Debug.Assert(Context is not null);
        if (!_ignoreChangingEvents && sender != Context.Instance)
        {
            _ignoreChangingEvents = true;
            Context.OnComponentChanging();
        }
    }

    /// <summary>
    ///  Removes the item from the column header from the listview column header collection
    /// </summary>
    internal virtual void OnItemRemoving(object? item)
    {
    }

    /// <summary>
    ///  Sets the specified collection to have the specified array of items.
    /// </summary>
    protected virtual object? SetItems(object? editValue, object[]? value)
    {
        // We look to see if the value implements IList, and if it does, we set through that.
        if (editValue is IList list)
        {
            list.Clear();
            if (value is not null)
            {
                for (int i = 0; i < value.Length; i++)
                {
                    list.Add(value[i]);
                }
            }
        }

        return editValue;
    }

    /// <summary>
    ///  Called when the help button is clicked.
    /// </summary>
    protected virtual void ShowHelp()
    {
        if (Context.TryGetService(out IHelpService? helpService))
        {
            helpService.ShowHelpFromKeyword(HelpTopic);
        }
    }
}
