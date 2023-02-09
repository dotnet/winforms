// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Windows.Win32.System.Com;
using Xunit;

namespace System.Windows.Forms.Tests;

public unsafe class AxHost_PropertyBagStreamTests : IClassFixture<ThreadExceptionFixture>
{
    [WinFormsFact]
    public void PropertyBagStream_WriteReadRoundTrip()
    {
        AxHost.PropertyBagStream bag = new();
        HRESULT hr = bag.Write("Integer", (VARIANT)42);
        Assert.True(hr.Succeeded);
        NameClass obj = new() { Name = "Hamlet" };
        hr = bag.Write("Object", VARIANT.FromObject(obj));
        Assert.True(hr.Succeeded);

        using MemoryStream stream = new();
        bag.Write(stream);
        Assert.NotEqual(0, stream.Length);
        stream.Position = 0;

        AxHost.PropertyBagStream newBag = new();
        newBag.Read(stream);

        VARIANT integer = new();
        hr = newBag.Read("Integer", ref integer, null);
        Assert.True(hr.Succeeded);
        Assert.Equal(42, (int)integer);

        VARIANT dispatch = new();
        hr = newBag.Read("Object", ref dispatch, null);
        Assert.True(hr.Succeeded);
        Assert.Equal(obj.Name, ((NameClass)dispatch.ToObject()).Name);
    }

    [Serializable]
    private class NameClass
    {
        public string Name { get; set; }
    }
}
