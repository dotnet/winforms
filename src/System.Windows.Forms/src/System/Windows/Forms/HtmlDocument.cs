// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Runtime.InteropServices;
using static Interop;
using static Interop.Mshtml;

namespace System.Windows.Forms
{
    public sealed partial class HtmlDocument
    {
        internal static object s_eventClick = new();
        internal static object s_eventContextMenuShowing = new();
        internal static object s_eventFocusing = new();
        internal static object s_eventLosingFocus = new();
        internal static object s_eventMouseDown = new();
        internal static object s_eventMouseLeave = new();
        internal static object s_eventMouseMove = new();
        internal static object s_eventMouseOver = new();
        internal static object s_eventMouseUp = new();
        internal static object s_eventStop = new();

        private readonly IHTMLDocument2 _htmlDocument2;
        private readonly HtmlShimManager _shimManager;

        internal HtmlDocument(HtmlShimManager shimManager, IHTMLDocument doc)
        {
            _htmlDocument2 = (IHTMLDocument2)doc;
            Debug.Assert(NativeHtmlDocument2 is not null, "The document should implement IHtmlDocument2");

            _shimManager = shimManager;
        }

        internal IHTMLDocument2 NativeHtmlDocument2
        {
            get
            {
                return _htmlDocument2;
            }
        }

        private HtmlDocumentShim DocumentShim
        {
            get
            {
                if (ShimManager is not null)
                {
                    HtmlDocumentShim shim = ShimManager.GetDocumentShim(this);
                    if (shim is null)
                    {
                        _shimManager.AddDocumentShim(this);
                        shim = ShimManager.GetDocumentShim(this);
                    }

                    return shim;
                }

                return null;
            }
        }

        private HtmlShimManager ShimManager
        {
            get
            {
                return _shimManager;
            }
        }

        public HtmlElement ActiveElement
        {
            get
            {
                IHTMLElement iHtmlElement = NativeHtmlDocument2.GetActiveElement();
                return iHtmlElement is not null ? new HtmlElement(ShimManager, iHtmlElement) : null;
            }
        }

        public HtmlElement Body
        {
            get
            {
                IHTMLElement iHtmlElement = NativeHtmlDocument2.GetBody();
                return iHtmlElement is not null ? new HtmlElement(ShimManager, iHtmlElement) : null;
            }
        }

        public string Domain
        {
            get
            {
                return NativeHtmlDocument2.GetDomain();
            }
            set
            {
                try
                {
                    NativeHtmlDocument2.SetDomain(value);
                }
                catch (ArgumentException)
                {
                    // Give a better message describing the error
                    throw new ArgumentException(SR.HtmlDocumentInvalidDomain);
                }
            }
        }

        public string Title
        {
            get
            {
                return NativeHtmlDocument2.GetTitle();
            }
            set
            {
                NativeHtmlDocument2.SetTitle(value);
            }
        }

        public Uri Url
        {
            get
            {
                IHTMLLocation iHtmlLocation = NativeHtmlDocument2.GetLocation();
                string stringLocation = (iHtmlLocation is null) ? "" : iHtmlLocation.GetHref();
                return string.IsNullOrEmpty(stringLocation) ? null : new Uri(stringLocation);
            }
        }

        public HtmlWindow Window
        {
            get
            {
                IHTMLWindow2 iHTMLWindow2 = NativeHtmlDocument2.GetParentWindow();
                return iHTMLWindow2 is not null ? new HtmlWindow(ShimManager, iHTMLWindow2) : null;
            }
        }

        public Color BackColor
        {
            get
            {
                Color c = Color.Empty;
                try
                {
                    c = ColorFromObject(NativeHtmlDocument2.GetBgColor());
                }
                catch (Exception ex)
                {
                    if (ClientUtils.IsCriticalException(ex))
                    {
                        throw;
                    }
                }

                return c;
            }
            set
            {
                int color = value.R << 16 | value.G << 8 | value.B;
                NativeHtmlDocument2.SetBgColor(color);
            }
        }

        public Color ForeColor
        {
            get
            {
                Color c = Color.Empty;
                try
                {
                    c = ColorFromObject(NativeHtmlDocument2.GetFgColor());
                }
                catch (Exception ex)
                {
                    if (ClientUtils.IsCriticalException(ex))
                    {
                        throw;
                    }
                }

                return c;
            }
            set
            {
                int color = value.R << 16 | value.G << 8 | value.B;
                NativeHtmlDocument2.SetFgColor(color);
            }
        }

