// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

global using System;
global using System.Collections.Generic;
global using System.Diagnostics;
global using System.Diagnostics.CodeAnalysis;
global using Windows.Win32;
global using Windows.Win32.Foundation;
global using Windows.Win32.Graphics.Gdi;
global using Windows.Win32.Graphics.GdiPlus;
global using Windows.Win32.Storage.Xps;
global using Windows.Win32.System.Memory;
global using Windows.Win32.UI.WindowsAndMessaging;

#if NET9_0_OR_GREATER
global using Lock = System.Threading.Lock;
#endif

global using BitmapData = System.Drawing.Imaging.BitmapData;
global using ColorPalette = System.Drawing.Imaging.ColorPalette;
global using DashCap = System.Drawing.Drawing2D.DashCap;
global using DashStyle = System.Drawing.Drawing2D.DashStyle;
global using EmfPlusRecordType = System.Drawing.Imaging.EmfPlusRecordType;
global using FillMode = System.Drawing.Drawing2D.FillMode;
global using ImageCodecInfo = System.Drawing.Imaging.ImageCodecInfo;
global using ImageLockMode = System.Drawing.Imaging.ImageLockMode;
global using LineCap = System.Drawing.Drawing2D.LineCap;
global using LineJoin = System.Drawing.Drawing2D.LineJoin;
global using Matrix = System.Drawing.Drawing2D.Matrix;
global using MatrixOrder = System.Drawing.Drawing2D.MatrixOrder;
global using PenAlignment = System.Drawing.Drawing2D.PenAlignment;
global using PixelFormat = System.Drawing.Imaging.PixelFormat;
global using PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode;
global using TextRenderingHint = System.Drawing.Text.TextRenderingHint;

#if NET9_0_OR_GREATER
global using DitherType = System.Drawing.Imaging.DitherType;
global using PaletteType = System.Drawing.Imaging.PaletteType;
#endif

global using GdiPlus = Windows.Win32.Graphics.GdiPlus;
