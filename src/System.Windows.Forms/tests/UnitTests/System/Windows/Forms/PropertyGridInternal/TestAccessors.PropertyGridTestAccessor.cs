// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System;

public static partial class TestAccessors
{
    public class PropertyGridTestAccessor : TestAccessor<PropertyGrid>
    {
        public PropertyGridTestAccessor(PropertyGrid instance) : base(instance) { }

        internal Windows.Forms.PropertyGridInternal.PropertyGridView GridView => Dynamic._gridView;

        internal void SaveSelectedTabIndex() { Dynamic.SaveSelectedTabIndex(); }

        internal bool TryGetSavedTabIndex(out int selectedTabIndex) { return Dynamic.TryGetSavedTabIndex(out selectedTabIndex); }

        internal Dictionary<int, int> _designerSelections => Dynamic._designerSelections;
    }

    public static PropertyGridTestAccessor TestAccessor(this PropertyGrid propertyGrid)
        => new(propertyGrid);
}
