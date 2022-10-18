// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Globalization;
using System.Reflection;
using System.Windows.Forms;

namespace System.ComponentModel.Design
{
    internal sealed partial class DesignerActionPanel
    {
        private abstract class PropertyLine : Line
        {
            private DesignerActionList _actionList;
            private DesignerActionPropertyItem _propertyItem;
            private object _value;
            private bool _pushingValue;
            private PropertyDescriptor _propDesc;
            private ITypeDescriptorContext _typeDescriptorContext;

            public PropertyLine(IServiceProvider serviceProvider, DesignerActionPanel actionPanel) : base(serviceProvider, actionPanel)
            {
            }

            public sealed override string FocusId
            {
                get => "PROPERTY:" + _actionList.GetType().FullName + "." + _propertyItem.MemberName;
            }

            protected PropertyDescriptor PropertyDescriptor
            {
                get
                {
                    _propDesc ??= TypeDescriptor.GetProperties(_actionList)[_propertyItem.MemberName];

                    return _propDesc;
                }
            }

            protected DesignerActionPropertyItem PropertyItem
            {
                get => _propertyItem;
            }

            protected ITypeDescriptorContext TypeDescriptorContext
            {
                get
                {
                    _typeDescriptorContext ??= new TypeDescriptorContext(ServiceProvider, PropertyDescriptor, _actionList);

                    return _typeDescriptorContext;
                }
            }

            protected object Value
            {
                get => _value;
            }

            protected abstract void OnPropertyTaskItemUpdated(ToolTip toolTip, ref int currentTabIndex);

            protected abstract void OnValueChanged();

            protected void SetValue(object newValue)
            {
                if (_pushingValue || ActionPanel.DropDownActive)
                {
                    return;
                }

                _pushingValue = true;
                try
                {
                    // Only push the change if the values are different
                    if (newValue is not null)
                    {
                        Type valueType = newValue.GetType();
                        // If it's not assignable, try to convert it
                        if (!PropertyDescriptor.PropertyType.IsAssignableFrom(valueType))
                        {
                            if (PropertyDescriptor.Converter is not null)
                            {
                                // If we can't convert it, show an error
                                if (!PropertyDescriptor.Converter.CanConvertFrom(_typeDescriptorContext, valueType))
                                {
                                    ActionPanel.ShowError(string.Format(SR.DesignerActionPanel_CouldNotConvertValue, newValue, _propDesc.PropertyType));
                                    return;
                                }
                                else
                                {
                                    newValue = PropertyDescriptor.Converter.ConvertFrom(_typeDescriptorContext, CultureInfo.CurrentCulture, newValue);
                                }
                            }
                        }
                    }

                    if (!Equals(_value, newValue))
                    {
                        PropertyDescriptor.SetValue(_actionList, newValue);
                        // Update the value we're caching
                        _value = PropertyDescriptor.GetValue(_actionList);
                        OnValueChanged();
                    }
                }
                catch (Exception e)
                {
                    if (e is TargetInvocationException)
                    {
                        e = e.InnerException;
                    }

                    ActionPanel.ShowError(string.Format(SR.DesignerActionPanel_ErrorSettingValue, newValue, PropertyDescriptor.Name, e.Message));
                }
                finally
                {
                    _pushingValue = false;
                }
            }

            internal sealed override void UpdateActionItem(DesignerActionList actionList, DesignerActionItem actionItem, ToolTip toolTip, ref int currentTabIndex)
            {
                _actionList = actionList;
                _propertyItem = (DesignerActionPropertyItem)actionItem;
                _propDesc = null;
                _typeDescriptorContext = null;
                _value = PropertyDescriptor.GetValue(actionList);
                OnPropertyTaskItemUpdated(toolTip, ref currentTabIndex);
                _pushingValue = true;
                try
                {
                    OnValueChanged();
                }
                finally
                {
                    _pushingValue = false;
                }
            }
        }
    }
}
