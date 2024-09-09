// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms.VisualStyles;

/// <summary>
///  Encapsulates the class, part and state of the "element" you wish to draw using the <see cref="VisualStyleRenderer"/>.
/// </summary>
/// <remarks>
///  <para>
///   Usage pattern is something like this:
///  </para>
///  <code>new VisualStyleRenderer(VisualStyleElement.Window.Caption.Active);</code>
/// </remarks>
public partial class VisualStyleElement
{
    internal const int Count = 25; // UPDATE THIS WHEN CLASSES ARE ADDED/REMOVED!

    private VisualStyleElement(string className, int part, int state)
    {
        ClassName = className.OrThrowIfNull();
        Part = part;
        State = state;
    }

    public static VisualStyleElement CreateElement(string className, int part, int state)
    {
        return new VisualStyleElement(className, part, state);
    }

    public string ClassName { get; }

    public int Part { get; }

    public int State { get; }

    public static class Button
    {
        private const string ClassName = "BUTTON";

        public static class PushButton
        {
            private const int Part = 1;

            private static VisualStyleElement? s_normal;

            public static VisualStyleElement Normal => s_normal ??= new VisualStyleElement(ClassName, Part, 1);

            private static VisualStyleElement? s_hot;

            public static VisualStyleElement Hot => s_hot ??= new VisualStyleElement(ClassName, Part, 2);

            private static VisualStyleElement? s_pressed;

            public static VisualStyleElement Pressed => s_pressed ??= new VisualStyleElement(ClassName, Part, 3);

            private static VisualStyleElement? s_disabled;

            public static VisualStyleElement Disabled => s_disabled ??= new VisualStyleElement(ClassName, Part, 4);

            private static VisualStyleElement? s_default;

            public static VisualStyleElement Default => s_default ??= new VisualStyleElement(ClassName, Part, 5);
        }

        public static class RadioButton
        {
            private const int Part = 2;
            // in Win10 RS3 a new part was added to BUTTONPARTS enum in vsstyle.h  - BP_RADIOBUTTON_HCDISABLED = 8
            internal const int HighContrastDisabledPart = 8;

            private static VisualStyleElement? s_uncheckednormal;

            public static VisualStyleElement UncheckedNormal => s_uncheckednormal ??= new VisualStyleElement(ClassName, Part, 1);

            private static VisualStyleElement? s_uncheckedhot;

            public static VisualStyleElement UncheckedHot => s_uncheckedhot ??= new VisualStyleElement(ClassName, Part, 2);

            private static VisualStyleElement? s_uncheckedpressed;

            public static VisualStyleElement UncheckedPressed => s_uncheckedpressed ??= new VisualStyleElement(ClassName, Part, 3);

            private static VisualStyleElement? s_uncheckeddisabled;

            public static VisualStyleElement UncheckedDisabled => s_uncheckeddisabled ??= new VisualStyleElement(ClassName, Part, 4);

            private static VisualStyleElement? s_checkednormal;

            public static VisualStyleElement CheckedNormal => s_checkednormal ??= new VisualStyleElement(ClassName, Part, 5);

            private static VisualStyleElement? s_checkedhot;

            public static VisualStyleElement CheckedHot => s_checkedhot ??= new VisualStyleElement(ClassName, Part, 6);

            private static VisualStyleElement? s_checkedpressed;

            public static VisualStyleElement CheckedPressed => s_checkedpressed ??= new VisualStyleElement(ClassName, Part, 7);

            private static VisualStyleElement? s_checkeddisabled;

            public static VisualStyleElement CheckedDisabled => s_checkeddisabled ??= new VisualStyleElement(ClassName, Part, 8);
        }

        public static class CheckBox
        {
            private const int Part = 3;
            // in Win10 RS3 a new part was added to BUTTONPARTS enum in vsstyle.h  - BP_CHECKBOX_HCDISABLED = 9
            internal const int HighContrastDisabledPart = 9;

            private static VisualStyleElement? s_uncheckednormal;

            public static VisualStyleElement UncheckedNormal => s_uncheckednormal ??= new VisualStyleElement(ClassName, Part, 1);

            private static VisualStyleElement? s_uncheckedhot;

            public static VisualStyleElement UncheckedHot => s_uncheckedhot ??= new VisualStyleElement(ClassName, Part, 2);

            private static VisualStyleElement? s_uncheckedpressed;

            public static VisualStyleElement UncheckedPressed => s_uncheckedpressed ??= new VisualStyleElement(ClassName, Part, 3);

            private static VisualStyleElement? s_uncheckeddisabled;

            public static VisualStyleElement UncheckedDisabled => s_uncheckeddisabled ??= new VisualStyleElement(ClassName, Part, 4);

            private static VisualStyleElement? s_checkednormal;

            public static VisualStyleElement CheckedNormal => s_checkednormal ??= new VisualStyleElement(ClassName, Part, 5);

            private static VisualStyleElement? s_checkedhot;

            public static VisualStyleElement CheckedHot => s_checkedhot ??= new VisualStyleElement(ClassName, Part, 6);

            private static VisualStyleElement? s_checkedpressed;

            public static VisualStyleElement CheckedPressed => s_checkedpressed ??= new VisualStyleElement(ClassName, Part, 7);

            private static VisualStyleElement? s_checkeddisabled;

            public static VisualStyleElement CheckedDisabled => s_checkeddisabled ??= new VisualStyleElement(ClassName, Part, 8);

            private static VisualStyleElement? s_mixednormal;

            public static VisualStyleElement MixedNormal => s_mixednormal ??= new VisualStyleElement(ClassName, Part, 9);

            private static VisualStyleElement? s_mixedhot;

            public static VisualStyleElement MixedHot => s_mixedhot ??= new VisualStyleElement(ClassName, Part, 10);

            private static VisualStyleElement? s_mixedpressed;

            public static VisualStyleElement MixedPressed => s_mixedpressed ??= new VisualStyleElement(ClassName, Part, 11);

            private static VisualStyleElement? s_mixeddisabled;

            public static VisualStyleElement MixedDisabled => s_mixeddisabled ??= new VisualStyleElement(ClassName, Part, 12);
        }

        public static class GroupBox
        {
            private const int Part = 4;
            // in Win10 RS3 a new part was added to BUTTONPARTS enum in vsstyle.h  - BP_GROUPBOX_HCDISABLED = 10
            internal const int HighContrastDisabledPart = 10;

            private static VisualStyleElement? s_normal;

            public static VisualStyleElement Normal => s_normal ??= new VisualStyleElement(ClassName, Part, 1);

            private static VisualStyleElement? s_disabled;

            public static VisualStyleElement Disabled => s_disabled ??= new VisualStyleElement(ClassName, Part, 2);
        }

        public static class UserButton
        {
            private const int Part = 5;

            private static VisualStyleElement? s_normal;

            public static VisualStyleElement Normal => s_normal ??= new VisualStyleElement(ClassName, Part, 0);
        }
    }

    public static class ComboBox
    {
        private const string ClassName = "COMBOBOX";

        public static class DropDownButton
        {
            private const int Part = 1;

            private static VisualStyleElement? s_normal;

            public static VisualStyleElement Normal => s_normal ??= new VisualStyleElement(ClassName, Part, 1);

            private static VisualStyleElement? s_hot;

            public static VisualStyleElement Hot => s_hot ??= new VisualStyleElement(ClassName, Part, 2);

            private static VisualStyleElement? s_pressed;

            public static VisualStyleElement Pressed => s_pressed ??= new VisualStyleElement(ClassName, Part, 3);

            private static VisualStyleElement? s_disabled;

            public static VisualStyleElement Disabled => s_disabled ??= new VisualStyleElement(ClassName, Part, 4);
        }

        internal static class Border
        {
            // Paints a rectangle with a 1 pixel edge + round corners,
            // and fills it with a color
            private const int Part = 4;

            // possible states for various fill color:
            //  2 - semi transparent
            //  3 - Window Color (white by default)
            //  4 - disabled (light gray by default)

            private static VisualStyleElement? s_normal;

            public static VisualStyleElement Normal => s_normal ??= new VisualStyleElement(ClassName, Part, 3);
        }

        internal static class ReadOnlyButton
        {
            // Paints a button for non-editable comboboxes (DropDownStyle=DropDownList)
            private const int Part = 5;

            // possible states:
            //  1 - disabled (gray by default)
            //  2 - normal (blue by default)
            //  3 - pressed (dark blue by default)
            //  4 - flat (light gray by default)

            private static VisualStyleElement? s_normal;

            public static VisualStyleElement Normal => s_normal ??= new VisualStyleElement(ClassName, Part, 2);
        }

        internal static class DropDownButtonRight
        {
            // Paints a dropdownbutton with right angles on the left side for
            // editable comboboxes (DropDownStyle=DropDown) for RTL=false case
            private const int Part = 6;

            // possible states:
            //  1 - normal (transparent background with just normal arrow)
            //  2 - hot (arrow with blue background by default)
            //  3 - pressed (arrow with dark blue background by default)
            //  4 - disabled (transparent background with just light arrow)

            private static VisualStyleElement? s_normal;

            public static VisualStyleElement Normal => s_normal ??= new VisualStyleElement(ClassName, Part, 1);
        }

        internal static class DropDownButtonLeft
        {
            // Paints a dropdownbutton with right angles on the right side for
            // editable comboboxes (DropDownStyle=DropDown) for RTL=true case
            private const int Part = 7;

            // possible states:
            //  1 - transparent normal (just normal arrow)
            //  2 - normal (blue by default)
            //  3 - pressed (dark blue by default)
            //  4 - transparent disabled (just light arrow)

            private static VisualStyleElement? s_normal;

            public static VisualStyleElement Normal => s_normal ??= new VisualStyleElement(ClassName, Part, 2);
        }
    }

    public static class Page
    {
        private const string ClassName = "PAGE";

        public static class Up
        {
            private const int Part = 1;

            private static VisualStyleElement? s_normal;

            public static VisualStyleElement Normal => s_normal ??= new VisualStyleElement(ClassName, Part, 1);

            private static VisualStyleElement? s_hot;

            public static VisualStyleElement Hot => s_hot ??= new VisualStyleElement(ClassName, Part, 2);

            private static VisualStyleElement? s_pressed;

            public static VisualStyleElement Pressed => s_pressed ??= new VisualStyleElement(ClassName, Part, 3);

            private static VisualStyleElement? s_disabled;

            public static VisualStyleElement Disabled => s_disabled ??= new VisualStyleElement(ClassName, Part, 4);
        }

        public static class Down
        {
            private const int Part = 2;

            private static VisualStyleElement? s_normal;

            public static VisualStyleElement Normal => s_normal ??= new VisualStyleElement(ClassName, Part, 1);

            private static VisualStyleElement? s_hot;

            public static VisualStyleElement Hot => s_hot ??= new VisualStyleElement(ClassName, Part, 2);

