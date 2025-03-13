// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;

namespace System.Windows.Forms;

public partial class Binding
{
    private class BindToObject
    {
        private BindingManagerBase? _bindingManager;
        private readonly Binding _owner;
        private bool _dataSourceInitialized;
        private bool _waitingOnDataSource;

        private void PropValueChanged(object? sender, EventArgs e)
        {
            _bindingManager?.OnCurrentChanged(EventArgs.Empty);
        }

        private bool IsDataSourceInitialized
        {
            get
            {
                Debug.Assert(_owner.DataSource is not null, "how can we determine if DataSource is initialized or not if we have no data source?");
                if (_dataSourceInitialized)
                {
                    return true;
                }

                if (_owner.DataSource is not ISupportInitializeNotification ds || ds.IsInitialized)
                {
                    _dataSourceInitialized = true;
                    return true;
                }

                // We have an ISupportInitializeNotification which was not initialized yet.
                // We already hooked up the Initialized event and the data source is not initialized yet.
                if (_waitingOnDataSource)
                {
                    return false;
                }

                // Hook up the Initialized event.
                ds.Initialized += DataSource_Initialized;
                _waitingOnDataSource = true;
                return false;
            }
        }

        internal BindToObject(Binding owner)
        {
            Debug.Assert(owner is not null);
            _owner = owner;
            CheckBinding();
        }

        private void DataSource_Initialized(object? sender, EventArgs e)
        {
            Debug.Assert(sender == _owner.DataSource, "data source should not change");
            Debug.Assert(_owner.DataSource is ISupportInitializeNotification, "data source should not change on the BindToObject");
            Debug.Assert(_waitingOnDataSource);

            // Unhook the Initialized event.
            if (_owner.DataSource is ISupportInitializeNotification ds)
            {
                ds.Initialized -= DataSource_Initialized;
            }

            // The wait is over: DataSource is initialized.
            _waitingOnDataSource = false;
            _dataSourceInitialized = true;

            // Rebind.
            CheckBinding();
        }

        internal void SetBindingManagerBase(BindingManagerBase? lManager)
        {
            if (_bindingManager == lManager)
            {
                return;
            }

            // remove notification from the backEnd
            if (_bindingManager is not null
                && FieldInfo is not null
                && _bindingManager.IsBinding
                && _bindingManager is not CurrencyManager)
            {
                FieldInfo.RemoveValueChanged(_bindingManager.Current!, PropValueChanged);
                FieldInfo = null;
            }

            _bindingManager = lManager;
            CheckBinding();
        }

        internal string DataErrorText { get; private set; } = string.Empty;

        /// <summary>
        ///  Returns any data error info on the data source for the bound data field
        ///  in the current row
        /// </summary>
        private string GetErrorText(object? value)
        {
            if (value is not IDataErrorInfo errorInfo)
            {
                return string.Empty;
            }

            // Get the row error if there is no DataMember
            if (FieldInfo is null)
            {
                return errorInfo.Error ?? string.Empty;
            }

            // Get the column error if there is a DataMember.
            // The DataTable uses its own Locale to lookup column names <sigh>.
            // So passing the DataMember from the BindingField could cause problems.
            // Pass the name from the PropertyDescriptor that the DataTable gave us.
            // (If there is no fieldInfo, data binding would have failed already )
            return errorInfo[FieldInfo.Name] ?? string.Empty;
        }

        internal object? GetValue()
        {
            object? obj = _bindingManager?.Current;

            // Update IDataErrorInfo text: it's ok to get this now because we're going to need
            // this as part of the BindingCompleteEventArgs anyway.
            DataErrorText = GetErrorText(obj);

            if (FieldInfo is not null)
            {
                obj = FieldInfo.GetValue(obj);
            }

            return obj;
        }

        internal Type? BindToType
        {
            get
            {
                if (_owner.BindingMemberInfo.BindingField.Length == 0)
                {
                    // if we are bound to a list w/o any properties, then
                    // take the type from the BindingManager
                    Type? type = _bindingManager?.BindType;
                    if (typeof(Array).IsAssignableFrom(type))
                    {
                        type = type.GetElementType();
                    }

                    return type;
                }

                return FieldInfo?.PropertyType;
            }
        }

        internal void SetValue(object? value)
        {
            object? obj = null;

            if (FieldInfo is not null)
            {
                obj = _bindingManager?.Current;
                if (obj is IEditableObject editableObject)
                {
                    editableObject.BeginEdit();
                }

                if (!FieldInfo.IsReadOnly)
                {
                    FieldInfo.SetValue(obj, value);
                }
            }
            else
            {
                if (_bindingManager is CurrencyManager cm)
                {
                    cm[cm.Position] = value;
                    obj = value;
                }
            }

            // Update IDataErrorInfo text.
            DataErrorText = GetErrorText(obj);
        }

        internal PropertyDescriptor? FieldInfo { get; private set; }

        internal void CheckBinding()
        {
            // At design time, don't check anything.
            if (_owner.BindableComponent is not null && _owner.ControlAtDesignTime())
            {
                return;
            }

            // Remove propertyChangedNotification when this binding is deleted
            if (_bindingManager is not null
                && FieldInfo is not null
                && _bindingManager.IsBinding
                && _bindingManager is not CurrencyManager)
            {
                FieldInfo.RemoveValueChanged(_bindingManager.Current!, PropValueChanged);
            }

            if (_bindingManager is not null
                && _owner.BindableComponent is not null
                && _owner.ComponentCreated
                && IsDataSourceInitialized)
            {
                string dataField = _owner.BindingMemberInfo.BindingField;

                FieldInfo = _bindingManager.GetItemProperties().Find(dataField, true);
                if (_bindingManager.DataSource is not null && FieldInfo is null && dataField.Length > 0)
                {
                    throw new ArgumentException(string.Format(SR.ListBindingBindField, dataField), "dataMember");
                }

                // Do not add propertyChange notification if the fieldInfo is null
                //
                // We add an event handler to the dataSource in the BindingManagerBase because
                // if the binding is of the form (Control, ControlProperty, DataSource, Property1.Property2.Property3)
                // then we want to get notification from Current.Property1.Property2 and not from DataSource
                // when we get the backEnd notification we push the new value into the Control's property
                if (FieldInfo is not null
                    && _bindingManager.IsBinding
                    && _bindingManager is not CurrencyManager)
                {
                    FieldInfo.AddValueChanged(_bindingManager.Current!, PropValueChanged);
                }
            }
            else
            {
                FieldInfo = null;
            }
        }
    }
}
