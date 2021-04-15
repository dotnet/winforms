// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.ComponentModel.Design
{
    internal sealed partial class DesignerActionPanel
    {
        internal sealed class TypeDescriptorContext : ITypeDescriptorContext
        {
            private readonly IServiceProvider _serviceProvider;
            private readonly PropertyDescriptor _propDesc;
            private readonly object _instance;

            public TypeDescriptorContext(IServiceProvider serviceProvider, PropertyDescriptor propDesc, object instance)
            {
                _serviceProvider = serviceProvider;
                _propDesc = propDesc;
                _instance = instance;
            }

            private IComponentChangeService ComponentChangeService
            {
                get => (IComponentChangeService)_serviceProvider.GetService(typeof(IComponentChangeService));
            }

            public IContainer Container
            {
                get => (IContainer)_serviceProvider.GetService(typeof(IContainer));
            }

            public object Instance
            {
                get => _instance;
            }

            public PropertyDescriptor PropertyDescriptor
            {
                get => _propDesc;
            }

            public object GetService(Type serviceType) => _serviceProvider.GetService(serviceType);

            public bool OnComponentChanging()
            {
                if (ComponentChangeService != null)
                {
                    try
                    {
                        ComponentChangeService.OnComponentChanging(_instance, _propDesc);
                    }
                    catch (CheckoutException ce)
                    {
                        if (ce == CheckoutException.Canceled)
                        {
                            return false;
                        }

                        throw;
                    }
                }

                return true;
            }

            public void OnComponentChanged()
            {
                if (ComponentChangeService != null)
                {
                    ComponentChangeService.OnComponentChanged(_instance, _propDesc, null, null);
                }
            }
        }
    }
}