            private static VisualStyleElement? s_pressed;

            public static VisualStyleElement Pressed => s_pressed ??= new VisualStyleElement(ClassName, Part, 3);

            private static VisualStyleElement? s_disabled;

            public static VisualStyleElement Disabled => s_disabled ??= new VisualStyleElement(ClassName, Part, 4);
        }

        public static class UpHorizontal
        {
            private const int Part = 3;

            private static VisualStyleElement? s_normal;

            public static VisualStyleElement Normal => s_normal ??= new VisualStyleElement(ClassName, Part, 1);

            private static VisualStyleElement? s_hot;

            public static VisualStyleElement Hot => s_hot ??= new VisualStyleElement(ClassName, Part, 2);

            private static VisualStyleElement? s_pressed;

            public static VisualStyleElement Pressed => s_pressed ??= new VisualStyleElement(ClassName, Part, 3);

            private static VisualStyleElement? s_disabled;

            public static VisualStyleElement Disabled => s_disabled ??= new VisualStyleElement(ClassName, Part, 4);
        }

        public static class DownHorizontal
        {
            private const int Part = 4;

            private static VisualStyleElement? s_normal;

            public static VisualStyleElement Normal => s_normal ??= new VisualStyleElement(ClassName, Part, 1);

            private static VisualStyleElement? s_hot;

            public static VisualStyleElement Hot => s_hot ??= new VisualStyleElement(ClassName, Part, 2);

            private static VisualStyleElement? s_pressed;

            public static VisualStyleElement Pressed => s_pressed ??= new VisualStyleElement(ClassName, Part, 3);

            private static VisualStyleElement? s_disabled;

            public static VisualStyleElement Disabled => s_disabled ??= new VisualStyleElement(ClassName, Part, 4);
        }
    }

    public static class Spin
    {
        private const string ClassName = "SPIN";

        public static class Up
        {
            private const int Part = 1;

            private static VisualStyleElement? s_normal;

            public static VisualStyleElement Normal => s_normal ??= new VisualStyleElement(ClassName, Part, 1);

            private static VisualStyleElement? s_hot;

            public static VisualStyleElement Hot => s_hot ??= new VisualStyleElement(ClassName, Part, 2);

            private static VisualStyleElement? s_pressed;

            public static VisualStyleElement Pressed => s_pressed ??= new VisualStyleElement(ClassName, Part, 3);

            private static VisualStyleElement? s_disabled;

            public static VisualStyleElement Disabled => s_disabled ??= new VisualStyleElement(ClassName, Part, 4);
        }

        public static class Down
        {
            private const int Part = 2;

            private static VisualStyleElement? s_normal;

            public static VisualStyleElement Normal => s_normal ??= new VisualStyleElement(ClassName, Part, 1);

            private static VisualStyleElement? s_hot;

            public static VisualStyleElement Hot => s_hot ??= new VisualStyleElement(ClassName, Part, 2);

            private static VisualStyleElement? s_pressed;

            public static VisualStyleElement Pressed => s_pressed ??= new VisualStyleElement(ClassName, Part, 3);

            private static VisualStyleElement? s_disabled;

            public static VisualStyleElement Disabled => s_disabled ??= new VisualStyleElement(ClassName, Part, 4);
        }

        public static class UpHorizontal
        {
            private const int Part = 3;

            private static VisualStyleElement? s_normal;

            public static VisualStyleElement Normal => s_normal ??= new VisualStyleElement(ClassName, Part, 1);

            private static VisualStyleElement? s_hot;

            public static VisualStyleElement Hot => s_hot ??= new VisualStyleElement(ClassName, Part, 2);

            private static VisualStyleElement? s_pressed;

            public static VisualStyleElement Pressed => s_pressed ??= new VisualStyleElement(ClassName, Part, 3);

            private static VisualStyleElement? s_disabled;

            public static VisualStyleElement Disabled => s_disabled ??= new VisualStyleElement(ClassName, Part, 4);
        }

        public static class DownHorizontal
        {
            private const int Part = 4;

            private static VisualStyleElement? s_normal;

            public static VisualStyleElement Normal => s_normal ??= new VisualStyleElement(ClassName, Part, 1);

            private static VisualStyleElement? s_hot;

            public static VisualStyleElement Hot => s_hot ??= new VisualStyleElement(ClassName, Part, 2);

            private static VisualStyleElement? s_pressed;

            public static VisualStyleElement Pressed => s_pressed ??= new VisualStyleElement(ClassName, Part, 3);

            private static VisualStyleElement? s_disabled;

            public static VisualStyleElement Disabled => s_disabled ??= new VisualStyleElement(ClassName, Part, 4);
        }
    }

    public static class ScrollBar
    {
        private const string ClassName = "SCROLLBAR";

        public static class ArrowButton
        {
            private const int Part = 1;

            private static VisualStyleElement? s_upnormal;

            public static VisualStyleElement UpNormal => s_upnormal ??= new VisualStyleElement(ClassName, Part, 1);

            private static VisualStyleElement? s_uphot;

            public static VisualStyleElement UpHot => s_uphot ??= new VisualStyleElement(ClassName, Part, 2);

            private static VisualStyleElement? s_uppressed;

            public static VisualStyleElement UpPressed => s_uppressed ??= new VisualStyleElement(ClassName, Part, 3);

            private static VisualStyleElement? s_updisabled;

            public static VisualStyleElement UpDisabled => s_updisabled ??= new VisualStyleElement(ClassName, Part, 4);

            private static VisualStyleElement? s_downnormal;

            public static VisualStyleElement DownNormal => s_downnormal ??= new VisualStyleElement(ClassName, Part, 5);

            private static VisualStyleElement? s_downhot;

            public static VisualStyleElement DownHot => s_downhot ??= new VisualStyleElement(ClassName, Part, 6);

            private static VisualStyleElement? s_downpressed;

            public static VisualStyleElement DownPressed => s_downpressed ??= new VisualStyleElement(ClassName, Part, 7);

            private static VisualStyleElement? s_downdisabled;

            public static VisualStyleElement DownDisabled => s_downdisabled ??= new VisualStyleElement(ClassName, Part, 8);

            private static VisualStyleElement? s_leftnormal;

            public static VisualStyleElement LeftNormal => s_leftnormal ??= new VisualStyleElement(ClassName, Part, 9);

            private static VisualStyleElement? s_lefthot;

            public static VisualStyleElement LeftHot => s_lefthot ??= new VisualStyleElement(ClassName, Part, 10);

            private static VisualStyleElement? s_leftpressed;

            public static VisualStyleElement LeftPressed => s_leftpressed ??= new VisualStyleElement(ClassName, Part, 11);

            private static VisualStyleElement? s_leftdisabled;

            public static VisualStyleElement LeftDisabled => s_leftdisabled ??= new VisualStyleElement(ClassName, Part, 12);

            private static VisualStyleElement? s_rightnormal;

            public static VisualStyleElement RightNormal => s_rightnormal ??= new VisualStyleElement(ClassName, Part, 13);

            private static VisualStyleElement? s_righthot;

            public static VisualStyleElement RightHot => s_righthot ??= new VisualStyleElement(ClassName, Part, 14);

            private static VisualStyleElement? s_rightpressed;

            public static VisualStyleElement RightPressed => s_rightpressed ??= new VisualStyleElement(ClassName, Part, 15);

            private static VisualStyleElement? s_rightdisabled;

            public static VisualStyleElement RightDisabled => s_rightdisabled ??= new VisualStyleElement(ClassName, Part, 16);
        }

        public static class ThumbButtonHorizontal
        {
            private const int Part = 2;

            private static VisualStyleElement? s_normal;

            public static VisualStyleElement Normal => s_normal ??= new VisualStyleElement(ClassName, Part, 1);

            private static VisualStyleElement? s_hot;

            public static VisualStyleElement Hot => s_hot ??= new VisualStyleElement(ClassName, Part, 2);

            private static VisualStyleElement? s_pressed;

            public static VisualStyleElement Pressed => s_pressed ??= new VisualStyleElement(ClassName, Part, 3);

            private static VisualStyleElement? s_disabled;

            public static VisualStyleElement Disabled => s_disabled ??= new VisualStyleElement(ClassName, Part, 4);
        }

        public static class ThumbButtonVertical
        {
            private const int Part = 3;

            private static VisualStyleElement? s_normal;

            public static VisualStyleElement Normal => s_normal ??= new VisualStyleElement(ClassName, Part, 1);

            private static VisualStyleElement? s_hot;

            public static VisualStyleElement Hot => s_hot ??= new VisualStyleElement(ClassName, Part, 2);

            private static VisualStyleElement? s_pressed;

            public static VisualStyleElement Pressed => s_pressed ??= new VisualStyleElement(ClassName, Part, 3);

            private static VisualStyleElement? s_disabled;

            public static VisualStyleElement Disabled => s_disabled ??= new VisualStyleElement(ClassName, Part, 4);
        }

        public static class RightTrackHorizontal
        {
            private const int Part = 4;

            private static VisualStyleElement? s_normal;

            public static VisualStyleElement Normal => s_normal ??= new VisualStyleElement(ClassName, Part, 1);

            private static VisualStyleElement? s_hot;

            public static VisualStyleElement Hot => s_hot ??= new VisualStyleElement(ClassName, Part, 2);

            private static VisualStyleElement? s_pressed;

            public static VisualStyleElement Pressed => s_pressed ??= new VisualStyleElement(ClassName, Part, 3);

            private static VisualStyleElement? s_disabled;

            public static VisualStyleElement Disabled => s_disabled ??= new VisualStyleElement(ClassName, Part, 4);
        }

        public static class LeftTrackHorizontal
        {
            private const int Part = 5;

            private static VisualStyleElement? s_normal;

            public static VisualStyleElement Normal => s_normal ??= new VisualStyleElement(ClassName, Part, 1);

            private static VisualStyleElement? s_hot;

            public static VisualStyleElement Hot => s_hot ??= new VisualStyleElement(ClassName, Part, 2);

            private static VisualStyleElement? s_pressed;

            public static VisualStyleElement Pressed => s_pressed ??= new VisualStyleElement(ClassName, Part, 3);

            private static VisualStyleElement? s_disabled;

            public static VisualStyleElement Disabled => s_disabled ??= new VisualStyleElement(ClassName, Part, 4);
        }

        public static class LowerTrackVertical
        {
            private const int Part = 6;

            private static VisualStyleElement? s_normal;

            public static VisualStyleElement Normal => s_normal ??= new VisualStyleElement(ClassName, Part, 1);

            private static VisualStyleElement? s_hot;

            public static VisualStyleElement Hot => s_hot ??= new VisualStyleElement(ClassName, Part, 2);

            private static VisualStyleElement? s_pressed;

            public static VisualStyleElement Pressed => s_pressed ??= new VisualStyleElement(ClassName, Part, 3);

