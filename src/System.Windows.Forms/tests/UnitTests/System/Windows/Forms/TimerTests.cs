// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using Moq;

namespace System.Windows.Forms.Tests;

public class TimerTests
{
    [WinFormsFact]
    public void Timer_Ctor_Default()
    {
        using SubTimer timer = new();
        Assert.Null(timer.Container);
        Assert.False(timer.DesignMode);
        Assert.False(timer.Enabled);
        Assert.Equal(100, timer.Interval);
        Assert.Null(timer.Site);
        Assert.Null(timer.Tag);
    }

    [WinFormsFact]
    public void Timer_Ctor_IContainer()
    {
        using Container container = new();
        using SubTimer timer = new(container);
        Assert.Same(container, timer.Container);
        Assert.False(timer.DesignMode);
        Assert.False(timer.Enabled);
        Assert.Equal(100, timer.Interval);
        Assert.NotNull(timer.Site);
        Assert.Null(timer.Tag);
    }

    [WinFormsFact]
    public void Timer_Ctor_NullContainer_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>("container", () => new Timer(null));
    }

    [WinFormsTheory]
    [BoolData]
    public void Timer_Enabled_Set_GetReturnsExpected(bool value)
    {
        using Timer timer = new()
        {
            Enabled = value
        };
        Assert.Equal(value, timer.Enabled);

        // Set same.
        timer.Enabled = value;
        Assert.Equal(value, timer.Enabled);
    }

    [WinFormsTheory]
    [BoolData]
    public void Timer_Enabled_SetDesignMode_GetReturnsExpected(bool value)
    {
        using SubTimer timer = new();
        Mock<ISite> mockSite = new(MockBehavior.Strict);
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

        // NB: disposing the component with strictly mocked object causes tests to fail
        // Moq.MockException : ISite.Container invocation failed with mock behavior Strict.
        // All invocations on the mock must have a corresponding setup.
        timer.Site = null;
    }

    [WinFormsTheory]
    [BoolData]
    public void Timer_Enabled_SetDesignModeAfterEnabling_GetReturnsExpected(bool value)
    {
        using SubTimer timer = new();
        Mock<ISite> mockSite = new(MockBehavior.Strict);
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

        // NB: disposing the component with strictly mocked object causes tests to fail
        // Moq.MockException : ISite.Container invocation failed with mock behavior Strict.
        // All invocations on the mock must have a corresponding setup.
        timer.Site = null;
    }

    [WinFormsTheory]
    [InlineData(1)]
    [InlineData(100)]
    public void Timer_Interval_Set_GetReturnsExpected(int value)
    {
        using Timer timer = new()
        {
            Interval = value
        };
        Assert.Equal(value, timer.Interval);

        // Set same.
        timer.Interval = value;
        Assert.Equal(value, timer.Interval);
    }

    [WinFormsTheory]
    [InlineData(1)]
    [InlineData(100)]
    public void Timer_Interval_SetStarted_GetReturnsExpected(int value)
    {
        using Timer timer = new();
        timer.Start();

        timer.Interval = value;
        Assert.Equal(value, timer.Interval);

        // Set same.
        timer.Interval = value;
        Assert.Equal(value, timer.Interval);
    }

    [WinFormsTheory]
    [InlineData(1)]
    [InlineData(100)]
    public void Timer_Interval_SetStopped_GetReturnsExpected(int value)
    {
        using Timer timer = new();
        timer.Start();
        timer.Stop();

        timer.Interval = value;
        Assert.Equal(value, timer.Interval);

        // Set same.
        timer.Interval = value;
        Assert.Equal(value, timer.Interval);
    }

    [WinFormsTheory]
    [InlineData(1)]
    [InlineData(100)]
    public void Timer_Interval_SetDesignMode_GetReturnsExpected(int value)
    {
        using SubTimer timer = new();
        Mock<ISite> mockSite = new(MockBehavior.Strict);
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

        // NB: disposing the component with strictly mocked object causes tests to fail
        // Moq.MockException : ISite.Container invocation failed with mock behavior Strict.
        // All invocations on the mock must have a corresponding setup.
        timer.Site = null;
    }

    [WinFormsTheory]
    [InlineData(1)]
    [InlineData(100)]
    public void Timer_Interval_SetDesignModeAfterEnabling_GetReturnsExpected(int value)
    {
        using SubTimer timer = new();
        Mock<ISite> mockSite = new(MockBehavior.Strict);
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

        // NB: disposing the component with strictly mocked object causes tests to fail
        // Moq.MockException : ISite.Container invocation failed with mock behavior Strict.
        // All invocations on the mock must have a corresponding setup.
        timer.Site = null;
    }

    [WinFormsTheory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Timer_Interval_SetInvalid_ThrowsArgumentOutOfRangeException(int value)
    {
        using Timer timer = new();
        Assert.Throws<ArgumentOutOfRangeException>("value", () => timer.Interval = value);
    }

    [WinFormsTheory]
    [StringWithNullData]
    public void Timer_Tag_Set_GetReturnsExpected(object value)
    {
        using Timer timer = new()
        {
            Tag = value
        };
        Assert.Same(value, timer.Tag);

        // Set same.
        timer.Tag = value;
        Assert.Same(value, timer.Tag);
    }

    [WinFormsTheory]
    [StringWithNullData]
    public void Timer_Tag_SetDesignMode_GetReturnsExpected(object value)
    {
        using SubTimer timer = new();
        Mock<ISite> mockSite = new(MockBehavior.Strict);
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

        // NB: disposing the component with strictly mocked object causes tests to fail
        // Moq.MockException : ISite.Container invocation failed with mock behavior Strict.
        // All invocations on the mock must have a corresponding setup.
        timer.Site = null;
    }

    [WinFormsTheory]
    [BoolData]
    public void Timer_Start_Stop_Success(bool designMode)
    {
        using SubTimer timer = new();
        Mock<ISite> mockSite = new(MockBehavior.Strict);
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

        // NB: disposing the component with strictly mocked object causes tests to fail
        // Moq.MockException : ISite.Container invocation failed with mock behavior Strict.
        // All invocations on the mock must have a corresponding setup.
        timer.Site = null;
    }

    [WinFormsTheory]
    [BoolData]
    public void Timer_Start_MultipleTimes_Success(bool designMode)
    {
        using SubTimer timer = new();
        Mock<ISite> mockSite = new(MockBehavior.Strict);
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

        // NB: disposing the component with strictly mocked object causes tests to fail
        // Moq.MockException : ISite.Container invocation failed with mock behavior Strict.
        // All invocations on the mock must have a corresponding setup.
        timer.Site = null;
    }

    [WinFormsTheory]
    [BoolData]
    public void Timer_Stop_Restart_Success(bool designMode)
    {
        using SubTimer timer = new();
        Mock<ISite> mockSite = new(MockBehavior.Strict);
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

        // NB: disposing the component with strictly mocked object causes tests to fail
        // Moq.MockException : ISite.Container invocation failed with mock behavior Strict.
        // All invocations on the mock must have a corresponding setup.
        timer.Site = null;
    }

    [WinFormsTheory]
    [BoolData]
    public void Timer_Stop_MultipleTimes_Success(bool designMode)
    {
        using SubTimer timer = new();
        Mock<ISite> mockSite = new(MockBehavior.Strict);
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

        // NB: disposing the component with strictly mocked object causes tests to fail
        // Moq.MockException : ISite.Container invocation failed with mock behavior Strict.
        // All invocations on the mock must have a corresponding setup.
        timer.Site = null;
    }

    [WinFormsFact]
    public void Timer_OnTick_Invoke_CallsTick()
    {
        using SubTimer timer = new();
        EventArgs eventArgs = new();
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

    [WinFormsFact]
    public void Timer_Dispose_NotStarted_Success()
    {
        using Timer timer = new();
        timer.Dispose();
        Assert.False(timer.Enabled);

        // Call again.
        timer.Dispose();
        Assert.False(timer.Enabled);
    }

    [WinFormsFact]
    public void Timer_Dispose_Started_Success()
    {
        using Timer timer = new();
        timer.Start();

        timer.Dispose();
        Assert.False(timer.Enabled);

        // Call again.
        timer.Dispose();
        Assert.False(timer.Enabled);
    }

    [WinFormsFact]
    public void Timer_Dispose_Stopped_Success()
    {
        using Timer timer = new();
        timer.Start();
        timer.Stop();

        timer.Dispose();
        Assert.False(timer.Enabled);

        // Call again.
        timer.Dispose();
        Assert.False(timer.Enabled);
    }

    [WinFormsFact]
    public void Timer_ToString_Invoke_ReturnsExpected()
    {
        using Timer timer = new();
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
