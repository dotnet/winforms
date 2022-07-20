// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using System.Drawing;
using Windows.Win32;
using static Interop;

namespace System.Windows.Forms.Design
{
    public partial class ComponentEditorForm
    {
        //  This should be moved into a shared location
        //  Its a duplication of what exists in the StyleBuilder.
        internal sealed class PageSelector : TreeView
        {
            private const int PADDING_VERT = 3;
            private const int PADDING_HORZ = 4;

            private const int SIZE_ICON_X = 16;
            private const int SIZE_ICON_Y = 16;

            private const int STATE_NORMAL = 0;
            private const int STATE_SELECTED = 1;
            private const int STATE_HOT = 2;

            private Gdi32.HBRUSH _hbrushDither;

            public PageSelector()
            {
                HotTracking = true;
                HideSelection = false;
                BackColor = SystemColors.Control;
                Indent = 0;
                LabelEdit = false;
                Scrollable = false;
                ShowLines = false;
                ShowPlusMinus = false;
                ShowRootLines = false;
                BorderStyle = BorderStyle.None;
                Indent = 0;
                FullRowSelect = true;
            }

            protected override CreateParams CreateParams
            {
                get
                {
                    CreateParams cp = base.CreateParams;

                    cp.ExStyle |= (int)User32.WS_EX.STATICEDGE;
                    return cp;
                }
            }

            private unsafe void CreateDitherBrush()
            {
                Debug.Assert(_hbrushDither.IsNull, "Brush should not be recreated.");

                short* patternBits = stackalloc short[]
                {
                    unchecked((short)0xAAAA),
                    unchecked((short)0x5555),
                    unchecked((short)0xAAAA),
                    unchecked((short)0x5555),
                    unchecked((short)0xAAAA),
                    unchecked((short)0x5555),
                    unchecked((short)0xAAAA),
                    unchecked((short)0x5555)
                };

                Gdi32.HBITMAP hbitmapTemp = PInvoke.CreateBitmap(8, 8, 1, 1, patternBits);
                Debug.Assert(
                    !hbitmapTemp.IsNull,
                    "could not create dither bitmap. Page selector UI will not be correct");

                if (!hbitmapTemp.IsNull)
                {
                    _hbrushDither = Gdi32.CreatePatternBrush(hbitmapTemp);

                    Debug.Assert(
                        !_hbrushDither.IsNull,
                        "Unable to created dithered brush. Page selector UI will not be correct");

                    Gdi32.DeleteObject(hbitmapTemp);
                }
            }

            private unsafe void DrawTreeItem(
                string itemText,
                int imageIndex,
                Gdi32.HDC dc,
                RECT rcIn,
                int state,
                int backColor,
                int textColor)
            {
                Size size = new Size();
                var rc2 = new RECT();
                var rc = new RECT(rcIn.left, rcIn.top, rcIn.right, rcIn.bottom);
                ImageList imagelist = ImageList;

                // Select the font of the dialog, so we don't get the underlined font
                // when the item is being tracked
                using var fontSelection = new Gdi32.SelectObjectScope(
                    dc,
                    (state & STATE_HOT) != 0 ? (Gdi32.HGDIOBJ)Parent!.FontHandle : default);

                GC.KeepAlive(Parent);

                // Fill the background
                if (((state & STATE_SELECTED) != 0) && !_hbrushDither.IsNull)
                {
                    FillRectDither(dc, rcIn);
                    Gdi32.SetBkMode(dc, Gdi32.BKMODE.TRANSPARENT);
                }
                else
                {
                    Gdi32.SetBkColor(dc, backColor);
                    Gdi32.ExtTextOutW(dc, 0, 0, Gdi32.ETO.CLIPPED | Gdi32.ETO.OPAQUE, ref rc, null, 0, null);
                }

                // Get the height of the font
                Gdi32.GetTextExtentPoint32W(dc, itemText, itemText.Length, ref size);

                // Draw the caption
                rc2.left = rc.left + SIZE_ICON_X + 2 * PADDING_HORZ;
                rc2.top = rc.top + (((rc.bottom - rc.top) - size.Height) >> 1);
                rc2.bottom = rc2.top + size.Height;
                rc2.right = rc.right;
                Gdi32.SetTextColor(dc, textColor);
                User32.DrawTextW(
                    dc,
                    itemText,
                    itemText.Length,
                    ref rc2,
                    User32.DT.LEFT | User32.DT.VCENTER | User32.DT.END_ELLIPSIS | User32.DT.NOPREFIX);

                ComCtl32.ImageList.Draw(
                    imagelist,
                    imageIndex,
                    dc,
                    PADDING_HORZ,
                    rc.top + (((rc.bottom - rc.top) - SIZE_ICON_Y) >> 1),
                    ComCtl32.ILD.TRANSPARENT);

                // Draw the hot-tracking border if needed
                if ((state & STATE_HOT) != 0)
                {
                    int savedColor;

                    // top left
                    savedColor = Gdi32.SetBkColor(dc, ColorTranslator.ToWin32(SystemColors.ControlLightLight));
                    rc2.left = rc.left;
                    rc2.top = rc.top;
                    rc2.bottom = rc.top + 1;
                    rc2.right = rc.right;
                    Gdi32.ExtTextOutW(dc, 0, 0, Gdi32.ETO.OPAQUE, ref rc2, null, 0, null);
                    rc2.bottom = rc.bottom;
                    rc2.right = rc.left + 1;
                    Gdi32.ExtTextOutW(dc, 0, 0, Gdi32.ETO.OPAQUE, ref rc2, null, 0, null);

                    // bottom right
                    Gdi32.SetBkColor(dc, ColorTranslator.ToWin32(SystemColors.ControlDark));
                    rc2.left = rc.left;
                    rc2.right = rc.right;
                    rc2.top = rc.bottom - 1;
                    rc2.bottom = rc.bottom;
                    Gdi32.ExtTextOutW(dc, 0, 0, Gdi32.ETO.OPAQUE, ref rc2, null, 0, null);
                    rc2.left = rc.right - 1;
                    rc2.top = rc.top;
                    Gdi32.ExtTextOutW(dc, 0, 0, Gdi32.ETO.OPAQUE, ref rc2, null, 0, null);

                    Gdi32.SetBkColor(dc, savedColor);
                }
            }

