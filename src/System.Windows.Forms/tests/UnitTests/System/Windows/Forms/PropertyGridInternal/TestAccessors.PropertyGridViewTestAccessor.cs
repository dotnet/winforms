// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System;

public static partial class TestAccessors
{
    internal class PropertyGridViewTestAccessor : TestAccessor<Windows.Forms.PropertyGridInternal.PropertyGridView>
    {
        public PropertyGridViewTestAccessor(Windows.Forms.PropertyGridInternal.PropertyGridView instance) : base(instance) { }

        internal Windows.Forms.PropertyGridInternal.PropertyDescriptorGridEntry SelectedGridEntry
        {
            get => Dynamic.selectedGridEntry;
            set => Dynamic.selectedGridEntry = value;
        }
    }

    internal static PropertyGridViewTestAccessor TestAccessor(this Windows.Forms.PropertyGridInternal.PropertyGridView propertyGridView)
        => new(propertyGridView);
}
