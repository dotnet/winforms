// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// This file contains the state enums used by the control renderer classes.

using System.Diagnostics.CodeAnalysis;
[assembly: SuppressMessage("Microsoft.MSInternal", "CA905:SystemAndMicrosoftNamespacesRequireApproval", Scope="namespace", Target="System.Windows.Forms.VisualStyles")]

namespace System.Windows.Forms.VisualStyles {

    [
        SuppressMessage("Microsoft.Design", "CA1008:EnumsShouldHaveZeroValue")
    ]
    public enum ComboBoxState {
        Normal = 1,
        Hot = 2,
        Pressed = 3,
        Disabled = 4
    }

    [
        SuppressMessage("Microsoft.Design", "CA1008:EnumsShouldHaveZeroValue")
    ]
    public enum CheckBoxState {
        UncheckedNormal = 1, 
        UncheckedHot = 2, 
        UncheckedPressed = 3, 
        UncheckedDisabled = 4, 
        CheckedNormal = 5, 
        CheckedHot = 6, 
        CheckedPressed = 7, 
        CheckedDisabled = 8,
        MixedNormal = 9, 
        MixedHot = 10, 
        MixedPressed = 11, 
        MixedDisabled = 12
    } 
    
    [
        SuppressMessage("Microsoft.Design", "CA1008:EnumsShouldHaveZeroValue")
    ]
    public enum GroupBoxState {
        Normal = 1, 
        Disabled = 2 
    }

    // Used by some DataGridView classes.
    [
        SuppressMessage("Microsoft.Design", "CA1008:EnumsShouldHaveZeroValue")
    ]
    internal enum HeaderItemState {
        Normal = 1,
        Hot = 2,
        Pressed = 3
    }

    [
        SuppressMessage("Microsoft.Design", "CA1008:EnumsShouldHaveZeroValue")
    ]
    public enum PushButtonState {
        Normal = 1,
        Hot = 2,
        Pressed = 3,
        Disabled = 4,
        Default = 5
    }
    
    [
        SuppressMessage("Microsoft.Design", "CA1008:EnumsShouldHaveZeroValue")
    ]
    public enum RadioButtonState {
        UncheckedNormal = 1, 
        UncheckedHot = 2, 
        UncheckedPressed = 3, 
        UncheckedDisabled = 4, 
        CheckedNormal = 5, 
        CheckedHot = 6, 
        CheckedPressed = 7, 
        CheckedDisabled = 8
    } 

    [
        SuppressMessage("Microsoft.Design", "CA1008:EnumsShouldHaveZeroValue")
    ]
    public enum ScrollBarArrowButtonState {
        UpNormal = 1,
        UpHot = 2,
        UpPressed = 3,
        UpDisabled = 4,
        DownNormal = 5,
        DownHot = 6,
        DownPressed = 7,
        DownDisabled = 8,
        LeftNormal = 9,
        LeftHot = 10,
        LeftPressed = 11,
        LeftDisabled = 12,
        RightNormal = 13,
        RightHot = 14,
        RightPressed = 15,
        RightDisabled = 16
    }


    [
        SuppressMessage("Microsoft.Design", "CA1008:EnumsShouldHaveZeroValue")
    ]
    public enum ScrollBarState {
        Normal = 1,
        Hot = 2,
        Pressed = 3,
        Disabled = 4,
        
    }

    [
        SuppressMessage("Microsoft.Design", "CA1008:EnumsShouldHaveZeroValue")
    ]
    public enum ScrollBarSizeBoxState {
        RightAlign = 1,
        LeftAlign = 2
    }

    [
        SuppressMessage("Microsoft.Design", "CA1008:EnumsShouldHaveZeroValue")
    ]
    public enum TabItemState {
        Normal = 1,
        Hot = 2,
        Selected = 3,
        Disabled = 4,
        //Focused = 5 
    }

    [
        SuppressMessage("Microsoft.Design", "CA1008:EnumsShouldHaveZeroValue"),
        SuppressMessage("Microsoft.Design", "CA1027:MarkEnumsWithFlags")
    ]
    public enum TextBoxState {
        Normal = 1,
        Hot = 2,
        Selected = 3,
        Disabled = 4,
        //Focused = 5, 
        Readonly = 6,
        Assist = 7
    }

    [
        SuppressMessage("Microsoft.Design", "CA1008:EnumsShouldHaveZeroValue")
    ]
    public enum ToolBarState {
        Normal = 1,
        Hot = 2,
        Pressed = 3,
        Disabled = 4,
        Checked = 5,
        HotChecked = 6
    }

    [
        SuppressMessage("Microsoft.Design", "CA1008:EnumsShouldHaveZeroValue")
    ]
    public enum TrackBarThumbState {
        Normal = 1,
        Hot = 2,
        Pressed = 3,
        //Focused = 4, 
        Disabled = 5
    }
}
