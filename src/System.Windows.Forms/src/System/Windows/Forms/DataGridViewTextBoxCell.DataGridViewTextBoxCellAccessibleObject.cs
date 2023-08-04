﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using static Interop;

namespace System.Windows.Forms;

public partial class DataGridViewTextBoxCell
{
    protected class DataGridViewTextBoxCellAccessibleObject : DataGridViewCellAccessibleObject
    {
        public DataGridViewTextBoxCellAccessibleObject(DataGridViewCell? owner) : base(owner)
        {
        }

        internal override bool IsIAccessibleExSupported() => true;

        internal override object? GetPropertyValue(UiaCore.UIA propertyID)
            => propertyID switch
            {
                UiaCore.UIA.ControlTypePropertyId => UiaCore.UIA.DataItemControlTypeId,
                _ => base.GetPropertyValue(propertyID)
            };
    }
}
