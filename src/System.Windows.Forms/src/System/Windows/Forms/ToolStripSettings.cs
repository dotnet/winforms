// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Configuration;
using System.Drawing;

namespace System.Windows.Forms;

/// <summary>
///  A settings class used by the ToolStripManager to save toolstrip settings.
/// </summary>
internal partial class ToolStripSettings : ApplicationSettingsBase
{
    internal ToolStripSettings(string settingsKey) : base(settingsKey) { }

    [UserScopedSetting]
    [DefaultSettingValue("true")]
    public bool IsDefault
    {
        get
        {
            return (bool)this[nameof(IsDefault)];
        }
        set
        {
            this[nameof(IsDefault)] = value;
        }
    }

    [UserScopedSetting]
    public string? ItemOrder
    {
        get
        {
            return this[nameof(ItemOrder)] as string;
        }
        set
        {
            this[nameof(ItemOrder)] = value;
        }
    }

    [UserScopedSetting]
    public string? Name
    {
        get
        {
            return this[nameof(Name)] as string;
        }
        set
        {
            this[nameof(Name)] = value;
        }
    }

    [UserScopedSetting]
    [DefaultSettingValue("0,0")]
    public Point Location
    {
        get
        {
            return (Point)this[nameof(Location)];
        }
        set
        {
            this[nameof(Location)] = value;
        }
    }

    [UserScopedSetting]
    [DefaultSettingValue("0,0")]
    public Size Size
    {
        get
        {
            return (Size)this[nameof(Size)];
        }
        set
        {
            this[nameof(Size)] = value;
        }
    }

    [UserScopedSetting]
    public string? ToolStripPanelName
    {
        get
        {
            return this[nameof(ToolStripPanelName)] as string;
        }
        set
        {
            this[nameof(ToolStripPanelName)] = value;
        }
    }

    [UserScopedSetting]
    [DefaultSettingValue("true")]
    public bool Visible
    {
        get
        {
            return (bool)this[nameof(Visible)];
        }
        set
        {
            this[nameof(Visible)] = value;
        }
    }

    public override void Save()
    {
        IsDefault = false;
        base.Save();
    }
}
