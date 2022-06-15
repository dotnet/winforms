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
        [Guid("0dc5e6ed-3e16-4bf1-8f9a-a979878bc195")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface ITextProvider2 : ITextProvider
        {
            new ITextRangeProvider[]? GetSelection();

            new ITextRangeProvider[]? GetVisibleRanges();

            new ITextRangeProvider? RangeFromChild(IRawElementProviderSimple childElement);

            new ITextRangeProvider? RangeFromPoint(Point screenLocation);

            new ITextRangeProvider? DocumentRange { get; }

            new SupportedTextSelection SupportedTextSelection { get; }

            ITextRangeProvider? RangeFromAnnotation(IRawElementProviderSimple annotation);

            ITextRangeProvider? GetCaretRange(out BOOL isActive);
        }
    }
}
