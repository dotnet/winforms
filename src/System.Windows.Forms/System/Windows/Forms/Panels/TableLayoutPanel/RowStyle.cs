// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms;

public class RowStyle : TableLayoutStyle
{
    public RowStyle()
    {
    }

    public RowStyle(SizeType sizeType)
    {
        SizeType = sizeType;
    }

    public RowStyle(SizeType sizeType, float height)
    {
        SizeType = sizeType;
        Height = height;
    }

    public float Height
    {
        get => Size;
        set => Size = value;
    }
}
