// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms {

    using System;
    using System.Windows.Forms;
    using System.Diagnostics.CodeAnalysis;
    using System.ComponentModel;
    using System.Collections;


    public class PropertyManager : BindingManagerBase {

        // PropertyManager class
        //

        private object dataSource;
        private string propName;
        private PropertyDescriptor propInfo;
        private bool bound;



        public override object Current {
            get {
                return this.dataSource;
            }
        }

        private void PropertyChanged(object sender, EventArgs ea) {
            EndCurrentEdit();
            OnCurrentChanged(EventArgs.Empty);
        }

        internal override void SetDataSource(object dataSource) {
            if (this.dataSource != null && !string.IsNullOrEmpty(this.propName)) {
                propInfo.RemoveValueChanged(this.dataSource, new EventHandler(PropertyChanged));
                propInfo = null;
            }

            this.dataSource = dataSource;

            if (this.dataSource != null && !string.IsNullOrEmpty(this.propName)) {
                propInfo = TypeDescriptor.GetProperties(dataSource).Find(propName, true);
                if (propInfo == null)
                    throw new ArgumentException(string.Format(SR.PropertyManagerPropDoesNotExist, propName, dataSource.ToString()));
                propInfo.AddValueChanged(dataSource, new EventHandler(PropertyChanged));
            }
        }


        public PropertyManager() {}

        [
            SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")  // If the constructor does not set the dataSource
                                                                                                    // it would be a breaking change.
        ]
        internal PropertyManager(object dataSource) : base(dataSource){}

        [
            SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")  // If the constructor does not set the dataSource
                                                                                                    // it would be a breaking change.
        ]
        internal PropertyManager(object dataSource, string propName) : base() {
            this.propName = propName;
            this.SetDataSource(dataSource);
        }

        internal override PropertyDescriptorCollection GetItemProperties(PropertyDescriptor[] listAccessors) {
            return ListBindingHelper.GetListItemProperties(dataSource, listAccessors);
        }

        internal override Type BindType {
            get {
                return dataSource.GetType();
            }
        }

        internal override string GetListName() {
            return TypeDescriptor.GetClassName(dataSource) + "." + propName;
        }


        public override void SuspendBinding() {
            EndCurrentEdit();
            if (bound) {
                try {
                    bound = false;
                    UpdateIsBinding();
                } catch {
                    bound = true;
                    UpdateIsBinding();
                    throw;
                }
            }
        }


        public override void ResumeBinding() {
            OnCurrentChanged(new EventArgs());
            if (!bound) {
                try {
                    bound = true;
                    UpdateIsBinding();
                } catch {
                    bound = false;
                    UpdateIsBinding();
                    throw;
                }
            }
        }


        protected internal override string GetListName(ArrayList listAccessors) {
            return "";
        }


        public override void CancelCurrentEdit() {
            IEditableObject obj = this.Current as IEditableObject;
            if (obj != null)
                obj.CancelEdit();
            PushData();
        }


        public override void EndCurrentEdit() {
            bool success;
            PullData(out success);

            if (success) {
                IEditableObject obj = this.Current as IEditableObject;
                if (obj != null)
                    obj.EndEdit();
            }
        }


        protected override void UpdateIsBinding() {
            for (int i = 0; i < this.Bindings.Count; i++)
                this.Bindings[i].UpdateIsBinding();
        }


        internal protected override void OnCurrentChanged(EventArgs ea) {
            PushData();

            if (this.onCurrentChangedHandler != null)
                this.onCurrentChangedHandler(this, ea);

            if (this.onCurrentItemChangedHandler != null)
                this.onCurrentItemChangedHandler(this, ea);
        }


        internal protected override void OnCurrentItemChanged(EventArgs ea) {
            PushData();

            if (this.onCurrentItemChangedHandler != null)
                this.onCurrentItemChangedHandler(this, ea);
        }

        internal override object DataSource {
            get {
                return this.dataSource;
            }
        }

        internal override bool IsBinding {
            get {
                return (dataSource != null);
            }
        }

        // no op on the propertyManager

        public override int Position {
            get {
                return 0;
            }
            set {
            }
        }


        public override int Count {
            get {
                return 1;
            }
        }

        // no-op on the propertyManager

        public override void AddNew() {
            throw new NotSupportedException(SR.DataBindingAddNewNotSupportedOnPropertyManager);
        }

        // no-op on the propertyManager

        public override void RemoveAt(int index) {
            throw new NotSupportedException(SR.DataBindingRemoveAtNotSupportedOnPropertyManager);
        }
    }
}
