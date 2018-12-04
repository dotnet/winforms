// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel.Design;

namespace System.Windows.Forms.Design
{
    /// <summary>
    ///     <para>
    ///         This class contains command ID's and GUIDS that
    ///         correspond
    ///         to the host Command Bar menu layout.
    ///     </para>
    /// </summary>
    public sealed class MenuCommands : StandardCommands
    {
        // Windows Forms specific popup menus
        //
        private const int mnuidSelection = 0x0500;
        private const int mnuidContainer = 0x0501;
        private const int mnuidTraySelection = 0x0503;
        private const int mnuidComponentTray = 0x0506;

        // Windows Forms specific menu items
        //
        private const int cmdidDesignerProperties = 0x1001;

        // Windows Forms specific keyboard commands
        //
        private const int cmdidReverseCancel = 0x4001;
        private const int cmdidSetStatusText = 0x4003;
        private const int cmdidSetStatusRectangle = 0x4004;

        private const int cmdidSpace = 0x4015;

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
        private const int cmdidEditLabel = 338;

        // Add Home and End Commands.
        private const int ECMD_HOME = 15;
        private const int ECMD_HOME_EXT = 16;
        private const int ECMD_END = 17;
        private const int ECMD_END_EXT = 18;

        /// <summary>
        ///     This guid corresponds to the standard set of commands for the shell and office.
        ///     This new giud is added so that the ToolStripDesigner now respond to the F2 command
        ///     and go into InSitu Edit mode.
        /// </summary>
        private static readonly Guid VSStandardCommandSet97 = new Guid("{5efc7975-14bc-11cf-9b2b-00aa00573819}");

        /// <summary>
        ///     This guid corresponds to the menu grouping Windows Forms will use for its menus.  This is
        ///     defined in the Windows Forms menu CTC file, but we need it here so we can define what
        ///     context menus to use.
        /// </summary>
        private static readonly Guid wfMenuGroup = new Guid("{74D21312-2AEE-11d1-8BFB-00A0C90F26F7}");

        /// <summary>
        ///     This guid corresponds to the Windows Forms command set.
        /// </summary>
        private static readonly Guid wfCommandSet = new Guid("{74D21313-2AEE-11d1-8BFB-00A0C90F26F7}");

        /// <summary>
        ///     This guid is the standard vs 2k commands for key bindings
        /// </summary>
        private static readonly Guid guidVSStd2K = new Guid("{1496A755-94DE-11D0-8C3F-00C04FC2AAE2}");

        /// <summary>
        ///     <para>[To be supplied.]</para>
        /// </summary>
        public static readonly CommandID SelectionMenu = new CommandID(wfMenuGroup, mnuidSelection);

        /// <summary>
        ///     <para>[To be supplied.]</para>
        /// </summary>
        public static readonly CommandID ContainerMenu = new CommandID(wfMenuGroup, mnuidContainer);

        /// <summary>
        ///     <para>[To be supplied.]</para>
        /// </summary>
        public static readonly CommandID TraySelectionMenu = new CommandID(wfMenuGroup, mnuidTraySelection);

        /// <summary>
        ///     <para>[To be supplied.]</para>
        /// </summary>
        public static readonly CommandID ComponentTrayMenu = new CommandID(wfMenuGroup, mnuidComponentTray);

        // Windows Forms commands

        /// <summary>
        ///     <para>[To be supplied.]</para>
        /// </summary>
        public static readonly CommandID DesignerProperties = new CommandID(wfCommandSet, cmdidDesignerProperties);

        // Windows Forms Key commands        

        /// <summary>
        ///     <para>[To be supplied.]</para>
        /// </summary>
        public static readonly CommandID KeyCancel = new CommandID(guidVSStd2K, ECMD_CANCEL);

        /// <summary>
        ///     <para>[To be supplied.]</para>
        /// </summary>
        public static readonly CommandID KeyReverseCancel = new CommandID(wfCommandSet, cmdidReverseCancel);

        /// <summary>
        ///     <para>[To be supplied.]</para>
        /// </summary>
        public static readonly CommandID KeyInvokeSmartTag = new CommandID(guidVSStd2K, ECMD_INVOKESMARTTAG);

        /// <summary>
        ///     <para>[To be supplied.]</para>
        /// </summary>
        public static readonly CommandID KeyDefaultAction = new CommandID(guidVSStd2K, ECMD_RETURN);

        /// <summary>
        ///     <para>[To be supplied.]</para>
        /// </summary>
        public static readonly CommandID KeyMoveUp = new CommandID(guidVSStd2K, ECMD_UP);

        /// <summary>
        ///     <para>[To be supplied.]</para>
        /// </summary>
        public static readonly CommandID KeyMoveDown = new CommandID(guidVSStd2K, ECMD_DOWN);

        /// <summary>
        ///     <para>[To be supplied.]</para>
        /// </summary>
        public static readonly CommandID KeyMoveLeft = new CommandID(guidVSStd2K, ECMD_LEFT);

