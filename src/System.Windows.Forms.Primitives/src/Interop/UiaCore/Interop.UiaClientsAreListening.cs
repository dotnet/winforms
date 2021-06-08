// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;

internal partial class Interop
{
    internal static partial class UiaCore
    {
        // Unfortunately UiaClientsAreListening() can't be used to specifically learn whether an assistive technology
        // (AT) like Narrator, Windows Magnifier, or Windows On-Screen Keyboard is running, (those 3 products are all
        // UIA clients).
        // Other UIA clients which aren't AT will also result in UiaClientsAreListening() returning true.
        // UiaClientsAreListening() was first majorly impacted when the touch keyboard was introduced for Windows touch
        // devices. The touch keyboard used UIA and so UiaClientsAreListening() returned true, regardless of whether any
        // AT is running.
        //
        // There is no known or supported way in Windows to determine that AT is running, or that a specific type of AT
        // like a screen reader is running.
        // (Don't even think about trying to achieve this with the SystemParameterInfo SPI_GETSCREENREADER flag!)
        //
        // Importing and using this method just in case one day it starts working.

        [DllImport(Libraries.UiaCore, ExactSpelling = true)]
        public static extern BOOL UiaClientsAreListening();
    }
}
