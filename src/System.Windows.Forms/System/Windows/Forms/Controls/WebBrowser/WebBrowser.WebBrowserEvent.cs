// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Runtime.InteropServices;
using Windows.Win32.System.Com;
using static Interop;

namespace System.Windows.Forms;

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

        public void CommandStateChange(CommandStateChangeConstants command, bool enable)
        {
            if (command == CommandStateChangeConstants.CSC_NAVIGATEBACK)
            {
                _parent.CanGoBackInternal = enable;
            }
            else if (command == CommandStateChangeConstants.CSC_NAVIGATEFORWARD)
            {
                _parent.CanGoForwardInternal = enable;
            }
        }

        public void BeforeNavigate2(
            object pDisp,
            ref object? urlObject,
            ref object flags,
            ref object? targetFrameName,
            ref object postData,
            ref object? headers,
            ref bool cancel)
        {
            Debug.Assert(_parent is not null, "Parent should have been set");
            // Note: we want to allow navigation if we haven't already navigated.
            if (AllowNavigation || !_haveNavigated)
            {
                Debug.Assert(urlObject is null or string, "invalid url type");
                Debug.Assert(targetFrameName is null or string, "invalid targetFrameName type");
                Debug.Assert(headers is null or string, "invalid headers type");

                // If during running interop code, the variant.bstr value gets set
                // to -1 on return back to native code, if the original value was null, we
                // have to set targetFrameName and headers to string.Empty.
                targetFrameName ??= string.Empty;

                headers ??= string.Empty;

                string urlString = urlObject is null ? string.Empty : (string)urlObject;
                WebBrowserNavigatingEventArgs e = new(
                    new Uri(urlString), targetFrameName is null ? string.Empty : (string)targetFrameName);
                _parent.OnNavigating(e);
                cancel = e.Cancel;
            }
            else
            {
                cancel = true;
            }
        }

        public unsafe void DocumentComplete(object pDisp, ref object? urlObject)
        {
            Debug.Assert(urlObject is null or string, "invalid url");
            _haveNavigated = true;
            if (_parent._documentStreamToSetOnLoad is not null && (string?)urlObject == "about:blank")
            {
                HtmlDocument? htmlDocument = _parent.Document;
                if (htmlDocument is not null)
                {
                    IPersistStreamInit.Interface? psi = htmlDocument.DomDocument as IPersistStreamInit.Interface;
                    Debug.Assert(psi is not null, "The Document does not implement IPersistStreamInit");
                    using var pStream = _parent._documentStreamToSetOnLoad.ToIStream();
                    psi.Load(pStream);
                    htmlDocument.Encoding = "unicode";
                }

                _parent._documentStreamToSetOnLoad = null;
            }
            else
            {
                string urlString = urlObject is null ? string.Empty : urlObject.ToString()!;
                WebBrowserDocumentCompletedEventArgs e = new(
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
            _parent._encryptionLevel = (WebBrowserEncryptionLevel)secureLockIcon;
            _parent.OnEncryptionLevelChanged(EventArgs.Empty);
        }

        public void NavigateComplete2(object pDisp, ref object? urlObject)
        {
            Debug.Assert(urlObject is null or string, "invalid url type");
            string urlString = urlObject is null ? string.Empty : (string)urlObject;
            WebBrowserNavigatedEventArgs e = new(
                    new Uri(urlString));
            _parent.OnNavigated(e);
        }

        public void NewWindow2(ref object ppDisp, ref bool cancel)
        {
            CancelEventArgs e = new();
            _parent.OnNewWindow(e);
            cancel = e.Cancel;
        }

        public void ProgressChange(int progress, int progressMax)
        {
            WebBrowserProgressChangedEventArgs e = new(progress, progressMax);
            _parent.OnProgressChanged(e);
        }

        public void StatusTextChange(string text)
        {
            _parent._statusText = text ?? string.Empty;
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