        /// <summary>
        ///     <para>[To be supplied.]</para>
        /// </summary>
        public static readonly CommandID KeyMoveRight = new CommandID(guidVSStd2K, ECMD_RIGHT);

        /// <summary>
        ///     <para>[To be supplied.]</para>
        /// </summary>
        public static readonly CommandID KeyNudgeUp = new CommandID(guidVSStd2K, ECMD_CTLMOVEUP);

        /// <summary>
        ///     <para>[To be supplied.]</para>
        /// </summary>
        public static readonly CommandID KeyNudgeDown = new CommandID(guidVSStd2K, ECMD_CTLMOVEDOWN);

        /// <summary>
        ///     <para>[To be supplied.]</para>
        /// </summary>
        public static readonly CommandID KeyNudgeLeft = new CommandID(guidVSStd2K, ECMD_CTLMOVELEFT);

        /// <summary>
        ///     <para>[To be supplied.]</para>
        /// </summary>
        public static readonly CommandID KeyNudgeRight = new CommandID(guidVSStd2K, ECMD_CTLMOVERIGHT);

        /// <summary>
        ///     <para>[To be supplied.]</para>
        /// </summary>
        public static readonly CommandID KeySizeWidthIncrease = new CommandID(guidVSStd2K, ECMD_RIGHT_EXT);

        /// <summary>
        ///     <para>[To be supplied.]</para>
        /// </summary>
        public static readonly CommandID KeySizeHeightIncrease = new CommandID(guidVSStd2K, ECMD_UP_EXT);

        /// <summary>
        ///     <para>[To be supplied.]</para>
        /// </summary>
        public static readonly CommandID KeySizeWidthDecrease = new CommandID(guidVSStd2K, ECMD_LEFT_EXT);

        /// <summary>
        ///     <para>[To be supplied.]</para>
        /// </summary>
        public static readonly CommandID KeySizeHeightDecrease = new CommandID(guidVSStd2K, ECMD_DOWN_EXT);

        /// <summary>
        ///     <para>[To be supplied.]</para>
        /// </summary>
        public static readonly CommandID KeyNudgeWidthIncrease = new CommandID(guidVSStd2K, ECMD_CTLSIZERIGHT);

        /// <summary>
        ///     <para>[To be supplied.]</para>
        /// </summary>
        public static readonly CommandID KeyNudgeHeightIncrease = new CommandID(guidVSStd2K, ECMD_CTLSIZEDOWN);

        /// <summary>
        ///     <para>[To be supplied.]</para>
        /// </summary>
        public static readonly CommandID KeyNudgeWidthDecrease = new CommandID(guidVSStd2K, ECMD_CTLSIZELEFT);

        /// <summary>
        ///     <para>[To be supplied.]</para>
        /// </summary>
        public static readonly CommandID KeyNudgeHeightDecrease = new CommandID(guidVSStd2K, ECMD_CTLSIZEUP);

        /// <summary>
        ///     <para>[To be supplied.]</para>
        /// </summary>
        public static readonly CommandID KeySelectNext = new CommandID(guidVSStd2K, ECMD_TAB);

        /// <summary>
        ///     <para>[To be supplied.]</para>
        /// </summary>
        public static readonly CommandID KeySelectPrevious = new CommandID(guidVSStd2K, ECMD_BACKTAB);

        /// <summary>
        ///     <para>[To be supplied.]</para>
        /// </summary>
        public static readonly CommandID KeyTabOrderSelect = new CommandID(wfCommandSet, cmdidSpace);

        /// <summary>
        ///     <para>[To be supplied.]</para>
        /// </summary>
        public static readonly CommandID EditLabel = new CommandID(VSStandardCommandSet97, cmdidEditLabel);

        /// <summary>
        ///     <para>[To be supplied.]</para>
        /// </summary>
        public static readonly CommandID KeyHome = new CommandID(guidVSStd2K, ECMD_HOME);

        /// <summary>
        ///     <para>[To be supplied.]</para>
        /// </summary>
        public static readonly CommandID KeyEnd = new CommandID(guidVSStd2K, ECMD_END);

        /// <summary>
        ///     <para>[To be supplied.]</para>
        /// </summary>
        public static readonly CommandID KeyShiftHome = new CommandID(guidVSStd2K, ECMD_HOME_EXT);

        /// <summary>
        ///     <para>[To be supplied.]</para>
        /// </summary>
        public static readonly CommandID KeyShiftEnd = new CommandID(guidVSStd2K, ECMD_END_EXT);

        /// <summary>
        ///     <para>[To be supplied.]</para>
        /// </summary>
        public static readonly CommandID SetStatusText = new CommandID(wfCommandSet, cmdidSetStatusText);

        /// <summary>
        ///     <para>[To be supplied.]</para>
        /// </summary>
        public static readonly CommandID SetStatusRectangle = new CommandID(wfCommandSet, cmdidSetStatusRectangle);
    }
}
