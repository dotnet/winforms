﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using static Interop;

namespace System.Windows.Forms.Tests;

public class DataGridViewTextBoxCell_DataGridViewTextBoxCellAccessibleObject
{
    [WinFormsFact]
    public void DataGridViewTextBoxCellAccessibleObject_ControlType_IsDataItem()
    {
        using var cell = new DataGridViewTextBoxCell();
        AccessibleObject accessibleObject = cell.AccessibilityObject;

        object actual = accessibleObject.GetPropertyValue(UiaCore.UIA.ControlTypePropertyId);

        Assert.Equal(UiaCore.UIA.DataItemControlTypeId, actual);
    }
}
