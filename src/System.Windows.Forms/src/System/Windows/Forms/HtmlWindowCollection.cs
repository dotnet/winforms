// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections;
using System.Runtime.InteropServices;
using Windows.Win32.System.Variant;
using Windows.Win32.Web.MsHtml;

namespace System.Windows.Forms;

public unsafe class HtmlWindowCollection : ICollection
{
    private readonly AgileComPointer<IHTMLFramesCollection2> _htmlFramesCollection2;
    private readonly HtmlShimManager _shimManager;

    internal HtmlWindowCollection(HtmlShimManager shimManager, IHTMLFramesCollection2* collection)
    {
#if DEBUG
        _htmlFramesCollection2 = new(collection, takeOwnership: true, trackDisposal: false);
#else
        _htmlFramesCollection2 = new(collection, takeOwnership: true);
#endif
        _shimManager = shimManager;

        Debug.Assert(NativeHTMLFramesCollection2 is not null, "The window collection object should implement IHTMLFramesCollection2");
    }

    private AgileComPointer<IHTMLFramesCollection2> NativeHTMLFramesCollection2 => _htmlFramesCollection2;

    public HtmlWindow? this[int index]
    {
        get
        {
            if (index < 0 || index >= Count)
            {
                throw new ArgumentOutOfRangeException(nameof(index), index, string.Format(SR.InvalidBoundArgument, nameof(index), index, 0, Count - 1));
            }

            var oIndex = (VARIANT)index;
            using var htmlFrames2 = NativeHTMLFramesCollection2.GetInterface();
            using VARIANT variantDispatch = default;
            htmlFrames2.Value->item(&oIndex, &variantDispatch).ThrowOnFailure();
            IHTMLWindow2* htmlWindow2;
            return variantDispatch.data.pdispVal->QueryInterface(IID.Get<IHTMLWindow2>(), (void**)&htmlWindow2).Succeeded
                ? new HtmlWindow(_shimManager, htmlWindow2)
                : null;
        }
    }

    public HtmlWindow? this[string windowId]
    {
        get
        {
            using var oWindowId = (VARIANT)windowId;
            IHTMLWindow2* htmlWindow2 = null;
            try
            {
                using var htmlFrameCollection2 = _htmlFramesCollection2.GetInterface();
                using VARIANT variantDispatch = default;
                htmlFrameCollection2.Value->item(&oWindowId, &variantDispatch).ThrowOnFailure();
                HRESULT hr = variantDispatch.data.pdispVal->QueryInterface(IID.Get<IHTMLWindow2>(), (void**)&htmlWindow2);
                hr.AssertSuccess();
            }
            catch (COMException)
            {
                throw new ArgumentException(string.Format(SR.InvalidArgument, nameof(windowId), windowId), nameof(windowId));
            }

            return htmlWindow2 is not null
                ? new HtmlWindow(_shimManager, htmlWindow2)
                : null;
        }
    }

    /// <summary>
    ///  Returns the total number of elements in the collection.
    /// </summary>
    public int Count
    {
        get
        {
            using var htmlFrames2 = NativeHTMLFramesCollection2.GetInterface();
            int length;
            htmlFrames2.Value->get_length(&length).ThrowOnFailure();
            return length;
        }
    }

    bool ICollection.IsSynchronized => false;

    object ICollection.SyncRoot => this;

    void ICollection.CopyTo(Array dest, int index)
    {
        int count = Count;
        for (int i = 0; i < count; i++)
        {
            dest.SetValue(this[i], index++);
        }
    }

    public IEnumerator GetEnumerator()
    {
        HtmlWindow[] htmlWindows = new HtmlWindow[Count];
        ((ICollection)this).CopyTo(htmlWindows, 0);

        return htmlWindows.GetEnumerator();
    }
}
