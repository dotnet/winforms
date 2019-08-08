// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    /// <summary>
    ///  Specifies the EncryptionLevel of the document in the WebBrowser control.
    ///  Returned by the <see cref='WebBrowser.EncryptionLevel'/> property.
    /// </summary>
    public enum WebBrowserEncryptionLevel
    {
        Insecure = 0,
        Mixed = 1,
        Unknown = 2,
        Bit40 = 3,
        Bit56 = 4,
        Fortezza = 5,
        Bit128 = 6
    }
}
