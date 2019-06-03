// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

internal static partial class Interop
{
    public static class CommonHandles
    {
        /// <summary>
        /// Handle type for accelerator tables.
        /// </summary>
        public static readonly int Accelerator = HandleCollector.RegisterType("Accelerator", 80, 50);

        /// <summary>
        /// Handle type for cursors.
        /// </summary>
        public static readonly int Cursor = HandleCollector.RegisterType("Cursor", 20, 500);

        /// <summary>
        /// Handle type for enhanced metafiles.
        /// </summary>
        public static readonly int EMF = HandleCollector.RegisterType("EnhancedMetaFile", 20, 500);

        /// <summary>
        /// Handle type for file find handles.
        /// </summary>
        public static readonly int Find = HandleCollector.RegisterType("Find", 0, 1000);

        /// <summary>
        /// Handle type for GDI objects.
        /// </summary>
        public static readonly int GDI = HandleCollector.RegisterType("GDI", 50, 500);

        /// <summary>
        /// Handle type for HDC's that count against the Win98 limit of five DC's.  HDC's
        /// which are not scarce, such as HDC's for bitmaps, are counted as GDIHANDLE's.
        /// </summary>
        public static readonly int HDC = HandleCollector.RegisterType("HDC", 100, 2); // wait for 2 dc's before collecting

        /// <summary>
        /// Handle type for Compatible HDC's used for ToolStrips
        /// </summary>
        public static readonly int CompatibleHDC = HandleCollector.RegisterType("ComptibleHDC", 50, 50); // wait for 2 dc's before collecting

        /// <summary>
        /// Handle type for icons.
        /// </summary>
        public static readonly int Icon = HandleCollector.RegisterType("Icon", 20, 500);

        /// <summary>
        /// Handle type for kernel objects.
        /// </summary>
        public static readonly int Kernel = HandleCollector.RegisterType("Kernel", 0, 1000);

        /// <summary>
        /// Handle type for files.
        /// </summary>
        public static readonly int Menu = HandleCollector.RegisterType("Menu", 30, 1000);

        /// <summary>
        /// Handle type for windows.
        /// </summary>
        public static readonly int Window = HandleCollector.RegisterType("Window", 5, 1000);
    }
}
