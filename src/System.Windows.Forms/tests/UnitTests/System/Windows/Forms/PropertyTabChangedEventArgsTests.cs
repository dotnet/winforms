// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Windows.Forms.Design;
using System.Windows.Forms.PropertyGridInternal;

namespace System.Windows.Forms.Tests;

// NB: doesn't require thread affinity
public class PropertyTabChangedEventArgsTests
{
    public static IEnumerable<object[]> Ctor_PropertyTab_PropertyTab_TestData()
    {
        yield return new object[] { null, null };
        yield return new object[] { new EventsTab(null), new PropertiesTab() };
    }

    [Theory]
    [MemberData(nameof(Ctor_PropertyTab_PropertyTab_TestData))]
    public void Ctor_PropertyTab_PropertyTab(PropertyTab oldTab, PropertyTab newTab)
    {
        PropertyTabChangedEventArgs e = new(oldTab, newTab);
        Assert.Equal(oldTab, e.OldTab);
        Assert.Equal(newTab, e.NewTab);
    }
}
