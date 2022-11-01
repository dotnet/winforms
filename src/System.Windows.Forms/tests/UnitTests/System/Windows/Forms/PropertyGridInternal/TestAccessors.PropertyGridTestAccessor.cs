// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System
{
    public static partial class TestAccessors
    {
        public class PropertyGridTestAccessor : TestAccessor<Windows.Forms.PropertyGrid>
        {
            public PropertyGridTestAccessor(Windows.Forms.PropertyGrid instance) : base(instance) { }

            internal Windows.Forms.PropertyGridInternal.PropertyGridView GridView => Dynamic._gridView;

            internal void SaveSelectedTabIndex() { Dynamic.SaveSelectedTabIndex(); }

            internal bool TryGetSavedTabIndex(out int selectedTabIndex) { return Dynamic.TryGetSavedTabIndex(out selectedTabIndex); }

            internal Dictionary<int, int> _designerSelections => Dynamic._designerSelections;
        }

        public static PropertyGridTestAccessor TestAccessor(this Windows.Forms.PropertyGrid propertyGrid)
            => new PropertyGridTestAccessor(propertyGrid);
    }
}