        public Color LinkColor
        {
            get
            {
                Color c = Color.Empty;
                try
                {
                    c = ColorFromObject(NativeHtmlDocument2.GetLinkColor());
                }
                catch (Exception ex)
                {
                    if (ClientUtils.IsCriticalException(ex))
                    {
                        throw;
                    }
                }

                return c;
            }
            set
            {
                int color = value.R << 16 | value.G << 8 | value.B;
                NativeHtmlDocument2.SetLinkColor(color);
            }
        }

        public Color ActiveLinkColor
        {
            get
            {
                Color c = Color.Empty;
                try
                {
                    c = ColorFromObject(NativeHtmlDocument2.GetAlinkColor());
                }
                catch (Exception ex)
                {
                    if (ClientUtils.IsCriticalException(ex))
                    {
                        throw;
                    }
                }

                return c;
            }
            set
            {
                int color = value.R << 16 | value.G << 8 | value.B;
                NativeHtmlDocument2.SetAlinkColor(color);
            }
        }

        public Color VisitedLinkColor
        {
            get
            {
                Color c = Color.Empty;
                try
                {
                    c = ColorFromObject(NativeHtmlDocument2.GetVlinkColor());
                }
                catch (Exception ex)
                {
                    if (ClientUtils.IsCriticalException(ex))
                    {
                        throw;
                    }
                }

                return c;
            }
            set
            {
                int color = value.R << 16 | value.G << 8 | value.B;
                NativeHtmlDocument2.SetVlinkColor(color);
            }
        }

        public bool Focused
        {
            get
            {
                return ((IHTMLDocument4)NativeHtmlDocument2).HasFocus();
            }
        }

        public object DomDocument
        {
            get
            {
                return NativeHtmlDocument2;
            }
        }

        public string Cookie
        {
            get
            {
                return NativeHtmlDocument2.GetCookie();
            }
            set
            {
                NativeHtmlDocument2.SetCookie(value);
            }
        }

        public bool RightToLeft
        {
            get
            {
                return ((IHTMLDocument3)NativeHtmlDocument2).GetDir() == "rtl";
            }
            set
            {
                ((IHTMLDocument3)NativeHtmlDocument2).SetDir(value ? "rtl" : "ltr");
            }
        }

        public string Encoding
        {
            get
            {
                return NativeHtmlDocument2.GetCharset();
            }
            set
            {
                NativeHtmlDocument2.SetCharset(value);
            }
        }

        public string DefaultEncoding
        {
            get
            {
                return NativeHtmlDocument2.GetDefaultCharset();
            }
        }

        public HtmlElementCollection All
        {
            get
            {
                IHTMLElementCollection iHTMLElementCollection = NativeHtmlDocument2.GetAll();
                return iHTMLElementCollection is not null ? new HtmlElementCollection(ShimManager, iHTMLElementCollection) : new HtmlElementCollection(ShimManager);
            }
        }

        public HtmlElementCollection Links
        {
            get
            {
                IHTMLElementCollection iHTMLElementCollection = NativeHtmlDocument2.GetLinks();
                return iHTMLElementCollection is not null ? new HtmlElementCollection(ShimManager, iHTMLElementCollection) : new HtmlElementCollection(ShimManager);
            }
        }

        public HtmlElementCollection Images
        {
            get
            {
                IHTMLElementCollection iHTMLElementCollection = NativeHtmlDocument2.GetImages();
                return iHTMLElementCollection is not null ? new HtmlElementCollection(ShimManager, iHTMLElementCollection) : new HtmlElementCollection(ShimManager);
            }
        }

        public HtmlElementCollection Forms
        {
            get
            {
                IHTMLElementCollection iHTMLElementCollection = NativeHtmlDocument2.GetForms();
                return iHTMLElementCollection is not null ? new HtmlElementCollection(ShimManager, iHTMLElementCollection) : new HtmlElementCollection(ShimManager);
            }
        }

        public void Write(string text)
        {
            object[] strs = new object[] { (object)text };
            NativeHtmlDocument2.Write(strs);
        }

        /// <summary>
        ///  Executes a command on the document
        /// </summary>
        public void ExecCommand(string command, bool showUI, object value)
        {
            NativeHtmlDocument2.ExecCommand(command, showUI, value);
        }

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public void Focus()
        {
            ((IHTMLDocument4)NativeHtmlDocument2).Focus();
            // Seems to have a problem in really setting focus the first time
            ((IHTMLDocument4)NativeHtmlDocument2).Focus();
        }

