// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// This file contains the state enums used by the control renderer classes.

using System.Diagnostics.CodeAnalysis;
[assembly: SuppressMessage("Microsoft.MSInternal", "CA905:SystemAndMicrosoftNamespacesRequireApproval", Scope="namespace", Target="System.Windows.Forms.VisualStyles")]

namespace System.Windows.Forms.VisualStyles {

    /// <include file='doc\VisualStyleStates.uex' path='docs/doc[@for="ComboBoxState"]/*' />
    [
        SuppressMessage("Microsoft.Design", "CA1008:EnumsShouldHaveZeroValue")
    ]
    public enum ComboBoxState {
        /// <include file='doc\VisualStyleStates.uex' path='docs/doc[@for="ComboBoxState.Normal"]/*' />
        Normal = 1,
        /// <include file='doc\VisualStyleStates.uex' path='docs/doc[@for="ComboBoxState.Hot"]/*' />
        Hot = 2,
        /// <include file='doc\VisualStyleStates.uex' path='docs/doc[@for="ComboBoxState.Pressed"]/*' />
        Pressed = 3,
        /// <include file='doc\VisualStyleStates.uex' path='docs/doc[@for="ComboBoxState.Disabled"]/*' />
        Disabled = 4
    }

    /// <include file='doc\VisualStyleStates.uex' path='docs/doc[@for="CheckBoxState"]/*' />
    [
        SuppressMessage("Microsoft.Design", "CA1008:EnumsShouldHaveZeroValue")
    ]
    public enum CheckBoxState {
        /// <include file='doc\VisualStyleStates.uex' path='docs/doc[@for="CheckBoxState.UncheckedNormal"]/*' />
        UncheckedNormal = 1, 
        /// <include file='doc\VisualStyleStates.uex' path='docs/doc[@for="CheckBoxState.UncheckedHot"]/*' />
        UncheckedHot = 2, 
        /// <include file='doc\VisualStyleStates.uex' path='docs/doc[@for="CheckBoxState.UncheckedPressed"]/*' />
        UncheckedPressed = 3, 
        /// <include file='doc\VisualStyleStates.uex' path='docs/doc[@for="CheckBoxState.UncheckedDisabled"]/*' />
        UncheckedDisabled = 4, 
        /// <include file='doc\VisualStyleStates.uex' path='docs/doc[@for="CheckBoxState.CheckedNormal"]/*' />
        CheckedNormal = 5, 
        /// <include file='doc\VisualStyleStates.uex' path='docs/doc[@for="CheckBoxState.CheckedHot"]/*' />
        CheckedHot = 6, 
        /// <include file='doc\VisualStyleStates.uex' path='docs/doc[@for="CheckBoxState.CheckedPressed"]/*' />
        CheckedPressed = 7, 
        /// <include file='doc\VisualStyleStates.uex' path='docs/doc[@for="CheckBoxState.CheckedDisabled"]/*' />
        CheckedDisabled = 8,
        /// <include file='doc\VisualStyleStates.uex' path='docs/doc[@for="CheckBoxState.MixedNormal"]/*' />
        MixedNormal = 9, 
        /// <include file='doc\VisualStyleStates.uex' path='docs/doc[@for="CheckBoxState.MixedHot"]/*' />
        MixedHot = 10, 
        /// <include file='doc\VisualStyleStates.uex' path='docs/doc[@for="CheckBoxState.MixedPressed"]/*' />
        MixedPressed = 11, 
        /// <include file='doc\VisualStyleStates.uex' path='docs/doc[@for="CheckBoxState.MixedDisabled"]/*' />
        MixedDisabled = 12
    } 
    
    /// <include file='doc\VisualStyleStates.uex' path='docs/doc[@for="GroupBoxState"]/*' />
    [
        SuppressMessage("Microsoft.Design", "CA1008:EnumsShouldHaveZeroValue")
    ]
    public enum GroupBoxState {
        /// <include file='doc\VisualStyleStates.uex' path='docs/doc[@for="GroupBoxState.Normal"]/*' />
        Normal = 1, 
        /// <include file='doc\VisualStyleStates.uex' path='docs/doc[@for="GroupBoxState.Disabled"]/*' />
        Disabled = 2 
    }

