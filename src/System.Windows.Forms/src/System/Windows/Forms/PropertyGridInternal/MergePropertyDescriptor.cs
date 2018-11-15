// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


namespace System.Windows.Forms.PropertyGridInternal {
    using System.Runtime.Serialization.Formatters;
    using System.Runtime.Serialization.Formatters.Binary;
    using System.ComponentModel;
    using System.Diagnostics;
    using System;
    using System.IO;
    using System.Collections;
    using System.Globalization;
    using System.Reflection;    
    using System.ComponentModel.Design;
    using System.ComponentModel.Design.Serialization;
    using System.Windows.Forms;
    using System.Drawing;
    using Microsoft.Win32;
    
    internal class MergePropertyDescriptor : PropertyDescriptor {

        private PropertyDescriptor[] descriptors;

        private enum TriState {
                Unknown,
                Yes,
                No
        }

        private TriState localizable = TriState.Unknown;
        private TriState readOnly = TriState.Unknown;
        private TriState canReset = TriState.Unknown;

        private MultiMergeCollection collection;

        
        public MergePropertyDescriptor(PropertyDescriptor[] descriptors) : base(descriptors[0].Name, null)  {
            this.descriptors = descriptors;
        }


        /// <include file='doc\PropertyDescriptor.uex' path='docs/doc[@for="PropertyDescriptor.ComponentType"]/*' />
        /// <devdoc>
        ///    <para>
        ///       When overridden in a derived class, gets the type of the
        ///       component this property
        ///       is bound to.
        ///    </para>
        /// </devdoc>
        public override Type ComponentType {
                get {
                    return descriptors[0].ComponentType;
                }
        }

        /// <include file='doc\PropertyDescriptor.uex' path='docs/doc[@for="PropertyDescriptor.Converter"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Gets the type converter for this property.
        ///    </para>
        /// </devdoc>
        public override TypeConverter Converter {
            get {
                return descriptors[0].Converter;
            }
        }

        public override string DisplayName {
            get {
                return descriptors[0].DisplayName;
            }
        }

        /// <include file='doc\PropertyDescriptor.uex' path='docs/doc[@for="PropertyDescriptor.IsLocalizable"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Gets a value
        ///       indicating whether this property should be localized, as
        ///       specified in the <see cref='System.ComponentModel.LocalizableAttribute'/>.
        ///    </para>
        /// </devdoc>
        public override bool IsLocalizable {
            get {
                if (localizable == TriState.Unknown) {
                    localizable = TriState.Yes;
                    foreach (PropertyDescriptor pd in descriptors) {
                        if (!pd.IsLocalizable) {
                            localizable = TriState.No;
                            break;
                        }
                    }
                }
                return (localizable == TriState.Yes);
            }
        }

        /// <include file='doc\PropertyDescriptor.uex' path='docs/doc[@for="PropertyDescriptor.IsReadOnly"]/*' />
        /// <devdoc>
        ///    <para>
        ///       When overridden in
        ///       a derived class, gets a value
        ///       indicating whether this property is read-only.
        ///    </para>
        /// </devdoc>
        public override bool IsReadOnly { 
            get {
                if (readOnly == TriState.Unknown) {
                    readOnly = TriState.No;
                    foreach (PropertyDescriptor pd in descriptors) {
                        if (pd.IsReadOnly) {
                            readOnly = TriState.Yes;
                            break;
                        }
                    }
                }
                return (readOnly == TriState.Yes);
            }
        }

   
        /// <include file='doc\PropertyDescriptor.uex' path='docs/doc[@for="PropertyDescriptor.PropertyType"]/*' />
        /// <devdoc>
        ///    <para>
        ///       When overridden in a derived class,
        ///       gets the type of the property.
        ///    </para>
        /// </devdoc>
        public override Type PropertyType { 
            get {
                return descriptors[0].PropertyType;
            }
        }

        public PropertyDescriptor this[int index] {
            get {
                return descriptors[index];
            }
        }

        /// <include file='doc\PropertyDescriptor.uex' path='docs/doc[@for="PropertyDescriptor.CanResetValue"]/*' />
        /// <devdoc>
        ///    <para>
        ///       When overridden in a derived class, indicates whether
        ///       resetting the <paramref name="component "/>will change the value of the
        ///    <paramref name="component"/>.
        /// </para>
        /// </devdoc>
        public override bool CanResetValue(object component) {
            Debug.Assert(component is Array, "MergePropertyDescriptor::CanResetValue called with non-array value");
            if (canReset == TriState.Unknown) {
                 canReset = TriState.Yes;
                 Array a = (Array)component;
                 for (int i = 0; i < descriptors.Length; i++) {
                     if (!descriptors[i].CanResetValue(GetPropertyOwnerForComponent(a, i))) {
                         canReset = TriState.No;
                         break;
                     }
                 }
                 
             }
             return (canReset == TriState.Yes);
        }

