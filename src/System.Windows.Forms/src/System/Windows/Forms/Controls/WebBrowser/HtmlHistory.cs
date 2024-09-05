// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Windows.Win32.System.Variant;
using Windows.Win32.Web.MsHtml;

namespace System.Windows.Forms;

public sealed unsafe class HtmlHistory : IDisposable
{
    private AgileComPointer<IOmHistory> _htmlHistory;
    private bool _disposed;

    internal HtmlHistory(IOmHistory* history)
    {
        _htmlHistory = new(history, takeOwnership: true);
        Debug.Assert(NativeOmHistory is not null, "The history object should implement IOmHistory");
    }

    private AgileComPointer<IOmHistory> NativeOmHistory
    {
        get
        {
            ObjectDisposedException.ThrowIf(_disposed, this);
            return _htmlHistory;
        }
    }

    public void Dispose()
    {
        DisposeHelper.NullAndDispose(ref _htmlHistory!);
        _disposed = true;
        GC.SuppressFinalize(this);
    }

    public int Length
    {
        get
        {
            using var omHistory = NativeOmHistory.GetInterface();
            return omHistory.Value->length;
        }
    }

    public void Back(int numberBack)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(numberBack);

        if (numberBack > 0)
        {
            var oNumForward = (VARIANT)(-numberBack);
            using var omHistory = NativeOmHistory.GetInterface();
            omHistory.Value->go(&oNumForward).ThrowOnFailure();
        }
    }

    public void Forward(int numberForward)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(numberForward);

        if (numberForward > 0)
        {
            var oNumForward = (VARIANT)numberForward;
            using var omHistory = NativeOmHistory.GetInterface();
            omHistory.Value->go(&oNumForward).ThrowOnFailure();
        }
    }

    /// <summary>
    ///  Go to a specific Uri in the history
    /// </summary>
    public void Go(Uri url) => Go(url.ToString());

    /// <summary>
    ///  Go to a specific url(string) in the history
    /// </summary>
    ///  Note: We intentionally have a string overload (apparently Mort wants one). We don't have
    ///  string overloads call Uri overloads because that breaks Uris that aren't fully qualified
    ///  (things like "www.Microsoft.com") that the underlying objects support and we don't want to
    ///  break.
    public void Go(string urlString)
    {
        using var loc = (VARIANT)urlString;
        using var omHistory = NativeOmHistory.GetInterface();
        omHistory.Value->go(&loc).ThrowOnFailure();
    }

    /// <summary>
    ///  Go to the specified position in the history list
    /// </summary>
    public void Go(int relativePosition)
    {
        var loc = (VARIANT)relativePosition;
        using var omHistory = NativeOmHistory.GetInterface();
        omHistory.Value->go(&loc).ThrowOnFailure();
    }

    public object DomHistory => NativeOmHistory.GetManagedObject();
}