    // Used by some DataGridView classes.
    [
        SuppressMessage("Microsoft.Design", "CA1008:EnumsShouldHaveZeroValue")
    ]
    internal enum HeaderItemState {
        /// <include file='doc\VisualStyleStates.uex' path='docs/doc[@for="HeaderItemState.Normal"]/*' />
        Normal = 1,
        /// <include file='doc\VisualStyleStates.uex' path='docs/doc[@for="HeaderItemState.Hot"]/*' />
        Hot = 2,
        /// <include file='doc\VisualStyleStates.uex' path='docs/doc[@for="HeaderItemState.Pressed"]/*' />
        Pressed = 3
    }

    /// <include file='doc\VisualStyleStates.uex' path='docs/doc[@for="PushButtonState"]/*' />
    [
        SuppressMessage("Microsoft.Design", "CA1008:EnumsShouldHaveZeroValue")
    ]
    public enum PushButtonState {
        /// <include file='doc\VisualStyleStates.uex' path='docs/doc[@for="PushButtonState.Normal"]/*' />
        Normal = 1,
        /// <include file='doc\VisualStyleStates.uex' path='docs/doc[@for="PushButtonState.Hot"]/*' />
        Hot = 2,
        /// <include file='doc\VisualStyleStates.uex' path='docs/doc[@for="PushButtonState.Pressed"]/*' />
        Pressed = 3,
        /// <include file='doc\VisualStyleStates.uex' path='docs/doc[@for="PushButtonState.Disabled"]/*' />
        Disabled = 4,
        /// <include file='doc\VisualStyleStates.uex' path='docs/doc[@for="PushButtonState.Default"]/*' />
        Default = 5
    }
    
    /// <include file='doc\VisualStyleStates.uex' path='docs/doc[@for="RadioButtonState"]/*' />
    [
        SuppressMessage("Microsoft.Design", "CA1008:EnumsShouldHaveZeroValue")
    ]
    public enum RadioButtonState {
        /// <include file='doc\VisualStyleStates.uex' path='docs/doc[@for="RadioButtonState.UncheckedNormal"]/*' />
        UncheckedNormal = 1, 
        /// <include file='doc\VisualStyleStates.uex' path='docs/doc[@for="RadioButtonState.UncheckedHot"]/*' />
        UncheckedHot = 2, 
        /// <include file='doc\VisualStyleStates.uex' path='docs/doc[@for="RadioButtonState.UncheckedPressed"]/*' />
        UncheckedPressed = 3, 
        /// <include file='doc\VisualStyleStates.uex' path='docs/doc[@for="RadioButtonState.UncheckedDisabled"]/*' />
        UncheckedDisabled = 4, 
        /// <include file='doc\VisualStyleStates.uex' path='docs/doc[@for="RadioButtonState.CheckedNormal"]/*' />
        CheckedNormal = 5, 
        /// <include file='doc\VisualStyleStates.uex' path='docs/doc[@for="RadioButtonState.CheckedHot"]/*' />
        CheckedHot = 6, 
        /// <include file='doc\VisualStyleStates.uex' path='docs/doc[@for="RadioButtonState.CheckedPressed"]/*' />
        CheckedPressed = 7, 
        /// <include file='doc\VisualStyleStates.uex' path='docs/doc[@for="RadioButtonState.CheckedDisabled"]/*' />
        CheckedDisabled = 8
    } 

