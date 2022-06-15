// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;

internal static partial class Interop
{
    internal static partial class UiaCore
    {
        [ComImport]
        [Guid("5347ad7b-c355-46f8-aff5-909033582f63")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface ITextRangeProvider
        {
            ITextRangeProvider Clone();

            BOOL Compare(ITextRangeProvider range);

            int CompareEndpoints(TextPatternRangeEndpoint endpoint, ITextRangeProvider targetRange, TextPatternRangeEndpoint targetEndpoint);

            void ExpandToEnclosingUnit(TextUnit unit);

            ITextRangeProvider? FindAttribute(int attribute, object value, BOOL backward);

            ITextRangeProvider? FindText(string text, BOOL backward, BOOL ignoreCase);

            object? GetAttributeValue(int attribute);

            double[] GetBoundingRectangles();

            IRawElementProviderSimple GetEnclosingElement();

            string GetText(int maxLength);

            int Move(TextUnit unit, int count);

            int MoveEndpointByUnit(TextPatternRangeEndpoint endpoint, TextUnit unit, int count);

            void MoveEndpointByRange(TextPatternRangeEndpoint endpoint, ITextRangeProvider targetRange, TextPatternRangeEndpoint targetEndpoint);

            void Select();

            void AddToSelection();

            void RemoveFromSelection();

            void ScrollIntoView(BOOL alignToTop);

            IRawElementProviderSimple[] GetChildren();
        }
    }
}
