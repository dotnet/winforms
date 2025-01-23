// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System;

internal static class DisposalTracking
{
    /// <summary>
    ///  Used to suppress finalization in debug builds only.
    /// </summary>
    /// <remarks>
    ///  <para>
    ///   Unfortunately this can only be used when there is a single implicit conversion operator when called from
    ///   a ref struct. C# tries to cast to anything that fits in object, which leads to an ambiguous error.
    ///  </para>
    ///  <para>
    ///   You need to add GC.SuppressFinalize under #ifdef when you don't have a single implicit conversion.
    ///  </para>
    /// </remarks>
    [Conditional("DEBUG")]
    public static void SuppressFinalize(object @object)
    {
        GC.SuppressFinalize(@object);
    }

#if DEBUG
    /// <summary>
    ///  Helper base class for tracking undisposed objects. Derive from this (in DEBUG builds only) to track
    ///  construction and proper destruction.
    /// </summary>
    /// <remarks>
    ///  <para>
    ///   Fires if <see cref="GC.SuppressFinalize(object)"/> is not called on the class and the class is finalized.
    ///   As such you must suppress finalization when disposing to "signal" that you've been disposed properly.
    ///  </para>
    ///  <para>
    ///   The debug only static <see cref="SuppressFinalize(object)"/> can be called when you only derive from this
    ///   class in debug builds.
    ///  </para>
    /// </remarks>
    internal abstract class Tracker
    {
        private readonly StackTrace? _originatingStack;
        private readonly bool _throwIfFinalized;

        public Tracker(bool throwIfFinalized = true)
        {
            _throwIfFinalized = throwIfFinalized;
            _originatingStack = _throwIfFinalized ? new StackTrace() : null;
        }

        ~Tracker()
        {
            if (_throwIfFinalized)
            {
                // Not asserting here as assertions take down test runs.
                throw new InvalidOperationException($"Did not dispose `{GetFriendlyTypeName(GetType())}`. Originating stack:\n{_originatingStack}");
            }
        }

        private static string GetFriendlyTypeName(Type type)
        {
            string friendlyName = type.Name;
            if (type.IsGenericType)
            {
                int backtick = friendlyName.IndexOf('`');
                if (backtick != -1)
                {
                    friendlyName = friendlyName[..backtick];
                }

                friendlyName += $"<{string.Join(",", type.GetGenericArguments().Select(GetFriendlyTypeName))}>";
            }

            return friendlyName;
        }
    }
#endif
}
