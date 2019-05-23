// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.Diagnostics;

namespace System.Windows.Forms
{
    public sealed class HtmlElementCollection : ICollection
    {
        private UnsafeNativeMethods.IHTMLElementCollection htmlElementCollection;
        private HtmlElement[] elementsArray;
        private HtmlShimManager shimManager;

        internal HtmlElementCollection(HtmlShimManager shimManager)
        {
            this.htmlElementCollection = null;
            this.elementsArray = null;

            this.shimManager = shimManager;
        }

        internal HtmlElementCollection(HtmlShimManager shimManager, UnsafeNativeMethods.IHTMLElementCollection elements)
        {
            this.htmlElementCollection = elements;
            this.elementsArray = null;
            this.shimManager = shimManager;
            Debug.Assert(this.NativeHtmlElementCollection != null, "The element collection object should implement IHTMLElementCollection");
        }

        internal HtmlElementCollection(HtmlShimManager shimManager, HtmlElement[] array)
        {
            this.htmlElementCollection = null;
            this.elementsArray = array;
            this.shimManager = shimManager;
        }

        private UnsafeNativeMethods.IHTMLElementCollection NativeHtmlElementCollection
        {
            get
            {
                return this.htmlElementCollection;
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

                if (this.NativeHtmlElementCollection != null)
                {
                    UnsafeNativeMethods.IHTMLElement htmlElement =
                            this.NativeHtmlElementCollection.Item((object)index, (object)0) as UnsafeNativeMethods.IHTMLElement;
                    return (htmlElement != null) ? new HtmlElement(shimManager, htmlElement) : null;
                }
                else if (elementsArray != null)
                {
                    return this.elementsArray[index];
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
                if (this.NativeHtmlElementCollection != null)
                {
                    UnsafeNativeMethods.IHTMLElement htmlElement =
                            this.NativeHtmlElementCollection.Item((object)elementId, (object)0) as UnsafeNativeMethods.IHTMLElement;
                    return (htmlElement != null) ? new HtmlElement(shimManager, htmlElement) : null;
                }
                else if (elementsArray != null)
                {
                    int count = this.elementsArray.Length;
                    for (int i = 0; i < count; i++)
                    {
                        HtmlElement element = this.elementsArray[i];
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
            int count = this.Count;
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
        ///     Returns the total number of elements in the collection.
        /// </summary>
        public int Count
        {
            get
            {
                if (this.NativeHtmlElementCollection != null)
                {
                    return this.NativeHtmlElementCollection.GetLength();
                }
                else if (elementsArray != null)
                {
                    return this.elementsArray.Length;
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
            int count = this.Count;
            for (int i = 0; i < count; i++)
            {
                dest.SetValue(this[i], index++);
            }
        }

        public IEnumerator GetEnumerator()
        {
            HtmlElement[] htmlElements = new HtmlElement[this.Count];
            ((ICollection)this).CopyTo(htmlElements, 0);

            return htmlElements.GetEnumerator();
        }
    }
}