            private static VisualStyleElement? s_disabled;

            public static VisualStyleElement Disabled => s_disabled ??= new VisualStyleElement(ClassName, Part, 4);
        }

        public static class UpperTrackVertical
        {
            private const int Part = 7;

            private static VisualStyleElement? s_normal;

            public static VisualStyleElement Normal => s_normal ??= new VisualStyleElement(ClassName, Part, 1);

            private static VisualStyleElement? s_hot;

            public static VisualStyleElement Hot => s_hot ??= new VisualStyleElement(ClassName, Part, 2);

            private static VisualStyleElement? s_pressed;

            public static VisualStyleElement Pressed => s_pressed ??= new VisualStyleElement(ClassName, Part, 3);

            private static VisualStyleElement? s_disabled;

            public static VisualStyleElement Disabled => s_disabled ??= new VisualStyleElement(ClassName, Part, 4);
        }

        public static class GripperHorizontal
        {
            private const int Part = 8;

            private static VisualStyleElement? s_normal;

            public static VisualStyleElement Normal => s_normal ??= new VisualStyleElement(ClassName, Part, 0);
        }

        public static class GripperVertical
        {
            private const int Part = 9;

            private static VisualStyleElement? s_normal;

            public static VisualStyleElement Normal => s_normal ??= new VisualStyleElement(ClassName, Part, 0);
        }

        public static class SizeBox
        {
            private const int Part = 10;

            private static VisualStyleElement? s_rightalign;

            public static VisualStyleElement RightAlign => s_rightalign ??= new VisualStyleElement(ClassName, Part, 1);

            private static VisualStyleElement? s_leftalign;

            public static VisualStyleElement LeftAlign => s_leftalign ??= new VisualStyleElement(ClassName, Part, 2);
        }
    }

    public static class Tab
    {
        private const string ClassName = "TAB";

        public static class TabItem
        {
            private const int Part = 1;

            private static VisualStyleElement? s_normal;

            public static VisualStyleElement Normal => s_normal ??= new VisualStyleElement(ClassName, Part, 1);

            private static VisualStyleElement? s_hot;

            public static VisualStyleElement Hot => s_hot ??= new VisualStyleElement(ClassName, Part, 2);

            private static VisualStyleElement? s_pressed;

            public static VisualStyleElement Pressed => s_pressed ??= new VisualStyleElement(ClassName, Part, 3);

            private static VisualStyleElement? s_disabled;

            public static VisualStyleElement Disabled => s_disabled ??= new VisualStyleElement(ClassName, Part, 4);
        }

        public static class TabItemLeftEdge
        {
            private const int Part = 2;

            private static VisualStyleElement? s_normal;

            public static VisualStyleElement Normal => s_normal ??= new VisualStyleElement(ClassName, Part, 1);

            private static VisualStyleElement? s_hot;

            public static VisualStyleElement Hot => s_hot ??= new VisualStyleElement(ClassName, Part, 2);

            private static VisualStyleElement? s_pressed;

            public static VisualStyleElement Pressed => s_pressed ??= new VisualStyleElement(ClassName, Part, 3);

            private static VisualStyleElement? s_disabled;

            public static VisualStyleElement Disabled => s_disabled ??= new VisualStyleElement(ClassName, Part, 4);
        }

        public static class TabItemRightEdge
        {
            private const int Part = 3;

            private static VisualStyleElement? s_normal;

            public static VisualStyleElement Normal => s_normal ??= new VisualStyleElement(ClassName, Part, 1);

            private static VisualStyleElement? s_hot;

            public static VisualStyleElement Hot => s_hot ??= new VisualStyleElement(ClassName, Part, 2);

            private static VisualStyleElement? s_pressed;

            public static VisualStyleElement Pressed => s_pressed ??= new VisualStyleElement(ClassName, Part, 3);

            private static VisualStyleElement? s_disabled;

            public static VisualStyleElement Disabled => s_disabled ??= new VisualStyleElement(ClassName, Part, 4);
        }

        public static class TabItemBothEdges
        {
            private const int Part = 4;

            private static VisualStyleElement? s_normal;

            public static VisualStyleElement Normal => s_normal ??= new VisualStyleElement(ClassName, Part, 0);
        }

        public static class TopTabItem
        {
            private const int Part = 5;

            private static VisualStyleElement? s_normal;

            public static VisualStyleElement Normal => s_normal ??= new VisualStyleElement(ClassName, Part, 1);

            private static VisualStyleElement? s_hot;

            public static VisualStyleElement Hot => s_hot ??= new VisualStyleElement(ClassName, Part, 2);

            private static VisualStyleElement? s_pressed;

            public static VisualStyleElement Pressed => s_pressed ??= new VisualStyleElement(ClassName, Part, 3);

            private static VisualStyleElement? s_disabled;

            public static VisualStyleElement Disabled => s_disabled ??= new VisualStyleElement(ClassName, Part, 4);
        }

        public static class TopTabItemLeftEdge
        {
            private const int Part = 6;

            private static VisualStyleElement? s_normal;

            public static VisualStyleElement Normal => s_normal ??= new VisualStyleElement(ClassName, Part, 1);

            private static VisualStyleElement? s_hot;

            public static VisualStyleElement Hot => s_hot ??= new VisualStyleElement(ClassName, Part, 2);

            private static VisualStyleElement? s_pressed;

            public static VisualStyleElement Pressed => s_pressed ??= new VisualStyleElement(ClassName, Part, 3);

            private static VisualStyleElement? s_disabled;

            public static VisualStyleElement Disabled => s_disabled ??= new VisualStyleElement(ClassName, Part, 4);
        }

        public static class TopTabItemRightEdge
        {
            private const int Part = 7;

            private static VisualStyleElement? s_normal;

            public static VisualStyleElement Normal => s_normal ??= new VisualStyleElement(ClassName, Part, 1);

            private static VisualStyleElement? s_hot;

            public static VisualStyleElement Hot => s_hot ??= new VisualStyleElement(ClassName, Part, 2);

            private static VisualStyleElement? s_pressed;

            public static VisualStyleElement Pressed => s_pressed ??= new VisualStyleElement(ClassName, Part, 3);

            private static VisualStyleElement? s_disabled;

            public static VisualStyleElement Disabled => s_disabled ??= new VisualStyleElement(ClassName, Part, 4);
        }

        public static class TopTabItemBothEdges
        {
            private const int Part = 8;

            private static VisualStyleElement? s_normal;

            public static VisualStyleElement Normal => s_normal ??= new VisualStyleElement(ClassName, Part, 0);
        }

        public static class Pane
        {
            private const int Part = 9;

            private static VisualStyleElement? s_normal;

            public static VisualStyleElement Normal => s_normal ??= new VisualStyleElement(ClassName, Part, 0);
        }

        public static class Body
        {
            private const int Part = 10;

            private static VisualStyleElement? s_normal;

            public static VisualStyleElement Normal => s_normal ??= new VisualStyleElement(ClassName, Part, 0);
        }
    }

    public static class ExplorerBar
    {
        private const string ClassName = "EXPLORERBAR";

        public static class HeaderBackground
        {
            private const int Part = 1;

            private static VisualStyleElement? s_normal;

            public static VisualStyleElement Normal => s_normal ??= new VisualStyleElement(ClassName, Part, 0);
        }

        public static class HeaderClose
        {
            private const int Part = 2;

            private static VisualStyleElement? s_normal;

            public static VisualStyleElement Normal => s_normal ??= new VisualStyleElement(ClassName, Part, 1);

            private static VisualStyleElement? s_hot;

            public static VisualStyleElement Hot => s_hot ??= new VisualStyleElement(ClassName, Part, 2);

            private static VisualStyleElement? s_pressed;

            public static VisualStyleElement Pressed => s_pressed ??= new VisualStyleElement(ClassName, Part, 3);
        }

        public static class HeaderPin
        {
            private const int Part = 3;

            private static VisualStyleElement? s_normal;

            public static VisualStyleElement Normal => s_normal ??= new VisualStyleElement(ClassName, Part, 1);

            private static VisualStyleElement? s_hot;

            public static VisualStyleElement Hot => s_hot ??= new VisualStyleElement(ClassName, Part, 2);

            private static VisualStyleElement? s_pressed;

            public static VisualStyleElement Pressed => s_pressed ??= new VisualStyleElement(ClassName, Part, 3);

            private static VisualStyleElement? s_selectednormal;

            public static VisualStyleElement SelectedNormal => s_selectednormal ??= new VisualStyleElement(ClassName, Part, 4);

            private static VisualStyleElement? s_selectedhot;

            public static VisualStyleElement SelectedHot => s_selectedhot ??= new VisualStyleElement(ClassName, Part, 5);

            private static VisualStyleElement? s_selectedpressed;

            public static VisualStyleElement SelectedPressed => s_selectedpressed ??= new VisualStyleElement(ClassName, Part, 6);
        }

        public static class IEBarMenu
        {
            private const int Part = 4;

            private static VisualStyleElement? s_normal;

            public static VisualStyleElement Normal => s_normal ??= new VisualStyleElement(ClassName, Part, 1);

            private static VisualStyleElement? s_hot;

            public static VisualStyleElement Hot => s_hot ??= new VisualStyleElement(ClassName, Part, 2);

            private static VisualStyleElement? s_pressed;

            public static VisualStyleElement Pressed => s_pressed ??= new VisualStyleElement(ClassName, Part, 3);
        }

        public static class NormalGroupBackground
        {
            private const int Part = 5;

            private static VisualStyleElement? s_normal;

            public static VisualStyleElement Normal => s_normal ??= new VisualStyleElement(ClassName, Part, 0);
        }

        public static class NormalGroupCollapse
        {
            private const int Part = 6;

            private static VisualStyleElement? s_normal;

            public static VisualStyleElement Normal => s_normal ??= new VisualStyleElement(ClassName, Part, 1);

            private static VisualStyleElement? s_hot;

            public static VisualStyleElement Hot => s_hot ??= new VisualStyleElement(ClassName, Part, 2);

            private static VisualStyleElement? s_pressed;

            public static VisualStyleElement Pressed => s_pressed ??= new VisualStyleElement(ClassName, Part, 3);
        }

        public static class NormalGroupExpand
        {
            private const int Part = 7;

            private static VisualStyleElement? s_normal;

            public static VisualStyleElement Normal => s_normal ??= new VisualStyleElement(ClassName, Part, 1);

            private static VisualStyleElement? s_hot;

            public static VisualStyleElement Hot => s_hot ??= new VisualStyleElement(ClassName, Part, 2);

            private static VisualStyleElement? s_pressed;

            public static VisualStyleElement Pressed => s_pressed ??= new VisualStyleElement(ClassName, Part, 3);
        }

        public static class NormalGroupHead
        {
            private const int Part = 8;

