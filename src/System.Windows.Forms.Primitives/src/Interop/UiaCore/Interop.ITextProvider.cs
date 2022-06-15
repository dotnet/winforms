// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;
using System.Runtime.InteropServices;

internal static partial class Interop
{
    internal static partial class UiaCore
    {
        [ComImport]
        [Guid("3589c92c-63f3-4367-99bb-ada653b77cf2")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface ITextProvider
        {
            ITextRangeProvider[]? GetSelection();

            ITextRangeProvider[]? GetVisibleRanges();

            ITextRangeProvider? RangeFromChild(IRawElementProviderSimple childElement);

            ITextRangeProvider? RangeFromPoint(Point screenLocation);

            ITextRangeProvider? DocumentRange { get; }

            SupportedTextSelection SupportedTextSelection { get; }
        }
    }
}
