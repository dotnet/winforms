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

    public class HtmlWindowCollection : ICollection {
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
                if (index < 0 || index >= this.Count) {
                    throw new ArgumentOutOfRangeException(nameof(index), string.Format(SR.InvalidBoundArgument, "index", index, 0, this.Count - 1));
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
                    throw new ArgumentException(string.Format(SR.InvalidArgument, "windowId", windowId));
                }
                return (htmlWindow2 != null) ? new HtmlWindow(shimManager, htmlWindow2) : null;
            }
        }


        /// <devdoc>
        ///     Returns the total number of elements in the collection.
        /// </devdoc>
        public int Count {
            get {
                return this.NativeHTMLFramesCollection2.GetLength();
            }
        }


        /// <internalonly/>
        bool ICollection.IsSynchronized {
            get {
                return false;
            }
        }
        

        /// <internalonly/>
        object ICollection.SyncRoot {
            get {
                return this;
            }
        }


        /// <internalonly/>
        void ICollection.CopyTo(Array dest, int index) {
            int count = this.Count;
            for (int i = 0; i < count; i++) {
                dest.SetValue(this[i], index++);
            }
        }


        /// <internalonly/>
        public IEnumerator GetEnumerator() {
            HtmlWindow[] htmlWindows = new HtmlWindow[this.Count];
            ((ICollection)this).CopyTo(htmlWindows, 0);
            
            return htmlWindows.GetEnumerator();
        }

    }
}

