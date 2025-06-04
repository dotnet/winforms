// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Private.Windows.Ole;

/// <summary>
///  This exception is used to indicate that clipboard contains a serialized
///  managed object that contains unexpected types and that we should stop processing this data.
/// </summary>
internal class RestrictedTypeDeserializationException : Exception
{
    public RestrictedTypeDeserializationException(string? message) : base(message)
    {
    }
}
