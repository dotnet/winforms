﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Windows.Win32;

internal delegate LRESULT HOOKPROC(int nCode, WPARAM wParam, LPARAM lParam);
