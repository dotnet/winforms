// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.InteropServices;
using Windows.Win32.System.Com;
using Windows.Win32.System.Ole;

namespace System.Windows.Forms.Primitives.Tests.Windows.Win32;

/// <summary>
///  Verifies native reference ownership separately from managed object identity.
/// </summary>
public unsafe class ComHelpersTests
{
    [Theory]
    [InlineData(false, false)]
    [InlineData(false, true)]
    [InlineData(true, false)]
    [InlineData(true, true)]
    public void GetObjectForIUnknown_BorrowsInput(bool typedPointer, bool useScope)
    {
        GlobalInterfaceTableTests.MyStream source = new();
        using var stream = ComHelpers.GetComScope<IStream>(source);
        using var observer = stream.Query<IUnknown>();
        uint before = GetReferenceCount(observer);

        object actual = (typedPointer, useScope) switch
        {
            (true, true) => ComHelpers.GetObjectForIUnknown(stream),
            (true, false) => ComHelpers.GetObjectForIUnknown(stream.Value),
            (false, true) => ComHelpers.GetObjectForIUnknown(observer),
            (false, false) => ComHelpers.GetObjectForIUnknown(observer.Value)
        };

        Assert.Same(source, actual);
        Assert.Equal(before, GetReferenceCount(observer));
    }

    [Theory]
    [InlineData(false, false)]
    [InlineData(false, true)]
    [InlineData(true, false)]
    [InlineData(true, true)]
    public void TryGetObjectForIUnknown_BorrowsInput(bool typedPointer, bool useScope)
    {
        GlobalInterfaceTableTests.MyStream source = new();
        using var stream = ComHelpers.GetComScope<IStream>(source);
        using var observer = stream.Query<IUnknown>();
        uint before = GetReferenceCount(observer);

        GlobalInterfaceTableTests.MyStream? actual;
        bool success = (typedPointer, useScope) switch
        {
            (true, true) => ComHelpers.TryGetObjectForIUnknown(stream, out actual),
            (true, false) => ComHelpers.TryGetObjectForIUnknown(stream.Value, out actual),
            (false, true) => ComHelpers.TryGetObjectForIUnknown(observer, out actual),
            (false, false) => ComHelpers.TryGetObjectForIUnknown(observer.Value, out actual)
        };

        Assert.True(success);
        Assert.Same(source, actual);
        Assert.Equal(before, GetReferenceCount(observer));
    }

    [Theory]
    [InlineData(false, false)]
    [InlineData(false, true)]
    [InlineData(true, false)]
    [InlineData(true, true)]
    public void TryGetObjectForIUnknown_FailedCast_BorrowsInput(bool typedPointer, bool useScope)
    {
        GlobalInterfaceTableTests.MyStream source = new();
        using var stream = ComHelpers.GetComScope<IStream>(source);
        using var observer = stream.Query<IUnknown>();
        uint before = GetReferenceCount(observer);

        string? actual;
        bool success = (typedPointer, useScope) switch
        {
            (true, true) => ComHelpers.TryGetObjectForIUnknown(stream, out actual),
            (true, false) => ComHelpers.TryGetObjectForIUnknown(stream.Value, out actual),
            (false, true) => ComHelpers.TryGetObjectForIUnknown(observer, out actual),
            (false, false) => ComHelpers.TryGetObjectForIUnknown(observer.Value, out actual)
        };

        Assert.False(success);
        Assert.Null(actual);
        Assert.Equal(before, GetReferenceCount(observer));
    }

    [Theory]
    [InlineData(false, false)]
    [InlineData(false, true)]
    [InlineData(true, false)]
    [InlineData(true, true)]
    public void TryGetObjectForIUnknown_ReleasesInputOnlyWhenTakingOwnership(bool takeOwnership, bool failedCast)
    {
        GlobalInterfaceTableTests.MyStream source = new();
        using var observer = ComHelpers.GetComScope<IUnknown>(source);
        IUnknown* input = observer.Value;
        input->AddRef();
        uint before = GetReferenceCount(observer);

        try
        {
            if (failedCast)
            {
                Assert.False(ComHelpers.TryGetObjectForIUnknown(input, takeOwnership, out string? actual));
                Assert.Null(actual);
            }
            else
            {
                Assert.True(ComHelpers.TryGetObjectForIUnknown(input, takeOwnership, out GlobalInterfaceTableTests.MyStream? actual));
                Assert.Same(source, actual);
            }

            Assert.Equal(takeOwnership ? before - 1 : before, GetReferenceCount(observer));
        }
        finally
        {
            if (!takeOwnership)
            {
                input->Release();
            }
        }
    }

    [StaTheory]
    [InlineData(false)]
    [InlineData(true)]
    public void GetObjectForIUnknown_Ownership_NativeRcw(bool useTypedHelper)
    {
        using ComScope<IUnknown> observer = new(null);
        uint beforeRetrieval;
        uint[] afterRetrieval = new uint[3];
        uint afterCleanup;
        {
            using ComScope<IFont> created = new(null);
            string fontName = "Arial";
            FONTDESC description = new()
            {
                cbSizeofstruct = (uint)sizeof(FONTDESC),
                cySize = (CY)10f,
                sWeight = 400,
                sCharset = 1
            };

            fixed (char* name = fontName)
            {
                description.lpstrName = name;
                Guid interfaceId = IID.GetRef<IFont>();
                PInvoke.OleCreateFontIndirect(&description, &interfaceId, created).ThrowOnFailure();
            }

            object expected = Marshal.GetObjectForIUnknown((nint)created.Value);
            try
            {
                Assert.True(Marshal.IsComObject(expected));
                IFont.Interface nativeFont = (IFont.Interface)expected;
                Assert.NotEqual(IntPtr.Zero, nativeFont.hFont);

                using (ComScope<IUnknown> warmUnknown = created.Query<IUnknown>())
                {
                    Assert.Same(expected, ComHelpers.GetObjectForIUnknown(warmUnknown.Value));
                }

                created.Value->QueryInterface(IID.Get<IUnknown>(), observer).ThrowOnFailure();
                beforeRetrieval = GetReferenceCount(observer.Value);

                for (int retrievalIndex = 0; retrievalIndex < afterRetrieval.Length; retrievalIndex++)
                {
                    object actual = useTypedHelper
                        ? ComHelpers.GetObjectForIUnknown(created.Value)
                        : ComHelpers.GetObjectForIUnknown(observer.Value);

                    Assert.Same(expected, actual);
                    Assert.NotEqual(IntPtr.Zero, ((IFont.Interface)actual).hFont);
                    afterRetrieval[retrievalIndex] = GetReferenceCount(observer.Value);
                }
            }
            finally
            {
                Marshal.FinalReleaseComObject(expected);
            }
        }

        afterCleanup = GetReferenceCount(observer.Value);
        uint[] expectedCounts = [beforeRetrieval, beforeRetrieval, beforeRetrieval, 1];
        uint[] actualCounts = [.. afterRetrieval, afterCleanup];
        Assert.Equal(expectedCounts, actualCounts);
    }

    private static uint GetReferenceCount(IUnknown* unknown)
    {
        unknown->AddRef();
        return unknown->Release();
    }
}
