// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Windows.Forms.Layout;

namespace System.Windows.Forms;

[TypeConverter(typeof(TableLayoutSettings.StyleConverter))]
[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)]
public abstract class TableLayoutStyle
{
    private IArrangedElement? _owner;
    private SizeType _sizeType = SizeType.AutoSize;
    private float _size;

    [DefaultValue(SizeType.AutoSize)]
    public SizeType SizeType
    {
        get { return _sizeType; }
        set
        {
            if (_sizeType != value)
            {
                _sizeType = value;
                if (Owner is not null)
                {
                    LayoutTransaction.DoLayout(Owner, Owner, PropertyNames.Style);
                    if (Owner is Control owner)
                    {
                        owner.Invalidate();
                    }
                }
            }
        }
    }

    internal float Size
    {
        get { return _size; }
        set
        {
            ArgumentOutOfRangeException.ThrowIfNegative(value);

            if (_size != value)
            {
                _size = value;
                if (Owner is not null)
                {
                    LayoutTransaction.DoLayout(Owner, Owner, PropertyNames.Style);
                    if (Owner is Control owner)
                    {
                        owner.Invalidate();
                    }
                }
            }
        }
    }

    private bool ShouldSerializeSize()
    {
        return SizeType != SizeType.AutoSize;
    }

    internal IArrangedElement? Owner
    {
        get { return _owner; }
        set { _owner = value; }
    }

    // set the size without doing a layout
    internal void SetSize(float size)
    {
        Debug.Assert(size >= 0);
        _size = size;
    }

    // Workaround for https://github.com/dotnet/runtime/issues/100786
    [return: DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)]
    internal Type GetTypeWithConstructor() => this.GetType();
}
