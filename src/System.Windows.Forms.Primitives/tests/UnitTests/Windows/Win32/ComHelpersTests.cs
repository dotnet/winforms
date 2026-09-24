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
        using MemoryStream data = new([42]);
        ComManagedStream source = new(data);
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
        Assert.Equal(42, ((ComManagedStream)actual).GetDataStream().ReadByte());
        Assert.Equal(HRESULT.S_OK, stream.Value->Commit(0));
    }

    [Theory]
    [InlineData(false, false)]
    [InlineData(false, true)]
    [InlineData(true, false)]
    [InlineData(true, true)]
    public void TryGetObjectForIUnknown_BorrowsInput(bool typedPointer, bool useScope)
    {
        using MemoryStream data = new([42]);
        ComManagedStream source = new(data);
        using var stream = ComHelpers.GetComScope<IStream>(source);
        using var observer = stream.Query<IUnknown>();
        uint before = GetReferenceCount(observer);

        ComManagedStream? actual;
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
        Assert.Equal(42, actual!.GetDataStream().ReadByte());
        Assert.Equal(HRESULT.S_OK, stream.Value->Commit(0));
    }

    [Theory]
    [InlineData(false, false)]
    [InlineData(false, true)]
    [InlineData(true, false)]
    [InlineData(true, true)]
    public void TryGetObjectForIUnknown_FailedCast_BorrowsInput(bool typedPointer, bool useScope)
    {
        using MemoryStream data = new([42]);
        ComManagedStream source = new(data);
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
        Assert.Equal(42, source.GetDataStream().ReadByte());
        Assert.Equal(HRESULT.S_OK, stream.Value->Commit(0));
    }

    [Theory]
    [InlineData(false, false)]
    [InlineData(false, true)]
    [InlineData(true, false)]
    [InlineData(true, true)]
    public void TryGetObjectForIUnknown_ReleasesInputOnlyWhenTakingOwnership(bool takeOwnership, bool failedCast)
    {
        using MemoryStream data = new([42]);
        ComManagedStream source = new(data);
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
                Assert.True(ComHelpers.TryGetObjectForIUnknown(input, takeOwnership, out ComManagedStream? actual));
                Assert.Same(source, actual);
            }

            Assert.Equal(takeOwnership ? before - 1 : before, GetReferenceCount(observer));
            Assert.Equal(42, source.GetDataStream().ReadByte());
        }
        finally
        {
            if (!takeOwnership)
            {
                input->Release();
            }
        }
    }

    [Theory]
    [InlineData(false, false, false)]
    [InlineData(false, true, false)]
    [InlineData(true, false, false)]
    [InlineData(true, true, false)]
    [InlineData(false, true, true)]
    [InlineData(true, true, true)]
    public void GetObjectForIUnknown_BorrowsRuntimeCcw(bool typedPointer, bool tryGet, bool failedCast)
    {
        using MemoryStream data = new([42]);
        ComManagedStream source = new(data);

        // Bypass our manual ComWrappers CCW to cover the built-in interop unwrapping path.
        using ComScope<IUnknown> observer = new((IUnknown*)Marshal.GetIUnknownForObject(source));
        using var stream = observer.Query<IStream>();
        uint before = GetReferenceCount(observer);

        if (failedCast)
        {
            string? actual;
            bool success = typedPointer
                ? ComHelpers.TryGetObjectForIUnknown(stream.Value, out actual)
                : ComHelpers.TryGetObjectForIUnknown(observer.Value, out actual);
            Assert.False(success);
            Assert.Null(actual);
        }
        else
        {
            object? actual;
            if (tryGet)
            {
                bool success = typedPointer
                    ? ComHelpers.TryGetObjectForIUnknown(stream.Value, out actual)
                    : ComHelpers.TryGetObjectForIUnknown(observer.Value, out actual);
                Assert.True(success);
            }
            else
            {
                actual = typedPointer
                    ? ComHelpers.GetObjectForIUnknown(stream.Value)
                    : ComHelpers.GetObjectForIUnknown(observer.Value);
            }

            Assert.Same(source, actual);
            Assert.Same(data, Assert.IsType<ComManagedStream>(actual).GetDataStream());
        }

        Assert.Equal(before, GetReferenceCount(observer));
        Assert.Equal(42, source.GetDataStream().ReadByte());
        Assert.Equal(HRESULT.S_OK, stream.Value->Commit(0));
    }

    [StaTheory]
    [InlineData(false, false, false)]
    [InlineData(false, true, false)]
    [InlineData(true, false, false)]
    [InlineData(true, true, false)]
    [InlineData(false, true, true)]
    [InlineData(true, true, true)]
    public void GetObjectForIUnknown_Ownership_NativeRcw(bool useTypedHelper, bool tryGet, bool failedCast)
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
                var nativeFont = (IFont.Interface)expected;
                Assert.NotEqual(IntPtr.Zero, nativeFont.hFont);

                using (ComScope<IUnknown> warmUnknown = created.Query<IUnknown>())
                {
                    Assert.Same(expected, ComHelpers.GetObjectForIUnknown(warmUnknown.Value));
                }

                created.Value->QueryInterface(IID.Get<IUnknown>(), observer).ThrowOnFailure();
                beforeRetrieval = GetReferenceCount(observer.Value);

                for (int retrievalIndex = 0; retrievalIndex < afterRetrieval.Length; retrievalIndex++)
                {
                    if (failedCast)
                    {
                        string? actual;
                        bool success = useTypedHelper
                            ? ComHelpers.TryGetObjectForIUnknown(created.Value, out actual)
                            : ComHelpers.TryGetObjectForIUnknown(observer.Value, out actual);
                        Assert.False(success);
                        Assert.Null(actual);
                    }
                    else
                    {
                        object? actual;
                        if (tryGet)
                        {
                            bool success = useTypedHelper
                                ? ComHelpers.TryGetObjectForIUnknown(created.Value, out actual)
                                : ComHelpers.TryGetObjectForIUnknown(observer.Value, out actual);
                            Assert.True(success);
                        }
                        else
                        {
                            actual = useTypedHelper
                                ? ComHelpers.GetObjectForIUnknown(created.Value)
                                : ComHelpers.GetObjectForIUnknown(observer.Value);
                        }

                        Assert.Same(expected, actual);
                        Assert.NotEqual(IntPtr.Zero, ((IFont.Interface)actual!).hFont);
                    }

                    Assert.NotEqual(IntPtr.Zero, nativeFont.hFont);
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
