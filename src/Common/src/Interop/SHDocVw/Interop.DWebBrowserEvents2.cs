// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;

internal partial class Interop
{
    internal partial class SHDocVw
    {
        [ComImport]
        [Guid("34A715A0-6587-11D0-924A-0020AFC7AC4D")]
        [InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
        [TypeLibType(TypeLibTypeFlags.FHidden)]
        public interface DWebBrowserEvents2
        {
            [DispId(102)]
            void StatusTextChange(
                string text);

            [DispId(108)]
            void ProgressChange(
                int progress,
                int progressMax);

            [DispId(105)]
            void CommandStateChange(
                SHDocVw.CSC command,
                bool enable);

            [DispId(106)]
            void DownloadBegin();

            [DispId(104)]
            void DownloadComplete();

            [DispId(113)]
            void TitleChange(
                string text);

            [DispId(112)]
            void PropertyChange(
                string szProperty);

            [DispId(250)]
            void BeforeNavigate2(
                [MarshalAs(UnmanagedType.IDispatch)] object pDisp,
                ref object URL,
                ref object flags,
                ref object targetFrameName,
                ref object postData,
                ref object headers,
                ref bool cancel);

            [DispId(251)]
            void NewWindow2(
                [MarshalAs(UnmanagedType.IDispatch)] ref object pDisp,
                ref bool cancel);

            [DispId(252)]
            void NavigateComplete2(
                [MarshalAs(UnmanagedType.IDispatch)] object pDisp,
                ref object URL);

            [DispId(259)]
            void DocumentComplete(
                [MarshalAs(UnmanagedType.IDispatch)] object pDisp,
                ref object URL);

            [DispId(253)]
            void OnQuit();

            [DispId(254)]
            void OnVisible(
                bool visible);

            [DispId(255)]
            void OnToolBar(
                bool toolBar);

            [DispId(256)]
            void OnMenuBar(
                bool menuBar);

            [DispId(257)]
            void OnStatusBar(
                bool statusBar);

            [DispId(258)]
            void OnFullScreen(
                bool fullScreen);

            [DispId(260)]
            void OnTheaterMode(
                bool theaterMode);

            [DispId(262)]
            void WindowSetResizable(
                bool resizable);

            [DispId(264)]
            void WindowSetLeft(
                int left);

            [DispId(265)]
            void WindowSetTop(
                int top);

            [DispId(266)]
            void WindowSetWidth(
                int width);

            [DispId(267)]
            void WindowSetHeight(
                int height);

            [DispId(263)]
            void WindowClosing(
                bool isChildWindow,
                ref bool cancel);

            [DispId(268)]
            void ClientToHostWindow(
                ref long cx,
                ref long cy);

            [DispId(269)]
            void SetSecureLockIcon(
                int secureLockIcon);

            [DispId(270)]
            void FileDownload(
                ref bool cancel);

            [DispId(271)]
            void NavigateError(
                [MarshalAs(UnmanagedType.IDispatch)] object pDisp,
                ref object URL,
                ref object frame,
                ref object statusCode,
                ref bool cancel);

            [DispId(225)]
            void PrintTemplateInstantiation(
                [MarshalAs(UnmanagedType.IDispatch)] object pDisp);

            [DispId(226)]
            void PrintTemplateTeardown(
                [MarshalAs(UnmanagedType.IDispatch)] object pDisp);

            [DispId(227)]
            void UpdatePageStatus(
                [MarshalAs(UnmanagedType.IDispatch)] object pDisp,
                ref object nPage,
                ref object fDone);

            [DispId(272)]
            void PrivacyImpactedStateChange(bool bImpacted);
        }
    }
}
