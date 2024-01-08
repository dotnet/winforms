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
    internal static readonly int sinkAttached = BitVector32.CreateMask();
    internal static readonly int manualUpdate = BitVector32.CreateMask(sinkAttached);
    internal static readonly int setClientSiteFirst = BitVector32.CreateMask(manualUpdate);
    internal static readonly int addedSelectionHandler = BitVector32.CreateMask(setClientSiteFirst);
    internal static readonly int siteProcessedInputKey = BitVector32.CreateMask(addedSelectionHandler);
    internal static readonly int inTransition = BitVector32.CreateMask(siteProcessedInputKey);
    internal static readonly int processingKeyUp = BitVector32.CreateMask(inTransition);
    internal static readonly int isMaskEdit = BitVector32.CreateMask(processingKeyUp);
    internal static readonly int recomputeContainingControl = BitVector32.CreateMask(isMaskEdit);

    // Gets the LOGPIXELSX of the screen DC.
    private static int logPixelsX = -1;
    private static int logPixelsY = -1;
    private const int HMperInch = 2540;

    // Special guids
    internal static Guid windowsMediaPlayer_Clsid = new("{22d6f312-b0f6-11d0-94ab-0080c74c7e95}");
    internal static Guid comctlImageCombo_Clsid = new("{a98a24c0-b06f-3684-8c12-c52ae341e0bc}");
    internal static Guid maskEdit_Clsid = new("{c932ba85-4374-101b-a56c-00aa003668dc}");

    // Window message to check if we have already sub-classed
    internal static readonly MessageId REGMSG_MSG = PInvoke.RegisterWindowMessage($"{Application.WindowMessagesVersion}_subclassCheck");
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
            if (logPixelsX == -1)
            {
                using var dc = GetDcScope.ScreenDC;
                logPixelsX = PInvokeCore.GetDeviceCaps(dc, GET_DEVICE_CAPS_INDEX.LOGPIXELSX);
            }

            return logPixelsX;
        }
    }

    // We cache LOGPIXELSY for optimization
    internal static int LogPixelsY
    {
        get
        {
            if (logPixelsY == -1)
            {
                using var dc = GetDcScope.ScreenDC;
                logPixelsY = PInvokeCore.GetDeviceCaps(dc, GET_DEVICE_CAPS_INDEX.LOGPIXELSY);
            }

            return logPixelsY;
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
    ///  Returns a big clip RECT.
    /// </remarks>
    internal static RECT GetClipRect()
    {
        return new Rectangle(0, 0, 32000, 32000);
    }
}
