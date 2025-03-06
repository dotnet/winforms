// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms.PropertyGridInternal;

internal class ArrayElementGridEntry : GridEntry
{
    protected int _index;

    public ArrayElementGridEntry(PropertyGrid ownerGrid, GridEntry parent, int index)
        : base(ownerGrid, parent)
    {
        _index = index;
        SetFlag(Flags.RenderReadOnly, parent.EntryFlags.HasFlag(Flags.RenderReadOnly) || parent.ForceReadOnly);
    }

    public override GridItemType GridItemType => GridItemType.ArrayValue;

    public override bool IsValueEditable => ParentGridEntry?.IsValueEditable ?? false;

    public override string PropertyLabel => $"[{_index}]";

    public override Type? PropertyType => ParentGridEntry?.PropertyType?.GetElementType();

    public override object? PropertyValue
    {
        get
        {
            object? owner = GetValueOwner();
            Debug.Assert(owner is Array, "Owner is not array type!");
            return ((Array)owner).GetValue(_index);
        }
        set
        {
            object? owner = GetValueOwner();
            Debug.Assert(owner is Array, "Owner is not array type!");
            ((Array)owner).SetValue(value, _index);
        }
    }

    public override bool ShouldRenderReadOnly => ParentGridEntry?.ShouldRenderReadOnly ?? false;
}
