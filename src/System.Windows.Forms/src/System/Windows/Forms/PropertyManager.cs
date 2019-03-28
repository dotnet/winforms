// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics.CodeAnalysis;
using System.ComponentModel;
using System.Collections;

namespace System.Windows.Forms
{
    public class PropertyManager : BindingManagerBase
    {
        private object _dataSource;
        private string _propName;
        private PropertyDescriptor _propInfo;
        private bool _bound;

        public override object Current => _dataSource;

        private void PropertyChanged(object sender, EventArgs ea)
        {
            EndCurrentEdit();
            OnCurrentChanged(EventArgs.Empty);
        }

        internal override void SetDataSource(object dataSource)
        {
            if (_dataSource != null && !string.IsNullOrEmpty(_propName))
            {
                _propInfo.RemoveValueChanged(_dataSource, new EventHandler(PropertyChanged));
                _propInfo = null;
            }

            _dataSource = dataSource;

            if (_dataSource != null && !string.IsNullOrEmpty(_propName))
            {
                _propInfo = TypeDescriptor.GetProperties(dataSource).Find(_propName, true);
                if (_propInfo == null)
                {
                    throw new ArgumentException(string.Format(SR.PropertyManagerPropDoesNotExist, _propName, dataSource.ToString()));
                }

                _propInfo.AddValueChanged(dataSource, new EventHandler(PropertyChanged));
            }
        }

        public PropertyManager()
        {
        }

        [SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors", Justification = "If the constructor does not set the dataSource it would be a breaking change.")]
        internal PropertyManager(object dataSource) : base(dataSource)
        {
        }

        [SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors", Justification = "If the constructor does not set the dataSource it would be a breaking change.")]
        internal PropertyManager(object dataSource, string propName) : base()
        {
            this._propName = propName;
            this.SetDataSource(dataSource);
        }

        internal override PropertyDescriptorCollection GetItemProperties(PropertyDescriptor[] listAccessors)
        {
            return ListBindingHelper.GetListItemProperties(_dataSource, listAccessors);
        }

        internal override Type BindType => _dataSource.GetType();

        internal override string GetListName()
        {
            return TypeDescriptor.GetClassName(_dataSource) + "." + _propName;
        }

        public override void SuspendBinding()
        {
            EndCurrentEdit();
            if (_bound)
            {
                try
                {
                    _bound = false;
                    UpdateIsBinding();
                }
                catch
                {
                    _bound = true;
                    UpdateIsBinding();
                    throw;
                }
            }
        }

        public override void ResumeBinding()
        {
            OnCurrentChanged(EventArgs.Empty);
            if (!_bound)
            {
                try
                {
                    _bound = true;
                    UpdateIsBinding();
                }
                catch
                {
                    _bound = false;
                    UpdateIsBinding();
                    throw;
                }
            }
        }

        protected internal override string GetListName(ArrayList listAccessors) => string.Empty;

        public override void CancelCurrentEdit()
        {
            IEditableObject obj = Current as IEditableObject;
            obj?.CancelEdit();
            PushData();
        }

        public override void EndCurrentEdit()
        {
            bool success;
            PullData(out success);

            if (success)
            {
                IEditableObject obj = Current as IEditableObject;
                obj?.EndEdit();
            }
        }

        protected override void UpdateIsBinding()
        {
            for (int i = 0; i < Bindings.Count; i++)
            {
                Bindings[i].UpdateIsBinding();
            }
        }

        internal protected override void OnCurrentChanged(EventArgs ea)
        {
            PushData();

            onCurrentChangedHandler?.Invoke(this, ea);
            onCurrentItemChangedHandler?.Invoke(this, ea);
        }

        internal protected override void OnCurrentItemChanged(EventArgs ea)
        {
            PushData();

            onCurrentItemChangedHandler?.Invoke(this, ea);
        }

        internal override object DataSource => _dataSource;

        internal override bool IsBinding => _dataSource != null;

        /// <remarks>
        /// no op on the propertyManager
        /// </remarks>
        public override int Position
        {
            get => 0;
            set
            {
            }
        }

        public override int Count => 1;

        public override void AddNew()
        {
            throw new NotSupportedException(SR.DataBindingAddNewNotSupportedOnPropertyManager);
        }

        public override void RemoveAt(int index)
        {
            throw new NotSupportedException(SR.DataBindingRemoveAtNotSupportedOnPropertyManager);
        }
    }
}
