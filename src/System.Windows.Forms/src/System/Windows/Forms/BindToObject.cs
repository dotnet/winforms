// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.Diagnostics;

namespace System.Windows.Forms
{
    internal class BindToObject
    {
        private PropertyDescriptor _fieldInfo;
        private BindingMemberInfo _dataMember;
        private object _dataSource;
        private BindingManagerBase _bindingManager;
        private Binding _owner;
        private string _errorText = string.Empty;

        private bool _dataSourceInitialized = false;
        private bool _waitingOnDataSource = false;

        private void PropValueChanged(object sender, EventArgs e)
        {
            _bindingManager?.OnCurrentChanged(EventArgs.Empty);
        }

        private bool IsDataSourceInitialized
        {
            get
            {
                Debug.Assert(_dataSource != null, "how can we determine if DataSource is initialized or not if we have no data source?");
                if (_dataSourceInitialized)
                {
                    return true;
                }

                if (!(_dataSource is ISupportInitializeNotification ds) || ds.IsInitialized)
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
                ds.Initialized += new EventHandler(DataSource_Initialized);
                _waitingOnDataSource = true;
                return false;
            }
        }

        internal BindToObject(Binding owner, object dataSource, string dataMember)
        {
            this._owner = owner;
            this._dataSource = dataSource;
            this._dataMember = new BindingMemberInfo(dataMember);
            CheckBinding();
        }

        private void DataSource_Initialized(object sender, EventArgs e)
        {
            Debug.Assert(sender == _dataSource, "data source should not change");
            Debug.Assert(_dataSource is ISupportInitializeNotification, "data source should not change on the BindToObject");
            Debug.Assert(_waitingOnDataSource);

            // Unhook the Initialized event.
            if (_dataSource is ISupportInitializeNotification ds)
            {
                ds.Initialized -= new EventHandler(DataSource_Initialized);
            }

            // The wait is over: DataSource is initialized.
            _waitingOnDataSource = false;
            _dataSourceInitialized = true;

            // Rebind.
            CheckBinding();
        }

        internal void SetBindingManagerBase(BindingManagerBase lManager)
        {
            if (_bindingManager == lManager)
            {
                return;
            }

            // remove notification from the backEnd
            if (_bindingManager != null && _fieldInfo != null && _bindingManager.IsBinding && !(_bindingManager is CurrencyManager))
            {
                _fieldInfo.RemoveValueChanged(_bindingManager.Current, new EventHandler(PropValueChanged));
                _fieldInfo = null;
            }

            _bindingManager = lManager;
            CheckBinding();
        }

        internal string DataErrorText => _errorText;

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
                if (_fieldInfo == null)
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
                    text = errorInfo[_fieldInfo.Name];
                }
            }

            return text ?? string.Empty;
        }

        internal object GetValue()
        {
            object obj = _bindingManager.Current;

            // Update IDataErrorInfo text: it's ok to get this now because we're going to need
            // this as part of the BindingCompleteEventArgs anyway.
            _errorText = GetErrorText(obj);

            if (_fieldInfo != null)
            {
                obj = _fieldInfo.GetValue(obj);
            }

            return obj;
        }

        internal Type BindToType
        {
            get
            {
                if (_dataMember.BindingField.Length == 0)
                {
                    // if we are bound to a list w/o any properties, then
                    // take the type from the BindingManager
                    Type type = _bindingManager.BindType;
                    if (typeof(Array).IsAssignableFrom(type))
                    {
                        type = type.GetElementType();
                    }
                    return type;
                }

                return _fieldInfo?.PropertyType;
            }
        }

        internal void SetValue(object value)
        {
            object obj = null;

            if (_fieldInfo != null)
            {
                obj = _bindingManager.Current;
                if (obj is IEditableObject editableObject)
                {
                    editableObject.BeginEdit();
                }
                if (!_fieldInfo.IsReadOnly)
                {
                    _fieldInfo.SetValue(obj, value);
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
            _errorText = GetErrorText(obj);
        }

        internal BindingMemberInfo BindingMemberInfo => _dataMember;

        internal object DataSource => _dataSource;

        internal PropertyDescriptor FieldInfo => _fieldInfo;

        internal BindingManagerBase BindingManagerBase => _bindingManager;

        internal void CheckBinding()
        {
            // At design time, don't check anything.
            if (_owner != null && _owner.BindableComponent != null && _owner.ControlAtDesignTime())
            {
                return;
            }

            // Remove propertyChangedNotification when this binding is deleted
            if (_owner.BindingManagerBase != null &&
                _fieldInfo != null &&
                _owner.BindingManagerBase.IsBinding &&
                !(_owner.BindingManagerBase is CurrencyManager))
            {

                _fieldInfo.RemoveValueChanged(_owner.BindingManagerBase.Current, new EventHandler(PropValueChanged));
            }

            if (_owner != null &&
                _owner.BindingManagerBase != null &&
                _owner.BindableComponent != null &&
                _owner.ComponentCreated &&
                IsDataSourceInitialized)
            {

                string dataField = _dataMember.BindingField;

                _fieldInfo = _owner.BindingManagerBase.GetItemProperties().Find(dataField, true);
                if (_owner.BindingManagerBase.DataSource != null && _fieldInfo == null && dataField.Length > 0)
                {
                    throw new ArgumentException(string.Format(SR.ListBindingBindField, dataField), "dataMember");
                }

                // Do not add propertyChange notification if the fieldInfo is null
                //
                // We add an event handler to the dataSource in the BindingManagerBase because
                // if the binding is of the form (Control, ControlProperty, DataSource, Property1.Property2.Property3)
                // then we want to get notification from Current.Property1.Property2 and not from DataSource
                // when we get the backEnd notification we push the new value into the Control's property
                if (_fieldInfo != null && _owner.BindingManagerBase.IsBinding &&
                    !(_owner.BindingManagerBase is CurrencyManager))
                {

                    _fieldInfo.AddValueChanged(_owner.BindingManagerBase.Current, new EventHandler(PropValueChanged));
                }
            }
            else
            {
                _fieldInfo = null;
            }
        }
    }
}
