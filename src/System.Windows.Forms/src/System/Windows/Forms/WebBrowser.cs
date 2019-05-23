// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Drawing;
using System.Diagnostics.CodeAnalysis;
using System.Drawing.Printing;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Net;
using System.Text;

using IComDataObject = System.Runtime.InteropServices.ComTypes.IDataObject;

namespace System.Windows.Forms
{
    /// <summary>
    ///     <para>
    /// This is a wrapper over the native WebBrowser control implemented in shdocvw.dll.
    ///     </para>
    /// </summary>
    [ComVisible(true),
    ClassInterface(ClassInterfaceType.AutoDispatch),
    DefaultProperty(nameof(Url)), DefaultEvent(nameof(DocumentCompleted)),
    Docking(DockingBehavior.AutoDock),
    SRDescription(nameof(SR.DescriptionWebBrowser)),
    Designer("System.Windows.Forms.Design.WebBrowserDesigner, " + AssemblyRef.SystemDesign)]
    public class WebBrowser : WebBrowserBase
    {
        private static bool createdInIE;

        // Reference to the native ActiveX control's IWebBrowser2
        // Do not reference this directly. Use the AxIWebBrowser2
        // property instead.
        private UnsafeNativeMethods.IWebBrowser2 axIWebBrowser2;

        private AxHost.ConnectionPointCookie cookie;   // To hook up events from the native WebBrowser
        private Stream documentStreamToSetOnLoad;
        private WebBrowserEncryptionLevel encryptionLevel = WebBrowserEncryptionLevel.Insecure;
        private object objectForScripting;
        private WebBrowserEvent webBrowserEvent;
        internal string statusText = string.Empty;


        private const int WEBBROWSERSTATE_webBrowserShortcutsEnabled = 0x00000001;
        private const int WEBBROWSERSTATE_documentStreamJustSet = 0x00000002;
        private const int WEBBROWSERSTATE_isWebBrowserContextMenuEnabled = 0x00000004;
        private const int WEBBROWSERSTATE_canGoBack = 0x00000008;
        private const int WEBBROWSERSTATE_canGoForward = 0x00000010;
        private const int WEBBROWSERSTATE_scrollbarsEnabled = 0x00000020;
        private const int WEBBROWSERSTATE_allowNavigation = 0x00000040;

        // PERF: take all the bools and put them into a state variable
        private System.Collections.Specialized.BitVector32 webBrowserState;          // see TREEVIEWSTATE_ consts above


        //
        // 8856f961-340a-11d0-a96b-00c04fd705a2 is the clsid for the native webbrowser control
        //
        /// <summary>
        ///     <para>
        /// Creates an instance of the <see cref='System.Windows.Forms.WebBrowser'/> control.
        ///     </para>
        /// </summary>
        public WebBrowser() : base("8856f961-340a-11d0-a96b-00c04fd705a2")
        {
            CheckIfCreatedInIE();

            webBrowserState = new System.Collections.Specialized.BitVector32(WEBBROWSERSTATE_isWebBrowserContextMenuEnabled |
                    WEBBROWSERSTATE_webBrowserShortcutsEnabled | WEBBROWSERSTATE_scrollbarsEnabled);
            AllowNavigation = true;
        }


        //
        // Public properties:
        //


        /// <summary>
        ///     <para>
        /// Specifies whether the WebBrowser control may navigate to another page once 
        /// it has been loaded.  NOTE: it will always be able to navigate before being loaded.
        /// "Loaded" here means setting Url, DocumentText, or DocumentStream.
        ///     </para>
        /// </summary>
        [SRDescription(nameof(SR.WebBrowserAllowNavigationDescr)),
        SRCategory(nameof(SR.CatBehavior)), DefaultValue(true)]
        public bool AllowNavigation
        {
            get
            {
                return webBrowserState[WEBBROWSERSTATE_allowNavigation];
            }
            set
            {
                webBrowserState[WEBBROWSERSTATE_allowNavigation] = value;
                if (webBrowserEvent != null)
                {
                    webBrowserEvent.AllowNavigation = value;
                }
            }
        }

        /// <summary>
        ///     <para>
        /// Specifies whether the WebBrowser control will receive drop notifcations.
        /// Maps to IWebBrowser2:RegisterAsDropTarget.
        /// Note that this does not mean that the WebBrowser control integrates with
        /// Windows Forms drag/drop i.e. the DragDrop event does not fire.  It does
        /// control whether you can drag new documents into the browser control.
        ///     </para>
        /// </summary>
        [SRDescription(nameof(SR.WebBrowserAllowWebBrowserDropDescr)),
        SRCategory(nameof(SR.CatBehavior)), DefaultValue(true)]
        public bool AllowWebBrowserDrop
        {
            get
            {
                return this.AxIWebBrowser2.RegisterAsDropTarget;
            }
            set
            {
                //Note: you lose this value when you load a new document: the value needs to be refreshed in
                //OnDocumentCompleted.
                if (value != AllowWebBrowserDrop)
                {
                    this.AxIWebBrowser2.RegisterAsDropTarget = value;
                    this.Refresh();
                }
            }
        }

        /// <summary>
        ///     <para>
        /// Specifies whether the browser control shows script errors in dialogs or not.
        /// Maps to IWebBrowser2:Silent.
        ///     </para>
        /// </summary>
        [SRDescription(nameof(SR.WebBrowserScriptErrorsSuppressedDescr)),
        SRCategory(nameof(SR.CatBehavior)), DefaultValue(false)]
        public bool ScriptErrorsSuppressed
        {
            get
            {
                return this.AxIWebBrowser2.Silent;
            }
            set
            {
                if (value != ScriptErrorsSuppressed)
                {
                    this.AxIWebBrowser2.Silent = value;
                }
            }
        }

        /// <summary>
        ///     <para>
        /// Specifies whether the browser control Shortcuts are enabled.
        /// Maps to IDocHostUIHandler:TranslateAccelerator event.
        ///     </para>
        /// </summary>
        [SRDescription(nameof(SR.WebBrowserWebBrowserShortcutsEnabledDescr)),
        SRCategory(nameof(SR.CatBehavior)), DefaultValue(true)]
        public bool WebBrowserShortcutsEnabled
        {
            get
            {
                return webBrowserState[WEBBROWSERSTATE_webBrowserShortcutsEnabled];
            }
            set
            {
                webBrowserState[WEBBROWSERSTATE_webBrowserShortcutsEnabled] = value;
            }
        }

        /// <summary>
        ///     <para>
        /// If true, there is navigation history such that calling GoBack() will succeed.
        /// Defaults to false.  After that it's value is kept up to date by hooking the
        /// DWebBrowserEvents2:CommandStateChange.
        ///     </para>
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool CanGoBack
        {
            get
            {
                return CanGoBackInternal;
            }
        }

        /// <summary>
        /// Returns the current WEBBROWSERSTATE_canGoBack value so that this value can be accessed
        /// from child classes.
        /// </summary>
        internal bool CanGoBackInternal
        {
            get
            {
                return webBrowserState[WEBBROWSERSTATE_canGoBack];
            }
            set
            {
                if (value != CanGoBackInternal)
                {
                    webBrowserState[WEBBROWSERSTATE_canGoBack] = value;
                    OnCanGoBackChanged(EventArgs.Empty);
                }
            }
        }

        /// <summary>
        ///     <para>
        /// If true, there is navigation history such that calling GoForward() will succeed.
        /// Defaults to false.  After that it's value is kept up to date by hooking the
        /// DWebBrowserEvents2:CommandStateChange.
        ///     </para>
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool CanGoForward
        {
            get
            {
                return CanGoForwardInternal;
            }
        }

        /// <summary>
        /// Returns the current WEBBROWSERSTATE_canGoForward value so that this value can
        /// be accessed from child classes.
        /// </summary>
        internal bool CanGoForwardInternal
        {
            get
            {
                return webBrowserState[WEBBROWSERSTATE_canGoForward];
            }
            set
            {
                if (value != CanGoForwardInternal)
                {
                    webBrowserState[WEBBROWSERSTATE_canGoForward] = value;
                    OnCanGoForwardChanged(EventArgs.Empty);
                }
            }
        }

