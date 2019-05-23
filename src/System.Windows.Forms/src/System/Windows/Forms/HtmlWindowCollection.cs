// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace System.Windows.Forms
{
    public class HtmlWindowCollection : ICollection
    {
        private UnsafeNativeMethods.IHTMLFramesCollection2 htmlFramesCollection2;
        private HtmlShimManager shimManager;
        
        internal HtmlWindowCollection(HtmlShimManager shimManager, UnsafeNativeMethods.IHTMLFramesCollection2 collection) {
            this.htmlFramesCollection2 = collection;
            this.shimManager = shimManager;
            
            Debug.Assert(this.NativeHTMLFramesCollection2 != null, "The window collection object should implement IHTMLFramesCollection2");
        }

        private UnsafeNativeMethods.IHTMLFramesCollection2 NativeHTMLFramesCollection2 {
            get {
                return this.htmlFramesCollection2;
            }
        }


        public HtmlWindow this[int index] {
            get {
                if (index < 0 || index >= Count) {
                    throw new ArgumentOutOfRangeException(nameof(index), index, string.Format(SR.InvalidBoundArgument, nameof(index), index, 0, Count - 1));
                }

                object oIndex = (object)index;
                UnsafeNativeMethods.IHTMLWindow2 htmlWindow2 = this.NativeHTMLFramesCollection2.Item(ref oIndex)
                        as UnsafeNativeMethods.IHTMLWindow2;
                return (htmlWindow2 != null) ? new HtmlWindow(shimManager, htmlWindow2) : null;
            }
        }

        public HtmlWindow this[string windowId] {
            get {
                object oWindowId = (object)windowId;
                UnsafeNativeMethods.IHTMLWindow2 htmlWindow2 = null;
                try {
                    htmlWindow2 = this.htmlFramesCollection2.Item(ref oWindowId)
                            as UnsafeNativeMethods.IHTMLWindow2;
                }
                catch (COMException) {
                    throw new ArgumentException(string.Format(SR.InvalidArgument, nameof(windowId), windowId), nameof(windowId));
                }
                return (htmlWindow2 != null) ? new HtmlWindow(shimManager, htmlWindow2) : null;
            }
        }

        /// <summary>
        ///     Returns the total number of elements in the collection.
        /// </summary>
        public int Count {
            get {
                return this.NativeHTMLFramesCollection2.GetLength();
            }
        }

        bool ICollection.IsSynchronized {
            get {
                return false;
            }
        }
        
        object ICollection.SyncRoot {
            get {
                return this;
            }
        }

        void ICollection.CopyTo(Array dest, int index) {
            int count = this.Count;
            for (int i = 0; i < count; i++) {
                dest.SetValue(this[i], index++);
            }
        }

        public IEnumerator GetEnumerator() {
            HtmlWindow[] htmlWindows = new HtmlWindow[this.Count];
            ((ICollection)this).CopyTo(htmlWindows, 0);
            
            return htmlWindows.GetEnumerator();
        }

    }
}

