// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.Collections.Specialized;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Drawing;
using static Interop;

namespace System.Windows.Forms
{
    /// <summary>
    ///  This class contains static properties/methods that are internal.
    ///  It also has types that make sense only for ActiveX hosting classes.
    ///  In other words, this is a helper class for the ActiveX hosting classes.
    /// </summary>
    internal static class WebBrowserHelper
    {
        // Enumeration of the different states of the ActiveX control
        internal enum AXState
        {
            Passive = 0,        // Not loaded
            Loaded = 1,         // Loaded, but no server   [ocx created]
            Running = 2,        // Server running, invisible [depersisted]
            InPlaceActive = 4,  // Server in-place active [visible]
            UIActive = 8        // Used only by WebBrowserSiteBase
        }

        // Enumeration of the different Edit modes
        internal enum AXEditMode
        {
            None = 0,       // object not being edited
            Object = 1,     // object provided an edit verb and we invoked it
            Host = 2        // we invoked our own edit verb
        };

        // Enumeration of Selection Styles
        internal enum SelectionStyle
        {
            NotSelected = 0,
            Selected = 1,
            Active = 2
        };

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
        internal static Guid windowsMediaPlayer_Clsid = new Guid("{22d6f312-b0f6-11d0-94ab-0080c74c7e95}");
        internal static Guid comctlImageCombo_Clsid = new Guid("{a98a24c0-b06f-3684-8c12-c52ae341e0bc}");
        internal static Guid maskEdit_Clsid = new Guid("{c932ba85-4374-101b-a56c-00aa003668dc}");

        // Window message to check if we have already sub-classed
        internal static readonly User32.WM REGMSG_MSG = User32.RegisterWindowMessageW(Application.WindowMessagesVersion + "_subclassCheck");
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
                    using var dc = User32.GetDcScope.ScreenDC;
                    logPixelsX = Gdi32.GetDeviceCaps(dc, Gdi32.DeviceCapability.LOGPIXELSX);
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
                    using var dc = User32.GetDcScope.ScreenDC;
                    logPixelsY = Gdi32.GetDeviceCaps(dc, Gdi32.DeviceCapability.LOGPIXELSY);
                }
                return logPixelsY;
            }
        }

        // Gets the selection service from the control's site
        internal static ISelectionService GetSelectionService(Control ctl)
        {
            ISite site = ctl.Site;
            if (site != null)
            {
                object o = site.GetService(typeof(ISelectionService));
                Debug.Assert(o is null || o is ISelectionService, "service must implement ISelectionService");
                if (o is ISelectionService)
                {
                    return (ISelectionService)o;
                }
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
}