        /// <summary>
        ///     <para>
        /// The HtmlDocument for page hosted in the html page.  If no page is loaded, it returns null.
        /// Maps to IWebBrowser2:Document.
        ///     </para>
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public HtmlDocument Document
        {
            get
            {
                object objDoc = this.AxIWebBrowser2.Document;
                if (objDoc != null)
                {
                    // Document is not necessarily an IHTMLDocument, it might be an office document as well.
                    UnsafeNativeMethods.IHTMLDocument2 iHTMLDocument2 = null;
                    try
                    {
                        iHTMLDocument2 = objDoc as UnsafeNativeMethods.IHTMLDocument2;
                    }
                    catch (InvalidCastException)
                    {
                    }
                    if (iHTMLDocument2 != null)
                    {
                        UnsafeNativeMethods.IHTMLLocation iHTMLLocation = iHTMLDocument2.GetLocation();
                        if (iHTMLLocation != null)
                        {
                            string href = iHTMLLocation.GetHref();
                            if (!string.IsNullOrEmpty(href))
                            {
                                Uri url = new Uri(href);
                                return new HtmlDocument(ShimManager, iHTMLDocument2 as UnsafeNativeMethods.IHTMLDocument);
                            }
                        }
                    }
                }
                return null;
            }
        }

        /// <summary>
        ///     <para>
        /// Get/sets the stream for the html document.
        /// Uses the IPersisteStreamInit interface on the HtmlDocument to set/retrieve the html stream.
        ///     </para>
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Stream DocumentStream
        {
            get
            {
                HtmlDocument htmlDocument = this.Document;
                if (htmlDocument == null)
                {
                    return null;
                }
                else
                {
                    UnsafeNativeMethods.IPersistStreamInit psi = htmlDocument.DomDocument as UnsafeNativeMethods.IPersistStreamInit;
                    Debug.Assert(psi != null, "Object isn't an IPersistStreamInit!");
                    if (psi == null)
                    {
                        return null;
                    }
                    else
                    {
                        MemoryStream memoryStream = new MemoryStream();
                        UnsafeNativeMethods.IStream iStream = (UnsafeNativeMethods.IStream)new UnsafeNativeMethods.ComStreamFromDataStream(memoryStream);
                        psi.Save(iStream, false);
                        return new MemoryStream(memoryStream.GetBuffer(), 0, (int)memoryStream.Length, false);
                    }
                }
            }
            set
            {
                this.documentStreamToSetOnLoad = value;
                try
                {
                    webBrowserState[WEBBROWSERSTATE_documentStreamJustSet] = true;
                    // Lets navigate to "about:blank" so that we get a "clean" document
                    this.Url = new Uri("about:blank");
                }
                finally
                {
                    webBrowserState[WEBBROWSERSTATE_documentStreamJustSet] = false;
                }
            }
        }

        /// <summary>
        ///     <para>
        /// Sets/sets the text of the contained html page.
        ///     </para>
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string DocumentText
        {
            get
            {
                Stream stream = this.DocumentStream;
                if (stream == null)
                {
                    return "";
                }
                StreamReader reader = new StreamReader(stream);
                stream.Position = 0;
                return reader.ReadToEnd();
            }
            set
            {
                if (value == null)
                {
                    value = string.Empty;
                }
                //string length is a good initial guess for capacity -- 
                //if it needs more room, it'll take it.
                MemoryStream ms = new MemoryStream(value.Length);
                StreamWriter sw = new StreamWriter(ms, Encoding.UTF8);
                sw.Write(value);
                sw.Flush();
                ms.Position = 0;
                this.DocumentStream = ms;
            }
        }

        /// <summary>
        ///     <para>
        /// The title of the html page currently loaded. If none are loaded, returns empty string.
        /// Maps to IWebBrowser2:LocationName.
        ///     </para>
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string DocumentTitle
        {
            get
            {
                string documentTitle;

                HtmlDocument htmlDocument = this.Document;
                if (htmlDocument == null)
                {
                    documentTitle = this.AxIWebBrowser2.LocationName;
                }
                else
                {
                    UnsafeNativeMethods.IHTMLDocument2 htmlDocument2 = htmlDocument.DomDocument as UnsafeNativeMethods.IHTMLDocument2;
                    Debug.Assert(htmlDocument2 != null, "The HtmlDocument object must implement IHTMLDocument2.");
                    try
                    {
                        documentTitle = htmlDocument2.GetTitle();
                    }
                    catch (COMException)
                    {
                        documentTitle = string.Empty;
                    }
                }
                return documentTitle;
            }
        }

        /// <summary>
        ///     <para>
        /// A string containing the MIME type of the document hosted in the browser control.
        /// If none are loaded, returns empty string.  Maps to IHTMLDocument2:mimeType.
        ///     </para>
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string DocumentType
        {
            get
            {
                string docType = string.Empty;
                HtmlDocument htmlDocument = this.Document;
                if (htmlDocument != null)
                {
                    UnsafeNativeMethods.IHTMLDocument2 htmlDocument2 = htmlDocument.DomDocument as UnsafeNativeMethods.IHTMLDocument2;
                    Debug.Assert(htmlDocument2 != null, "The HtmlDocument object must implement IHTMLDocument2.");
                    try
                    {
                        docType = htmlDocument2.GetMimeType();
                    }
                    catch (COMException)
                    {
                        docType = string.Empty;
                    }
                }
                return docType;
            }
        }

        /// <summary>
        ///     <para>
        /// Initially set to WebBrowserEncryptionLevel.Insecure.
        /// After that it's kept up to date by hooking the DWebBrowserEvents2:SetSecureLockIcon.
        ///     </para>
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public WebBrowserEncryptionLevel EncryptionLevel
        {
            get
            {
                if (this.Document == null)
                {
                    encryptionLevel = WebBrowserEncryptionLevel.Unknown;
                }
                return encryptionLevel;
            }
        }

        /// <summary>
        ///     <para>
        /// True if the browser is engaged in navigation or download.  Maps to IWebBrowser2:Busy.
        ///     </para>
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsBusy
        {
            get
            {
                if (this.Document == null)
                {
                    return false;
                }
                else
                {
                    return this.AxIWebBrowser2.Busy;
                }
            }
        }

