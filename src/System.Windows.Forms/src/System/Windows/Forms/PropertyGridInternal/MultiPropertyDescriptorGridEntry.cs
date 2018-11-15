// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


namespace System.Windows.Forms.PropertyGridInternal {
    using System.Runtime.Serialization.Formatters;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System;
    using System.Collections;
    using System.Reflection;    
    using System.ComponentModel.Design;
    using System.Windows.Forms;
    using System.Drawing;
    using Microsoft.Win32;

    internal class MultiPropertyDescriptorGridEntry : PropertyDescriptorGridEntry {


        private MergePropertyDescriptor mergedPd;
        private object[]                objs;

        [
            SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")  // GridEntry classes are internal so we have complete
                                                                                                    // control over who does what in the constructor.
        ]
        public MultiPropertyDescriptorGridEntry(PropertyGrid ownerGrid, GridEntry peParent, object[] objectArray, PropertyDescriptor[] propInfo, bool hide) 
        : base (ownerGrid, peParent, hide) {
            mergedPd = new MergePropertyDescriptor(propInfo);
            this.objs = objectArray;
            base.Initialize(mergedPd);
        }

        public override IContainer Container {
            get {
                IContainer c = null;

                foreach (object o in objs) {
                    IComponent comp = o as IComponent;
                    if (comp == null) {
                        c = null;
                        break;
                    }

                    if (comp.Site != null) {
                        if (c == null) {
                            c = comp.Site.Container;
                            continue;
                        }
                        else if (c == comp.Site.Container) {
                            continue;
                        }
                    }
                    c = null;
                    break;
                }
                return c;
            }
        }

        public override bool Expandable {
            get {
                bool fExpandable = GetFlagSet(FL_EXPANDABLE);

                if (fExpandable && ChildCollection.Count > 0) {
                    return true;
                }

                if (GetFlagSet(FL_EXPANDABLE_FAILED)) {
                    return false;
                }

                try {
                    foreach (object o in mergedPd.GetValues(objs)) {
                        if (o == null) {
                            fExpandable = false;
                            break;
                        }
                    }
                }
                catch {
                    fExpandable = false;
                }

                return fExpandable;
            }
        }

        public override object PropertyValue{
            set {
                base.PropertyValue = value;

                RecreateChildren();
                if (Expanded)
                    GridEntryHost.Refresh(false);

            }
        }

        protected override bool CreateChildren() {
            return CreateChildren(false);
        }

        protected override bool CreateChildren(bool diffOldChildren) {
            try {

                if (mergedPd.PropertyType.IsValueType || (Flags & GridEntry.FLAG_IMMUTABLE) != 0) {
                    return base.CreateChildren(diffOldChildren);
                }

                ChildCollection.Clear();

                MultiPropertyDescriptorGridEntry[] mergedProps = MultiSelectRootGridEntry.PropertyMerger.GetMergedProperties(mergedPd.GetValues(objs), this, this.PropertySort, this.CurrentTab);

                Debug.WriteLineIf(CompModSwitches.DebugGridView.TraceVerbose && mergedProps == null, "PropertyGridView: MergedProps returned null!");

                if (mergedProps != null) {
                    ChildCollection.AddRange(mergedProps);
                }
                bool fExpandable = this.Children.Count > 0;
                if (!fExpandable) {
                    SetFlag(GridEntry.FL_EXPANDABLE_FAILED,true);
                }
                return fExpandable;
            }
            catch {
                return false;
            }
        }

        public override object GetChildValueOwner(GridEntry childEntry) {
            if (mergedPd.PropertyType.IsValueType || (Flags & GridEntry.FLAG_IMMUTABLE) != 0) {
                return base.GetChildValueOwner(childEntry);
            }
            return mergedPd.GetValues(objs);
        }

        public override IComponent[] GetComponents() {
            IComponent[] temp = new IComponent[objs.Length];
            Array.Copy(objs, 0, temp, 0, objs.Length);
            return temp;
        }

        /// <include file='doc\MultiSelectPropertyGridEntry.uex' path='docs/doc[@for="MultiSelectPropertyGridEntry.GetPropertyTextValue"]/*' />
        /// <devdoc>
        /// Returns the text value of this property.
        /// </devdoc>
        public override string GetPropertyTextValue(object value) {

            bool allEqual = true;
            try{
                if (value == null && mergedPd.GetValue(objs, out allEqual) == null) {
                    if (!allEqual) {
                        return "";
                    }
                }
            }
            catch{
                return "";
            }
            return base.GetPropertyTextValue(value);
        }

        internal override bool NotifyChildValue(GridEntry pe, int type) {
            bool success = false;

            IDesignerHost host = DesignerHost;
            DesignerTransaction trans = null;

            if (host != null) {
                trans = host.CreateTransaction();
            }
            try {
                success = base.NotifyChildValue(pe, type);
            }
            finally {
                if (trans != null) {
                    trans.Commit();
                }
            }
            return success;
        }