            private static VisualStyleElement? s_normal;

            public static VisualStyleElement Normal => s_normal ??= new VisualStyleElement(ClassName, Part, 0);
        }

        public static class SpecialGroupBackground
        {
            private const int Part = 9;

            private static VisualStyleElement? s_normal;

            public static VisualStyleElement Normal => s_normal ??= new VisualStyleElement(ClassName, Part, 0);
        }

        public static class SpecialGroupCollapse
        {
            private const int Part = 10;

            private static VisualStyleElement? s_normal;

            public static VisualStyleElement Normal => s_normal ??= new VisualStyleElement(ClassName, Part, 1);

            private static VisualStyleElement? s_hot;

            public static VisualStyleElement Hot => s_hot ??= new VisualStyleElement(ClassName, Part, 2);

            private static VisualStyleElement? s_pressed;

            public static VisualStyleElement Pressed => s_pressed ??= new VisualStyleElement(ClassName, Part, 3);
        }

        public static class SpecialGroupExpand
        {
            private const int Part = 11;

            private static VisualStyleElement? s_normal;

            public static VisualStyleElement Normal => s_normal ??= new VisualStyleElement(ClassName, Part, 1);

            private static VisualStyleElement? s_hot;

            public static VisualStyleElement Hot => s_hot ??= new VisualStyleElement(ClassName, Part, 2);

            private static VisualStyleElement? s_pressed;

            public static VisualStyleElement Pressed => s_pressed ??= new VisualStyleElement(ClassName, Part, 3);
        }

        public static class SpecialGroupHead
        {
            private const int Part = 12;

            private static VisualStyleElement? s_normal;

            public static VisualStyleElement Normal => s_normal ??= new VisualStyleElement(ClassName, Part, 0);
        }
    }

    public static class Header
    {
        private const string ClassName = "HEADER";

        public static class Item
        {
            private const int Part = 1;

            private static VisualStyleElement? s_normal;

            public static VisualStyleElement Normal => s_normal ??= new VisualStyleElement(ClassName, Part, 1);

            private static VisualStyleElement? s_hot;

            public static VisualStyleElement Hot => s_hot ??= new VisualStyleElement(ClassName, Part, 2);

            private static VisualStyleElement? s_pressed;

            public static VisualStyleElement Pressed => s_pressed ??= new VisualStyleElement(ClassName, Part, 3);
        }

        public static class ItemLeft
        {
            private const int Part = 2;

            private static VisualStyleElement? s_normal;

            public static VisualStyleElement Normal => s_normal ??= new VisualStyleElement(ClassName, Part, 1);

            private static VisualStyleElement? s_hot;

            public static VisualStyleElement Hot => s_hot ??= new VisualStyleElement(ClassName, Part, 2);

            private static VisualStyleElement? s_pressed;

            public static VisualStyleElement Pressed => s_pressed ??= new VisualStyleElement(ClassName, Part, 3);
        }

        public static class ItemRight
        {
            private const int Part = 3;

            private static VisualStyleElement? s_normal;

            public static VisualStyleElement Normal => s_normal ??= new VisualStyleElement(ClassName, Part, 1);

            private static VisualStyleElement? s_hot;

            public static VisualStyleElement Hot => s_hot ??= new VisualStyleElement(ClassName, Part, 2);

            private static VisualStyleElement? s_pressed;

            public static VisualStyleElement Pressed => s_pressed ??= new VisualStyleElement(ClassName, Part, 3);
        }

        public static class SortArrow
        {
            private const int Part = 4;

            private static VisualStyleElement? s_sortedup;

            public static VisualStyleElement SortedUp => s_sortedup ??= new VisualStyleElement(ClassName, Part, 1);

            private static VisualStyleElement? s_sorteddown;

            public static VisualStyleElement SortedDown => s_sorteddown ??= new VisualStyleElement(ClassName, Part, 2);
        }
    }

    public static class ListView
    {
        private const string ClassName = "LISTVIEW";

        public static class Item
        {
            private const int Part = 1;

            private static VisualStyleElement? s_normal;

            public static VisualStyleElement Normal => s_normal ??= new VisualStyleElement(ClassName, Part, 1);

            private static VisualStyleElement? s_hot;

            public static VisualStyleElement Hot => s_hot ??= new VisualStyleElement(ClassName, Part, 2);

            private static VisualStyleElement? s_selected;

            public static VisualStyleElement Selected => s_selected ??= new VisualStyleElement(ClassName, Part, 3);

            private static VisualStyleElement? s_disabled;

            public static VisualStyleElement Disabled => s_disabled ??= new VisualStyleElement(ClassName, Part, 4);

            private static VisualStyleElement? s_selectednotfocus;

            public static VisualStyleElement SelectedNotFocus => s_selectednotfocus ??= new VisualStyleElement(ClassName, Part, 5);
        }

        public static class Group
        {
            private const int Part = 2;

            private static VisualStyleElement? s_normal;

            public static VisualStyleElement Normal => s_normal ??= new VisualStyleElement(ClassName, Part, 0);
        }

        public static class Detail
        {
            private const int Part = 3;

            private static VisualStyleElement? s_normal;

            public static VisualStyleElement Normal => s_normal ??= new VisualStyleElement(ClassName, Part, 0);
        }

        public static class SortedDetail
        {
            private const int Part = 4;

            private static VisualStyleElement? s_normal;

            public static VisualStyleElement Normal => s_normal ??= new VisualStyleElement(ClassName, Part, 0);
        }

        public static class EmptyText
        {
            private const int Part = 5;

            private static VisualStyleElement? s_normal;

            public static VisualStyleElement Normal => s_normal ??= new VisualStyleElement(ClassName, Part, 0);
        }
    }

    public static class MenuBand
    {
        private const string ClassName = "MENUBAND";

        public static class NewApplicationButton
        {
            private const int Part = 1;

            private static VisualStyleElement? s_normal;

            public static VisualStyleElement Normal => s_normal ??= new VisualStyleElement(ClassName, Part, 1);

            private static VisualStyleElement? s_hot;

            public static VisualStyleElement Hot => s_hot ??= new VisualStyleElement(ClassName, Part, 2);

            private static VisualStyleElement? s_pressed;

            public static VisualStyleElement Pressed => s_pressed ??= new VisualStyleElement(ClassName, Part, 3);

            private static VisualStyleElement? s_disabled;

            public static VisualStyleElement Disabled => s_disabled ??= new VisualStyleElement(ClassName, Part, 4);

            private static VisualStyleElement? s_checked;

            public static VisualStyleElement Checked => s_checked ??= new VisualStyleElement(ClassName, Part, 5);

            private static VisualStyleElement? s_hotchecked;

            public static VisualStyleElement HotChecked => s_hotchecked ??= new VisualStyleElement(ClassName, Part, 6);
        }

        public static class Separator
        {
            private const int Part = 2;

            private static VisualStyleElement? s_normal;

            public static VisualStyleElement Normal => s_normal ??= new VisualStyleElement(ClassName, Part, 0);
        }
    }

    public static class Menu
    {
        private const string ClassName = "MENU";

        public static class Item
        {
            private const int Part = 1;

            private static VisualStyleElement? s_normal;

            public static VisualStyleElement Normal => s_normal ??= new VisualStyleElement(ClassName, Part, 1);

            private static VisualStyleElement? s_selected;

            public static VisualStyleElement Selected => s_selected ??= new VisualStyleElement(ClassName, Part, 2);

            private static VisualStyleElement? s_demoted;

            public static VisualStyleElement Demoted => s_demoted ??= new VisualStyleElement(ClassName, Part, 3);
        }

        public static class DropDown
        {
            private const int Part = 2;

            private static VisualStyleElement? s_normal;

            public static VisualStyleElement Normal => s_normal ??= new VisualStyleElement(ClassName, Part, 0);
        }

        public static class BarItem
        {
            private const int Part = 3;

            private static VisualStyleElement? s_normal;

            public static VisualStyleElement Normal => s_normal ??= new VisualStyleElement(ClassName, Part, 0);
        }

        public static class BarDropDown
        {
            private const int Part = 4;

            private static VisualStyleElement? s_normal;

            public static VisualStyleElement Normal => s_normal ??= new VisualStyleElement(ClassName, Part, 0);
        }

        public static class Chevron
        {
            private const int Part = 5;

            private static VisualStyleElement? s_normal;

            public static VisualStyleElement Normal => s_normal ??= new VisualStyleElement(ClassName, Part, 0);
        }

        public static class Separator
        {
            private const int Part = 6;

            private static VisualStyleElement? s_normal;

            public static VisualStyleElement Normal => s_normal ??= new VisualStyleElement(ClassName, Part, 0);
        }
    }

    public static class ProgressBar
    {
        private const string ClassName = "PROGRESS";

        public static class Bar
        {
            private const int Part = 1;

            private static VisualStyleElement? s_normal;

            public static VisualStyleElement Normal => s_normal ??= new VisualStyleElement(ClassName, Part, 0);
        }

        public static class BarVertical
        {
            private const int Part = 2;

            private static VisualStyleElement? s_normal;

            public static VisualStyleElement Normal => s_normal ??= new VisualStyleElement(ClassName, Part, 0);
        }

        public static class Chunk
        {
            private const int Part = 3;

            private static VisualStyleElement? s_normal;

            public static VisualStyleElement Normal => s_normal ??= new VisualStyleElement(ClassName, Part, 0);
        }

        public static class ChunkVertical
        {
            private const int Part = 4;

            private static VisualStyleElement? s_normal;

            public static VisualStyleElement Normal => s_normal ??= new VisualStyleElement(ClassName, Part, 0);
        }
    }

    public static class Rebar
    {
        private const string ClassName = "REBAR";

        public static class Gripper
        {
            private const int Part = 1;

            private static VisualStyleElement? s_normal;

            public static VisualStyleElement Normal => s_normal ??= new VisualStyleElement(ClassName, Part, 0);
        }

        public static class GripperVertical
        {
            private const int Part = 2;

            private static VisualStyleElement? s_normal;

            public static VisualStyleElement Normal => s_normal ??= new VisualStyleElement(ClassName, Part, 0);
        }

        public static class Band
        {
            private const int Part = 3;

            private static VisualStyleElement? s_normal;

            public static VisualStyleElement Normal => s_normal ??= new VisualStyleElement(ClassName, Part, 0);
        }

        public static class Chevron
        {
            private const int Part = 4;

            private static VisualStyleElement? s_normal;

            public static VisualStyleElement Normal => s_normal ??= new VisualStyleElement(ClassName, Part, 1);

            private static VisualStyleElement? s_hot;

            public static VisualStyleElement Hot => s_hot ??= new VisualStyleElement(ClassName, Part, 2);

            private static VisualStyleElement? s_pressed;

            public static VisualStyleElement Pressed => s_pressed ??= new VisualStyleElement(ClassName, Part, 3);
        }

