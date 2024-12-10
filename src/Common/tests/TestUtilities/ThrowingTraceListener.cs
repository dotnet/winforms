// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System;

public sealed class ThrowingTraceListener : TraceListener
{
    public static ThrowingTraceListener Instance { get; } = new();

    public override void Fail(string? message, string? detailMessage)
    {
        throw new InvalidOperationException(
            $"{(string.IsNullOrEmpty(message) ? "Assertion failed" : message)}{(string.IsNullOrEmpty(detailMessage)
                ? ""
                : $"{Environment.NewLine}{detailMessage}")}");
    }

    public override void Write(object? o)
    {
    }

    public override void Write(object? o, string? category)
    {
    }

    public override void Write(string? message)
    {
    }

    public override void Write(string? message, string? category)
    {
    }

    public override void WriteLine(object? o)
    {
    }

    public override void WriteLine(object? o, string? category)
    {
    }

    public override void WriteLine(string? message)
    {
    }

    public override void WriteLine(string? message, string? category)
    {
    }
}
