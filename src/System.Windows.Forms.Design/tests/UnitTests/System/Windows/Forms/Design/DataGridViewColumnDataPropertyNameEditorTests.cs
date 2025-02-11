// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Reflection;
using Castle.Core.Internal;

namespace System.Windows.Forms.Design.Tests;

public class DataGridViewColumnDataPropertyNameEditorTests
{
    [Fact]
    public void DataGridViewColumnDataPropertyNameEditorExist()
    {
        PropertyInfo propertyInfo = typeof(DataGridViewColumn).GetProperty(nameof(DataGridViewColumn.DataPropertyName));

        string editorTypeName = propertyInfo.GetAttribute<EditorAttribute>().EditorTypeName;

        Type.GetType(editorTypeName).Should().NotBeNull();
    }
}
