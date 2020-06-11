// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using Xunit;
using static Interop;

namespace System.Windows.Forms.Tests
{
    [Collection("Sequential")] // workaround for WebBrowser control corrupting memory when run on multiple UI threads (instantiated via GUID)
    public class AxHostConnectionPointCookieTests : IClassFixture<ThreadExceptionFixture>
    {
        private static readonly Guid CLSID_WebBrowser = new Guid("8856f961-340a-11d0-a96b-00c04fd705a2");

        [WinFormsFact]
        public void ConnectionPointCookie_Ctor_Object_Object_Type()
        {
            Type type = Type.GetTypeFromCLSID(CLSID_WebBrowser);
            object source = Activator.CreateInstance(type);
            var sink = new CustomPropertyNotifySink();
            Type eventType = typeof(Ole32.IPropertyNotifySink);

            // Just verify that creation succeeded.
            var cookie = new AxHost.ConnectionPointCookie(source, sink, eventType);
        }

        public static IEnumerable<object[]> Ctor_InvalidSource_TestData()
        {
            yield return new object[] { null };
            yield return new object[] { new object() };
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
        [InlineData(typeof(Ole32.IConnectionPoint))]
        [InlineData(typeof(Ole32.IConnectionPointContainer))]
        public void ConnectionPointCookie_Ctor_InvalidEventInterface_ThrowsArgumentException(Type eventInterface)
        {
            Type type = Type.GetTypeFromCLSID(CLSID_WebBrowser);
            object source = Activator.CreateInstance(type);
            Assert.Throws<ArgumentException>(null, () => new AxHost.ConnectionPointCookie(source, null, eventInterface));
        }

        public static IEnumerable<object[]> Ctor_InvalidSink_TestData()
        {
            yield return new object[] { null };
            yield return new object[] { new object() };
        }

        [WinFormsTheory]
        [MemberData(nameof(Ctor_InvalidSink_TestData))]
        public void ConnectionPointCookie_Ctor_InvalidSink_ThrowsInvalidCastException(object sink)
        {
            Type type = Type.GetTypeFromCLSID(CLSID_WebBrowser);
            object source = Activator.CreateInstance(type);
            Type eventInterface = typeof(Ole32.IPropertyNotifySink);
            Assert.Throws<InvalidCastException>(() => new AxHost.ConnectionPointCookie(source, sink, eventInterface));
        }

        [WinFormsFact]
        public void ConnectionPointCookie_Disconnect_InvokeMultipleTimes_Success()
        {
            Type type = Type.GetTypeFromCLSID(CLSID_WebBrowser);
            object source = Activator.CreateInstance(type);
            var sink = new CustomPropertyNotifySink();
            Type eventType = typeof(Ole32.IPropertyNotifySink);
            var cookie = new AxHost.ConnectionPointCookie(source, sink, eventType);
            cookie.Disconnect();

            // Call again.
            cookie.Disconnect();
        }

        private class CustomPropertyNotifySink : Ole32.IPropertyNotifySink
        {
            public HRESULT OnChanged(Ole32.DispatchID dispID) => throw new NotImplementedException();

            public HRESULT OnRequestEdit(Ole32.DispatchID dispID) => throw new NotImplementedException();
        }
    }
}