        /// <summary>
        ///     <para>
        /// Gets the offline state of the browser control. Maps to IWebBrowser2:Offline.
        ///     </para>
        /// </summary>
        [SRDescription(nameof(SR.WebBrowserIsOfflineDescr)), Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsOffline
        {
            get
            {
                return this.AxIWebBrowser2.Offline;
            }
        }

        /// <summary>
        ///     <para>
        /// Indicates whether to use the WebBrowser context menu.
        /// It's technically possible to have both the WebBrowser & Windows Forms context
        /// menu enabled, but making this property effect the behavior of the Windows Form
        /// context menu does not lead to a clean OM.  Maps to sinking the
        /// IDocHostUIHandler:ShowContextMenu 
        ///     </para>
        /// </summary>
        [SRDescription(nameof(SR.WebBrowserIsWebBrowserContextMenuEnabledDescr)),
        SRCategory(nameof(SR.CatBehavior)), DefaultValue(true)]
        public bool IsWebBrowserContextMenuEnabled
        {
            get
            {
                return webBrowserState[WEBBROWSERSTATE_isWebBrowserContextMenuEnabled];
            }
            set
            {
                webBrowserState[WEBBROWSERSTATE_isWebBrowserContextMenuEnabled] = value;
            }
        }

        /// <summary>
        ///     <para>
        /// Allows the host application to provide an object that the contained html
        /// pages can access programatically in script.  The object specified here
        /// will be accessible in script as the "window.external" object via IDispatch
        /// COM interop. Maps to an implementation of the IDocUIHandler.GetExternal event.
        ///     </para>
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public object ObjectForScripting
        {
            get
            {
                return objectForScripting;
            }
            set
            {
                if (value != null)
                {
                    Type t = value.GetType();
#if Marshal_IsTypeVisibleFromCom
                    if (!Marshal.IsTypeVisibleFromCom(t))
                    {
                        throw new ArgumentException(SR.WebBrowserObjectForScriptingComVisibleOnly);
                    }
#endif
                }
                objectForScripting = value;
            }
        }

        [
        Browsable(false),
        EditorBrowsable(EditorBrowsableState.Never),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
        ]
        public new Padding Padding
        {
            get { return base.Padding; }
            set { base.Padding = value; }
        }

        [
        Browsable(false),
        EditorBrowsable(EditorBrowsableState.Never),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        SRCategory(nameof(SR.CatLayout)), SRDescription(nameof(SR.ControlOnPaddingChangedDescr))
        ]
        public new event EventHandler PaddingChanged
        {
            add => base.PaddingChanged += value;
            remove => base.PaddingChanged -= value;
        }

        /// <summary>
        ///     <para>
        /// Gets the ReadyState of the browser control. (ex.. document loading vs. load complete).
        /// Maps to IWebBrowser2:ReadyState.
        ///     </para>
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public WebBrowserReadyState ReadyState
        {
            get
            {
                if (this.Document == null)
                {
                    return WebBrowserReadyState.Uninitialized;
                }
                else
                {
                    return (WebBrowserReadyState)this.AxIWebBrowser2.ReadyState;
                }
            }
        }

        /// <summary>
        ///     <para>
        /// The text that would be displayed in the IE status bar.
        /// There is no direct WebBrowser property that maps to this. This property is
        /// initially an empty string.  After that the value is kept up to date via the
        /// DWebBrowserEvents2:StatusTextChange event.  
        ///     </para>
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public virtual string StatusText
        {
            get
            {
                if (this.Document == null)
                {
                    statusText = string.Empty;
                }
                return statusText;
            }
        }

        /// <summary>
        ///     <para>
        /// The url of the HtmlDocument for page hosted in the html page.
        /// Get Maps to IWebBrowser2:LocationUrl.  Set is the equivalent of calling Navigate(Url).
        /// Note this means that setting the Url property & then reading it immediately may not
        /// return the result that you just set (since the get always returns the url you are currently at).
        ///     </para>
        /// </summary>
        [
            SRDescription(nameof(SR.WebBrowserUrlDescr)),
            Bindable(true),
            SRCategory(nameof(SR.CatBehavior)),
            TypeConverter(typeof(System.Windows.Forms.WebBrowserUriTypeConverter)),
            DefaultValue(null)
        ]
        public Uri Url
        {
            get
            {
                string urlString = this.AxIWebBrowser2.LocationURL;

                if (string.IsNullOrEmpty(urlString))
                {
                    return null;
                }
                try
                {
                    return new Uri(urlString);
                }
                catch (UriFormatException)
                {
                    return null;
                }
            }
            set
            {
                if (value != null && value.ToString() == "")
                {
                    value = null;
                }
                this.PerformNavigateHelper(ReadyNavigateToUrl(value), false, null, null, null);
            }
        }

        /// <summary>
        ///     <para>
        /// Returns the version property of IE.
        /// Determined by reading the file version of mshtml.dll in the %system% directory.
        ///     </para>
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Version Version
        {
            get
            {
                string mshtmlPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), "mshtml.dll");
                FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(mshtmlPath);
                return new Version(fvi.FileMajorPart, fvi.FileMinorPart, fvi.FileBuildPart, fvi.FilePrivatePart);
            }
        }



        //
        // Public methods:
        //

        /// <summary>
        ///     <para>
        /// Navigates the browser to the previous page in the navigation history list.
        /// Maps to IWebBrowser2:GoBack.
        /// Returns true if the operation succeeds, else returns false.  It will return
        /// false if there is no page in the navigation history to go back to.
        ///     </para>
        /// </summary>
        public bool GoBack()
        {
            bool retVal = true;
            try
            {
                this.AxIWebBrowser2.GoBack();
            }
            catch (Exception ex)
            {
                if (ClientUtils.IsSecurityOrCriticalException(ex))
                {
                    throw;
                }
                retVal = false;
            }
            return retVal;
        }

        /// <summary>
        ///     <para>
        /// Navigates the browser to the next page in the navigation history list.
        /// Maps to IWebBrowser2:GoForward.
        /// Returns true if the operation succeeds, else returns false.  It will return
        /// false if there is no page in the navigation history to go forward to.
        ///     </para>
        /// </summary>
        public bool GoForward()
        {
            bool retVal = true;
            try
            {
                this.AxIWebBrowser2.GoForward();
            }
            catch (Exception ex)
            {
                if (ClientUtils.IsSecurityOrCriticalException(ex))
                {
                    throw;
                }
                retVal = false;
            }
            return retVal;
        }

        /// <summary>
        ///     <para>
        /// Navigates the browser to user's homepage.  Maps to IWebBrowser2:GoHome.
        ///     </para>
        /// </summary>
        public void GoHome()
        {
            this.AxIWebBrowser2.GoHome();
        }

        /// <summary>
        ///     <para>
        /// Navigates the browser to user's default search page.  Maps to IWebBrowser2:GoSearch.
        ///     </para>
        /// </summary>
        public void GoSearch()
        {
            this.AxIWebBrowser2.GoSearch();
        }

        /// <summary>
        ///     <para>
        /// Navigates to the specified Uri's AbsolutePath
        ///     </para>
        /// </summary>
        public void Navigate(Uri url)
        {
            Url = url; // Does null check in PerformNavigate2
        }

        /// <summary>
        ///     <para>
        /// String overload for Navigate(Uri)
        ///     </para>
        /// </summary>
        /// Note: We intentionally have a string overload (apparently Mort wants one).  We don't have 
        /// string overloads call Uri overloads because that breaks Uris that aren't fully qualified 
        /// (things like "www.microsoft.com") that the underlying objects support and we don't want to 
        /// break.
        [SuppressMessage("Microsoft.Design", "CA1057:StringUriOverloadsCallSystemUriOverloads"),
            SuppressMessage("Microsoft.Usage", "CA2234:PassSystemUriObjectsInsteadOfStrings")]
        public void Navigate(string urlString)
        {
            PerformNavigateHelper(ReadyNavigateToUrl(urlString), false, null, null, null);
        }

        /// <summary>
        ///     <para>
        /// Navigates the specified frame to the specified URL.
        /// If the frame name is invalid, it opens a new window (not ideal, but it's the current behavior).
        /// Maps to IWebBrowser2:Navigate.
        ///     </para>
        /// </summary>
        public void Navigate(Uri url, string targetFrameName)
        {
            PerformNavigateHelper(ReadyNavigateToUrl(url), false, targetFrameName, null, null);
        }

        /// <summary>
        ///     <para>
        /// String overload for Navigate(Uri, string)
        ///     </para>
        /// </summary>
        /// Note: We intentionally have a string overload (apparently Mort wants one).  We don't have 
        /// string overloads call Uri overloads because that breaks Uris that aren't fully qualified 
        /// (things like "www.microsoft.com") that the underlying objects support and we don't want to 
        /// break.
        [SuppressMessage("Microsoft.Design", "CA1057:StringUriOverloadsCallSystemUriOverloads"),
            SuppressMessage("Microsoft.Usage", "CA2234:PassSystemUriObjectsInsteadOfStrings")]
        public void Navigate(string urlString, string targetFrameName)
        {
            PerformNavigateHelper(ReadyNavigateToUrl(urlString), false, targetFrameName, null, null);
        }

        /// <summary>
        ///     <para>
        /// Opens a new window if newWindow is true, navigating it to the specified URL. Maps to IWebBrowser2:Navigate.
        ///     </para>
        /// </summary>
        public void Navigate(Uri url, bool newWindow)
        {
            PerformNavigateHelper(ReadyNavigateToUrl(url), newWindow, null, null, null);
        }

        /// <summary>
        ///     <para>
        /// String overload for Navigate(Uri, bool)
        ///     </para>
        /// </summary>
        /// Note: We intentionally have a string overload (apparently Mort wants one).  We don't have 
        /// string overloads call Uri overloads because that breaks Uris that aren't fully qualified 
        /// (things like "www.microsoft.com") that the underlying objects support and we don't want to 
        /// break.
        [SuppressMessage("Microsoft.Design", "CA1057:StringUriOverloadsCallSystemUriOverloads"),
            SuppressMessage("Microsoft.Usage", "CA2234:PassSystemUriObjectsInsteadOfStrings")]
        public void Navigate(string urlString, bool newWindow)
        {
            PerformNavigateHelper(ReadyNavigateToUrl(urlString), newWindow, null, null, null);
        }

