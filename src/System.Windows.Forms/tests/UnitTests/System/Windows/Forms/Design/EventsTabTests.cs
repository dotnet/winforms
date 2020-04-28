// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using Moq;
using WinForms.Common.Tests;
using Xunit;

namespace System.Windows.Forms.Design.Tests
{
    // NB: doesn't require thread affinity
    public class EventsTabTests : IClassFixture<ThreadExceptionFixture>
    {
        [Fact]
        public void EventsTab_Ctor_IServiceProvider()
        {
            var mockServiceProvider = new Mock<IServiceProvider>(MockBehavior.Strict);
            var tab = new EventsTab(mockServiceProvider.Object);
            Assert.NotNull(tab.Bitmap);
            Assert.Null(tab.Components);
            Assert.Equal("Events", tab.HelpKeyword);
            Assert.Equal("Events", tab.TabName);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetStringWithNullTheoryData))]
        public void EventsTab_CanExtend_Invoke_ReturnsTrue(object extendee)
        {
            var mockServiceProvider = new Mock<IServiceProvider>(MockBehavior.Strict);
            var tab = new EventsTab(mockServiceProvider.Object);
            Assert.True(tab.CanExtend(extendee));
        }

        public static IEnumerable<object[]> GetDefaultProperty_TestData()
        {
            var nullMockServiceProvider = new Mock<IServiceProvider>(MockBehavior.Strict);
            nullMockServiceProvider
                .Setup(p => p.GetService(typeof(IDesignerEventService)))
                .Returns(null);

            var invalidMockServiceProvider = new Mock<IServiceProvider>(MockBehavior.Strict);
            invalidMockServiceProvider
                .Setup(p => p.GetService(typeof(IDesignerEventService)))
                .Returns(new object());

            foreach (IServiceProvider provider in new object[] { null, nullMockServiceProvider.Object, invalidMockServiceProvider.Object })
            {
                yield return new object[] { null, provider, null };
                yield return new object[] { new object(), provider, null };
                yield return new object[] { new ClassWithDefaultEvent(), provider, null };

                var nullMockSite = new Mock<ISite>();
                nullMockSite
                    .Setup(s => s.GetService(typeof(IEventBindingService)))
                    .Returns(null);
                yield return new object[] { new ClassWithDefaultEvent { Site = nullMockSite.Object }, provider, null };

                var invalidMockSite = new Mock<ISite>();
                invalidMockSite
                    .Setup(s => s.GetService(typeof(IEventBindingService)))
                    .Returns(new object());
                yield return new object[] { new ClassWithDefaultEvent { Site = invalidMockSite.Object }, provider, null };

                PropertyDescriptor descriptor = TypeDescriptor.GetProperties(typeof(ClassWithDefaultEvent))[0];
                var mockEventBindingService = new Mock<IEventBindingService>();
                mockEventBindingService
                    .Setup(e => e.GetEventProperty(TypeDescriptor.GetDefaultEvent(typeof(ClassWithDefaultEvent))))
                    .Returns(descriptor);
                var mockSite = new Mock<ISite>();
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
            var tab = new EventsTab(serviceProvider);
            Assert.Same(expected, tab.GetDefaultProperty(obj));

            // Call again to test caching behavior.
            Assert.Same(expected, tab.GetDefaultProperty(obj));
        }

        public static IEnumerable<object[]> GetDefaultProperty_IDesignerEventService_TestData()
        {
            var nullMockDesignerHost = new Mock<IDesignerHost>(MockBehavior.Strict);
            nullMockDesignerHost
                .Setup(s => s.GetService(typeof(IEventBindingService)))
                .Returns(null);
            var invalidMockDesignerHost = new Mock<IDesignerHost>(MockBehavior.Strict);
            invalidMockDesignerHost
                .Setup(s => s.GetService(typeof(IEventBindingService)))
                .Returns(new object());

            PropertyDescriptor descriptor = TypeDescriptor.GetProperties(typeof(ClassWithDefaultEvent))[0];
            var mockEventBindingService = new Mock<IEventBindingService>();
            mockEventBindingService
                .Setup(e => e.GetEventProperty(TypeDescriptor.GetDefaultEvent(typeof(ClassWithDefaultEvent))))
                .Returns(descriptor);
            var mockDesignerHost = new Mock<IDesignerHost>(MockBehavior.Strict);
            mockDesignerHost
                .Setup(s => s.GetService(typeof(IEventBindingService)))
                .Returns(mockEventBindingService.Object);

            foreach (ActiveDesignerEventArgs e in new object[] { null, new ActiveDesignerEventArgs(null, nullMockDesignerHost.Object), new ActiveDesignerEventArgs(null, invalidMockDesignerHost.Object), new ActiveDesignerEventArgs(null, mockDesignerHost.Object) })
            {
                yield return new object[] { null, e, null };
                yield return new object[] { new object(), e, null };
                yield return new object[] { new Component(), e, null };
                yield return new object[] { new ClassWithDefaultEvent(), e, e?.NewDesigner == mockDesignerHost.Object ? descriptor : null };
            }
        }

        [WinFormsTheory]
        [MemberData(nameof(GetDefaultProperty_IDesignerEventService_TestData))]
        public void EventsTab_GetDefaultProperty_IDesignerEventService_Success(object obj, ActiveDesignerEventArgs e, object expected)
        {
            using var service = new CustomDesignerEventService();
            var mockServiceProvider = new Mock<IServiceProvider>(MockBehavior.Strict);
            mockServiceProvider
                .Setup(p => p.GetService(typeof(IDesignerEventService)))
                .Returns(service);
            var tab = new EventsTab(mockServiceProvider.Object);
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

        [DefaultEvent(nameof(ClassWithDefaultEvent.Event))]
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
}
