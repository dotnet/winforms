// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Windows.Win32.System.Com;
using Windows.Win32.System.Ole;

namespace System.Windows.Forms.Tests;

[Collection("Sequential")] // workaround for WebBrowser control corrupting memory when run on multiple UI threads (instantiated via GUID)
public class AxHostConnectionPointCookieTests
{
    private static Guid CLSID_WebBrowser { get; } = new("8856f961-340a-11d0-a96b-00c04fd705a2");

    [WinFormsFact]
    public void ConnectionPointCookie_Ctor_Object_Object_Type()
    {
        Type type = Type.GetTypeFromCLSID(CLSID_WebBrowser);
        object source = Activator.CreateInstance(type);
        CustomPropertyNotifySink sink = new();
        Type eventType = typeof(IPropertyNotifySink.Interface);

        // Just verify that creation succeeded.
        var cookie = new AxHost.ConnectionPointCookie(source, sink, eventType);
        cookie.Disconnect();
    }

    public static IEnumerable<object[]> Ctor_InvalidSource_TestData()
    {
        yield return new object[] { null };
        yield return new object[] { new() };
    }

    [WinFormsTheory]
    [MemberData(nameof(Ctor_InvalidSource_TestData))]
    public void ConnectionPointCookie_Ctor_InvalidSource_ThrowsInvalidCastException(object source)
    {
        Assert.Throws<InvalidCastException>(() => new AxHost.ConnectionPointCookie(source, null, null));
        Assert.Throws<InvalidCastException>(() => new AxHost.ConnectionPointCookie(source, new object(), typeof(int)));
    }

    [WinFormsFact]
    public void ConnectionPointCookie_Ctor_NullEventInterface_ThrowsNullReferenceException()
    {
        Type type = Type.GetTypeFromCLSID(CLSID_WebBrowser);
        object source = Activator.CreateInstance(type);
        Assert.Throws<NullReferenceException>(() => new AxHost.ConnectionPointCookie(source, null, null));
    }

    [WinFormsTheory]
    [InlineData(typeof(int))]
    [InlineData(typeof(IComparable))]
    [InlineData(typeof(IConnectionPoint.Interface))]
    [InlineData(typeof(IConnectionPointContainer.Interface))]
    public void ConnectionPointCookie_Ctor_NullSink_ThrowsInvalidCastException(Type eventInterface)
    {
        Type type = Type.GetTypeFromCLSID(CLSID_WebBrowser);
        object source = Activator.CreateInstance(type);
        Assert.Throws<InvalidCastException>(() => new AxHost.ConnectionPointCookie(source, null, eventInterface));
    }

    public static IEnumerable<object[]> Ctor_InvalidSink_TestData()
    {
        yield return new object[] { null };
        yield return new object[] { new() };
    }

    [WinFormsTheory]
    [MemberData(nameof(Ctor_InvalidSink_TestData))]
    public void ConnectionPointCookie_Ctor_InvalidSink_ThrowsInvalidCastException(object sink)
    {
        Type type = Type.GetTypeFromCLSID(CLSID_WebBrowser);
        object source = Activator.CreateInstance(type);
        Type eventInterface = typeof(IPropertyNotifySink.Interface);
        Assert.Throws<InvalidCastException>(() => new AxHost.ConnectionPointCookie(source, sink, eventInterface));
    }

    [WinFormsFact]
    public void ConnectionPointCookie_Disconnect_InvokeMultipleTimes_Success()
    {
        Type type = Type.GetTypeFromCLSID(CLSID_WebBrowser);
        object source = Activator.CreateInstance(type);
        CustomPropertyNotifySink sink = new();
        Type eventType = typeof(IPropertyNotifySink.Interface);
        var cookie = new AxHost.ConnectionPointCookie(source, sink, eventType);
        cookie.Disconnect();

        // Call again.
        cookie.Disconnect();
    }

    private class CustomPropertyNotifySink : IPropertyNotifySink.Interface
    {
        public HRESULT OnChanged(int dispID) => throw new NotImplementedException();

        public HRESULT OnRequestEdit(int dispID) => throw new NotImplementedException();
    }
}
