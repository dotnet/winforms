// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

namespace System.Windows.Forms
{
    public partial class PropertyGrid
    {
        private class PropertyGridServiceProvider : IServiceProvider
        {
            private readonly PropertyGrid _owner;

            public PropertyGridServiceProvider(PropertyGrid owner)
            {
                _owner = owner;
            }

            public object GetService(Type serviceType)
            {
                object s = null;

                if (_owner.ActiveDesigner != null)
                {
                    s = _owner.ActiveDesigner.GetService(serviceType);
                }

                if (s is null)
                {
                    s = _owner._gridView.GetService(serviceType);
                }

                if (s is null && _owner.Site != null)
                {
                    s = _owner.Site.GetService(serviceType);
                }
                return s;
            }
        }
    }
}