        protected override void NotifyParentChange(GridEntry ge) {
            // now see if we need to notify the parent(s) up the chain
            while (ge != null &&
                   ge is PropertyDescriptorGridEntry &&
                   ((PropertyDescriptorGridEntry)ge).propertyInfo.Attributes.Contains(NotifyParentPropertyAttribute.Yes)) {

                // find the next parent property with a differnet value owner
                object owner = ge.GetValueOwner();

                // find the next property descriptor with a different parent
                while (!(ge is PropertyDescriptorGridEntry) || OwnersEqual(owner, ge.GetValueOwner())) {
                    ge = ge.ParentGridEntry;
                    if (ge == null) {
                        break;
                    }
                }

                // fire the change on that owner
                if (ge != null) {
                    owner = ge.GetValueOwner();

                    IComponentChangeService changeService = ComponentChangeService;

                    if (changeService != null) {
                        Array ownerArray = owner as Array;
                        if (ownerArray != null) {
                            for (int i = 0; i < ownerArray.Length; i++) {
                                PropertyDescriptor pd = ((PropertyDescriptorGridEntry)ge).propertyInfo;;

                                if (pd is MergePropertyDescriptor) {
                                    pd = ((MergePropertyDescriptor)pd)[i];
                                }

                                if (pd != null) {
                                    changeService.OnComponentChanging(ownerArray.GetValue(i), pd);
                                    changeService.OnComponentChanged(ownerArray.GetValue(i), pd, null, null);
                                }
                            }
                        }
                        else {
                            changeService.OnComponentChanging(owner, ((PropertyDescriptorGridEntry)ge).propertyInfo);
                            changeService.OnComponentChanged(owner, ((PropertyDescriptorGridEntry)ge).propertyInfo, null, null);
                        }
                    }
                }
            }
        }

        internal override bool NotifyValueGivenParent(object obj, int type) {
            if (obj is ICustomTypeDescriptor) {
                obj = ((ICustomTypeDescriptor)obj).GetPropertyOwner(propertyInfo);
            }

            switch (type) {
                case NOTIFY_RESET:

                    object[] objects = (object[])obj;

                    if (objects != null && objects.Length > 0) {
                
                        IDesignerHost host = DesignerHost;
                        DesignerTransaction trans = null;
            
                        if (host != null) {
                            trans = host.CreateTransaction(string.Format(SR.PropertyGridResetValue, this.PropertyName));
                        }
                        try {
                            
                            bool needChangeNotify = !(objects[0] is IComponent) || ((IComponent)objects[0]).Site == null;
                            if (needChangeNotify) {
                                if (!OnComponentChanging()) {
                                    if (trans != null) {
                                        trans.Cancel();
                                        trans = null;
                                    }
                                    return false;
                                }
                            }
    
                            this.mergedPd.ResetValue(obj);
    
                            if (needChangeNotify) {
                                OnComponentChanged();
                            }
                            NotifyParentChange(this);
                        }
                        finally {
                            if (trans != null) {
                                trans.Commit();
                            }
                        }
                    } 
                    return false;
                case NOTIFY_DBL_CLICK:
                case NOTIFY_RETURN:
                    Debug.Assert(propertyInfo is MergePropertyDescriptor, "Did not get a MergePropertyDescriptor!!!");
                    Debug.Assert(obj is object[], "Did not get an array of objects!!");

                    MergePropertyDescriptor mpd = propertyInfo as MergePropertyDescriptor;
                    
                    if (mpd != null) {
                        object[] objs = (object[])obj;

                        if (eventBindings == null) {
                            eventBindings = (IEventBindingService)GetService(typeof(IEventBindingService));
                        }

                        if (eventBindings != null) {
                            EventDescriptor descriptor = eventBindings.GetEvent(mpd[0]);
                            if (descriptor != null) {
                                return ViewEvent(obj, null, descriptor, true);
                            }
                        }
                    
                        return false;
                    }
                    else {
                        return base.NotifyValueGivenParent(obj, type);
                    }
            }

            return base.NotifyValueGivenParent(obj, type);
        }

        private bool OwnersEqual(object owner1, object owner2) {
            if (!(owner1 is Array)) {
                return owner1 == owner2;
            }
            else {
                Array a1 = owner1 as Array;
                Array a2 = owner2 as Array;

                if (a1 != null && a2 != null && a1.Length == a2.Length) {
                    for (int i = 0; i < a1.Length; i++) {
                        if (a1.GetValue(i) != a2.GetValue(i)) {
                            return false;
                        }
                    }
                    return true;
                }
                return false;
            }
        }


        public override bool OnComponentChanging() {
            if (ComponentChangeService != null) {
                int cLength = objs.Length;
                for (int i = 0; i < cLength; i++) {
                    try {
                        ComponentChangeService.OnComponentChanging(objs[i], mergedPd[i]);
                    }
                    catch (CheckoutException co) {
                        if (co == CheckoutException.Canceled) {
                            return false;
                        }
                        throw co;
                    }
                }
            }
            return true;
        }

        public override void OnComponentChanged() {
            if (ComponentChangeService != null) {
                int cLength = objs.Length;
                for (int i = 0; i < cLength; i++) {
                    ComponentChangeService.OnComponentChanged(objs[i], mergedPd[i], null, null);
                }
            }
        }

    }
}
