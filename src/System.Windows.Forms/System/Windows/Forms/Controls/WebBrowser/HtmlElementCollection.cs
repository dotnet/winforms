// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections;
using Windows.Win32.System.Com;
using Windows.Win32.System.Variant;
using Windows.Win32.Web.MsHtml;

namespace System.Windows.Forms;

public sealed unsafe class HtmlElementCollection : ICollection
{
    private readonly AgileComPointer<IHTMLElementCollection>? _htmlElementCollection;
    private readonly HtmlElement[]? _elementsArray;
    private readonly HtmlShimManager _shimManager;

    internal HtmlElementCollection(HtmlShimManager shimManager)
    {
        _htmlElementCollection = null;
        _elementsArray = null;

        _shimManager = shimManager;
    }

    internal HtmlElementCollection(HtmlShimManager shimManager, IHTMLElementCollection* elements)
    {
#if DEBUG
        _htmlElementCollection = new(elements, takeOwnership: true, trackDisposal: false);
#else
        _htmlElementCollection = new(elements, takeOwnership: true);
#endif
        _elementsArray = null;
        _shimManager = shimManager;
        Debug.Assert(NativeHtmlElementCollection is not null, "The element collection object should implement IHTMLElementCollection");
    }

    internal HtmlElementCollection(HtmlShimManager shimManager, HtmlElement[] array)
    {
        _htmlElementCollection = null;
        _elementsArray = array;
        _shimManager = shimManager;
    }

    private AgileComPointer<IHTMLElementCollection>? NativeHtmlElementCollection => _htmlElementCollection;

    public HtmlElement? this[int index]
    {
        get
        {
            // do some bounds checking here...
            ArgumentOutOfRangeException.ThrowIfNegative(index);
            ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(index, Count);

            if (NativeHtmlElementCollection is not null)
            {
                using var htmlElementCollection = NativeHtmlElementCollection.GetInterface();
                using ComScope<IDispatch> dispatch = new(null);
                htmlElementCollection.Value->item((VARIANT)index, (VARIANT)0, dispatch).ThrowOnFailure();
                IHTMLElement* htmlElement;
                return !dispatch.IsNull && dispatch.Value->QueryInterface(IID.Get<IHTMLElement>(), (void**)&htmlElement).Succeeded
                    ? new HtmlElement(_shimManager, htmlElement)
                    : null;
            }

            return _elementsArray?[index];
        }
    }

    public HtmlElement? this[string elementId]
    {
        get
        {
            if (NativeHtmlElementCollection is not null)
            {
                using var nativeHtmlElementCollection = NativeHtmlElementCollection.GetInterface();
                using var variantElementId = (VARIANT)elementId;
                using ComScope<IDispatch> dispatch = new(null);
                nativeHtmlElementCollection.Value->item(variantElementId, (VARIANT)0, dispatch).ThrowOnFailure();
                IHTMLElement* htmlElement;
                return !dispatch.IsNull && dispatch.Value->QueryInterface(IID.Get<IHTMLElement>(), (void**)&htmlElement).Succeeded
                    ? new(_shimManager, htmlElement)
                    : null;
            }
            else if (_elementsArray is not null)
            {
                int count = _elementsArray.Length;
                for (int i = 0; i < count; i++)
                {
                    HtmlElement element = _elementsArray[i];
                    if (element.Id == elementId)
                    {
                        return element;
                    }
                }

                return null;    // not found
            }
            else
            {
                return null;
            }
        }
    }

    public HtmlElementCollection GetElementsByName(string name)
    {
        int count = Count;
        HtmlElement[] temp = new HtmlElement[count];    // count is the maximum # of matches
        int tempIndex = 0;

        for (int i = 0; i < count; i++)
        {
            HtmlElement element = this[i]!;
            if (element.GetAttribute("name") == name)
            {
                temp[tempIndex] = element;
                tempIndex++;
            }
        }

        if (tempIndex == 0)
        {
            return new HtmlElementCollection(_shimManager);
        }
        else
        {
            HtmlElement[] elements = new HtmlElement[tempIndex];
            for (int i = 0; i < tempIndex; i++)
            {
                elements[i] = temp[i];
            }

            return new HtmlElementCollection(_shimManager, elements);
        }
    }

    /// <summary>
    ///  Returns the total number of elements in the collection.
    /// </summary>
    public int Count
    {
        get
        {
            if (NativeHtmlElementCollection is not null)
            {
                using var nativeHtmlElementCollection = NativeHtmlElementCollection.GetInterface();
                int length;
                nativeHtmlElementCollection.Value->get_length(&length).ThrowOnFailure();
                return length;
            }

            return _elementsArray is not null ? _elementsArray.Length : 0;
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
        HtmlElement[] htmlElements = new HtmlElement[Count];
        ((ICollection)this).CopyTo(htmlElements, 0);

        return htmlElements.GetEnumerator();
    }
}
