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
using System.Security.Permissions;
using System.Security;
using System.Runtime.InteropServices;
using System.Net;
using System.Text;

using IComDataObject = System.Runtime.InteropServices.ComTypes.IDataObject;

namespace System.Windows.Forms {
    /// <include file='doc\WebBrowser.uex' path='docs/doc[@for="WebBrowser"]/*' />
    /// <devdoc>
    ///     <para>
    /// This is a wrapper over the native WebBrowser control implemented in shdocvw.dll.
    ///     </para>
    /// </devdoc>
    [ComVisible(true),
    ClassInterface(ClassInterfaceType.AutoDispatch),
    PermissionSet(SecurityAction.LinkDemand, Name = "FullTrust"),
    PermissionSetAttribute(SecurityAction.InheritanceDemand, Name="FullTrust"),
    DefaultProperty(nameof(Url)), DefaultEvent(nameof(DocumentCompleted)),
    Docking(DockingBehavior.AutoDock),
    SRDescription(nameof(SR.DescriptionWebBrowser)),
    Designer("System.Windows.Forms.Design.WebBrowserDesigner, " + AssemblyRef.SystemDesign)]
    public class WebBrowser : WebBrowserBase {
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
        internal string statusText = "";


        private const int   WEBBROWSERSTATE_webBrowserShortcutsEnabled      = 0x00000001;
        private const int   WEBBROWSERSTATE_documentStreamJustSet           = 0x00000002;
        private const int   WEBBROWSERSTATE_isWebBrowserContextMenuEnabled  = 0x00000004;
        private const int   WEBBROWSERSTATE_canGoBack                       = 0x00000008;
        private const int   WEBBROWSERSTATE_canGoForward                    = 0x00000010;
        private const int   WEBBROWSERSTATE_scrollbarsEnabled               = 0x00000020;
        private const int   WEBBROWSERSTATE_allowNavigation                 = 0x00000040;

        // PERF: take all the bools and put them into a state variable
        private System.Collections.Specialized.BitVector32 webBrowserState;          // see TREEVIEWSTATE_ consts above


        //
        // 8856f961-340a-11d0-a96b-00c04fd705a2 is the clsid for the native webbrowser control
        //
        /// <include file='doc\WebBrowser.uex' path='docs/doc[@for="WebBrowser.WebBrowser"]/*' />
        /// <devdoc>
        ///     <para>
        /// Creates an instance of the <see cref='System.Windows.Forms.WebBrowser'/> control.
        ///     </para>
        /// </devdoc>
        [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
        public WebBrowser() : base("8856f961-340a-11d0-a96b-00c04fd705a2") {
                CheckIfCreatedInIE();
    
            webBrowserState = new System.Collections.Specialized.BitVector32(WEBBROWSERSTATE_isWebBrowserContextMenuEnabled |
                    WEBBROWSERSTATE_webBrowserShortcutsEnabled | WEBBROWSERSTATE_scrollbarsEnabled);
            AllowNavigation = true;
        }


        //
        // Public properties:
        //


        /// <include file='doc\WebBrowser.uex' path='docs/doc[@for="WebBrowser.AllowNavigation"]/*' />
        /// <devdoc>
        ///     <para>
        /// Specifies whether the WebBrowser control may navigate to another page once 
        /// it has been loaded.  NOTE: it will always be able to navigate before being loaded.
        /// "Loaded" here means setting Url, DocumentText, or DocumentStream.
        ///     </para>
        /// </devdoc>
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

        /// <include file='doc\WebBrowser.uex' path='docs/doc[@for="WebBrowser.AllowWebBrowserDrop"]/*' />
        /// <devdoc>
        ///     <para>
        /// Specifies whether the WebBrowser control will receive drop notifcations.
        /// Maps to IWebBrowser2:RegisterAsDropTarget.
        /// Note that this does not mean that the WebBrowser control integrates with
        /// Windows Forms drag/drop i.e. the DragDrop event does not fire.  It does
        /// control whether you can drag new documents into the browser control.
        ///     </para>
        /// </devdoc>
        [SRDescription(nameof(SR.WebBrowserAllowWebBrowserDropDescr)),
        SRCategory(nameof(SR.CatBehavior)), DefaultValue(true)]
        public bool AllowWebBrowserDrop {
            get {
                return this.AxIWebBrowser2.RegisterAsDropTarget;
            }
            set {
                //Note: you lose this value when you load a new document: the value needs to be refreshed in
                //OnDocumentCompleted.
                if (value != AllowWebBrowserDrop) {
                    this.AxIWebBrowser2.RegisterAsDropTarget = value;
                    this.Refresh();
                }
            }
        }
        
        /// <include file='doc\WebBrowser.uex' path='docs/doc[@for="WebBrowser.ScriptErrorsSuppressed"]/*' />
        /// <devdoc>
        ///     <para>
        /// Specifies whether the browser control shows script errors in dialogs or not.
        /// Maps to IWebBrowser2:Silent.
        ///     </para>
        /// </devdoc>
        [SRDescription(nameof(SR.WebBrowserScriptErrorsSuppressedDescr)),
        SRCategory(nameof(SR.CatBehavior)), DefaultValue(false)]
        public bool ScriptErrorsSuppressed {
            get {
                return this.AxIWebBrowser2.Silent;
            }
            set {
                if (value != ScriptErrorsSuppressed) {
                    this.AxIWebBrowser2.Silent = value;
                }
            }
        }

        /// <include file='doc\WebBrowser.uex' path='docs/doc[@for="WebBrowser.WebBrowserShortcutsEnabled"]/*' />
        /// <devdoc>
        ///     <para>
        /// Specifies whether the browser control Shortcuts are enabled.
        /// Maps to IDocHostUIHandler:TranslateAccelerator event.
        ///     </para>
        /// </devdoc>
        [SRDescription(nameof(SR.WebBrowserWebBrowserShortcutsEnabledDescr)),
        SRCategory(nameof(SR.CatBehavior)), DefaultValue(true)]
        public bool WebBrowserShortcutsEnabled {
            get {
                return webBrowserState[WEBBROWSERSTATE_webBrowserShortcutsEnabled];
            }
            set {
                webBrowserState[WEBBROWSERSTATE_webBrowserShortcutsEnabled] = value;
            }
        }

        /// <include file='doc\WebBrowser.uex' path='docs/doc[@for="WebBrowser.CanGoBack"]/*' />
        /// <devdoc>
        ///     <para>
        /// If true, there is navigation history such that calling GoBack() will succeed.
        /// Defaults to false.  After that it's value is kept up to date by hooking the
        /// DWebBrowserEvents2:CommandStateChange.
        /// Requires WebPermission for Url specified by Document.Domain (indirect access
        /// to user browsing stats).
        ///     </para>
        /// </devdoc>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool CanGoBack {
            get {
                return CanGoBackInternal;
            }
        }

        /// <devdoc>
        /// Returns the current WEBBROWSERSTATE_canGoBack value so that this value can be accessed
        /// from child classes.
        /// </devdoc>
        internal bool CanGoBackInternal {
            get {
                return webBrowserState[WEBBROWSERSTATE_canGoBack];
            }
            set {
                if (value != CanGoBackInternal) {
                    webBrowserState[WEBBROWSERSTATE_canGoBack] = value;
                    OnCanGoBackChanged(EventArgs.Empty);
                }
            }
        }
        
        /// <include file='doc\WebBrowser.uex' path='docs/doc[@for="WebBrowser.CanGoForward"]/*' />
        /// <devdoc>
        ///     <para>
        /// If true, there is navigation history such that calling GoForward() will succeed.
        /// Defaults to false.  After that it's value is kept up to date by hooking the
        /// DWebBrowserEvents2:CommandStateChange.
        /// Requires WebPermission for Url specified by Document.Domain (indirect access
        /// to user browsing stats).
        ///     </para>
        /// </devdoc>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool CanGoForward {
            get {
                return CanGoForwardInternal;
            }
        }
        
        /// <devdoc>
        /// Returns the current WEBBROWSERSTATE_canGoForward value so that this value can
        /// be accessed from child classes.
        /// </devdoc>
        internal bool CanGoForwardInternal {
            get {
                return webBrowserState[WEBBROWSERSTATE_canGoForward];
            }
            set {
                if (value != CanGoForwardInternal) {
                    webBrowserState[WEBBROWSERSTATE_canGoForward] = value;
                    OnCanGoForwardChanged(EventArgs.Empty);
                }
            }
        }

