﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using static Interop;

namespace System.Windows.Forms
{
    public partial class WebBrowser
    {
        [ClassInterface(ClassInterfaceType.None)]
        private class WebBrowserEvent : StandardOleMarshalObject, SHDocVw.DWebBrowserEvents2
        {
            private readonly WebBrowser _parent;
            private bool _haveNavigated;

            public WebBrowserEvent(WebBrowser parent)
            {
                _parent = parent;
            }

            public bool AllowNavigation { get; set; }

            public void CommandStateChange(SHDocVw.CSC command, bool enable)
            {
                if (command == SHDocVw.CSC.NAVIGATEBACK)
                {
                    _parent.CanGoBackInternal = enable;
                }
                else if (command == SHDocVw.CSC.NAVIGATEFORWARD)
                {
                    _parent.CanGoForwardInternal = enable;
                }
            }

            public void BeforeNavigate2(object pDisp, ref object urlObject, ref object flags, ref object targetFrameName, ref object postData, ref object headers, ref bool cancel)
            {
                Debug.Assert(_parent is not null, "Parent should have been set");
                //Note: we want to allow navigation if we haven't already navigated.
                if (AllowNavigation || !_haveNavigated)
                {
                    Debug.Assert(urlObject is null || urlObject is string, "invalid url type");
                    Debug.Assert(targetFrameName is null || targetFrameName is string, "invalid targetFrameName type");
                    Debug.Assert(headers is null || headers is string, "invalid headers type");
                    //
                    // If during running interop code, the variant.bstr value gets set
                    // to -1 on return back to native code, if the original value was null, we
                    // have to set targetFrameName and headers to string.Empty.
                    if (targetFrameName is null)
                    {
                        targetFrameName = string.Empty;
                    }

                    if (headers is null)
                    {
                        headers = string.Empty;
                    }

                    string urlString = urlObject is null ? string.Empty : (string)urlObject;
                    WebBrowserNavigatingEventArgs e = new WebBrowserNavigatingEventArgs(
                        new Uri(urlString), targetFrameName is null ? string.Empty : (string)targetFrameName);
                    _parent.OnNavigating(e);
                    cancel = e.Cancel;
                }
                else
                {
                    cancel = true;
                }
            }

            public void DocumentComplete(object pDisp, ref object urlObject)
            {
                Debug.Assert(urlObject is null || urlObject is string, "invalid url");
                _haveNavigated = true;
                if (_parent.documentStreamToSetOnLoad is not null && (string)urlObject == "about:blank")
                {
                    HtmlDocument htmlDocument = _parent.Document;
                    if (htmlDocument is not null)
                    {
                        Ole32.IPersistStreamInit psi = htmlDocument.DomDocument as Ole32.IPersistStreamInit;
                        Debug.Assert(psi is not null, "The Document does not implement IPersistStreamInit");
                        Ole32.IStream iStream = (Ole32.IStream)new Ole32.GPStream(
                                                    _parent.documentStreamToSetOnLoad);
                        psi.Load(iStream);
                        htmlDocument.Encoding = "unicode";
                    }

                    _parent.documentStreamToSetOnLoad = null;
                }
                else
                {
                    string urlString = urlObject is null ? string.Empty : urlObject.ToString();
                    WebBrowserDocumentCompletedEventArgs e = new WebBrowserDocumentCompletedEventArgs(
                            new Uri(urlString));
                    _parent.OnDocumentCompleted(e);
                }
            }

            public void TitleChange(string text)
            {
                _parent.OnDocumentTitleChanged(EventArgs.Empty);
            }

            public void SetSecureLockIcon(int secureLockIcon)
            {
                _parent.encryptionLevel = (WebBrowserEncryptionLevel)secureLockIcon;
                _parent.OnEncryptionLevelChanged(EventArgs.Empty);
            }

            public void NavigateComplete2(object pDisp, ref object urlObject)
            {
                Debug.Assert(urlObject is null || urlObject is string, "invalid url type");
                string urlString = urlObject is null ? string.Empty : (string)urlObject;
                WebBrowserNavigatedEventArgs e = new WebBrowserNavigatedEventArgs(
                        new Uri(urlString));
                _parent.OnNavigated(e);
            }

            public void NewWindow2(ref object ppDisp, ref bool cancel)
            {
                CancelEventArgs e = new CancelEventArgs();
                _parent.OnNewWindow(e);
                cancel = e.Cancel;
            }

            public void ProgressChange(int progress, int progressMax)
            {
                WebBrowserProgressChangedEventArgs e = new WebBrowserProgressChangedEventArgs(progress, progressMax);
                _parent.OnProgressChanged(e);
            }

            public void StatusTextChange(string text)
            {
                _parent.statusText = text ?? string.Empty;
                _parent.OnStatusTextChanged(EventArgs.Empty);
            }

            public void DownloadBegin() => _parent.OnFileDownload(EventArgs.Empty);

            public void FileDownload(ref bool cancel)
            {
            }

            public void PrivacyImpactedStateChange(bool bImpacted)
            {
            }

            public void UpdatePageStatus(object pDisp, ref object nPage, ref object fDone)
            {
            }

            public void PrintTemplateTeardown(object pDisp)
            {
            }

            public void PrintTemplateInstantiation(object pDisp)
            {
            }

            public void NavigateError(object pDisp, ref object url, ref object frame, ref object statusCode, ref bool cancel)
            {
            }

            public void ClientToHostWindow(ref long cX, ref long cY)
            {
            }

            public void WindowClosing(bool isChildWindow, ref bool cancel)
            {
            }

            public void WindowSetHeight(int height)
            {
            }

            public void WindowSetWidth(int width)
            {
            }

            public void WindowSetTop(int top)
            {
            }

            public void WindowSetLeft(int left)
            {
            }

            public void WindowSetResizable(bool resizable)
            {
            }

            public void OnTheaterMode(bool theaterMode)
            {
            }

            public void OnFullScreen(bool fullScreen)
            {
            }

            public void OnStatusBar(bool statusBar)
            {
            }

            public void OnMenuBar(bool menuBar)
            {
            }

            public void OnToolBar(bool toolBar)
            {
            }

            public void OnVisible(bool visible)
            {
            }

            public void OnQuit()
            {
            }

            public void PropertyChange(string szProperty)
            {
            }

            public void DownloadComplete()
            {
            }
        }
    }
}
