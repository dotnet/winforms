// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;

namespace System.Windows.Forms.Design;

public partial class ComponentEditorForm
{
    // This should be moved into a shared location
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

        private HBRUSH _hbrushDither;

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

                cp.ExStyle |= (int)WINDOW_EX_STYLE.WS_EX_STATICEDGE;
                return cp;
            }
        }

        private unsafe void CreateDitherBrush()
        {
            Debug.Assert(_hbrushDither.IsNull, "Brush should not be recreated.");

            short* patternBits = stackalloc short[]
            {
                unchecked((short)0xAAAA),
                unchecked(0x5555),
                unchecked((short)0xAAAA),
                unchecked(0x5555),
                unchecked((short)0xAAAA),
                unchecked(0x5555),
                unchecked((short)0xAAAA),
                unchecked(0x5555)
            };

            HBITMAP hbitmapTemp = PInvokeCore.CreateBitmap(8, 8, 1, 1, patternBits);
            Debug.Assert(
                !hbitmapTemp.IsNull,
                "could not create dither bitmap. Page selector UI will not be correct");

            if (!hbitmapTemp.IsNull)
            {
                _hbrushDither = PInvoke.CreatePatternBrush(hbitmapTemp);

                Debug.Assert(
                    !_hbrushDither.IsNull,
                    "Unable to created dithered brush. Page selector UI will not be correct");

                PInvokeCore.DeleteObject(hbitmapTemp);
            }
        }

        private unsafe void DrawTreeItem(
            string itemText,
            int imageIndex,
            HDC dc,
            RECT rcIn,
            int state,
            COLORREF backColor,
            COLORREF textColor)
        {
            Size size = default;
            RECT rc2 = default;
            RECT rc = rcIn;
            ImageList? imageList = ImageList;

            // Select the font of the dialog, so we don't get the underlined font
            // when the item is being tracked
            using SelectObjectScope fontSelection = new(
                dc,
                (state & STATE_HOT) != 0 ? (HGDIOBJ)Parent!.FontHandle : default);

            GC.KeepAlive(Parent);

            // Fill the background
            if (((state & STATE_SELECTED) != 0) && !_hbrushDither.IsNull)
            {
                FillRectDither(dc, rcIn);
                PInvoke.SetBkMode(dc, BACKGROUND_MODE.TRANSPARENT);
            }
            else
            {
                PInvoke.SetBkColor(dc, backColor);
                PInvoke.ExtTextOut(dc, 0, 0, ETO_OPTIONS.ETO_CLIPPED | ETO_OPTIONS.ETO_OPAQUE, &rc, lpString: null, 0, lpDx: null);
            }

            fixed (char* pItemText = itemText)
            {
                // Get the height of the font
                PInvoke.GetTextExtentPoint32W(dc, pItemText, itemText.Length, (SIZE*)(void*)&size);
            }

            // Draw the caption
            rc2.left = rc.left + SIZE_ICON_X + 2 * PADDING_HORZ;
            rc2.top = rc.top + (((rc.bottom - rc.top) - size.Height) >> 1);
            rc2.bottom = rc2.top + size.Height;
            rc2.right = rc.right;
            PInvoke.SetTextColor(dc, textColor);

            fixed (char* t = itemText)
            {
                PInvoke.DrawText(
                    dc,
                    t,
                    itemText.Length,
                    ref rc2,
                    DRAW_TEXT_FORMAT.DT_LEFT | DRAW_TEXT_FORMAT.DT_VCENTER | DRAW_TEXT_FORMAT.DT_END_ELLIPSIS | DRAW_TEXT_FORMAT.DT_NOPREFIX);
            }

            if (imageList is not null)
            {
                PInvoke.ImageList.Draw(
                    imageList,
                    imageIndex,
                    dc,
                    PADDING_HORZ,
                    rc.top + (((rc.bottom - rc.top) - SIZE_ICON_Y) >> 1),
                    IMAGE_LIST_DRAW_STYLE.ILD_TRANSPARENT);
            }

            // Draw the hot-tracking border if needed
            if ((state & STATE_HOT) != 0)
            {
                COLORREF savedColor;

                // top left
                savedColor = PInvoke.SetBkColor(dc, (COLORREF)(uint)ColorTranslator.ToWin32(SystemColors.ControlLightLight));
                rc2.left = rc.left;
                rc2.top = rc.top;
                rc2.bottom = rc.top + 1;
                rc2.right = rc.right;
                PInvoke.ExtTextOut(dc, 0, 0, ETO_OPTIONS.ETO_OPAQUE, &rc2, lpString: null, 0, lpDx: null);
                rc2.bottom = rc.bottom;
                rc2.right = rc.left + 1;
                PInvoke.ExtTextOut(dc, 0, 0, ETO_OPTIONS.ETO_OPAQUE, &rc2, lpString: null, 0, lpDx: null);

                // bottom right
                PInvoke.SetBkColor(dc, (COLORREF)(uint)ColorTranslator.ToWin32(SystemColors.ControlDark));
                rc2.left = rc.left;
                rc2.right = rc.right;
                rc2.top = rc.bottom - 1;
                rc2.bottom = rc.bottom;
                PInvoke.ExtTextOut(dc, 0, 0, ETO_OPTIONS.ETO_OPAQUE, &rc2, lpString: null, 0, lpDx: null);
                rc2.left = rc.right - 1;
                rc2.top = rc.top;
                PInvoke.ExtTextOut(dc, 0, 0, ETO_OPTIONS.ETO_OPAQUE, &rc2, lpString: null, 0, lpDx: null);

                PInvoke.SetBkColor(dc, savedColor);
            }
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);

            int itemHeight = (int)PInvoke.SendMessage(this, PInvoke.TVM_GETITEMHEIGHT);
            itemHeight += 2 * PADDING_VERT;
            PInvoke.SendMessage(this, PInvoke.TVM_SETITEMHEIGHT, (WPARAM)itemHeight);

            if (_hbrushDither.IsNull)
            {
                CreateDitherBrush();
            }
        }

        private unsafe void OnCustomDraw(ref Message m)
        {
            NMTVCUSTOMDRAW* nmtvcd = (NMTVCUSTOMDRAW*)(nint)m.LParamInternal;

            switch (nmtvcd->nmcd.dwDrawStage)
            {
                case NMCUSTOMDRAW_DRAW_STAGE.CDDS_PREPAINT:
                    m.ResultInternal = (LRESULT)(nint)(PInvoke.CDRF_NOTIFYITEMDRAW | PInvoke.CDRF_NOTIFYPOSTPAINT);
                    break;
                case NMCUSTOMDRAW_DRAW_STAGE.CDDS_ITEMPREPAINT:
                    {
                        TreeNode? itemNode = TreeNode.FromHandle(this, (nint)nmtvcd->nmcd.dwItemSpec);
                        if (itemNode is not null)
                        {
                            int state = STATE_NORMAL;
                            NMCUSTOMDRAW_DRAW_STATE_FLAGS itemState = nmtvcd->nmcd.uItemState;
                            if (((itemState & NMCUSTOMDRAW_DRAW_STATE_FLAGS.CDIS_HOT) != 0) ||
                                ((itemState & NMCUSTOMDRAW_DRAW_STATE_FLAGS.CDIS_FOCUS) != 0))
                            {
                                state |= STATE_HOT;
                            }

                            if ((itemState & NMCUSTOMDRAW_DRAW_STATE_FLAGS.CDIS_SELECTED) != 0)
                            {
                                state |= STATE_SELECTED;
                            }

                            DrawTreeItem(
                                itemNode.Text,
                                itemNode.ImageIndex,
                                nmtvcd->nmcd.hdc,
                                nmtvcd->nmcd.rc,
                                state,
                                (COLORREF)(uint)ColorTranslator.ToWin32(SystemColors.Control),
                                (COLORREF)(uint)ColorTranslator.ToWin32(SystemColors.ControlText));
                        }

                        m.ResultInternal = (LRESULT)(nint)PInvoke.CDRF_SKIPDEFAULT;
                    }

                    break;
                case NMCUSTOMDRAW_DRAW_STAGE.CDDS_POSTPAINT:
                    m.ResultInternal = (LRESULT)(nint)PInvoke.CDRF_SKIPDEFAULT;
                    break;
                default:
                    m.ResultInternal = (LRESULT)(nint)PInvoke.CDRF_DODEFAULT;
                    break;
            }
        }

        protected override void OnHandleDestroyed(EventArgs e)
        {
            base.OnHandleDestroyed(e);

            if (!RecreatingHandle && !_hbrushDither.IsNull)
            {
                PInvokeCore.DeleteObject(_hbrushDither);
                _hbrushDither = default;
            }
        }

        private void FillRectDither(HDC dc, RECT rc)
        {
            HGDIOBJ hbrushOld = PInvoke.SelectObject(dc, _hbrushDither);

            if (!hbrushOld.IsNull)
            {
                COLORREF oldTextColor = PInvoke.SetTextColor(dc, (COLORREF)(uint)ColorTranslator.ToWin32(SystemColors.ControlLightLight));
                COLORREF oldBackColor = PInvoke.SetBkColor(dc, (COLORREF)(uint)ColorTranslator.ToWin32(SystemColors.Control));

                PInvoke.PatBlt(dc, rc.left, rc.top, rc.Width, rc.Height, ROP_CODE.PATCOPY);
                PInvoke.SetTextColor(dc, oldTextColor);
                PInvoke.SetBkColor(dc, oldBackColor);
            }
        }

        protected override unsafe void WndProc(ref Message m)
        {
            if (m.MsgInternal == MessageId.WM_REFLECT_NOTIFY)
            {
                NMHDR* nmhdr = (NMHDR*)(nint)m.LParamInternal;
                if (nmhdr->code == PInvoke.NM_CUSTOMDRAW)
                {
                    OnCustomDraw(ref m);
                    return;
                }
            }

            base.WndProc(ref m);
        }
    }
}
