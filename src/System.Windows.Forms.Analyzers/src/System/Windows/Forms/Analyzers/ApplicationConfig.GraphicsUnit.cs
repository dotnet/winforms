// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms.Analyzers;

internal partial record ApplicationConfig
{
    // Copied from https://github.com/dotnet/runtime/blob/00ee1c18715723e62484c9bc8a14f517455fc3b3/src/libraries/System.Drawing.Common/src/System/Drawing/GraphicsUnit.cs
    public enum GraphicsUnit
    {
        /// <summary>
        /// Specifies the world unit as the unit of measure.
        /// </summary>
        World = 0,

        /// <summary>
        /// Specifies 1/75 inch as the unit of measure.
        /// </summary>
        Display = 1,

        /// <summary>
        /// Specifies a device pixel as the unit of measure.
        /// </summary>
        Pixel = 2,

        /// <summary>
        /// Specifies a printer's point (1/72 inch) as the unit of measure.
        /// </summary>
        Point = 3,

        /// <summary>
        /// Specifies the inch as the unit of measure.
        /// </summary>
        Inch = 4,

        /// <summary>
        /// Specifies the document unit (1/300 inch) as the unit of measure.
        /// </summary>
        Document = 5,

        /// <summary>
        /// Specifies the millimeter as the unit of measure.
        /// </summary>
        Millimeter = 6
    }
}