        /// <summary>
        ///     <para>
        /// Navigates to the specified Uri's AbsolutePath with specified args
        ///     </para>
        /// </summary>
        public void Navigate(Uri url, string targetFrameName, byte[] postData, string additionalHeaders)
        {
            PerformNavigateHelper(ReadyNavigateToUrl(url), false, targetFrameName, postData, additionalHeaders);
        }

        /// <summary>
        ///     <para>
        /// String overload for Navigate(Uri, string, byte[], string)
        ///     </para>
        /// </summary>
        /// Note: We intentionally have a string overload (apparently Mort wants one).  We don't have 
        /// string overloads call Uri overloads because that breaks Uris that aren't fully qualified 
        /// (things like "www.microsoft.com") that the underlying objects support and we don't want to 
        /// break.
        [SuppressMessage("Microsoft.Design", "CA1057:StringUriOverloadsCallSystemUriOverloads"),
            SuppressMessage("Microsoft.Usage", "CA2234:PassSystemUriObjectsInsteadOfStrings")]
        public void Navigate(string urlString, string targetFrameName, byte[] postData, string additionalHeaders)
        {
            PerformNavigateHelper(ReadyNavigateToUrl(urlString), false, targetFrameName, postData, additionalHeaders);
        }

        /// <summary>
        ///     <para>
        /// Prints the html document to the default printer w/ no print dialog.
        /// Maps to IWebBrowser2:ExecWB w/ IDM_PRINT flag & LECMDEXECOPT_DONTPROMPTUSER.
        ///     </para>
        /// </summary>
        public void Print()
        {
            object nullObjectArray = null;
            try
            {
                this.AxIWebBrowser2.ExecWB(NativeMethods.OLECMDID.OLECMDID_PRINT, NativeMethods.OLECMDEXECOPT.OLECMDEXECOPT_DONTPROMPTUSER, ref nullObjectArray, IntPtr.Zero);
            }
            catch (Exception ex)
            {
                if (ClientUtils.IsSecurityOrCriticalException(ex))
                {
                    throw;
                }
            }
        }

        /// <summary>
        ///     <para>
        /// Refreshes the current page.  Maps to IWebBrowser2:Refresh.
        ///     </para>
        /// </summary>
        public override void Refresh()
        {
            try
            {
                if (ShouldSerializeDocumentText())
                {
                    string text = this.DocumentText;
                    this.AxIWebBrowser2.Refresh();
                    this.DocumentText = text;
                }
                else
                {
                    this.AxIWebBrowser2.Refresh();
                }
            }
            catch (Exception ex)
            {
                if (ClientUtils.IsSecurityOrCriticalException(ex))
                {
                    throw;
                }
            }
        }

        /// <summary>
        ///     <para>
        /// Refreshes the current page w/ the specified refresh option. The refresh option
        /// controls how much is loaded out of the browser cache vs. rechecking the server for.
        /// Maps to IWebBrowser2:Refresh2
        ///     </para>
        /// </summary>
        public void Refresh(WebBrowserRefreshOption opt)
        {
            object level = (object)opt;
            try
            {
                if (ShouldSerializeDocumentText())
                {
                    string text = this.DocumentText;
                    this.AxIWebBrowser2.Refresh2(ref level);
                    this.DocumentText = text;
                }
                else
                {
                    this.AxIWebBrowser2.Refresh2(ref level);
                }
            }
            catch (Exception ex)
            {
                if (ClientUtils.IsSecurityOrCriticalException(ex))
                {
                    throw;
                }
            }
        }

        /// <summary>
        ///     <para>
        /// Enables/disables the webbrowser's scrollbars.
        ///     </para>
        /// </summary>
        [SRDescription(nameof(SR.WebBrowserScrollBarsEnabledDescr)),
        SRCategory(nameof(SR.CatBehavior)), DefaultValue(true)]
        public bool ScrollBarsEnabled
        {
            get
            {
                return webBrowserState[WEBBROWSERSTATE_scrollbarsEnabled];
            }
            set
            {
                if (value != webBrowserState[WEBBROWSERSTATE_scrollbarsEnabled])
                {
                    webBrowserState[WEBBROWSERSTATE_scrollbarsEnabled] = value;
                    this.Refresh();
                }
            }
        }

        /// <summary>
        ///     <para>
        /// Opens the IE page setup dialog for the current page.
        /// Maps to IWebBrowser2:ExecWebBrowser w/ IDM_PAGESETUP flag & LECMDEXECOPT_PROMPTUSER.
        ///     </para>
        /// </summary>
        public void ShowPageSetupDialog()
        {
            object nullObjectArray = null;
            try
            {
                this.AxIWebBrowser2.ExecWB(NativeMethods.OLECMDID.OLECMDID_PAGESETUP, NativeMethods.OLECMDEXECOPT.OLECMDEXECOPT_PROMPTUSER, ref nullObjectArray, IntPtr.Zero);
            }
            catch (Exception ex)
            {
                if (ClientUtils.IsSecurityOrCriticalException(ex))
                {
                    throw;
                }
            }
        }

        /// <summary>
        ///     <para>
        /// Opens the IE print dialog.
        /// Maps to IWebBrowser2:ExecWebBrowser w/ IDM_PRINT flag & OLECMDEXECOPT_PROMPTUSER.
        ///     </para>
        /// </summary>
        public void ShowPrintDialog()
        {
            object nullObjectArray = null;

            try
            {
                this.AxIWebBrowser2.ExecWB(NativeMethods.OLECMDID.OLECMDID_PRINT, NativeMethods.OLECMDEXECOPT.OLECMDEXECOPT_PROMPTUSER, ref nullObjectArray, IntPtr.Zero);
            }
            catch (Exception ex)
            {
                if (ClientUtils.IsSecurityOrCriticalException(ex))
                {
                    throw;
                }
            }
        }

        /// <summary>
        ///     <para>
        /// Opens the IE print preview dialog.  Maps to IWebBrowser2:ExecWebBrowser w/ IDM_PRINTPREVIEW flag.
        ///     </para>
        /// </summary>
        public void ShowPrintPreviewDialog()
        {
            object nullObjectArray = null;

            try
            {
                this.AxIWebBrowser2.ExecWB(NativeMethods.OLECMDID.OLECMDID_PRINTPREVIEW, NativeMethods.OLECMDEXECOPT.OLECMDEXECOPT_PROMPTUSER, ref nullObjectArray, IntPtr.Zero);
            }
            catch (Exception ex)
            {
                if (ClientUtils.IsSecurityOrCriticalException(ex))
                {
                    throw;
                }
            }
        }

        /// <summary>
        ///     <para>
        /// Opens the properties dialog for the current html page.
        /// Maps to IWebBrowser2:ExecWebBrowser w/ IDM_PROPERTIES flag & LECMDEXECOPT_PROMPTUSER.
        ///     </para>
        /// </summary>
        public void ShowPropertiesDialog()
        {
            object nullObjectArray = null;

            try
            {
                this.AxIWebBrowser2.ExecWB(NativeMethods.OLECMDID.OLECMDID_PROPERTIES, NativeMethods.OLECMDEXECOPT.OLECMDEXECOPT_PROMPTUSER, ref nullObjectArray, IntPtr.Zero);
            }
            catch (Exception ex)
            {
                if (ClientUtils.IsSecurityOrCriticalException(ex))
                {
                    throw;
                }
            }
        }

        /// <summary>
        ///     <para>
        /// Opens the IE File-Save dialog.
        /// Maps to IWebBrowser2:ExecWebBrowser w/ IDM_SAVEAS flag & LECMDEXECOPT_PROMPTUSER.
        ///     </para>
        /// </summary>
        public void ShowSaveAsDialog()
        {
            object nullObjectArray = null;

            try
            {
                this.AxIWebBrowser2.ExecWB(NativeMethods.OLECMDID.OLECMDID_SAVEAS, NativeMethods.OLECMDEXECOPT.OLECMDEXECOPT_DODEFAULT, ref nullObjectArray, IntPtr.Zero);
            }
            catch (Exception ex)
            {
                if (ClientUtils.IsSecurityOrCriticalException(ex))
                {
                    throw;
                }
            }
        }

        /// <summary>
        ///     <para>
        /// Stops the current navigation.  Maps to IWebBrowser2:Stop.
        ///     </para>
        /// </summary>
        public void Stop()
        {
            try
            {
                this.AxIWebBrowser2.Stop();
            }
            catch (Exception ex)
            {
                if (ClientUtils.IsSecurityOrCriticalException(ex))
                {
                    throw;
                }
            }
        }

