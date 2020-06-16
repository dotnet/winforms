// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.InteropServices;

internal static partial class Interop
{
    internal static partial class UiaCore
    {
        [ComImport]
        [TypeLibType(0x1050)]
        [Guid("618736E0-3C3D-11CF-810C-00AA00389B71")]
        public interface IAccessibleInternal
        {
            [return: MarshalAs(UnmanagedType.IDispatch)]
            [DispId(unchecked((int)0xFFFFEC78))]
            [TypeLibFunc(0x0040)]
            object? get_accParent();

            [DispId(unchecked((int)0xFFFFEC77))]
            [TypeLibFunc(0x0040)]
            int get_accChildCount();

            [return: MarshalAs(UnmanagedType.IDispatch)]
            [TypeLibFunc(0x0040)]
            [DispId(unchecked((int)0xFFFFEC76))]
            object? get_accChild(
                [MarshalAs(UnmanagedType.Struct)] object varChild);

            [DispId(unchecked((int)0xFFFFEC75))]
            [TypeLibFunc(0x0040)]
            string? get_accName(
                [Optional][MarshalAs(UnmanagedType.Struct)] object varChild);

            [TypeLibFunc(0x0040)]
            [DispId(unchecked((int)0xFFFFEC74))]
            string? get_accValue(
                [Optional][MarshalAs(UnmanagedType.Struct)] object varChild);

            [DispId(unchecked((int)0xFFFFEC73))]
            [TypeLibFunc(0x0040)]
            string? get_accDescription(
                [Optional][MarshalAs(UnmanagedType.Struct)] object varChild);

            [return: MarshalAs(UnmanagedType.Struct)]
            [DispId(unchecked((int)0xFFFFEC72))]
            [TypeLibFunc(0x0040)]
            object? get_accRole(
                [Optional][MarshalAs(UnmanagedType.Struct)] object varChild);

            [return: MarshalAs(UnmanagedType.Struct)]
            [TypeLibFunc(0x0040)]
            [DispId(unchecked((int)0xFFFFEC71))]
            object? get_accState(
                [Optional][MarshalAs(UnmanagedType.Struct)] object varChild);

            [TypeLibFunc(0x0040)]
            [DispId(unchecked((int)0xFFFFEC70))]
            string? get_accHelp(
                [Optional][MarshalAs(UnmanagedType.Struct)] object varChild);

            [DispId(unchecked((int)0xFFFFEC6F))]
            [TypeLibFunc(0x0040)]
            int get_accHelpTopic(
                out string pszHelpFile,
                [Optional][MarshalAs(UnmanagedType.Struct)] object varChild);

            [DispId(unchecked((int)0xFFFFEC6E))]
            [TypeLibFunc(0x0040)]
            string? get_accKeyboardShortcut(
                [Optional][MarshalAs(UnmanagedType.Struct)] object varChild);

            [return: MarshalAs(UnmanagedType.Struct)]
            [DispId(unchecked((int)0xFFFFEC6D))]
            [TypeLibFunc(0x0040)]
            object? get_accFocus();

            [return: MarshalAs(UnmanagedType.Struct)]
            [DispId(unchecked((int)0xFFFFEC6C))]
            [TypeLibFunc(0x0040)]
            object? get_accSelection();

            [return: MarshalAs(UnmanagedType.BStr)]
            [TypeLibFunc(0x0040)]
            [DispId(unchecked((int)0xFFFFEC6B))]
            string? get_accDefaultAction(
                [Optional][MarshalAs(UnmanagedType.Struct)] object varChild);

            [DispId(unchecked((int)0xFFFFEC6A))]
            [TypeLibFunc(0x0040)]
            void accSelect(
                int flagsSelect,
                [Optional][MarshalAs(UnmanagedType.Struct)] object varChild);

            [DispId(unchecked((int)0xFFFFEC69))]
            [TypeLibFunc(0x0040)]
            void accLocation(
                out int pxLeft,
                out int pyTop,
                out int pcxWidth,
                out int pcyHeight,
                [Optional][MarshalAs(UnmanagedType.Struct)] object varChild);

            [return: MarshalAs(UnmanagedType.Struct)]
            [TypeLibFunc(0x0040)]
            [DispId(unchecked((int)0xFFFFEC68))]
            object? accNavigate(
                int navDir,
                [Optional][MarshalAs(UnmanagedType.Struct)] object varStart);

            [return: MarshalAs(UnmanagedType.Struct)]
            [TypeLibFunc(0x0040)]
            [DispId(unchecked((int)0xFFFFEC67))]
            object? accHitTest(
                int xLeft,
                int yTop);

            [TypeLibFunc(0x0040)]
            [DispId(unchecked((int)0xFFFFEC66))]
            void accDoDefaultAction(
                [Optional][MarshalAs(UnmanagedType.Struct)] object varChild);

            [TypeLibFunc(0x0040)]
            [DispId(unchecked((int)0xFFFFEC75))]
            void set_accName(
                [Optional][MarshalAs(UnmanagedType.Struct)] object varChild,
                string pszName);

            [TypeLibFunc(0x0040)]
            [DispId(unchecked((int)0xFFFFEC74))]
            void set_accValue(
                [Optional][MarshalAs(UnmanagedType.Struct)] object varChild,
                string pszValue);
        }
    }
}
