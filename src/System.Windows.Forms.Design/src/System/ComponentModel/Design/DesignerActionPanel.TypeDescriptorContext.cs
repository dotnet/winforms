// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

namespace System.ComponentModel.Design
{
    internal sealed partial class DesignerActionPanel
    {
        internal sealed class TypeDescriptorContext : ITypeDescriptorContext
        {
            private readonly IServiceProvider _serviceProvider;
            private readonly PropertyDescriptor _propertyDescriptor;
            private readonly object _instance;

            public TypeDescriptorContext(IServiceProvider serviceProvider, PropertyDescriptor propertyDescriptor, object instance)
            {
                _serviceProvider = serviceProvider;
                _propertyDescriptor = propertyDescriptor;
                _instance = instance;
            }

            private IComponentChangeService ComponentChangeService => _serviceProvider.GetService<IComponentChangeService>();

            public IContainer Container => _serviceProvider.GetService<IContainer>();

            public object Instance => _instance;

            public PropertyDescriptor PropertyDescriptor => _propertyDescriptor;

            public object GetService(Type serviceType) => _serviceProvider.GetService(serviceType);

            public bool OnComponentChanging()
            {
                try
                {
                    ComponentChangeService?.OnComponentChanging(_instance, _propertyDescriptor);
                    return true;
                }
                catch (CheckoutException ce) when (ce == CheckoutException.Canceled)
                {
                    return false;
                }
            }

            public void OnComponentChanged() => ComponentChangeService?.OnComponentChanged(_instance, _propertyDescriptor);
        }
    }
}
