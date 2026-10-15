// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable disable

using Moq;

namespace System.Windows.Forms.Tests;

public class ThreadContextTests
{
    private delegate bool MessageCallback(ref Message m);

    // WM_USER is 0x400, just need to be above that
    private const uint TestMessageId1 = 0x441;
    private const uint TestMessageId2 = 0x442;
    private const uint TestMessageId3 = 0x443;

    [StaFact]
    public void ThreadContext_EmptyProcessFiltersWorks()
    {
        // Test that no filters at all does not throw, and that returns false from translation
        Application.ThreadContext threadContext = new Application.LightThreadContext();
        MSG msg = default;
        Assert.False(threadContext.PreTranslateMessage(ref msg));
    }

    [StaFact]
    public void ThreadContext_WrongProcessFiltersPassesThrough()
    {
        // Test that a filter for the wrong ID returns false, but does get called
        Application.ThreadContext threadContext = new Application.LightThreadContext();

        MessageId filterId = TestMessageId2;
        Mock<IMessageFilter> mockContext = new(MockBehavior.Strict);
        mockContext.Setup(c => c.PreFilterMessage(ref It.Ref<Message>.IsAny))
                   .Returns((MessageCallback)((ref Message m) => m.Msg == (int)filterId));

        threadContext.AddMessageFilter(mockContext.Object);
        MSG msg = new()
        {
            message = TestMessageId1
        };
        Assert.False(threadContext.PreTranslateMessage(ref msg));
        mockContext.Verify(c => c.PreFilterMessage(ref It.Ref<Message>.IsAny), Times.Exactly(1));
    }

    [StaFact]
    public void ThreadContext_CorrectProcessFiltersProcesses()
    {
        // Test that a filter with the correct ID returns true
        Application.ThreadContext threadContext = new Application.LightThreadContext();

        MessageId filterId = TestMessageId2;
        Mock<IMessageFilter> mockContext = new(MockBehavior.Strict);
        mockContext.Setup(c => c.PreFilterMessage(ref It.Ref<Message>.IsAny))
                   .Returns((MessageCallback)((ref Message m) => m.Msg == (int)filterId));

        threadContext.AddMessageFilter(mockContext.Object);
        MSG msg = new()
        {
            message = (uint)filterId
        };
        Assert.True(threadContext.PreTranslateMessage(ref msg));
        mockContext.Verify(c => c.PreFilterMessage(ref It.Ref<Message>.IsAny), Times.Exactly(1));
    }

    [StaFact]
    public void ThreadContext_MultipleProcessFiltersProcesses()
    {
        // Test that multiple filters work
        Application.ThreadContext threadContext = new Application.LightThreadContext();

        MessageId filterId2 = TestMessageId2;
        Mock<IMessageFilter> mockContext2 = new(MockBehavior.Strict);
        mockContext2.Setup(c => c.PreFilterMessage(ref It.Ref<Message>.IsAny))
                   .Returns((MessageCallback)((ref Message m) => m.Msg == (int)filterId2));
        threadContext.AddMessageFilter(mockContext2.Object);

        MessageId filterId3 = TestMessageId3;
        Mock<IMessageFilter> mockContext3 = new(MockBehavior.Strict);
        mockContext3.Setup(c => c.PreFilterMessage(ref It.Ref<Message>.IsAny))
                   .Returns((MessageCallback)((ref Message m) => m.Msg == (int)filterId3));
        threadContext.AddMessageFilter(mockContext3.Object);

        MSG msg = new()
        {
            message = TestMessageId1
        };
        Assert.False(threadContext.PreTranslateMessage(ref msg));

        mockContext2.Verify(c => c.PreFilterMessage(ref It.Ref<Message>.IsAny), Times.Exactly(1));
        mockContext3.Verify(c => c.PreFilterMessage(ref It.Ref<Message>.IsAny), Times.Exactly(1));

        msg = new MSG
        {
            message = TestMessageId2
        };
        Assert.True(threadContext.PreTranslateMessage(ref msg));

        mockContext2.Verify(c => c.PreFilterMessage(ref It.Ref<Message>.IsAny), Times.Exactly(2));
        mockContext3.Verify(c => c.PreFilterMessage(ref It.Ref<Message>.IsAny), Times.Exactly(1));

        msg = new MSG
        {
            message = TestMessageId3
        };
        Assert.True(threadContext.PreTranslateMessage(ref msg));

        mockContext2.Verify(c => c.PreFilterMessage(ref It.Ref<Message>.IsAny), Times.Exactly(3));
        mockContext3.Verify(c => c.PreFilterMessage(ref It.Ref<Message>.IsAny), Times.Exactly(2));
    }

