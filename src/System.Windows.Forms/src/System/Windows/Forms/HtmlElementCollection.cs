// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.Collections;
using System.Diagnostics;
using static Interop.Mshtml;

namespace System.Windows.Forms
{
    public sealed class HtmlElementCollection : ICollection
    {
        private readonly IHTMLElementCollection htmlElementCollection;
        private readonly HtmlElement[] elementsArray;
        private readonly HtmlShimManager shimManager;

        internal HtmlElementCollection(HtmlShimManager shimManager)
        {
            htmlElementCollection = null;
            elementsArray = null;

            this.shimManager = shimManager;
        }

        internal HtmlElementCollection(HtmlShimManager shimManager, IHTMLElementCollection elements)
        {
            htmlElementCollection = elements;
            elementsArray = null;
            this.shimManager = shimManager;
            Debug.Assert(NativeHtmlElementCollection != null, "The element collection object should implement IHTMLElementCollection");
        }

        internal HtmlElementCollection(HtmlShimManager shimManager, HtmlElement[] array)
        {
            htmlElementCollection = null;
            elementsArray = array;
            this.shimManager = shimManager;
        }

        private IHTMLElementCollection NativeHtmlElementCollection
        {
            get
            {
                return htmlElementCollection;
            }
        }

        public HtmlElement this[int index]
        {
            get
            {
                //do some bounds checking here...
                if (index < 0 || index >= Count)
                {
                    throw new ArgumentOutOfRangeException(nameof(index), index, string.Format(SR.InvalidBoundArgument, nameof(index), index, 0, Count - 1));
                }

                if (NativeHtmlElementCollection != null)
                {
                    return (NativeHtmlElementCollection.Item((object)index, (object)0) is IHTMLElement htmlElement) ? new HtmlElement(shimManager, htmlElement) : null;
                }
                else if (elementsArray != null)
                {
                    return elementsArray[index];
                }
                else
                {
                    return null;
                }
            }
        }

        public HtmlElement this[string elementId]
        {
            get
            {
                if (NativeHtmlElementCollection != null)
                {
                    return (NativeHtmlElementCollection.Item((object)elementId, (object)0) is IHTMLElement htmlElement) ? new HtmlElement(shimManager, htmlElement) : null;
                }
                else if (elementsArray != null)
                {
                    int count = elementsArray.Length;
                    for (int i = 0; i < count; i++)
                    {
                        HtmlElement element = elementsArray[i];
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
                HtmlElement element = this[i];
                if (element.GetAttribute("name") == name)
                {
                    temp[tempIndex] = element;
                    tempIndex++;
                }
            }

            if (tempIndex == 0)
            {
                return new HtmlElementCollection(shimManager);
            }
            else
            {
                HtmlElement[] elements = new HtmlElement[tempIndex];
                for (int i = 0; i < tempIndex; i++)
                {
                    elements[i] = temp[i];
                }
                return new HtmlElementCollection(shimManager, elements);
            }
        }

        /// <summary>
        ///  Returns the total number of elements in the collection.
        /// </summary>
        public int Count
        {
            get
            {
                if (NativeHtmlElementCollection != null)
                {
                    return NativeHtmlElementCollection.GetLength();
                }
                else if (elementsArray != null)
                {
                    return elementsArray.Length;
                }
                else
                {
                    return 0;
                }
            }
        }

        bool ICollection.IsSynchronized
        {
            get
            {
                return false;
            }
        }

        object ICollection.SyncRoot
        {
            get
            {
                return this;
            }
        }

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
}
