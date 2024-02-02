// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Reflection;

namespace System.Windows.Forms.Tests;

public class MessageBoxTests
{
    [WinFormsTheory]
    [InvalidEnumData<MessageBoxButtons>]
    public void MessageBox_MessageBoxButtons_ThrowsInvalidEnumArgumentException(MessageBoxButtons value)
    {
        Assert.Throws<InvalidEnumArgumentException>(
            "buttons",
            () => GetMessageBoxStyle(
                null,
                value,
                MessageBoxIcon.None,
                MessageBoxDefaultButton.Button1,
                MessageBoxOptions.DefaultDesktopOnly,
                false));
    }

    public static IEnumerable<object[]> MessageBoxButtons_Set_TestData()
    {
        foreach (MessageBoxButtons value in Enum.GetValues(typeof(MessageBoxButtons)))
        {
            yield return new object[] { value };
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(MessageBoxButtons_Set_TestData))]
    public void MessageBox_MessageBoxButtons_Valid(MessageBoxButtons value)
    {
        MESSAGEBOX_STYLE style = GetMessageBoxStyle(
            null,
            value,
            MessageBoxIcon.None,
            MessageBoxDefaultButton.Button1,
            MessageBoxOptions.DefaultDesktopOnly,
            false);

        Assert.Equal(
            style,
            (MESSAGEBOX_STYLE)value
                | (MESSAGEBOX_STYLE)MessageBoxIcon.None
                | (MESSAGEBOX_STYLE)MessageBoxDefaultButton.Button1
                | (MESSAGEBOX_STYLE)MessageBoxOptions.DefaultDesktopOnly);
    }

    [WinFormsTheory]
    [InvalidEnumData<MessageBoxIcon>]
    public void MessageBox_MessageBoxIcon_ThrowsInvalidEnumArgumentException(MessageBoxIcon value)
    {
        Assert.Throws<InvalidEnumArgumentException>(
            "icon",
            () => GetMessageBoxStyle(
                null,
                MessageBoxButtons.OK,
                value,
                MessageBoxDefaultButton.Button1,
                MessageBoxOptions.DefaultDesktopOnly,
                false));
    }

    public static IEnumerable<object[]> MessageBoxIcon_Set_TestData()
    {
        foreach (MessageBoxIcon value in Enum.GetValues(typeof(MessageBoxIcon)))
        {
            yield return new object[] { value };
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(MessageBoxIcon_Set_TestData))]
    public void MessageBox_MessageBoxIcon_Valid(MessageBoxIcon value)
    {
        MESSAGEBOX_STYLE style = GetMessageBoxStyle(
            null,
            MessageBoxButtons.OK,
            value,
            MessageBoxDefaultButton.Button1,
            MessageBoxOptions.DefaultDesktopOnly,
            false);
        Assert.Equal(
            style,
            (MESSAGEBOX_STYLE)MessageBoxButtons.OK
                | (MESSAGEBOX_STYLE)value
                | (MESSAGEBOX_STYLE)MessageBoxDefaultButton.Button1
                | (MESSAGEBOX_STYLE)MessageBoxOptions.DefaultDesktopOnly);
    }

    [WinFormsTheory]
    [InvalidEnumData<MessageBoxDefaultButton>]
    public void MessageBox_MessageBoxDefaultButton_ThrowsInvalidEnumArgumentException(MessageBoxDefaultButton value)
    {
        Assert.Throws<InvalidEnumArgumentException>(
            "defaultButton",
            () => GetMessageBoxStyle(
                null,
                MessageBoxButtons.OK,
                MessageBoxIcon.None,
                value,
                MessageBoxOptions.DefaultDesktopOnly,
                false));
    }

    public static IEnumerable<object[]> MessageBoxDefaultButton_Set_TestData()
    {
        foreach (MessageBoxDefaultButton value in Enum.GetValues(typeof(MessageBoxDefaultButton)))
        {
            yield return new object[] { value };
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(MessageBoxDefaultButton_Set_TestData))]
    public void MessageBox_MessageBoxDefaultButton_Valid(MessageBoxDefaultButton value)
    {
        MESSAGEBOX_STYLE style = GetMessageBoxStyle(
            null,
            MessageBoxButtons.OK,
            MessageBoxIcon.None,
            value,
            MessageBoxOptions.DefaultDesktopOnly,
            false);
        Assert.Equal(
            style,
            (MESSAGEBOX_STYLE)MessageBoxButtons.OK
                | (MESSAGEBOX_STYLE)MessageBoxIcon.None
                | (MESSAGEBOX_STYLE)value
                | (MESSAGEBOX_STYLE)MessageBoxOptions.DefaultDesktopOnly);
    }

    private static MESSAGEBOX_STYLE GetMessageBoxStyle(
        IWin32Window owner,
        MessageBoxButtons buttons,
        MessageBoxIcon icon,
        MessageBoxDefaultButton defaultButton,
        MessageBoxOptions options,
        bool showHelp)
    {
        try
        {
            return typeof(MessageBox).TestAccessor().Dynamic.GetMessageBoxStyle(owner, buttons, icon, defaultButton, options, showHelp);
        }
        catch (TargetInvocationException ex)
        {
            throw ex.InnerException;
        }
    }
}
