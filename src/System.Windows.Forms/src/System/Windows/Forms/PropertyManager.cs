// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms {

    using System;
    using System.Windows.Forms;
    using System.Diagnostics.CodeAnalysis;
    using System.ComponentModel;
    using System.Collections;

    /// <include file='doc\PropertyManager.uex' path='docs/doc[@for="PropertyManager"]/*' />
    public class PropertyManager : BindingManagerBase {

        // PropertyManager class
        //

        private object dataSource;
        private string propName;
        private PropertyDescriptor propInfo;
        private bool bound;


        /// <include file='doc\PropertyManager.uex' path='docs/doc[@for="PropertyManager.Current"]/*' />
        public override Object Current {
            get {
                return this.dataSource;
            }
        }

        private void PropertyChanged(object sender, EventArgs ea) {
            EndCurrentEdit();
            OnCurrentChanged(EventArgs.Empty);
        }

        internal override void SetDataSource(Object dataSource) {
            if (this.dataSource != null && !String.IsNullOrEmpty(this.propName)) {
                propInfo.RemoveValueChanged(this.dataSource, new EventHandler(PropertyChanged));
                propInfo = null;
            }

            this.dataSource = dataSource;

            if (this.dataSource != null && !String.IsNullOrEmpty(this.propName)) {
                propInfo = TypeDescriptor.GetProperties(dataSource).Find(propName, true);
                if (propInfo == null)
                    throw new ArgumentException(string.Format(SR.PropertyManagerPropDoesNotExist, propName, dataSource.ToString()));
                propInfo.AddValueChanged(dataSource, new EventHandler(PropertyChanged));
            }
        }

        /// <include file='doc\PropertyManager.uex' path='docs/doc[@for="PropertyManager.PropertyManager"]/*' />
        public PropertyManager() {}

        [
            SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")  // If the constructor does not set the dataSource
                                                                                                    // it would be a breaking change.
        ]
        internal PropertyManager(Object dataSource) : base(dataSource){}

        [
            SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")  // If the constructor does not set the dataSource
                                                                                                    // it would be a breaking change.
        ]
        internal PropertyManager(Object dataSource, string propName) : base() {
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

        internal override String GetListName() {
            return TypeDescriptor.GetClassName(dataSource) + "." + propName;
        }

        /// <include file='doc\PropertyManager.uex' path='docs/doc[@for="PropertyManager.SuspendBinding"]/*' />
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

        /// <include file='doc\PropertyManager.uex' path='docs/doc[@for="PropertyManager.ResumeBinding"]/*' />
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

        /// <include file='doc\PropertyManager.uex' path='docs/doc[@for="PropertyManager.GetListName"]/*' />
        protected internal override String GetListName(ArrayList listAccessors) {
            return "";
        }

        /// <include file='doc\PropertyManager.uex' path='docs/doc[@for="PropertyManager.CancelCurrentEdit"]/*' />
        public override void CancelCurrentEdit() {
            IEditableObject obj = this.Current as IEditableObject;
            if (obj != null)
                obj.CancelEdit();
            PushData();
        }

        /// <include file='doc\PropertyManager.uex' path='docs/doc[@for="PropertyManager.EndCurrentEdit"]/*' />
        public override void EndCurrentEdit() {
            bool success;
            PullData(out success);

            if (success) {
                IEditableObject obj = this.Current as IEditableObject;
                if (obj != null)
                    obj.EndEdit();
            }
        }

        /// <include file='doc\PropertyManager.uex' path='docs/doc[@for="PropertyManager.UpdateIsBinding"]/*' />
        protected override void UpdateIsBinding() {
            for (int i = 0; i < this.Bindings.Count; i++)
                this.Bindings[i].UpdateIsBinding();
        }

        /// <include file='doc\PropertyManager.uex' path='docs/doc[@for="PropertyManager.OnCurrentChanged"]/*' />
        internal protected override void OnCurrentChanged(EventArgs ea) {
            PushData();

            if (this.onCurrentChangedHandler != null)
                this.onCurrentChangedHandler(this, ea);

            if (this.onCurrentItemChangedHandler != null)
                this.onCurrentItemChangedHandler(this, ea);
        }

        /// <include file='doc\PropertyManager.uex' path='docs/doc[@for="PropertyManager.OnCurrentItemChanged"]/*' />
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
        /// <include file='doc\PropertyManager.uex' path='docs/doc[@for="PropertyManager.Position"]/*' />
        public override int Position {
            get {
                return 0;
            }
            set {
            }
        }

        /// <include file='doc\PropertyManager.uex' path='docs/doc[@for="PropertyManager.Count"]/*' />
        public override int Count {
            get {
                return 1;
            }
        }

        // no-op on the propertyManager
        /// <include file='doc\PropertyManager.uex' path='docs/doc[@for="PropertyManager.AddNew"]/*' />
        public override void AddNew() {
            throw new NotSupportedException(SR.DataBindingAddNewNotSupportedOnPropertyManager);
        }

        // no-op on the propertyManager
        /// <include file='doc\PropertyManager.uex' path='docs/doc[@for="PropertyManager.RemoveAt"]/*' />
        public override void RemoveAt(int index) {
            throw new NotSupportedException(SR.DataBindingRemoveAtNotSupportedOnPropertyManager);
        }
    }
}