    /// <include file='doc\VisualStyleStates.uex' path='docs/doc[@for="ScrollBarArrowButtonState"]/*' />
    [
        SuppressMessage("Microsoft.Design", "CA1008:EnumsShouldHaveZeroValue")
    ]
    public enum ScrollBarArrowButtonState {
        /// <include file='doc\VisualStyleStates.uex' path='docs/doc[@for="ScrollBarArrowButtonState.UpNormal"]/*' />
        UpNormal = 1,
        /// <include file='doc\VisualStyleStates.uex' path='docs/doc[@for="ScrollBarArrowButtonState.UpHot"]/*' />
        UpHot = 2,
        /// <include file='doc\VisualStyleStates.uex' path='docs/doc[@for="ScrollBarArrowButtonState.UpPressed"]/*' />
        UpPressed = 3,
        /// <include file='doc\VisualStyleStates.uex' path='docs/doc[@for="ScrollBarArrowButtonState.UpDisabled"]/*' />
        UpDisabled = 4,
        /// <include file='doc\VisualStyleStates.uex' path='docs/doc[@for="ScrollBarArrowButtonState.DownNormal"]/*' />
        DownNormal = 5,
        /// <include file='doc\VisualStyleStates.uex' path='docs/doc[@for="ScrollBarArrowButtonState.DownHot"]/*' />
        DownHot = 6,
        /// <include file='doc\VisualStyleStates.uex' path='docs/doc[@for="ScrollBarArrowButtonState.DownPressed"]/*' />
        DownPressed = 7,
        /// <include file='doc\VisualStyleStates.uex' path='docs/doc[@for="ScrollBarArrowButtonState.DownDisabled"]/*' />
        DownDisabled = 8,
        /// <include file='doc\VisualStyleStates.uex' path='docs/doc[@for="ScrollBarArrowButtonState.LeftNormal"]/*' />
        LeftNormal = 9,
        /// <include file='doc\VisualStyleStates.uex' path='docs/doc[@for="ScrollBarArrowButtonState.LeftHot"]/*' />
        LeftHot = 10,
        /// <include file='doc\VisualStyleStates.uex' path='docs/doc[@for="ScrollBarArrowButtonState.LeftPressed"]/*' />
        LeftPressed = 11,
        /// <include file='doc\VisualStyleStates.uex' path='docs/doc[@for="ScrollBarArrowButtonState.LeftDisabled"]/*' />
        LeftDisabled = 12,
        /// <include file='doc\VisualStyleStates.uex' path='docs/doc[@for="ScrollBarArrowButtonState.RightNormal"]/*' />
        RightNormal = 13,
        /// <include file='doc\VisualStyleStates.uex' path='docs/doc[@for="ScrollBarArrowButtonState.RightHot"]/*' />
        RightHot = 14,
        /// <include file='doc\VisualStyleStates.uex' path='docs/doc[@for="ScrollBarArrowButtonState.RightPressed"]/*' />
        RightPressed = 15,
        /// <include file='doc\VisualStyleStates.uex' path='docs/doc[@for="ScrollBarArrowButtonState.RightDisabled"]/*' />
        RightDisabled = 16
    }


    /// <include file='doc\VisualStyleStates.uex' path='docs/doc[@for="ScrollBarState"]/*' />
    [
        SuppressMessage("Microsoft.Design", "CA1008:EnumsShouldHaveZeroValue")
    ]
    public enum ScrollBarState {
        /// <include file='doc\VisualStyleStates.uex' path='docs/doc[@for="ScrollBarState.Normal"]/*' />
        Normal = 1,
        /// <include file='doc\VisualStyleStates.uex' path='docs/doc[@for="ScrollBarState.Hot"]/*' />
        Hot = 2,
        /// <include file='doc\VisualStyleStates.uex' path='docs/doc[@for="ScrollBarState.Pressed"]/*' />
        Pressed = 3,
        /// <include file='doc\VisualStyleStates.uex' path='docs/doc[@for="ScrollBarState.Disabled"]/*' />
        Disabled = 4,
        
    }

    /// <include file='doc\VisualStyleStates.uex' path='docs/doc[@for="ScrollBarSizeBoxState"]/*' />
    [
        SuppressMessage("Microsoft.Design", "CA1008:EnumsShouldHaveZeroValue")
    ]
    public enum ScrollBarSizeBoxState {
        /// <include file='doc\VisualStyleStates.uex' path='docs/doc[@for="ScrollBarSizeBoxState.RightAlign"]/*' />
        RightAlign = 1,
        /// <include file='doc\VisualStyleStates.uex' path='docs/doc[@for="ScrollBarSizeBoxState.LeftAlign"]/*' />
        LeftAlign = 2
    }

