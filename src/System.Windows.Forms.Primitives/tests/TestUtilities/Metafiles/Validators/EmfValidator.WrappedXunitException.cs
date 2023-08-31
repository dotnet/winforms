// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable enable

using Xunit.Sdk;

namespace System.Windows.Forms.Metafiles;

internal static partial class EmfValidator
{
    private class WrappedXunitException : XunitException
    {
        public WrappedXunitException(string userMessage, XunitException innerException)
            : base(userMessage, innerException)
        {
        }
    }
}
