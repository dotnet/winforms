﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms;

// this is used by Application.cs to detect if we should respect changes to
// the message as well as whether or not we should filter the message.
internal interface IMessageModifyAndFilter : IMessageFilter
{
}