        public static class ChevronVertical
        {
            private const int Part = 5;

            private static VisualStyleElement? s_normal;

            public static VisualStyleElement Normal => s_normal ??= new VisualStyleElement(ClassName, Part, 1);

            private static VisualStyleElement? s_hot;

            public static VisualStyleElement Hot => s_hot ??= new VisualStyleElement(ClassName, Part, 2);

            private static VisualStyleElement? s_pressed;

            public static VisualStyleElement Pressed => s_pressed ??= new VisualStyleElement(ClassName, Part, 3);
        }
    }

    public static class StartPanel
    {
        private const string ClassName = "STARTPANEL";

        public static class UserPane
        {
            private const int Part = 1;

            private static VisualStyleElement? s_normal;

            public static VisualStyleElement Normal => s_normal ??= new VisualStyleElement(ClassName, Part, 0);
        }

        public static class MorePrograms
        {
            private const int Part = 2;

            private static VisualStyleElement? s_normal;

            public static VisualStyleElement Normal => s_normal ??= new VisualStyleElement(ClassName, Part, 0);
        }

        public static class MoreProgramsArrow
        {
            private const int Part = 3;

            private static VisualStyleElement? s_normal;

            public static VisualStyleElement Normal => s_normal ??= new VisualStyleElement(ClassName, Part, 1);

            private static VisualStyleElement? s_hot;

            public static VisualStyleElement Hot => s_hot ??= new VisualStyleElement(ClassName, Part, 2);

            private static VisualStyleElement? s_pressed;

            public static VisualStyleElement Pressed => s_pressed ??= new VisualStyleElement(ClassName, Part, 3);
        }

        public static class ProgList
        {
            private const int Part = 4;

            private static VisualStyleElement? s_normal;

            public static VisualStyleElement Normal => s_normal ??= new VisualStyleElement(ClassName, Part, 0);
        }

        public static class ProgListSeparator
        {
            private const int Part = 5;

            private static VisualStyleElement? s_normal;

            public static VisualStyleElement Normal => s_normal ??= new VisualStyleElement(ClassName, Part, 0);
        }

        public static class PlaceList
        {
            private const int Part = 6;

            private static VisualStyleElement? s_normal;

            public static VisualStyleElement Normal => s_normal ??= new VisualStyleElement(ClassName, Part, 0);
        }

        public static class PlaceListSeparator
        {
            private const int Part = 7;

            private static VisualStyleElement? s_normal;

            public static VisualStyleElement Normal => s_normal ??= new VisualStyleElement(ClassName, Part, 0);
        }

        // The verb, not the noun. Matches "Log Off" button.
        public static class LogOff
        {
            private const int Part = 8;

            private static VisualStyleElement? s_normal;

            public static VisualStyleElement Normal => s_normal ??= new VisualStyleElement(ClassName, Part, 0);
        }

        // The verb, not the noun. Matches "Log Off" button.
        public static class LogOffButtons
        {
            private const int Part = 9;

            private static VisualStyleElement? s_normal;

            public static VisualStyleElement Normal => s_normal ??= new VisualStyleElement(ClassName, Part, 1);

            private static VisualStyleElement? s_hot;

            public static VisualStyleElement Hot => s_hot ??= new VisualStyleElement(ClassName, Part, 2);

            private static VisualStyleElement? s_pressed;

            public static VisualStyleElement Pressed => s_pressed ??= new VisualStyleElement(ClassName, Part, 3);
        }

        public static class UserPicture
        {
            private const int Part = 10;

            private static VisualStyleElement? s_normal;

            public static VisualStyleElement Normal => s_normal ??= new VisualStyleElement(ClassName, Part, 0);
        }

        public static class Preview
        {
            private const int Part = 11;

            private static VisualStyleElement? s_normal;

            public static VisualStyleElement Normal => s_normal ??= new VisualStyleElement(ClassName, Part, 0);
        }
    }

    public static class Status
    {
        private const string ClassName = "STATUS";

        public static class Bar
        {
            private static VisualStyleElement? s_normal;

            public static VisualStyleElement Normal => s_normal ??= new VisualStyleElement(ClassName, 0, 0);
        }

        public static class Pane
        {
            private const int Part = 1;

            private static VisualStyleElement? s_normal;

            public static VisualStyleElement Normal => s_normal ??= new VisualStyleElement(ClassName, Part, 0);
        }

        public static class GripperPane
        {
            private const int Part = 2;

            private static VisualStyleElement? s_normal;

            public static VisualStyleElement Normal => s_normal ??= new VisualStyleElement(ClassName, Part, 0);
        }

        public static class Gripper
        {
            private const int Part = 3;

            private static VisualStyleElement? s_normal;

            public static VisualStyleElement Normal => s_normal ??= new VisualStyleElement(ClassName, Part, 0);
        }
    }

    public static class TaskBand
    {
        private const string ClassName = "TASKBAND";

        public static class GroupCount
        {
            private const int Part = 1;

            private static VisualStyleElement? s_normal;

            public static VisualStyleElement Normal => s_normal ??= new VisualStyleElement(ClassName, Part, 0);
        }

        public static class FlashButton
        {
            private const int Part = 2;

            private static VisualStyleElement? s_normal;

            public static VisualStyleElement Normal => s_normal ??= new VisualStyleElement(ClassName, Part, 0);
        }

        public static class FlashButtonGroupMenu
        {
            private const int Part = 3;

            private static VisualStyleElement? s_normal;

            public static VisualStyleElement Normal => s_normal ??= new VisualStyleElement(ClassName, Part, 0);
        }
    }

    public static class TaskbarClock
    {
        private const string ClassName = "CLOCK";

        public static class Time
        {
            private const int Part = 1;

            private static VisualStyleElement? s_normal;

            public static VisualStyleElement Normal => s_normal ??= new VisualStyleElement(ClassName, Part, 1);
        }
    }

    public static class Taskbar
    {
        private const string ClassName = "TASKBAR";

        public static class BackgroundBottom
        {
            private const int Part = 1;

            private static VisualStyleElement? s_normal;

            public static VisualStyleElement Normal => s_normal ??= new VisualStyleElement(ClassName, Part, 0);
        }

        public static class BackgroundRight
        {
            private const int Part = 2;

            private static VisualStyleElement? s_normal;

            public static VisualStyleElement Normal => s_normal ??= new VisualStyleElement(ClassName, Part, 0);
        }

        public static class BackgroundTop
        {
            private const int Part = 3;

            private static VisualStyleElement? s_normal;

            public static VisualStyleElement Normal => s_normal ??= new VisualStyleElement(ClassName, Part, 0);
        }

        public static class BackgroundLeft
        {
            private const int Part = 4;

            private static VisualStyleElement? s_normal;

            public static VisualStyleElement Normal => s_normal ??= new VisualStyleElement(ClassName, Part, 0);
        }

        public static class SizingBarBottom
        {
            private const int Part = 5;

            private static VisualStyleElement? s_normal;

            public static VisualStyleElement Normal => s_normal ??= new VisualStyleElement(ClassName, Part, 0);
        }

        public static class SizingBarRight
        {
            private const int Part = 6;

            private static VisualStyleElement? s_normal;

            public static VisualStyleElement Normal => s_normal ??= new VisualStyleElement(ClassName, Part, 0);
        }

        public static class SizingBarTop
        {
            private const int Part = 7;

            private static VisualStyleElement? s_normal;

            public static VisualStyleElement Normal => s_normal ??= new VisualStyleElement(ClassName, Part, 0);
        }

        public static class SizingBarLeft
        {
            private const int Part = 8;

            private static VisualStyleElement? s_normal;

            public static VisualStyleElement Normal => s_normal ??= new VisualStyleElement(ClassName, Part, 0);
        }
    }

    public static class ToolBar
    {
        private const string ClassName = "TOOLBAR";

        internal static class Bar
        {
            private static VisualStyleElement? s_normal;

            public static VisualStyleElement Normal => s_normal ??= new VisualStyleElement(ClassName, 0, 0);
        }

        public static class Button
        {
            private const int Part = 1;

            private static VisualStyleElement? s_normal;

            public static VisualStyleElement Normal => s_normal ??= new VisualStyleElement(ClassName, Part, 1);

            private static VisualStyleElement? s_hot;

            public static VisualStyleElement Hot => s_hot ??= new VisualStyleElement(ClassName, Part, 2);

            private static VisualStyleElement? s_pressed;

            public static VisualStyleElement Pressed => s_pressed ??= new VisualStyleElement(ClassName, Part, 3);

            private static VisualStyleElement? s_disabled;

            public static VisualStyleElement Disabled => s_disabled ??= new VisualStyleElement(ClassName, Part, 4);

            private static VisualStyleElement? s_checked;

            public static VisualStyleElement Checked => s_checked ??= new VisualStyleElement(ClassName, Part, 5);

            private static VisualStyleElement? s_hotchecked;

            public static VisualStyleElement HotChecked => s_hotchecked ??= new VisualStyleElement(ClassName, Part, 6);
        }

        public static class DropDownButton
        {
            private const int Part = 2;

            private static VisualStyleElement? s_normal;

            public static VisualStyleElement Normal => s_normal ??= new VisualStyleElement(ClassName, Part, 1);

            private static VisualStyleElement? s_hot;

            public static VisualStyleElement Hot => s_hot ??= new VisualStyleElement(ClassName, Part, 2);

            private static VisualStyleElement? s_pressed;

            public static VisualStyleElement Pressed => s_pressed ??= new VisualStyleElement(ClassName, Part, 3);

            private static VisualStyleElement? s_disabled;

            public static VisualStyleElement Disabled => s_disabled ??= new VisualStyleElement(ClassName, Part, 4);

            private static VisualStyleElement? s_checked;

            public static VisualStyleElement Checked => s_checked ??= new VisualStyleElement(ClassName, Part, 5);

            private static VisualStyleElement? s_hotchecked;

            public static VisualStyleElement HotChecked => s_hotchecked ??= new VisualStyleElement(ClassName, Part, 6);
        }

        public static class SplitButton
        {
            private const int Part = 3;

            private static VisualStyleElement? s_normal;

            public static VisualStyleElement Normal => s_normal ??= new VisualStyleElement(ClassName, Part, 1);

            private static VisualStyleElement? s_hot;

            public static VisualStyleElement Hot => s_hot ??= new VisualStyleElement(ClassName, Part, 2);

            private static VisualStyleElement? s_pressed;

            public static VisualStyleElement Pressed => s_pressed ??= new VisualStyleElement(ClassName, Part, 3);

            private static VisualStyleElement? s_disabled;

            public static VisualStyleElement Disabled => s_disabled ??= new VisualStyleElement(ClassName, Part, 4);

            private static VisualStyleElement? s_checked;

            public static VisualStyleElement Checked => s_checked ??= new VisualStyleElement(ClassName, Part, 5);

