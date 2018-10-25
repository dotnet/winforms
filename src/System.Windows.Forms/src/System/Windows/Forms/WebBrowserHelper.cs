// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using System.Configuration.Assemblies;
using System.Runtime.Remoting;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.ComponentModel;
using System.Diagnostics;
using System;
using System.Reflection;
using System.Globalization;
using System.Security.Permissions;
using Microsoft.Win32;
using System.Collections;
using System.Collections.Specialized;
using System.IO;
using System.Drawing;    
using System.Windows.Forms.Design;
using System.Windows.Forms.ComponentModel;
using System.Windows.Forms.ComponentModel.Com2Interop;
using System.ComponentModel.Design;
using System.Windows.Forms;
using System.Drawing.Imaging;
using System.Drawing.Design;

namespace System.Windows.Forms {
    //
    // This class contains static properties/methods that are internal.
    // It also has types that make sense only for ActiveX hosting classes.
    // In other words, this is a helper class for the ActiveX hosting classes.
    //
    internal static class WebBrowserHelper {
        //
        // Types:
        //
        
        //
        // Enumeration of the different states of the ActiveX control
        internal enum AXState {
            Passive = 0,        // Not loaded
            Loaded = 1,         // Loaded, but no server   [ocx created]
            Running = 2,        // Server running, invisible [depersisted]
            InPlaceActive = 4,  // Server in-place active [visible]
            UIActive = 8        // Used only by WebBrowserSiteBase
        }
        //
        // Enumeration of the different Edit modes
        internal enum AXEditMode {
            None = 0,       // object not being edited
            Object = 1,     // object provided an edit verb and we invoked it
            Host = 2        // we invoked our own edit verb
        };
        //
        // Enumeration of Selection Styles
        internal enum SelectionStyle {
            NotSelected = 0,
            Selected = 1,
            Active = 2
        };

        
        //
        // Static members:
        //
        
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
        //
        // Gets the LOGPIXELSX of the screen DC.
        private static int logPixelsX = -1;
        private static int logPixelsY = -1;
        private const int HMperInch = 2540;
        //
        // Special guids
        private static Guid ifont_Guid = typeof(UnsafeNativeMethods.IFont).GUID;
        internal  static Guid windowsMediaPlayer_Clsid = new Guid("{22d6f312-b0f6-11d0-94ab-0080c74c7e95}");
        internal  static Guid comctlImageCombo_Clsid   = new Guid("{a98a24c0-b06f-3684-8c12-c52ae341e0bc}");
        internal  static Guid maskEdit_Clsid           = new Guid("{c932ba85-4374-101b-a56c-00aa003668dc}");
        //
        // Window message to check if we have already sub-classed
        internal static readonly int REGMSG_MSG = SafeNativeMethods.RegisterWindowMessage(Application.WindowMessagesVersion + "_subclassCheck");
        internal const int REGMSG_RETVAL = 123;


        //
        // Static helper methods:
        //

        internal static int Pix2HM(int pix, int logP) {
            return(HMperInch * pix + ( logP >> 1)) / logP;
        }

        internal static int HM2Pix(int hm, int logP) {
            return(logP * hm + HMperInch / 2) / HMperInch;
        }

        //
        // We cache LOGPIXELSX for optimization
        internal static int LogPixelsX {
            get {
                if (logPixelsX == -1) {
                    IntPtr hDC = UnsafeNativeMethods.GetDC(NativeMethods.NullHandleRef);
                    if (hDC != IntPtr.Zero) {
                        logPixelsX = UnsafeNativeMethods.GetDeviceCaps(new HandleRef(null, hDC), NativeMethods.LOGPIXELSX);
                        UnsafeNativeMethods.ReleaseDC(NativeMethods.NullHandleRef, new HandleRef(null, hDC));
                    }
                }
                return logPixelsX;
            }
        }
        internal static void ResetLogPixelsX() {
            logPixelsX = -1;
        }
            
        //
        // We cache LOGPIXELSY for optimization
        internal static int LogPixelsY {
            get {
                if (logPixelsY == -1) {
                    IntPtr hDC = UnsafeNativeMethods.GetDC(NativeMethods.NullHandleRef);
                    if (hDC != IntPtr.Zero) {
                        logPixelsY = UnsafeNativeMethods.GetDeviceCaps(new HandleRef(null, hDC), NativeMethods.LOGPIXELSY);
                        UnsafeNativeMethods.ReleaseDC(NativeMethods.NullHandleRef, new HandleRef(null, hDC));
                    }
                }
                return logPixelsY;
            }
        }
        internal static void ResetLogPixelsY() {
            logPixelsY = -1;
        }


        //
        // Gets the selection service from the control's site
        internal static ISelectionService GetSelectionService(Control ctl) {
            ISite site = ctl.Site;
            if (site != null) {
                Object o = site.GetService(typeof(ISelectionService));
                Debug.Assert(o == null || o is ISelectionService, "service must implement ISelectionService");
                if (o is ISelectionService) {
                    return(ISelectionService) o;
                }
            }
            return null;
        }

        //
        // Returns a big COMRECT
        internal static NativeMethods.COMRECT GetClipRect() {
            return new NativeMethods.COMRECT(new Rectangle(0, 0, 32000, 32000));
        }
    }
}

