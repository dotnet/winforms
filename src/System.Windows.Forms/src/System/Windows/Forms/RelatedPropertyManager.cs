// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms {

    using System;
    using Microsoft.Win32;
    using System.Diagnostics;    
    using System.Diagnostics.CodeAnalysis;
    using System.ComponentModel;
    using System.Collections;

    internal class RelatedPropertyManager : PropertyManager {

        BindingManagerBase parentManager;
        string dataField;
        PropertyDescriptor fieldInfo;
        
        [
            SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")  // If the constructor does not set the dataSource
                                                                                                    // it would be a breaking change.
        ]
        internal RelatedPropertyManager(BindingManagerBase parentManager, string dataField) : base(GetCurrentOrNull(parentManager), dataField) {
            Bind(parentManager, dataField);
        }

        private void Bind(BindingManagerBase parentManager, string dataField) {
            Debug.Assert(parentManager != null, "How could this be a null parentManager.");
            this.parentManager = parentManager;
            this.dataField = dataField;
            this.fieldInfo = parentManager.GetItemProperties().Find(dataField, true);
            if (fieldInfo == null)
                throw new ArgumentException(string.Format(SR.RelatedListManagerChild, dataField));
            // this.finalType = fieldInfo.PropertyType;
            parentManager.CurrentItemChanged += new EventHandler(ParentManager_CurrentItemChanged);
            Refresh();
        }

        internal override string GetListName() {
            string name = GetListName(new ArrayList());
            if (name.Length > 0) {
                return name;
            }
            return base.GetListName();
        }
        
        protected internal override string GetListName(ArrayList listAccessors) {
            listAccessors.Insert(0, fieldInfo);
            return parentManager.GetListName(listAccessors);
        }
        
        internal override PropertyDescriptorCollection GetItemProperties(PropertyDescriptor[] listAccessors) {
            PropertyDescriptor[] accessors;

            if (listAccessors != null && listAccessors.Length > 0) {
                accessors = new PropertyDescriptor[listAccessors.Length + 1];
                listAccessors.CopyTo(accessors, 1);
            }
            else {
                accessors = new PropertyDescriptor[1];
            }

            // Set this accessor (add to the beginning)
            accessors[0] = this.fieldInfo;

            // Get props
            return parentManager.GetItemProperties(accessors);
        }

        private void ParentManager_CurrentItemChanged(object sender, EventArgs e) {
            Refresh();
        }

        private void Refresh() {
            EndCurrentEdit();
            SetDataSource(GetCurrentOrNull(parentManager));
            OnCurrentChanged(EventArgs.Empty);
        }

        
        internal override Type BindType {
            get {
                return fieldInfo.PropertyType;
            }
        }

        public override object Current {
            get {
                return (this.DataSource != null) ? fieldInfo.GetValue(this.DataSource) : null;
            }
        }

        static private object GetCurrentOrNull(BindingManagerBase parentManager) {
            bool anyCurrent = (parentManager.Position >= 0 && parentManager.Position < parentManager.Count);
            return anyCurrent ? parentManager.Current : null;
        }

    }
}