        /// <devdoc>
        ///     This method attempts to copy the given value so unique values are
        ///     always passed to each object.  If the object cannot be copied it
        ///     will be returned.
        /// </devdoc>
        private object CopyValue(object value) {

            // null is always OK
            if (value == null) {
                return value;
            }

            Type type = value.GetType();

            // value types are always copies
            if (type.IsValueType) {
                return value;
            }

            object clonedValue = null;

            // ICloneable is the next easiest thing
            ICloneable clone = value as ICloneable;
            if (clone != null) {
                clonedValue = clone.Clone();
            }

            // Next, access the type converter
            if (clonedValue == null) {
                TypeConverter converter = TypeDescriptor.GetConverter(value);
                if (converter.CanConvertTo(typeof(InstanceDescriptor))) {
                    // Instance descriptors provide full fidelity unless
                    // they are marked as incomplete.
                    InstanceDescriptor desc = (InstanceDescriptor)converter.ConvertTo(null, CultureInfo.InvariantCulture, value, typeof(InstanceDescriptor));
                    if (desc != null && desc.IsComplete) {
                        clonedValue = desc.Invoke();
                    }
                }

                // If that didn't work, try conversion to/from string
                if (clonedValue == null && converter.CanConvertTo(typeof(string)) && converter.CanConvertFrom(typeof(string))) {
                    object stringRep = converter.ConvertToInvariantString(value);
                    clonedValue = converter.ConvertFromInvariantString((string)stringRep);
                }
            }

            

            // How about serialization?
            if (clonedValue == null && type.IsSerializable) {
                BinaryFormatter f = new BinaryFormatter();
                MemoryStream ms = new MemoryStream();
                f.Serialize(ms, value);
                ms.Position = 0;
                clonedValue = f.Deserialize(ms);
            }

            if (clonedValue != null) {
                return clonedValue;
            }

            // we failed.  This object's reference will be set on each property.
            return value;
        }

         /// <include file='doc\MemberDescriptor.uex' path='docs/doc[@for="MemberDescriptor.CreateAttributeCollection"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Creates a collection of attributes using the
        ///       array of attributes that you passed to the constructor.
        ///    </para>
        /// </devdoc>
        protected override AttributeCollection CreateAttributeCollection() {
            return new MergedAttributeCollection(this);
        }


        private object GetPropertyOwnerForComponent(Array a, int i) {
            object propertyOwner = a.GetValue(i);
            if (propertyOwner is ICustomTypeDescriptor) {
                propertyOwner = ((ICustomTypeDescriptor) propertyOwner).GetPropertyOwner(descriptors[i]);
            }
            return propertyOwner;
        }

        /// <include file='doc\PropertyDescriptor.uex' path='docs/doc[@for="PropertyDescriptor.GetEditor"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Gets an editor of the specified type.
        ///    </para>
        /// </devdoc>
        public override object GetEditor(Type editorBaseType) {
            return descriptors[0].GetEditor(editorBaseType);
        }


        /// <include file='doc\PropertyDescriptor.uex' path='docs/doc[@for="PropertyDescriptor.GetValue"]/*' />
        /// <devdoc>
        ///    <para>
        ///       When overridden in a derived class, gets the current
        ///       value
        ///       of the
        ///       property on a component.
        ///    </para>
        /// </devdoc>
        public override object GetValue(object component) {
            Debug.Assert(component is Array, "MergePropertyDescriptor::GetValue called with non-array value");
            bool temp;
            return GetValue((Array)component, out temp);
        }

        public object GetValue(Array components, out bool allEqual) {
            allEqual = true;
            object obj = descriptors[0].GetValue(GetPropertyOwnerForComponent(components, 0));
                    
            if (obj is ICollection) {
               if (collection == null) {
                   collection = new MultiMergeCollection((ICollection)obj);
               }
               else if (collection.Locked) {
                   return collection;
               }
               else {
                   collection.SetItems((ICollection)obj);
               }
            }
            
            for (int i = 1; i < descriptors.Length; i++) {
                object objCur = descriptors[i].GetValue(GetPropertyOwnerForComponent(components, i));

                if (collection != null) {
                   if (!collection.MergeCollection((ICollection)objCur)){
                      allEqual = false;
                      return null;
                   }
                }
                else if ((obj == null && objCur == null) ||
                         (obj != null && obj.Equals(objCur))) {
                
                   continue;
                }
                else {
                    allEqual = false;
                    return null;
                }
            }
            
            if (allEqual && collection != null && collection.Count == 0) {
                return null;
            }
            
            return (collection != null ? collection : obj);
        }

        internal object[] GetValues(Array components) {
            object[] values = new object[components.Length];

