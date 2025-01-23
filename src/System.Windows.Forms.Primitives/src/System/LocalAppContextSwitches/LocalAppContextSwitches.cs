// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.CompilerServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Versioning;

namespace System.Windows.Forms.Primitives;

// Borrowed from https://github.com/dotnet/runtime/blob/main/src/libraries/Common/src/System/LocalAppContextSwitches.Common.cs
internal static partial class LocalAppContextSwitches
{
    // Enabling switches in Core is different from Framework. See https://learn.microsoft.com/dotnet/core/runtime-config/
    // for details on how to set switches.

    // Switch names declared internal below are used in unit/integration tests. Refer to
    // https://github.com/dotnet/winforms/blob/tree/main/docs/design/anchor-layout-changes-in-net80.md
    // for more details on how to enable these switches in the application.
    private const string ScaleTopLevelFormMinMaxSizeForDpiSwitchName = "System.Windows.Forms.ScaleTopLevelFormMinMaxSizeForDpi";
    internal const string AnchorLayoutV2SwitchName = "System.Windows.Forms.AnchorLayoutV2";
    internal const string ApplyParentFontToMenusSwitchName = "System.Windows.Forms.ApplyParentFontToMenus";
    internal const string ServicePointManagerCheckCrlSwitchName = "System.Windows.Forms.ServicePointManagerCheckCrl";
    internal const string TrackBarModernRenderingSwitchName = "System.Windows.Forms.TrackBarModernRendering";
    private const string DoNotCatchUnhandledExceptionsSwitchName = "System.Windows.Forms.DoNotCatchUnhandledExceptions";
    internal const string DataGridViewUIAStartRowCountAtZeroSwitchName = "System.Windows.Forms.DataGridViewUIAStartRowCountAtZero";
    internal const string NoClientNotificationsSwitchName = "Switch.System.Windows.Forms.AccessibleObject.NoClientNotifications";
    internal const string EnableMsoComponentManagerSwitchName = "Switch.System.Windows.Forms.EnableMsoComponentManager";
    internal const string TreeNodeCollectionAddRangeRespectsSortOrderSwitchName = "System.Windows.Forms.TreeNodeCollectionAddRangeRespectsSortOrder";
    internal const string ClipboardDragDropEnableUnsafeBinaryFormatterSerializationSwitchName = "Windows.ClipboardDragDrop.EnableUnsafeBinaryFormatterSerialization";
    internal const string ClipboardDragDropEnableNrbfSerializationSwitchName = "Windows.ClipboardDragDrop.EnableNrbfSerialization";
    internal const string MoveTreeViewTextLocationOnePixelSwitchName = "System.Windows.Forms.TreeView.MoveTreeViewTextLocationOnePixel";

    private static int s_scaleTopLevelFormMinMaxSizeForDpi;
    private static int s_anchorLayoutV2;
    private static int s_applyParentFontToMenus;
    private static int s_servicePointManagerCheckCrl;
    private static int s_trackBarModernRendering;
    private static int s_doNotCatchUnhandledExceptions;
    private static int s_dataGridViewUIAStartRowCountAtZero;
    private static int s_noClientNotifications;
    private static int s_enableMsoComponentManager;
    private static int s_treeNodeCollectionAddRangeRespectsSortOrder;
    private static int s_clipboardDragDropEnableUnsafeBinaryFormatterSerialization;
    private static int s_clipboardDragDropEnableNrbfSerialization;
    private static int s_moveTreeViewTextLocationOnePixel;

    private static FrameworkName? s_targetFrameworkName;

