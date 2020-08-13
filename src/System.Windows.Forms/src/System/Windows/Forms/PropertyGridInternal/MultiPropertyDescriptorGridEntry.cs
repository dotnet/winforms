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
        private readonly MergePropertyDescriptor mergedPd;
        private readonly object[] objs;

        public MultiPropertyDescriptorGridEntry(PropertyGrid ownerGrid, GridEntry peParent, object[] objectArray, PropertyDescriptor[] propInfo, bool hide)
        : base(ownerGrid, peParent, hide)
        {
            mergedPd = new MergePropertyDescriptor(propInfo);
            objs = objectArray;
            base.Initialize(mergedPd);
        }

        public override IContainer Container
        {
            get
            {
                IContainer c = null;

                foreach (object o in objs)
                {
                    if (!(o is IComponent comp))
                    {
                        c = null;
                        break;
                    }

                    if (comp.Site != null)
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
                bool fExpandable = GetFlagSet(FL_EXPANDABLE);

                if (fExpandable && ChildCollection.Count > 0)
                {
                    return true;
                }

                if (GetFlagSet(FL_EXPANDABLE_FAILED))
                {
                    return false;
                }

                try
                {
                    foreach (object o in mergedPd.GetValues(objs))
                    {
                        if (o is null)
                        {
                            fExpandable = false;
                            break;
                        }
                    }
                }
                catch
                {
                    fExpandable = false;
                }

                return fExpandable;
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
                if (mergedPd.PropertyType.IsValueType || (Flags & GridEntry.FLAG_IMMUTABLE) != 0)
                {
                    return base.CreateChildren(diffOldChildren);
                }

                ChildCollection.Clear();

                MultiPropertyDescriptorGridEntry[] mergedProps = MultiSelectRootGridEntry.PropertyMerger.GetMergedProperties(mergedPd.GetValues(objs), this, PropertySort, CurrentTab);

                Debug.WriteLineIf(CompModSwitches.DebugGridView.TraceVerbose && mergedProps is null, "PropertyGridView: MergedProps returned null!");

                if (mergedProps != null)
                {
                    ChildCollection.AddRange(mergedProps);
                }
                bool fExpandable = Children.Count > 0;
                if (!fExpandable)
                {
                    SetFlag(GridEntry.FL_EXPANDABLE_FAILED, true);
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
            if (mergedPd.PropertyType.IsValueType || (Flags & GridEntry.FLAG_IMMUTABLE) != 0)
            {
                return base.GetChildValueOwner(childEntry);
            }
            return mergedPd.GetValues(objs);
        }

        public override IComponent[] GetComponents()
        {
            IComponent[] temp = new IComponent[objs.Length];
            Array.Copy(objs, 0, temp, 0, objs.Length);
            return temp;
        }

        /// <summary>
        ///  Returns the text value of this property.
        /// </summary>
        public override string GetPropertyTextValue(object value)
        {
            bool allEqual = true;
            try
            {
                if (value is null && mergedPd.GetValue(objs, out allEqual) is null)
                {
                    if (!allEqual)
                    {
                        return "";
                    }
                }
            }
            catch
            {
                return "";
            }
            return base.GetPropertyTextValue(value);
        }

        internal override bool NotifyChildValue(GridEntry pe, int type)
        {
            bool success = false;

            IDesignerHost host = DesignerHost;
            DesignerTransaction trans = null;

            if (host != null)
            {
                trans = host.CreateTransaction();
            }
            try
            {
                success = base.NotifyChildValue(pe, type);
            }
            finally
            {
                if (trans != null)
                {
                    trans.Commit();
                }
            }
            return success;
        }

        protected override void NotifyParentChange(GridEntry ge)
        {
            // now see if we need to notify the parent(s) up the chain
            while (ge != null &&
                   ge is PropertyDescriptorGridEntry &&
                   ((PropertyDescriptorGridEntry)ge)._propertyInfo.Attributes.Contains(NotifyParentPropertyAttribute.Yes))
            {
                // find the next parent property with a differnet value owner
                object owner = ge.GetValueOwner();

                // find the next property descriptor with a different parent
                while (!(ge is PropertyDescriptorGridEntry) || OwnersEqual(owner, ge.GetValueOwner()))
                {
                    ge = ge.ParentGridEntry;
                    if (ge is null)
                    {
                        break;
                    }
                }

                // fire the change on that owner
                if (ge != null)
                {
                    owner = ge.GetValueOwner();

                    IComponentChangeService changeService = ComponentChangeService;

                    if (changeService != null)
                    {
                        if (owner is Array ownerArray)
                        {
                            for (int i = 0; i < ownerArray.Length; i++)
                            {
                                PropertyDescriptor pd = ((PropertyDescriptorGridEntry)ge)._propertyInfo;
                                ;

                                if (pd is MergePropertyDescriptor)
                                {
                                    pd = ((MergePropertyDescriptor)pd)[i];
                                }

                                if (pd != null)
                                {
                                    changeService.OnComponentChanging(ownerArray.GetValue(i), pd);
                                    changeService.OnComponentChanged(ownerArray.GetValue(i), pd, null, null);
                                }
                            }
                        }
                        else
                        {
                            changeService.OnComponentChanging(owner, ((PropertyDescriptorGridEntry)ge)._propertyInfo);
                            changeService.OnComponentChanged(owner, ((PropertyDescriptorGridEntry)ge)._propertyInfo, null, null);
                        }
                    }
                }
            }
        }

        internal override bool NotifyValueGivenParent(object obj, int type)
        {
            if (obj is ICustomTypeDescriptor)
            {
                obj = ((ICustomTypeDescriptor)obj).GetPropertyOwner(_propertyInfo);
            }

            switch (type)
            {
                case NOTIFY_RESET:

                    object[] objects = (object[])obj;

                    if (objects != null && objects.Length > 0)
                    {
                        IDesignerHost host = DesignerHost;
                        DesignerTransaction trans = null;

                        if (host != null)
                        {
                            trans = host.CreateTransaction(string.Format(SR.PropertyGridResetValue, PropertyName));
                        }
                        try
                        {
                            bool needChangeNotify = !(objects[0] is IComponent) || ((IComponent)objects[0]).Site is null;
                            if (needChangeNotify)
                            {
                                if (!OnComponentChanging())
                                {
                                    if (trans != null)
                                    {
                                        trans.Cancel();
                                        trans = null;
                                    }
                                    return false;
                                }
                            }

                            mergedPd.ResetValue(obj);

                            if (needChangeNotify)
                            {
                                OnComponentChanged();
                            }
                            NotifyParentChange(this);
                        }
                        finally
                        {
                            if (trans != null)
                            {
                                trans.Commit();
                            }
                        }
                    }
                    return false;
                case NOTIFY_DBL_CLICK:
                case NOTIFY_RETURN:
                    Debug.Assert(_propertyInfo is MergePropertyDescriptor, "Did not get a MergePropertyDescriptor!!!");
                    Debug.Assert(obj is object[], "Did not get an array of objects!!");

                    if (_propertyInfo is MergePropertyDescriptor mpd)
                    {
                        object[] objs = (object[])obj;

                        if (_eventBindings is null)
                        {
                            _eventBindings = (IEventBindingService)GetService(typeof(IEventBindingService));
                        }

                        if (_eventBindings != null)
                        {
                            EventDescriptor descriptor = _eventBindings.GetEvent(mpd[0]);
                            if (descriptor != null)
                            {
                                return ViewEvent(obj, null, descriptor, true);
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
            if (ComponentChangeService != null)
            {
                int cLength = objs.Length;
                for (int i = 0; i < cLength; i++)
                {
                    try
                    {
                        ComponentChangeService.OnComponentChanging(objs[i], mergedPd[i]);
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
            if (ComponentChangeService != null)
            {
                int cLength = objs.Length;
                for (int i = 0; i < cLength; i++)
                {
                    ComponentChangeService.OnComponentChanged(objs[i], mergedPd[i], null, null);
                }
            }
        }
    }
}
