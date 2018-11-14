// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


namespace System.Windows.Forms {

    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System;
    using System.IO;
    using System.Security.Permissions;
    using System.Security;
    using System.Drawing.Printing;
    using System.Runtime.Versioning;

    internal static class IntSecurity {
        public static readonly TraceSwitch SecurityDemand = new TraceSwitch("SecurityDemand", "Trace when security demands occur.");
#if DEBUG
        public static readonly BooleanSwitch MapSafeTopLevelToSafeSub = new BooleanSwitch("MapSafeTopLevelToSafeSub", "Maps the SafeTopLevelWindow UI permission to SafeSubWindow permission. Must restart to take effect.");
#endif        

        private static CodeAccessPermission adjustCursorClip;
        private static CodeAccessPermission affectMachineState;
        private static CodeAccessPermission affectThreadBehavior;
        private static CodeAccessPermission allPrinting;
        private static PermissionSet        allPrintingAndUnmanagedCode; // Can't assert twice in the same method
        private static CodeAccessPermission allWindows;
        private static CodeAccessPermission clipboardRead;
        private static CodeAccessPermission clipboardOwn;
        private static PermissionSet        clipboardWrite; // Can't assert twice in the same method
        private static CodeAccessPermission changeWindowRegionForTopLevel;
        private static CodeAccessPermission controlFromHandleOrLocation;
        private static CodeAccessPermission createAnyWindow;
        private static CodeAccessPermission createGraphicsForControl;
        private static CodeAccessPermission defaultPrinting;
        private static CodeAccessPermission fileDialogCustomization;
        private static CodeAccessPermission fileDialogOpenFile;
        private static CodeAccessPermission fileDialogSaveFile;
        private static CodeAccessPermission getCapture;
        private static CodeAccessPermission getParent;
        private static CodeAccessPermission manipulateWndProcAndHandles;
        private static CodeAccessPermission modifyCursor;
        private static CodeAccessPermission modifyFocus;
        private static CodeAccessPermission objectFromWin32Handle;
        private static CodeAccessPermission safePrinting;
        private static CodeAccessPermission safeSubWindows;
        private static CodeAccessPermission safeTopLevelWindows;
        private static CodeAccessPermission sendMessages;
        private static CodeAccessPermission sensitiveSystemInformation;
        private static CodeAccessPermission transparentWindows;
        private static CodeAccessPermission topLevelWindow;
        private static CodeAccessPermission unmanagedCode;
        private static CodeAccessPermission unrestrictedWindows;
        private static CodeAccessPermission windowAdornmentModification;

        /* Unused permissions
        private static CodeAccessPermission win32HandleManipulation;
        private static CodeAccessPermission minimizeWindowProgrammatically;
        private static CodeAccessPermission restrictedWebBrowserPermission;
        private static CodeAccessPermission noPrinting;
        private static CodeAccessPermission topMostWindow;
        private static CodeAccessPermission unrestrictedEnvironment;
        private static CodeAccessPermission autoComplete;
        private static CodeAccessPermission defaultWebBrowserPermission;
        private static CodeAccessPermission screenDC;
        */

        //        
        // Property accessors for permissions.  Don't allocate permissions up front -- always
        // demand create them.  A great many codepaths never need all these permissions so it is wasteful to
        // create them.
        //
        
        
        public static CodeAccessPermission AdjustCursorClip { 
            get { 
                if (adjustCursorClip == null) {
                    adjustCursorClip = AllWindows;
                }
                return adjustCursorClip;
            } 
        }
        
        public static CodeAccessPermission AdjustCursorPosition { 
            get { 
                return AllWindows;
            } 
        }

        public static CodeAccessPermission AffectMachineState {
            get { 
                if (affectMachineState == null) {
                    affectMachineState = UnmanagedCode;
                }
                return affectMachineState;
            } 
        }
        
        public static CodeAccessPermission AffectThreadBehavior {
            get { 
                if (affectThreadBehavior == null) {
                    affectThreadBehavior = UnmanagedCode;
                }
                return affectThreadBehavior;
            } 
        }
        
        public static CodeAccessPermission AllPrinting {
            get { 
                if (allPrinting == null) {
                    allPrinting = new PrintingPermission(PrintingPermissionLevel.AllPrinting);
                }
                return allPrinting;
            } 
        }
        
        public static PermissionSet AllPrintingAndUnmanagedCode { // Can't assert twice in the same method.
            get {
                if (allPrintingAndUnmanagedCode == null) {
                    PermissionSet temp = new PermissionSet(PermissionState.None);
                    temp.SetPermission(IntSecurity.UnmanagedCode);
                    temp.SetPermission(IntSecurity.AllPrinting);
                    allPrintingAndUnmanagedCode = temp;
                }
                return allPrintingAndUnmanagedCode;
            } 
        }
        
        public static CodeAccessPermission AllWindows {
            get { 
                if (allWindows == null) {
                    allWindows = new UIPermission(UIPermissionWindow.AllWindows);
                }
                return allWindows;
            } 
        }
        
