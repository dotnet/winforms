// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using static Interop.Mshtml;

namespace System.Windows.Forms
{
    public sealed class HtmlHistory : IDisposable
    {
        private IOmHistory htmlHistory;
        private bool disposed;

        internal HtmlHistory(IOmHistory history)
        {
            htmlHistory = history;
            Debug.Assert(NativeOmHistory != null, "The history object should implement IOmHistory");
        }

        private IOmHistory NativeOmHistory
        {
            get
            {
                if (disposed)
                {
                    throw new ObjectDisposedException(GetType().Name);
                }
                return htmlHistory;
            }
        }

        public void Dispose()
        {
            htmlHistory = null;
            disposed = true;
            GC.SuppressFinalize(this);
        }

        public int Length
        {
            get
            {
                return (int)NativeOmHistory.GetLength();
            }
        }

        public void Back(int numberBack)
        {
            if (numberBack < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(numberBack), numberBack, string.Format(SR.InvalidLowBoundArgumentEx, nameof(numberBack), numberBack, 0));
            }
            else if (numberBack > 0)
            {
                object oNumForward = (object)(-numberBack);
                NativeOmHistory.Go(ref oNumForward);
            }
        }

        public void Forward(int numberForward)
        {
            if (numberForward < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(numberForward), numberForward, string.Format(SR.InvalidLowBoundArgumentEx, nameof(numberForward), numberForward, 0));
            }
            else if (numberForward > 0)
            {
                object oNumForward = (object)numberForward;
                NativeOmHistory.Go(ref oNumForward);
            }
        }

        /// <summary>
        ///  Go to a specific Uri in the history
        /// </summary>
        public void Go(Uri url)
        {
            Go(url.ToString());
        }

        /// <summary>
        ///  Go to a specific url(string) in the history
        /// </summary>
        ///  Note: We intentionally have a string overload (apparently Mort wants one).  We don't have
        ///  string overloads call Uri overloads because that breaks Uris that aren't fully qualified
        ///  (things like "www.microsoft.com") that the underlying objects support and we don't want to
        ///  break.
        public void Go(string urlString)
        {
            object loc = (object)urlString;
            NativeOmHistory.Go(ref loc);
        }

        /// <summary>
        ///  Go to the specified position in the history list
        /// </summary>
        public void Go(int relativePosition)
        {
            object loc = (object)relativePosition;
            NativeOmHistory.Go(ref loc);
        }

        public object DomHistory
        {
            get
            {
                return NativeOmHistory;
            }
        }
    }
}

