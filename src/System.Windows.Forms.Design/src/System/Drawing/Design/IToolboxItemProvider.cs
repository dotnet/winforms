// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Drawing.Design;

public interface IToolboxItemProvider
{
    ToolboxItemCollection Items { get; }
}
