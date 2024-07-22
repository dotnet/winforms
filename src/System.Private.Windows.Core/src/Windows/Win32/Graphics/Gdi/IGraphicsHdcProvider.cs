// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Windows.Win32.Graphics.GdiPlus;

namespace System.Drawing;

/// <summary>
///  Used to provide a way to give <see cref="DeviceContextHdcScope"/> direct internal access to HDC's.
/// </summary>
internal interface IGraphicsHdcProvider
{
    /// <summary>
    ///  If this flag is true we expect that the <see cref="GpGraphics"/> object obtained through
    ///  <see cref="GetGraphics(bool)"/> should not have a <see cref="GpRegion"/> clip or GpMatrix
    ///  applied and therefore it is safe to skip getting them.
    /// </summary>
    /// <remarks>
    ///  <para>
    ///   If a <see cref="GpGraphics"/> object hasn't been created it, by definition, will be clean when it is
    ///   created, so this will return true.
    ///  </para>
    /// </remarks>
    bool IsGraphicsStateClean { get; }

    /// <summary>
    ///  Gets the <see cref="HDC"/>, if the object was created from one.
    /// </summary>
    HDC GetHdc();

    /// <summary>
    ///  Get the <see cref="GpGraphics"/> object.
    /// </summary>
    /// <param name="createIfNeeded">
    ///  If true, this will pass back a <see cref="GpGraphics"/> object, creating a new one *if* needed.
    ///  If false, will pass back a <see cref="GpGraphics"/> object *if* one exists, otherwise returns null.
    /// </param>
    /// <remarks>
    ///  <para>Do not dispose of the returned <see cref="GpGraphics"/> object.</para>
    /// </remarks>
    IGraphics? GetGraphics(bool createIfNeeded);
}
