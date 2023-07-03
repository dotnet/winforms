// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Globalization;
using System.Reflection;
using System.Windows.Forms;

namespace System.ComponentModel.Design;

internal sealed partial class DesignerActionPanel
{
    private abstract class PropertyLine : Line
    {
        private DesignerActionList? _actionList;
        private bool _pushingValue;
        private PropertyDescriptor? _propDesc;
        private ITypeDescriptorContext? _typeDescriptorContext;

        protected PropertyLine(IServiceProvider serviceProvider, DesignerActionPanel actionPanel) : base(serviceProvider, actionPanel)
        {
        }

        public sealed override string FocusId
        {
            get => $"PROPERTY:{_actionList!.GetType().FullName}.{PropertyItem!.MemberName}";
        }

        protected PropertyDescriptor PropertyDescriptor
        {
            get
            {
                return _propDesc ??= TypeDescriptor.GetProperties(_actionList!)[PropertyItem!.MemberName]!;
            }
        }

        protected DesignerActionPropertyItem? PropertyItem { get; private set; }

        protected ITypeDescriptorContext TypeDescriptorContext
        {
            get
            {
                return _typeDescriptorContext ??= new TypeDescriptorContext(ServiceProvider, PropertyDescriptor, _actionList!);
            }
        }

        protected object? Value { get; private set; }

        protected abstract void OnPropertyTaskItemUpdated(ToolTip toolTip, ref int currentTabIndex);

        protected abstract void OnValueChanged();

        protected void SetValue(object? newValue)
        {
            if (_pushingValue || ActionPanel._dropDownActive)
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
                                ActionPanel.ShowError(string.Format(SR.DesignerActionPanel_CouldNotConvertValue, newValue, _propDesc!.PropertyType));
                                return;
                            }
                            else
                            {
                                newValue = PropertyDescriptor.Converter.ConvertFrom(_typeDescriptorContext, CultureInfo.CurrentCulture, newValue);
                            }
                        }
                    }
                }

                if (!Equals(Value, newValue))
                {
                    PropertyDescriptor.SetValue(_actionList, newValue);
                    // Update the value we're caching
                    Value = PropertyDescriptor.GetValue(_actionList);
                    OnValueChanged();
                }
            }
            catch (Exception e)
            {
                if (e is TargetInvocationException)
                {
                    e = e.InnerException!;
                }

                ActionPanel.ShowError(string.Format(SR.DesignerActionPanel_ErrorSettingValue, newValue, PropertyDescriptor.Name, e.Message));
            }
            finally
            {
                _pushingValue = false;
            }
        }

        internal sealed override void UpdateActionItem(DesignerActionList? actionList, DesignerActionItem? actionItem, ToolTip toolTip, ref int currentTabIndex)
        {
            _actionList = actionList;
            PropertyItem = (DesignerActionPropertyItem?)actionItem;
            _propDesc = null;
            _typeDescriptorContext = null;
            Value = PropertyDescriptor.GetValue(actionList);
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
