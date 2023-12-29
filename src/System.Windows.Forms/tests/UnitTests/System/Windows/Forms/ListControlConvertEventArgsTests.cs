// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms.Tests;

// NB: doesn't require thread affinity
public class ListControlConvertEventArgsTests
{
    [Theory]
    [InlineData("value", typeof(int), 1)]
    [InlineData(null, null, null)]
    public void Ctor_Object_Type(object value, Type desiredType, object listItem)
    {
        ListControlConvertEventArgs e = new(value, desiredType, listItem);
        Assert.Equal(value, e.Value);
        Assert.Equal(desiredType, e.DesiredType);
        Assert.Equal(listItem, e.ListItem);
    }
}
