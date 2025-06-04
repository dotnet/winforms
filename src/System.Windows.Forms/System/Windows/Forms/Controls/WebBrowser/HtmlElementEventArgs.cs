// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using Windows.Win32.Web.MsHtml;

namespace System.Windows.Forms;

public sealed class HtmlElementEventArgs : EventArgs
{
    private readonly HtmlShimManager _shimManager;

    internal HtmlElementEventArgs(HtmlShimManager shimManager, Interop.Mshtml.IHTMLEventObj eventObj)
    {
        NativeHTMLEventObj = eventObj;
        Debug.Assert(NativeHTMLEventObj is not null, "The event object should implement IHTMLEventObj");

        _shimManager = shimManager;
    }

    private Interop.Mshtml.IHTMLEventObj NativeHTMLEventObj { get; }

    public MouseButtons MouseButtonsPressed
    {
        get
        {
            MouseButtons buttons = MouseButtons.None;
            int nButtons = NativeHTMLEventObj.GetButton();
            if ((nButtons & 1) != 0)
            {
                buttons |= MouseButtons.Left;
            }

            if ((nButtons & 2) != 0)
            {
                buttons |= MouseButtons.Right;
            }

            if ((nButtons & 4) != 0)
            {
                buttons |= MouseButtons.Middle;
            }

            return buttons;
        }
    }

    public Point ClientMousePosition
    {
        get => new(NativeHTMLEventObj.GetClientX(), NativeHTMLEventObj.GetClientY());
    }

    public Point OffsetMousePosition
    {
        get => new(NativeHTMLEventObj.GetOffsetX(), NativeHTMLEventObj.GetOffsetY());
    }

    public Point MousePosition
    {
        get => new(NativeHTMLEventObj.GetX(), NativeHTMLEventObj.GetY());
    }

    public bool BubbleEvent
    {
        get => !NativeHTMLEventObj.GetCancelBubble();
        set => NativeHTMLEventObj.SetCancelBubble(!value);
    }

    public int KeyPressedCode => NativeHTMLEventObj.GetKeyCode();

    /// <summary>
    ///  Indicates whether the Alt key was pressed, if this information is
    ///  provided to the IHtmlEventObj
    /// </summary>
    public bool AltKeyPressed => NativeHTMLEventObj.GetAltKey();

    /// <summary>
    ///  Indicates whether the Ctrl key was pressed, if this information is
    ///  provided to the IHtmlEventObj
    /// </summary>
    public bool CtrlKeyPressed => NativeHTMLEventObj.GetCtrlKey();

    /// <summary>
    ///  Indicates whether the Shift key was pressed, if this information is
    ///  provided to the IHtmlEventObj
    /// </summary>
    public bool ShiftKeyPressed => NativeHTMLEventObj.GetShiftKey();

    public string EventType => NativeHTMLEventObj.GetEventType();

    public bool ReturnValue
    {
        get
        {
            object obj = NativeHTMLEventObj.GetReturnValue();
            return obj is null || (bool)obj;
        }
        set => NativeHTMLEventObj.SetReturnValue(value);
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public unsafe HtmlElement? FromElement
    {
        get
        {
            IHTMLElement.Interface htmlElement = NativeHTMLEventObj.GetFromElement();
            return htmlElement is null ? null : new HtmlElement(_shimManager, ComHelpers.GetComPointer<IHTMLElement>(htmlElement));
        }
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public unsafe HtmlElement? ToElement
    {
        get
        {
            IHTMLElement.Interface htmlElement = NativeHTMLEventObj.GetToElement();
            return htmlElement is null ? null : new HtmlElement(_shimManager, ComHelpers.GetComPointer<IHTMLElement>(htmlElement));
        }
    }
}
