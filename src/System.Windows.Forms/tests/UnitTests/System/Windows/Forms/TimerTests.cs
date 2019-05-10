// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using Moq;
using WinForms.Common.Tests;
using Xunit;

namespace System.Windows.Forms.Tests
{
    public class TimerTests
    {
        [Fact]
        public void Timer_Ctor_Default()
        {
            var timer = new SubTimer();
            Assert.Null(timer.Container);
            Assert.False(timer.DesignMode);
            Assert.False(timer.Enabled);
            Assert.Equal(100, timer.Interval);
            Assert.Null(timer.Site);
            Assert.Null(timer.Tag);
        }

        [Fact]
        public void Timer_Ctor_IContainer()
        {
            var container = new Container();
            var timer = new SubTimer(container);
            Assert.Same(container, timer.Container);
            Assert.False(timer.DesignMode);
            Assert.False(timer.Enabled);
            Assert.Equal(100, timer.Interval);
            Assert.NotNull(timer.Site);
            Assert.Null(timer.Tag);
        }

        [Fact]
        public void Timer_Ctor_NullContainer_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>("container", () => new Timer(null));
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void Timer_Enabled_Set_GetReturnsExpected(bool value)
        {
            var timer = new Timer
            {
                Enabled = value
            };
            Assert.Equal(value, timer.Enabled);

            // Set same.
            timer.Enabled = value;
            Assert.Equal(value, timer.Enabled);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void Timer_Enabled_SetDesignMode_GetReturnsExpected(bool value)
        {
            var timer = new SubTimer();
            var mockSite = new Mock<ISite>(MockBehavior.Strict);
            mockSite
                .Setup(s => s.DesignMode)
                .Returns(true);
            timer.Site = mockSite.Object;
            Assert.True(timer.DesignMode);

            timer.Enabled = value;
            Assert.Equal(value, timer.Enabled);

            // Set same.
            timer.Enabled = value;
            Assert.Equal(value, timer.Enabled);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void Timer_Enabled_SetDesignModeAfterEnabling_GetReturnsExpected(bool value)
        {
            var timer = new SubTimer();
            var mockSite = new Mock<ISite>(MockBehavior.Strict);
            mockSite
                .Setup(s => s.DesignMode)
                .Returns(true);
            timer.Site = mockSite.Object;
            Assert.True(timer.DesignMode);

            timer.Start();

            mockSite
                .Setup(s => s.DesignMode)
                .Returns(false);

            timer.Enabled = value;
            Assert.Equal(value, timer.Enabled);

            // Set same.
            timer.Enabled = value;
            Assert.Equal(value, timer.Enabled);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(100)]
        public void Timer_Interval_Set_GetReturnsExpected(int value)
        {
            var timer = new Timer
            {
                Interval = value
            };
            Assert.Equal(value, timer.Interval);

            // Set same.
            timer.Interval = value;
            Assert.Equal(value, timer.Interval);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(100)]
        public void Timer_Interval_SetStarted_GetReturnsExpected(int value)
        {
            var timer = new Timer();
            timer.Start();

            timer.Interval = value;
            Assert.Equal(value, timer.Interval);

            // Set same.
            timer.Interval = value;
            Assert.Equal(value, timer.Interval);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(100)]
        public void Timer_Interval_SetStopped_GetReturnsExpected(int value)
        {
            var timer = new Timer();
            timer.Start();
            timer.Stop();

            timer.Interval = value;
            Assert.Equal(value, timer.Interval);

            // Set same.
            timer.Interval = value;
            Assert.Equal(value, timer.Interval);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(100)]
        public void Timer_Interval_SetDesignMode_GetReturnsExpected(int value)
        {
            var timer = new SubTimer();
            var mockSite = new Mock<ISite>(MockBehavior.Strict);
            mockSite
                .Setup(s => s.DesignMode)
                .Returns(true);
            timer.Site = mockSite.Object;
            Assert.True(timer.DesignMode);

            timer.Interval = value;
            Assert.Equal(value, timer.Interval);

            // Set same.
            timer.Interval = value;
            Assert.Equal(value, timer.Interval);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(100)]
        public void Timer_Interval_SetDesignModeAfterEnabling_GetReturnsExpected(int value)
        {
            var timer = new SubTimer();
            var mockSite = new Mock<ISite>(MockBehavior.Strict);
            mockSite
                .Setup(s => s.DesignMode)
                .Returns(true);
            timer.Site = mockSite.Object;
            Assert.True(timer.DesignMode);

            timer.Start();

            mockSite
                .Setup(s => s.DesignMode)
                .Returns(false);

            timer.Interval = value;
            Assert.Equal(value, timer.Interval);

            // Set same.
            timer.Interval = value;
            Assert.Equal(value, timer.Interval);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void Timer_Interval_SetInvalid_ThrowsArgumentOutOfRangeException(int value)
        {
            var timer = new Timer();
            Assert.Throws<ArgumentOutOfRangeException>("value", () => timer.Interval = value);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetStringWithNullTheoryData))]
        public void Timer_Tag_Set_GetReturnsExpected(object value)
        {
            var timer = new Timer
            {
                Tag = value
            };
            Assert.Same(value, timer.Tag);

            // Set same.
            timer.Tag = value;
            Assert.Same(value, timer.Tag);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetStringWithNullTheoryData))]
        public void Timer_Tag_SetDesignMode_GetReturnsExpected(object value)
        {
            var timer = new SubTimer();
            var mockSite = new Mock<ISite>(MockBehavior.Strict);
            mockSite
                .Setup(s => s.DesignMode)
                .Returns(true);
            timer.Site = mockSite.Object;
            Assert.True(timer.DesignMode);

            timer.Tag = value;
            Assert.Same(value, timer.Tag);

            // Set same.
            timer.Tag = value;
            Assert.Same(value, timer.Tag);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void Timer_Start_Stop_Success(bool designMode)
        {
            var timer = new SubTimer();
            var mockSite = new Mock<ISite>(MockBehavior.Strict);
            mockSite
                .Setup(s => s.DesignMode)
                .Returns(designMode);
            timer.Site = mockSite.Object;
            Assert.Equal(designMode, timer.DesignMode);

            // Start
            timer.Start();
            Assert.True(timer.Enabled);

            // Stop.
            timer.Stop();
            Assert.False(timer.Enabled);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void Timer_Start_MultipleTimes_Success(bool designMode)
        {
            var timer = new SubTimer();
            var mockSite = new Mock<ISite>(MockBehavior.Strict);
            mockSite
                .Setup(s => s.DesignMode)
                .Returns(designMode);
            timer.Site = mockSite.Object;
            Assert.Equal(designMode, timer.DesignMode);

            // Start
            timer.Start();
            Assert.True(timer.Enabled);

            // Start again.
            timer.Start();
            Assert.True(timer.Enabled);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void Timer_Stop_Restart_Success(bool designMode)
        {
            var timer = new SubTimer();
            var mockSite = new Mock<ISite>(MockBehavior.Strict);
            mockSite
                .Setup(s => s.DesignMode)
                .Returns(designMode);
            timer.Site = mockSite.Object;
            Assert.Equal(designMode, timer.DesignMode);

            // Start
            timer.Start();
            Assert.True(timer.Enabled);

            // Stop.
            timer.Stop();
            Assert.False(timer.Enabled);

            // Start again.
            timer.Start();
            Assert.True(timer.Enabled);

            // Stop again.
            timer.Stop();
            Assert.False(timer.Enabled);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void Timer_Stop_MultipleTimes_Success(bool designMode)
        {
            var timer = new SubTimer();
            var mockSite = new Mock<ISite>(MockBehavior.Strict);
            mockSite
                .Setup(s => s.DesignMode)
                .Returns(designMode);
            timer.Site = mockSite.Object;
            Assert.Equal(designMode, timer.DesignMode);

            // Start
            timer.Start();
            Assert.True(timer.Enabled);

            // Stop.
            timer.Stop();
            Assert.False(timer.Enabled);

            // Stop again.
            timer.Stop();
            Assert.False(timer.Enabled);
        }

        [Fact]
        public void Timer_OnTick_Invoke_CallsTick()
        {
            var timer = new SubTimer();
            var eventArgs = new EventArgs();
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(timer, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };

            // Call with handler.
            timer.Tick += handler;
            timer.OnTick(eventArgs);
            Assert.Equal(1, callCount);

            // Remove handler.
            timer.Tick -= handler;
            timer.OnTick(eventArgs);
            Assert.Equal(1, callCount);
        }

        [Fact]
        public void Timer_Dispose_NotStarted_Succcess()
        {
            var timer = new Timer();
            timer.Dispose();
            Assert.False(timer.Enabled);

            // Call again.
            timer.Dispose();
            Assert.False(timer.Enabled);
        }

        [Fact]
        public void Timer_Dispose_Started_Succcess()
        {
            var timer = new Timer();
            timer.Start();

            timer.Dispose();
            Assert.False(timer.Enabled);

            // Call again.
            timer.Dispose();
            Assert.False(timer.Enabled);
        }

        [Fact]
        public void Timer_Dispose_Stopped_Succcess()
        {
            var timer = new Timer();
            timer.Start();
            timer.Stop();

            timer.Dispose();
            Assert.False(timer.Enabled);

            // Call again.
            timer.Dispose();
            Assert.False(timer.Enabled);
        }

        [Fact]
        public void Timer_ToString_Invoke_ReturnsExpected()
        {
            var timer = new Timer();
            Assert.Equal("System.Windows.Forms.Timer, Interval: 100", timer.ToString());
        }

        private class SubTimer : Timer
        {
            public SubTimer() : base()
            {
            }

            public SubTimer(IContainer container) : base(container)
            {
            }

            public new bool DesignMode => base.DesignMode;

            public new void OnTick(EventArgs e) => base.OnTick(e);
        }
    }
}
