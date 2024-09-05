// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel.Design;

namespace System.Windows.Forms.Design;

/// <summary>
///  This class contains command ID's and GUIDS that correspond to the host Command Bar menu layout.
/// </summary>
public sealed class MenuCommands : StandardCommands
{
    // Windows Forms specific popup menus
    private const int MnuidSelection = 0x0500;
    private const int MnuidContainer = 0x0501;
    private const int MnuidTraySelection = 0x0503;
    private const int MnuidComponentTray = 0x0506;

    // Windows Forms specific menu items
    private const int CmdidDesignerProperties = 0x1001;

    // Windows Forms specific keyboard commands
    private const int CmdidReverseCancel = 0x4001;
    private const int CmdidSetStatusText = 0x4003;
    private const int CmdidSetStatusRectangle = 0x4004;

    private const int CmdidSpace = 0x4015;

    private const int ECMD_CANCEL = 103;
    private const int ECMD_RETURN = 3;
    private const int ECMD_UP = 11;
    private const int ECMD_DOWN = 13;
    private const int ECMD_LEFT = 7;
    private const int ECMD_RIGHT = 9;
    private const int ECMD_RIGHT_EXT = 10;
    private const int ECMD_UP_EXT = 12;
    private const int ECMD_LEFT_EXT = 8;
    private const int ECMD_DOWN_EXT = 14;
    private const int ECMD_TAB = 4;
    private const int ECMD_BACKTAB = 5;
    private const int ECMD_INVOKESMARTTAG = 147;

    private const int ECMD_CTLMOVELEFT = 1224;
    private const int ECMD_CTLMOVEDOWN = 1225;
    private const int ECMD_CTLMOVERIGHT = 1226;
    private const int ECMD_CTLMOVEUP = 1227;
    private const int ECMD_CTLSIZEDOWN = 1228;
    private const int ECMD_CTLSIZEUP = 1229;
    private const int ECMD_CTLSIZELEFT = 1230;
    private const int ECMD_CTLSIZERIGHT = 1231;
    private const int CmdidEditLabel = 338;

    // Add Home and End Commands.
    private const int ECMD_HOME = 15;
    private const int ECMD_HOME_EXT = 16;
    private const int ECMD_END = 17;
    private const int ECMD_END_EXT = 18;

    /// <summary>
    ///  This guid corresponds to the standard set of commands for the shell and office.
    ///  This new guid is added so that the ToolStripDesigner now respond to the F2 command
    ///  and go into InSitu Edit mode.
    /// </summary>
    private static readonly Guid s_vsStandardCommandSet97 = new("{5efc7975-14bc-11cf-9b2b-00aa00573819}");

    /// <summary>
    ///  This guid corresponds to the menu grouping Windows Forms will use for its menus. This is
    ///  defined in the Windows Forms menu CTC file, but we need it here so we can define what
    ///  context menus to use.
    /// </summary>
    private static readonly Guid s_wfMenuGroup = new("{74D21312-2AEE-11d1-8BFB-00A0C90F26F7}");

    /// <summary>
    ///  This guid corresponds to the Windows Forms command set.
    /// </summary>
    private static readonly Guid s_wfCommandSet = new("{74D21313-2AEE-11d1-8BFB-00A0C90F26F7}");

    /// <summary>
    ///  This guid is the standard vs 2k commands for key bindings
    /// </summary>
    private static readonly Guid s_guidVSStd2K = new("{1496A755-94DE-11D0-8C3F-00C04FC2AAE2}");

    public static readonly CommandID SelectionMenu = new(s_wfMenuGroup, MnuidSelection);

    public static readonly CommandID ContainerMenu = new(s_wfMenuGroup, MnuidContainer);

    public static readonly CommandID TraySelectionMenu = new(s_wfMenuGroup, MnuidTraySelection);

    public static readonly CommandID ComponentTrayMenu = new(s_wfMenuGroup, MnuidComponentTray);

    // Windows Forms commands
    public static readonly CommandID DesignerProperties = new(s_wfCommandSet, CmdidDesignerProperties);

    // Windows Forms Key commands
    public static readonly CommandID KeyCancel = new(s_guidVSStd2K, ECMD_CANCEL);

    public static readonly CommandID KeyReverseCancel = new(s_wfCommandSet, CmdidReverseCancel);

    public static readonly CommandID KeyInvokeSmartTag = new(s_guidVSStd2K, ECMD_INVOKESMARTTAG);

    public static readonly CommandID KeyDefaultAction = new(s_guidVSStd2K, ECMD_RETURN);

    public static readonly CommandID KeyMoveUp = new(s_guidVSStd2K, ECMD_UP);

    public static readonly CommandID KeyMoveDown = new(s_guidVSStd2K, ECMD_DOWN);

    public static readonly CommandID KeyMoveLeft = new(s_guidVSStd2K, ECMD_LEFT);

    public static readonly CommandID KeyMoveRight = new(s_guidVSStd2K, ECMD_RIGHT);

    public static readonly CommandID KeyNudgeUp = new(s_guidVSStd2K, ECMD_CTLMOVEUP);

    public static readonly CommandID KeyNudgeDown = new(s_guidVSStd2K, ECMD_CTLMOVEDOWN);

    public static readonly CommandID KeyNudgeLeft = new(s_guidVSStd2K, ECMD_CTLMOVELEFT);

    public static readonly CommandID KeyNudgeRight = new(s_guidVSStd2K, ECMD_CTLMOVERIGHT);

    public static readonly CommandID KeySizeWidthIncrease = new(s_guidVSStd2K, ECMD_RIGHT_EXT);

    public static readonly CommandID KeySizeHeightIncrease = new(s_guidVSStd2K, ECMD_UP_EXT);

    public static readonly CommandID KeySizeWidthDecrease = new(s_guidVSStd2K, ECMD_LEFT_EXT);

    public static readonly CommandID KeySizeHeightDecrease = new(s_guidVSStd2K, ECMD_DOWN_EXT);

    public static readonly CommandID KeyNudgeWidthIncrease = new(s_guidVSStd2K, ECMD_CTLSIZERIGHT);

    public static readonly CommandID KeyNudgeHeightIncrease = new(s_guidVSStd2K, ECMD_CTLSIZEDOWN);

    public static readonly CommandID KeyNudgeWidthDecrease = new(s_guidVSStd2K, ECMD_CTLSIZELEFT);

    public static readonly CommandID KeyNudgeHeightDecrease = new(s_guidVSStd2K, ECMD_CTLSIZEUP);

    public static readonly CommandID KeySelectNext = new(s_guidVSStd2K, ECMD_TAB);

    public static readonly CommandID KeySelectPrevious = new(s_guidVSStd2K, ECMD_BACKTAB);

    public static readonly CommandID KeyTabOrderSelect = new(s_wfCommandSet, CmdidSpace);

    public static readonly CommandID EditLabel = new(s_vsStandardCommandSet97, CmdidEditLabel);

    public static readonly CommandID KeyHome = new(s_guidVSStd2K, ECMD_HOME);

    public static readonly CommandID KeyEnd = new(s_guidVSStd2K, ECMD_END);

    public static readonly CommandID KeyShiftHome = new(s_guidVSStd2K, ECMD_HOME_EXT);

    public static readonly CommandID KeyShiftEnd = new(s_guidVSStd2K, ECMD_END_EXT);

    public static readonly CommandID SetStatusText = new(s_wfCommandSet, CmdidSetStatusText);

    public static readonly CommandID SetStatusRectangle = new(s_wfCommandSet, CmdidSetStatusRectangle);
}
