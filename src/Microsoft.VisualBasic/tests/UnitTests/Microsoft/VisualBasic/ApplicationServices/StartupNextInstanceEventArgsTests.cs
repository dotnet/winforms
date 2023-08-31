// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections.ObjectModel;

namespace Microsoft.VisualBasic.ApplicationServices.Tests;

public class StartupNextInstanceEventArgsTests
{
    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void Ctor_ReadOnlyCollection_Boolean(bool bringToForeground)
    {
        var collection = new ReadOnlyCollection<string>(new string[] { "a" });
        var args = new StartupNextInstanceEventArgs(collection, bringToForeground);
        Assert.Same(collection, args.CommandLine);
        Assert.Equal(bringToForeground, args.BringToForeground);
    }

    [Fact]
    public void Ctor_NullCommandLine_ThrowsArgumentNullException()
    {
        AssertExtensions.Throws<ArgumentNullException>("list", () => new StartupNextInstanceEventArgs(null, bringToForegroundFlag: true));
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void BringToForeground_Set_GetReturnsExpected(bool value)
    {
        var collection = new ReadOnlyCollection<string>(new string[] { "a" });
        var args = new StartupNextInstanceEventArgs(collection, bringToForegroundFlag: true)
        {
            BringToForeground = value
        };
        Assert.Equal(value, args.BringToForeground);
    }
}
