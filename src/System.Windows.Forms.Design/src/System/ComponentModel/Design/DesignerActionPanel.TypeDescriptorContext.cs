// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.ComponentModel.Design;

internal sealed partial class DesignerActionPanel
{
    internal sealed class TypeDescriptorContext : ITypeDescriptorContext
    {
        private readonly IServiceProvider _serviceProvider;

        public TypeDescriptorContext(IServiceProvider serviceProvider, PropertyDescriptor propertyDescriptor, object instance)
        {
            _serviceProvider = serviceProvider;
            PropertyDescriptor = propertyDescriptor;
            Instance = instance;
        }

        private IComponentChangeService? ComponentChangeService => _serviceProvider.GetService<IComponentChangeService>();

        public IContainer? Container => _serviceProvider.GetService<IContainer>();

        public object Instance { get; }

        public PropertyDescriptor PropertyDescriptor { get; }

        public object? GetService(Type serviceType) => _serviceProvider.GetService(serviceType);

        public bool OnComponentChanging()
        {
            try
            {
                ComponentChangeService?.OnComponentChanging(Instance, PropertyDescriptor);
                return true;
            }
            catch (CheckoutException ce) when (ce == CheckoutException.Canceled)
            {
                return false;
            }
        }

        public void OnComponentChanged() => ComponentChangeService?.OnComponentChanged(Instance, PropertyDescriptor);
    }
}
