// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.InteropServices;

internal static partial class Interop
{
    internal static partial class Shell32
    {
        [ComImport]
        [Guid("e6fdd21a-163f-4975-9c8c-a69f1ba37034")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface IFileDialogCustomize
        {
            [PreserveSig]
            HRESULT EnableOpenDropDown(
                uint dwIDCtl);

            [PreserveSig]
            HRESULT AddMenu(
                uint dwIDCtl,
                [MarshalAs(UnmanagedType.LPWStr)] string pszLabel);

            [PreserveSig]
            HRESULT AddPushButton(
                uint dwIDCtl,
                [MarshalAs(UnmanagedType.LPWStr)] string pszLabel);

            [PreserveSig]
            HRESULT AddComboBox(
                uint dwIDCtl);

            [PreserveSig]
            HRESULT AddRadioButtonList(
                uint dwIDCtl);

            [PreserveSig]
            HRESULT AddCheckButton(
                uint dwIDCtl,
                [MarshalAs(UnmanagedType.LPWStr)] string pszLabel,
                BOOL bChecked);

            [PreserveSig]
            HRESULT AddEditBox(
                uint dwIDCtl,
                [MarshalAs(UnmanagedType.LPWStr)] string pszText);

            [PreserveSig]
            HRESULT AddSeparator(
                uint dwIDCtl);

            void AddText(
                uint dwIDCtl,
                [MarshalAs(UnmanagedType.LPWStr)] string pszText);

            [PreserveSig]
            HRESULT SetControlLabel(
                uint dwIDCtl,
                [MarshalAs(UnmanagedType.LPWStr)] string pszLabel);

            [PreserveSig]
            HRESULT GetControlState(
                uint dwIDCtl,
                out CDCS pdwState);

            [PreserveSig]
            HRESULT SetControlState(
                uint dwIDCtl,
                CDCS dwState);

            [PreserveSig]
            HRESULT GetEditBoxText(
                uint dwIDCtl,
                IntPtr ppszText);

            [PreserveSig]
            HRESULT SetEditBoxText(
                uint dwIDCtl,
                [MarshalAs(UnmanagedType.LPWStr)] string pszText);

            [PreserveSig]
            HRESULT GetCheckButtonState(
                uint dwIDCtl,
                out BOOL pbChecked);

            [PreserveSig]
            HRESULT SetCheckButtonState(
                uint dwIDCtl,
                BOOL bChecked);

            [PreserveSig]
            HRESULT AddControlItem(
                uint dwIDCtl,
                uint dwIDItem,
                [MarshalAs(UnmanagedType.LPWStr)] string pszLabel);

            [PreserveSig]
            HRESULT RemoveControlItem(
                uint dwIDCtl,
                uint dwIDItem);

            [PreserveSig]
            HRESULT RemoveAllControlItems(
                uint dwIDCtl);

            [PreserveSig]
            HRESULT GetControlItemState(
                uint dwIDCtl,
                uint dwIDItem,
                out CDCS pdwState);

            [PreserveSig]
            HRESULT SetControlItemState(
                uint dwIDCtl,
                uint dwIDItem,
                CDCS dwState);

            [PreserveSig]
            HRESULT GetSelectedControlItem(
                uint dwIDCtl,
                out int pdwIDItem);

            [PreserveSig]
            HRESULT SetSelectedControlItem(
                uint dwIDCtl,
                uint dwIDItem);

            [PreserveSig]
            HRESULT StartVisualGroup(
                uint dwIDCtl,
                [MarshalAs(UnmanagedType.LPWStr)] string pszLabel);

            [PreserveSig]
            HRESULT EndVisualGroup();

            [PreserveSig]
            HRESULT MakeProminent(
                uint dwIDCtl);
        }
    }
}