    /// <include file='doc\VisualStyleStates.uex' path='docs/doc[@for="TabItemState"]/*' />
    [
        SuppressMessage("Microsoft.Design", "CA1008:EnumsShouldHaveZeroValue")
    ]
    public enum TabItemState {
        /// <include file='doc\VisualStyleStates.uex' path='docs/doc[@for="TabItemState.Normal"]/*' />
        Normal = 1,
        /// <include file='doc\VisualStyleStates.uex' path='docs/doc[@for="TabItemState.Hot"]/*' />
        Hot = 2,
        /// <include file='doc\VisualStyleStates.uex' path='docs/doc[@for="TabItemState.Selected"]/*' />
        Selected = 3,
        /// <include file='doc\VisualStyleStates.uex' path='docs/doc[@for="TabItemState.Disabled"]/*' />
        Disabled = 4,
        //Focused = 5 
    }

    /// <include file='doc\VisualStyleStates.uex' path='docs/doc[@for="TextBoxState"]/*' />
    [
        SuppressMessage("Microsoft.Design", "CA1008:EnumsShouldHaveZeroValue"),
        SuppressMessage("Microsoft.Design", "CA1027:MarkEnumsWithFlags")
    ]
    public enum TextBoxState {
        /// <include file='doc\VisualStyleStates.uex' path='docs/doc[@for="TextBoxState.Normal"]/*' />
        Normal = 1,
        /// <include file='doc\VisualStyleStates.uex' path='docs/doc[@for="TextBoxState.Hot"]/*' />
        Hot = 2,
        /// <include file='doc\VisualStyleStates.uex' path='docs/doc[@for="TextBoxState.Selected"]/*' />
        Selected = 3,
        /// <include file='doc\VisualStyleStates.uex' path='docs/doc[@for="TextBoxState.Disabled"]/*' />
        Disabled = 4,
        //Focused = 5, 
        /// <include file='doc\VisualStyleStates.uex' path='docs/doc[@for="TextBoxState.Readonly"]/*' />
        Readonly = 6,
        /// <include file='doc\VisualStyleStates.uex' path='docs/doc[@for="TextBoxState.Assist"]/*' />
        Assist = 7
    }

    /// <include file='doc\VisualStyleStates.uex' path='docs/doc[@for="WindowCloseButtonState"]/*' />
    [
        SuppressMessage("Microsoft.Design", "CA1008:EnumsShouldHaveZeroValue")
    ]
    public enum ToolBarState {
        /// <include file='doc\VisualStyleStates.uex' path='docs/doc[@for="ToolbarState.Normal"]/*' />
        Normal = 1,
        /// <include file='doc\VisualStyleStates.uex' path='docs/doc[@for="ToolbarState.Hot"]/*' />
        Hot = 2,
        /// <include file='doc\VisualStyleStates.uex' path='docs/doc[@for="ToolbarState.Pressed"]/*' />
        Pressed = 3,
        /// <include file='doc\VisualStyleStates.uex' path='docs/doc[@for="ToolbarState.Disabled"]/*' />
        Disabled = 4,
        /// <include file='doc\VisualStyleStates.uex' path='docs/doc[@for="ToolbarState.Checked"]/*' />
        Checked = 5,
        /// <include file='doc\VisualStyleStates.uex' path='docs/doc[@for="ToolbarState.HotChecked"]/*' />
        HotChecked = 6
    }

    /// <include file='doc\VisualStyleStates.uex' path='docs/doc[@for="TrackBarThumb]/*' />
    [
        SuppressMessage("Microsoft.Design", "CA1008:EnumsShouldHaveZeroValue")
    ]
    public enum TrackBarThumbState {
        /// <include file='doc\VisualStyleStates.uex' path='docs/doc[@for="TrackBarThumbState.Normal"]/*' />
        Normal = 1,
        /// <include file='doc\VisualStyleStates.uex' path='docs/doc[@for="TrackBarThumbState.Hot"]/*' />
        Hot = 2,
        /// <include file='doc\VisualStyleStates.uex' path='docs/doc[@for="TrackBarThumbState.Pressed"]/*' />
        Pressed = 3,
        //Focused = 4, 
        /// <include file='doc\VisualStyleStates.uex' path='docs/doc[@for="TrackBarThumbState.Disabled"]/*' />
        Disabled = 5
    }
}