            for (int i = 0; i < components.Length; i++) {
                values[i] = descriptors[i].GetValue(GetPropertyOwnerForComponent(components, i));
            }
            return values;
        }

        /// <include file='doc\PropertyDescriptor.uex' path='docs/doc[@for="PropertyDescriptor.ResetValue"]/*' />
        /// <devdoc>
        ///    <para>
        ///       When overridden in a derived class, resets the
        ///       value
        ///       for this property
        ///       of the component.
        ///    </para>
        /// </devdoc>
        public override void ResetValue(object component) {

            Debug.Assert(component is Array, "MergePropertyDescriptor::ResetValue called with non-array value");
            Array a = (Array)component;
            for (int i = 0; i < descriptors.Length; i++) {
                descriptors[i].ResetValue(GetPropertyOwnerForComponent(a, i));
            }
        }

        private void SetCollectionValues(Array a, IList listValue) {
            
            try {
                if (collection != null) {
                   collection.Locked = true;
                }

                // now we have to copy the value into each property.
                object[] values = new object[listValue.Count];
                
                listValue.CopyTo(values, 0);
                
                for (int i = 0; i < descriptors.Length; i++) {
                    IList propList = descriptors[i].GetValue(GetPropertyOwnerForComponent(a, i)) as IList;

                    if (propList == null) {
                       continue;
                    }
                    
                    propList.Clear();
                    foreach (object val in values) {
                        propList.Add(val);
                    }
                }
            }
            finally {
               if (collection != null) {
                  collection.Locked = false;
               }
            }

        }

        /// <include file='doc\PropertyDescriptor.uex' path='docs/doc[@for="PropertyDescriptor.SetValue"]/*' />
        /// <devdoc>
        ///    <para>
        ///       When overridden in a derived class, sets the value of
        ///       the component to a different value.
        ///    </para>
        /// </devdoc>
        public override void SetValue(object component, object value) {
            Debug.Assert(component is Array, "MergePropertyDescriptor::SetValue called with non-array value");
            Array a = (Array)component;
            if (value is IList && typeof(IList).IsAssignableFrom(PropertyType)) {
                SetCollectionValues(a, (IList)value);
            }
            else {
                for (int i = 0; i < descriptors.Length; i++) {
                    object clonedValue = CopyValue(value);
                    descriptors[i].SetValue(GetPropertyOwnerForComponent(a, i), clonedValue);
                }
            }
        }

        /// <include file='doc\PropertyDescriptor.uex' path='docs/doc[@for="PropertyDescriptor.ShouldSerializeValue"]/*' />
        /// <devdoc>
        ///    <para>
        ///       When overridden in a derived class, indicates whether the
        ///       value of
        ///       this property needs to be persisted.
        ///    </para>
        /// </devdoc>

        public override bool ShouldSerializeValue(object component) {
            Debug.Assert(component is Array, "MergePropertyDescriptor::ShouldSerializeValue called with non-array value");
            Array a = (Array)component;
            for (int i = 0; i < descriptors.Length; i++) {
                if (!descriptors[i].ShouldSerializeValue(GetPropertyOwnerForComponent(a, i))) {
                    return false;
                }
            }
            return true;
        }

        private class MultiMergeCollection : ICollection {
        
            private object[] items;
            private bool     locked;
            
            public MultiMergeCollection(ICollection original) {
               SetItems(original);
            }
            
            /// <include file='doc\MultiSelectPropertyGridEntry.uex' path='docs/doc[@for="MultiSelectPropertyGridEntry.MultiMergeCollection.Count"]/*' />
            /// <devdoc>
            ///     Retrieves the number of items.
            /// </devdoc>
            public int Count {
                get {
                    if (items != null) {
                        return items.Length;
                    }
                    else {
                        return 0;
                    }
                }
            }
            
            
            /// <include file='doc\MultiSelectPropertyGridEntry.uex' path='docs/doc[@for="MultiSelectPropertyGridEntry.MultiMergeCollection.Locked"]/*' />
            /// <devdoc>
            ///     Prevents the contents of the collection from being re-initialized;
            /// </devdoc>
            public bool Locked {
               get {
                  return locked;
               }
               set {
                  this.locked = value;
               }
            }

            object ICollection.SyncRoot {
                get {
                    return this;
                }
            }

            bool ICollection.IsSynchronized {
                get {
                    return false;
                }
            }
            
            public void CopyTo(Array array, int index) {
               if (items == null) return;
               
               Array.Copy(items, 0, array, index, items.Length);
            }
            
            public IEnumerator GetEnumerator(){
               if (items != null) {
                  return items.GetEnumerator();
               }
               else {
                  return new object[0].GetEnumerator();
               }
            }
            