        /// <include file='doc\WebBrowser.uex' path='docs/doc[@for="WebBrowser.Document"]/*' />
        /// <devdoc>
        ///     <para>
        /// The HtmlDocument for page hosted in the html page.  If no page is loaded, it returns null.
        /// Maps to IWebBrowser2:Document.
        /// Requires WebPermission for Url specified by Document.Domain.
        ///     </para>
        /// </devdoc>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public HtmlDocument Document {
            get {
                object objDoc = this.AxIWebBrowser2.Document;
                if (objDoc != null) {
                    // Document is not necessarily an IHTMLDocument, it might be an office document as well.
                    UnsafeNativeMethods.IHTMLDocument2 iHTMLDocument2 = null;
                    try {
                        iHTMLDocument2 = objDoc as UnsafeNativeMethods.IHTMLDocument2;
                    } 
                    catch (InvalidCastException) { 
                    }
                    if (iHTMLDocument2 != null) {
                        UnsafeNativeMethods.IHTMLLocation iHTMLLocation = iHTMLDocument2.GetLocation();
                        if (iHTMLLocation != null) {
                            string href = iHTMLLocation.GetHref();
                            if (!string.IsNullOrEmpty(href))
                            {
                                Uri url = new Uri(href);
                                WebBrowser.EnsureUrlConnectPermission(url);  // Security check
                                return new HtmlDocument (ShimManager, iHTMLDocument2 as UnsafeNativeMethods.IHTMLDocument);
                            }
                        }
                    }
                }
                return null;
            }
        }
        
        /// <include file='doc\WebBrowser.uex' path='docs/doc[@for="WebBrowser.DocumentStream"]/*' />
        /// <devdoc>
        ///     <para>
        /// Get/sets the stream for the html document.
        /// Uses the IPersisteStreamInit interface on the HtmlDocument to set/retrieve the html stream.
        ///     </para>
        /// </devdoc>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Stream DocumentStream {
            get {
                HtmlDocument htmlDocument = this.Document; 
                if (htmlDocument == null) {
                    return null;
                }
                else {
                    UnsafeNativeMethods.IPersistStreamInit psi = htmlDocument.DomDocument as UnsafeNativeMethods.IPersistStreamInit;
                    Debug.Assert(psi != null, "Object isn't an IPersistStreamInit!");
                    if (psi == null) {
                        return null;
                    }
                    else {
                        MemoryStream memoryStream = new MemoryStream();
                        UnsafeNativeMethods.IStream iStream = (UnsafeNativeMethods.IStream)new UnsafeNativeMethods.ComStreamFromDataStream(memoryStream);
                        psi.Save(iStream, false);
                        return new MemoryStream(memoryStream.GetBuffer(), 0, (int)memoryStream.Length, false);
                    }
                }
            }
            set {
                this.documentStreamToSetOnLoad = value;
                try {
                    webBrowserState[WEBBROWSERSTATE_documentStreamJustSet] = true;
                    // Lets navigate to "about:blank" so that we get a "clean" document
                    this.Url = new Uri("about:blank");
                }
                finally {
                    webBrowserState[WEBBROWSERSTATE_documentStreamJustSet] = false;
                }
            }
        }
        
        /// <include file='doc\WebBrowser.uex' path='docs/doc[@for="WebBrowser.DocumentText"]/*' />
        /// <devdoc>
        ///     <para>
        /// Sets/sets the text of the contained html page.
        ///     </para>
        /// </devdoc>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string DocumentText {
            get {
                Stream stream = this.DocumentStream;
                if (stream == null)
                {
                    return "";
                }
                StreamReader reader = new StreamReader(stream);
                stream.Position = 0;
                return reader.ReadToEnd();
            }
            set {
                if (value == null)
                {
                    value = "";
                }
                //string length is a good initial guess for capacity -- 
                //if it needs more room, it'll take it.
                MemoryStream ms = new MemoryStream(value.Length);
                StreamWriter sw = new StreamWriter(ms, Encoding.UTF8 );
                sw.Write(value);
                sw.Flush();
                ms.Position = 0;
                this.DocumentStream = ms;
            }
        }

        /// <include file='doc\WebBrowser.uex' path='docs/doc[@for="WebBrowser.DocumentTitle"]/*' />
        /// <devdoc>
        ///     <para>
        /// The title of the html page currently loaded. If none are loaded, returns empty string.
        /// Maps to IWebBrowser2:LocationName.
        /// Requires WebPermission for Url specified by Document.Domain.
        ///     </para>
        /// </devdoc>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string DocumentTitle {
            get {
                string documentTitle;

                HtmlDocument htmlDocument = this.Document;
                if (htmlDocument == null) {
                    documentTitle = this.AxIWebBrowser2.LocationName;
                }
                else {
                    UnsafeNativeMethods.IHTMLDocument2 htmlDocument2 = htmlDocument.DomDocument as UnsafeNativeMethods.IHTMLDocument2;
                    Debug.Assert(htmlDocument2 != null, "The HtmlDocument object must implement IHTMLDocument2.");
                    try {
                        documentTitle = htmlDocument2.GetTitle();
                    }
                    catch (COMException) {
                        documentTitle = "";
                    }
                }
                return documentTitle;
            }
        }
        
        /// <include file='doc\WebBrowser.uex' path='docs/doc[@for="WebBrowser.DocumentType"]/*' />
        /// <devdoc>
        ///     <para>
        /// A string containing the MIME type of the document hosted in the browser control.
        /// If none are loaded, returns empty string.  Maps to IHTMLDocument2:mimeType.
        /// Requires WebPermission for Url specified by Document.Domain.
        ///     </para>
        /// </devdoc>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string DocumentType {
            get {
                string docType = "";
                HtmlDocument htmlDocument = this.Document;
                if (htmlDocument != null) {
                    UnsafeNativeMethods.IHTMLDocument2 htmlDocument2 = htmlDocument.DomDocument as UnsafeNativeMethods.IHTMLDocument2;
                    Debug.Assert(htmlDocument2 != null, "The HtmlDocument object must implement IHTMLDocument2.");
                    try {
                        docType = htmlDocument2.GetMimeType();
                    }
                    catch (COMException) {
                        docType = "";
                    }
                }
                return docType;
            }
        }
        
        /// <include file='doc\WebBrowser.uex' path='docs/doc[@for="WebBrowser.EncryptionLevel"]/*' />
        /// <devdoc>
        ///     <para>
        /// Initially set to WebBrowserEncryptionLevel.Insecure.
        /// After that it's kept up to date by hooking the DWebBrowserEvents2:SetSecureLockIcon.
        /// Requires WebPermission for Url specified by Document.Domain (indirect access
        /// to user browsing stats).
        ///     </para>
        /// </devdoc>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public WebBrowserEncryptionLevel EncryptionLevel {
            get {
                if (this.Document == null) {
                    encryptionLevel = WebBrowserEncryptionLevel.Unknown;
                }
                return encryptionLevel;
            }
        }
        
        /// <include file='doc\WebBrowser.uex' path='docs/doc[@for="WebBrowser.IsBusy"]/*' />
        /// <devdoc>
        ///     <para>
        /// True if the browser is engaged in navigation or download.  Maps to IWebBrowser2:Busy.
        /// Requires WebPermission for Url specified by Document.Domain (indirect access
        /// to user browsing stats).
        ///     </para>
        /// </devdoc>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsBusy {
            get {
                if (this.Document == null) {
                    return false;
                }
                else {
                    return this.AxIWebBrowser2.Busy;
                }
            }
        }
        
        /// <include file='doc\WebBrowser.uex' path='docs/doc[@for="WebBrowser.IsOffline"]/*' />
        /// <devdoc>
        ///     <para>
        /// Gets the offline state of the browser control. Maps to IWebBrowser2:Offline.
        ///     </para>
        /// </devdoc>
        [SRDescription(nameof(SR.WebBrowserIsOfflineDescr)), Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsOffline {
            get {
                return this.AxIWebBrowser2.Offline;
            }
        }
        
        /// <include file='doc\WebBrowser.uex' path='docs/doc[@for="WebBrowser.IsWebBrowserContextMenuEnabled"]/*' />
        /// <devdoc>
        ///     <para>
        /// Indicates whether to use the WebBrowser context menu.
        /// It's technically possible to have both the WebBrowser & Windows Forms context
        /// menu enabled, but making this property effect the behavior of the Windows Form
        /// context menu does not lead to a clean OM.  Maps to sinking the
        /// IDocHostUIHandler:ShowContextMenu 
        ///     </para>
        /// </devdoc>
        [SRDescription(nameof(SR.WebBrowserIsWebBrowserContextMenuEnabledDescr)),
        SRCategory(nameof(SR.CatBehavior)), DefaultValue(true)]
        public bool IsWebBrowserContextMenuEnabled {
            get {
                return webBrowserState[WEBBROWSERSTATE_isWebBrowserContextMenuEnabled];
            }
            set {
                webBrowserState[WEBBROWSERSTATE_isWebBrowserContextMenuEnabled] = value;
            }
        }

        /// <include file='doc\WebBrowser.uex' path='docs/doc[@for="WebBrowser.ObjectForScripting"]/*' />
        /// <devdoc>
        ///     <para>
        /// Allows the host application to provide an object that the contained html
        /// pages can access programatically in script.  The object specified here
        /// will be accessible in script as the "window.external" object via IDispatch
        /// COM interop. Maps to an implementation of the IDocUIHandler.GetExternal event.
        ///     </para>
        /// </devdoc>
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

        /// <include file='doc\WebBrowser.uex' path='docs/doc[@for="WebBrowser.Padding"]/*' />
        [
        Browsable(false),
        EditorBrowsable(EditorBrowsableState.Never),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
        ]
        public new Padding Padding {
            get { return base.Padding; }
            set { base.Padding = value;}
        }

        /// <include file='doc\WebBrowser.uex' path='docs/doc[@for="WebBrowser.PaddingChanged"]/*' />
        [
        Browsable(false),
        EditorBrowsable(EditorBrowsableState.Never),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        SRCategory(nameof(SR.CatLayout)), SRDescription(nameof(SR.ControlOnPaddingChangedDescr))
        ]
        public new event EventHandler PaddingChanged
        {
            add
            {
                base.PaddingChanged += value;
            }
            remove
            {
                base.PaddingChanged -= value;
            }
        }

        /// <include file='doc\WebBrowser.uex' path='docs/doc[@for="WebBrowser.ReadyState"]/*' />
        /// <devdoc>
        ///     <para>
        /// Gets the ReadyState of the browser control. (ex.. document loading vs. load complete).
        /// Maps to IWebBrowser2:ReadyState.
        /// Requires WebPermission for Url specified by Document.Domain (indirect access
        /// to user browsing stats).
        ///     </para>
        /// </devdoc>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public WebBrowserReadyState ReadyState {
            get {
                if (this.Document == null) {
                    return WebBrowserReadyState.Uninitialized;
                }
                else {
                    return (WebBrowserReadyState)this.AxIWebBrowser2.ReadyState;
                }
            }
        }

        /// <include file='doc\WebBrowser.uex' path='docs/doc[@for="WebBrowser.StatusText"]/*' />
        /// <devdoc>
        ///     <para>
        /// The text that would be displayed in the IE status bar.
        /// There is no direct WebBrowser property that maps to this. This property is
        /// initially an empty string.  After that the value is kept up to date via the
        /// DWebBrowserEvents2:StatusTextChange event.  
        /// Requires WebPermission for Url specified by Document.Domain (indirect access
        /// to user browsing stats).
        ///     </para>
        /// </devdoc>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public virtual string StatusText {
            get {
                if (this.Document == null) {
                    statusText = "";
                }
                return statusText;
            }
        }

        /// <include file='doc\WebBrowser.uex' path='docs/doc[@for="WebBrowser.Url"]/*' />
        /// <devdoc>
        ///     <para>
        /// The url of the HtmlDocument for page hosted in the html page.
        /// Get Maps to IWebBrowser2:LocationUrl.  Set is the equivalent of calling Navigate(Url).
        /// Note this means that setting the Url property & then reading it immediately may not
        /// return the result that you just set (since the get always returns the url you are currently at).
        ///     </para>
        /// </devdoc>
        [
            SRDescription(nameof(SR.WebBrowserUrlDescr)),
            Bindable(true),
            SRCategory(nameof(SR.CatBehavior)),
            TypeConverter(typeof(System.Windows.Forms.WebBrowserUriTypeConverter)),
            DefaultValue(null)
        ]
        public Uri Url {
            get {
                string urlString = this.AxIWebBrowser2.LocationURL;
                //NOTE: If we weren't going to require FullTrust, we'd need to require permissions here
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
            set {
                if (value != null && value.ToString() == "")
                {
                    value = null;
                }
                this.PerformNavigateHelper(ReadyNavigateToUrl(value), false, null, null, null);
            }
        }

        /// <include file='doc\WebBrowser.uex' path='docs/doc[@for="WebBrowser.Version"]/*' />
        /// <devdoc>
        ///     <para>
        /// Returns the version property of IE.
        /// Determined by reading the file version of mshtml.dll in the %system% directory.
        ///     </para>
        /// </devdoc>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Version Version {
            get {
                string mshtmlPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), "mshtml.dll");
                FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(mshtmlPath);
                return new Version(fvi.FileMajorPart, fvi.FileMinorPart, fvi.FileBuildPart, fvi.FilePrivatePart);
            }
        }
        


        //
        // Public methods:
        //

        /// <include file='doc\WebBrowser.uex' path='docs/doc[@for="WebBrowser.GoBack"]/*' />
        /// <devdoc>
        ///     <para>
        /// Navigates the browser to the previous page in the navigation history list.
        /// Maps to IWebBrowser2:GoBack.
        /// Returns true if the operation succeeds, else returns false.  It will return
        /// false if there is no page in the navigation history to go back to.
        ///     </para>
        /// </devdoc>
        public bool GoBack() {
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
        
        /// <include file='doc\WebBrowser.uex' path='docs/doc[@for="WebBrowser.GoForward"]/*' />
        /// <devdoc>
        ///     <para>
        /// Navigates the browser to the next page in the navigation history list.
        /// Maps to IWebBrowser2:GoForward.
        /// Returns true if the operation succeeds, else returns false.  It will return
        /// false if there is no page in the navigation history to go forward to.
        ///     </para>
        /// </devdoc>
        public bool GoForward() {
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
        
        /// <include file='doc\WebBrowser.uex' path='docs/doc[@for="WebBrowser.GoHome"]/*' />
        /// <devdoc>
        ///     <para>
        /// Navigates the browser to user's homepage.  Maps to IWebBrowser2:GoHome.
        ///     </para>
        /// </devdoc>
        public void GoHome() {
            this.AxIWebBrowser2.GoHome();
        }
        
        /// <include file='doc\WebBrowser.uex' path='docs/doc[@for="WebBrowser.GoSearch"]/*' />
        /// <devdoc>
        ///     <para>
        /// Navigates the browser to user's default search page.  Maps to IWebBrowser2:GoSearch.
        ///     </para>
        /// </devdoc>
        public void GoSearch() {
            this.AxIWebBrowser2.GoSearch();
        }

        /// <include file='doc\WebBrowser.uex' path='docs/doc[@for="WebBrowser.Navigate3"]/*' />
        /// <devdoc>
        ///     <para>
        /// Navigates to the specified Uri's AbsolutePath
        ///     </para>
        /// </devdoc>
        public void Navigate(Uri url)
        {
            Url = url; // Does null check in PerformNavigate2
        }

        /// <include file='doc\WebBrowser.uex' path='docs/doc[@for="WebBrowser.Navigate3"]/*' />
        /// <devdoc>
        ///     <para>
        /// String overload for Navigate(Uri)
        ///     </para>
        /// </devdoc>
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

        /// <include file='doc\WebBrowser.uex' path='docs/doc[@for="WebBrowser.Navigate1"]/*' />
        /// <devdoc>
        ///     <para>
        /// Navigates the specified frame to the specified URL.
        /// If the frame name is invalid, it opens a new window (not ideal, but it's the current behavior).
        /// Maps to IWebBrowser2:Navigate.
        ///     </para>
        /// </devdoc>
        public void Navigate(Uri url, string targetFrameName)
        {
            PerformNavigateHelper(ReadyNavigateToUrl(url), false, targetFrameName, null, null);
        }

        /// <include file='doc\WebBrowser.uex' path='docs/doc[@for="WebBrowser.Navigate1"]/*' />
        /// <devdoc>
        ///     <para>
        /// String overload for Navigate(Uri, string)
        ///     </para>
        /// </devdoc>
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

        /// <include file='doc\WebBrowser.uex' path='docs/doc[@for="WebBrowser.Navigate2"]/*' />
        /// <devdoc>
        ///     <para>
        /// Opens a new window if newWindow is true, navigating it to the specified URL. Maps to IWebBrowser2:Navigate.
        ///     </para>
        /// </devdoc>
        public void Navigate(Uri url, bool newWindow) {
            PerformNavigateHelper(ReadyNavigateToUrl(url), newWindow, null, null, null);
        }

        /// <include file='doc\WebBrowser.uex' path='docs/doc[@for="WebBrowser.Navigate2"]/*' />
        /// <devdoc>
        ///     <para>
        /// String overload for Navigate(Uri, bool)
        ///     </para>
        /// </devdoc>
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

        /// <include file='doc\WebBrowser.uex' path='docs/doc[@for="WebBrowser.Navigate4"]/*' />
        /// <devdoc>
        ///     <para>
        /// Navigates to the specified Uri's AbsolutePath with specified args
        ///     </para>
        /// </devdoc>
        public void Navigate(Uri url, string targetFrameName, byte[] postData, string additionalHeaders)
        {
            PerformNavigateHelper(ReadyNavigateToUrl(url), false, targetFrameName, postData, additionalHeaders);
        }

        /// <include file='doc\WebBrowser.uex' path='docs/doc[@for="WebBrowser.Navigate4"]/*' />
        /// <devdoc>
        ///     <para>
        /// String overload for Navigate(Uri, string, byte[], string)
        ///     </para>
        /// </devdoc>
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

        /// <include file='doc\WebBrowser.uex' path='docs/doc[@for="WebBrowser.Print"]/*' />
        /// <devdoc>
        ///     <para>
        /// Prints the html document to the default printer w/ no print dialog.
        /// Maps to IWebBrowser2:ExecWB w/ IDM_PRINT flag & LECMDEXECOPT_DONTPROMPTUSER.
        ///     </para>
        /// </devdoc>
        public void Print() {
            IntSecurity.DefaultPrinting.Demand();

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
        
        /// <include file='doc\WebBrowser.uex' path='docs/doc[@for="WebBrowser.Refresh"]/*' />
        /// <devdoc>
        ///     <para>
        /// Refreshes the current page.  Maps to IWebBrowser2:Refresh.
        ///     </para>
        /// </devdoc>
        public override void Refresh() {
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
        
        /// <include file='doc\WebBrowser.uex' path='docs/doc[@for="WebBrowser.Refresh1"]/*' />
        /// <devdoc>
        ///     <para>
        /// Refreshes the current page w/ the specified refresh option. The refresh option
        /// controls how much is loaded out of the browser cache vs. rechecking the server for.
        /// Maps to IWebBrowser2:Refresh2
        ///     </para>
        /// </devdoc>
        public void Refresh(WebBrowserRefreshOption opt) {
            object level = (object) opt;
            try {
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
            catch (Exception ex) {
                if (ClientUtils.IsSecurityOrCriticalException(ex)) {
                    throw;
                }
            }
        }
        
        /// <include file='doc\WebBrowser.uex' path='docs/doc[@for="WebBrowser.ScrollBarsEnabled"]/*' />
        /// <devdoc>
        ///     <para>
        /// Enables/disables the webbrowser's scrollbars.
        ///     </para>
        /// </devdoc>
        [SRDescription(nameof(SR.WebBrowserScrollBarsEnabledDescr)),
        SRCategory(nameof(SR.CatBehavior)), DefaultValue(true)]
        public bool ScrollBarsEnabled {
            get {
                return webBrowserState[WEBBROWSERSTATE_scrollbarsEnabled];
            }
            set {
                if (value != webBrowserState[WEBBROWSERSTATE_scrollbarsEnabled]) {
                    webBrowserState[WEBBROWSERSTATE_scrollbarsEnabled] = value;
                    this.Refresh();
                }
            }
        }
        
        /// <include file='doc\WebBrowser.uex' path='docs/doc[@for="WebBrowser.ShowPageSetupDialog"]/*' />
        /// <devdoc>
        ///     <para>
        /// Opens the IE page setup dialog for the current page.
        /// Maps to IWebBrowser2:ExecWebBrowser w/ IDM_PAGESETUP flag & LECMDEXECOPT_PROMPTUSER.
        ///     </para>
        /// </devdoc>
        public void ShowPageSetupDialog() {
            IntSecurity.SafePrinting.Demand();

            object nullObjectArray = null;
            try {
                this.AxIWebBrowser2.ExecWB(NativeMethods.OLECMDID.OLECMDID_PAGESETUP, NativeMethods.OLECMDEXECOPT.OLECMDEXECOPT_PROMPTUSER, ref nullObjectArray, IntPtr.Zero);
            }
            catch (Exception ex) {
                if (ClientUtils.IsSecurityOrCriticalException(ex)) {
                    throw;
                }
            }
        }
        
        /// <include file='doc\WebBrowser.uex' path='docs/doc[@for="WebBrowser.ShowPrintDialog"]/*' />
        /// <devdoc>
        ///     <para>
        /// Opens the IE print dialog.
        /// Maps to IWebBrowser2:ExecWebBrowser w/ IDM_PRINT flag & OLECMDEXECOPT_PROMPTUSER.
        ///     </para>
        /// </devdoc>
        public void ShowPrintDialog() {
            IntSecurity.SafePrinting.Demand();

            object nullObjectArray = null;
            
            try {
                this.AxIWebBrowser2.ExecWB(NativeMethods.OLECMDID.OLECMDID_PRINT, NativeMethods.OLECMDEXECOPT.OLECMDEXECOPT_PROMPTUSER, ref nullObjectArray, IntPtr.Zero);
            }
            catch (Exception ex) {
                if (ClientUtils.IsSecurityOrCriticalException(ex)) {
                    throw;
                }
            }
        }
        
        /// <include file='doc\WebBrowser.uex' path='docs/doc[@for="WebBrowser.ShowPrintPreviewDialog"]/*' />
        /// <devdoc>
        ///     <para>
        /// Opens the IE print preview dialog.  Maps to IWebBrowser2:ExecWebBrowser w/ IDM_PRINTPREVIEW flag.
        ///     </para>
        /// </devdoc>
        public void ShowPrintPreviewDialog() {
            IntSecurity.SafePrinting.Demand();

            object nullObjectArray = null;
            
            try {
                this.AxIWebBrowser2.ExecWB(NativeMethods.OLECMDID.OLECMDID_PRINTPREVIEW, NativeMethods.OLECMDEXECOPT.OLECMDEXECOPT_PROMPTUSER, ref nullObjectArray, IntPtr.Zero);
            }
            catch (Exception ex) {
                if (ClientUtils.IsSecurityOrCriticalException(ex)) {
                    throw;
                }
            }
        }
        
        /// <include file='doc\WebBrowser.uex' path='docs/doc[@for="WebBrowser.ShowPropertiesDialog"]/*' />
        /// <devdoc>
        ///     <para>
        /// Opens the properties dialog for the current html page.
        /// Maps to IWebBrowser2:ExecWebBrowser w/ IDM_PROPERTIES flag & LECMDEXECOPT_PROMPTUSER.
        ///     </para>
        /// </devdoc>
        public void ShowPropertiesDialog() {
            object nullObjectArray = null;
            
            try {
                this.AxIWebBrowser2.ExecWB(NativeMethods.OLECMDID.OLECMDID_PROPERTIES, NativeMethods.OLECMDEXECOPT.OLECMDEXECOPT_PROMPTUSER, ref nullObjectArray, IntPtr.Zero);
            }
            catch (Exception ex) {
                if (ClientUtils.IsSecurityOrCriticalException(ex)) {
                    throw;
                }
            }
        }
        
        /// <include file='doc\WebBrowser.uex' path='docs/doc[@for="WebBrowser.ShowSaveAsDialog"]/*' />
        /// <devdoc>
        ///     <para>
        /// Opens the IE File-Save dialog.
        /// Maps to IWebBrowser2:ExecWebBrowser w/ IDM_SAVEAS flag & LECMDEXECOPT_PROMPTUSER.
        ///     </para>
        /// </devdoc>
        public void ShowSaveAsDialog() {
            IntSecurity.FileDialogSaveFile.Demand();

            object nullObjectArray = null;
            
            try {
                this.AxIWebBrowser2.ExecWB(NativeMethods.OLECMDID.OLECMDID_SAVEAS, NativeMethods.OLECMDEXECOPT.OLECMDEXECOPT_DODEFAULT, ref nullObjectArray, IntPtr.Zero);
            }
            catch (Exception ex) {
                if (ClientUtils.IsSecurityOrCriticalException(ex)) {
                    throw;
                }
            }
        }
        
        /// <include file='doc\WebBrowser.uex' path='docs/doc[@for="WebBrowser.Stop"]/*' />
        /// <devdoc>
        ///     <para>
        /// Stops the current navigation.  Maps to IWebBrowser2:Stop.
        ///     </para>
        /// </devdoc>
        public void Stop() {
            try {
                this.AxIWebBrowser2.Stop();
            }
            catch (Exception ex) {
                if (ClientUtils.IsSecurityOrCriticalException(ex)) {
                    throw;
                }
            }
        }

        //
        // Public events:
        //
        /// <include file='doc\WebBrowser.uex' path='docs/doc[@for="WebBrowser.CanGoBackChanged"]/*' />
        /// <devdoc>
        ///     <para>
        /// Occurs when the IE back button would change from enabled to disabled or vice versa.
        /// Maps to DWebBrowserEvents2:CommandStateChange w/ CSC_NAVIGATEBACK.
        ///     </para>
        /// </devdoc>
        [Browsable(false), SRCategory(nameof(SR.CatPropertyChanged)), SRDescription(nameof(SR.WebBrowserCanGoBackChangedDescr))]
        public event EventHandler CanGoBackChanged;
        /// <include file='doc\WebBrowser.uex' path='docs/doc[@for="WebBrowser.CanGoForwardChanged"]/*' />
        /// <devdoc>
        ///     <para>
        /// Occurs when the IE forward button would change from enabled to disabled or vice versa.
        /// Maps to DWebBrowserEvents2:CommandStateChange w/ CSC_NAVIGATEFORWARD.
        ///     </para>
        /// </devdoc>
        [Browsable(false), SRCategory(nameof(SR.CatPropertyChanged)), SRDescription(nameof(SR.WebBrowserCanGoForwardChangedDescr))]
        public event EventHandler CanGoForwardChanged;
        /// <include file='doc\WebBrowser.uex' path='docs/doc[@for="WebBrowser.DocumentCompleted"]/*' />
        /// <devdoc>
        ///     <para>
        /// Occurs when the document hosted in the web browser control is fully loaded.
        /// This is conceptially similar to Form.Load().  You need to wait until this event fires
        /// before doing anything that manipulates the html page, ex. reading the Document
        /// property of the webbrowser control. Maps to DWebBrowserEvents2:DocumentComplete.
        ///     </para>
        /// </devdoc>
        [SRCategory(nameof(SR.CatBehavior)), SRDescription(nameof(SR.WebBrowserDocumentCompletedDescr))]
        public event WebBrowserDocumentCompletedEventHandler DocumentCompleted;
        /// <include file='doc\WebBrowser.uex' path='docs/doc[@for="WebBrowser.DocumentTitleChanged"]/*' />
        /// <devdoc>
        ///     <para>
        /// Occurs whenever the title text changes. The Title is the html page title
        /// or the file path/url if not title is available. This is the text you see as
        /// the title of the IE window preceeding "Microsoft Internet Explorer".
        /// Maps to DWebBrowserEvents2:TitleChange.
        ///     </para>
        /// </devdoc>
        [Browsable(false), SRCategory(nameof(SR.CatPropertyChanged)), SRDescription(nameof(SR.WebBrowserDocumentTitleChangedDescr))]
        public event EventHandler DocumentTitleChanged;
        /// <include file='doc\WebBrowser.uex' path='docs/doc[@for="WebBrowser.EncryptionLevelChanged"]/*' />
        /// <devdoc>
        ///     <para>
        /// Occurs whenever encryption level changes.
        /// Can be used to set a custom security lock icon similar to what IE shows when
        /// you go to an https site. Maps to DWebBrowserEvents2:SetSecureLockIcon.
        ///     </para>
        /// </devdoc>
        [Browsable(false), SRCategory(nameof(SR.CatPropertyChanged)), SRDescription(nameof(SR.WebBrowserEncryptionLevelChangedDescr))]
        public event EventHandler EncryptionLevelChanged;
        /// <include file='doc\WebBrowser.uex' path='docs/doc[@for="WebBrowser.FileDownload"]/*' />
        /// <devdoc>
        ///     <para>
        /// Occurs when a file download occurs.
        /// Can be used to cancel file downloads. Maps to DWebBrowserEvents2:FileDownload.
        ///     </para>
        /// </devdoc>
        [SRCategory(nameof(SR.CatBehavior)), SRDescription(nameof(SR.WebBrowserFileDownloadDescr))]
        public event EventHandler FileDownload;
        /// <include file='doc\WebBrowser.uex' path='docs/doc[@for="WebBrowser.Navigated"]/*' />
        /// <devdoc>
        ///     <para>
        /// Occurs after browser control navigation occurs.
        /// Fires after browser navigation is complete. Maps to DWebBrowserEvents2:NavigateComplete.
        ///     </para>
        /// </devdoc>
        [SRCategory(nameof(SR.CatAction)), SRDescription(nameof(SR.WebBrowserNavigatedDescr))]
        public event WebBrowserNavigatedEventHandler Navigated;
        /// <include file='doc\WebBrowser.uex' path='docs/doc[@for="WebBrowser.Navigating"]/*' />
        /// <devdoc>
        ///     <para>
        /// Occurs before browser control navigation occurs.
        /// Fires before browser navigation occurs. Allows navigation to be canceled if
        /// NavigatingEventArgs.Cancel is set to false. Maps to DWebBrowserEvents2:BeforeNavigate2.
        ///     </para>
        /// </devdoc>
        [SRCategory(nameof(SR.CatAction)), SRDescription(nameof(SR.WebBrowserNavigatingDescr))]
        public event WebBrowserNavigatingEventHandler Navigating;
        /// <include file='doc\WebBrowser.uex' path='docs/doc[@for="WebBrowser.NewWindow"]/*' />
        /// <devdoc>
        ///     <para>
        /// Occurs when a new browser window is created.
        /// Can be used to cancel the creation of the new browser window. Maps to DWebBrowserEvents2:NewWindow2.
        ///     </para>
        /// </devdoc>
        [SRCategory(nameof(SR.CatAction)), SRDescription(nameof(SR.WebBrowserNewWindowDescr))]
        public event CancelEventHandler NewWindow;
        /// <include file='doc\WebBrowser.uex' path='docs/doc[@for="WebBrowser.ProgressChanged"]/*' />
        /// <devdoc>
        ///     <para>
        /// Occurs when an update to the progress of a download occurs.
        /// Fires whenever the browser control has updated info on the download. Can be
        /// used to provide a download status bar and display the number of bytes downloaded.
        /// Maps to DWebBrowserEvents2:ProgressChange.
        ///     </para>
        /// </devdoc>
        [SRCategory(nameof(SR.CatAction)), SRDescription(nameof(SR.WebBrowserProgressChangedDescr))]
        public event WebBrowserProgressChangedEventHandler ProgressChanged;
        /// <include file='doc\WebBrowser.uex' path='docs/doc[@for="WebBrowser.StatusTextChanged"]/*' />
        /// <devdoc>
        ///     <para>
        /// Occurs whenever the status text changes.
        /// Can be used to keep a status bar populated with uptodate text.
        /// Maps to DWebBrowserEvents2:StatusTextChange.
        ///     </para>
        /// </devdoc>
        [Browsable(false), SRCategory(nameof(SR.CatPropertyChanged)), SRDescription(nameof(SR.WebBrowserStatusTextChangedDescr))]
        public event EventHandler StatusTextChanged;



        //
        // public overrides:
        //

        /// <include file='doc\WebBrowser.uex' path='docs/doc[@for="WebBrowser.Focused"]/*' />
        /// <devdoc>
        ///     Returns true if this control (or any of its child windows) has focus.
        /// </devdoc>
        public override bool Focused {
            get {
                if (base.Focused) {
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
        /// <include file='doc\WebBrowser.uex' path='docs/doc[@for="WebBrowser.Dispose"]/*' />
        protected override void Dispose(bool disposing) {
            if (disposing) {
                if (htmlShimManager != null)
                {
                    htmlShimManager.Dispose();
                }
                DetachSink();
                ActiveXSite.Dispose();
            }
            base.Dispose(disposing);
        }

        /// <include file='doc\WebBrowser.uex' path='docs/doc[@for="WebBrowser.DefaultSize"]/*' />
        /// <devdoc>
        ///     <para>
        /// Overrides the default size property of Control to specify a bigger default size of 250 x 250.
        ///     </para>
        /// </devdoc>
        protected override Size DefaultSize {
            get {
                return new Size(250, 250);
            }
        }

        /// <include file='doc\WebBrowser.uex' path='docs/doc[@for="WebBrowser.AttachInterfaces"]/*' />
        /// <devdoc>
        ///     <para>
        /// Retrieves IWebBrowser2 from the native object. Overriding classes should first call base.AttachInterfaces.
        ///     </para>
        /// </devdoc>
        protected override void AttachInterfaces(object nativeActiveXObject) {
            this.axIWebBrowser2 = (UnsafeNativeMethods.IWebBrowser2)nativeActiveXObject;
        }

        /// <include file='doc\WebBrowser.uex' path='docs/doc[@for="WebBrowser.DetachInterfaces"]/*' />
        /// <devdoc>
        ///     <para>
        /// Discards the IWebBrowser2 reference. Overriding classes should call base.DetachInterfaces.
        ///     </para>
        /// </devdoc>
        protected override void DetachInterfaces() {
            this.axIWebBrowser2 = null;
        }
        
        /// <include file='doc\WebBrowser.uex' path='docs/doc[@for="WebBrowser.CreateWebBrowserSite"]/*' />
        /// <devdoc>
        ///     <para>
        /// Returns a WebBrowserSite object.
        ///     </para>
        /// </devdoc>
        protected override WebBrowserSiteBase CreateWebBrowserSiteBase() {
            return new WebBrowserSite(this);
        }

        /// <include file='doc\WebBrowser.uex' path='docs/doc[@for="WebBrowser.CreateSink"]/*' />
        /// <devdoc>
        ///     <para>
        /// Attaches to the DWebBrowserEvents2 connection point.
        ///     </para>
        /// </devdoc>
        protected override void CreateSink() {
            object ax = this.activeXInstance;
            if (ax != null) {
                webBrowserEvent = new WebBrowserEvent(this);
                webBrowserEvent.AllowNavigation = AllowNavigation;
                this.cookie = new AxHost.ConnectionPointCookie(ax, webBrowserEvent,
                        typeof(UnsafeNativeMethods.DWebBrowserEvents2));
            }
        }
        
        /// <include file='doc\WebBrowser.uex' path='docs/doc[@for="WebBrowser.DetachSink"]/*' />
        /// <devdoc>
        ///     <para>
        /// Releases the DWebBrowserEvents2 connection point.
        ///     </para>
        /// </devdoc>
        protected override void DetachSink() {
            //If we have a cookie get rid of it
            if (this.cookie != null) {
                this.cookie.Disconnect();
                this.cookie = null;
            }
        }

        /// <include file='doc\WebBrowser.uex' path='docs/doc[@for="WebBrowser.OnTopMostActiveXParent"]/*' />
        internal override void OnTopMostActiveXParentChanged(EventArgs e) {
            if (TopMostParent.IsIEParent) {
                WebBrowser.createdInIE = true;
                CheckIfCreatedInIE();
            }
            else {
                WebBrowser.createdInIE = false;
                base.OnTopMostActiveXParentChanged(e);
            }
        }



        //
        // protected virtuals:
        //

        /// <include file='doc\WebBrowser.uex' path='docs/doc[@for="WebBrowser.OnCanGoBackChanged"]/*' />
        /// <devdoc>
        ///     <para>
        /// Raises the <see cref='System.Windows.Forms.WebBrowser.CanGoBackChanged'/> event.
        ///     </para>
        /// </devdoc>
        [SuppressMessage("Microsoft.Security", "CA2109:ReviewVisibleEventHandlers")]
        // 
        protected virtual void OnCanGoBackChanged(EventArgs e)
        {
            if (this.CanGoBackChanged != null)
            {
                this.CanGoBackChanged(this, e);
            }
        }

        /// <include file='doc\WebBrowser.uex' path='docs/doc[@for="WebBrowser.OnCanGoForwardChanged"]/*' />
        /// <devdoc>
        ///     <para>
        /// Raises the <see cref='System.Windows.Forms.WebBrowser.CanGoForwardChanged'/> event.
        ///     </para>
        /// </devdoc>
        [SuppressMessage("Microsoft.Security", "CA2109:ReviewVisibleEventHandlers")]
        // 
        protected virtual void OnCanGoForwardChanged(EventArgs e)
        {
            if (this.CanGoForwardChanged != null)
            {
                this.CanGoForwardChanged(this, e);
            }
        }

        /// <include file='doc\WebBrowser.uex' path='docs/doc[@for="WebBrowser.OnDocumentCompleted"]/*' />
        /// <devdoc>
        ///     <para>
        /// Raises the <see cref='System.Windows.Forms.WebBrowser.DocumentCompleted'/> event.
        ///     </para>
        /// </devdoc>
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

        /// <include file='doc\WebBrowser.uex' path='docs/doc[@for="WebBrowser.OnDocumentTitleChanged"]/*' />
        /// <devdoc>
        ///     <para>
        /// Raises the <see cref='System.Windows.Forms.WebBrowser.DocumentTitleChanged'/> event.
        ///     </para>
        /// </devdoc>
        [SuppressMessage("Microsoft.Security", "CA2109:ReviewVisibleEventHandlers")]
        // 
        protected virtual void OnDocumentTitleChanged(EventArgs e)
        {
            if (this.DocumentTitleChanged != null)
            {
                this.DocumentTitleChanged(this, e);
            }
        }

        /// <include file='doc\WebBrowser.uex' path='docs/doc[@for="WebBrowser.OnEncryptionLevelChanged"]/*' />
        /// <devdoc>
        ///     <para>
        /// Raises the <see cref='System.Windows.Forms.WebBrowser.EncryptionLevelChanged'/> event.
        ///     </para>
        /// </devdoc>
        [SuppressMessage("Microsoft.Security", "CA2109:ReviewVisibleEventHandlers")]
        // 
        protected virtual void OnEncryptionLevelChanged(EventArgs e)
        {
            if (this.EncryptionLevelChanged != null)
            {
                this.EncryptionLevelChanged(this, e);
            }
        }

        /// <include file='doc\WebBrowser.uex' path='docs/doc[@for="WebBrowser.OnFileDownload"]/*' />
        /// <devdoc>
        ///     <para>
        /// Raises the <see cref='System.Windows.Forms.WebBrowser.FileDownload'/> event.
        ///     </para>
        /// </devdoc>
        [SuppressMessage("Microsoft.Security", "CA2109:ReviewVisibleEventHandlers")]
        // 
        protected virtual void OnFileDownload(EventArgs e)
        {
            if (this.FileDownload != null)
            {
                this.FileDownload(this, e);
            }
        }

        /// <include file='doc\WebBrowser.uex' path='docs/doc[@for="WebBrowser.OnNavigated"]/*' />
        /// <devdoc>
        ///     <para>
        /// Raises the <see cref='System.Windows.Forms.WebBrowser.Navigated'/> event.
        ///     </para>
        /// </devdoc>
        [SuppressMessage("Microsoft.Security", "CA2109:ReviewVisibleEventHandlers")]
        // 
        protected virtual void OnNavigated(WebBrowserNavigatedEventArgs e)
        {
            if (this.Navigated != null)
            {
                this.Navigated(this, e);
            }
        }

        /// <include file='doc\WebBrowser.uex' path='docs/doc[@for="WebBrowser.OnNavigating"]/*' />
        /// <devdoc>
        ///     <para>
        /// Raises the <see cref='System.Windows.Forms.WebBrowser.Navigating'/> event.
        ///     </para>
        /// </devdoc>
        [SuppressMessage("Microsoft.Security", "CA2109:ReviewVisibleEventHandlers")]
        // 
        protected virtual void OnNavigating(WebBrowserNavigatingEventArgs e)
        {
            if (this.Navigating != null)
            {
                this.Navigating(this, e);
            }
        }

        /// <include file='doc\WebBrowser.uex' path='docs/doc[@for="WebBrowser.OnNewWindow"]/*' />
        /// <devdoc>
        ///     <para>
        /// Raises the <see cref='System.Windows.Forms.WebBrowser.NewWindow'/> event.
        ///     </para>
        /// </devdoc>
        [SuppressMessage("Microsoft.Security", "CA2109:ReviewVisibleEventHandlers")]
        // 
        protected virtual void OnNewWindow(CancelEventArgs e)
        {
            if (this.NewWindow != null)
            {
                this.NewWindow(this, e);
            }
        }

        /// <include file='doc\WebBrowser.uex' path='docs/doc[@for="WebBrowser.OnProgressChanged"]/*' />
        /// <devdoc>
        ///     <para>
        /// Raises the <see cref='System.Windows.Forms.WebBrowser.ProgressChanged'/> event.
        ///     </para>
        /// </devdoc>
        [SuppressMessage("Microsoft.Security", "CA2109:ReviewVisibleEventHandlers")]
        // 
        protected virtual void OnProgressChanged(WebBrowserProgressChangedEventArgs e)
        {
            if (this.ProgressChanged != null)
            {
                this.ProgressChanged(this, e);
            }
        }

        /// <include file='doc\WebBrowser.uex' path='docs/doc[@for="WebBrowser.OnStatusTextChanged"]/*' />
        /// <devdoc>
        ///     <para>
        /// Raises the <see cref='System.Windows.Forms.WebBrowser.StatusTextChanged'/> event.
        ///     </para>
        /// </devdoc>
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
       internal HtmlShimManager ShimManager {
            get {
                if (htmlShimManager == null) {
                    htmlShimManager = new HtmlShimManager();
                }
                return htmlShimManager;
            }
        }
#endregion


        //
        // Private methods:
        //
        private void CheckIfCreatedInIE() {
            if (WebBrowser.createdInIE) {
                if (this.ParentInternal != null) {
                    this.ParentInternal.Controls.Remove(this);
                    this.Dispose();
                }
                else {
                    this.Dispose();
                    throw new NotSupportedException(SR.WebBrowserInIENotSupported);
                }
            }
        }

        [SuppressMessage("Microsoft.Security", "CA2103:ReviewImperativeSecurity")]
        [SuppressMessage("Microsoft.Security", "CA2122:DoNotIndirectlyExposeMethodsWithLinkDemands")]
        internal static void EnsureUrlConnectPermission(Uri url) {
            //
            WebPermission permission = new WebPermission(NetworkAccess.Connect, url.AbsoluteUri);
            permission.Demand();
        }

        private string ReadyNavigateToUrl(string urlString) {
            if (string.IsNullOrEmpty(urlString)) {
                urlString = "about:blank";
            } 

            //
            // Nullify any calls to set_DocumentStream which may still be pending
            if (!webBrowserState[WEBBROWSERSTATE_documentStreamJustSet]) {
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
            object objFlags = (object) (newWindow ? 1 : 0);
            object objTargetFrameName = (object)targetFrameName;
            object objPostData = (object)postData;
            object objHeaders = (object)headers;
            PerformNavigate2(ref objUrlString, ref objFlags, ref objTargetFrameName, ref objPostData, ref objHeaders);
        }

        private void PerformNavigate2(ref object URL, ref object flags, ref object targetFrameName, ref object postData, ref object headers) 
        {
            try {
                this.AxIWebBrowser2.Navigate2(ref URL, ref flags, ref targetFrameName, ref postData, ref headers);
            }
            catch (COMException ce) {
                if ((uint)unchecked(ce.ErrorCode) != (uint)unchecked(0x800704c7)) {
                    // "the operation was canceled by the user" - navigation failed
                    // ignore this error, IE has already alerted the user. 
                    throw;
                }
            }
        }

        private bool ShouldSerializeDocumentText() {
            return IsValidUrl;
        }

        bool IsValidUrl
        {
            get
            {
                return Url == null || Url.AbsoluteUri == "about:blank";
            }
        }
        
        private bool ShouldSerializeUrl() {
            return !ShouldSerializeDocumentText();
        }

        /// <devdoc>
        ///     Returns TRUE if there is a context menu to show
        ///     Returns FALSE otherwise
        /// </devdoc>
        private bool ShowContextMenu(int x, int y) {
            ContextMenuStrip contextMenuStrip = ContextMenuStrip;
            ContextMenu contextMenu = contextMenuStrip != null ? null : ContextMenu;

            if (contextMenuStrip != null || contextMenu != null) {
                Point client;
                bool keyboardActivated = false;
                // X will be exactly -1 when the user invokes the context menu from the keyboard
                //
                if (x == -1) {
                    keyboardActivated = true;
                    client = new Point(Width / 2, Height / 2);
                } else {
                    client = PointToClientInternal(new Point(x, y));
                }

                if (ClientRectangle.Contains(client)) {
                    if (contextMenuStrip != null) {
                        contextMenuStrip.ShowInternal(this, client, keyboardActivated);
                    } else if (contextMenu != null) {
                        contextMenu.Show(this, client);
                    }

                    return true;
                } else {
                    return false;
                }
            } else {
                return false;
            }
        }

        /// <include file='doc\WebBrowser.uex' path='docs/doc[@for="WebBrowser.WndProc"]/*' />
        protected override void WndProc(ref Message m) {
            switch (m.Msg) {
                case NativeMethods.WM_CONTEXTMENU:
                    int x = NativeMethods.Util.SignedLOWORD(m.LParam);
                    int y = NativeMethods.Util.SignedHIWORD(m.LParam);

                    if (!ShowContextMenu(x, y)) {
                        DefWndProc(ref m);
                    }
                    break;
                default:
                    base.WndProc(ref m);
                    break;
            }
        }
        
        private UnsafeNativeMethods.IWebBrowser2 AxIWebBrowser2 {
            get {
                if (this.axIWebBrowser2 == null) {
                    if (!this.IsDisposed) {
                        // This should call AttachInterfaces
                        TransitionUpTo(WebBrowserHelper.AXState.InPlaceActive);
                    }
                    else {
                        throw new System.ObjectDisposedException(GetType().Name);
                    }
                }
                // We still don't have this.axIWebBrowser2. Throw an exception.
                if (this.axIWebBrowser2 == null) {
                    throw new InvalidOperationException(SR.WebBrowserNoCastToIWebBrowser2); 
                }
                return this.axIWebBrowser2;
            }
        }



        //
        // WebBrowserSite class:
        //
        //
        // We slap InheritanceDemand on this class so that only users with
        // UnmanagedCode permissions can override this type.
        //
        /// <include file='doc\WebBrowser.uex' path='docs/doc[@for="WebBrowser.WebBrowserSite"]/*' />
        /// <devdoc>
        ///     <para>
        /// Provides a default WebBrowserSite implementation for use in the CreateWebBrowserSite
        /// method in the WebBrowser class. 
        ///     </para>
        /// </devdoc>
        [SecurityPermission(SecurityAction.InheritanceDemand, Flags=SecurityPermissionFlag.UnmanagedCode),
         ComVisible(false)]
        protected class WebBrowserSite : WebBrowserSiteBase, UnsafeNativeMethods.IDocHostUIHandler
        {
            /// <include file='doc\WebBrowser.uex' path='docs/doc[@for="WebBrowser.WebBrowserSite.WebBrowserSite"]/*' />
            /// <devdoc>
            ///     <para>
            /// Creates an instance of the <see cref='System.Windows.Forms.WebBrowser.WebBrowserSite'/> class.
            ///     </para>
            /// </devdoc>
            [SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.UnmanagedCode)]
            public WebBrowserSite(WebBrowser host) : base(host) {
            }


            //
            // IDocHostUIHandler Implementation
            //
            /// <include file='doc\WebBrowser.uex' path='docs/doc[@for="WebBrowser.WebBrowserSite.UnsafeNativeMethods.IDocHostUIHandler.ShowContextMenu"]/*' />
            /// <internalonly/>
            int UnsafeNativeMethods.IDocHostUIHandler.ShowContextMenu(int dwID, NativeMethods.POINT pt, object pcmdtReserved, object pdispReserved) {
                WebBrowser wb = (WebBrowser)this.Host;

                if (wb.IsWebBrowserContextMenuEnabled) {
                    // let MSHTML display its UI
                    return NativeMethods.S_FALSE;
                } else {
                    if (pt.x == 0 && pt.y == 0) {
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


            /// <include file='doc\WebBrowser.uex' path='docs/doc[@for="WebBrowser.WebBrowserSite.UnsafeNativeMethods.IDocHostUIHandler.GetHostInfo"]/*' />
            /// <internalonly/>
            int UnsafeNativeMethods.IDocHostUIHandler.GetHostInfo(NativeMethods.DOCHOSTUIINFO info) {
                WebBrowser wb = (WebBrowser)this.Host;

                info.dwDoubleClick = (int)NativeMethods.DOCHOSTUIDBLCLICK.DEFAULT;
                info.dwFlags = (int)NativeMethods.DOCHOSTUIFLAG.NO3DOUTERBORDER |
                                (int)NativeMethods.DOCHOSTUIFLAG.DISABLE_SCRIPT_INACTIVE;

                if (wb.ScrollBarsEnabled) {
                    info.dwFlags |= (int)NativeMethods.DOCHOSTUIFLAG.FLAT_SCROLLBAR;
                }
                else {
                    info.dwFlags |= (int)NativeMethods.DOCHOSTUIFLAG.SCROLL_NO;
                }

                if (Application.RenderWithVisualStyles) {
                    info.dwFlags |= (int)NativeMethods.DOCHOSTUIFLAG.THEME;
                }
                else {
                    info.dwFlags |= (int)NativeMethods.DOCHOSTUIFLAG.NOTHEME;
                }
                
                return NativeMethods.S_OK;
            }

            /// <include file='doc\WebBrowser.uex' path='docs/doc[@for="WebBrowser.WebBrowserSite.UnsafeNativeMethods.IDocHostUIHandler.EnableModeless"]/*' />
            /// <internalonly/>
            int UnsafeNativeMethods.IDocHostUIHandler.EnableModeless(bool fEnable) {
                return NativeMethods.E_NOTIMPL;
            }

            /// <include file='doc\WebBrowser.uex' path='docs/doc[@for="WebBrowser.WebBrowserSite.UnsafeNativeMethods.IDocHostUIHandler.ShowUI"]/*' />
            /// <internalonly/>
            int UnsafeNativeMethods.IDocHostUIHandler.ShowUI(int dwID, UnsafeNativeMethods.IOleInPlaceActiveObject activeObject, 
                    NativeMethods.IOleCommandTarget commandTarget, UnsafeNativeMethods.IOleInPlaceFrame frame, 
                    UnsafeNativeMethods.IOleInPlaceUIWindow doc) {
                return NativeMethods.S_FALSE;
            }

            /// <include file='doc\WebBrowser.uex' path='docs/doc[@for="WebBrowser.WebBrowserSite.UnsafeNativeMethods.IDocHostUIHandler.HideUI"]/*' />
            /// <internalonly/>
            int UnsafeNativeMethods.IDocHostUIHandler.HideUI() {
                return NativeMethods.E_NOTIMPL;
            }

            /// <include file='doc\WebBrowser.uex' path='docs/doc[@for="WebBrowser.WebBrowserSite.UnsafeNativeMethods.IDocHostUIHandler.UpdateUI"]/*' />
            /// <internalonly/>
            int UnsafeNativeMethods.IDocHostUIHandler.UpdateUI() {
                return NativeMethods.E_NOTIMPL;
            }

            /// <include file='doc\WebBrowser.uex' path='docs/doc[@for="WebBrowser.WebBrowserSite.UnsafeNativeMethods.IDocHostUIHandler.OnDocWindowActivate"]/*' />
            /// <internalonly/>
            int UnsafeNativeMethods.IDocHostUIHandler.OnDocWindowActivate(bool fActivate) {
                return NativeMethods.E_NOTIMPL;
            }

            /// <include file='doc\WebBrowser.uex' path='docs/doc[@for="WebBrowser.WebBrowserSite.UnsafeNativeMethods.IDocHostUIHandler.OnFrameWindowActivate"]/*' />
            /// <internalonly/>
            int UnsafeNativeMethods.IDocHostUIHandler.OnFrameWindowActivate(bool fActivate) {
                return NativeMethods.E_NOTIMPL;
            }

            /// <include file='doc\WebBrowser.uex' path='docs/doc[@for="WebBrowser.WebBrowserSite.UnsafeNativeMethods.IDocHostUIHandler.ResizeBorder"]/*' />
            /// <internalonly/>
            int UnsafeNativeMethods.IDocHostUIHandler.ResizeBorder(NativeMethods.COMRECT rect, UnsafeNativeMethods.IOleInPlaceUIWindow doc, bool fFrameWindow) {
                return NativeMethods.E_NOTIMPL;
            }

            /// <include file='doc\WebBrowser.uex' path='docs/doc[@for="WebBrowser.WebBrowserSite.UnsafeNativeMethods.IDocHostUIHandler.GetOptionKeyPath"]/*' />
            /// <internalonly/>
            int UnsafeNativeMethods.IDocHostUIHandler.GetOptionKeyPath(string[] pbstrKey, int dw) {
                return NativeMethods.E_NOTIMPL;
            }
            
            /// <include file='doc\WebBrowser.uex' path='docs/doc[@for="WebBrowser.WebBrowserSite.UnsafeNativeMethods.IDocHostUIHandler.GetDropTarget"]/*' />
            /// <internalonly/>
            int UnsafeNativeMethods.IDocHostUIHandler.GetDropTarget(UnsafeNativeMethods.IOleDropTarget pDropTarget, out UnsafeNativeMethods.IOleDropTarget ppDropTarget) {
                //
                // Set to null no matter what we return, to prevent the marshaller
                // from having issues if the pointer points to random stuff.
                ppDropTarget = null;
                return NativeMethods.E_NOTIMPL;
            }

            /// <include file='doc\WebBrowser.uex' path='docs/doc[@for="WebBrowser.WebBrowserSite.UnsafeNativeMethods.IDocHostUIHandler.GetExternal"]/*' />
            /// <internalonly/>
            int UnsafeNativeMethods.IDocHostUIHandler.GetExternal(out object ppDispatch) {
                WebBrowser wb = (WebBrowser)this.Host;
                ppDispatch = wb.ObjectForScripting;
                return NativeMethods.S_OK;
            }

            /// <include file='doc\WebBrowser.uex' path='docs/doc[@for="WebBrowser.WebBrowserSite.UnsafeNativeMethods.IDocHostUIHandler.TranslateAccelerator"]/*' />
            /// <internalonly/>
            [SuppressMessage("Microsoft.Performance", "CA1803:AvoidCostlyCallsWherePossible")]                     
            int UnsafeNativeMethods.IDocHostUIHandler.TranslateAccelerator(ref NativeMethods.MSG msg, ref Guid group, int nCmdID) {
                //
                // Returning S_FALSE will allow the native control to do default processing,
                // i.e., execute the shortcut key. Returning S_OK will cancel the shortcut key.

                WebBrowser wb = (WebBrowser)this.Host;
                
                if (!wb.WebBrowserShortcutsEnabled)
                {
                    int keyCode = (int)msg.wParam | (int)Control.ModifierKeys;

                    if (msg.message != NativeMethods.WM_CHAR
                            && Enum.IsDefined(typeof(Shortcut), (Shortcut)keyCode)) {
                        return NativeMethods.S_OK;
                    }
                    return NativeMethods.S_FALSE;
                }
                return NativeMethods.S_FALSE;
            }

            /// <include file='doc\WebBrowser.uex' path='docs/doc[@for="WebBrowser.WebBrowserSite.UnsafeNativeMethods.IDocHostUIHandler.TranslateUrl"]/*' />
            /// <internalonly/>
            int UnsafeNativeMethods.IDocHostUIHandler.TranslateUrl(int dwTranslate, string strUrlIn, out string pstrUrlOut) {
                //
                // Set to null no matter what we return, to prevent the marshaller
                // from having issues if the pointer points to random stuff.
                pstrUrlOut = null;
                return NativeMethods.S_FALSE;
            }

            /// <include file='doc\WebBrowser.uex' path='docs/doc[@for="WebBrowser.WebBrowserSite.UnsafeNativeMethods.IDocHostUIHandler.FilterDataObject"]/*' />
            /// <internalonly/>
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
            internal override void OnPropertyChanged(int dispid) {
                if (dispid != NativeMethods.ActiveX.DISPID_READYSTATE) {
                    base.OnPropertyChanged(dispid);
                }
            }
        }
        
        
        //
        // Private classes:
        //
        [ClassInterface(ClassInterfaceType.None)]
        private class WebBrowserEvent : StandardOleMarshalObject, UnsafeNativeMethods.DWebBrowserEvents2{
            
            private WebBrowser parent;
            private bool allowNavigation;
            private bool haveNavigated = false;

            public WebBrowserEvent(WebBrowser parent) {
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

            public void CommandStateChange(long command, bool enable) {
                if (command == NativeMethods.CSC_NAVIGATEBACK) {
                    this.parent.CanGoBackInternal = enable;
                }
                else if (command == NativeMethods.CSC_NAVIGATEFORWARD) {
                    this.parent.CanGoForwardInternal = enable;
                }
            }
            
            public void BeforeNavigate2(object pDisp, ref object urlObject, ref object flags, ref object targetFrameName, ref object postData, ref object headers, ref bool cancel) {
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
                    if (targetFrameName == null) {
                        targetFrameName = "";
                    }
                    if (headers == null) {
                        headers = "";
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

            public void DocumentComplete(object pDisp, ref object urlObject) {
                Debug.Assert(urlObject == null || urlObject is string, "invalid url");
                haveNavigated = true;
                if (this.parent.documentStreamToSetOnLoad != null && (string)urlObject == "about:blank") {
                    HtmlDocument htmlDocument = this.parent.Document;
                    if (htmlDocument != null) {
                        UnsafeNativeMethods.IPersistStreamInit psi = htmlDocument.DomDocument as UnsafeNativeMethods.IPersistStreamInit;
                        Debug.Assert(psi != null, "The Document does not implement IPersistStreamInit");
                        UnsafeNativeMethods.IStream iStream = (UnsafeNativeMethods.IStream)new UnsafeNativeMethods.ComStreamFromDataStream(
                                                    this.parent.documentStreamToSetOnLoad);
                        psi.Load(iStream);
                        htmlDocument.Encoding = "unicode";
                    }
                    this.parent.documentStreamToSetOnLoad = null;
                }
                else {
                    string urlString = urlObject == null ? "" : urlObject.ToString();
                    WebBrowserDocumentCompletedEventArgs e = new WebBrowserDocumentCompletedEventArgs(
                            new Uri(urlString));
                    this.parent.OnDocumentCompleted(e);
                }
            }
            
            public void TitleChange(string text) {
                this.parent.OnDocumentTitleChanged(EventArgs.Empty);
            }
            
            public void SetSecureLockIcon(int secureLockIcon) {
                this.parent.encryptionLevel = (WebBrowserEncryptionLevel)secureLockIcon;
                this.parent.OnEncryptionLevelChanged(EventArgs.Empty);
            }
            
            public void NavigateComplete2(object pDisp, ref object urlObject) {
                Debug.Assert(urlObject == null || urlObject is string, "invalid url type");
                string urlString = urlObject == null ? "" : (string)urlObject;
                WebBrowserNavigatedEventArgs e = new WebBrowserNavigatedEventArgs(
                        new Uri(urlString));
                this.parent.OnNavigated(e);
            }
            
            public void NewWindow2(ref object ppDisp, ref bool cancel) {
                CancelEventArgs e = new CancelEventArgs();
                this.parent.OnNewWindow(e);
                cancel = e.Cancel;
            }
            
            public void ProgressChange(int progress, int progressMax) {
                WebBrowserProgressChangedEventArgs e = new WebBrowserProgressChangedEventArgs(progress, progressMax);
                this.parent.OnProgressChanged(e);
            }
            
            public void StatusTextChange(string text) {
                this.parent.statusText = (text == null) ? "" : text;
                this.parent.OnStatusTextChanged(EventArgs.Empty);
            }
            
            public void DownloadBegin() {
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

    
    //
    // Public enums:
    //

    /// <include file='doc\WebBrowser.uex' path='docs/doc[@for="WebBrowserEncryptionLevel"]/*' />
    /// <devdoc>
    ///     <para>
    /// Specifies the EncryptionLevel of the document in the WebBrowser control.
    /// Returned by the <see cref='System.Windows.Forms.WebBrowser.EncryptionLevel'/> property.
    ///     </para>
    /// </devdoc>
    public enum WebBrowserEncryptionLevel {
        /// <include file='doc\WebBrowser.uex' path='docs/doc[@for="WebBrowserEncryptionLevel.Insecure"]/*' />
        Insecure = 0, 
        /// <include file='doc\WebBrowser.uex' path='docs/doc[@for="WebBrowserEncryptionLevel.Mixed"]/*' />
        Mixed = 1,
        /// <include file='doc\WebBrowser.uex' path='docs/doc[@for="WebBrowserEncryptionLevel.Unknown"]/*' />
        Unknown = 2,
        /// <include file='doc\WebBrowser.uex' path='docs/doc[@for="WebBrowserEncryptionLevel.Bit40"]/*' />
        Bit40 = 3,
        /// <include file='doc\WebBrowser.uex' path='docs/doc[@for="WebBrowserEncryptionLevel.Bit56"]/*' />
        Bit56 = 4,
        /// <include file='doc\WebBrowser.uex' path='docs/doc[@for="WebBrowserEncryptionLevel.Fortezza"]/*' />
        Fortezza = 5,
        /// <include file='doc\WebBrowser.uex' path='docs/doc[@for="WebBrowserEncryptionLevel.Bit128"]/*' />
        Bit128 = 6
    }

    /// <include file='doc\WebBrowser.uex' path='docs/doc[@for="WebBrowserReadyState"]/*' />
    /// <devdoc>
    ///     <para>
    /// Specifies the ReadyState of the WebBrowser control.
    /// Returned by the <see cref='System.Windows.Forms.WebBrowser.ReadyState'/> property.
    ///     </para>
    /// </devdoc>
    public enum WebBrowserReadyState {
        /// <include file='doc\WebBrowser.uex' path='docs/doc[@for="WebBrowserReadyState.Uninitialized"]/*' />
        Uninitialized = 0,
        /// <include file='doc\WebBrowser.uex' path='docs/doc[@for="WebBrowserReadyState.Loading"]/*' />
        Loading = 1,
        /// <include file='doc\WebBrowser.uex' path='docs/doc[@for="WebBrowserReadyState.Loaded"]/*' />
        Loaded = 2,
        /// <include file='doc\WebBrowser.uex' path='docs/doc[@for="WebBrowserReadyState.Interactive"]/*' />
        Interactive = 3,
        /// <include file='doc\WebBrowser.uex' path='docs/doc[@for="WebBrowserReadyState.Complete"]/*' />
        Complete = 4
    }

    /// <include file='doc\WebBrowser.uex' path='docs/doc[@for="WebBrowserRefreshOption"]/*' />
    /// <devdoc>
    ///     <para>
    /// Specifies the RefreshOptions in the <see cref='System.Windows.Forms.WebBrowser.Refresh'/> method.
    ///     </para>
    /// </devdoc>
    public enum WebBrowserRefreshOption {
        /// <include file='doc\WebBrowser.uex' path='docs/doc[@for="WebBrowserRefreshOption.Normal"]/*' />
        Normal = 0,
        /// <include file='doc\WebBrowser.uex' path='docs/doc[@for="WebBrowserRefreshOption.IfExpired"]/*' />
        IfExpired = 1,
        /// <include file='doc\WebBrowser.uex' path='docs/doc[@for="WebBrowserRefreshOption.Continue"]/*' />
        Continue = 2,
        /// <include file='doc\WebBrowser.uex' path='docs/doc[@for="WebBrowserRefreshOption.Completely"]/*' />
        Completely = 3
    }
}

