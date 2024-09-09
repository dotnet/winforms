// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.ComponentModel.Design;

public partial class CollectionEditor
{
    internal class PropertyGridSite : ISite
    {
        private readonly IServiceProvider? _serviceProvider;
        private bool _inGetService;

        public PropertyGridSite(IServiceProvider? serviceProvider, IComponent component)
        {
            _serviceProvider = serviceProvider;
            Component = component;
        }

        public IComponent Component { get; }

        public IContainer? Container => null;

        public bool DesignMode => false;

        public string? Name
        {
            get => null;
            set { }
        }

        public object? GetService(Type type)
        {
            if (!_inGetService && _serviceProvider is not null)
            {
                try
                {
                    _inGetService = true;
                    return _serviceProvider.GetService(type);
                }
                finally
                {
                    _inGetService = false;
                }
            }

            return null;
        }
    }
}