        public static CodeAccessPermission ClipboardRead {
            get { 
                if (clipboardRead == null) {
                    clipboardRead = new UIPermission(UIPermissionClipboard.AllClipboard);
                }
                return clipboardRead;
            } 
        }

        public static CodeAccessPermission ClipboardOwn {
            get { 
                if (clipboardOwn == null) {
                    clipboardOwn = new UIPermission(UIPermissionClipboard.OwnClipboard);
                }
                return clipboardOwn;
            } 
        }

        public static PermissionSet ClipboardWrite { // Can't assert OwnClipboard & UnmanagedCode in the same context, need permission set.
            get {
                if (clipboardWrite == null) {
                    clipboardWrite = new PermissionSet(PermissionState.None);
                    clipboardWrite.SetPermission(IntSecurity.UnmanagedCode);
                    clipboardWrite.SetPermission(IntSecurity.ClipboardOwn);
                }
                return clipboardWrite;
            } 
        }
        
        public static CodeAccessPermission ChangeWindowRegionForTopLevel {
            get { 
                if (changeWindowRegionForTopLevel == null) {
                    changeWindowRegionForTopLevel = AllWindows;
                }
                return changeWindowRegionForTopLevel;
            } 
        }
        
        public static CodeAccessPermission ControlFromHandleOrLocation {
            get { 
                if (controlFromHandleOrLocation == null) {
                    controlFromHandleOrLocation = AllWindows;
                }
                return controlFromHandleOrLocation;
            } 
        }
        
        public static CodeAccessPermission CreateAnyWindow {
            get { 
                if (createAnyWindow == null) {
                    createAnyWindow = SafeSubWindows;
                }
                return createAnyWindow;
            } 
        }
        
        public static CodeAccessPermission CreateGraphicsForControl {
            get { 
                if (createGraphicsForControl == null) {
                    createGraphicsForControl = SafeSubWindows;
                }
                return createGraphicsForControl;
            } 
        }
        
        public static CodeAccessPermission DefaultPrinting {
            get { 
                if (defaultPrinting == null) {
                    defaultPrinting = new PrintingPermission(PrintingPermissionLevel.DefaultPrinting);
                }
                return defaultPrinting;
            } 
        }
        
        public static CodeAccessPermission FileDialogCustomization {
            get { 
                if (fileDialogCustomization == null) {
                    fileDialogCustomization = new FileIOPermission(PermissionState.Unrestricted);
                }
                return fileDialogCustomization;
            } 
        }
        
        public static CodeAccessPermission FileDialogOpenFile {
            get { 
                if (fileDialogOpenFile == null) {
                    fileDialogOpenFile = new FileDialogPermission(FileDialogPermissionAccess.Open);
                }
                return fileDialogOpenFile;
            } 
        }
        
        public static CodeAccessPermission FileDialogSaveFile {
            get { 
                if (fileDialogSaveFile == null) {
                    fileDialogSaveFile = new FileDialogPermission(FileDialogPermissionAccess.Save);
                }
                return fileDialogSaveFile;
            } 
        }

        /* Unused
        public static CodeAccessPermission RestrictedWebBrowserPermission {
            get {
                if (restrictedWebBrowserPermission == null) {
                    restrictedWebBrowserPermission = new WebBrowserPermission(WebBrowserPermissionLevel.Restricted);
                }
                return restrictedWebBrowserPermission;
            }
        }

        public static CodeAccessPermission DefaultWebBrowserPermission {
            get {
                if (defaultWebBrowserPermission == null) {
                    defaultWebBrowserPermission = new WebBrowserPermission(WebBrowserPermissionLevel.Default);
                }
                return defaultWebBrowserPermission;
            }
        }
        */

        public static CodeAccessPermission GetCapture {
            get { 
                if (getCapture == null) {
                    getCapture = AllWindows;
                }
                return getCapture;
            } 
        }
        
        public static CodeAccessPermission GetParent {
            get { 
                if (getParent == null) {
                    getParent = AllWindows;
                }
                return getParent;
            } 
        }
        
        public static CodeAccessPermission ManipulateWndProcAndHandles {
            get { 
                if (manipulateWndProcAndHandles == null) {
                    manipulateWndProcAndHandles = AllWindows;
                }
                return manipulateWndProcAndHandles;
            } 
        }
        
        /* Unused
        public static CodeAccessPermission MinimizeWindowProgrammatically {
            get { 
                if (minimizeWindowProgrammatically == null) {
                    minimizeWindowProgrammatically = AllWindows;
                }
                return minimizeWindowProgrammatically;
            } 
        }
        */
        
        public static CodeAccessPermission ModifyCursor {
            get { 
                if (modifyCursor == null) {
                    modifyCursor = SafeSubWindows;
                }
                return modifyCursor;
            } 
        }
        
        public static CodeAccessPermission ModifyFocus {
            get { 
                if (modifyFocus == null) {
                    modifyFocus = AllWindows;
                }
                return modifyFocus;
            } 
        }
        
