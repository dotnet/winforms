// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.Collections;

namespace System.Windows.Forms
{
    public class PropertyManager : BindingManagerBase
    {
        private object _dataSource;
        private readonly string _propName;
        private PropertyDescriptor _propInfo;
        private bool _bound;

        /// <summary>
        ///  An object that represents the object to which the property belongs.
        /// </summary>
        public override object Current => _dataSource;

        private void PropertyChanged(object sender, EventArgs ea)
        {
            EndCurrentEdit();
            OnCurrentChanged(EventArgs.Empty);
        }

        private protected override void SetDataSource(object dataSource)
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

        internal PropertyManager(object dataSource) : base(dataSource)
        {
        }

        internal PropertyManager(object dataSource, string propName) : base()
        {
            _propName = propName;
            SetDataSource(dataSource);
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

        /// <summary>
        ///  Resumes data binding.
        /// </summary>
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

        /// <summary>
        ///  Gets the name of the list supplying the data for the binding.
        /// </summary>
        /// <returns>Always returns an empty string.</returns>
        protected internal override string GetListName(ArrayList listAccessors) => string.Empty;

        /// <summary>
        ///  Cancels the current edit.
        /// </summary>
        public override void CancelCurrentEdit()
        {
            IEditableObject obj = Current as IEditableObject;
            obj?.CancelEdit();
            PushData();
        }

        /// <summary>
        ///  Ends the current edit.
        /// </summary>
        public override void EndCurrentEdit()
        {
            PullData(out bool success);

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

        /// <summary>
        ///  Raises the <see cref="BindingManagerBase.CurrentChanged" /> event.
        /// </summary>
        /// <param name="ea">The event data.</param>
        internal protected override void OnCurrentChanged(EventArgs ea)
        {
            PushData();

            onCurrentChangedHandler?.Invoke(this, ea);
            _onCurrentItemChangedHandler?.Invoke(this, ea);
        }

        /// <summary>
        ///  Raises the <see cref="BindingManagerBase.CurrentItemChanged" /> event.
        /// </summary>
        /// <param name="ea">The event data.</param>
        internal protected override void OnCurrentItemChanged(EventArgs ea)
        {
            PushData();

            _onCurrentItemChangedHandler?.Invoke(this, ea);
        }

        internal override object DataSource => _dataSource;

        internal override bool IsBinding => _dataSource != null;

        /// <summary>
        ///  Gets the position in the underlying list that controls bound to this data source point to.
        /// </summary>
        /// <value>Always returns 0.</value>
        public override int Position
        {
            get => 0;
            set
            {
            }
        }

        /// <summary>
        ///  Gets the number of rows managed by the <see cref="BindingManagerBase" />.
        /// </summary>
        /// <value>Always returns 1.</value>
        public override int Count => 1;

        /// <summary>
        /// Throws a <see cref="NotSupportedException" /> in all cases.
        /// </summary>
        /// <exception cref="NotSupportedException">In all cases.</exception>
        public override void AddNew()
        {
            throw new NotSupportedException(SR.DataBindingAddNewNotSupportedOnPropertyManager);
        }

        /// <summary>
        ///  Throws a <see cref="NotSupportedException" /> in all cases.
        /// </summary>
        /// <param name="index">The index of the row to delete.</param>
        /// <exception cref="NotSupportedException">In all cases.</exception>
        public override void RemoveAt(int index)
        {
            throw new NotSupportedException(SR.DataBindingRemoveAtNotSupportedOnPropertyManager);
        }
    }
}
