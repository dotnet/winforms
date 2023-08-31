// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms;

public partial class PropertyGrid
{
    private readonly ref struct FreezePaintScope
    {
        private readonly PropertyGrid _propertyGrid;

        public FreezePaintScope(PropertyGrid propertyGrid)
        {
            _propertyGrid = propertyGrid;
            _propertyGrid.FreezePainting = true;
        }

        public void Dispose() => _propertyGrid.FreezePainting = false;
    }
}
