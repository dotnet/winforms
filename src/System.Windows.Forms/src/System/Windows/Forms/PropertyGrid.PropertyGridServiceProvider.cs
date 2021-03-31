// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    public partial class PropertyGrid
    {
        private class PropertyGridServiceProvider : IServiceProvider
        {
            private readonly PropertyGrid _ownerPropertyGrid;

            public PropertyGridServiceProvider(PropertyGrid ownerPropertyGrid)
            {
                _ownerPropertyGrid = ownerPropertyGrid;
            }

            public object? GetService(Type serviceType)
            {
                object? s = null;

                if (_ownerPropertyGrid.ActiveDesigner is not null)
                {
                    s = _ownerPropertyGrid.ActiveDesigner.GetService(serviceType);
                }

                if (s is null)
                {
                    s = _ownerPropertyGrid._gridView.GetService(serviceType);
                }

                if (s is null && _ownerPropertyGrid.Site is not null)
                {
                    s = _ownerPropertyGrid.Site.GetService(serviceType);
                }

                return s;
            }
        }
    }
}
