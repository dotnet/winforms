// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections.Specialized;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;

namespace System.Windows.Forms;

/// <summary>
///  This class contains static properties/methods that are internal.
///  It also has types that make sense only for ActiveX hosting classes.
///  In other words, this is a helper class for the ActiveX hosting classes.
/// </summary>
internal static partial class WebBrowserHelper
{
    //
    // Static members:
    //

    // BitVector32 masks for various internal state flags.
    internal static readonly int s_sinkAttached = BitVector32.CreateMask();
    internal static readonly int s_manualUpdate = BitVector32.CreateMask(s_sinkAttached);
    internal static readonly int s_setClientSiteFirst = BitVector32.CreateMask(s_manualUpdate);
    internal static readonly int s_addedSelectionHandler = BitVector32.CreateMask(s_setClientSiteFirst);
    internal static readonly int s_siteProcessedInputKey = BitVector32.CreateMask(s_addedSelectionHandler);
    internal static readonly int s_inTransition = BitVector32.CreateMask(s_siteProcessedInputKey);
    internal static readonly int s_processingKeyUp = BitVector32.CreateMask(s_inTransition);
    internal static readonly int s_isMaskEdit = BitVector32.CreateMask(s_processingKeyUp);
    internal static readonly int s_recomputeContainingControl = BitVector32.CreateMask(s_isMaskEdit);

    // Gets the LOGPIXELSX of the screen DC.
    private static int s_logPixelsX = -1;
    private static int s_logPixelsY = -1;
    private const int HMperInch = 2540;

    // Special guids
    internal static Guid s_windowsMediaPlayer_Clsid = new("{22d6f312-b0f6-11d0-94ab-0080c74c7e95}");
    internal static Guid s_comctlImageCombo_Clsid = new("{a98a24c0-b06f-3684-8c12-c52ae341e0bc}");
    internal static Guid s_maskEdit_Clsid = new("{c932ba85-4374-101b-a56c-00aa003668dc}");

    // Window message to check if we have already sub-classed
    internal static uint REGMSG_MSG { get; } = PInvoke.RegisterWindowMessage($"{Application.WindowMessagesVersion}_subclassCheck");
    internal const int REGMSG_RETVAL = 123;

    //
    // Static helper methods:
    //

    internal static int Pix2HM(int pix, int logP)
    {
        return (HMperInch * pix + (logP >> 1)) / logP;
    }

    internal static int HM2Pix(int hm, int logP)
    {
        return (logP * hm + HMperInch / 2) / HMperInch;
    }

    // We cache LOGPIXELSX for optimization
    internal static int LogPixelsX
    {
        get
        {
            if (s_logPixelsX == -1)
            {
                using var dc = GetDcScope.ScreenDC;
                s_logPixelsX = PInvokeCore.GetDeviceCaps(dc, GET_DEVICE_CAPS_INDEX.LOGPIXELSX);
            }

            return s_logPixelsX;
        }
    }

    // We cache LOGPIXELSY for optimization
    internal static int LogPixelsY
    {
        get
        {
            if (s_logPixelsY == -1)
            {
                using var dc = GetDcScope.ScreenDC;
                s_logPixelsY = PInvokeCore.GetDeviceCaps(dc, GET_DEVICE_CAPS_INDEX.LOGPIXELSY);
            }

            return s_logPixelsY;
        }
    }

    // Gets the selection service from the control's site
    internal static ISelectionService? GetSelectionService(Control ctl)
    {
        ISite? site = ctl.Site;
        if (site is not null)
        {
            ISelectionService? selectionService = site.GetService<ISelectionService>();
            Debug.Assert(selectionService is not null, "service must implement ISelectionService");

            return selectionService;
        }

        return null;
    }

    /// <remarks>
    ///  <para>Returns a big clip RECT.</para>
    /// </remarks>
    internal static RECT GetClipRect() => new Rectangle(0, 0, 32000, 32000);
}
