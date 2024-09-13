// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms.Tests;

public class ApplicationContextTests
{
    [WinFormsFact]
    public void Ctor_Default()
    {
        using ApplicationContext context = new();

        Assert.Null(context.MainForm);
        Assert.Null(context.Tag);
    }

    public static IEnumerable<object[]> Ctor_Form_TestData()
    {
        yield return new object[] { null };
        yield return new object[] { new Form() };
    }

    [WinFormsTheory]
    [MemberData(nameof(Ctor_Form_TestData))]
    public void Ctor_Form(Form mainForm)
    {
        using ApplicationContext context = new(mainForm);

        Assert.Same(mainForm, context.MainForm);
        Assert.Null(context.Tag);
    }

    [WinFormsTheory]
    [MemberData(nameof(Ctor_Form_TestData))]
    public void MainForm_Set_GetReturnsExpected(Form value)
    {
        using ApplicationContext context = new()
        {
            MainForm = value
        };
        Assert.Equal(value, context.MainForm);

        // Set same
        context.MainForm = value;
        Assert.Equal(value, context.MainForm);
    }

    [WinFormsTheory]
    [MemberData(nameof(Ctor_Form_TestData))]
    public void MainForm_SetWithNonNullOldValue_GetReturnsExpected(Form value)
    {
        using ApplicationContext context = new()
        {
            MainForm = new Form()
        };

        context.MainForm = value;
        Assert.Equal(value, context.MainForm);

        // Set same
        context.MainForm = value;
        Assert.Equal(value, context.MainForm);
    }

    [WinFormsFact]
    public void MainForm_DestroyHandleWithThreadExit_CallsHandler()
    {
        using SubForm mainForm = new();
        using ApplicationContext context = new(mainForm);
        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            Assert.Same(context, sender);
            Assert.Same(EventArgs.Empty, e);
            callCount++;
        };
        context.ThreadExit += handler;

        Assert.NotEqual(IntPtr.Zero, mainForm.Handle);
        Assert.Equal(0, callCount);

        mainForm.OnHandleDestroyed(EventArgs.Empty);
        Assert.Equal(1, callCount);

