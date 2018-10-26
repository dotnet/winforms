// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms {

    using System;
    using Microsoft.Win32;
    using System.Diagnostics;    
    using System.ComponentModel;
    using System.ComponentModel.Design;
    using System.Collections;
   
    internal class BindToObject {
        private PropertyDescriptor fieldInfo;
        private BindingMemberInfo dataMember;
        private Object dataSource;
        private BindingManagerBase bindingManager;
        private Binding owner;
        private string errorText = string.Empty;

        private bool dataSourceInitialized = false;
        private bool waitingOnDataSource = false;

        private void PropValueChanged(object sender, EventArgs e) {
            if(this.bindingManager != null) {
                this.bindingManager.OnCurrentChanged(EventArgs.Empty);
            }
        }

        private bool IsDataSourceInitialized {
            get {
                Debug.Assert(this.dataSource != null, "how can we determine if DataSource is initialized or not if we have no data source?");

                if (this.dataSourceInitialized) {
                    return true;
                }

                ISupportInitializeNotification ds = this.dataSource as ISupportInitializeNotification;
                if (ds == null || ds.IsInitialized) {
                    this.dataSourceInitialized = true;
                    return true;
                }

                // We have an ISupportInitializeNotification which was not initialized yet.

                // We already hooked up the Initialized event and the data source is not initialized yet.
                if (this.waitingOnDataSource) {
                    return false;
                }

                // Hook up the Initialized event.
                ds.Initialized += new EventHandler(DataSource_Initialized);
                this.waitingOnDataSource = true;
                return false;
            }
        }

        internal BindToObject(Binding owner, Object dataSource, string dataMember) {
            this.owner = owner;
            this.dataSource = dataSource;
            this.dataMember = new BindingMemberInfo(dataMember);
            CheckBinding();
        }

        private void DataSource_Initialized(object sender, EventArgs e) {
            Debug.Assert(sender == this.dataSource, "data source should not change");
            Debug.Assert(this.dataSource is ISupportInitializeNotification, "data source should not change on the BindToObject");
            Debug.Assert(this.waitingOnDataSource);

            ISupportInitializeNotification ds = this.dataSource as ISupportInitializeNotification;
            // Unhook the Initialized event.
            if (ds != null) {
                ds.Initialized -= new EventHandler(DataSource_Initialized);
            }

            // The wait is over: DataSource is initialized.
            this.waitingOnDataSource = false;
            this.dataSourceInitialized = true;

            // Rebind.
            CheckBinding();
        }

        internal void SetBindingManagerBase(BindingManagerBase lManager) {
            if (bindingManager == lManager)
            {
                return;
            }

            // remove notification from the backEnd
            if (bindingManager != null && fieldInfo != null && bindingManager.IsBinding && !(bindingManager is CurrencyManager)) {
                fieldInfo.RemoveValueChanged(bindingManager.Current, new EventHandler(PropValueChanged));
                fieldInfo = null;
            }

            this.bindingManager = lManager;
            CheckBinding();
        }

        internal string DataErrorText
        {
            get
            {
                return this.errorText;
            }
        }

        // Returns any data error info on the data source for the bound data field in the current row
        string GetErrorText(object value) {

            IDataErrorInfo errorInfo = value as IDataErrorInfo;
            string text = string.Empty;

            if (errorInfo != null) {
                // Get the row error if there is no DataMember
                if (fieldInfo == null) {
                    text = errorInfo.Error;
                }
                // Get the column error if there is a DataMember.
                // The DataTable uses its own Locale to lookup column names <sigh>.
                // So passing the DataMember from the BindingField could cause problems.
                // Pass the name from the PropertyDescriptor that the DataTable gave us.
                // (If there is no fieldInfo, data binding would have failed already )
                else {
                    text = errorInfo[fieldInfo.Name];
                }
            }

            return text ?? string.Empty;
        }

        internal Object GetValue() {
            object obj = bindingManager.Current;

            // Update IDataErrorInfo text: it's ok to get this now because we're going to need
            // this as part of the BindingCompleteEventArgs anyway.
            this.errorText = GetErrorText(obj);

            if (fieldInfo != null) {
               obj = fieldInfo.GetValue(obj);
            }

            return obj;
        }

        internal Type BindToType {
            get {
                if (dataMember.BindingField.Length == 0) {
                    // if we are bound to a list w/o any properties, then
                    // take the type from the BindingManager
                    Type type = this.bindingManager.BindType;
                    if (typeof(Array).IsAssignableFrom(type))
                        type = type.GetElementType();
                    return type;
                }
                else
                    return fieldInfo == null ? null : fieldInfo.PropertyType;
            }
        }

        internal void SetValue(Object value) {
            object obj = null;

            if (fieldInfo != null) {
                obj = bindingManager.Current;
                if (obj is IEditableObject)
                    ((IEditableObject) obj).BeginEdit();
                //(
                if (!fieldInfo.IsReadOnly) {
                    fieldInfo.SetValue(obj, value);
                }
                
            }
            else {
                CurrencyManager cm = bindingManager as CurrencyManager;
                if (cm != null)
                {
                    cm[cm.Position] = value;
                    obj = value;
                }
            }

            // Update IDataErrorInfo text. 
            this.errorText = GetErrorText(obj);
        }

        internal BindingMemberInfo BindingMemberInfo {
            get {
                return this.dataMember;
            }
        }

        internal Object DataSource {
            get {
                return dataSource;
            }
        }

        internal PropertyDescriptor FieldInfo {
            get {
                return fieldInfo;
            }
        }

        internal BindingManagerBase BindingManagerBase {
            get {
                return this.bindingManager;
            }
        }

        internal void CheckBinding() {

            // At design time, don't check anything.
            //
            if (owner != null &&
                owner.BindableComponent != null &&
                owner.ControlAtDesignTime()) {

                return;
            }

            // force Column to throw if it's currently a bad column.
            //DataColumn tempColumn = this.Column;

            // remove propertyChangedNotification when this binding is deleted
            if (this.owner.BindingManagerBase != null &&
                this.fieldInfo != null &&
                this.owner.BindingManagerBase.IsBinding &&
                !(this.owner.BindingManagerBase is CurrencyManager)) {

                fieldInfo.RemoveValueChanged(owner.BindingManagerBase.Current, new EventHandler(PropValueChanged));
            }

            if (owner != null &&
                owner.BindingManagerBase != null &&
                owner.BindableComponent != null &&
                owner.ComponentCreated && 
                this.IsDataSourceInitialized) {

                string dataField = dataMember.BindingField;

                fieldInfo = owner.BindingManagerBase.GetItemProperties().Find(dataField, true);
                if (owner.BindingManagerBase.DataSource != null && fieldInfo == null && dataField.Length > 0) {
                    throw new ArgumentException(string.Format(SR.ListBindingBindField, dataField), "dataMember");
                }

                // Do not add propertyChange notification if
                // the fieldInfo is null                
                //
                // we add an event handler to the dataSource in the BindingManagerBase because
                // if the binding is of the form (Control, ControlProperty, DataSource, Property1.Property2.Property3)
                // then we want to get notification from Current.Property1.Property2 and not from DataSource
                // when we get the backEnd notification we push the new value into the Control's property
                //
                if (fieldInfo != null &&
                    owner.BindingManagerBase.IsBinding &&
                    !(this.owner.BindingManagerBase is CurrencyManager)) {

                    fieldInfo.AddValueChanged(this.owner.BindingManagerBase.Current, new EventHandler(PropValueChanged));
                }
            }
            else {
                fieldInfo = null;
            }
        }
    }
}
