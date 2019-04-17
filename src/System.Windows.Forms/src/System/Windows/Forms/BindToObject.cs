// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.Diagnostics;

namespace System.Windows.Forms
{
    internal class BindToObject
    {
        private PropertyDescriptor fieldInfo;
        private BindingMemberInfo dataMember;
        private object dataSource;
        private BindingManagerBase bindingManager;
        private Binding owner;
        private string errorText = string.Empty;

        private bool dataSourceInitialized = false;
        private bool waitingOnDataSource = false;

        private void PropValueChanged(object sender, EventArgs e)
        {
            bindingManager?.OnCurrentChanged(EventArgs.Empty);
        }

        private bool IsDataSourceInitialized
        {
            get
            {
                Debug.Assert(dataSource != null, "how can we determine if DataSource is initialized or not if we have no data source?");
                if (dataSourceInitialized)
                {
                    return true;
                }

                if (!(dataSource is ISupportInitializeNotification ds) || ds.IsInitialized)
                {
                    dataSourceInitialized = true;
                    return true;
                }

                // We have an ISupportInitializeNotification which was not initialized yet.
                // We already hooked up the Initialized event and the data source is not initialized yet.
                if (waitingOnDataSource)
                {
                    return false;
                }

                // Hook up the Initialized event.
                ds.Initialized += new EventHandler(DataSource_Initialized);
                waitingOnDataSource = true;
                return false;
            }
        }

        internal BindToObject(Binding owner, object dataSource, string dataMember)
        {
            this.owner = owner;
            this.dataSource = dataSource;
            this.dataMember = new BindingMemberInfo(dataMember);
            CheckBinding();
        }

        private void DataSource_Initialized(object sender, EventArgs e)
        {
            Debug.Assert(sender == dataSource, "data source should not change");
            Debug.Assert(dataSource is ISupportInitializeNotification, "data source should not change on the BindToObject");
            Debug.Assert(waitingOnDataSource);

            // Unhook the Initialized event.
            if (dataSource is ISupportInitializeNotification ds)
            {
                ds.Initialized -= new EventHandler(DataSource_Initialized);
            }

            // The wait is over: DataSource is initialized.
            waitingOnDataSource = false;
            dataSourceInitialized = true;

            // Rebind.
            CheckBinding();
        }

        internal void SetBindingManagerBase(BindingManagerBase lManager)
        {
            if (bindingManager == lManager)
            {
                return;
            }

            // remove notification from the backEnd
            if (bindingManager != null && fieldInfo != null && bindingManager.IsBinding && !(bindingManager is CurrencyManager))
            {
                fieldInfo.RemoveValueChanged(bindingManager.Current, new EventHandler(PropValueChanged));
                fieldInfo = null;
            }

            bindingManager = lManager;
            CheckBinding();
        }

        internal string DataErrorText => errorText;

        /// <summary>
        /// Returns any data error info on the data source for the bound data field
        /// in the current row
        /// </summary>
        private string GetErrorText(object value)
        {
            string text = string.Empty;

            if (value is IDataErrorInfo errorInfo)
            {
                // Get the row error if there is no DataMember
                if (fieldInfo == null)
                {
                    text = errorInfo.Error;
                }
                // Get the column error if there is a DataMember.
                // The DataTable uses its own Locale to lookup column names <sigh>.
                // So passing the DataMember from the BindingField could cause problems.
                // Pass the name from the PropertyDescriptor that the DataTable gave us.
                // (If there is no fieldInfo, data binding would have failed already )
                else
                {
                    text = errorInfo[fieldInfo.Name];
                }
            }

            return text ?? string.Empty;
        }

        internal object GetValue()
        {
            object obj = bindingManager.Current;

            // Update IDataErrorInfo text: it's ok to get this now because we're going to need
            // this as part of the BindingCompleteEventArgs anyway.
            errorText = GetErrorText(obj);

            if (fieldInfo != null)
            {
                obj = fieldInfo.GetValue(obj);
            }

            return obj;
        }

        internal Type BindToType
        {
            get
            {
                if (dataMember.BindingField.Length == 0)
                {
                    // if we are bound to a list w/o any properties, then
                    // take the type from the BindingManager
                    Type type = bindingManager.BindType;
                    if (typeof(Array).IsAssignableFrom(type))
                    {
                        type = type.GetElementType();
                    }
                    return type;
                }
                
                return fieldInfo?.PropertyType;
            }
        }

        internal void SetValue(object value)
        {
            object obj = null;

            if (fieldInfo != null)
            {
                obj = bindingManager.Current;
                if (obj is IEditableObject editableObject)
                {
                    editableObject.BeginEdit();
                }
                if (!fieldInfo.IsReadOnly)
                {
                    fieldInfo.SetValue(obj, value);
                }
            }
            else
            {
                if (bindingManager is CurrencyManager cm)
                {
                    cm[cm.Position] = value;
                    obj = value;
                }
            }

            // Update IDataErrorInfo text. 
            errorText = GetErrorText(obj);
        }

        internal BindingMemberInfo BindingMemberInfo => dataMember;

        internal object DataSource => dataSource;

        internal PropertyDescriptor FieldInfo => fieldInfo;

        internal BindingManagerBase BindingManagerBase => bindingManager;

        internal void CheckBinding()
        {
            // At design time, don't check anything.
            if (owner != null && owner.BindableComponent != null && owner.ControlAtDesignTime())
            {
                return;
            }

            // Remove propertyChangedNotification when this binding is deleted
            if (owner.BindingManagerBase != null &&
                fieldInfo != null &&
                owner.BindingManagerBase.IsBinding &&
                !(owner.BindingManagerBase is CurrencyManager))
            {

                fieldInfo.RemoveValueChanged(owner.BindingManagerBase.Current, new EventHandler(PropValueChanged));
            }

            if (owner != null &&
                owner.BindingManagerBase != null &&
                owner.BindableComponent != null &&
                owner.ComponentCreated &&
                IsDataSourceInitialized)
            {

                string dataField = dataMember.BindingField;

                fieldInfo = owner.BindingManagerBase.GetItemProperties().Find(dataField, true);
                if (owner.BindingManagerBase.DataSource != null && fieldInfo == null && dataField.Length > 0)
                {
                    throw new ArgumentException(string.Format(SR.ListBindingBindField, dataField), "dataMember");
                }

                // Do not add propertyChange notification if the fieldInfo is null                
                //
                // We add an event handler to the dataSource in the BindingManagerBase because
                // if the binding is of the form (Control, ControlProperty, DataSource, Property1.Property2.Property3)
                // then we want to get notification from Current.Property1.Property2 and not from DataSource
                // when we get the backEnd notification we push the new value into the Control's property
                if (fieldInfo != null && owner.BindingManagerBase.IsBinding &&
                    !(owner.BindingManagerBase is CurrencyManager))
                {

                    fieldInfo.AddValueChanged(owner.BindingManagerBase.Current, new EventHandler(PropValueChanged));
                }
            }
            else
            {
                fieldInfo = null;
            }
        }
    }
}