            protected override void OnHandleCreated(EventArgs e)
            {
                base.OnHandleCreated(e);

                int itemHeight = (int)User32.SendMessageW(this, (User32.WM)ComCtl32.TVM.GETITEMHEIGHT);
                itemHeight += 2 * PADDING_VERT;
                User32.SendMessageW(this, (User32.WM)ComCtl32.TVM.SETITEMHEIGHT, itemHeight);

                if (_hbrushDither.IsNull)
                {
                    CreateDitherBrush();
                }
            }

            private unsafe void OnCustomDraw(ref Message m)
            {
                ComCtl32.NMTVCUSTOMDRAW* nmtvcd = (ComCtl32.NMTVCUSTOMDRAW*)m.LParamInternal;
                switch (nmtvcd->nmcd.dwDrawStage)
                {
                    case ComCtl32.CDDS.PREPAINT:
                        m.ResultInternal = (nint)(ComCtl32.CDRF.NOTIFYITEMDRAW | ComCtl32.CDRF.NOTIFYPOSTPAINT);
                        break;
                    case ComCtl32.CDDS.ITEMPREPAINT:
                        {
                            TreeNode itemNode = TreeNode.FromHandle(this, nmtvcd->nmcd.dwItemSpec);
                            if (itemNode is not null)
                            {
                                int state = STATE_NORMAL;
                                ComCtl32.CDIS itemState = nmtvcd->nmcd.uItemState;
                                if (((itemState & ComCtl32.CDIS.HOT) != 0) || ((itemState & ComCtl32.CDIS.FOCUS) != 0))
                                {
                                    state |= STATE_HOT;
                                }

                                if ((itemState & ComCtl32.CDIS.SELECTED) != 0)
                                {
                                    state |= STATE_SELECTED;
                                }

                                DrawTreeItem(
                                    itemNode.Text,
                                    itemNode.ImageIndex,
                                    nmtvcd->nmcd.hdc,
                                    nmtvcd->nmcd.rc,
                                    state,
                                    ColorTranslator.ToWin32(SystemColors.Control),
                                    ColorTranslator.ToWin32(SystemColors.ControlText));
                            }

                            m.ResultInternal = (nint)ComCtl32.CDRF.SKIPDEFAULT;
                        }

                        break;
                    case ComCtl32.CDDS.POSTPAINT:
                        m.ResultInternal = (nint)ComCtl32.CDRF.SKIPDEFAULT;
                        break;
                    default:
                        m.ResultInternal = (nint)ComCtl32.CDRF.DODEFAULT;
                        break;
                }
            }

            protected override void OnHandleDestroyed(EventArgs e)
            {
                base.OnHandleDestroyed(e);

                if (!RecreatingHandle && !_hbrushDither.IsNull)
                {
                    Gdi32.DeleteObject(_hbrushDither);
                    _hbrushDither = default;
                }
            }

            private void FillRectDither(Gdi32.HDC dc, RECT rc)
            {
                Gdi32.HGDIOBJ hbrushOld = Gdi32.SelectObject(dc, _hbrushDither);

                if (!hbrushOld.IsNull)
                {
                    int oldTextColor = Gdi32.SetTextColor(dc, ColorTranslator.ToWin32(SystemColors.ControlLightLight));
                    int oldBackColor = Gdi32.SetBkColor(dc, ColorTranslator.ToWin32(SystemColors.Control));

                    Gdi32.PatBlt(dc, rc.left, rc.top, rc.right - rc.left, rc.bottom - rc.top, Gdi32.ROP.PATCOPY);
                    Gdi32.SetTextColor(dc, oldTextColor);
                    Gdi32.SetBkColor(dc, oldBackColor);
                }
            }

            protected unsafe override void WndProc(ref Message m)
            {
                if (m.MsgInternal == User32.WM.REFLECT_NOTIFY)
                {
                    User32.NMHDR* nmhdr = (User32.NMHDR*)m.LParamInternal;
                    if (nmhdr->code == (int)ComCtl32.NM.CUSTOMDRAW)
                    {
                        OnCustomDraw(ref m);
                        return;
                    }
                }

                base.WndProc(ref m);
            }
        }
    }
}
