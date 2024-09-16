// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.Serialization.Formatters.Binary;

namespace System;

public static class AppContextSwitchNames
{
    /// <summary>
    ///  The switch that controls whether AnchorLayoutV2 feature is enabled.
    /// </summary>
    public const string AnchorLayoutV2
        = "System.Windows.Forms.AnchorLayoutV2";

    /// <summary>
    ///  The switch that controls whether the parent font
    ///  (as set by <see cref="M:System.Windows.Forms.Application.SetDefaultFont(System.Drawing.Font)" />
    ///  or by the parent control or form's font) is applied to menus.
    /// </summary>
    public const string ApplyParentFontToMenus
        = "System.Windows.Forms.ApplyParentFontToMenus";

    /// <summary>
    ///  The switch that controls whether or not the DataGridView starts its UI row count at zero.
    /// </summary>
    public const string DataGridViewUIAStartRowCountAtZero
        = "System.Windows.Forms.DataGridViewUIAStartRowCountAtZero";

    /// <summary>
    ///  The switch that controls whether or not the <see cref="BinaryFormatter"/> is enabled.
    /// </summary>
    public const string EnableUnsafeBinaryFormatterSerialization
        = "System.Runtime.Serialization.EnableUnsafeBinaryFormatterSerialization";

    /// <summary>
    ///  Switch that controls <see cref="AppContext"/> switch caching.
    /// </summary>
    public const string LocalAppContext_DisableCaching
        = "TestSwitch.LocalAppContext.DisableCaching";

    /// <summary>
    ///  The switch that controls whether UIA notifications are raised.
    /// </summary>
    public const string NoClientNotifications
        = "Switch.System.Windows.Forms.AccessibleObject.NoClientNotifications";

    /// <summary>
    ///  The switch that controls whether to scale the top level form min/max size for dpi.
    /// </summary>
    public const string ScaleTopLevelFormMinMaxSizeForDpi
        = "System.Windows.Forms.ScaleTopLevelFormMinMaxSizeForDpi";

    /// <summary>
    ///  The switch that controls whether certificates are checked against the certificate authority revocation list.
    ///  If true, revoked certificates will not be accepted by WebRequests and WebClients as valid.
    ///  Otherwise, revoked certificates will be accepted as valid.
    /// </summary>
    public const string ServicePointManagerCheckCrl
        = "System.Windows.Forms.ServicePointManagerCheckCrl";

    /// <summary>
    ///  The switch that controls whether the TreeNodeCollection will insert nodes in the sorted order.
    /// </summary>
    public const string TreeNodeCollectionAddRangeRespectsSortOrder
        = "System.Windows.Forms.ApplyParentFontToMenus";
}
