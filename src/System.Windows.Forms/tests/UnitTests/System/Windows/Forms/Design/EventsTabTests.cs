// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.ComponentModel.Design;
using Moq;

namespace System.Windows.Forms.Design.Tests;

// NB: doesn't require thread affinity
public class EventsTabTests
{
    [Fact]
    public void EventsTab_Ctor_IServiceProvider()
    {
        Mock<IServiceProvider> mockServiceProvider = new(MockBehavior.Strict);
        EventsTab tab = new(mockServiceProvider.Object);
        Assert.NotNull(tab.Bitmap);
        Assert.Null(tab.Components);
        Assert.Equal("Events", tab.HelpKeyword);
        Assert.Equal("Events", tab.TabName);
    }

    [Theory]
    [StringWithNullData]
    public void EventsTab_CanExtend_Invoke_ReturnsTrue(object extendee)
    {
        Mock<IServiceProvider> mockServiceProvider = new(MockBehavior.Strict);
        EventsTab tab = new(mockServiceProvider.Object);
        Assert.True(tab.CanExtend(extendee));
    }

    public static IEnumerable<object[]> GetDefaultProperty_TestData()
    {
        Mock<IServiceProvider> nullMockServiceProvider = new(MockBehavior.Strict);
        nullMockServiceProvider
            .Setup(p => p.GetService(typeof(IDesignerEventService)))
            .Returns(null);

        Mock<IServiceProvider> invalidMockServiceProvider = new(MockBehavior.Strict);
        invalidMockServiceProvider
            .Setup(p => p.GetService(typeof(IDesignerEventService)))
            .Returns(new object());

        foreach (IServiceProvider provider in new object[] { null, nullMockServiceProvider.Object, invalidMockServiceProvider.Object })
        {
            yield return new object[] { null, provider, null };
            yield return new object[] { new(), provider, null };
            yield return new object[] { new ClassWithDefaultEvent(), provider, null };

            Mock<ISite> nullMockSite = new();
            nullMockSite
                .Setup(s => s.GetService(typeof(IEventBindingService)))
                .Returns(null);
            yield return new object[] { new ClassWithDefaultEvent { Site = nullMockSite.Object }, provider, null };

            Mock<ISite> invalidMockSite = new();
            invalidMockSite
                .Setup(s => s.GetService(typeof(IEventBindingService)))
                .Returns(new object());
            yield return new object[] { new ClassWithDefaultEvent { Site = invalidMockSite.Object }, provider, null };

            PropertyDescriptor descriptor = TypeDescriptor.GetProperties(typeof(ClassWithDefaultEvent))[0];
            Mock<IEventBindingService> mockEventBindingService = new();
            mockEventBindingService
                .Setup(e => e.GetEventProperty(TypeDescriptor.GetDefaultEvent(typeof(ClassWithDefaultEvent))))
                .Returns(descriptor);
            Mock<ISite> mockSite = new();
            mockSite
                .Setup(s => s.GetService(typeof(IEventBindingService)))
                .Returns(mockEventBindingService.Object);
            yield return new object[] { new Component { Site = mockSite.Object }, provider, null };
            yield return new object[] { new ClassWithDefaultEvent { Site = mockSite.Object }, provider, descriptor };
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(GetDefaultProperty_TestData))]
    public void EventsTab_GetDefaultProperty_Invoke_ReturnsExpected(object obj, IServiceProvider serviceProvider, object expected)
    {
        EventsTab tab = new(serviceProvider);
        Assert.Same(expected, tab.GetDefaultProperty(obj));

        // Call again to test caching behavior.
        Assert.Same(expected, tab.GetDefaultProperty(obj));
    }

    public static IEnumerable<object[]> GetDefaultProperty_IDesignerEventService_TestData()
    {
        Mock<IDesignerHost> nullMockDesignerHost = new(MockBehavior.Strict);
        nullMockDesignerHost
            .Setup(s => s.GetService(typeof(IEventBindingService)))
            .Returns(null);
        Mock<IDesignerHost> invalidMockDesignerHost = new(MockBehavior.Strict);
        invalidMockDesignerHost
            .Setup(s => s.GetService(typeof(IEventBindingService)))
            .Returns(new object());

        PropertyDescriptor descriptor = TypeDescriptor.GetProperties(typeof(ClassWithDefaultEvent))[0];
        Mock<IEventBindingService> mockEventBindingService = new();
        mockEventBindingService
            .Setup(e => e.GetEventProperty(TypeDescriptor.GetDefaultEvent(typeof(ClassWithDefaultEvent))))
            .Returns(descriptor);
        Mock<IDesignerHost> mockDesignerHost = new(MockBehavior.Strict);
        mockDesignerHost
            .Setup(s => s.GetService(typeof(IEventBindingService)))
            .Returns(mockEventBindingService.Object);

        foreach (ActiveDesignerEventArgs e in new object[]
            {
                new ActiveDesignerEventArgs(null, nullMockDesignerHost.Object),
                new ActiveDesignerEventArgs(null, invalidMockDesignerHost.Object),
                new ActiveDesignerEventArgs(null, mockDesignerHost.Object)
            })
        {
            yield return new object[] { null, e, null };
            yield return new object[] { new(), e, null };
            yield return new object[] { new Component(), e, null };
            yield return new object[] { new ClassWithDefaultEvent(), e, e?.NewDesigner == mockDesignerHost.Object ? descriptor : null };
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(GetDefaultProperty_IDesignerEventService_TestData))]
    public void EventsTab_GetDefaultProperty_IDesignerEventService_Success(object obj, ActiveDesignerEventArgs e, object expected)
    {
        using CustomDesignerEventService service = new();
        Mock<IServiceProvider> mockServiceProvider = new(MockBehavior.Strict);
        mockServiceProvider
            .Setup(p => p.GetService(typeof(IDesignerEventService)))
            .Returns(service);
        EventsTab tab = new(mockServiceProvider.Object);
        Assert.Null(tab.GetDefaultProperty(obj));

        service.OnActiveDesignerChanged(e);
        Assert.Equal(expected, tab.GetDefaultProperty(obj));
    }

    private class CustomDesignerEventService : Component, IDesignerEventService
    {
        public IDesignerHost ActiveDesigner { get; set; }
        public DesignerCollection Designers { get; }

        public event ActiveDesignerEventHandler ActiveDesignerChanged
        {
            add => Events.AddHandler(nameof(ActiveDesignerChanged), value);
            remove => Events.RemoveHandler(nameof(ActiveDesignerChanged), value);
        }

        public event DesignerEventHandler DesignerCreated
        {
            add => Events.AddHandler(nameof(DesignerCreated), value);
            remove => Events.RemoveHandler(nameof(DesignerCreated), value);
        }

        public event DesignerEventHandler DesignerDisposed
        {
            add => Events.AddHandler(nameof(DesignerDisposed), value);
            remove => Events.RemoveHandler(nameof(DesignerDisposed), value);
        }

        public event EventHandler SelectionChanged
        {
            add => Events.AddHandler(nameof(SelectionChanged), value);
            remove => Events.RemoveHandler(nameof(SelectionChanged), value);
        }

        public void OnActiveDesignerChanged(ActiveDesignerEventArgs e)
        {
            ActiveDesignerEventHandler handler = (ActiveDesignerEventHandler)Events[nameof(ActiveDesignerChanged)];
            handler?.Invoke(this, e);
        }
    }

    [DefaultEvent(nameof(Event))]
    private class ClassWithDefaultEvent : Component
    {
        public int Value { get; set; }

        public event EventHandler Event
        {
            add { }
            remove { }
        }
    }
}
