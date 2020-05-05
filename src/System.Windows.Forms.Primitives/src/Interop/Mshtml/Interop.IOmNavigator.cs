// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;

internal partial class Interop
{
    internal partial class Mshtml
    {
        [ComImport]
        [Guid("FECEAAA5-8405-11CF-8BA1-00AA00476DA6")]
        [InterfaceType(ComInterfaceType.InterfaceIsDual)]
        public interface IOmNavigator
        {
            string GetAppCodeName();
            string GetAppName();
            string GetAppVersion();
            string GetUserAgent();
            bool JavaEnabled();
            bool TaintEnabled();
            object GetMimeTypes();
            object GetPlugins();
            bool GetCookieEnabled();
            object GetOpsProfile();
            string GetCpuClass();
            string GetSystemLanguage();
            string GetBrowserLanguage();
            string GetUserLanguage();
            string GetPlatform();
            string GetAppMinorVersion();
            int GetConnectionSpeed();
            bool GetOnLine();
            object GetUserProfile();
        }
    }
}
