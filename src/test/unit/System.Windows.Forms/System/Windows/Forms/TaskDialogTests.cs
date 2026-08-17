// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable disable

using Microsoft.DotNet.RemoteExecutor;
using System.Reflection;

namespace System.Windows.Forms.Tests;

public class TaskDialogTests
{
    [WinFormsFact]
    public void TaskDialog_ShowDialog_SetProperty_SameThread_Success()
    {
        // Run this from another thread as we call Application.EnableVisualStyles.
        using RemoteInvokeHandle invokerHandle = RemoteExecutor.Invoke(() =>
        {
            Application.EnableVisualStyles();
            Control.CheckForIllegalCrossThreadCalls = true;

            TaskDialogPage page = new();
            page.Created += (_, __) =>
            {
                // Set the property in the same thread.
                page.Text = "X";
                page.BoundDialog.Close();
            };

            TaskDialog.ShowDialog(page);
        });

        // verify the remote process succeeded
        Assert.Equal(RemoteExecutor.SuccessExitCode, invokerHandle.ExitCode);
    }

    [WinFormsFact]
    public void TaskDialog_ShowDialog_SetProperty_DifferentThread_ThrowsInvalidOperationException()
    {
        // Run this from another thread as we call Application.EnableVisualStyles.
        using RemoteInvokeHandle invokerHandle = RemoteExecutor.Invoke(() =>
        {
            Application.EnableVisualStyles();
            Control.CheckForIllegalCrossThreadCalls = true;

            TaskDialogPage page = new();
            page.Created += (_, __) =>
            {
                // Set the property in a different thread.
                var separateTask = Task.Run(() => page.Text = "X");
                Assert.Throws<InvalidOperationException>(separateTask.GetAwaiter().GetResult);

                page.BoundDialog.Close();
            };

            TaskDialog.ShowDialog(page);
        });

        // verify the remote process succeeded
        Assert.Equal(RemoteExecutor.SuccessExitCode, invokerHandle.ExitCode);
    }

    [WinFormsFact]
    public void TaskDialogPage_GetBoundButtonByID_CustomRangeOutOfBounds_ReturnsNull()
    {
        TaskDialogPage page = new();
        PrepareBoundLikeState(page);
        dynamic access = page.TestAccessor.Dynamic;
        access._boundCustomButtons = Array.Empty<TaskDialogButton>();
        access._boundStandardButtonsByID = new Dictionary<int, TaskDialogButton>();

        TaskDialogButton button = page.GetBoundButtonByID(buttonID: 100);

        Assert.Null(button);
    }

    [WinFormsFact]
    public void TaskDialogPage_GetBoundRadioButtonByID_OutOfBounds_ReturnsNull()
    {
        TaskDialogPage page = new();
        PrepareBoundLikeState(page);

        TaskDialogRadioButton radioButton = page.GetBoundRadioButtonByID(buttonID: 1);

        Assert.Null(radioButton);
    }

    private static void PrepareBoundLikeState(TaskDialogPage page)
    {
        ConstructorInfo constructor = typeof(TaskDialog).GetConstructor(
            BindingFlags.Instance | BindingFlags.NonPublic,
            binder: null,
            Type.EmptyTypes,
            modifiers: null);
        Assert.NotNull(constructor);

        TaskDialog dialog = (TaskDialog)constructor.Invoke(null);
        SetPrivateField(page, "<BoundDialog>k__BackingField", dialog);
    }

    private static void SetPrivateField(object instance, string fieldName, object value)
    {
        FieldInfo field = instance.GetType().GetField(
            fieldName,
            BindingFlags.Instance | BindingFlags.NonPublic);
        Assert.NotNull(field);
        field.SetValue(instance, value);
    }
}
