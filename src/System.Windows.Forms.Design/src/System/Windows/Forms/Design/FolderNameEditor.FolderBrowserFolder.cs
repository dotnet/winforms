// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms.Design;

public partial class FolderNameEditor
{
    /// <devdoc>
    ///  <para>
    ///   <see cref="Environment.SpecialFolder"/> is also a list of CSIDL values. Unfortunately CSIDL_CONNECTIONS,
    ///   CSIDL_NETWORK, and CSIDL_PRINTERS are not currently defined in that enum and can't be passed to
    ///   <see cref="Environment.GetFolderPath(Environment.SpecialFolder)"/>.
    ///  </para>
    /// </devdoc>
    protected enum FolderBrowserFolder
    {
        Desktop = (int)PInvoke.CSIDL_DESKTOP,
        Favorites = (int)PInvoke.CSIDL_FAVORITES,
        MyComputer = (int)PInvoke.CSIDL_DRIVES,
        MyDocuments = (int)PInvoke.CSIDL_PERSONAL,
        MyPictures = (int)PInvoke.CSIDL_MYPICTURES,
        NetAndDialUpConnections = (int)PInvoke.CSIDL_CONNECTIONS,
        NetworkNeighborhood = (int)PInvoke.CSIDL_NETWORK,
        Printers = (int)PInvoke.CSIDL_PRINTERS,
        Recent = (int)PInvoke.CSIDL_RECENT,
        SendTo = (int)PInvoke.CSIDL_SENDTO,
        StartMenu = (int)PInvoke.CSIDL_STARTMENU,
        Templates = (int)PInvoke.CSIDL_TEMPLATES
    }
}
