// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.InteropServices;

internal partial class Interop
{
    internal static partial class Richedit
    {
        [ComImport]
        [Guid("8CC497C0-A1DF-11ce-8098-00AA0047BE5D")]
        [InterfaceType(ComInterfaceType.InterfaceIsDual)]
        public interface ITextDocument
        {
            string GetName();

            object GetSelection();

            int GetStoryCount();

            object GetStoryRanges();

            int GetSaved();

            void SetSaved(
                int value);

            object GetDefaultTabStop();

            void SetDefaultTabStop(
                object value);

            void New();

            void Open(
                object pVar,
                int flags,
                int codePage);

            void Save(
                object pVar,
                int flags,
                int codePage);

            int Freeze();

            int Unfreeze();

            void BeginEditCollection();

            void EndEditCollection();

            int Undo(
                int count);

            int Redo(
                int count);

            ITextRange Range(
                int cp1,
                int cp2);

            ITextRange RangeFromPoint(
                int x,
                int y);
        }
    }
}
