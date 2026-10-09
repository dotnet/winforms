// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable enable

using System.Collections.Specialized;

namespace System.Windows.Forms.Tests;

#pragma warning disable WFDEV005 // Legacy APIs are intentionally exercised for compatibility.

public partial class ClipboardTests
{
    private const string DataObjectSemanticsFormat = "WinForms.ClipboardTests.DataObjectSemantics";

    // Verifies that an unflushed managed data object retains identity and is returned without an interop wrapper.
    [WinFormsFact]
    public void Clipboard_SetDataObject_CopyFalse_ReturnsOriginalManagedIDataObject()
    {
        DataObject original = new(DataObjectSemanticsFormat, 42);

        Clipboard.SetDataObject(original, copy: false);

        IDataObject actual = Assert.IsAssignableFrom<IDataObject>(Clipboard.GetDataObject());
        Assert.Same(original, actual);
        Assert.Equal(42, actual.GetData(DataObjectSemanticsFormat, autoConvert: false));
    }

    // Verifies that a flushed data object is retrieved through fresh proxy-backed wrappers, not by managed identity.
    [WinFormsFact]
    public void Clipboard_SetDataObject_CopyTrue_ReturnsProxyBackedDataObjectNotOriginal()
    {
        DataObject original = new(DataObjectSemanticsFormat, 42);

        Clipboard.SetDataObject(original, copy: true);

        DataObject first = Assert.IsType<DataObject>(Clipboard.GetDataObject());
        DataObject second = Assert.IsType<DataObject>(Clipboard.GetDataObject());
        Assert.NotSame(original, first);
        Assert.NotSame(first, second);
        Assert.Contains(DataObjectSemanticsFormat, first.GetFormats(autoConvert: false));
    }

    // Verifies that an exited delayed-rendering owner produces neutral reads and does not block later replacement.
    [WinFormsFact]
    public void Clipboard_SetDataObject_CopyFalse_AfterOwnerStaExits_ReturnsNeutralReadsAndCanBeReplaced()
    {
        Exception? firstException = RunOnStaThread(() =>
        {
            Clipboard.SetDataObject(new DataObject(DataFormats.UnicodeText, "text"), copy: false);
        });

        Assert.Null(firstException);

        Exception? readException = RunOnStaThread(() =>
        {
            Assert.NotNull(Clipboard.GetDataObject());
            Assert.False(Clipboard.ContainsText());
            Assert.Equal(string.Empty, Clipboard.GetText());
        });

        Assert.Null(readException);

        Exception? secondException = RunOnStaThread(() =>
        {
            DataObject replacement = new(DataObjectSemanticsFormat, 43);
            Clipboard.SetDataObject(replacement, copy: false);

            Assert.Same(replacement, Clipboard.GetDataObject());
            Assert.Equal(43, replacement.GetData(DataObjectSemanticsFormat, autoConvert: false));
            Clipboard.Clear();
        });

        Assert.Null(secondException);
    }

    public static TheoryData<Action> SetDataObjectInvalidArgumentsOnMtaTheoryData => new()
    {
        () => Clipboard.SetDataObject(null!, copy: false, retryTimes: -1, retryDelay: -1),
        () => Clipboard.SetDataObject(new object(), copy: false, retryTimes: -1, retryDelay: -1),
        () => Clipboard.SetDataObject(new object(), copy: false, retryTimes: 0, retryDelay: -1)
    };

    // Verifies the public SetDataObject contract that apartment validation precedes invalid argument checks.
    [Theory] // x-thread
    [MemberData(nameof(SetDataObjectInvalidArgumentsOnMtaTheoryData))]
    public void Clipboard_SetDataObject_InvalidArguments_OnMta_StaRequirementPrecedesArguments(Action action)
    {
        action.Should().Throw<ThreadStateException>();
    }

    public static TheoryData<Action, Type, string?> TypedSetterInvalidArgumentsOnMtaTheoryData => new()
    {
        { () => Clipboard.SetAudio((byte[])null!), typeof(ArgumentNullException), "audioBytes" },
        { () => Clipboard.SetAudio((Stream)null!), typeof(ArgumentNullException), "audioStream" },
        { () => Clipboard.SetData(null!, 42), typeof(ArgumentNullException), "format" },
        { () => Clipboard.SetData(" ", 42), typeof(ArgumentException), "format" },
        { () => Clipboard.SetFileDropList(null!), typeof(ArgumentNullException), "filePaths" },
        { () => Clipboard.SetFileDropList([]), typeof(ArgumentException), null },
        {
            () => Clipboard.SetFileDropList(new StringCollection { null! }),
            typeof(ArgumentException),
            null
        },
        { () => Clipboard.SetImage(null!), typeof(ArgumentNullException), "image" },
    };

    // Verifies that typed setters validate their own arguments before delegating to the STA-only core API.
    [Theory] // x-thread
    [MemberData(nameof(TypedSetterInvalidArgumentsOnMtaTheoryData))]
    public void Clipboard_TypedSetters_InvalidArguments_OnMta_UseLocalValidationOrder(
        Action action,
        Type expectedExceptionType,
        string? expectedParameterName)
    {
        ArgumentException exception = Assert.ThrowsAny<ArgumentException>(action);

        Assert.Equal(expectedExceptionType, exception.GetType());
        Assert.Equal(expectedParameterName, exception.ParamName);
    }

    private static Exception? RunOnStaThread(Action action)
    {
        Exception? exception = null;
        Thread thread = new(() => exception = Record.Exception(action));
        thread.SetApartmentState(ApartmentState.STA);
        thread.Start();
        thread.Join();
        return exception;
    }
}

#pragma warning restore WFDEV005
