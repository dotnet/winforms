// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.ComponentModel.Design;

namespace System.Windows.Forms.PropertyGridInternal;

internal sealed class MultiPropertyDescriptorGridEntry : PropertyDescriptorGridEntry
{
    private readonly MergePropertyDescriptor _mergedDescriptor;
    private readonly object[] _objects;

    public MultiPropertyDescriptorGridEntry(
        PropertyGrid ownerGrid,
        GridEntry parent,
        object[] objectArray,
        PropertyDescriptor[] propertyDescriptors,
        bool hide)
        : base(
            ownerGrid,
            parent,
            new MergePropertyDescriptor(propertyDescriptors),
            hide)
    {
        _objects = objectArray;
        _mergedDescriptor = (MergePropertyDescriptor)PropertyDescriptor;
    }

    public override IContainer? Container
    {
        get
        {
            IContainer? container = null;

            foreach (object o in _objects)
            {
                if (o is not IComponent component)
                {
                    container = null;
                    break;
                }

                if (component.Site is not null)
                {
                    if (container is null)
                    {
                        container = component.Site.Container;
                        continue;
                    }
                    else if (container == component.Site.Container)
                    {
                        continue;
                    }
                }

                container = null;
                break;
            }

            return container;
        }
    }

    public override bool Expandable
    {
        get
        {
            bool expandable = GetFlagSet(Flags.Expandable);

            if (expandable && ChildCollection.Count > 0)
            {
                return true;
            }

            if (GetFlagSet(Flags.ExpandableFailed))
            {
                return false;
            }

            try
            {
                foreach (object? o in _mergedDescriptor.GetValues(_objects))
                {
                    if (o is null)
                    {
                        expandable = false;
                        break;
                    }
                }
            }
            catch
            {
                expandable = false;
            }

            return expandable;
        }
    }

    public override object? PropertyValue
    {
        set
        {
            base.PropertyValue = value;

            RecreateChildren();
            if (Expanded)
            {
                OwnerGridView?.Refresh(fullRefresh: false);
            }
        }
    }

    protected override bool CreateChildren(bool diffOldChildren = false)
    {
        try
        {
            if (_mergedDescriptor.PropertyType.IsValueType || EntryFlags.HasFlag(Flags.Immutable))
            {
                return base.CreateChildren(diffOldChildren);
            }

            ChildCollection.Clear();

            var mergedProperties = MultiSelectRootGridEntry.PropertyMerger.GetMergedProperties(
                _mergedDescriptor.GetValues(_objects),
                this,
                _propertySort,
                OwnerTab);

            if (mergedProperties is not null)
            {
                ChildCollection.AddRange(mergedProperties);
            }

            bool expandable = Children.Count > 0;
            if (!expandable)
            {
                SetFlag(Flags.ExpandableFailed, true);
            }

            return expandable;
        }
        catch (Exception e)
        {
            Debug.Fail(e.Message);
            return false;
        }
    }

    internal override object? GetValueOwnerInternal()
        => _mergedDescriptor.PropertyType.IsValueType || EntryFlags.HasFlag(Flags.Immutable)
            ? base.GetValueOwnerInternal()
            : _mergedDescriptor.GetValues(_objects);

    public override IComponent[] GetComponents()
    {
        var copy = new IComponent[_objects.Length];
        Array.Copy(_objects, 0, copy, 0, _objects.Length);
        return copy;
    }

    public override string GetPropertyTextValue(object? value)
    {
        try
        {
            if (value is null && _mergedDescriptor.GetValue(_objects, out bool allEqual) is null && !allEqual)
            {
                return string.Empty;
            }
        }
        catch
        {
            return string.Empty;
        }

        return base.GetPropertyTextValue(value);
    }

    internal override bool SendNotification(GridEntry entry, Notify notification)
    {
        DesignerTransaction? transaction = DesignerHost?.CreateTransaction();

        try
        {
            return base.SendNotification(entry, notification);
        }
        finally
        {
            transaction?.Commit();
        }
    }

