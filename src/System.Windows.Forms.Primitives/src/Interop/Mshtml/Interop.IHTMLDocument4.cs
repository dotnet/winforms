// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;

internal partial class Interop
{
    internal partial class Mshtml
    {
        [ComImport]
        [Guid("3050F69A-98B5-11CF-BB82-00AA00BDCE0B")]
        [InterfaceType(ComInterfaceType.InterfaceIsDual)]
        public interface IHTMLDocument4
        {
            void Focus();
            bool HasFocus();
            void SetOnselectionchange(object p);
            object GetOnselectionchange();
            object GetNamespaces();
            object createDocumentFromUrl(string bstrUrl, string bstrOptions);
            void SetMedia(string bstrMedia);
            string GetMedia();
            object CreateEventObject([In, Optional] ref object eventObject);
            bool FireEvent(string eventName, object pvarEventObject);
            object CreateRenderStyle(string bstr);
            void SetOncontrolselect(object p);
            object GetOncontrolselect();
            string GetURLUnencoded();
        }
    }
}
