// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Reflection;
using Castle.Core.Internal;

namespace System.Windows.Forms.Design.Tests;

public class DataMemberListEditorTests
{
    [Fact]
    public void DataMemberListEditorExist()
    {
        PropertyInfo propertyInfo = typeof(DataGridView).GetProperty(nameof(DataGridView.DataMember));

        string editorTypeName = propertyInfo.GetAttribute<EditorAttribute>().EditorTypeName;

        Type.GetType(editorTypeName).Should().NotBeNull();
    }
}