        // Call again.
        mainForm.OnHandleDestroyed(EventArgs.Empty);
        Assert.Equal(1, callCount);
    }

    [WinFormsFact]
    public void MainForm_DestroyOldHandleWithThreadExit_CallsHandler()
    {
        using SubForm mainForm = new();
        using SubForm newMainForm = new();
        using ApplicationContext context = new(mainForm)
        {
            MainForm = newMainForm
        };
        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            Assert.Same(context, sender);
            Assert.Same(EventArgs.Empty, e);
            callCount++;
        };
        context.ThreadExit += handler;

        Assert.NotEqual(IntPtr.Zero, mainForm.Handle);
        Assert.Equal(0, callCount);

        Assert.NotEqual(IntPtr.Zero, newMainForm.Handle);
        Assert.Equal(0, callCount);

        mainForm.OnHandleDestroyed(EventArgs.Empty);
        Assert.Equal(0, callCount);

        newMainForm.OnHandleDestroyed(EventArgs.Empty);
        Assert.Equal(1, callCount);

        // Call again.
        mainForm.OnHandleDestroyed(EventArgs.Empty);
        Assert.Equal(1, callCount);

        newMainForm.OnHandleDestroyed(EventArgs.Empty);
        Assert.Equal(1, callCount);
    }

    [WinFormsFact]
    public void MainForm_RecreateHandleWithThreadExit_DoesNotCallHandler()
    {
        using SubForm mainForm = new();
        using ApplicationContext context = new(mainForm);
        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            Assert.Same(context, sender);
            Assert.Same(EventArgs.Empty, e);
            callCount++;
        };
        context.ThreadExit += handler;

        Assert.NotEqual(IntPtr.Zero, mainForm.Handle);
        Assert.Equal(0, callCount);

        mainForm.RecreateHandle();
        Assert.Equal(0, callCount);

        // Call again.
        mainForm.RecreateHandle();
        Assert.Equal(0, callCount);
    }

    [WinFormsTheory]
    [StringWithNullData]
    public void Tag_Set_GetReturnsExpected(string value)
    {
        using ApplicationContext context = new()
        {
            Tag = value
        };
        Assert.Equal(value, context.Tag);

        // Set same
        context.Tag = value;
        Assert.Equal(value, context.Tag);
    }

    [WinFormsFact]
    public void Dispose_InvokeWithForm_Success()
    {
        using Form mainForm = new();
        using ApplicationContext context = new(mainForm);
        context.Dispose();
        Assert.Null(context.MainForm);
        Assert.True(mainForm.IsDisposed);

        context.Dispose();
        Assert.Null(context.MainForm);
        Assert.True(mainForm.IsDisposed);
    }

    [WinFormsFact]
    public void Dispose_InvokeWithDisposedForm_Success()
    {
        using Form mainForm = new();
        using ApplicationContext context = new(mainForm);
        mainForm.Dispose();
        Assert.True(mainForm.IsDisposed);

        context.Dispose();
        Assert.Null(context.MainForm);
        Assert.True(mainForm.IsDisposed);

        context.Dispose();
        Assert.Null(context.MainForm);
        Assert.True(mainForm.IsDisposed);
    }

    [WinFormsFact]
    public void Dispose_InvokeWithoutForm_Success()
    {
        using ApplicationContext context = new();
        context.Dispose();
        Assert.Null(context.MainForm);

        context.Dispose();
        Assert.Null(context.MainForm);
    }

    private class TestApplicationContext : ApplicationContext
    {
        public TestApplicationContext() : base()
        {
        }

        public int DisposeCallCount { get; private set; }

        public int ExitThreadCoreCount { get; private set; }

        protected override void Dispose(bool disposing)
        {
            DisposeCallCount++;
            base.Dispose(disposing);
        }

        protected override void ExitThreadCore()
        {
            ExitThreadCoreCount++;
            base.ExitThreadCore();
        }
    }

    [WinFormsFact]
    public void Dispose_Invoke_CallsDisposeDisposing()
    {
        TestApplicationContext context = new();
        context.Dispose();
        context.DisposeCallCount.Should().Be(1);

        // Call again.
        context.Dispose();
        context.DisposeCallCount.Should().Be(2);
    }

    [WinFormsFact]
    public void Dispose_InvokeDisposingWithForm_Success()
    {
        using Form mainForm = new();
        using SubApplicationContext context = new(mainForm);
        context.Dispose(true);
        Assert.Null(context.MainForm);
        Assert.True(mainForm.IsDisposed);

        context.Dispose(true);
        Assert.Null(context.MainForm);
        Assert.True(mainForm.IsDisposed);
    }

    [WinFormsFact]
    public void Dispose_InvokeNotDisposingWithForm_Nop()
    {
        using Form mainForm = new();
        using SubApplicationContext context = new(mainForm);
        context.Dispose(false);
        Assert.Same(mainForm, context.MainForm);
        Assert.False(mainForm.IsDisposed);

        context.Dispose(false);
        Assert.Same(mainForm, context.MainForm);
        Assert.False(mainForm.IsDisposed);
    }

    [WinFormsTheory]
    [BoolData]
    public void Dispose_InvokeDisposingNoForm_Nop(bool disposing)
    {
        using SubApplicationContext context = new();
        context.Dispose(disposing);
        Assert.Null(context.MainForm);

        context.Dispose(disposing);
        Assert.Null(context.MainForm);
    }

    [WinFormsFact]
    public void ExitThread_InvokeWithThreadExit_CallsHandler()
    {
        using ApplicationContext context = new();
        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            Assert.Same(context, sender);
            Assert.Same(EventArgs.Empty, e);
            callCount++;
        };
        context.ThreadExit += handler;

        // Call with handler.
        context.ExitThread();
        Assert.Equal(1, callCount);

        // Call again.
        context.ExitThread();
        Assert.Equal(2, callCount);

        // Remove handler.
        context.ThreadExit -= handler;
        context.ExitThread();
        Assert.Equal(2, callCount);
    }

    [WinFormsFact]
    public void ExitThread_Invoke_CallsExitThreadCore()
    {
        TestApplicationContext context = new();
        context.ExitThread();
        context.ExitThreadCoreCount.Should().Be(1);

        // Call again.
        context.ExitThread();
        context.ExitThreadCoreCount.Should().Be(2);
    }

    [WinFormsFact]
    public void ExitThreadCore_InvokeWithThreadExit_CallsHandler()
    {
        using SubApplicationContext context = new();
        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            Assert.Same(context, sender);
            Assert.Same(EventArgs.Empty, e);
            callCount++;
        };
        context.ThreadExit += handler;

        // Call with handler.
        context.ExitThreadCore();
        Assert.Equal(1, callCount);

        // Call again.
        context.ExitThreadCore();
        Assert.Equal(2, callCount);

        // Remove handler.
        context.ThreadExit -= handler;
        context.ExitThreadCore();
        Assert.Equal(2, callCount);
    }

    public static IEnumerable<object[]> OnMainFormClosed_TestData()
    {
        yield return new object[] { null, null };
        yield return new object[] { new(), new EventArgs() };
    }

    [WinFormsTheory]
    [MemberData(nameof(OnMainFormClosed_TestData))]
    public void OnMainFormClosed_InvokeWithThreadExit_CallsHandler(object sender, EventArgs e)
    {
        using SubApplicationContext context = new();
        int callCount = 0;
        EventHandler handler = (actualSender, actualE) =>
        {
            Assert.Same(context, actualSender);
            Assert.Same(EventArgs.Empty, actualE);
            callCount++;
        };
        context.ThreadExit += handler;

        // Call with handler.
        context.OnMainFormClosed(sender, e);
        Assert.Equal(1, callCount);

        // Call again.
        context.OnMainFormClosed(sender, e);
        Assert.Equal(2, callCount);

        // Remove handler.
        context.ThreadExit -= handler;
        context.OnMainFormClosed(sender, e);
        Assert.Equal(2, callCount);
    }

    [WinFormsFact]
    public void ApplicationContext_Subclasses_SuppressFinalizeCall()
    {
        foreach (var type in typeof(ApplicationContext).Assembly.GetTypes().
            Where(type => type == typeof(ApplicationContext) || type.IsSubclassOf(typeof(ApplicationContext))))
        {
            Assert.True(type == typeof(ApplicationContext) || type == Application.s_typeOfModalApplicationContext,
                $"Type {type} is not one of [{typeof(ApplicationContext)}, {Application.s_typeOfModalApplicationContext}]. " +
                $"Consider adding it here and to the ApplicationContext(Form? mainForm) constructor. Or add exclusion to this test (if a new class really needs a finalizer).");
        }
    }

    private class SubApplicationContext : ApplicationContext
    {
        public SubApplicationContext() : base()
        {
        }

        public SubApplicationContext(Form mainForm) : base(mainForm)
        {
        }

        public new void Dispose(bool disposing) => base.Dispose(disposing);

        public new void ExitThreadCore() => base.ExitThreadCore();

        public new void OnMainFormClosed(object sender, EventArgs e) => base.OnMainFormClosed(sender, e);
    }

    private class SubForm : Form
    {
        public new void OnHandleDestroyed(EventArgs e) => base.OnHandleDestroyed(e);

        public new void RecreateHandle() => base.RecreateHandle();
    }
}
