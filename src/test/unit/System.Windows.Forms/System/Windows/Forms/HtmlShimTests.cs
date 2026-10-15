// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Windows.Win32.Web.MsHtml;

namespace System.Windows.Forms.Tests;

/// <summary>
///  Verifies subscription bookkeeping independently of MSHTML and COM reference-count caching.
/// </summary>
public class HtmlShimTests
{
    [Fact]
    public void RemoveHandler_UnmatchedOrNull_PreservesRegisteredHandlers()
    {
        using TestShim shim = new();
        object key = new();
        int calls = 0;
        EventHandler handler = (_, _) => calls++;

        shim.AddHandler(key, handler);
        shim.RemoveHandler(key, new EventHandler((_, _) => { }));
        shim.RemoveHandler(new object(), handler);
        shim.RemoveHandler(key, null);
        shim.FireEvent(key, EventArgs.Empty);

        Assert.True(shim.Connected);
        Assert.Equal(1, calls);
        Assert.Equal(0, shim.DisconnectCalls);
    }

    [Fact]
    public void AddHandler_Null_DoesNotConnect()
    {
        using TestShim shim = new();

        shim.AddHandler(new object(), null);

        Assert.False(shim.Connected);
    }

    [Fact]
    public void RemoveHandler_PartOfMulticast_PreservesRemainingHandler()
    {
        using TestShim shim = new();
        object key = new();
        int firstCalls = 0;
        int secondCalls = 0;
        EventHandler first = (_, _) => firstCalls++;
        EventHandler second = (_, _) => secondCalls++;
        shim.AddHandler(key, first + second);

        shim.RemoveHandler(key, first);
        shim.FireEvent(key, EventArgs.Empty);

        Assert.True(shim.Connected);
        Assert.Equal(0, firstCalls);
        Assert.Equal(1, secondCalls);

        shim.RemoveHandler(key, second);
        shim.RemoveHandler(key, second);
        Assert.False(shim.Connected);
        Assert.Equal(1, shim.DisconnectCalls);
    }

    [Fact]
    public void RemoveHandler_Multicast_RemovesActualInvocationCount()
    {
        using TestShim shim = new();
        object key = new();
        object otherKey = new();
        EventHandler first = (_, _) => { };
        EventHandler second = (_, _) => { };
        shim.AddHandler(key, first);
        shim.AddHandler(key, second);
        shim.AddHandler(otherKey, first);

        shim.RemoveHandler(key, first + second);
        Assert.True(shim.Connected);
        shim.RemoveHandler(otherKey, first);
        Assert.False(shim.Connected);
    }

    [Fact]
    public void RemoveHandler_Duplicate_RemovesOneOccurrence()
    {
        using TestShim shim = new();
        object key = new();
        int calls = 0;
        EventHandler handler = (_, _) => calls++;
        shim.AddHandler(key, handler);
        shim.AddHandler(key, handler);

        shim.RemoveHandler(key, handler);
        shim.FireEvent(key, EventArgs.Empty);

        Assert.Equal(1, calls);
        Assert.True(shim.Connected);
        shim.RemoveHandler(key, handler);
        Assert.False(shim.Connected);
    }

    [Fact]
    public void DetachEventHandler_SameDelegateDifferentNames_DetachesCorrectProxy()
    {
        using TestShim shim = new();
        int calls = 0;
        EventHandler handler = (_, _) => calls++;
        shim.AttachEventHandler("onclick", handler);
        shim.AttachEventHandler("onmouseover", handler);

        shim.DetachEventHandler("onclick", handler);
        HtmlToClrEventProxy remaining = Assert.Single(shim.Attached);
        Assert.Equal("onmouseover", remaining.EventName);
        remaining.OnHtmlEvent();
        Assert.Equal(1, calls);
        shim.DetachEventHandler("onmouseover", handler);
        Assert.Empty(shim.Attached);
    }

    [Fact]
    public void DetachEventHandler_Duplicate_DetachesMostRecentRegistration()
    {
        using TestShim shim = new();
        EventHandler handler = (_, _) => { };
        shim.AttachEventHandler("onclick", handler);
        HtmlToClrEventProxy first = Assert.Single(shim.Attached);
        shim.AttachEventHandler("onclick", handler);

        shim.DetachEventHandler("onclick", handler);

        Assert.Same(first, Assert.Single(shim.Attached));
        shim.DetachEventHandler("onclick", handler);
        Assert.Empty(shim.Attached);
    }

    [Fact]
    public void DetachEventHandler_Unmatched_DoesNotConsumeRegistration()
    {
        using TestShim shim = new();
        EventHandler handler = (_, _) => { };
        shim.AttachEventHandler("onclick", handler);

        shim.DetachEventHandler("onmouseover", handler);
        shim.DetachEventHandler("onclick", new EventHandler((_, _) => { }));
        Assert.Single(shim.Attached);

        shim.DetachEventHandler("onclick", handler);
        shim.DetachEventHandler("onclick", handler);
        Assert.Empty(shim.Attached);
        Assert.Equal(1, shim.DetachCalls);
    }

    [Fact]
    public void RemoveHandler_LastStandardHandler_DoesNotDetachAttachedHandlers()
    {
        using TestShim shim = new();
        object key = new();
        EventHandler handler = (_, _) => { };
        shim.AddHandler(key, handler);
        shim.AttachEventHandler("onclick", handler);

        shim.RemoveHandler(key, handler);

        Assert.False(shim.Connected);
        Assert.Single(shim.Attached);
        Assert.Equal(0, shim.DetachCalls);
    }

    [Fact]
    public void AttachEventHandler_Failure_DoesNotTrackFailedRegistration()
    {
        using TestShim shim = new() { FailAttach = true };

        Assert.Throws<InvalidOperationException>(() => shim.AttachEventHandler("onclick", (_, _) => { }));
        shim.Dispose();

        Assert.Empty(shim.Attached);
        Assert.Equal(0, shim.DetachCalls);
    }

