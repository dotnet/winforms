// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

global using System.Diagnostics;
global using System.Diagnostics.CodeAnalysis;
global using System.Private.Windows;

global using AppContextSwitches = System.Windows.Forms.Primitives.LocalAppContextSwitches;

// Having these as global usings reduces verbiage in code and avoids accidental mismatching when defining types.
global using ClipboardCore = System.Private.Windows.Ole.ClipboardCore<
    System.Windows.Forms.Ole.WinFormsOleServices>;
global using Composition = System.Private.Windows.Ole.Composition<
    System.Windows.Forms.Ole.WinFormsOleServices,
    System.Windows.Forms.Nrbf.WinFormsNrbfSerializer,
    System.Windows.Forms.DataFormats.Format>;
global using DataFormatsCore = System.Private.Windows.Ole.DataFormatsCore<
    System.Windows.Forms.DataFormats.Format>;
global using DragDropHelper = System.Private.Windows.Ole.DragDropHelper<
    System.Windows.Forms.Ole.WinFormsOleServices,
    System.Windows.Forms.DataFormats.Format>;

global using Windows.Win32;
global using Windows.Win32.Foundation;
global using Windows.Win32.Graphics.Gdi;
global using Windows.Win32.UI.Controls;
global using Windows.Win32.UI.HiDpi;
global using Windows.Win32.UI.Shell;
global using Windows.Win32.UI.Shell.Common;
global using Windows.Win32.UI.WindowsAndMessaging;
global using PCWSTR = Windows.Win32.Foundation.PCWSTR;
global using PWSTR = Windows.Win32.Foundation.PWSTR;

global using Color = System.Drawing.Color;
global using Point = System.Drawing.Point;
global using PointF = System.Drawing.PointF;
