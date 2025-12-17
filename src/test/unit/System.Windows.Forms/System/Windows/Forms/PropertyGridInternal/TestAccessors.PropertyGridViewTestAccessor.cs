// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable disable

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

    extension(Windows.Forms.PropertyGridInternal.PropertyGridView propertyGridView)
    {
        internal PropertyGridViewTestAccessor TestAccessor => new(propertyGridView);
    }
}
