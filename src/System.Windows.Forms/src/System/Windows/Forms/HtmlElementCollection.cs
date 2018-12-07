// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Drawing;
using System.Drawing.Printing;
using System.Windows.Forms;
using System.Security.Permissions;
using System.Security;
using System.Runtime.InteropServices;
using System.Net;
using System.Collections;

namespace System.Windows.Forms {
    /// <include file='doc\HtmlElementCollection.uex' path='docs/doc[@for="HtmlElementCollection"]/*' />
    public sealed class HtmlElementCollection : ICollection {
        private UnsafeNativeMethods.IHTMLElementCollection htmlElementCollection;
        private HtmlElement[] elementsArray;
        private HtmlShimManager shimManager;
        
        internal HtmlElementCollection(HtmlShimManager shimManager) {
            this.htmlElementCollection = null;
            this.elementsArray = null;

            this.shimManager = shimManager;
        }
        
        internal HtmlElementCollection(HtmlShimManager shimManager, UnsafeNativeMethods.IHTMLElementCollection elements) {
            this.htmlElementCollection = elements;
            this.elementsArray = null;
            this.shimManager = shimManager;
            Debug.Assert(this.NativeHtmlElementCollection != null, "The element collection object should implement IHTMLElementCollection");
        }

        internal HtmlElementCollection(HtmlShimManager shimManager, HtmlElement[] array) {
            this.htmlElementCollection = null;
            this.elementsArray = array;
            this.shimManager = shimManager;
        }

        private UnsafeNativeMethods.IHTMLElementCollection NativeHtmlElementCollection {
            get {
                return this.htmlElementCollection;
            }
        }

        /// <include file='doc\HtmlElementCollection.uex' path='docs/doc[@for="HtmlElementCollection.this"]/*' />
        public HtmlElement this[int index] {
            get {
                //do some bounds checking here...
                if (index < 0 || index >= this.Count) {
                    throw new ArgumentOutOfRangeException(nameof(index), string.Format(SR.InvalidBoundArgument, "index", index, 0, this.Count - 1));
                }

                if (this.NativeHtmlElementCollection != null) {
                    UnsafeNativeMethods.IHTMLElement htmlElement =
                            this.NativeHtmlElementCollection.Item((object)index, (object)0) as UnsafeNativeMethods.IHTMLElement;
                    return (htmlElement != null) ? new HtmlElement(shimManager, htmlElement) : null;
                }
                else if (elementsArray != null) {
                    return this.elementsArray[index];
                }
                else {
                    return null;
                }
            }
        }

        /// <include file='doc\HtmlElementCollection.uex' path='docs/doc[@for="HtmlElementCollection.this1"]/*' />
        public HtmlElement this[string elementId] {
            get {
                if (this.NativeHtmlElementCollection != null) {
                    UnsafeNativeMethods.IHTMLElement htmlElement =
                            this.NativeHtmlElementCollection.Item((object)elementId, (object)0) as UnsafeNativeMethods.IHTMLElement;
                    return (htmlElement != null) ? new HtmlElement(shimManager, htmlElement) : null;
                }
                else if (elementsArray != null) {
                    int count = this.elementsArray.Length;
                    for (int i = 0; i < count; i++) {
                        HtmlElement element = this.elementsArray[i];
                        if (element.Id == elementId) {
                            return element;
                        }
                    }
                    return null;    // not found
                }
                else {
                    return null;
                }
            }
        }

        /// <include file='doc\HtmlElementCollection.uex' path='docs/doc[@for="HtmlElementCollection.GetElementsByName"]/*' />
        public HtmlElementCollection GetElementsByName(string name) {
            int count = this.Count;
            HtmlElement[] temp = new HtmlElement[count];    // count is the maximum # of matches
            int tempIndex = 0;
            
            for (int i = 0; i < count; i++) {
                HtmlElement element = this[i];
                if (element.GetAttribute("name") == name) {
                    temp[tempIndex] = element;
                    tempIndex++;
                }
            }
            
            if (tempIndex == 0) {
                return new HtmlElementCollection(shimManager);
            }
            else {
                HtmlElement[] elements = new HtmlElement[tempIndex];
                for (int i = 0; i < tempIndex; i++) {
                    elements[i] = temp[i];
                }
                return new HtmlElementCollection(shimManager, elements);
            }
        }

        /// <include file='doc\HtmlElementCollection.uex' path='docs/doc[@for="HtmlElementCollection.Count"]/*' />
        /// <devdoc>
        ///     Returns the total number of elements in the collection.
        /// </devdoc>
        public int Count {
            get {
                if (this.NativeHtmlElementCollection != null) {
                    return this.NativeHtmlElementCollection.GetLength();
                }
                else if (elementsArray != null) {
                    return this.elementsArray.Length;
                }
                else {
                    return 0;
                }
            }
        }
        
        /// <include file='doc\HtmlElementCollection.uex' path='docs/doc[@for="HtmlElementCollection.ICollection.IsSynchronized"]/*' />
        /// <internalonly/>
        bool ICollection.IsSynchronized {
            get {
                return false;
            }
        }
        
        /// <include file='doc\HtmlElementCollection.uex' path='docs/doc[@for="HtmlElementCollection.ICollection.SyncRoot"]/*' />
        /// <internalonly/>
        object ICollection.SyncRoot {
            get {
                return this;
            }
        }

        /// <include file='doc\HtmlElementCollection.uex' path='docs/doc[@for="HtmlElementCollection.ICollection.CopyTo"]/*' />
        /// <internalonly/>
        void ICollection.CopyTo(Array dest, int index) {
            int count = this.Count;
            for (int i = 0; i < count; i++) {
                dest.SetValue(this[i], index++);
            }
        }

        /// <include file='doc\HtmlElementCollection.uex' path='docs/doc[@for="HtmlElementCollection.GetEnumerator"]/*' />
        /// <internalonly/>
        public IEnumerator GetEnumerator() {
            HtmlElement[] htmlElements = new HtmlElement[this.Count];
            ((ICollection)this).CopyTo(htmlElements, 0);
            
            return htmlElements.GetEnumerator();
        }
    }
}

