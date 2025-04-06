// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms;

[Flags]
public enum GetChildAtPointSkip
{
    None = (int)CWP_FLAGS.CWP_ALL,
    Invisible = (int)CWP_FLAGS.CWP_SKIPINVISIBLE,
    Disabled = (int)CWP_FLAGS.CWP_SKIPDISABLED,
    Transparent = (int)CWP_FLAGS.CWP_SKIPTRANSPARENT
}