        //
        // Public events:
        //
        /// <summary>
        ///     <para>
        /// Occurs when the IE back button would change from enabled to disabled or vice versa.
        /// Maps to DWebBrowserEvents2:CommandStateChange w/ CSC_NAVIGATEBACK.
        ///     </para>
        /// </summary>
        [Browsable(false), SRCategory(nameof(SR.CatPropertyChanged)), SRDescription(nameof(SR.WebBrowserCanGoBackChangedDescr))]
        public event EventHandler CanGoBackChanged;
        /// <summary>
        ///     <para>
        /// Occurs when the IE forward button would change from enabled to disabled or vice versa.
        /// Maps to DWebBrowserEvents2:CommandStateChange w/ CSC_NAVIGATEFORWARD.
        ///     </para>
        /// </summary>
        [Browsable(false), SRCategory(nameof(SR.CatPropertyChanged)), SRDescription(nameof(SR.WebBrowserCanGoForwardChangedDescr))]
        public event EventHandler CanGoForwardChanged;
        /// <summary>
        ///     <para>
        /// Occurs when the document hosted in the web browser control is fully loaded.
        /// This is conceptially similar to Form.Load().  You need to wait until this event fires
        /// before doing anything that manipulates the html page, ex. reading the Document
        /// property of the webbrowser control. Maps to DWebBrowserEvents2:DocumentComplete.
        ///     </para>
        /// </summary>
        [SRCategory(nameof(SR.CatBehavior)), SRDescription(nameof(SR.WebBrowserDocumentCompletedDescr))]
        public event WebBrowserDocumentCompletedEventHandler DocumentCompleted;
        /// <summary>
        ///     <para>
        /// Occurs whenever the title text changes. The Title is the html page title
        /// or the file path/url if not title is available. This is the text you see as
        /// the title of the IE window preceeding "Microsoft Internet Explorer".
        /// Maps to DWebBrowserEvents2:TitleChange.
        ///     </para>
        /// </summary>
        [Browsable(false), SRCategory(nameof(SR.CatPropertyChanged)), SRDescription(nameof(SR.WebBrowserDocumentTitleChangedDescr))]
        public event EventHandler DocumentTitleChanged;
        /// <summary>
        ///     <para>
        /// Occurs whenever encryption level changes.
        /// Can be used to set a custom security lock icon similar to what IE shows when
        /// you go to an https site. Maps to DWebBrowserEvents2:SetSecureLockIcon.
        ///     </para>
        /// </summary>
        [Browsable(false), SRCategory(nameof(SR.CatPropertyChanged)), SRDescription(nameof(SR.WebBrowserEncryptionLevelChangedDescr))]
        public event EventHandler EncryptionLevelChanged;
        /// <summary>
        ///     <para>
        /// Occurs when a file download occurs.
        /// Can be used to cancel file downloads. Maps to DWebBrowserEvents2:FileDownload.
        ///     </para>
        /// </summary>
        [SRCategory(nameof(SR.CatBehavior)), SRDescription(nameof(SR.WebBrowserFileDownloadDescr))]
        public event EventHandler FileDownload;
        /// <summary>
        ///     <para>
        /// Occurs after browser control navigation occurs.
        /// Fires after browser navigation is complete. Maps to DWebBrowserEvents2:NavigateComplete.
        ///     </para>
        /// </summary>
        [SRCategory(nameof(SR.CatAction)), SRDescription(nameof(SR.WebBrowserNavigatedDescr))]
        public event WebBrowserNavigatedEventHandler Navigated;
        /// <summary>
        ///     <para>
        /// Occurs before browser control navigation occurs.
        /// Fires before browser navigation occurs. Allows navigation to be canceled if
        /// NavigatingEventArgs.Cancel is set to false. Maps to DWebBrowserEvents2:BeforeNavigate2.
        ///     </para>
        /// </summary>
        [SRCategory(nameof(SR.CatAction)), SRDescription(nameof(SR.WebBrowserNavigatingDescr))]
        public event WebBrowserNavigatingEventHandler Navigating;
        /// <summary>
        ///     <para>
        /// Occurs when a new browser window is created.
        /// Can be used to cancel the creation of the new browser window. Maps to DWebBrowserEvents2:NewWindow2.
        ///     </para>
        /// </summary>
        [SRCategory(nameof(SR.CatAction)), SRDescription(nameof(SR.WebBrowserNewWindowDescr))]
        public event CancelEventHandler NewWindow;
        /// <summary>
        ///     <para>
        /// Occurs when an update to the progress of a download occurs.
        /// Fires whenever the browser control has updated info on the download. Can be
        /// used to provide a download status bar and display the number of bytes downloaded.
        /// Maps to DWebBrowserEvents2:ProgressChange.
        ///     </para>
        /// </summary>
        [SRCategory(nameof(SR.CatAction)), SRDescription(nameof(SR.WebBrowserProgressChangedDescr))]
        public event WebBrowserProgressChangedEventHandler ProgressChanged;
        /// <summary>
        ///     <para>
        /// Occurs whenever the status text changes.
        /// Can be used to keep a status bar populated with uptodate text.
        /// Maps to DWebBrowserEvents2:StatusTextChange.
        ///     </para>
        /// </summary>
        [Browsable(false), SRCategory(nameof(SR.CatPropertyChanged)), SRDescription(nameof(SR.WebBrowserStatusTextChangedDescr))]
        public event EventHandler StatusTextChanged;



        //
        // public overrides:
        //

        /// <summary>
        ///     Returns true if this control (or any of its child windows) has focus.
        /// </summary>
        public override bool Focused
        {
            get
            {
                if (base.Focused)
                {
                    return true;
                }
                IntPtr hwndFocus = UnsafeNativeMethods.GetFocus();
                return hwndFocus != IntPtr.Zero
                    && SafeNativeMethods.IsChild(new HandleRef(this, this.Handle), new HandleRef(null, hwndFocus));
            }
        }


