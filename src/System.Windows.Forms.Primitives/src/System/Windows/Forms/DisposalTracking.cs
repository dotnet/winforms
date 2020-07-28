// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;

namespace System
{
    internal static class DisposalTracking
    {
        /// <summary>
        ///  Used to suppress finalization in debug builds only.
        /// </summary>
        /// <remarks>
        ///  Unfortunately this can only be used when there is a single implicit conversion operator when called from
        ///  a ref struct. C# tries to cast to anything that fits in object, which leads to an ambiguous error.
        ///
        ///  You need to add GC.SuppressFinalize under #ifdef when you don't have a single implicit conversion.
        /// </remarks>
        [Conditional("DEBUG")]
        public static void SuppressFinalize(object @object)
        {
            GC.SuppressFinalize(@object);
        }

        /// <summary>
        ///  Helper base class for tracking undisposed objects.
        /// </summary>
        /// <remarks>
        ///  Fires if <see cref="GC.SuppressFinalize(object)"/> is not called on the class and the class is finalized.
        ///  As such you must suppress finalization when disposing to "signal" that you've been disposed properly.
        ///
        ///  The debug only static <see cref="SuppressFinalize(object)"/> can be called when you only derive from this
        ///  class in debug builds.
        /// </remarks>
        internal abstract class Tracker
        {
            private readonly string _originatingStack = new StackTrace().ToString();

            ~Tracker()
            {
                Debug.Fail($"Did not dispose {GetType().Name}. Originating stack:\n{_originatingStack}");
            }
        }
    }
}