    /// <summary>
    ///  When there is no exception handler registered for a thread, re-throws the exception. The exception will
    ///  not be presented in a dialog or swallowed when not in interactive mode. This is always opt-in and is
    ///  intended for scenarios where setting handlers for threads isn't practical.
    /// </summary>
    public static bool DoNotCatchUnhandledExceptions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => GetCachedSwitchValue(DoNotCatchUnhandledExceptionsSwitchName, ref s_doNotCatchUnhandledExceptions);
    }

    /// <summary>
    ///  The <see cref="TargetFrameworkAttribute"/> value for the entry assembly, if any.
    /// </summary>
    public static FrameworkName? TargetFrameworkName
    {
        get
        {
            s_targetFrameworkName ??= AppContext.TargetFrameworkName is { } name ? new(name) : null;
            return s_targetFrameworkName;
        }
    }

    private static bool GetCachedSwitchValue(string switchName, ref int cachedSwitchValue)
    {
        // The cached switch value has 3 states: 0 - unknown, 1 - true, -1 - false
        if (cachedSwitchValue < 0)
        {
            return false;
        }

        if (cachedSwitchValue > 0)
        {
            return true;
        }

        return GetSwitchValue(switchName, ref cachedSwitchValue);
    }

    private static bool GetSwitchValue(string switchName, ref int cachedSwitchValue)
    {
        bool hasSwitch = AppContext.TryGetSwitch(switchName, out bool isSwitchEnabled);
        if (!hasSwitch)
        {
            isSwitchEnabled = GetSwitchDefaultValue(switchName);
        }

        AppContext.TryGetSwitch("TestSwitch.LocalAppContext.DisableCaching", out bool disableCaching);
        if (!disableCaching)
        {
            cachedSwitchValue = isSwitchEnabled ? 1 /*true*/ : -1 /*false*/;
        }
        else if (!hasSwitch)
        {
            AppContext.SetSwitch(switchName, isSwitchEnabled);
        }

        return isSwitchEnabled;
    }

    private static bool GetSwitchDefaultValue(string switchName)
    {
        if (switchName == TreeNodeCollectionAddRangeRespectsSortOrderSwitchName)
        {
            return true;
        }

        if (switchName == ClipboardDragDropEnableNrbfSerializationSwitchName)
        {
            return true;
        }

        if (TargetFrameworkName is not { } framework)
        {
            return false;
        }

        if (framework.Version.Major >= 8)
        {
            // Behavior changes added in .NET 8

            if (switchName == ScaleTopLevelFormMinMaxSizeForDpiSwitchName)
            {
                return true;
            }

            if (switchName == TrackBarModernRenderingSwitchName)
            {
                return true;
            }

            if (switchName == ServicePointManagerCheckCrlSwitchName)
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    ///  Indicates whether AnchorLayoutV2 feature is enabled.
    /// </summary>
    /// <devdoc>
    ///  Returns AnchorLayoutV2 switch value from runtimeconfig.json. Defaults to false.
    ///  Refer to
    ///  https://github.com/dotnet/winforms/blob/tree/main/docs/design/anchor-layout-changes-in-net80.md for more details.
    /// </devdoc>
    public static bool AnchorLayoutV2
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => GetCachedSwitchValue(AnchorLayoutV2SwitchName, ref s_anchorLayoutV2);
    }

    /// <summary>
    ///  Gets or sets a value indicating whether the parent font (as set by
    ///  <see cref="M:System.Windows.Forms.Application.SetDefaultFont(System.Drawing.Font)" />
    ///  or by the parent control or form's font) is applied to menus.
    /// </summary>
    public static bool ApplyParentFontToMenus
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => GetCachedSwitchValue(ApplyParentFontToMenusSwitchName, ref s_applyParentFontToMenus);
    }

    public static bool ScaleTopLevelFormMinMaxSizeForDpi
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => GetCachedSwitchValue(ScaleTopLevelFormMinMaxSizeForDpiSwitchName, ref s_scaleTopLevelFormMinMaxSizeForDpi);
    }

    public static bool TrackBarModernRendering
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => GetCachedSwitchValue(TrackBarModernRenderingSwitchName, ref s_trackBarModernRendering);
    }

    /// <summary>
    ///  Indicates whether certificates are checked against the certificate authority revocation list.
    ///  If true, revoked certificates will not be accepted by WebRequests and WebClients as valid.
    ///  Otherwise, revoked certificates will be accepted as valid.
    /// </summary>
    public static bool ServicePointManagerCheckCrl
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => GetCachedSwitchValue(ServicePointManagerCheckCrlSwitchName, ref s_servicePointManagerCheckCrl);
    }

    public static bool DataGridViewUIAStartRowCountAtZero
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => GetCachedSwitchValue(DataGridViewUIAStartRowCountAtZeroSwitchName, ref s_dataGridViewUIAStartRowCountAtZero);
    }

    public static bool NoClientNotifications
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => GetCachedSwitchValue(NoClientNotificationsSwitchName, ref s_noClientNotifications);
    }

    /// <summary>
    ///  If <see langword="true"/> Windows Forms threads will register with existing IMsoComponentManager instances.
    /// </summary>
    /// <remarks>
    ///  <para>
    ///   See <see href="https://learn.microsoft.com/previous-versions/office/developer/office-2007/ff518974(v=office.12)">
    ///   Component API Reference for the 2007 Office System</see> for more information on the IMsoComponentManager.
    ///  </para>
    /// </remarks>
    public static bool EnableMsoComponentManager
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => GetCachedSwitchValue(EnableMsoComponentManagerSwitchName, ref s_enableMsoComponentManager);
    }

    /// <summary>
    ///  When set to (default), API will insert nodes in the sorted order.
    ///  To get behavior compatible with the previous versions of .NET and .NET Framework, set this switch to.
    /// </summary>
    public static bool TreeNodeCollectionAddRangeRespectsSortOrder
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => GetCachedSwitchValue(TreeNodeCollectionAddRangeRespectsSortOrderSwitchName, ref s_treeNodeCollectionAddRangeRespectsSortOrder);
    }

    /// <summary>
    ///  If <see langword="true"/>, then Clipboard and DataObject Get and Set methods will attempts to serialize or deserialize
    ///  binary formatted content using either <see cref="BinaryFormatter"/> or System.Windows.Forms.BinaryFormat.Deserializer.
    ///  To use <see cref="BinaryFormatter"/>, application should also opt in into the
    ///  "System.Runtime.Serialization.EnableUnsafeBinaryFormatterSerialization" option and reference the out-of-band
    ///  "System.Runtime.Serialization.Formatters" NuGet package and opt out from using the System.Windows.Forms.BinaryFormat.Deserializer
    ///  by setting "Windows.ClipboardDragDrop.EnableNrbfSerialization" to <see langword="true"/>
    /// </summary>
    public static bool ClipboardDragDropEnableUnsafeBinaryFormatterSerialization
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => GetCachedSwitchValue(ClipboardDragDropEnableUnsafeBinaryFormatterSerializationSwitchName,
            ref s_clipboardDragDropEnableUnsafeBinaryFormatterSerialization);
    }

    /// <summary>
    ///  If <see langword="true"/>, then Clipboard Get methods will prefer System.Windows.Forms.BinaryFormat.Deserializer
    ///  to deserialize the payload, if needed. If <see langword="false"/>, then <see cref="BinaryFormatter"/> is used
    ///  to get full compatibility with the downlevel versions of .NET.
    /// </summary>
    /// <remarks>
    ///  <para>
    ///   This switch has no effect if "Windows.ClipboardDragDrop.EnableUnsafeBinaryFormatterSerialization"
    ///   is set to <see langword="false"/>.
    ///  </para>
    /// </remarks>
    public static bool ClipboardDragDropEnableNrbfSerialization
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => GetCachedSwitchValue(ClipboardDragDropEnableNrbfSerializationSwitchName, ref s_clipboardDragDropEnableNrbfSerialization);
    }

    /// <summary>
    ///  Indicates whether to move the text position of a TreeView node one pixel
    ///  to the right relative to the upper-left corner of the TreeView control.
    /// </summary>
    public static bool MoveTreeViewTextLocationOnePixel
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => GetCachedSwitchValue(MoveTreeViewTextLocationOnePixelSwitchName, ref s_moveTreeViewTextLocationOnePixel);
    }
}
