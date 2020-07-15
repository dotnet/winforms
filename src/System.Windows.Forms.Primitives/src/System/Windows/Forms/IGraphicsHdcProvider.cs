// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;
using System.Drawing.Drawing2D;
using static Interop;

namespace System.Windows.Forms
{
    /// <summary>
    ///  Used to provide a way to give <see cref="DeviceContextHdcScope"/> direct internal access to HDC's.
    /// </summary>
    internal interface IGraphicsHdcProvider
    {
        /// <summary>
        ///  If this flag is true we expect that the <see cref="Graphics"/> object obtained through
        ///  <see cref="GetGraphics(bool)"/> should not have a <see cref="Region"/> clip or <see cref="Matrix"/>
        ///  applied and therefore it is safe to skip getting them via <see cref="Graphics.GetContextInfo()"/>.
        /// </summary>
        /// <remarks>
        ///  If a <see cref="Graphics"/> object hasn't been created it, by definition, will be clean when it is
        ///  created, so this will return true.
        /// </remarks>
        bool IsGraphicsStateClean { get; }

        /// <summary>
        ///  Gets the <see cref="Gdi32.HDC"/>, if the object was created from one.
        /// </summary>
        Gdi32.HDC GetHDC();

        /// <summary>
        ///  Get the <see cref="Graphics"/> object.
        /// </summary>
        /// <param name="createIfNeeded">
        ///  If true, this will pass back a <see cref="Graphics"/> object, creating a new one *if* needed.
        ///  If false, will pass back a <see cref="Graphics"/> object *if* one exists, otherwise returns null.
        /// </param>
        /// <remarks>
        ///  Do not dispose of the returned <see cref="Graphics"/> object.
        /// </remarks>
        Graphics? GetGraphics(bool createIfNeeded);
    }
}