    protected override void NotifyParentsOfChanges(GridEntry? entry)
    {
        // Now see if we need to notify the parent(s) up the chain.
        while (entry is PropertyDescriptorGridEntry propertyEntry &&
            propertyEntry.PropertyDescriptor.Attributes.Contains(NotifyParentPropertyAttribute.Yes))
        {
            // Find the next parent property with a different value owner.
            object? owner = entry.GetValueOwner();

            // Find the next property descriptor with a different parent.
            while (entry is not PropertyDescriptorGridEntry || OwnersEqual(owner, entry.GetValueOwner()))
            {
                entry = entry.ParentGridEntry;
                if (entry is null)
                {
                    break;
                }
            }

            // Fire the change on the owner.
            if (entry is null)
            {
                continue;
            }

            owner = entry.GetValueOwner();

            IComponentChangeService? changeService = ComponentChangeService;

            if (changeService is null)
            {
                continue;
            }

            if (owner is Array ownerArray)
            {
                for (int i = 0; i < ownerArray.Length; i++)
                {
                    PropertyDescriptor? propertyInfo = entry.PropertyDescriptor;

                    if (propertyInfo is MergePropertyDescriptor descriptor)
                    {
                        propertyInfo = descriptor[i];
                    }

                    if (propertyInfo is not null)
                    {
                        object component = ownerArray.GetValue(i)!;
                        changeService.OnComponentChanging(component, propertyInfo);
                        changeService.OnComponentChanged(component, propertyInfo);
                    }
                }
            }
            else if (owner is not null)
            {
                changeService.OnComponentChanging(owner, entry.PropertyDescriptor);
                changeService.OnComponentChanged(owner, entry.PropertyDescriptor);
            }
        }
    }

    protected override bool SendNotification(object? owner, Notify notification)
    {
        if (owner is ICustomTypeDescriptor descriptor)
        {
            owner = descriptor.GetPropertyOwner(PropertyDescriptor);
        }

        switch (notification)
        {
            case Notify.Reset:

                object[]? objects = (object[]?)owner;

                if (objects is null || objects.Length == 0)
                {
                    return false;
                }

                IDesignerHost? host = DesignerHost;
                DesignerTransaction? transaction = host?.CreateTransaction(string.Format(SR.PropertyGridResetValue, PropertyName));

                try
                {
                    bool needChangeNotify = objects[0] is not IComponent component || component.Site is null;
                    if (needChangeNotify)
                    {
                        if (!OnComponentChanging())
                        {
                            transaction?.Cancel();
                            transaction = null;

                            return false;
                        }
                    }

                    _mergedDescriptor.ResetValue(owner!);

                    if (needChangeNotify)
                    {
                        OnComponentChanged();
                    }

                    NotifyParentsOfChanges(this);
                }
                finally
                {
                    transaction?.Commit();
                }

                return false;
            case Notify.DoubleClick:
            case Notify.Return:
                Debug.Assert(PropertyDescriptor is MergePropertyDescriptor, "Did not get a MergePropertyDescriptor!!!");
                Debug.Assert(owner is object[], "Did not get an array of objects!!");

                if (PropertyDescriptor is MergePropertyDescriptor mergeDescriptor)
                {
                    _eventBindings ??= this.GetService<IEventBindingService>();

                    if (_eventBindings is not null)
                    {
                        EventDescriptor? eventDescriptor = _eventBindings.GetEvent(mergeDescriptor[0]);
                        if (eventDescriptor is not null)
                        {
                            return ViewEvent(owner, newHandler: null, eventDescriptor, alwaysNavigate: true);
                        }
                    }

                    return false;
                }
                else
                {
                    return base.SendNotification(owner, notification);
                }
        }

        return base.SendNotification(owner, notification);
    }

    private static bool OwnersEqual(object? owner1, object? owner2)
    {
        if (owner1 is not Array)
        {
            return owner1 == owner2;
        }
        else
        {
            if (owner1 is Array a1 && owner2 is Array a2 && a1.Length == a2.Length)
            {
                for (int i = 0; i < a1.Length; i++)
                {
                    if (a1.GetValue(i) != a2.GetValue(i))
                    {
                        return false;
                    }
                }

                return true;
            }

            return false;
        }
    }

    public override bool OnComponentChanging()
    {
        if (ComponentChangeService is null)
        {
            return true;
        }

        int length = _objects.Length;
        for (int i = 0; i < length; i++)
        {
            try
            {
                ComponentChangeService.OnComponentChanging(_objects[i], _mergedDescriptor[i]);
            }
            catch (CheckoutException co) when (co == CheckoutException.Canceled)
            {
                return false;
            }
        }

        return true;
    }

    public override void OnComponentChanged()
    {
        if (ComponentChangeService is not null)
        {
            int length = _objects.Length;
            for (int i = 0; i < length; i++)
            {
                ComponentChangeService.OnComponentChanged(_objects[i], _mergedDescriptor[i]);
            }
        }
    }
}