            private static VisualStyleElement? s_hotchecked;

            public static VisualStyleElement HotChecked => s_hotchecked ??= new VisualStyleElement(ClassName, Part, 6);
        }

        public static class SplitButtonDropDown
        {
            private const int Part = 4;

            private static VisualStyleElement? s_normal;

            public static VisualStyleElement Normal => s_normal ??= new VisualStyleElement(ClassName, Part, 1);

            private static VisualStyleElement? s_hot;

            public static VisualStyleElement Hot => s_hot ??= new VisualStyleElement(ClassName, Part, 2);

            private static VisualStyleElement? s_pressed;

            public static VisualStyleElement Pressed => s_pressed ??= new VisualStyleElement(ClassName, Part, 3);

            private static VisualStyleElement? s_disabled;

            public static VisualStyleElement Disabled => s_disabled ??= new VisualStyleElement(ClassName, Part, 4);

            private static VisualStyleElement? s_checked;

            public static VisualStyleElement Checked => s_checked ??= new VisualStyleElement(ClassName, Part, 5);

            private static VisualStyleElement? s_hotchecked;

            public static VisualStyleElement HotChecked => s_hotchecked ??= new VisualStyleElement(ClassName, Part, 6);
        }

        public static class SeparatorHorizontal
        {
            private const int Part = 5;

            private static VisualStyleElement? s_normal;

            public static VisualStyleElement Normal => s_normal ??= new VisualStyleElement(ClassName, Part, 0);
        }

        public static class SeparatorVertical
        {
            private const int Part = 6;

            private static VisualStyleElement? s_normal;

            public static VisualStyleElement Normal => s_normal ??= new VisualStyleElement(ClassName, Part, 0);
        }
    }

    public static class ToolTip
    {
        private const string ClassName = "TOOLTIP";

        public static class Standard
        {
            private const int Part = 1;

            private static VisualStyleElement? s_normal;

            public static VisualStyleElement Normal => s_normal ??= new VisualStyleElement(ClassName, Part, 1);

            private static VisualStyleElement? s_link;

            public static VisualStyleElement Link => s_link ??= new VisualStyleElement(ClassName, Part, 2);
        }

        public static class StandardTitle
        {
            private const int Part = 2;

            private static VisualStyleElement? s_normal;

            public static VisualStyleElement Normal => s_normal ??= new VisualStyleElement(ClassName, Part, 0);
        }

        public static class Balloon
        {
            private const int Part = 3;

            private static VisualStyleElement? s_normal;

            public static VisualStyleElement Normal => s_normal ??= new VisualStyleElement(ClassName, Part, 1);

            private static VisualStyleElement? s_link;

            public static VisualStyleElement Link => s_link ??= new VisualStyleElement(ClassName, Part, 2);
        }

        public static class BalloonTitle
        {
            private const int Part = 4;

            private static VisualStyleElement? s_normal;

            public static VisualStyleElement Normal => s_normal ??= new VisualStyleElement(ClassName, Part, 0);
        }

        public static class Close
        {
            private const int Part = 5;

            private static VisualStyleElement? s_normal;

            public static VisualStyleElement Normal => s_normal ??= new VisualStyleElement(ClassName, Part, 1);

            private static VisualStyleElement? s_hot;

            public static VisualStyleElement Hot => s_hot ??= new VisualStyleElement(ClassName, Part, 2);

            private static VisualStyleElement? s_pressed;

            public static VisualStyleElement Pressed => s_pressed ??= new VisualStyleElement(ClassName, Part, 3);
        }
    }

    public static class TrackBar
    {
        private const string ClassName = "TRACKBAR";

        public static class Track
        {
            private const int Part = 1;

            private static VisualStyleElement? s_normal;

            public static VisualStyleElement Normal => s_normal ??= new VisualStyleElement(ClassName, Part, 1);
        }

        public static class TrackVertical
        {
            private const int Part = 2;

            private static VisualStyleElement? s_normal;

            public static VisualStyleElement Normal => s_normal ??= new VisualStyleElement(ClassName, Part, 1);
        }

        public static class Thumb
        {
            private const int Part = 3;

            private static VisualStyleElement? s_normal;

            public static VisualStyleElement Normal => s_normal ??= new VisualStyleElement(ClassName, Part, 1);

            private static VisualStyleElement? s_hot;

            public static VisualStyleElement Hot => s_hot ??= new VisualStyleElement(ClassName, Part, 2);

            private static VisualStyleElement? s_pressed;

            public static VisualStyleElement Pressed => s_pressed ??= new VisualStyleElement(ClassName, Part, 3);

            private static VisualStyleElement? s_focused;

            public static VisualStyleElement Focused => s_focused ??= new VisualStyleElement(ClassName, Part, 4);

            private static VisualStyleElement? s_disabled;

            public static VisualStyleElement Disabled => s_disabled ??= new VisualStyleElement(ClassName, Part, 5);
        }

        public static class ThumbBottom
        {
            private const int Part = 4;

            private static VisualStyleElement? s_normal;

            public static VisualStyleElement Normal => s_normal ??= new VisualStyleElement(ClassName, Part, 1);

            private static VisualStyleElement? s_hot;

            public static VisualStyleElement Hot => s_hot ??= new VisualStyleElement(ClassName, Part, 2);

            private static VisualStyleElement? s_pressed;

            public static VisualStyleElement Pressed => s_pressed ??= new VisualStyleElement(ClassName, Part, 3);

            private static VisualStyleElement? s_focused;

            public static VisualStyleElement Focused => s_focused ??= new VisualStyleElement(ClassName, Part, 4);

            private static VisualStyleElement? s_disabled;

            public static VisualStyleElement Disabled => s_disabled ??= new VisualStyleElement(ClassName, Part, 5);
        }

        public static class ThumbTop
        {
            private const int Part = 5;

            private static VisualStyleElement? s_normal;

            public static VisualStyleElement Normal => s_normal ??= new VisualStyleElement(ClassName, Part, 1);

            private static VisualStyleElement? s_hot;

            public static VisualStyleElement Hot => s_hot ??= new VisualStyleElement(ClassName, Part, 2);

            private static VisualStyleElement? s_pressed;

            public static VisualStyleElement Pressed => s_pressed ??= new VisualStyleElement(ClassName, Part, 3);

            private static VisualStyleElement? s_focused;

            public static VisualStyleElement Focused => s_focused ??= new VisualStyleElement(ClassName, Part, 4);

            private static VisualStyleElement? s_disabled;

            public static VisualStyleElement Disabled => s_disabled ??= new VisualStyleElement(ClassName, Part, 5);
        }

        public static class ThumbVertical
        {
            private const int Part = 6;

            private static VisualStyleElement? s_normal;

            public static VisualStyleElement Normal => s_normal ??= new VisualStyleElement(ClassName, Part, 1);

            private static VisualStyleElement? s_hot;

            public static VisualStyleElement Hot => s_hot ??= new VisualStyleElement(ClassName, Part, 2);

            private static VisualStyleElement? s_pressed;

            public static VisualStyleElement Pressed => s_pressed ??= new VisualStyleElement(ClassName, Part, 3);

            private static VisualStyleElement? s_focused;

            public static VisualStyleElement Focused => s_focused ??= new VisualStyleElement(ClassName, Part, 4);

            private static VisualStyleElement? s_disabled;

            public static VisualStyleElement Disabled => s_disabled ??= new VisualStyleElement(ClassName, Part, 5);
        }

        public static class ThumbLeft
        {
            private const int Part = 7;

            private static VisualStyleElement? s_normal;

            public static VisualStyleElement Normal => s_normal ??= new VisualStyleElement(ClassName, Part, 1);

            private static VisualStyleElement? s_hot;

            public static VisualStyleElement Hot => s_hot ??= new VisualStyleElement(ClassName, Part, 2);

            private static VisualStyleElement? s_pressed;

            public static VisualStyleElement Pressed => s_pressed ??= new VisualStyleElement(ClassName, Part, 3);

            private static VisualStyleElement? s_focused;

            public static VisualStyleElement Focused => s_focused ??= new VisualStyleElement(ClassName, Part, 4);

            private static VisualStyleElement? s_disabled;

            public static VisualStyleElement Disabled => s_disabled ??= new VisualStyleElement(ClassName, Part, 5);
        }

        public static class ThumbRight
        {
            private const int Part = 8;

            private static VisualStyleElement? s_normal;

            public static VisualStyleElement Normal => s_normal ??= new VisualStyleElement(ClassName, Part, 1);

            private static VisualStyleElement? s_hot;

            public static VisualStyleElement Hot => s_hot ??= new VisualStyleElement(ClassName, Part, 2);

            private static VisualStyleElement? s_pressed;

            public static VisualStyleElement Pressed => s_pressed ??= new VisualStyleElement(ClassName, Part, 3);

            private static VisualStyleElement? s_focused;

            public static VisualStyleElement Focused => s_focused ??= new VisualStyleElement(ClassName, Part, 4);

            private static VisualStyleElement? s_disabled;

            public static VisualStyleElement Disabled => s_disabled ??= new VisualStyleElement(ClassName, Part, 5);
        }

        public static class Ticks
        {
            private const int Part = 9;

            private static VisualStyleElement? s_normal;

            public static VisualStyleElement Normal => s_normal ??= new VisualStyleElement(ClassName, Part, 1);
        }

        public static class TicksVertical
        {
            private const int Part = 10;

            private static VisualStyleElement? s_normal;

            public static VisualStyleElement Normal => s_normal ??= new VisualStyleElement(ClassName, Part, 1);
        }
    }

    public static class TreeView
    {
        private const string ClassName = "TREEVIEW";

        public static class Item
        {
            private const int Part = 1;

            private static VisualStyleElement? s_normal;

            public static VisualStyleElement Normal => s_normal ??= new VisualStyleElement(ClassName, Part, 1);

            private static VisualStyleElement? s_hot;

            public static VisualStyleElement Hot => s_hot ??= new VisualStyleElement(ClassName, Part, 2);

            private static VisualStyleElement? s_selected;

            public static VisualStyleElement Selected => s_selected ??= new VisualStyleElement(ClassName, Part, 3);

            private static VisualStyleElement? s_disabled;

            public static VisualStyleElement Disabled => s_disabled ??= new VisualStyleElement(ClassName, Part, 4);

            private static VisualStyleElement? s_selectednotfocus;

            public static VisualStyleElement SelectedNotFocus => s_selectednotfocus ??= new VisualStyleElement(ClassName, Part, 5);
        }

        public static class Glyph
        {
            private const int Part = 2;

            private static VisualStyleElement? s_closed;

            public static VisualStyleElement Closed => s_closed ??= new VisualStyleElement(ClassName, Part, 1);

            private static VisualStyleElement? s_opened;

            public static VisualStyleElement Opened => s_opened ??= new VisualStyleElement(ClassName, Part, 2);
        }