            /// <include file='doc\MultiSelectPropertyGridEntry.uex' path='docs/doc[@for="MultiSelectPropertyGridEntry.MultiMergeCollection.MergeCollection"]/*' />
            /// <devdoc>
            /// Ensures that the new collection equals the exisitng one.
            /// Otherwise, it wipes out the contents of the new collection.
            /// </devdoc>
            public bool MergeCollection(ICollection newCollection) {
                
                if (locked) {
                   return true;
                }
               
                if (items.Length != newCollection.Count) {
                     items = new object[0];
                     return false;
                }
                
                object[] newItems = new object[newCollection.Count];
                newCollection.CopyTo(newItems, 0);
                for (int i = 0;i < newItems.Length; i++) {
                     if (((newItems[i] == null) != (items[i] == null)) ||
                         (items[i] != null && !items[i].Equals(newItems[i]))){
                           items = new object[0];
                           return false;  
                         }
                         
                }
                return true;
            }
            
            public void SetItems(ICollection collection) {
                if (locked) {
                  return;
                }
                items = new object[collection.Count];
                collection.CopyTo(items, 0);
            }
            
        }

        private class MergedAttributeCollection : AttributeCollection {
            private MergePropertyDescriptor owner;

            private AttributeCollection[] attributeCollections = null;
            private IDictionary             foundAttributes = null;
            
            public MergedAttributeCollection(MergePropertyDescriptor owner) : base((Attribute[])null) {
                this.owner = owner;
            }

            public override Attribute this[Type attributeType] {
                get {
                    return GetCommonAttribute(attributeType);
                }
            }

            #if false
            private void FullMerge() {
                Attribute[][] collections = new Attribute[owner.descriptors.Length][];
                for (int i = 0; i < owner.descriptors.Length; i++) {
                    AttributeCollection attrCollection = owner.descriptors[i].Attributes;
                    collections[i] = new Attribute[attrCollection.Count];
                    attrCollection.CopyTo(collections[i], 0);
                    Array.Sort(collections[i], GridEntry.AttributeTypeSorter);
                }
                
                ArrayList mergedList = new ArrayList();
    
                // merge the sorted lists -- note that lists aren't fully sorted just by
                // Attribute.TypeId
                //
                int[] posArray = new int[collections.Length];
                for (int i = 0; i < collections[0].Length; i++) {
                    Attribute pivotAttr = collections[0][i];
                    bool match = true;
                    for (int j = 1; j < collections.Length; j++) {
    
                        if (posArray[j] >= collections[j].Length) {
                            match = false;
                            break;
                        }
    
                        // check to see if we're on a match
                        //
                        if (pivotAttr.Equals(collections[j][posArray[j]])) {
                            posArray[j] += 1;
                            continue;
                        }
    
                        int jPos = posArray[j];
                        Attribute jAttr = collections[j][jPos];
    
                        match = false;
    
                        // if we aren't on a match, check all the items until we're past
                        // where the matching item would be
                        while (GridEntry.AttributeTypeSorter.Compare(jAttr, pivotAttr) <= 0) {
                            
                            // got a match!
                            if (pivotAttr.Equals(jAttr)) {
                                posArray[j] = jPos + 1;
                                match = true;
                                break;
                            }
    
                            // try again
                            jPos++;
                            if (jPos < collections[j].Length) {
                                jAttr = collections[j][jPos];
                            }
                            else {
                                break;
                            }
                        }
    
                        // if we got here, there is no match, quit for this guy
                        if (!match) {
                            posArray[j] = jPos;
                            break;
                        }
                    }
    
                    // do we have a match?
                    if (match) {
                        mergedList.Add(pivotAttr);
                    }
                }
    
                // create our merged array
                Attribute[] mergedAttrs = new Attribute[mergedList.Count];
                mergedList.CopyTo(mergedAttrs, 0);
            }

            #endif

            private Attribute GetCommonAttribute(Type attributeType) {
                if (attributeCollections == null) {
                    attributeCollections = new AttributeCollection[owner.descriptors.Length];
                    for (int i = 0; i < owner.descriptors.Length; i++) {
                        attributeCollections[i] = owner.descriptors[i].Attributes;
                    }
                }

                if (attributeCollections.Length == 0) {
                    return GetDefaultAttribute(attributeType);
                }

                Attribute value;
                if (foundAttributes != null) {
                    value = foundAttributes[attributeType] as Attribute;
                    if (value != null) {
                        return value;
                    }
                }

                value = attributeCollections[0][attributeType];

                if (value == null) {
                    return null;
                }
                
                for (int i = 1; i < attributeCollections.Length; i++) {
                    Attribute newValue = attributeCollections[i][attributeType];
                    if (!value.Equals(newValue)) {
                        value = GetDefaultAttribute(attributeType);
                        break;
                    }
                }

                if (foundAttributes == null) {
                    foundAttributes = new Hashtable();
                }
                foundAttributes[attributeType] = value;
                return value;
            }
        }
    }
}