        public HtmlElement GetElementById(string id)
        {
            IHTMLElement iHTMLElement = ((IHTMLDocument3)NativeHtmlDocument2).GetElementById(id);
            return iHTMLElement is not null ? new HtmlElement(ShimManager, iHTMLElement) : null;
        }

        public HtmlElement GetElementFromPoint(Point point)
        {
            IHTMLElement iHTMLElement = NativeHtmlDocument2.ElementFromPoint(point.X, point.Y);
            return iHTMLElement is not null ? new HtmlElement(ShimManager, iHTMLElement) : null;
        }

        public HtmlElementCollection GetElementsByTagName(string tagName)
        {
            IHTMLElementCollection iHTMLElementCollection = ((IHTMLDocument3)NativeHtmlDocument2).GetElementsByTagName(tagName);
            return iHTMLElementCollection is not null ? new HtmlElementCollection(ShimManager, iHTMLElementCollection) : new HtmlElementCollection(ShimManager);
        }

        public HtmlDocument OpenNew(bool replaceInHistory)
        {
            object name = (object)(replaceInHistory ? "replace" : "");
            object nullObject = null;
            object ohtmlDocument = NativeHtmlDocument2.Open("text/html", name, nullObject, nullObject);
            return ohtmlDocument is IHTMLDocument iHTMLDocument ? new HtmlDocument(ShimManager, iHTMLDocument) : null;
        }

        public HtmlElement CreateElement(string elementTag)
        {
            IHTMLElement iHTMLElement = NativeHtmlDocument2.CreateElement(elementTag);
            return iHTMLElement is not null ? new HtmlElement(ShimManager, iHTMLElement) : null;
        }

        public unsafe object InvokeScript(string scriptName, object[] args)
        {
            try
            {
                if (NativeHtmlDocument2.GetScript() is Oleaut32.IDispatch scriptObject)
                {
                    Guid g = Guid.Empty;
                    string[] names = new string[] { scriptName };
                    Ole32.DispatchID dispid = Ole32.DispatchID.UNKNOWN;
                    HRESULT hr = scriptObject.GetIDsOfNames(&g, names, 1, PInvoke.GetThreadLocale(), &dispid);
                    if (!hr.Succeeded || dispid == Ole32.DispatchID.UNKNOWN)
                    {
                        return null;
                    }

                    if (args is not null)
                    {
                        // Reverse the arg order so that they read naturally after IDispatch.
                        Array.Reverse(args);
                    }

                    using var vectorArgs = new Oleaut32.VARIANTVector(args);
                    fixed (Oleaut32.VARIANT* pVariants = vectorArgs.Variants)
                    {
                        var dispParams = new Oleaut32.DISPPARAMS();
                        dispParams.rgvarg = pVariants;
                        dispParams.cArgs = (uint)vectorArgs.Variants.Length;
                        dispParams.rgdispidNamedArgs = null;
                        dispParams.cNamedArgs = 0;

                        var retVals = new object[1];
                        var excepInfo = new Oleaut32.EXCEPINFO();
                        hr = scriptObject.Invoke(
                            dispid,
                            &g,
                            PInvoke.GetThreadLocale(),
                            Oleaut32.DISPATCH.METHOD,
                            &dispParams,
                            retVals,
                            &excepInfo,
                            null);
                        if (hr == HRESULT.Values.S_OK)
                        {
                            return retVals[0];
                        }
                    }
                }
            }
            catch (Exception ex) when (!ClientUtils.IsCriticalException(ex))
            {
            }

            return null;
        }

        public object InvokeScript(string scriptName)
        {
            return InvokeScript(scriptName, null);
        }

        public void AttachEventHandler(string eventName, EventHandler eventHandler)
        {
            HtmlDocumentShim shim = DocumentShim;
            if (shim is not null)
            {
                shim.AttachEventHandler(eventName, eventHandler);
            }
        }

        public void DetachEventHandler(string eventName, EventHandler eventHandler)
        {
            HtmlDocumentShim shim = DocumentShim;
            if (shim is not null)
            {
                shim.DetachEventHandler(eventName, eventHandler);
            }
        }

        public event HtmlElementEventHandler Click
        {
            add => DocumentShim.AddHandler(s_eventClick, value);
            remove => DocumentShim.RemoveHandler(s_eventClick, value);
        }