        public static class Branch
        {
            private const int Part = 3;

            private static VisualStyleElement? s_normal;

            public static VisualStyleElement Normal => s_normal ??= new VisualStyleElement(ClassName, Part, 0);
        }
    }

    internal static class ExplorerTreeView
    {
        private const string ClassName = "Explorer::TreeView";

        public static class Glyph
        {
            private const int Part = 2;

            private static VisualStyleElement? s_closed;

            public static VisualStyleElement Closed => s_closed ??= new VisualStyleElement(ClassName, Part, 1);

            private static VisualStyleElement? s_opened;

            public static VisualStyleElement Opened => s_opened ??= new VisualStyleElement(ClassName, Part, 2);
        }
    }

    public static class TextBox
    {
        private const string ClassName = "EDIT";

        public static class TextEdit
        {
            private const int Part = 1;

            private static VisualStyleElement? s_normal;

            public static VisualStyleElement Normal => s_normal ??= new VisualStyleElement(ClassName, Part, 1);

            private static VisualStyleElement? s_hot;

            public static VisualStyleElement Hot => s_hot ??= new VisualStyleElement(ClassName, Part, 2);

            private static VisualStyleElement? s_selected;

            public static VisualStyleElement Selected => s_selected ??= new VisualStyleElement(ClassName, Part, 3);

            private static VisualStyleElement? s_disabled;

            public static VisualStyleElement Disabled => s_disabled ??= new VisualStyleElement(ClassName, Part, 4);

            private static VisualStyleElement? s_focused;

            public static VisualStyleElement Focused => s_focused ??= new VisualStyleElement(ClassName, Part, 5);

            private static VisualStyleElement? s_readonly;

            public static VisualStyleElement ReadOnly => s_readonly ??= new VisualStyleElement(ClassName, Part, 6);

            private static VisualStyleElement? s_assist;

            public static VisualStyleElement Assist => s_assist ??= new VisualStyleElement(ClassName, Part, 7);
        }

        public static class Caret
        {
            private const int Part = 2;

            private static VisualStyleElement? s_normal;

            public static VisualStyleElement Normal => s_normal ??= new VisualStyleElement(ClassName, Part, 0);
        }
    }

    public static class TrayNotify
    {
        private const string ClassName = "TRAYNOTIFY";

        public static class Background
        {
            private const int Part = 1;

            private static VisualStyleElement? s_normal;

            public static VisualStyleElement Normal => s_normal ??= new VisualStyleElement(ClassName, Part, 0);
        }

        public static class AnimateBackground
        {
            private const int Part = 2;

            private static VisualStyleElement? s_normal;

            public static VisualStyleElement Normal => s_normal ??= new VisualStyleElement(ClassName, Part, 0);
        }
    }

    public static class Window
    {
        private const string ClassName = "WINDOW";

        public static class Caption
        {
            private const int Part = 1;

            private static VisualStyleElement? s_active;

            public static VisualStyleElement Active => s_active ??= new VisualStyleElement(ClassName, Part, 1);

            private static VisualStyleElement? s_inactive;

            public static VisualStyleElement Inactive => s_inactive ??= new VisualStyleElement(ClassName, Part, 2);

            private static VisualStyleElement? s_disabled;

            public static VisualStyleElement Disabled => s_disabled ??= new VisualStyleElement(ClassName, Part, 3);
        }

        public static class SmallCaption
        {
            private const int Part = 2;

            private static VisualStyleElement? s_active;

            public static VisualStyleElement Active => s_active ??= new VisualStyleElement(ClassName, Part, 1);

            private static VisualStyleElement? s_inactive;

            public static VisualStyleElement Inactive => s_inactive ??= new VisualStyleElement(ClassName, Part, 2);

            private static VisualStyleElement? s_disabled;

            public static VisualStyleElement Disabled => s_disabled ??= new VisualStyleElement(ClassName, Part, 3);
        }

        public static class MinCaption
        {
            private const int Part = 3;

            private static VisualStyleElement? s_active;

            public static VisualStyleElement Active => s_active ??= new VisualStyleElement(ClassName, Part, 1);

            private static VisualStyleElement? s_inactive;

            public static VisualStyleElement Inactive => s_inactive ??= new VisualStyleElement(ClassName, Part, 2);

            private static VisualStyleElement? s_disabled;

            public static VisualStyleElement Disabled => s_disabled ??= new VisualStyleElement(ClassName, Part, 3);
        }

        public static class SmallMinCaption
        {
            private const int Part = 4;

            private static VisualStyleElement? s_active;

            public static VisualStyleElement Active => s_active ??= new VisualStyleElement(ClassName, Part, 1);

            private static VisualStyleElement? s_inactive;

            public static VisualStyleElement Inactive => s_inactive ??= new VisualStyleElement(ClassName, Part, 2);

            private static VisualStyleElement? s_disabled;

            public static VisualStyleElement Disabled => s_disabled ??= new VisualStyleElement(ClassName, Part, 3);
        }

        public static class MaxCaption
        {
            private const int Part = 5;

            private static VisualStyleElement? s_active;

            public static VisualStyleElement Active => s_active ??= new VisualStyleElement(ClassName, Part, 1);

            private static VisualStyleElement? s_inactive;

            public static VisualStyleElement Inactive => s_inactive ??= new VisualStyleElement(ClassName, Part, 2);

            private static VisualStyleElement? s_disabled;

            public static VisualStyleElement Disabled => s_disabled ??= new VisualStyleElement(ClassName, Part, 3);
        }

        public static class SmallMaxCaption
        {
            private const int Part = 6;

            private static VisualStyleElement? s_active;

            public static VisualStyleElement Active => s_active ??= new VisualStyleElement(ClassName, Part, 1);

            private static VisualStyleElement? s_inactive;

            public static VisualStyleElement Inactive => s_inactive ??= new VisualStyleElement(ClassName, Part, 2);

            private static VisualStyleElement? s_disabled;

            public static VisualStyleElement Disabled => s_disabled ??= new VisualStyleElement(ClassName, Part, 3);
        }

        public static class FrameLeft
        {
            private const int Part = 7;

            private static VisualStyleElement? s_active;

            public static VisualStyleElement Active => s_active ??= new VisualStyleElement(ClassName, Part, 1);

            private static VisualStyleElement? s_inactive;

            public static VisualStyleElement Inactive => s_inactive ??= new VisualStyleElement(ClassName, Part, 2);
        }

        public static class FrameRight
        {
            private const int Part = 8;

            private static VisualStyleElement? s_active;

            public static VisualStyleElement Active => s_active ??= new VisualStyleElement(ClassName, Part, 1);

            private static VisualStyleElement? s_inactive;

            public static VisualStyleElement Inactive => s_inactive ??= new VisualStyleElement(ClassName, Part, 2);
        }

        public static class FrameBottom
        {
            private const int Part = 9;

            private static VisualStyleElement? s_active;

            public static VisualStyleElement Active => s_active ??= new VisualStyleElement(ClassName, Part, 1);

            private static VisualStyleElement? s_inactive;

            public static VisualStyleElement Inactive => s_inactive ??= new VisualStyleElement(ClassName, Part, 2);
        }

        public static class SmallFrameLeft
        {
            private const int Part = 10;

            private static VisualStyleElement? s_active;

            public static VisualStyleElement Active => s_active ??= new VisualStyleElement(ClassName, Part, 1);

            private static VisualStyleElement? s_inactive;

            public static VisualStyleElement Inactive => s_inactive ??= new VisualStyleElement(ClassName, Part, 2);
        }

        public static class SmallFrameRight
        {
            private const int Part = 11;

            private static VisualStyleElement? s_active;

            public static VisualStyleElement Active => s_active ??= new VisualStyleElement(ClassName, Part, 1);

            private static VisualStyleElement? s_inactive;

            public static VisualStyleElement Inactive => s_inactive ??= new VisualStyleElement(ClassName, Part, 2);
        }

        public static class SmallFrameBottom
        {
            private const int Part = 12;

            private static VisualStyleElement? s_active;

            public static VisualStyleElement Active => s_active ??= new VisualStyleElement(ClassName, Part, 1);

            private static VisualStyleElement? s_inactive;

            public static VisualStyleElement Inactive => s_inactive ??= new VisualStyleElement(ClassName, Part, 2);
        }

        public static class SysButton
        {
            private const int Part = 13;

            private static VisualStyleElement? s_normal;

            public static VisualStyleElement Normal => s_normal ??= new VisualStyleElement(ClassName, Part, 1);

            private static VisualStyleElement? s_hot;

            public static VisualStyleElement Hot => s_hot ??= new VisualStyleElement(ClassName, Part, 2);

            private static VisualStyleElement? s_pressed;

            public static VisualStyleElement Pressed => s_pressed ??= new VisualStyleElement(ClassName, Part, 3);

            private static VisualStyleElement? s_disabled;

            public static VisualStyleElement Disabled => s_disabled ??= new VisualStyleElement(ClassName, Part, 4);
        }

        public static class MdiSysButton
        {
            private const int Part = 14;

            private static VisualStyleElement? s_normal;

            public static VisualStyleElement Normal => s_normal ??= new VisualStyleElement(ClassName, Part, 1);

            private static VisualStyleElement? s_hot;

            public static VisualStyleElement Hot => s_hot ??= new VisualStyleElement(ClassName, Part, 2);

            private static VisualStyleElement? s_pressed;

            public static VisualStyleElement Pressed => s_pressed ??= new VisualStyleElement(ClassName, Part, 3);

            private static VisualStyleElement? s_disabled;

            public static VisualStyleElement Disabled => s_disabled ??= new VisualStyleElement(ClassName, Part, 4);
        }

        public static class MinButton
        {
            private const int Part = 15;

            private static VisualStyleElement? s_normal;

            public static VisualStyleElement Normal => s_normal ??= new VisualStyleElement(ClassName, Part, 1);

            private static VisualStyleElement? s_hot;

            public static VisualStyleElement Hot => s_hot ??= new VisualStyleElement(ClassName, Part, 2);

            private static VisualStyleElement? s_pressed;

            public static VisualStyleElement Pressed => s_pressed ??= new VisualStyleElement(ClassName, Part, 3);

            private static VisualStyleElement? s_disabled;

            public static VisualStyleElement Disabled => s_disabled ??= new VisualStyleElement(ClassName, Part, 4);
        }

        public static class MdiMinButton
        {
            private const int Part = 16;

            private static VisualStyleElement? s_normal;

            public static VisualStyleElement Normal => s_normal ??= new VisualStyleElement(ClassName, Part, 1);

            private static VisualStyleElement? s_hot;

            public static VisualStyleElement Hot => s_hot ??= new VisualStyleElement(ClassName, Part, 2);

            private static VisualStyleElement? s_pressed;

            public static VisualStyleElement Pressed => s_pressed ??= new VisualStyleElement(ClassName, Part, 3);