        /* Unused
        public static CodeAccessPermission NoPrinting {
            get { 
                if (noPrinting == null) {
                    noPrinting = new PrintingPermission(PrintingPermissionLevel.NoPrinting);
                }
                return noPrinting;
            } 
        }
        */
        
        public static CodeAccessPermission ObjectFromWin32Handle {
            get { 
                if (objectFromWin32Handle == null) {
                    objectFromWin32Handle = UnmanagedCode;
                }
                return objectFromWin32Handle;
            } 
        }
        
        public static CodeAccessPermission SafePrinting {
            get { 
                if (safePrinting == null) {
                    safePrinting = new PrintingPermission(PrintingPermissionLevel.SafePrinting);
                }
                return safePrinting;
            } 
        }
        
        public static CodeAccessPermission SafeSubWindows {
            get { 
                if (safeSubWindows == null) {
                    safeSubWindows = new UIPermission(UIPermissionWindow.SafeSubWindows);
                }
                return safeSubWindows;
            } 
        }
        
        public static CodeAccessPermission SafeTopLevelWindows {
            get { 
                if (safeTopLevelWindows == null) {
                    safeTopLevelWindows = new UIPermission(UIPermissionWindow.SafeTopLevelWindows);
                }
                return safeTopLevelWindows;
            } 
        }
        
        public static CodeAccessPermission SendMessages {
            get { 
                if (sendMessages == null) {
                    sendMessages = UnmanagedCode;
                }
                return sendMessages;
            } 
        }
        
        public static CodeAccessPermission SensitiveSystemInformation {
            get { 
                if (sensitiveSystemInformation == null) {
                    sensitiveSystemInformation = new EnvironmentPermission(PermissionState.Unrestricted);
                }
                return sensitiveSystemInformation;
            } 
        }

        /* Unused
        public static CodeAccessPermission ScreenDC {
            get { 
                if (screenDC == null) {
                    screenDC = AllWindows;
                }
                return screenDC;
            } 
        }
        */
        
        public static CodeAccessPermission TransparentWindows {
            get { 
                if (transparentWindows == null) {
                    transparentWindows = AllWindows;
                }
                return transparentWindows;
            } 
        }

        public static CodeAccessPermission TopLevelWindow {
            get { 
                if (topLevelWindow == null) {
                    topLevelWindow = SafeTopLevelWindows;
                }
                return topLevelWindow;
            } 
        }
        
        /* Unused
        public static CodeAccessPermission TopMostWindow {
            get { 
                if (topMostWindow == null) {
                    topMostWindow = AllWindows;
                }
                return topMostWindow;
            } 
        }
        */
        
        public static CodeAccessPermission UnmanagedCode {
            get { 
                if (unmanagedCode == null) {
                    unmanagedCode = new SecurityPermission(SecurityPermissionFlag.UnmanagedCode);
                }
                return unmanagedCode;
            } 
        }
        
        /*
        public static CodeAccessPermission UnrestrictedEnvironment {
            get { 
                if (unrestrictedEnvironment == null) {
                    unrestrictedEnvironment = new EnvironmentPermission(PermissionState.Unrestricted);
                }
                return unrestrictedEnvironment;
            } 
        }
        */
        
        public static CodeAccessPermission UnrestrictedWindows {
            get { 
                if (unrestrictedWindows == null) {
                    unrestrictedWindows = AllWindows;
                }
                return unrestrictedWindows;
            } 
        }
        
        /* FXCop avoid unused code
        public static CodeAccessPermission Win32HandleManipulation {
            get { 
                if (win32HandleManipulation == null) {
                    win32HandleManipulation = UnmanagedCode;
                }
                return win32HandleManipulation;
            } 
        }
        */
        
        public static CodeAccessPermission WindowAdornmentModification {
            get { 
                if (windowAdornmentModification == null) {
                    windowAdornmentModification = AllWindows;
                }
                return windowAdornmentModification;
            } 
        }

        /* unused
        public static CodeAccessPermission AutoComplete {
            get { 
                if (autoComplete == null) {
                    autoComplete = UnmanagedCode;
                }
                return autoComplete;
            } 
        }
        */

        [ResourceExposure(ResourceScope.Machine)]
        [ResourceConsumption(ResourceScope.Machine)]
        internal static string UnsafeGetFullPath(string fileName) {
            string full = fileName;

            FileIOPermission fiop = new FileIOPermission( PermissionState.None );
            fiop.AllFiles = FileIOPermissionAccess.PathDiscovery;
            fiop.Assert();
            try {
                full = Path.GetFullPath(fileName);
            }
            finally {
                CodeAccessPermission.RevertAssert();
            }
            return full;
        }

        /// 


        [SuppressMessage("Microsoft.Security", "CA2103:ReviewImperativeSecurity")]
        internal static void DemandFileIO(FileIOPermissionAccess access, string fileName) {
            new FileIOPermission(access, UnsafeGetFullPath(fileName)).Demand();
        }        
    }
}