        public event HtmlElementEventHandler ContextMenuShowing
        {
            add => DocumentShim.AddHandler(s_eventContextMenuShowing, value);
            remove => DocumentShim.RemoveHandler(s_eventContextMenuShowing, value);
        }

        public event HtmlElementEventHandler Focusing
        {
            add => DocumentShim.AddHandler(s_eventFocusing, value);
            remove => DocumentShim.RemoveHandler(s_eventFocusing, value);
        }

        public event HtmlElementEventHandler LosingFocus
        {
            add => DocumentShim.AddHandler(s_eventLosingFocus, value);
            remove => DocumentShim.RemoveHandler(s_eventLosingFocus, value);
        }

        public event HtmlElementEventHandler MouseDown
        {
            add => DocumentShim.AddHandler(s_eventMouseDown, value);
            remove => DocumentShim.RemoveHandler(s_eventMouseDown, value);
        }

        /// <summary>
        ///  Occurs when the mouse leaves the document
        /// </summary>
        public event HtmlElementEventHandler MouseLeave
        {
            add => DocumentShim.AddHandler(s_eventMouseLeave, value);
            remove => DocumentShim.RemoveHandler(s_eventMouseLeave, value);
        }

        public event HtmlElementEventHandler MouseMove
        {
            add => DocumentShim.AddHandler(s_eventMouseMove, value);
            remove => DocumentShim.RemoveHandler(s_eventMouseMove, value);
        }

        public event HtmlElementEventHandler MouseOver
        {
            add => DocumentShim.AddHandler(s_eventMouseOver, value);
            remove => DocumentShim.RemoveHandler(s_eventMouseOver, value);
        }

        public event HtmlElementEventHandler MouseUp
        {
            add => DocumentShim.AddHandler(s_eventMouseUp, value);
            remove => DocumentShim.RemoveHandler(s_eventMouseUp, value);
        }

        public event HtmlElementEventHandler Stop
        {
            add => DocumentShim.AddHandler(s_eventStop, value);
            remove => DocumentShim.RemoveHandler(s_eventStop, value);
        }

        private static Color ColorFromObject(object oColor)
        {
            try
            {
                if (oColor is string)
                {
                    string strColor = oColor as string;
                    int index = strColor.IndexOf('#');
                    if (index >= 0)
                    {
                        // The string is of the form: #ff00a0. Skip past the #
                        string hexColor = strColor.Substring(index + 1);
                        // The actual color is non-transparent. So set alpha = 255.
                        return Color.FromArgb(255, Color.FromArgb(int.Parse(hexColor, NumberStyles.HexNumber, CultureInfo.InvariantCulture)));
                    }
                    else
                    {
                        return Color.FromName(strColor);
                    }
                }
                else if (oColor is int)
                {
                    // The actual color is non-transparent. So set alpha = 255.
                    return Color.FromArgb(255, Color.FromArgb((int)oColor));
                }
            }
            catch (Exception ex)
            {
                if (ClientUtils.IsCriticalException(ex))
                {
                    throw;
                }
            }

            return Color.Empty;
        }

        #region operators

        public static bool operator ==(HtmlDocument left, HtmlDocument right)
        {
            //Not equal if only one's null.
            if (left is null != right is null)
            {
                return false;
            }

            //Equal if both are null.
            if (left is null)
            {
                return true;
            }

            //Neither are null.  Get the IUnknowns and compare them.
            IntPtr leftPtr = IntPtr.Zero;
            IntPtr rightPtr = IntPtr.Zero;
            try
            {
                leftPtr = Marshal.GetIUnknownForObject(left.NativeHtmlDocument2);
                rightPtr = Marshal.GetIUnknownForObject(right.NativeHtmlDocument2);
                return leftPtr == rightPtr;
            }
            finally
            {
                if (leftPtr != IntPtr.Zero)
                {
                    Marshal.Release(leftPtr);
                }

                if (rightPtr != IntPtr.Zero)
                {
                    Marshal.Release(rightPtr);
                }
            }
        }

        public static bool operator !=(HtmlDocument left, HtmlDocument right)
        {
            return !(left == right);
        }

        public override int GetHashCode() => _htmlDocument2?.GetHashCode() ?? 0;

        public override bool Equals(object obj)
        {
            return (this == (HtmlDocument)obj);
        }
        #endregion

    }
}