            private static VisualStyleElement? s_disabled;

            public static VisualStyleElement Disabled => s_disabled ??= new VisualStyleElement(ClassName, Part, 4);
        }

        public static class MaxButton
        {
            private const int Part = 17;

            private static VisualStyleElement? s_normal;

            public static VisualStyleElement Normal => s_normal ??= new VisualStyleElement(ClassName, Part, 1);

            private static VisualStyleElement? s_hot;

            public static VisualStyleElement Hot => s_hot ??= new VisualStyleElement(ClassName, Part, 2);

            private static VisualStyleElement? s_pressed;

            public static VisualStyleElement Pressed => s_pressed ??= new VisualStyleElement(ClassName, Part, 3);

            private static VisualStyleElement? s_disabled;

            public static VisualStyleElement Disabled => s_disabled ??= new VisualStyleElement(ClassName, Part, 4);
        }

        public static class CloseButton
        {
            private const int Part = 18;

            private static VisualStyleElement? s_normal;

            public static VisualStyleElement Normal => s_normal ??= new VisualStyleElement(ClassName, Part, 1);

            private static VisualStyleElement? s_hot;

            public static VisualStyleElement Hot => s_hot ??= new VisualStyleElement(ClassName, Part, 2);

            private static VisualStyleElement? s_pressed;

            public static VisualStyleElement Pressed => s_pressed ??= new VisualStyleElement(ClassName, Part, 3);

            private static VisualStyleElement? s_disabled;

            public static VisualStyleElement Disabled => s_disabled ??= new VisualStyleElement(ClassName, Part, 4);
        }

        public static class SmallCloseButton
        {
            private const int Part = 19;

            private static VisualStyleElement? s_normal;

            public static VisualStyleElement Normal => s_normal ??= new VisualStyleElement(ClassName, Part, 1);

            private static VisualStyleElement? s_hot;

            public static VisualStyleElement Hot => s_hot ??= new VisualStyleElement(ClassName, Part, 2);

            private static VisualStyleElement? s_pressed;

            public static VisualStyleElement Pressed => s_pressed ??= new VisualStyleElement(ClassName, Part, 3);

            private static VisualStyleElement? s_disabled;

            public static VisualStyleElement Disabled => s_disabled ??= new VisualStyleElement(ClassName, Part, 4);
        }

        public static class MdiCloseButton
        {
            private const int Part = 20;

            private static VisualStyleElement? s_normal;

            public static VisualStyleElement Normal => s_normal ??= new VisualStyleElement(ClassName, Part, 1);

            private static VisualStyleElement? s_hot;

            public static VisualStyleElement Hot => s_hot ??= new VisualStyleElement(ClassName, Part, 2);

            private static VisualStyleElement? s_pressed;

            public static VisualStyleElement Pressed => s_pressed ??= new VisualStyleElement(ClassName, Part, 3);

            private static VisualStyleElement? s_disabled;

            public static VisualStyleElement Disabled => s_disabled ??= new VisualStyleElement(ClassName, Part, 4);
        }

        public static class RestoreButton
        {
            private const int Part = 21;

            private static VisualStyleElement? s_normal;

            public static VisualStyleElement Normal => s_normal ??= new VisualStyleElement(ClassName, Part, 1);

            private static VisualStyleElement? s_hot;

            public static VisualStyleElement Hot => s_hot ??= new VisualStyleElement(ClassName, Part, 2);

            private static VisualStyleElement? s_pressed;

            public static VisualStyleElement Pressed => s_pressed ??= new VisualStyleElement(ClassName, Part, 3);

            private static VisualStyleElement? s_disabled;

            public static VisualStyleElement Disabled => s_disabled ??= new VisualStyleElement(ClassName, Part, 4);
        }

        public static class MdiRestoreButton
        {
            private const int Part = 22;

            private static VisualStyleElement? s_normal;

            public static VisualStyleElement Normal => s_normal ??= new VisualStyleElement(ClassName, Part, 1);

            private static VisualStyleElement? s_hot;

            public static VisualStyleElement Hot => s_hot ??= new VisualStyleElement(ClassName, Part, 2);

            private static VisualStyleElement? s_pressed;

            public static VisualStyleElement Pressed => s_pressed ??= new VisualStyleElement(ClassName, Part, 3);

            private static VisualStyleElement? s_disabled;

            public static VisualStyleElement Disabled => s_disabled ??= new VisualStyleElement(ClassName, Part, 4);
        }

        public static class HelpButton
        {
            private const int Part = 23;

            private static VisualStyleElement? s_normal;

            public static VisualStyleElement Normal => s_normal ??= new VisualStyleElement(ClassName, Part, 1);

            private static VisualStyleElement? s_hot;

            public static VisualStyleElement Hot => s_hot ??= new VisualStyleElement(ClassName, Part, 2);

            private static VisualStyleElement? s_pressed;

            public static VisualStyleElement Pressed => s_pressed ??= new VisualStyleElement(ClassName, Part, 3);

            private static VisualStyleElement? s_disabled;

            public static VisualStyleElement Disabled => s_disabled ??= new VisualStyleElement(ClassName, Part, 4);
        }

        public static class MdiHelpButton
        {
            private const int Part = 24;

            private static VisualStyleElement? s_normal;

            public static VisualStyleElement Normal => s_normal ??= new VisualStyleElement(ClassName, Part, 1);

            private static VisualStyleElement? s_hot;

            public static VisualStyleElement Hot => s_hot ??= new VisualStyleElement(ClassName, Part, 2);

            private static VisualStyleElement? s_pressed;

            public static VisualStyleElement Pressed => s_pressed ??= new VisualStyleElement(ClassName, Part, 3);

            private static VisualStyleElement? s_disabled;

            public static VisualStyleElement Disabled => s_disabled ??= new VisualStyleElement(ClassName, Part, 4);
        }

        public static class HorizontalScroll
        {
            private const int Part = 25;

            private static VisualStyleElement? s_normal;

            public static VisualStyleElement Normal => s_normal ??= new VisualStyleElement(ClassName, Part, 1);

            private static VisualStyleElement? s_hot;

            public static VisualStyleElement Hot => s_hot ??= new VisualStyleElement(ClassName, Part, 2);

            private static VisualStyleElement? s_pressed;

            public static VisualStyleElement Pressed => s_pressed ??= new VisualStyleElement(ClassName, Part, 3);

            private static VisualStyleElement? s_disabled;

            public static VisualStyleElement Disabled => s_disabled ??= new VisualStyleElement(ClassName, Part, 4);
        }

        public static class HorizontalThumb
        {
            private const int Part = 26;

            private static VisualStyleElement? s_normal;

            public static VisualStyleElement Normal => s_normal ??= new VisualStyleElement(ClassName, Part, 1);

            private static VisualStyleElement? s_hot;

            public static VisualStyleElement Hot => s_hot ??= new VisualStyleElement(ClassName, Part, 2);

            private static VisualStyleElement? s_pressed;

            public static VisualStyleElement Pressed => s_pressed ??= new VisualStyleElement(ClassName, Part, 3);

            private static VisualStyleElement? s_disabled;

            public static VisualStyleElement Disabled => s_disabled ??= new VisualStyleElement(ClassName, Part, 4);
        }

        public static class VerticalScroll
        {
            private const int Part = 27;

            private static VisualStyleElement? s_normal;

            public static VisualStyleElement Normal => s_normal ??= new VisualStyleElement(ClassName, Part, 1);

            private static VisualStyleElement? s_hot;

            public static VisualStyleElement Hot => s_hot ??= new VisualStyleElement(ClassName, Part, 2);

            private static VisualStyleElement? s_pressed;

            public static VisualStyleElement Pressed => s_pressed ??= new VisualStyleElement(ClassName, Part, 3);

            private static VisualStyleElement? s_disabled;

            public static VisualStyleElement Disabled => s_disabled ??= new VisualStyleElement(ClassName, Part, 4);
        }

        public static class VerticalThumb
        {
            private const int Part = 28;

            private static VisualStyleElement? s_normal;

            public static VisualStyleElement Normal => s_normal ??= new VisualStyleElement(ClassName, Part, 1);

            private static VisualStyleElement? s_hot;

            public static VisualStyleElement Hot => s_hot ??= new VisualStyleElement(ClassName, Part, 2);

            private static VisualStyleElement? s_pressed;

            public static VisualStyleElement Pressed => s_pressed ??= new VisualStyleElement(ClassName, Part, 3);

            private static VisualStyleElement? s_disabled;

            public static VisualStyleElement Disabled => s_disabled ??= new VisualStyleElement(ClassName, Part, 4);
        }

        public static class Dialog
        {
            private const int Part = 29;

            private static VisualStyleElement? s_normal;

            public static VisualStyleElement Normal => s_normal ??= new VisualStyleElement(ClassName, Part, 0);
        }

        public static class CaptionSizingTemplate
        {
            private const int Part = 30;

            private static VisualStyleElement? s_normal;

            public static VisualStyleElement Normal => s_normal ??= new VisualStyleElement(ClassName, Part, 0);
        }

        public static class SmallCaptionSizingTemplate
        {
            private const int Part = 31;

            private static VisualStyleElement? s_normal;

            public static VisualStyleElement Normal => s_normal ??= new VisualStyleElement(ClassName, Part, 0);
        }

        public static class FrameLeftSizingTemplate
        {
            private const int Part = 32;

            private static VisualStyleElement? s_normal;

            public static VisualStyleElement Normal => s_normal ??= new VisualStyleElement(ClassName, Part, 0);
        }

        public static class SmallFrameLeftSizingTemplate
        {
            private const int Part = 33;

            private static VisualStyleElement? s_normal;

            public static VisualStyleElement Normal => s_normal ??= new VisualStyleElement(ClassName, Part, 0);
        }

        // Not used as compound word here.
        public static class FrameRightSizingTemplate
        {
            private const int Part = 34;

            private static VisualStyleElement? s_normal;

            public static VisualStyleElement Normal => s_normal ??= new VisualStyleElement(ClassName, Part, 0);
        }

        // Not used as compound word here.
        public static class SmallFrameRightSizingTemplate
        {
            private const int Part = 35;

            private static VisualStyleElement? s_normal;

            public static VisualStyleElement Normal => s_normal ??= new VisualStyleElement(ClassName, Part, 0);
        }

        public static class FrameBottomSizingTemplate
        {
            private const int Part = 36;

            private static VisualStyleElement? s_normal;

            public static VisualStyleElement Normal => s_normal ??= new VisualStyleElement(ClassName, Part, 0);
        }

        public static class SmallFrameBottomSizingTemplate
        {
            private const int Part = 37;

            private static VisualStyleElement? s_normal;

            public static VisualStyleElement Normal => s_normal ??= new VisualStyleElement(ClassName, Part, 0);
        }
    }
}
