// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.ComponentModel;
using System.ComponentModel.Design;
using System.Diagnostics;

namespace System.Windows.Forms.PropertyGridInternal
{
    internal class MultiPropertyDescriptorGridEntry : PropertyDescriptorGridEntry
    {
        private readonly MergePropertyDescriptor _mergedDescriptor;
        private readonly object[] _objects;

        public MultiPropertyDescriptorGridEntry(
            PropertyGrid ownerGrid,
            GridEntry peParent,
            object[] objectArray,
            PropertyDescriptor[] propInfo,
            bool hide)
            : base(ownerGrid, peParent, hide)
        {
            _mergedDescriptor = new MergePropertyDescriptor(propInfo);
            _objects = objectArray;
            Initialize(_mergedDescriptor);
        }

        public override IContainer Container
        {
            get
            {
                IContainer c = null;

                foreach (object o in _objects)
                {
                    if (o is not IComponent comp)
                    {
                        c = null;
                        break;
                    }

                    if (comp.Site is not null)
                    {
                        if (c is null)
                        {
                            c = comp.Site.Container;
                            continue;
                        }
                        else if (c == comp.Site.Container)
                        {
                            continue;
                        }
                    }

                    c = null;
                    break;
                }

                return c;
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
                    foreach (object o in _mergedDescriptor.GetValues(_objects))
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

        public override object PropertyValue
        {
            set
            {
                base.PropertyValue = value;

                RecreateChildren();
                if (Expanded)
                {
                    GridEntryHost.Refresh(false);
                }
            }
        }

        protected override bool CreateChildren()
        {
            return CreateChildren(false);
        }

        protected override bool CreateChildren(bool diffOldChildren)
        {
            try
            {
                if (_mergedDescriptor.PropertyType.IsValueType || (EntryFlags & Flags.Immutable) != 0)
                {
                    return base.CreateChildren(diffOldChildren);
                }

                ChildCollection.Clear();

                MultiPropertyDescriptorGridEntry[] mergedProps = MultiSelectRootGridEntry.PropertyMerger.GetMergedProperties(_mergedDescriptor.GetValues(_objects), this, _propertySort, CurrentTab);

                Debug.WriteLineIf(CompModSwitches.DebugGridView.TraceVerbose && mergedProps is null, "PropertyGridView: MergedProps returned null!");

                if (mergedProps is not null)
                {
                    ChildCollection.AddRange(mergedProps);
                }

                bool fExpandable = Children.Count > 0;
                if (!fExpandable)
                {
                    SetFlag(Flags.ExpandableFailed, true);
                }

                return fExpandable;
            }
            catch
            {
                return false;
            }
        }

        public override object GetChildValueOwner(GridEntry childEntry)
        {
            if (_mergedDescriptor.PropertyType.IsValueType || (EntryFlags & Flags.Immutable) != 0)
            {
                return base.GetChildValueOwner(childEntry);
            }

            return _mergedDescriptor.GetValues(_objects);
        }

        public override IComponent[] GetComponents()
        {
            var temp = new IComponent[_objects.Length];
            Array.Copy(_objects, 0, temp, 0, _objects.Length);
            return temp;
        }

        /// <summary>
        ///  Returns the text value of this property.
        /// </summary>
        public override string GetPropertyTextValue(object value)
        {
            try
            {
                if (value is null && _mergedDescriptor.GetValue(_objects, out bool allEqual) is null)
                {
                    if (!allEqual)
                    {
                        return string.Empty;
                    }
                }
            }
            catch
            {
                return string.Empty;
            }

            return base.GetPropertyTextValue(value);
        }

        protected internal override bool NotifyChildValue(GridEntry entry, Notify type)
        {
            DesignerTransaction transaction = DesignerHost?.CreateTransaction();

            try
            {
                return base.NotifyChildValue(entry, type);
            }
            finally
            {
                transaction?.Commit();
            }
        }

        protected override void NotifyParentChange(GridEntry entry)
        {
            // Now see if we need to notify the parent(s) up the chain.
            while (entry is PropertyDescriptorGridEntry propertyEntry
                && propertyEntry._propertyInfo.Attributes.Contains(NotifyParentPropertyAttribute.Yes))
            {
                // Find the next parent property with a different value owner.
                object owner = entry.GetValueOwner();

                // Find the next property descriptor with a different parent.
                while (!(entry is PropertyDescriptorGridEntry) || OwnersEqual(owner, entry.GetValueOwner()))
                {
                    entry = entry.ParentGridEntry;
                    if (entry is null)
                    {
                        break;
                    }
                }

                // Fire the change on the owner.
                if (entry is not null)
                {
                    owner = entry.GetValueOwner();

                    IComponentChangeService changeService = ComponentChangeService;

                    if (changeService is not null)
                    {
                        if (owner is Array ownerArray)
                        {
                            for (int i = 0; i < ownerArray.Length; i++)
                            {
                                PropertyDescriptor propertyInfo = propertyEntry._propertyInfo;

                                if (propertyInfo is MergePropertyDescriptor descriptor)
                                {
                                    propertyInfo = descriptor[i];
                                }

                                if (propertyInfo is not null)
                                {
                                    changeService.OnComponentChanging(ownerArray.GetValue(i), propertyInfo);
                                    changeService.OnComponentChanged(ownerArray.GetValue(i), propertyInfo);
                                }
                            }
                        }
                        else
                        {
                            changeService.OnComponentChanging(owner, propertyEntry._propertyInfo);
                            changeService.OnComponentChanged(owner, propertyEntry._propertyInfo);
                        }
                    }
                }
            }
        }

        protected internal override bool NotifyValueGivenParent(object obj, Notify type)
        {
            if (obj is ICustomTypeDescriptor descriptor)
            {
                obj = descriptor.GetPropertyOwner(_propertyInfo);
            }

            switch (type)
            {
                case Notify.Reset:

                    object[] objects = (object[])obj;

                    if (objects is not null && objects.Length > 0)
                    {
                        IDesignerHost host = DesignerHost;
                        DesignerTransaction trans = null;

                        if (host is not null)
                        {
                            trans = host.CreateTransaction(string.Format(SR.PropertyGridResetValue, PropertyName));
                        }

                        try
                        {
                            bool needChangeNotify = objects[0] is not IComponent component || component.Site is null;
                            if (needChangeNotify)
                            {
                                if (!OnComponentChanging())
                                {
                                    if (trans is not null)
                                    {
                                        trans.Cancel();
                                        trans = null;
                                    }

                                    return false;
                                }
                            }

                            _mergedDescriptor.ResetValue(obj);

                            if (needChangeNotify)
                            {
                                OnComponentChanged();
                            }

                            NotifyParentChange(this);
                        }
                        finally
                        {
                            if (trans is not null)
                            {
                                trans.Commit();
                            }
                        }
                    }

                    return false;
                case Notify.DoubleClick:
                case Notify.Return:
                    Debug.Assert(_propertyInfo is MergePropertyDescriptor, "Did not get a MergePropertyDescriptor!!!");
                    Debug.Assert(obj is object[], "Did not get an array of objects!!");

                    if (_propertyInfo is MergePropertyDescriptor mpd)
                    {
                        _eventBindings ??= this.GetService<IEventBindingService>();

                        if (_eventBindings is not null)
                        {
                            EventDescriptor eventDescriptor = _eventBindings.GetEvent(mpd[0]);
                            if (eventDescriptor is not null)
                            {
                                return ViewEvent(obj, null, eventDescriptor, true);
                            }
                        }

                        return false;
                    }
                    else
                    {
                        return base.NotifyValueGivenParent(obj, type);
                    }
            }

            return base.NotifyValueGivenParent(obj, type);
        }

        private bool OwnersEqual(object owner1, object owner2)
        {
            if (!(owner1 is Array))
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
            if (ComponentChangeService is not null)
            {
                int cLength = _objects.Length;
                for (int i = 0; i < cLength; i++)
                {
                    try
                    {
                        ComponentChangeService.OnComponentChanging(_objects[i], _mergedDescriptor[i]);
                    }
                    catch (CheckoutException co)
                    {
                        if (co == CheckoutException.Canceled)
                        {
                            return false;
                        }

                        throw;
                    }
                }
            }

            return true;
        }

        public override void OnComponentChanged()
        {
            if (ComponentChangeService is not null)
            {
                int cLength = _objects.Length;
                for (int i = 0; i < cLength; i++)
                {
                    ComponentChangeService.OnComponentChanged(_objects[i], _mergedDescriptor[i]);
                }
            }
        }
    }
}
