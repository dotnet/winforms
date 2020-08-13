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
        [Guid("8CC497C2-A1DF-11ce-8098-00AA0047BE5D")]
        [InterfaceType(ComInterfaceType.InterfaceIsDual)]
        public interface ITextRange
        {
            string GetText();

            void SetText(
                string text);

            object GetChar();

            void SetChar(
                object ch);

            ITextRange GetDuplicate();

            ITextRange GetFormattedText();

            void SetFormattedText(
                ITextRange range);

            int GetStart();

            void SetStart(
                int cpFirst);

            int GetEnd();

            void SetEnd(
                int cpLim);

            object GetFont();

            void SetFont(
                object font);

            object GetPara();

            void SetPara(
                object para);

            int GetStoryLength();

            int GetStoryType();

            void Collapse(
                int start);

            int Expand(
                int unit);

            int GetIndex(
                int unit);

            void SetIndex(
                int unit,
                int index,
                int extend);

            void SetRange(
                int cpActive,
                int cpOther);

            int InRange(
                ITextRange range);

            int InStory(
                ITextRange range);

            int IsEqual(
                ITextRange range);

            void Select();

            int StartOf(
                int unit,
                int extend);

            int EndOf(
                int unit,
                int extend);

            int Move(
                int unit,
                int count);

            int MoveStart(
                int unit,
                int count);

            int MoveEnd(
                int unit,
                int count);

            int MoveWhile(
                object cset,
                int count);

            int MoveStartWhile(
                object cset,
                int count);

            int MoveEndWhile(
                object cset,
                int count);

            int MoveUntil(
                object cset,
                int count);

            int MoveStartUntil(
                object cset,
                int count);

            int MoveEndUntil(
                object cset,
                int count);

            int FindText(
                string text,
                int cch,
                int flags);

            int FindTextStart(
                string text,
                int cch,
                int flags);

            int FindTextEnd(
                string text,
                int cch,
                int flags);

            int Delete(
                int unit,
                int count);

            void Cut(
                out object pVar);

            void Copy(
                out object pVar);

            void Paste(
                object pVar,
                int format);

            int CanPaste(
                object pVar,
                int format);

            int CanEdit();

            void ChangeCase(
                int type);

            void GetPoint(
                int type,
                out int x,
                out int y);

            void SetPoint(
                int x,
                int y,
                int type,
                int extend);

            void ScrollIntoView(
                int value);

            object GetEmbeddedObject();
        }
    }
}
