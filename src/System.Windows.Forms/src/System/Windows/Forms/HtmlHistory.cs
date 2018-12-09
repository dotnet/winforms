// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Drawing;
using System.Drawing.Printing;
using System.Windows.Forms;
using System.Security.Permissions;
using System.Security;
using System.Runtime.InteropServices;
using System.Net;
using System.Globalization;

namespace System.Windows.Forms {
    /// <include file='doc\HtmlHistory.uex' path='docs/doc[@for="HtmlHistory"]/*' />
    [PermissionSetAttribute(SecurityAction.LinkDemand, Name = "FullTrust")]
    public sealed class HtmlHistory : IDisposable
    {
        private UnsafeNativeMethods.IOmHistory htmlHistory;
        private bool disposed;

        [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
        internal HtmlHistory(UnsafeNativeMethods.IOmHistory history)
        {
            this.htmlHistory = history;
            Debug.Assert(this.NativeOmHistory != null, "The history object should implement IOmHistory");
        }

        private UnsafeNativeMethods.IOmHistory NativeOmHistory {
            get {
                if (this.disposed) {
                    throw new System.ObjectDisposedException(GetType().Name);
                }
                return this.htmlHistory;
            }
        }

        /// <include file='doc\HtmlHistory.uex' path='docs/doc[@for="HtmlHistory.Dispose"]/*' />
        public void Dispose() {
            this.htmlHistory = null;
            this.disposed = true;
            GC.SuppressFinalize(this);
        }

        /// <include file='doc\HtmlHistory.uex' path='docs/doc[@for="HtmlHistory.Length"]/*' />
        public int Length {
            get {
                return (int)this.NativeOmHistory.GetLength();
            }
        }

        /// <include file='doc\HtmlHistory.uex' path='docs/doc[@for="HtmlHistory.Back"]/*' />
        public void Back(int numberBack) {
            if (numberBack < 0) {
                throw new ArgumentOutOfRangeException(nameof(numberBack), string.Format(SR.InvalidLowBoundArgumentEx, "numberBack", numberBack.ToString(CultureInfo.CurrentCulture), (0).ToString(CultureInfo.CurrentCulture)));
            }
            else if (numberBack > 0) {
                object oNumForward = (object)(-numberBack);
                this.NativeOmHistory.Go(ref oNumForward);
            }
        }

        /// <include file='doc\HtmlHistory.uex' path='docs/doc[@for="HtmlHistory.Forward"]/*' />
        public void Forward(int numberForward) {
            if (numberForward < 0) {
                throw new ArgumentOutOfRangeException(nameof(numberForward), string.Format(SR.InvalidLowBoundArgumentEx, "numberForward", numberForward.ToString(CultureInfo.CurrentCulture), (0).ToString(CultureInfo.CurrentCulture)));
            }
            else if (numberForward > 0) {
                object oNumForward = (object)numberForward;
                this.NativeOmHistory.Go(ref oNumForward);
            }
        }

        /// <include file='doc\HtmlHistory.uex' path='docs/doc[@for="HtmlHistory.Go1"]/*' />
        /// <devdoc>
        ///    <para>Go to a specific Uri in the history</para>
        /// </devdoc>
        [SuppressMessage("Microsoft.Usage", "CA2234:PassSystemUriObjectsInsteadOfStrings")]
        public void Go(Uri url)
        {
            Go(url.ToString());
        }

        /// <include file='doc\HtmlHistory.uex' path='docs/doc[@for="HtmlHistory.Go1"]/*' />
        /// <devdoc>
        ///    <para>Go to a specific url(string) in the history</para>
        /// </devdoc>
        /// Note: We intentionally have a string overload (apparently Mort wants one).  We don't have 
        /// string overloads call Uri overloads because that breaks Uris that aren't fully qualified 
        /// (things like "www.microsoft.com") that the underlying objects support and we don't want to 
        /// break.
        [SuppressMessage("Microsoft.Design", "CA1057:StringUriOverloadsCallSystemUriOverloads")]
        public void Go(string urlString)
        {
            object loc = (object)urlString;
            this.NativeOmHistory.Go(ref loc);
        }

        /// <include file='doc\HtmlHistory.uex' path='docs/doc[@for="HtmlHistory.Go2"]/*' />
        /// <devdoc>
        ///    <para>Go to the specified position in the history list</para>
        /// </devdoc>
        public void Go(int relativePosition) {
            object loc = (object)relativePosition;
            this.NativeOmHistory.Go(ref loc);
        }

        /// <include file='doc\HtmlHistory.uex' path='docs/doc[@for="HtmlHistory.DomHistory"]/*' />
        public object DomHistory {
            get {
                return this.NativeOmHistory;
            }
        }
    }
}