    [WinFormsFact]
    public void ThreadContext_CurrentForm_ReflectsChangedMainForm()
    {
        RunOnStaThread(() =>
        {
            using Form form1 = new()
            {
                Text = "Form1"
            };

            using Form form2 = new()
            {
                Text = "Form2"
            };

            using ApplicationContext context = new(form1);

            Form currentFormBeforeChange = null;
            Form currentFormAfterChange = null;

            form1.Shown += (_, _) =>
            {
                currentFormBeforeChange = GetThreadContextCurrentForm();

                context.MainForm = form2;

                currentFormAfterChange = GetThreadContextCurrentForm();

                context.ExitThread();
            };

            Application.Run(context);

            Assert.Same(form1, currentFormBeforeChange);
            Assert.Same(form2, currentFormAfterChange);
        });
    }

    [WinFormsFact]
    public void ThreadContext_CurrentForm_ReflectsNullMainForm()
    {
        RunOnStaThread(() =>
        {
            using Form form = new()
            {
                Text = "Form1"
            };

            using ApplicationContext context = new(form);

            Form currentFormBeforeChange = null;
            Form currentFormAfterChange = null;

            form.Shown += (_, _) =>
            {
                currentFormBeforeChange = GetThreadContextCurrentForm();

                context.MainForm = null;

                currentFormAfterChange = GetThreadContextCurrentForm();

                context.ExitThread();
            };

            Application.Run(context);

            Assert.Same(form, currentFormBeforeChange);
            Assert.Null(currentFormAfterChange);
        });
    }

    [WinFormsFact]
    public void ThreadContext_CurrentForm_ReflectsMultipleMainFormChanges()
    {
        RunOnStaThread(() =>
        {
            using Form form1 = new()
            {
                Text = "Form1"
            };

            using Form form2 = new()
            {
                Text = "Form2"
            };

            using Form form3 = new()
            {
                Text = "Form3"
            };

            using ApplicationContext context = new(form1);

            Form currentFormAfterFirstChange = null;
            Form currentFormAfterSecondChange = null;

            form1.Shown += (_, _) =>
            {
                context.MainForm = form2;
                currentFormAfterFirstChange = GetThreadContextCurrentForm();

                context.MainForm = form3;
                currentFormAfterSecondChange = GetThreadContextCurrentForm();

                context.ExitThread();
            };

            Application.Run(context);

            Assert.Same(form2, currentFormAfterFirstChange);
            Assert.Same(form3, currentFormAfterSecondChange);
        });
    }

    private static void RunOnStaThread(Action action)
    {
        Exception exception = null;

        Thread thread = new(() =>
        {
            try
            {
                action();
            }
            catch (Exception ex)
            {
                exception = ex;
            }
        });

        thread.SetApartmentState(ApartmentState.STA);
        thread.IsBackground = true;
        thread.Start();

        Assert.True(thread.Join(TimeSpan.FromSeconds(30)), "The STA test thread timed out.");

        if (exception is not null)
        {
            throw exception;
        }
    }

    private static Form GetThreadContextCurrentForm()
    {
        Application.ThreadContext threadContext = Application.ThreadContext.FromCurrent();

        PropertyInfo currentFormProperty = typeof(Application.ThreadContext).GetProperty(
            "CurrentForm",
            BindingFlags.Instance | BindingFlags.NonPublic);

        return (Form)currentFormProperty.GetValue(threadContext);
    }
}