        //
        // protected overrides:
        //
        //
        //
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (htmlShimManager != null)
                {
                    htmlShimManager.Dispose();
                }
                DetachSink();
                ActiveXSite.Dispose();
            }
            base.Dispose(disposing);
        }

        /// <summary>
        ///     <para>
        /// Overrides the default size property of Control to specify a bigger default size of 250 x 250.
        ///     </para>
        /// </summary>
        protected override Size DefaultSize
        {
            get
            {
                return new Size(250, 250);
            }
        }

        /// <summary>
        ///     <para>
        /// Retrieves IWebBrowser2 from the native object. Overriding classes should first call base.AttachInterfaces.
        ///     </para>
        /// </summary>
        protected override void AttachInterfaces(object nativeActiveXObject)
        {
            this.axIWebBrowser2 = (UnsafeNativeMethods.IWebBrowser2)nativeActiveXObject;
        }

        /// <summary>
        ///     <para>
        /// Discards the IWebBrowser2 reference. Overriding classes should call base.DetachInterfaces.
        ///     </para>
        /// </summary>
        protected override void DetachInterfaces()
        {
            this.axIWebBrowser2 = null;
        }

        /// <summary>
        ///     <para>
        /// Returns a WebBrowserSite object.
        ///     </para>
        /// </summary>
        protected override WebBrowserSiteBase CreateWebBrowserSiteBase()
        {
            return new WebBrowserSite(this);
        }

        /// <summary>
        ///     <para>
        /// Attaches to the DWebBrowserEvents2 connection point.
        ///     </para>
        /// </summary>
        protected override void CreateSink()
        {
            object ax = this.activeXInstance;
            if (ax != null)
            {
                webBrowserEvent = new WebBrowserEvent(this);
                webBrowserEvent.AllowNavigation = AllowNavigation;
                this.cookie = new AxHost.ConnectionPointCookie(ax, webBrowserEvent,
                        typeof(UnsafeNativeMethods.DWebBrowserEvents2));
            }
        }

        /// <summary>
        ///     <para>
        /// Releases the DWebBrowserEvents2 connection point.
        ///     </para>
        /// </summary>
        protected override void DetachSink()
        {
            //If we have a cookie get rid of it
            if (this.cookie != null)
            {
                this.cookie.Disconnect();
                this.cookie = null;
            }
        }

        internal override void OnTopMostActiveXParentChanged(EventArgs e)
        {
            if (TopMostParent.IsIEParent)
            {
                WebBrowser.createdInIE = true;
                CheckIfCreatedInIE();
            }
            else
            {
                WebBrowser.createdInIE = false;
                base.OnTopMostActiveXParentChanged(e);
            }
        }



        //
        // protected virtuals:
        //

        /// <summary>
        ///     <para>
        /// Raises the <see cref='System.Windows.Forms.WebBrowser.CanGoBackChanged'/> event.
        ///     </para>
        /// </summary>
        [SuppressMessage("Microsoft.Security", "CA2109:ReviewVisibleEventHandlers")]
        // 
        protected virtual void OnCanGoBackChanged(EventArgs e)
        {
            if (this.CanGoBackChanged != null)
            {
                this.CanGoBackChanged(this, e);
            }
        }

        /// <summary>
        ///     <para>
        /// Raises the <see cref='System.Windows.Forms.WebBrowser.CanGoForwardChanged'/> event.
        ///     </para>
        /// </summary>
        [SuppressMessage("Microsoft.Security", "CA2109:ReviewVisibleEventHandlers")]
        // 
        protected virtual void OnCanGoForwardChanged(EventArgs e)
        {
            if (this.CanGoForwardChanged != null)
            {
                this.CanGoForwardChanged(this, e);
            }
        }

        /// <summary>
        ///     <para>
        /// Raises the <see cref='System.Windows.Forms.WebBrowser.DocumentCompleted'/> event.
        ///     </para>
        /// </summary>
        [SuppressMessage("Microsoft.Security", "CA2109:ReviewVisibleEventHandlers")]
        // 
        protected virtual void OnDocumentCompleted(WebBrowserDocumentCompletedEventArgs e)
        {
            this.AxIWebBrowser2.RegisterAsDropTarget = AllowWebBrowserDrop;
            if (this.DocumentCompleted != null)
            {
                this.DocumentCompleted(this, e);
            }
        }

        /// <summary>
        ///     <para>
        /// Raises the <see cref='System.Windows.Forms.WebBrowser.DocumentTitleChanged'/> event.
        ///     </para>
        /// </summary>
        [SuppressMessage("Microsoft.Security", "CA2109:ReviewVisibleEventHandlers")]
        // 
        protected virtual void OnDocumentTitleChanged(EventArgs e)
        {
            if (this.DocumentTitleChanged != null)
            {
                this.DocumentTitleChanged(this, e);
            }
        }

        /// <summary>
        ///     <para>
        /// Raises the <see cref='System.Windows.Forms.WebBrowser.EncryptionLevelChanged'/> event.
        ///     </para>
        /// </summary>
        [SuppressMessage("Microsoft.Security", "CA2109:ReviewVisibleEventHandlers")]
        // 
        protected virtual void OnEncryptionLevelChanged(EventArgs e)
        {
            if (this.EncryptionLevelChanged != null)
            {
                this.EncryptionLevelChanged(this, e);
            }
        }

        /// <summary>
        ///     <para>
        /// Raises the <see cref='System.Windows.Forms.WebBrowser.FileDownload'/> event.
        ///     </para>
        /// </summary>
        [SuppressMessage("Microsoft.Security", "CA2109:ReviewVisibleEventHandlers")]
        // 
        protected virtual void OnFileDownload(EventArgs e)
        {
            if (this.FileDownload != null)
            {
                this.FileDownload(this, e);
            }
        }

        /// <summary>
        ///     <para>
        /// Raises the <see cref='System.Windows.Forms.WebBrowser.Navigated'/> event.
        ///     </para>
        /// </summary>
        [SuppressMessage("Microsoft.Security", "CA2109:ReviewVisibleEventHandlers")]
        // 
        protected virtual void OnNavigated(WebBrowserNavigatedEventArgs e)
        {
            if (this.Navigated != null)
            {
                this.Navigated(this, e);
            }
        }

        /// <summary>
        ///     <para>
        /// Raises the <see cref='System.Windows.Forms.WebBrowser.Navigating'/> event.
        ///     </para>
        /// </summary>
        [SuppressMessage("Microsoft.Security", "CA2109:ReviewVisibleEventHandlers")]
        // 
        protected virtual void OnNavigating(WebBrowserNavigatingEventArgs e)
        {
            if (this.Navigating != null)
            {
                this.Navigating(this, e);
            }
        }

        /// <summary>
        ///     <para>
        /// Raises the <see cref='System.Windows.Forms.WebBrowser.NewWindow'/> event.
        ///     </para>
        /// </summary>
        [SuppressMessage("Microsoft.Security", "CA2109:ReviewVisibleEventHandlers")]
        // 
        protected virtual void OnNewWindow(CancelEventArgs e)
        {
            if (this.NewWindow != null)
            {
                this.NewWindow(this, e);
            }
        }

        /// <summary>
        ///     <para>
        /// Raises the <see cref='System.Windows.Forms.WebBrowser.ProgressChanged'/> event.
        ///     </para>
        /// </summary>
        [SuppressMessage("Microsoft.Security", "CA2109:ReviewVisibleEventHandlers")]
        // 
        protected virtual void OnProgressChanged(WebBrowserProgressChangedEventArgs e)
        {
            if (this.ProgressChanged != null)
            {
                this.ProgressChanged(this, e);
            }
        }

        /// <summary>
        ///     <para>
        /// Raises the <see cref='System.Windows.Forms.WebBrowser.StatusTextChanged'/> event.
        ///     </para>
        /// </summary>
        [SuppressMessage("Microsoft.Security", "CA2109:ReviewVisibleEventHandlers")]
        // 
        protected virtual void OnStatusTextChanged(EventArgs e)
        {
            if (this.StatusTextChanged != null)
            {
                this.StatusTextChanged(this, e);
            }
        }


        #region ShimSupport
        private HtmlShimManager htmlShimManager;
        internal HtmlShimManager ShimManager
        {
            get
            {
                if (htmlShimManager == null)
                {
                    htmlShimManager = new HtmlShimManager();
                }
                return htmlShimManager;
            }
        }
        #endregion


        //
        // Private methods:
        //
        private void CheckIfCreatedInIE()
        {
            if (WebBrowser.createdInIE)
            {
                if (this.ParentInternal != null)
                {
                    this.ParentInternal.Controls.Remove(this);
                    this.Dispose();
                }
                else
                {
                    this.Dispose();
                    throw new NotSupportedException(SR.WebBrowserInIENotSupported);
                }
            }
        }

        private string ReadyNavigateToUrl(string urlString)
        {
            if (string.IsNullOrEmpty(urlString))
            {
                urlString = "about:blank";
            }

            //
            // Nullify any calls to set_DocumentStream which may still be pending
            if (!webBrowserState[WEBBROWSERSTATE_documentStreamJustSet])
            {
                this.documentStreamToSetOnLoad = null;
            }

            return urlString;
        }

        private string ReadyNavigateToUrl(Uri url)
        {
            string urlString;
            if (url == null)
            {
                urlString = "about:blank";
            }
            else
            {
                if (!url.IsAbsoluteUri)
                {
                    throw new ArgumentException(string.Format(SR.WebBrowserNavigateAbsoluteUri, "uri"));
                }

                // Characters outside of US-ASCII may appear in Windows file paths and accordingly they are allowed in file URIs.
                // Therefore, do not use the escaped AbsoluteUri for file schemes. Can't use ToString() either since the correct 
                // syntax for file schemas includes percent escaped characters. We are stuck with OriginalString and hope that 
                // it is well-formed.
                urlString = url.IsFile ? url.OriginalString : url.AbsoluteUri;
            }

            return ReadyNavigateToUrl(urlString);
        }

        private void PerformNavigateHelper(string urlString, bool newWindow, string targetFrameName, byte[] postData, string headers)
        {
            object objUrlString = (object)urlString;
            object objFlags = (object)(newWindow ? 1 : 0);
            object objTargetFrameName = (object)targetFrameName;
            object objPostData = (object)postData;
            object objHeaders = (object)headers;
            PerformNavigate2(ref objUrlString, ref objFlags, ref objTargetFrameName, ref objPostData, ref objHeaders);
        }

        private void PerformNavigate2(ref object URL, ref object flags, ref object targetFrameName, ref object postData, ref object headers)
        {
            try
            {
                this.AxIWebBrowser2.Navigate2(ref URL, ref flags, ref targetFrameName, ref postData, ref headers);
            }
            catch (COMException ce)
            {
                if ((uint)unchecked(ce.ErrorCode) != (uint)unchecked(0x800704c7))
                {
                    // "the operation was canceled by the user" - navigation failed
                    // ignore this error, IE has already alerted the user. 
                    throw;
                }
            }
        }

        private bool ShouldSerializeDocumentText()
        {
            return IsValidUrl;
        }

        bool IsValidUrl
        {
            get
            {
                return Url == null || Url.AbsoluteUri == "about:blank";
            }
        }

        private bool ShouldSerializeUrl()
        {
            return !ShouldSerializeDocumentText();
        }

        /// <summary>
        ///     Returns TRUE if there is a context menu to show
        ///     Returns FALSE otherwise
        /// </summary>
        private bool ShowContextMenu(int x, int y)
        {
            ContextMenuStrip contextMenuStrip = ContextMenuStrip;
            ContextMenu contextMenu = contextMenuStrip != null ? null : ContextMenu;

            if (contextMenuStrip != null || contextMenu != null)
            {
                Point client;
                bool keyboardActivated = false;
                // X will be exactly -1 when the user invokes the context menu from the keyboard
                //
                if (x == -1)
                {
                    keyboardActivated = true;
                    client = new Point(Width / 2, Height / 2);
                }
                else
                {
                    client = PointToClient(new Point(x, y));
                }

                if (ClientRectangle.Contains(client))
                {
                    if (contextMenuStrip != null)
                    {
                        contextMenuStrip.ShowInternal(this, client, keyboardActivated);
                    }
                    else if (contextMenu != null)
                    {
                        contextMenu.Show(this, client);
                    }

                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        protected override void WndProc(ref Message m)
        {
            switch (m.Msg)
            {
                case Interop.WindowMessages.WM_CONTEXTMENU:
                    int x = NativeMethods.Util.SignedLOWORD(m.LParam);
                    int y = NativeMethods.Util.SignedHIWORD(m.LParam);

                    if (!ShowContextMenu(x, y))
                    {
                        DefWndProc(ref m);
                    }
                    break;
                default:
                    base.WndProc(ref m);
                    break;
            }
        }

        private UnsafeNativeMethods.IWebBrowser2 AxIWebBrowser2
        {
            get
            {
                if (this.axIWebBrowser2 == null)
                {
                    if (!this.IsDisposed)
                    {
                        // This should call AttachInterfaces
                        TransitionUpTo(WebBrowserHelper.AXState.InPlaceActive);
                    }
                    else
                    {
                        throw new System.ObjectDisposedException(GetType().Name);
                    }
                }
                // We still don't have this.axIWebBrowser2. Throw an exception.
                if (this.axIWebBrowser2 == null)
                {
                    throw new InvalidOperationException(SR.WebBrowserNoCastToIWebBrowser2);
                }
                return this.axIWebBrowser2;
            }
        }



        //
        // WebBrowserSite class:
        //
        /// <summary>
        ///     <para>
        /// Provides a default WebBrowserSite implementation for use in the CreateWebBrowserSite
        /// method in the WebBrowser class. 
        ///     </para>
        /// </summary>
        [ComVisible(false)]
        protected class WebBrowserSite : WebBrowserSiteBase, UnsafeNativeMethods.IDocHostUIHandler
        {
            /// <summary>
            ///     <para>
            /// Creates an instance of the <see cref='System.Windows.Forms.WebBrowser.WebBrowserSite'/> class.
            ///     </para>
            /// </summary>
            public WebBrowserSite(WebBrowser host) : base(host)
            {
            }


            //
            // IDocHostUIHandler Implementation
            //
            int UnsafeNativeMethods.IDocHostUIHandler.ShowContextMenu(int dwID, NativeMethods.POINT pt, object pcmdtReserved, object pdispReserved)
            {
                WebBrowser wb = (WebBrowser)this.Host;

                if (wb.IsWebBrowserContextMenuEnabled)
                {
                    // let MSHTML display its UI
                    return NativeMethods.S_FALSE;
                }
                else
                {
                    if (pt.x == 0 && pt.y == 0)
                    {
                        // IDocHostUIHandler::ShowContextMenu sends (0,0) when the context menu is invoked via the keyboard
                        // make it (-1, -1) for the WebBrowser::ShowContextMenu method
                        pt.x = -1;
                        pt.y = -1;
                    }
                    wb.ShowContextMenu(pt.x, pt.y);
                    // MSHTML should not display its context menu because we displayed ours
                    return NativeMethods.S_OK;
                }
            }


            int UnsafeNativeMethods.IDocHostUIHandler.GetHostInfo(NativeMethods.DOCHOSTUIINFO info)
            {
                WebBrowser wb = (WebBrowser)this.Host;

                info.dwDoubleClick = (int)NativeMethods.DOCHOSTUIDBLCLICK.DEFAULT;
                info.dwFlags = (int)NativeMethods.DOCHOSTUIFLAG.NO3DOUTERBORDER |
                                (int)NativeMethods.DOCHOSTUIFLAG.DISABLE_SCRIPT_INACTIVE;

                if (wb.ScrollBarsEnabled)
                {
                    info.dwFlags |= (int)NativeMethods.DOCHOSTUIFLAG.FLAT_SCROLLBAR;
                }
                else
                {
                    info.dwFlags |= (int)NativeMethods.DOCHOSTUIFLAG.SCROLL_NO;
                }

                if (Application.RenderWithVisualStyles)
                {
                    info.dwFlags |= (int)NativeMethods.DOCHOSTUIFLAG.THEME;
                }
                else
                {
                    info.dwFlags |= (int)NativeMethods.DOCHOSTUIFLAG.NOTHEME;
                }

                return NativeMethods.S_OK;
            }

            int UnsafeNativeMethods.IDocHostUIHandler.EnableModeless(bool fEnable)
            {
                return NativeMethods.E_NOTIMPL;
            }

            int UnsafeNativeMethods.IDocHostUIHandler.ShowUI(int dwID, UnsafeNativeMethods.IOleInPlaceActiveObject activeObject,
                    NativeMethods.IOleCommandTarget commandTarget, UnsafeNativeMethods.IOleInPlaceFrame frame,
                    UnsafeNativeMethods.IOleInPlaceUIWindow doc)
            {
                return NativeMethods.S_FALSE;
            }

            int UnsafeNativeMethods.IDocHostUIHandler.HideUI()
            {
                return NativeMethods.E_NOTIMPL;
            }

            int UnsafeNativeMethods.IDocHostUIHandler.UpdateUI()
            {
                return NativeMethods.E_NOTIMPL;
            }

            int UnsafeNativeMethods.IDocHostUIHandler.OnDocWindowActivate(bool fActivate)
            {
                return NativeMethods.E_NOTIMPL;
            }

            int UnsafeNativeMethods.IDocHostUIHandler.OnFrameWindowActivate(bool fActivate)
            {
                return NativeMethods.E_NOTIMPL;
            }

            int UnsafeNativeMethods.IDocHostUIHandler.ResizeBorder(NativeMethods.COMRECT rect, UnsafeNativeMethods.IOleInPlaceUIWindow doc, bool fFrameWindow)
            {
                return NativeMethods.E_NOTIMPL;
            }

            int UnsafeNativeMethods.IDocHostUIHandler.GetOptionKeyPath(string[] pbstrKey, int dw)
            {
                return NativeMethods.E_NOTIMPL;
            }

            int UnsafeNativeMethods.IDocHostUIHandler.GetDropTarget(UnsafeNativeMethods.IOleDropTarget pDropTarget, out UnsafeNativeMethods.IOleDropTarget ppDropTarget)
            {
                //
                // Set to null no matter what we return, to prevent the marshaller
                // from having issues if the pointer points to random stuff.
                ppDropTarget = null;
                return NativeMethods.E_NOTIMPL;
            }

            int UnsafeNativeMethods.IDocHostUIHandler.GetExternal(out object ppDispatch)
            {
                WebBrowser wb = (WebBrowser)this.Host;
                ppDispatch = wb.ObjectForScripting;
                return NativeMethods.S_OK;
            }

            [SuppressMessage("Microsoft.Performance", "CA1803:AvoidCostlyCallsWherePossible")]
            int UnsafeNativeMethods.IDocHostUIHandler.TranslateAccelerator(ref NativeMethods.MSG msg, ref Guid group, int nCmdID)
            {
                //
                // Returning S_FALSE will allow the native control to do default processing,
                // i.e., execute the shortcut key. Returning S_OK will cancel the shortcut key.

                WebBrowser wb = (WebBrowser)this.Host;

                if (!wb.WebBrowserShortcutsEnabled)
                {
                    int keyCode = (int)msg.wParam | (int)Control.ModifierKeys;

                    if (msg.message != Interop.WindowMessages.WM_CHAR
                            && Enum.IsDefined(typeof(Shortcut), (Shortcut)keyCode))
                    {
                        return NativeMethods.S_OK;
                    }
                    return NativeMethods.S_FALSE;
                }
                return NativeMethods.S_FALSE;
            }

            int UnsafeNativeMethods.IDocHostUIHandler.TranslateUrl(int dwTranslate, string strUrlIn, out string pstrUrlOut)
            {
                //
                // Set to null no matter what we return, to prevent the marshaller
                // from having issues if the pointer points to random stuff.
                pstrUrlOut = null;
                return NativeMethods.S_FALSE;
            }

            int UnsafeNativeMethods.IDocHostUIHandler.FilterDataObject(IComDataObject pDO, out IComDataObject ppDORet)
            {
                //
                // Set to null no matter what we return, to prevent the marshaller
                // from having issues if the pointer points to random stuff.
                ppDORet = null;
                return NativeMethods.S_FALSE;
            }

            //
            // Internal methods
            //
            internal override void OnPropertyChanged(int dispid)
            {
                if (dispid != NativeMethods.ActiveX.DISPID_READYSTATE)
                {
                    base.OnPropertyChanged(dispid);
                }
            }
        }


        //
        // Private classes:
        //
        [ClassInterface(ClassInterfaceType.None)]
        private class WebBrowserEvent : StandardOleMarshalObject, UnsafeNativeMethods.DWebBrowserEvents2
        {

            private WebBrowser parent;
            private bool allowNavigation;
            private bool haveNavigated = false;

            public WebBrowserEvent(WebBrowser parent)
            {
                this.parent = parent;
            }

            public bool AllowNavigation
            {
                get
                {
                    return allowNavigation;
                }
                set
                {
                    allowNavigation = value;
                }
            }

            public void CommandStateChange(long command, bool enable)
            {
                if (command == NativeMethods.CSC_NAVIGATEBACK)
                {
                    this.parent.CanGoBackInternal = enable;
                }
                else if (command == NativeMethods.CSC_NAVIGATEFORWARD)
                {
                    this.parent.CanGoForwardInternal = enable;
                }
            }

            public void BeforeNavigate2(object pDisp, ref object urlObject, ref object flags, ref object targetFrameName, ref object postData, ref object headers, ref bool cancel)
            {
                Debug.Assert(parent != null, "Parent should have been set");
                //Note: we want to allow navigation if we haven't already navigated.
                if (AllowNavigation || !haveNavigated)
                {
                    Debug.Assert(urlObject == null || urlObject is string, "invalid url type");
                    Debug.Assert(targetFrameName == null || targetFrameName is string, "invalid targetFrameName type");
                    Debug.Assert(headers == null || headers is string, "invalid headers type");
                    //
                    // If during running interop code, the variant.bstr value gets set
                    // to -1 on return back to native code, if the original value was null, we
                    // have to set targetFrameName and headers to "".
                    if (targetFrameName == null)
                    {
                        targetFrameName = string.Empty;
                    }
                    if (headers == null)
                    {
                        headers = string.Empty;
                    }

                    string urlString = urlObject == null ? "" : (string)urlObject;
                    WebBrowserNavigatingEventArgs e = new WebBrowserNavigatingEventArgs(
                        new Uri(urlString), targetFrameName == null ? "" : (string)targetFrameName);
                    this.parent.OnNavigating(e);
                    cancel = e.Cancel;
                }
                else
                {
                    cancel = true;
                }
            }

            public void DocumentComplete(object pDisp, ref object urlObject)
            {
                Debug.Assert(urlObject == null || urlObject is string, "invalid url");
                haveNavigated = true;
                if (this.parent.documentStreamToSetOnLoad != null && (string)urlObject == "about:blank")
                {
                    HtmlDocument htmlDocument = this.parent.Document;
                    if (htmlDocument != null)
                    {
                        UnsafeNativeMethods.IPersistStreamInit psi = htmlDocument.DomDocument as UnsafeNativeMethods.IPersistStreamInit;
                        Debug.Assert(psi != null, "The Document does not implement IPersistStreamInit");
                        UnsafeNativeMethods.IStream iStream = (UnsafeNativeMethods.IStream)new UnsafeNativeMethods.ComStreamFromDataStream(
                                                    this.parent.documentStreamToSetOnLoad);
                        psi.Load(iStream);
                        htmlDocument.Encoding = "unicode";
                    }
                    this.parent.documentStreamToSetOnLoad = null;
                }
                else
                {
                    string urlString = urlObject == null ? "" : urlObject.ToString();
                    WebBrowserDocumentCompletedEventArgs e = new WebBrowserDocumentCompletedEventArgs(
                            new Uri(urlString));
                    this.parent.OnDocumentCompleted(e);
                }
            }

            public void TitleChange(string text)
            {
                this.parent.OnDocumentTitleChanged(EventArgs.Empty);
            }

            public void SetSecureLockIcon(int secureLockIcon)
            {
                this.parent.encryptionLevel = (WebBrowserEncryptionLevel)secureLockIcon;
                this.parent.OnEncryptionLevelChanged(EventArgs.Empty);
            }

            public void NavigateComplete2(object pDisp, ref object urlObject)
            {
                Debug.Assert(urlObject == null || urlObject is string, "invalid url type");
                string urlString = urlObject == null ? "" : (string)urlObject;
                WebBrowserNavigatedEventArgs e = new WebBrowserNavigatedEventArgs(
                        new Uri(urlString));
                this.parent.OnNavigated(e);
            }

            public void NewWindow2(ref object ppDisp, ref bool cancel)
            {
                CancelEventArgs e = new CancelEventArgs();
                this.parent.OnNewWindow(e);
                cancel = e.Cancel;
            }

            public void ProgressChange(int progress, int progressMax)
            {
                WebBrowserProgressChangedEventArgs e = new WebBrowserProgressChangedEventArgs(progress, progressMax);
                this.parent.OnProgressChanged(e);
            }

            public void StatusTextChange(string text)
            {
                this.parent.statusText = (text == null) ? "" : text;
                this.parent.OnStatusTextChanged(EventArgs.Empty);
            }

            public void DownloadBegin()
            {
                this.parent.OnFileDownload(EventArgs.Empty);
            }

            public void FileDownload(ref bool cancel) { }
            public void PrivacyImpactedStateChange(bool bImpacted) { }
            public void UpdatePageStatus(object pDisp, ref object nPage, ref object fDone) { }
            public void PrintTemplateTeardown(object pDisp) { }
            public void PrintTemplateInstantiation(object pDisp) { }
            public void NavigateError(object pDisp, ref object url, ref object frame, ref object statusCode, ref bool cancel) { }
            public void ClientToHostWindow(ref long cX, ref long cY) { }
            public void WindowClosing(bool isChildWindow, ref bool cancel) { }
            public void WindowSetHeight(int height) { }
            public void WindowSetWidth(int width) { }
            public void WindowSetTop(int top) { }
            public void WindowSetLeft(int left) { }
            public void WindowSetResizable(bool resizable) { }
            public void OnTheaterMode(bool theaterMode) { }
            public void OnFullScreen(bool fullScreen) { }
            public void OnStatusBar(bool statusBar) { }
            public void OnMenuBar(bool menuBar) { }
            public void OnToolBar(bool toolBar) { }
            public void OnVisible(bool visible) { }
            public void OnQuit() { }
            public void PropertyChange(string szProperty) { }
            public void DownloadComplete() { }
        }
    }
}
