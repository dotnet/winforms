// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Windows.Forms.PropertyGridInternal;

namespace System.Windows.Forms;

public partial class PropertyGrid
{
    /// <summary>
    ///  Service provider that searches the <see cref="ActiveDesigner"/>, then the <see cref="PropertyGridView"/>,
    ///  then finally the <see cref="Site"/> for requested services.
    /// </summary>
    private class PropertyGridServiceProvider : IServiceProvider
    {
        private readonly PropertyGrid _ownerPropertyGrid;

        public PropertyGridServiceProvider(PropertyGrid ownerPropertyGrid)
        {
            _ownerPropertyGrid = ownerPropertyGrid;
        }

        public object? GetService(Type serviceType)
        {
            object? service = _ownerPropertyGrid.ActiveDesigner?.GetService(serviceType);
            service ??= _ownerPropertyGrid._gridView.GetService(serviceType);
            service ??= _ownerPropertyGrid.Site?.GetService(serviceType);
            return service;
        }
    }
}
