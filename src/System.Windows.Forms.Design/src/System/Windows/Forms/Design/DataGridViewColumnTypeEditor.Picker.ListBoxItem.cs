// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms.Design;

internal partial class DataGridViewColumnTypePicker : ContainerControl
{
    private class ListBoxItem
    {
        private readonly Type _columnType;

        public ListBoxItem(Type columnType) => _columnType = columnType;

        public override string ToString() => _columnType.Name;

        public Type ColumnType => _columnType;
    }
}
