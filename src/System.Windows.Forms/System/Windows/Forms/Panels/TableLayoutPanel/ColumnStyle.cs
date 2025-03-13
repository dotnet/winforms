// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms;

public class ColumnStyle : TableLayoutStyle
{
    public ColumnStyle()
    {
    }

    public ColumnStyle(SizeType sizeType)
    {
        SizeType = sizeType;
    }

    public ColumnStyle(SizeType sizeType, float width)
    {
        SizeType = sizeType;
        Width = width;
    }

    public float Width
    {
        get => Size;
        set => Size = value;
    }
}