    [Fact]
    public void AttachEventHandler_NativeRejection_DoesNotTrackRegistration()
    {
        using TestShim shim = new() { RejectAttach = true };

        shim.AttachEventHandler(null!, (_, _) => { });
        shim.Dispose();

        Assert.Empty(shim.Attached);
        Assert.Equal(0, shim.DetachCalls);
    }

    [Fact]
    public void DetachEventHandler_Failure_PreservesRegistrationForRetry()
    {
        using TestShim shim = new();
        EventHandler handler = (_, _) => { };
        shim.AttachEventHandler("onclick", handler);
        shim.FailDetach = true;

        Assert.Throws<InvalidOperationException>(() => shim.DetachEventHandler("onclick", handler));
        Assert.Single(shim.Attached);

        shim.FailDetach = false;
        shim.DetachEventHandler("onclick", handler);
        Assert.Empty(shim.Attached);
    }

    [Fact]
    public void DetachEventHandler_ReentrantDetach_DoesNotDetachSameProxyTwice()
    {
        using TestShim shim = new();
        EventHandler handler = (_, _) => { };
        shim.AttachEventHandler("onclick", handler);
        shim.OnDetach = () => shim.DetachEventHandler("onclick", handler);

        shim.DetachEventHandler("onclick", handler);

        Assert.Empty(shim.Attached);
        Assert.Equal(1, shim.DetachCalls);
    }

    [Fact]
    public void Dispose_DetachesEveryRegistrationAndIsReentrant()
    {
        TestShim shim = new();
        EventHandler handler = (_, _) => { };
        shim.AttachEventHandler("onclick", handler);
        shim.AttachEventHandler("onclick", handler);
        shim.AttachEventHandler("onmouseover", handler);
        shim.OnDetach = shim.Dispose;

        shim.Dispose();
        shim.Dispose();

        Assert.True(shim.IsDisposed);
        Assert.Empty(shim.Attached);
        Assert.Equal(3, shim.DetachCalls);
        Assert.Equal(1, shim.ResourceReleaseCalls);
    }

    [Fact]
    public void Dispose_DetachFailure_AttemptsRemainingRegistrationsAndReleasesOwner()
    {
        TestShim shim = new();
        shim.AttachEventHandler("onclick", (_, _) => { });
        shim.AttachEventHandler("onmouseover", (_, _) => { });
        shim.FailDetach = true;

        AggregateException exception = Assert.Throws<AggregateException>(shim.Dispose);

        Assert.Equal(2, exception.InnerExceptions.Count);
        Assert.Equal(2, shim.DetachCalls);
        Assert.Equal(1, shim.ResourceReleaseCalls);
        shim.Dispose();
        Assert.Equal(2, shim.DetachCalls);
    }

    [Fact]
    public void DisposeAll_Failure_DoesNotSkipAnotherShim()
    {
        TestShim first = new() { FailDetach = true };
        TestShim second = new();
        first.AttachEventHandler("onclick", (_, _) => { });
        second.AttachEventHandler("onclick", (_, _) => { });

        Assert.Throws<InvalidOperationException>(() => HtmlShim.DisposeAll([first, second]));

        Assert.Equal(1, first.ResourceReleaseCalls);
        Assert.Equal(1, second.ResourceReleaseCalls);
        Assert.Empty(second.Attached);
    }

    [Fact]
    public void Dispose_PreventsNewRegistrationsAndClearsStandardHandlers()
    {
        using TestShim shim = new();
        object key = new();
        int calls = 0;
        shim.AddHandler(key, new EventHandler((_, _) => calls++));
        shim.Dispose();

        shim.FireEvent(key, EventArgs.Empty);

        Assert.Equal(0, calls);
        Assert.Throws<ObjectDisposedException>(() => shim.AddHandler(key, new EventHandler((_, _) => { })));
        Assert.Throws<ObjectDisposedException>(() => shim.AttachEventHandler("onclick", (_, _) => { }));
    }

    /// <summary>
    ///  Models native registrations by proxy identity without requiring MSHTML or an RCW cache.
    /// </summary>
    private sealed class TestShim : HtmlShim
    {
        public List<HtmlToClrEventProxy> Attached { get; } = [];
        public bool Connected { get; private set; }
        public int DisconnectCalls { get; private set; }
        public int DetachCalls { get; private set; }
        public int ResourceReleaseCalls { get; private set; }
        public bool FailAttach { get; set; }
        public bool RejectAttach { get; set; }
        public bool FailDetach { get; set; }
        public Action? OnDetach { get; set; }

        public override IHTMLWindow2.Interface? AssociatedWindow => null;

        public override void ConnectToEvents() => Connected = true;

        public override void DisconnectFromEvents()
        {
            DisconnectCalls++;
            Connected = false;
        }

        protected override bool AttachEventProxy(HtmlToClrEventProxy proxy)
        {
            if (FailAttach)
            {
                throw new InvalidOperationException("Injected native attach failure.");
            }

            if (RejectAttach)
            {
                return false;
            }

            Attached.Add(proxy);
            return true;
        }

        protected override void DetachEventProxy(HtmlToClrEventProxy proxy)
        {
            DetachCalls++;
            if (FailDetach)
            {
                throw new InvalidOperationException("Injected native detach failure.");
            }

            OnDetach?.Invoke();
            Assert.True(Attached.Remove(proxy));
        }

        protected override object GetEventSender() => this;

        protected override void Dispose(bool disposing)
        {
            try
            {
                base.Dispose(disposing);
            }
            finally
            {
                ResourceReleaseCalls++;
            }
        }
    }
}
