// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms.VisualStyles
{
    /// <summary>
    ///  Encapsulates the class, part and state of the "element" you wish to draw using
    ///  the VisualStyleRenderer.
    ///  Usage pattern is something like this: new VisualStyleRenderer(VisualStyleElement.Window.Caption.Active);
    /// </summary>
    public class VisualStyleElement
    {
        internal static readonly int Count = 25; //UPDATE THIS WHEN CLASSES ARE ADDED/REMOVED!
        private readonly string className;
        private readonly int part;
        private readonly int state;

        private VisualStyleElement(string className, int part, int state)
        {
            this.className = className;
            this.part = part;
            this.state = state;
        }

        public static VisualStyleElement CreateElement(string className, int part, int state)
        {
            return new VisualStyleElement(className, part, state);
        }

        public string ClassName
        {
            get
            {
                return className;
            }
        }

        public int Part
        {
            get
            {
                return part;
            }
        }

        public int State
        {
            get
            {
                return state;
            }
        }

        public static class Button
        {
            private static readonly string className = "BUTTON";

            public static class PushButton
            {
                private static readonly int part = 1;

                private static VisualStyleElement normal;

                public static VisualStyleElement Normal => normal ??= new VisualStyleElement(className, part, 1);

                private static VisualStyleElement hot;

                public static VisualStyleElement Hot => hot ??= new VisualStyleElement(className, part, 2);

                private static VisualStyleElement pressed;

                public static VisualStyleElement Pressed => pressed ??= new VisualStyleElement(className, part, 3);

                private static VisualStyleElement disabled;

                public static VisualStyleElement Disabled => disabled ??= new VisualStyleElement(className, part, 4);

                private static VisualStyleElement _default;

                public static VisualStyleElement Default => _default ??= new VisualStyleElement(className, part, 5);
            }

            public static class RadioButton
            {
                private static readonly int part = 2;
                // in Win10 RS3 a new part was added to BUTTONPARTS enum in vsstyle.h  - BP_RADIOBUTTON_HCDISABLED = 8
                internal static readonly int HighContrastDisabledPart = 8;

                private static VisualStyleElement uncheckednormal;

                public static VisualStyleElement UncheckedNormal => uncheckednormal ??= new VisualStyleElement(className, part, 1);

                private static VisualStyleElement uncheckedhot;

                public static VisualStyleElement UncheckedHot => uncheckedhot ??= new VisualStyleElement(className, part, 2);

                private static VisualStyleElement uncheckedpressed;

                public static VisualStyleElement UncheckedPressed => uncheckedpressed ??= new VisualStyleElement(className, part, 3);

                private static VisualStyleElement uncheckeddisabled;

                public static VisualStyleElement UncheckedDisabled => uncheckeddisabled ??= new VisualStyleElement(className, part, 4);

                private static VisualStyleElement checkednormal;

                public static VisualStyleElement CheckedNormal => checkednormal ??= new VisualStyleElement(className, part, 5);

                private static VisualStyleElement checkedhot;

                public static VisualStyleElement CheckedHot => checkedhot ??= new VisualStyleElement(className, part, 6);

                private static VisualStyleElement checkedpressed;

                public static VisualStyleElement CheckedPressed => checkedpressed ??= new VisualStyleElement(className, part, 7);

                private static VisualStyleElement checkeddisabled;

                public static VisualStyleElement CheckedDisabled => checkeddisabled ??= new VisualStyleElement(className, part, 8);
            }

            public static class CheckBox
            {
                private static readonly int part = 3;
                // in Win10 RS3 a new part was added to BUTTONPARTS enum in vsstyle.h  - BP_CHECKBOX_HCDISABLED = 9
                internal static readonly int HighContrastDisabledPart = 9;

                private static VisualStyleElement uncheckednormal;

                public static VisualStyleElement UncheckedNormal => uncheckednormal ??= new VisualStyleElement(className, part, 1);

                private static VisualStyleElement uncheckedhot;

                public static VisualStyleElement UncheckedHot => uncheckedhot ??= new VisualStyleElement(className, part, 2);

                private static VisualStyleElement uncheckedpressed;

                public static VisualStyleElement UncheckedPressed => uncheckedpressed ??= new VisualStyleElement(className, part, 3);

                private static VisualStyleElement uncheckeddisabled;

                public static VisualStyleElement UncheckedDisabled => uncheckeddisabled ??= new VisualStyleElement(className, part, 4);

                private static VisualStyleElement checkednormal;

                public static VisualStyleElement CheckedNormal => checkednormal ??= new VisualStyleElement(className, part, 5);

                private static VisualStyleElement checkedhot;

                public static VisualStyleElement CheckedHot => checkedhot ??= new VisualStyleElement(className, part, 6);

                private static VisualStyleElement checkedpressed;

                public static VisualStyleElement CheckedPressed => checkedpressed ??= new VisualStyleElement(className, part, 7);

                private static VisualStyleElement checkeddisabled;

                public static VisualStyleElement CheckedDisabled => checkeddisabled ??= new VisualStyleElement(className, part, 8);

                private static VisualStyleElement mixednormal;

                public static VisualStyleElement MixedNormal => mixednormal ??= new VisualStyleElement(className, part, 9);

                private static VisualStyleElement mixedhot;

                public static VisualStyleElement MixedHot => mixedhot ??= new VisualStyleElement(className, part, 10);

                private static VisualStyleElement mixedpressed;

                public static VisualStyleElement MixedPressed => mixedpressed ??= new VisualStyleElement(className, part, 11);

                private static VisualStyleElement mixeddisabled;

                public static VisualStyleElement MixedDisabled => mixeddisabled ??= new VisualStyleElement(className, part, 12);
            }

            public static class GroupBox
            {
                private static readonly int part = 4;
                // in Win10 RS3 a new part was added to BUTTONPARTS enum in vsstyle.h  - BP_GROUPBOX_HCDISABLED = 10
                internal static readonly int HighContrastDisabledPart = 10;

                private static VisualStyleElement normal;

                public static VisualStyleElement Normal => normal ??= new VisualStyleElement(className, part, 1);

                private static VisualStyleElement disabled;

                public static VisualStyleElement Disabled => disabled ??= new VisualStyleElement(className, part, 2);
            }

            public static class UserButton
            {
                private static readonly int part = 5;

                private static VisualStyleElement normal;

                public static VisualStyleElement Normal => normal ??= new VisualStyleElement(className, part, 0);
            }
        } //END BUTTON

        public static class ComboBox
        {
            private static readonly string className = "COMBOBOX";

            public static class DropDownButton
            {
                private static readonly int part = 1;

                private static VisualStyleElement normal;

                public static VisualStyleElement Normal => normal ??= new VisualStyleElement(className, part, 1);

                private static VisualStyleElement hot;

                public static VisualStyleElement Hot => hot ??= new VisualStyleElement(className, part, 2);

                private static VisualStyleElement pressed;

                public static VisualStyleElement Pressed => pressed ??= new VisualStyleElement(className, part, 3);

                private static VisualStyleElement disabled;

                public static VisualStyleElement Disabled => disabled ??= new VisualStyleElement(className, part, 4);
            }

            internal static class Border
            {
                // Paints a rectangle with a 1 pixel edge + round corners,
                // and fills it with a color
                private const int part = 4;

                // possible states for various fill color:
                //  2 - semi transparent
                //  3 - Window Color (white by default)
                //  4 - disabled (light gray by default)

                private static VisualStyleElement normal;

                public static VisualStyleElement Normal => normal ??= new VisualStyleElement(className, part, 3);
            }

            internal static class ReadOnlyButton
            {
                // Paints a button for non-editable comboboxes (DropDownStyle=DropDownList)
                private const int part = 5;

                // possible states:
                //  1 - disabled (gray by default)
                //  2 - normal (blue by default)
                //  3 - pressed (dark blue by default)
                //  4 - flat (light gray by default)

                private static VisualStyleElement normal;

                public static VisualStyleElement Normal => normal ??= new VisualStyleElement(className, part, 2);
            }

            internal static class DropDownButtonRight
            {
                // Paints a dropdownbutton with right angles on the left side for
                // editable comboboxes (DropDownStyle=DropDown) for RTL=false case
                private const int part = 6;

                // possible states:
                //  1 - normal (transparent background with just normal arrow)
                //  2 - hot (arrow with blue background by default)
                //  3 - pressed (arrow with dark blue background by default)
                //  4 - disabled (transparent background with just light arrow)

                private static VisualStyleElement normal;

                public static VisualStyleElement Normal => normal ??= new VisualStyleElement(className, part, 1);
            }

            internal static class DropDownButtonLeft
            {
                // Paints a dropdownbutton with right angles on the right side for
                // editable comboboxes (DropDownStyle=DropDown) for RTL=true case
                private const int part = 7;

                // possible states:
                //  1 - transparent normal (just normal arrow)
                //  2 - normal (blue by default)
                //  3 - pressed (dark blue by default)
                //  4 - transparent disabled (just light arrow)

                private static VisualStyleElement normal;

                public static VisualStyleElement Normal => normal ??= new VisualStyleElement(className, part, 2);
            }
        } //END COMBOBOX

        public static class Page
        {
            private static readonly string className = "PAGE";

            public static class Up
            {
                private static readonly int part = 1;

                private static VisualStyleElement normal;

                public static VisualStyleElement Normal => normal ??= new VisualStyleElement(className, part, 1);

                private static VisualStyleElement hot;

                public static VisualStyleElement Hot => hot ??= new VisualStyleElement(className, part, 2);

                private static VisualStyleElement pressed;

                public static VisualStyleElement Pressed => pressed ??= new VisualStyleElement(className, part, 3);

                private static VisualStyleElement disabled;

                public static VisualStyleElement Disabled => disabled ??= new VisualStyleElement(className, part, 4);
            }

            public static class Down
            {
                private static readonly int part = 2;

                private static VisualStyleElement normal;

                public static VisualStyleElement Normal => normal ??= new VisualStyleElement(className, part, 1);

                private static VisualStyleElement hot;

                public static VisualStyleElement Hot => hot ??= new VisualStyleElement(className, part, 2);

                private static VisualStyleElement pressed;

                public static VisualStyleElement Pressed => pressed ??= new VisualStyleElement(className, part, 3);

                private static VisualStyleElement disabled;

                public static VisualStyleElement Disabled => disabled ??= new VisualStyleElement(className, part, 4);
            }

            public static class UpHorizontal
            {
                private static readonly int part = 3;

                private static VisualStyleElement normal;

                public static VisualStyleElement Normal => normal ??= new VisualStyleElement(className, part, 1);

                private static VisualStyleElement hot;

                public static VisualStyleElement Hot => hot ??= new VisualStyleElement(className, part, 2);

                private static VisualStyleElement pressed;

                public static VisualStyleElement Pressed => pressed ??= new VisualStyleElement(className, part, 3);

                private static VisualStyleElement disabled;

                public static VisualStyleElement Disabled => disabled ??= new VisualStyleElement(className, part, 4);
            }

            public static class DownHorizontal
            {
                private static readonly int part = 4;

                private static VisualStyleElement normal;

                public static VisualStyleElement Normal => normal ??= new VisualStyleElement(className, part, 1);

                private static VisualStyleElement hot;

                public static VisualStyleElement Hot => hot ??= new VisualStyleElement(className, part, 2);

                private static VisualStyleElement pressed;

                public static VisualStyleElement Pressed => pressed ??= new VisualStyleElement(className, part, 3);

                private static VisualStyleElement disabled;

                public static VisualStyleElement Disabled => disabled ??= new VisualStyleElement(className, part, 4);
            }
        } //END PAGE

        public static class Spin
        {
            private static readonly string className = "SPIN";

            public static class Up
            {
                private static readonly int part = 1;

                private static VisualStyleElement normal;

                public static VisualStyleElement Normal => normal ??= new VisualStyleElement(className, part, 1);

                private static VisualStyleElement hot;

                public static VisualStyleElement Hot => hot ??= new VisualStyleElement(className, part, 2);

                private static VisualStyleElement pressed;

                public static VisualStyleElement Pressed => pressed ??= new VisualStyleElement(className, part, 3);

                private static VisualStyleElement disabled;

                public static VisualStyleElement Disabled => disabled ??= new VisualStyleElement(className, part, 4);
            }

            public static class Down
            {
                private static readonly int part = 2;

                private static VisualStyleElement normal;

                public static VisualStyleElement Normal => normal ??= new VisualStyleElement(className, part, 1);

                private static VisualStyleElement hot;

                public static VisualStyleElement Hot => hot ??= new VisualStyleElement(className, part, 2);

                private static VisualStyleElement pressed;

                public static VisualStyleElement Pressed => pressed ??= new VisualStyleElement(className, part, 3);

                private static VisualStyleElement disabled;

                public static VisualStyleElement Disabled => disabled ??= new VisualStyleElement(className, part, 4);
            }

            public static class UpHorizontal
            {
                private static readonly int part = 3;

                private static VisualStyleElement normal;

                public static VisualStyleElement Normal => normal ??= new VisualStyleElement(className, part, 1);

                private static VisualStyleElement hot;

                public static VisualStyleElement Hot => hot ??= new VisualStyleElement(className, part, 2);

                private static VisualStyleElement pressed;

                public static VisualStyleElement Pressed => pressed ??= new VisualStyleElement(className, part, 3);

                private static VisualStyleElement disabled;

                public static VisualStyleElement Disabled => disabled ??= new VisualStyleElement(className, part, 4);
            }

            public static class DownHorizontal
            {
                private static readonly int part = 4;

                private static VisualStyleElement normal;

                public static VisualStyleElement Normal => normal ??= new VisualStyleElement(className, part, 1);

                private static VisualStyleElement hot;

                public static VisualStyleElement Hot => hot ??= new VisualStyleElement(className, part, 2);

                private static VisualStyleElement pressed;

                public static VisualStyleElement Pressed => pressed ??= new VisualStyleElement(className, part, 3);

                private static VisualStyleElement disabled;

                public static VisualStyleElement Disabled => disabled ??= new VisualStyleElement(className, part, 4);
            }
        } //END SPIN

        public static class ScrollBar
        {
            private static readonly string className = "SCROLLBAR";

            public static class ArrowButton
            {
                private static readonly int part = 1;

                private static VisualStyleElement upnormal;

                public static VisualStyleElement UpNormal => upnormal ??= new VisualStyleElement(className, part, 1);

                private static VisualStyleElement uphot;

                public static VisualStyleElement UpHot => uphot ??= new VisualStyleElement(className, part, 2);

                private static VisualStyleElement uppressed;

                public static VisualStyleElement UpPressed => uppressed ??= new VisualStyleElement(className, part, 3);

                private static VisualStyleElement updisabled;

                public static VisualStyleElement UpDisabled => updisabled ??= new VisualStyleElement(className, part, 4);

                private static VisualStyleElement downnormal;

                public static VisualStyleElement DownNormal => downnormal ??= new VisualStyleElement(className, part, 5);

                private static VisualStyleElement downhot;

                public static VisualStyleElement DownHot => downhot ??= new VisualStyleElement(className, part, 6);

                private static VisualStyleElement downpressed;

                public static VisualStyleElement DownPressed => downpressed ??= new VisualStyleElement(className, part, 7);

                private static VisualStyleElement downdisabled;

                public static VisualStyleElement DownDisabled => downdisabled ??= new VisualStyleElement(className, part, 8);

                private static VisualStyleElement leftnormal;

                public static VisualStyleElement LeftNormal => leftnormal ??= new VisualStyleElement(className, part, 9);

                private static VisualStyleElement lefthot;

                public static VisualStyleElement LeftHot => lefthot ??= new VisualStyleElement(className, part, 10);

                private static VisualStyleElement leftpressed;

                public static VisualStyleElement LeftPressed => leftpressed ??= new VisualStyleElement(className, part, 11);

                private static VisualStyleElement leftdisabled;

                public static VisualStyleElement LeftDisabled => leftdisabled ??= new VisualStyleElement(className, part, 12);

                private static VisualStyleElement rightnormal;

                public static VisualStyleElement RightNormal => rightnormal ??= new VisualStyleElement(className, part, 13);

                private static VisualStyleElement righthot;

                public static VisualStyleElement RightHot => righthot ??= new VisualStyleElement(className, part, 14);

                private static VisualStyleElement rightpressed;

                public static VisualStyleElement RightPressed => rightpressed ??= new VisualStyleElement(className, part, 15);

                private static VisualStyleElement rightdisabled;

                public static VisualStyleElement RightDisabled => rightdisabled ??= new VisualStyleElement(className, part, 16);
            }

            public static class ThumbButtonHorizontal
            {
                private static readonly int part = 2;

                private static VisualStyleElement normal;

                public static VisualStyleElement Normal => normal ??= new VisualStyleElement(className, part, 1);

                private static VisualStyleElement hot;

                public static VisualStyleElement Hot => hot ??= new VisualStyleElement(className, part, 2);

                private static VisualStyleElement pressed;

                public static VisualStyleElement Pressed => pressed ??= new VisualStyleElement(className, part, 3);

                private static VisualStyleElement disabled;

                public static VisualStyleElement Disabled => disabled ??= new VisualStyleElement(className, part, 4);
            }

            public static class ThumbButtonVertical
            {
                private static readonly int part = 3;

                private static VisualStyleElement normal;

                public static VisualStyleElement Normal => normal ??= new VisualStyleElement(className, part, 1);

                private static VisualStyleElement hot;

                public static VisualStyleElement Hot => hot ??= new VisualStyleElement(className, part, 2);

                private static VisualStyleElement pressed;

                public static VisualStyleElement Pressed => pressed ??= new VisualStyleElement(className, part, 3);

                private static VisualStyleElement disabled;

                public static VisualStyleElement Disabled => disabled ??= new VisualStyleElement(className, part, 4);
            }

            public static class RightTrackHorizontal
            {
                private static readonly int part = 4;

                private static VisualStyleElement normal;

                public static VisualStyleElement Normal => normal ??= new VisualStyleElement(className, part, 1);

                private static VisualStyleElement hot;

                public static VisualStyleElement Hot => hot ??= new VisualStyleElement(className, part, 2);

                private static VisualStyleElement pressed;

                public static VisualStyleElement Pressed => pressed ??= new VisualStyleElement(className, part, 3);

                private static VisualStyleElement disabled;

                public static VisualStyleElement Disabled => disabled ??= new VisualStyleElement(className, part, 4);
            }

            public static class LeftTrackHorizontal
            {
                private static readonly int part = 5;

                private static VisualStyleElement normal;

                public static VisualStyleElement Normal => normal ??= new VisualStyleElement(className, part, 1);

                private static VisualStyleElement hot;

                public static VisualStyleElement Hot => hot ??= new VisualStyleElement(className, part, 2);

                private static VisualStyleElement pressed;

                public static VisualStyleElement Pressed => pressed ??= new VisualStyleElement(className, part, 3);

                private static VisualStyleElement disabled;

                public static VisualStyleElement Disabled => disabled ??= new VisualStyleElement(className, part, 4);
            }

            public static class LowerTrackVertical
            {
                private static readonly int part = 6;

                private static VisualStyleElement normal;

                public static VisualStyleElement Normal => normal ??= new VisualStyleElement(className, part, 1);

                private static VisualStyleElement hot;

                public static VisualStyleElement Hot => hot ??= new VisualStyleElement(className, part, 2);

                private static VisualStyleElement pressed;

                public static VisualStyleElement Pressed => pressed ??= new VisualStyleElement(className, part, 3);

                private static VisualStyleElement disabled;

                public static VisualStyleElement Disabled => disabled ??= new VisualStyleElement(className, part, 4);
            }

            public static class UpperTrackVertical
            {
                private static readonly int part = 7;

                private static VisualStyleElement normal;

                public static VisualStyleElement Normal => normal ??= new VisualStyleElement(className, part, 1);

                private static VisualStyleElement hot;

                public static VisualStyleElement Hot => hot ??= new VisualStyleElement(className, part, 2);

                private static VisualStyleElement pressed;

                public static VisualStyleElement Pressed => pressed ??= new VisualStyleElement(className, part, 3);

                private static VisualStyleElement disabled;

                public static VisualStyleElement Disabled => disabled ??= new VisualStyleElement(className, part, 4);
            }

            public static class GripperHorizontal
            {
                private static readonly int part = 8;

                private static VisualStyleElement normal;

                public static VisualStyleElement Normal => normal ??= new VisualStyleElement(className, part, 0);
            }

            public static class GripperVertical
            {
                private static readonly int part = 9;

                private static VisualStyleElement normal;

                public static VisualStyleElement Normal => normal ??= new VisualStyleElement(className, part, 0);
            }

            public static class SizeBox
            {
                private static readonly int part = 10;

                private static VisualStyleElement rightalign;

                public static VisualStyleElement RightAlign => rightalign ??= new VisualStyleElement(className, part, 1);

                private static VisualStyleElement leftalign;

                public static VisualStyleElement LeftAlign => leftalign ??= new VisualStyleElement(className, part, 2);
            }
        } // END SCROLLBAR

        public static class Tab
        {
            private static readonly string className = "TAB";

            public static class TabItem
            {
                private static readonly int part = 1;

                private static VisualStyleElement normal;

                public static VisualStyleElement Normal => normal ??= new VisualStyleElement(className, part, 1);

                private static VisualStyleElement hot;

                public static VisualStyleElement Hot => hot ??= new VisualStyleElement(className, part, 2);

                private static VisualStyleElement pressed;

                public static VisualStyleElement Pressed => pressed ??= new VisualStyleElement(className, part, 3);

                private static VisualStyleElement disabled;

                public static VisualStyleElement Disabled => disabled ??= new VisualStyleElement(className, part, 4);
            }

            public static class TabItemLeftEdge
            {
                private static readonly int part = 2;

                private static VisualStyleElement normal;

                public static VisualStyleElement Normal => normal ??= new VisualStyleElement(className, part, 1);

                private static VisualStyleElement hot;

                public static VisualStyleElement Hot => hot ??= new VisualStyleElement(className, part, 2);

                private static VisualStyleElement pressed;

                public static VisualStyleElement Pressed => pressed ??= new VisualStyleElement(className, part, 3);

                private static VisualStyleElement disabled;

                public static VisualStyleElement Disabled => disabled ??= new VisualStyleElement(className, part, 4);
            }

            public static class TabItemRightEdge
            {
                private static readonly int part = 3;

                private static VisualStyleElement normal;

                public static VisualStyleElement Normal => normal ??= new VisualStyleElement(className, part, 1);

                private static VisualStyleElement hot;

                public static VisualStyleElement Hot => hot ??= new VisualStyleElement(className, part, 2);

                private static VisualStyleElement pressed;

                public static VisualStyleElement Pressed => pressed ??= new VisualStyleElement(className, part, 3);

                private static VisualStyleElement disabled;

                public static VisualStyleElement Disabled => disabled ??= new VisualStyleElement(className, part, 4);
            }

            public static class TabItemBothEdges
            {
                private static readonly int part = 4;

                private static VisualStyleElement normal;

                public static VisualStyleElement Normal => normal ??= new VisualStyleElement(className, part, 0);
            }

            public static class TopTabItem
            {
                private static readonly int part = 5;

                private static VisualStyleElement normal;

                public static VisualStyleElement Normal => normal ??= new VisualStyleElement(className, part, 1);

                private static VisualStyleElement hot;

                public static VisualStyleElement Hot => hot ??= new VisualStyleElement(className, part, 2);

                private static VisualStyleElement pressed;

                public static VisualStyleElement Pressed => pressed ??= new VisualStyleElement(className, part, 3);

                private static VisualStyleElement disabled;

                public static VisualStyleElement Disabled => disabled ??= new VisualStyleElement(className, part, 4);

            }

            public static class TopTabItemLeftEdge
            {
                private static readonly int part = 6;

                private static VisualStyleElement normal;

                public static VisualStyleElement Normal => normal ??= new VisualStyleElement(className, part, 1);

                private static VisualStyleElement hot;

                public static VisualStyleElement Hot => hot ??= new VisualStyleElement(className, part, 2);

                private static VisualStyleElement pressed;

                public static VisualStyleElement Pressed => pressed ??= new VisualStyleElement(className, part, 3);

                private static VisualStyleElement disabled;

                public static VisualStyleElement Disabled => disabled ??= new VisualStyleElement(className, part, 4);
            }

            public static class TopTabItemRightEdge
            {
                private static readonly int part = 7;

                private static VisualStyleElement normal;

                public static VisualStyleElement Normal => normal ??= new VisualStyleElement(className, part, 1);

                private static VisualStyleElement hot;

                public static VisualStyleElement Hot => hot ??= new VisualStyleElement(className, part, 2);

                private static VisualStyleElement pressed;

                public static VisualStyleElement Pressed => pressed ??= new VisualStyleElement(className, part, 3);

                private static VisualStyleElement disabled;

                public static VisualStyleElement Disabled => disabled ??= new VisualStyleElement(className, part, 4);

            }

            public static class TopTabItemBothEdges
            {
                private static readonly int part = 8;

                private static VisualStyleElement normal;

                public static VisualStyleElement Normal => normal ??= new VisualStyleElement(className, part, 0);
            }

            public static class Pane
            {
                private static readonly int part = 9;

                private static VisualStyleElement normal;

                public static VisualStyleElement Normal => normal ??= new VisualStyleElement(className, part, 0);

            }

            public static class Body
            {
                private static readonly int part = 10;

                private static VisualStyleElement normal;

                public static VisualStyleElement Normal => normal ??= new VisualStyleElement(className, part, 0);

            }
        } // END TAB

        public static class ExplorerBar
        {
            private static readonly string className = "EXPLORERBAR";

            public static class HeaderBackground
            {
                private static readonly int part = 1;

                private static VisualStyleElement normal;

                public static VisualStyleElement Normal => normal ??= new VisualStyleElement(className, part, 0);

            }

            public static class HeaderClose
            {
                private static readonly int part = 2;

                private static VisualStyleElement normal;

                public static VisualStyleElement Normal => normal ??= new VisualStyleElement(className, part, 1);

                private static VisualStyleElement hot;

                public static VisualStyleElement Hot => hot ??= new VisualStyleElement(className, part, 2);

                private static VisualStyleElement pressed;

                public static VisualStyleElement Pressed => pressed ??= new VisualStyleElement(className, part, 3);
            }

            public static class HeaderPin
            {
                private static readonly int part = 3;

                private static VisualStyleElement normal;

                public static VisualStyleElement Normal => normal ??= new VisualStyleElement(className, part, 1);

                private static VisualStyleElement hot;

                public static VisualStyleElement Hot => hot ??= new VisualStyleElement(className, part, 2);

                private static VisualStyleElement pressed;

                public static VisualStyleElement Pressed => pressed ??= new VisualStyleElement(className, part, 3);

                private static VisualStyleElement selectednormal;

                public static VisualStyleElement SelectedNormal => selectednormal ??= new VisualStyleElement(className, part, 4);

                private static VisualStyleElement selectedhot;

                public static VisualStyleElement SelectedHot => selectedhot ??= new VisualStyleElement(className, part, 5);

                private static VisualStyleElement selectedpressed;

                public static VisualStyleElement SelectedPressed => selectedpressed ??= new VisualStyleElement(className, part, 6);
            }

            public static class IEBarMenu
            {
                private static readonly int part = 4;

                private static VisualStyleElement normal;

                public static VisualStyleElement Normal => normal ??= new VisualStyleElement(className, part, 1);

                private static VisualStyleElement hot;

                public static VisualStyleElement Hot => hot ??= new VisualStyleElement(className, part, 2);

                private static VisualStyleElement pressed;

                public static VisualStyleElement Pressed => pressed ??= new VisualStyleElement(className, part, 3);
            }

            public static class NormalGroupBackground
            {
                private static readonly int part = 5;

                private static VisualStyleElement normal;

                public static VisualStyleElement Normal => normal ??= new VisualStyleElement(className, part, 0);

            }

            public static class NormalGroupCollapse
            {
                private static readonly int part = 6;

                private static VisualStyleElement normal;

                public static VisualStyleElement Normal => normal ??= new VisualStyleElement(className, part, 1);

                private static VisualStyleElement hot;

                public static VisualStyleElement Hot => hot ??= new VisualStyleElement(className, part, 2);

                private static VisualStyleElement pressed;

                public static VisualStyleElement Pressed => pressed ??= new VisualStyleElement(className, part, 3);
            }

            public static class NormalGroupExpand
            {
                private static readonly int part = 7;

                private static VisualStyleElement normal;

                public static VisualStyleElement Normal => normal ??= new VisualStyleElement(className, part, 1);

                private static VisualStyleElement hot;

                public static VisualStyleElement Hot => hot ??= new VisualStyleElement(className, part, 2);

                private static VisualStyleElement pressed;

                public static VisualStyleElement Pressed => pressed ??= new VisualStyleElement(className, part, 3);
            }

            public static class NormalGroupHead
            {
                private static readonly int part = 8;

                private static VisualStyleElement normal;

                public static VisualStyleElement Normal => normal ??= new VisualStyleElement(className, part, 0);
            }

            public static class SpecialGroupBackground
            {
                private static readonly int part = 9;

                private static VisualStyleElement normal;

                public static VisualStyleElement Normal => normal ??= new VisualStyleElement(className, part, 0);

            }

            public static class SpecialGroupCollapse
            {
                private static readonly int part = 10;

                private static VisualStyleElement normal;

                public static VisualStyleElement Normal => normal ??= new VisualStyleElement(className, part, 1);

                private static VisualStyleElement hot;

                public static VisualStyleElement Hot => hot ??= new VisualStyleElement(className, part, 2);

                private static VisualStyleElement pressed;

                public static VisualStyleElement Pressed => pressed ??= new VisualStyleElement(className, part, 3);
            }

            public static class SpecialGroupExpand
            {
                private static readonly int part = 11;

                private static VisualStyleElement normal;

                public static VisualStyleElement Normal => normal ??= new VisualStyleElement(className, part, 1);

                private static VisualStyleElement hot;

                public static VisualStyleElement Hot => hot ??= new VisualStyleElement(className, part, 2);

                private static VisualStyleElement pressed;

                public static VisualStyleElement Pressed => pressed ??= new VisualStyleElement(className, part, 3);
            }

            public static class SpecialGroupHead
            {
                private static readonly int part = 12;

                private static VisualStyleElement normal;

                public static VisualStyleElement Normal => normal ??= new VisualStyleElement(className, part, 0);

            }
        } // END EXPLORERBAR

        public static class Header
        {
            private static readonly string className = "HEADER";

            public static class Item
            {
                private static readonly int part = 1;

                private static VisualStyleElement normal;

                public static VisualStyleElement Normal => normal ??= new VisualStyleElement(className, part, 1);

                private static VisualStyleElement hot;

                public static VisualStyleElement Hot => hot ??= new VisualStyleElement(className, part, 2);

                private static VisualStyleElement pressed;

                public static VisualStyleElement Pressed => pressed ??= new VisualStyleElement(className, part, 3);
            }

            public static class ItemLeft
            {
                private static readonly int part = 2;

                private static VisualStyleElement normal;

                public static VisualStyleElement Normal => normal ??= new VisualStyleElement(className, part, 1);

                private static VisualStyleElement hot;

                public static VisualStyleElement Hot => hot ??= new VisualStyleElement(className, part, 2);

                private static VisualStyleElement pressed;

                public static VisualStyleElement Pressed => pressed ??= new VisualStyleElement(className, part, 3);
            }

            public static class ItemRight
            {
                private static readonly int part = 3;

                private static VisualStyleElement normal;

                public static VisualStyleElement Normal => normal ??= new VisualStyleElement(className, part, 1);

                private static VisualStyleElement hot;

                public static VisualStyleElement Hot => hot ??= new VisualStyleElement(className, part, 2);

                private static VisualStyleElement pressed;

                public static VisualStyleElement Pressed => pressed ??= new VisualStyleElement(className, part, 3);
            }

            public static class SortArrow
            {
                private static readonly int part = 4;

                private static VisualStyleElement sortedup;

                public static VisualStyleElement SortedUp => sortedup ??= new VisualStyleElement(className, part, 1);

                private static VisualStyleElement sorteddown;

                public static VisualStyleElement SortedDown => sorteddown ??= new VisualStyleElement(className, part, 2);
            }
        } // END HEADER

        public static class ListView
        {
            private static readonly string className = "LISTVIEW";

            public static class Item
            {
                private static readonly int part = 1;

                private static VisualStyleElement normal;

                public static VisualStyleElement Normal => normal ??= new VisualStyleElement(className, part, 1);

                private static VisualStyleElement hot;

                public static VisualStyleElement Hot => hot ??= new VisualStyleElement(className, part, 2);

                private static VisualStyleElement selected;

                public static VisualStyleElement Selected => selected ??= new VisualStyleElement(className, part, 3);

                private static VisualStyleElement disabled;

                public static VisualStyleElement Disabled => disabled ??= new VisualStyleElement(className, part, 4);

                private static VisualStyleElement selectednotfocus;

                public static VisualStyleElement SelectedNotFocus => selectednotfocus ??= new VisualStyleElement(className, part, 5);
            }

            public static class Group
            {
                private static readonly int part = 2;

                private static VisualStyleElement normal;

                public static VisualStyleElement Normal => normal ??= new VisualStyleElement(className, part, 0);
            }

            public static class Detail
            {
                private static readonly int part = 3;

                private static VisualStyleElement normal;

                public static VisualStyleElement Normal => normal ??= new VisualStyleElement(className, part, 0);
            }

            public static class SortedDetail
            {
                private static readonly int part = 4;

                private static VisualStyleElement normal;

                public static VisualStyleElement Normal => normal ??= new VisualStyleElement(className, part, 0);
            }

            public static class EmptyText
            {
                private static readonly int part = 5;

                private static VisualStyleElement normal;

                public static VisualStyleElement Normal => normal ??= new VisualStyleElement(className, part, 0);

            }
        } // END LISTVIEW

        public static class MenuBand
        {
            private static readonly string className = "MENUBAND";

            public static class NewApplicationButton
            {
                private static readonly int part = 1;

                private static VisualStyleElement normal;

                public static VisualStyleElement Normal => normal ??= new VisualStyleElement(className, part, 1);

                private static VisualStyleElement hot;

                public static VisualStyleElement Hot => hot ??= new VisualStyleElement(className, part, 2);

                private static VisualStyleElement pressed;

                public static VisualStyleElement Pressed => pressed ??= new VisualStyleElement(className, part, 3);

                private static VisualStyleElement disabled;

                public static VisualStyleElement Disabled => disabled ??= new VisualStyleElement(className, part, 4);

                private static VisualStyleElement _checked;

                public static VisualStyleElement Checked => _checked ??= new VisualStyleElement(className, part, 5);

                private static VisualStyleElement hotchecked;

                public static VisualStyleElement HotChecked => hotchecked ??= new VisualStyleElement(className, part, 6);
            }

            public static class Separator
            {
                private static readonly int part = 2;

                private static VisualStyleElement normal;

                public static VisualStyleElement Normal => normal ??= new VisualStyleElement(className, part, 0);
            }
        } // END MENUBAND

        public static class Menu
        {
            private static readonly string className = "MENU";

            public static class Item
            {
                private static readonly int part = 1;

                private static VisualStyleElement normal;

                public static VisualStyleElement Normal => normal ??= new VisualStyleElement(className, part, 1);

                private static VisualStyleElement selected;

                public static VisualStyleElement Selected => selected ??= new VisualStyleElement(className, part, 2);

                private static VisualStyleElement demoted;

                public static VisualStyleElement Demoted => demoted ??= new VisualStyleElement(className, part, 3);
            }

            public static class DropDown
            {
                private static readonly int part = 2;

                private static VisualStyleElement normal;

                public static VisualStyleElement Normal => normal ??= new VisualStyleElement(className, part, 0);
            }

            public static class BarItem
            {
                private static readonly int part = 3;

                private static VisualStyleElement normal;

                public static VisualStyleElement Normal => normal ??= new VisualStyleElement(className, part, 0);
            }

            public static class BarDropDown
            {
                private static readonly int part = 4;

                private static VisualStyleElement normal;

                public static VisualStyleElement Normal => normal ??= new VisualStyleElement(className, part, 0);
            }

            public static class Chevron
            {
                private static readonly int part = 5;

                private static VisualStyleElement normal;

                public static VisualStyleElement Normal => normal ??= new VisualStyleElement(className, part, 0);
            }

            public static class Separator
            {
                private static readonly int part = 6;

                private static VisualStyleElement normal;

                public static VisualStyleElement Normal => normal ??= new VisualStyleElement(className, part, 0);
            }
        } // END MENU

        public static class ProgressBar
        {
            private static readonly string className = "PROGRESS";

            public static class Bar
            {
                private static readonly int part = 1;

                private static VisualStyleElement normal;

                public static VisualStyleElement Normal => normal ??= new VisualStyleElement(className, part, 0);
            }

            public static class BarVertical
            {
                private static readonly int part = 2;

                private static VisualStyleElement normal;

                public static VisualStyleElement Normal => normal ??= new VisualStyleElement(className, part, 0);
            }

            public static class Chunk
            {
                private static readonly int part = 3;

                private static VisualStyleElement normal;

                public static VisualStyleElement Normal => normal ??= new VisualStyleElement(className, part, 0);
            }

            public static class ChunkVertical
            {
                private static readonly int part = 4;

                private static VisualStyleElement normal;

                public static VisualStyleElement Normal => normal ??= new VisualStyleElement(className, part, 0);
            }
        } // END PROGRESSBAR

        public static class Rebar
        {
            private static readonly string className = "REBAR";

            public static class Gripper
            {
                private static readonly int part = 1;

                private static VisualStyleElement normal;

                public static VisualStyleElement Normal => normal ??= new VisualStyleElement(className, part, 0);
            }

            public static class GripperVertical
            {
                private static readonly int part = 2;

                private static VisualStyleElement normal;

                public static VisualStyleElement Normal => normal ??= new VisualStyleElement(className, part, 0);
            }

            public static class Band
            {
                private static readonly int part = 3;

                private static VisualStyleElement normal;

                public static VisualStyleElement Normal => normal ??= new VisualStyleElement(className, part, 0);

            }

            public static class Chevron
            {
                private static readonly int part = 4;

                private static VisualStyleElement normal;

                public static VisualStyleElement Normal => normal ??= new VisualStyleElement(className, part, 1);

                private static VisualStyleElement hot;

                public static VisualStyleElement Hot => hot ??= new VisualStyleElement(className, part, 2);

                private static VisualStyleElement pressed;

                public static VisualStyleElement Pressed => pressed ??= new VisualStyleElement(className, part, 3);
            }

            public static class ChevronVertical
            {
                private static readonly int part = 5;

                private static VisualStyleElement normal;

                public static VisualStyleElement Normal => normal ??= new VisualStyleElement(className, part, 1);

                private static VisualStyleElement hot;

                public static VisualStyleElement Hot => hot ??= new VisualStyleElement(className, part, 2);

                private static VisualStyleElement pressed;

                public static VisualStyleElement Pressed => pressed ??= new VisualStyleElement(className, part, 3);
            }
        } // END REBAR

        public static class StartPanel
        {
            private static readonly string className = "STARTPANEL";

            public static class UserPane
            {
                private static readonly int part = 1;

                private static VisualStyleElement normal;

                public static VisualStyleElement Normal => normal ??= new VisualStyleElement(className, part, 0);
            }

            public static class MorePrograms
            {
                private static readonly int part = 2;

                private static VisualStyleElement normal;

                public static VisualStyleElement Normal => normal ??= new VisualStyleElement(className, part, 0);
            }

            public static class MoreProgramsArrow
            {
                private static readonly int part = 3;

                private static VisualStyleElement normal;

                public static VisualStyleElement Normal => normal ??= new VisualStyleElement(className, part, 1);

                private static VisualStyleElement hot;

                public static VisualStyleElement Hot => hot ??= new VisualStyleElement(className, part, 2);

                private static VisualStyleElement pressed;

                public static VisualStyleElement Pressed => pressed ??= new VisualStyleElement(className, part, 3);
            }

            public static class ProgList
            {
                private static readonly int part = 4;

                private static VisualStyleElement normal;

                public static VisualStyleElement Normal => normal ??= new VisualStyleElement(className, part, 0);
            }

            public static class ProgListSeparator
            {
                private static readonly int part = 5;

                private static VisualStyleElement normal;

                public static VisualStyleElement Normal => normal ??= new VisualStyleElement(className, part, 0);
            }

            public static class PlaceList
            {
                private static readonly int part = 6;

                private static VisualStyleElement normal;

                public static VisualStyleElement Normal => normal ??= new VisualStyleElement(className, part, 0);
            }

            public static class PlaceListSeparator
            {
                private static readonly int part = 7;

                private static VisualStyleElement normal;

                public static VisualStyleElement Normal => normal ??= new VisualStyleElement(className, part, 0);
            }

            //The verb, not the noun.  Matches "Log Off" button.
            public static class LogOff
            {
                private static readonly int part = 8;

                private static VisualStyleElement normal;

                public static VisualStyleElement Normal => normal ??= new VisualStyleElement(className, part, 0);
            }

            //The verb, not the noun.  Matches "Log Off" button.
            public static class LogOffButtons
            {
                private static readonly int part = 9;

                private static VisualStyleElement normal;

                public static VisualStyleElement Normal => normal ??= new VisualStyleElement(className, part, 1);

                private static VisualStyleElement hot;

                public static VisualStyleElement Hot => hot ??= new VisualStyleElement(className, part, 2);

                private static VisualStyleElement pressed;

                public static VisualStyleElement Pressed => pressed ??= new VisualStyleElement(className, part, 3);
            }

            public static class UserPicture
            {
                private static readonly int part = 10;

                private static VisualStyleElement normal;

                public static VisualStyleElement Normal => normal ??= new VisualStyleElement(className, part, 0);
            }

            public static class Preview
            {
                private static readonly int part = 11;

                private static VisualStyleElement normal;

                public static VisualStyleElement Normal => normal ??= new VisualStyleElement(className, part, 0);
            }
        } // END STARTPANEL

        public static class Status
        {
            private static readonly string className = "STATUS";

            public static class Bar
            {
                private static VisualStyleElement normal;

                public static VisualStyleElement Normal => normal ??= new VisualStyleElement(className, 0, 0);
            }

            public static class Pane
            {
                private static readonly int part = 1;

                private static VisualStyleElement normal;

                public static VisualStyleElement Normal => normal ??= new VisualStyleElement(className, part, 0);
            }

            public static class GripperPane
            {
                private static readonly int part = 2;

                private static VisualStyleElement normal;

                public static VisualStyleElement Normal => normal ??= new VisualStyleElement(className, part, 0);
            }

            public static class Gripper
            {
                private static readonly int part = 3;

                private static VisualStyleElement normal;

                public static VisualStyleElement Normal => normal ??= new VisualStyleElement(className, part, 0);
            }
        } // END STATUS

        public static class TaskBand
        {
            private static readonly string className = "TASKBAND";

            public static class GroupCount
            {
                private static readonly int part = 1;

                private static VisualStyleElement normal;

                public static VisualStyleElement Normal => normal ??= new VisualStyleElement(className, part, 0);
            }

            public static class FlashButton
            {
                private static readonly int part = 2;

                private static VisualStyleElement normal;

                public static VisualStyleElement Normal => normal ??= new VisualStyleElement(className, part, 0);
            }

            public static class FlashButtonGroupMenu
            {
                private static readonly int part = 3;

                private static VisualStyleElement normal;

                public static VisualStyleElement Normal => normal ??= new VisualStyleElement(className, part, 0);
            }
        } // END TASKBAND

        public static class TaskbarClock
        {
            private static readonly string className = "CLOCK";

            public static class Time
            {
                private static readonly int part = 1;

                private static VisualStyleElement normal;

                public static VisualStyleElement Normal => normal ??= new VisualStyleElement(className, part, 1);
            }
        } // END TASKBARCLOCK

        public static class Taskbar
        {
            private static readonly string className = "TASKBAR";

            public static class BackgroundBottom
            {
                private static readonly int part = 1;

                private static VisualStyleElement normal;

                public static VisualStyleElement Normal => normal ??= new VisualStyleElement(className, part, 0);
            }

            public static class BackgroundRight
            {
                private static readonly int part = 2;

                private static VisualStyleElement normal;

                public static VisualStyleElement Normal => normal ??= new VisualStyleElement(className, part, 0);
            }

            public static class BackgroundTop
            {
                private static readonly int part = 3;

                private static VisualStyleElement normal;

                public static VisualStyleElement Normal => normal ??= new VisualStyleElement(className, part, 0);
            }

            public static class BackgroundLeft
            {
                private static readonly int part = 4;

                private static VisualStyleElement normal;

                public static VisualStyleElement Normal => normal ??= new VisualStyleElement(className, part, 0);
            }

            public static class SizingBarBottom
            {
                private static readonly int part = 5;

                private static VisualStyleElement normal;

                public static VisualStyleElement Normal => normal ??= new VisualStyleElement(className, part, 0);
            }

            public static class SizingBarRight
            {
                private static readonly int part = 6;

                private static VisualStyleElement normal;

                public static VisualStyleElement Normal => normal ??= new VisualStyleElement(className, part, 0);
            }

            public static class SizingBarTop
            {
                private static readonly int part = 7;

                private static VisualStyleElement normal;

                public static VisualStyleElement Normal => normal ??= new VisualStyleElement(className, part, 0);
            }

            public static class SizingBarLeft
            {
                private static readonly int part = 8;

                private static VisualStyleElement normal;

                public static VisualStyleElement Normal => normal ??= new VisualStyleElement(className, part, 0);
            }
        } // END TASKBAR

        public static class ToolBar
        {
            private static readonly string className = "TOOLBAR";

            //
            internal static class Bar
            {
                private static VisualStyleElement normal;

                public static VisualStyleElement Normal => normal ??= new VisualStyleElement(className, 0, 0);
            }

            public static class Button
            {
                private static readonly int part = 1;

                private static VisualStyleElement normal;

                public static VisualStyleElement Normal => normal ??= new VisualStyleElement(className, part, 1);

                private static VisualStyleElement hot;

                public static VisualStyleElement Hot => hot ??= new VisualStyleElement(className, part, 2);

                private static VisualStyleElement pressed;

                public static VisualStyleElement Pressed => pressed ??= new VisualStyleElement(className, part, 3);

                private static VisualStyleElement disabled;

                public static VisualStyleElement Disabled => disabled ??= new VisualStyleElement(className, part, 4);

                private static VisualStyleElement _checked;

                public static VisualStyleElement Checked => _checked ??= new VisualStyleElement(className, part, 5);

                private static VisualStyleElement hotchecked;

                public static VisualStyleElement HotChecked => hotchecked ??= new VisualStyleElement(className, part, 6);
            }

            public static class DropDownButton
            {
                private static readonly int part = 2;

                private static VisualStyleElement normal;

                public static VisualStyleElement Normal => normal ??= new VisualStyleElement(className, part, 1);

                private static VisualStyleElement hot;

                public static VisualStyleElement Hot => hot ??= new VisualStyleElement(className, part, 2);

                private static VisualStyleElement pressed;

                public static VisualStyleElement Pressed => pressed ??= new VisualStyleElement(className, part, 3);

                private static VisualStyleElement disabled;

                public static VisualStyleElement Disabled => disabled ??= new VisualStyleElement(className, part, 4);

                private static VisualStyleElement _checked;

                public static VisualStyleElement Checked => _checked ??= new VisualStyleElement(className, part, 5);

                private static VisualStyleElement hotchecked;

                public static VisualStyleElement HotChecked => hotchecked ??= new VisualStyleElement(className, part, 6);
            }

            public static class SplitButton
            {
                private static readonly int part = 3;

                private static VisualStyleElement normal;

                public static VisualStyleElement Normal => normal ??= new VisualStyleElement(className, part, 1);

                private static VisualStyleElement hot;

                public static VisualStyleElement Hot => hot ??= new VisualStyleElement(className, part, 2);

                private static VisualStyleElement pressed;

                public static VisualStyleElement Pressed => pressed ??= new VisualStyleElement(className, part, 3);

                private static VisualStyleElement disabled;

                public static VisualStyleElement Disabled => disabled ??= new VisualStyleElement(className, part, 4);

                private static VisualStyleElement _checked;

                public static VisualStyleElement Checked => _checked ??= new VisualStyleElement(className, part, 5);

                private static VisualStyleElement hotchecked;

                public static VisualStyleElement HotChecked => hotchecked ??= new VisualStyleElement(className, part, 6);
            }

            public static class SplitButtonDropDown
            {
                private static readonly int part = 4;

                private static VisualStyleElement normal;

                public static VisualStyleElement Normal => normal ??= new VisualStyleElement(className, part, 1);

                private static VisualStyleElement hot;

                public static VisualStyleElement Hot => hot ??= new VisualStyleElement(className, part, 2);

                private static VisualStyleElement pressed;

                public static VisualStyleElement Pressed => pressed ??= new VisualStyleElement(className, part, 3);

                private static VisualStyleElement disabled;

                public static VisualStyleElement Disabled => disabled ??= new VisualStyleElement(className, part, 4);

                private static VisualStyleElement _checked;

                public static VisualStyleElement Checked => _checked ??= new VisualStyleElement(className, part, 5);

                private static VisualStyleElement hotchecked;

                public static VisualStyleElement HotChecked => hotchecked ??= new VisualStyleElement(className, part, 6);
            }

            public static class SeparatorHorizontal
            {
                private static readonly int part = 5;

                private static VisualStyleElement normal;

                public static VisualStyleElement Normal => normal ??= new VisualStyleElement(className, part, 0);
            }

            public static class SeparatorVertical
            {
                private static readonly int part = 6;

                private static VisualStyleElement normal;

                public static VisualStyleElement Normal => normal ??= new VisualStyleElement(className, part, 0);
            }

        } // END TOOLBAR

        public static class ToolTip
        {
            private static readonly string className = "TOOLTIP";

            public static class Standard
            {
                private static readonly int part = 1;

                private static VisualStyleElement normal;

                public static VisualStyleElement Normal => normal ??= new VisualStyleElement(className, part, 1);

                private static VisualStyleElement link;

                public static VisualStyleElement Link => link ??= new VisualStyleElement(className, part, 2);
            }

            public static class StandardTitle
            {
                private static readonly int part = 2;

                private static VisualStyleElement normal;

                public static VisualStyleElement Normal => normal ??= new VisualStyleElement(className, part, 0);
            }

            public static class Balloon
            {
                private static readonly int part = 3;

                private static VisualStyleElement normal;

                public static VisualStyleElement Normal => normal ??= new VisualStyleElement(className, part, 1);

                private static VisualStyleElement link;

                public static VisualStyleElement Link => link ??= new VisualStyleElement(className, part, 2);
            }

            public static class BalloonTitle
            {
                private static readonly int part = 4;

                private static VisualStyleElement normal;

                public static VisualStyleElement Normal => normal ??= new VisualStyleElement(className, part, 0);
            }

            public static class Close
            {
                private static readonly int part = 5;

                private static VisualStyleElement normal;

                public static VisualStyleElement Normal => normal ??= new VisualStyleElement(className, part, 1);

                private static VisualStyleElement hot;

                public static VisualStyleElement Hot => hot ??= new VisualStyleElement(className, part, 2);

                private static VisualStyleElement pressed;

                public static VisualStyleElement Pressed => pressed ??= new VisualStyleElement(className, part, 3);
            }
        } // END TOOLTIP

        public static class TrackBar
        {
            private static readonly string className = "TRACKBAR";

            public static class Track
            {
                private static readonly int part = 1;

                private static VisualStyleElement normal;

                public static VisualStyleElement Normal => normal ??= new VisualStyleElement(className, part, 1);
            }

            public static class TrackVertical
            {
                private static readonly int part = 2;

                private static VisualStyleElement normal;

                public static VisualStyleElement Normal => normal ??= new VisualStyleElement(className, part, 1);
            }

            public static class Thumb
            {
                private static readonly int part = 3;

                private static VisualStyleElement normal;

                public static VisualStyleElement Normal => normal ??= new VisualStyleElement(className, part, 1);

                private static VisualStyleElement hot;

                public static VisualStyleElement Hot => hot ??= new VisualStyleElement(className, part, 2);

                private static VisualStyleElement pressed;

                public static VisualStyleElement Pressed => pressed ??= new VisualStyleElement(className, part, 3);

                private static VisualStyleElement focused;

                public static VisualStyleElement Focused => focused ??= new VisualStyleElement(className, part, 4);

                private static VisualStyleElement disabled;

                public static VisualStyleElement Disabled => disabled ??= new VisualStyleElement(className, part, 5);
            }

            public static class ThumbBottom
            {
                private static readonly int part = 4;

                private static VisualStyleElement normal;

                public static VisualStyleElement Normal => normal ??= new VisualStyleElement(className, part, 1);

                private static VisualStyleElement hot;

                public static VisualStyleElement Hot => hot ??= new VisualStyleElement(className, part, 2);

                private static VisualStyleElement pressed;

                public static VisualStyleElement Pressed => pressed ??= new VisualStyleElement(className, part, 3);

                private static VisualStyleElement focused;

                public static VisualStyleElement Focused => focused ??= new VisualStyleElement(className, part, 4);

                private static VisualStyleElement disabled;

                public static VisualStyleElement Disabled => disabled ??= new VisualStyleElement(className, part, 5);
            }

            public static class ThumbTop
            {
                private static readonly int part = 5;

                private static VisualStyleElement normal;

                public static VisualStyleElement Normal => normal ??= new VisualStyleElement(className, part, 1);

                private static VisualStyleElement hot;

                public static VisualStyleElement Hot => hot ??= new VisualStyleElement(className, part, 2);

                private static VisualStyleElement pressed;

                public static VisualStyleElement Pressed => pressed ??= new VisualStyleElement(className, part, 3);

                private static VisualStyleElement focused;

                public static VisualStyleElement Focused => focused ??= new VisualStyleElement(className, part, 4);

                private static VisualStyleElement disabled;

                public static VisualStyleElement Disabled => disabled ??= new VisualStyleElement(className, part, 5);
            }

            public static class ThumbVertical
            {
                private static readonly int part = 6;

                private static VisualStyleElement normal;

                public static VisualStyleElement Normal => normal ??= new VisualStyleElement(className, part, 1);

                private static VisualStyleElement hot;

                public static VisualStyleElement Hot => hot ??= new VisualStyleElement(className, part, 2);

                private static VisualStyleElement pressed;

                public static VisualStyleElement Pressed => pressed ??= new VisualStyleElement(className, part, 3);

                private static VisualStyleElement focused;

                public static VisualStyleElement Focused => focused ??= new VisualStyleElement(className, part, 4);

                private static VisualStyleElement disabled;

                public static VisualStyleElement Disabled => disabled ??= new VisualStyleElement(className, part, 5);
            }

            public static class ThumbLeft
            {
                private static readonly int part = 7;

                private static VisualStyleElement normal;

                public static VisualStyleElement Normal => normal ??= new VisualStyleElement(className, part, 1);

                private static VisualStyleElement hot;

                public static VisualStyleElement Hot => hot ??= new VisualStyleElement(className, part, 2);

                private static VisualStyleElement pressed;

                public static VisualStyleElement Pressed => pressed ??= new VisualStyleElement(className, part, 3);

                private static VisualStyleElement focused;

                public static VisualStyleElement Focused => focused ??= new VisualStyleElement(className, part, 4);

                private static VisualStyleElement disabled;

                public static VisualStyleElement Disabled => disabled ??= new VisualStyleElement(className, part, 5);
            }

            public static class ThumbRight
            {
                private static readonly int part = 8;

                private static VisualStyleElement normal;

                public static VisualStyleElement Normal => normal ??= new VisualStyleElement(className, part, 1);

                private static VisualStyleElement hot;

                public static VisualStyleElement Hot => hot ??= new VisualStyleElement(className, part, 2);

                private static VisualStyleElement pressed;

                public static VisualStyleElement Pressed => pressed ??= new VisualStyleElement(className, part, 3);

                private static VisualStyleElement focused;

                public static VisualStyleElement Focused => focused ??= new VisualStyleElement(className, part, 4);

                private static VisualStyleElement disabled;

                public static VisualStyleElement Disabled => disabled ??= new VisualStyleElement(className, part, 5);
            }

            public static class Ticks
            {
                private static readonly int part = 9;

                private static VisualStyleElement normal;

                public static VisualStyleElement Normal => normal ??= new VisualStyleElement(className, part, 1);
            }

            public static class TicksVertical
            {
                private static readonly int part = 10;

                private static VisualStyleElement normal;

                public static VisualStyleElement Normal => normal ??= new VisualStyleElement(className, part, 1);
            }
        } // END TRACKBAR

        public static class TreeView
        {
            private static readonly string className = "TREEVIEW";

            public static class Item
            {
                private static readonly int part = 1;

                private static VisualStyleElement normal;

                public static VisualStyleElement Normal => normal ??= new VisualStyleElement(className, part, 1);

                private static VisualStyleElement hot;

                public static VisualStyleElement Hot => hot ??= new VisualStyleElement(className, part, 2);

                private static VisualStyleElement selected;

                public static VisualStyleElement Selected => selected ??= new VisualStyleElement(className, part, 3);

                private static VisualStyleElement disabled;

                public static VisualStyleElement Disabled => disabled ??= new VisualStyleElement(className, part, 4);

                private static VisualStyleElement selectednotfocus;

                public static VisualStyleElement SelectedNotFocus => selectednotfocus ??= new VisualStyleElement(className, part, 5);
            }

            public static class Glyph
            {
                private static readonly int part = 2;

                private static VisualStyleElement closed;

                public static VisualStyleElement Closed => closed ??= new VisualStyleElement(className, part, 1);

                private static VisualStyleElement opened;

                public static VisualStyleElement Opened => opened ??= new VisualStyleElement(className, part, 2);
            }

            public static class Branch
            {
                private static readonly int part = 3;

                private static VisualStyleElement normal;

                public static VisualStyleElement Normal => normal ??= new VisualStyleElement(className, part, 0);
            }
        } // END TREEVIEW

        internal static class ExplorerTreeView
        {
            private static readonly string className = "Explorer::TreeView";

            public static class Glyph
            {
                private static readonly int part = 2;

                private static VisualStyleElement closed;

                public static VisualStyleElement Closed => closed ??= new VisualStyleElement(className, part, 1);

                private static VisualStyleElement opened;

                public static VisualStyleElement Opened => opened ??= new VisualStyleElement(className, part, 2);
            }
        } // END Explorer::Tree

        public static class TextBox
        {
            private static readonly string className = "EDIT";

            public static class TextEdit
            {
                private static readonly int part = 1;

                private static VisualStyleElement normal;

                public static VisualStyleElement Normal => normal ??= new VisualStyleElement(className, part, 1);

                private static VisualStyleElement hot;

                public static VisualStyleElement Hot => hot ??= new VisualStyleElement(className, part, 2);

                private static VisualStyleElement selected;

                public static VisualStyleElement Selected => selected ??= new VisualStyleElement(className, part, 3);

                private static VisualStyleElement disabled;

                public static VisualStyleElement Disabled => disabled ??= new VisualStyleElement(className, part, 4);

                private static VisualStyleElement focused;

                public static VisualStyleElement Focused => focused ??= new VisualStyleElement(className, part, 5);

                private static VisualStyleElement _readonly;

                public static VisualStyleElement ReadOnly => _readonly ??= new VisualStyleElement(className, part, 6);

                private static VisualStyleElement assist;

                public static VisualStyleElement Assist => assist ??= new VisualStyleElement(className, part, 7);
            }

            public static class Caret
            {
                private static readonly int part = 2;

                private static VisualStyleElement normal;

                public static VisualStyleElement Normal => normal ??= new VisualStyleElement(className, part, 0);
            }
        } // END TEXTBOX

        public static class TrayNotify
        {
            private static readonly string className = "TRAYNOTIFY";

            public static class Background
            {
                private static readonly int part = 1;

                private static VisualStyleElement normal;

                public static VisualStyleElement Normal => normal ??= new VisualStyleElement(className, part, 0);
            }

            public static class AnimateBackground
            {
                private static readonly int part = 2;

                private static VisualStyleElement normal;

                public static VisualStyleElement Normal => normal ??= new VisualStyleElement(className, part, 0);
            }
        } // END TRAYNOTIFY

        public static class Window
        {
            private static readonly string className = "WINDOW";

            public static class Caption
            {
                private static readonly int part = 1;

                private static VisualStyleElement active;

                public static VisualStyleElement Active => active ??= new VisualStyleElement(className, part, 1);

                private static VisualStyleElement inactive;

                public static VisualStyleElement Inactive => inactive ??= new VisualStyleElement(className, part, 2);

                private static VisualStyleElement disabled;

                public static VisualStyleElement Disabled => disabled ??= new VisualStyleElement(className, part, 3);
            }

            public static class SmallCaption
            {
                private static readonly int part = 2;

                private static VisualStyleElement active;

                public static VisualStyleElement Active => active ??= new VisualStyleElement(className, part, 1);

                private static VisualStyleElement inactive;

                public static VisualStyleElement Inactive => inactive ??= new VisualStyleElement(className, part, 2);

                private static VisualStyleElement disabled;

                public static VisualStyleElement Disabled => disabled ??= new VisualStyleElement(className, part, 3);
            }

            public static class MinCaption
            {
                private static readonly int part = 3;

                private static VisualStyleElement active;

                public static VisualStyleElement Active => active ??= new VisualStyleElement(className, part, 1);

                private static VisualStyleElement inactive;

                public static VisualStyleElement Inactive => inactive ??= new VisualStyleElement(className, part, 2);

                private static VisualStyleElement disabled;

                public static VisualStyleElement Disabled => disabled ??= new VisualStyleElement(className, part, 3);
            }

            public static class SmallMinCaption
            {
                private static readonly int part = 4;

                private static VisualStyleElement active;

                public static VisualStyleElement Active => active ??= new VisualStyleElement(className, part, 1);

                private static VisualStyleElement inactive;

                public static VisualStyleElement Inactive => inactive ??= new VisualStyleElement(className, part, 2);

                private static VisualStyleElement disabled;

                public static VisualStyleElement Disabled => disabled ??= new VisualStyleElement(className, part, 3);
            }

            public static class MaxCaption
            {
                private static readonly int part = 5;

                private static VisualStyleElement active;

                public static VisualStyleElement Active => active ??= new VisualStyleElement(className, part, 1);

                private static VisualStyleElement inactive;

                public static VisualStyleElement Inactive => inactive ??= new VisualStyleElement(className, part, 2);

                private static VisualStyleElement disabled;

                public static VisualStyleElement Disabled => disabled ??= new VisualStyleElement(className, part, 3);
            }

            public static class SmallMaxCaption
            {
                private static readonly int part = 6;

                private static VisualStyleElement active;

                public static VisualStyleElement Active => active ??= new VisualStyleElement(className, part, 1);

                private static VisualStyleElement inactive;

                public static VisualStyleElement Inactive => inactive ??= new VisualStyleElement(className, part, 2);

                private static VisualStyleElement disabled;

                public static VisualStyleElement Disabled => disabled ??= new VisualStyleElement(className, part, 3);
            }

            public static class FrameLeft
            {
                private static readonly int part = 7;

                private static VisualStyleElement active;

                public static VisualStyleElement Active => active ??= new VisualStyleElement(className, part, 1);

                private static VisualStyleElement inactive;

                public static VisualStyleElement Inactive => inactive ??= new VisualStyleElement(className, part, 2);
            }

            public static class FrameRight
            {
                private static readonly int part = 8;

                private static VisualStyleElement active;

                public static VisualStyleElement Active => active ??= new VisualStyleElement(className, part, 1);

                private static VisualStyleElement inactive;

                public static VisualStyleElement Inactive => inactive ??= new VisualStyleElement(className, part, 2);
            }

            public static class FrameBottom
            {
                private static readonly int part = 9;

                private static VisualStyleElement active;

                public static VisualStyleElement Active => active ??= new VisualStyleElement(className, part, 1);

                private static VisualStyleElement inactive;

                public static VisualStyleElement Inactive => inactive ??= new VisualStyleElement(className, part, 2);
            }

            public static class SmallFrameLeft
            {
                private static readonly int part = 10;

                private static VisualStyleElement active;

                public static VisualStyleElement Active => active ??= new VisualStyleElement(className, part, 1);

                private static VisualStyleElement inactive;

                public static VisualStyleElement Inactive => inactive ??= new VisualStyleElement(className, part, 2);
            }

            public static class SmallFrameRight
            {
                private static readonly int part = 11;

                private static VisualStyleElement active;

                public static VisualStyleElement Active => active ??= new VisualStyleElement(className, part, 1);

                private static VisualStyleElement inactive;

                public static VisualStyleElement Inactive => inactive ??= new VisualStyleElement(className, part, 2);
            }

            public static class SmallFrameBottom
            {
                private static readonly int part = 12;

                private static VisualStyleElement active;

                public static VisualStyleElement Active => active ??= new VisualStyleElement(className, part, 1);

                private static VisualStyleElement inactive;

                public static VisualStyleElement Inactive => inactive ??= new VisualStyleElement(className, part, 2);
            }

            public static class SysButton
            {
                private static readonly int part = 13;

                private static VisualStyleElement normal;

                public static VisualStyleElement Normal => normal ??= new VisualStyleElement(className, part, 1);

                private static VisualStyleElement hot;

                public static VisualStyleElement Hot => hot ??= new VisualStyleElement(className, part, 2);

                private static VisualStyleElement pressed;

                public static VisualStyleElement Pressed => pressed ??= new VisualStyleElement(className, part, 3);

                private static VisualStyleElement disabled;

                public static VisualStyleElement Disabled => disabled ??= new VisualStyleElement(className, part, 4);
            }

            public static class MdiSysButton
            {
                private static readonly int part = 14;

                private static VisualStyleElement normal;

                public static VisualStyleElement Normal => normal ??= new VisualStyleElement(className, part, 1);

                private static VisualStyleElement hot;

                public static VisualStyleElement Hot => hot ??= new VisualStyleElement(className, part, 2);

                private static VisualStyleElement pressed;

                public static VisualStyleElement Pressed => pressed ??= new VisualStyleElement(className, part, 3);

                private static VisualStyleElement disabled;

                public static VisualStyleElement Disabled => disabled ??= new VisualStyleElement(className, part, 4);
            }

            public static class MinButton
            {
                private static readonly int part = 15;

                private static VisualStyleElement normal;

                public static VisualStyleElement Normal => normal ??= new VisualStyleElement(className, part, 1);

                private static VisualStyleElement hot;

                public static VisualStyleElement Hot => hot ??= new VisualStyleElement(className, part, 2);

                private static VisualStyleElement pressed;

                public static VisualStyleElement Pressed => pressed ??= new VisualStyleElement(className, part, 3);

                private static VisualStyleElement disabled;

                public static VisualStyleElement Disabled => disabled ??= new VisualStyleElement(className, part, 4);
            }

            public static class MdiMinButton
            {
                private static readonly int part = 16;

                private static VisualStyleElement normal;

                public static VisualStyleElement Normal => normal ??= new VisualStyleElement(className, part, 1);

                private static VisualStyleElement hot;

                public static VisualStyleElement Hot => hot ??= new VisualStyleElement(className, part, 2);

                private static VisualStyleElement pressed;

                public static VisualStyleElement Pressed => pressed ??= new VisualStyleElement(className, part, 3);

                private static VisualStyleElement disabled;

                public static VisualStyleElement Disabled => disabled ??= new VisualStyleElement(className, part, 4);
            }

            public static class MaxButton
            {
                private static readonly int part = 17;

                private static VisualStyleElement normal;

                public static VisualStyleElement Normal => normal ??= new VisualStyleElement(className, part, 1);

                private static VisualStyleElement hot;

                public static VisualStyleElement Hot => hot ??= new VisualStyleElement(className, part, 2);

                private static VisualStyleElement pressed;

                public static VisualStyleElement Pressed => pressed ??= new VisualStyleElement(className, part, 3);

                private static VisualStyleElement disabled;

                public static VisualStyleElement Disabled => disabled ??= new VisualStyleElement(className, part, 4);
            }

            public static class CloseButton
            {
                private static readonly int part = 18;

                private static VisualStyleElement normal;

                public static VisualStyleElement Normal => normal ??= new VisualStyleElement(className, part, 1);

                private static VisualStyleElement hot;

                public static VisualStyleElement Hot => hot ??= new VisualStyleElement(className, part, 2);

                private static VisualStyleElement pressed;

                public static VisualStyleElement Pressed => pressed ??= new VisualStyleElement(className, part, 3);

                private static VisualStyleElement disabled;

                public static VisualStyleElement Disabled => disabled ??= new VisualStyleElement(className, part, 4);
            }

            public static class SmallCloseButton
            {
                private static readonly int part = 19;

                private static VisualStyleElement normal;

                public static VisualStyleElement Normal => normal ??= new VisualStyleElement(className, part, 1);

                private static VisualStyleElement hot;

                public static VisualStyleElement Hot => hot ??= new VisualStyleElement(className, part, 2);

                private static VisualStyleElement pressed;

                public static VisualStyleElement Pressed => pressed ??= new VisualStyleElement(className, part, 3);

                private static VisualStyleElement disabled;

                public static VisualStyleElement Disabled => disabled ??= new VisualStyleElement(className, part, 4);
            }

            public static class MdiCloseButton
            {
                private static readonly int part = 20;

                private static VisualStyleElement normal;

                public static VisualStyleElement Normal => normal ??= new VisualStyleElement(className, part, 1);

                private static VisualStyleElement hot;

                public static VisualStyleElement Hot => hot ??= new VisualStyleElement(className, part, 2);

                private static VisualStyleElement pressed;

                public static VisualStyleElement Pressed => pressed ??= new VisualStyleElement(className, part, 3);

                private static VisualStyleElement disabled;

                public static VisualStyleElement Disabled => disabled ??= new VisualStyleElement(className, part, 4);
            }

            public static class RestoreButton
            {
                private static readonly int part = 21;

                private static VisualStyleElement normal;

                public static VisualStyleElement Normal => normal ??= new VisualStyleElement(className, part, 1);

                private static VisualStyleElement hot;

                public static VisualStyleElement Hot => hot ??= new VisualStyleElement(className, part, 2);

                private static VisualStyleElement pressed;

                public static VisualStyleElement Pressed => pressed ??= new VisualStyleElement(className, part, 3);

                private static VisualStyleElement disabled;

                public static VisualStyleElement Disabled => disabled ??= new VisualStyleElement(className, part, 4);
            }

            public static class MdiRestoreButton
            {
                private static readonly int part = 22;

                private static VisualStyleElement normal;

                public static VisualStyleElement Normal => normal ??= new VisualStyleElement(className, part, 1);

                private static VisualStyleElement hot;

                public static VisualStyleElement Hot => hot ??= new VisualStyleElement(className, part, 2);

                private static VisualStyleElement pressed;

                public static VisualStyleElement Pressed => pressed ??= new VisualStyleElement(className, part, 3);

                private static VisualStyleElement disabled;

                public static VisualStyleElement Disabled => disabled ??= new VisualStyleElement(className, part, 4);
            }

            public static class HelpButton
            {
                private static readonly int part = 23;

                private static VisualStyleElement normal;

                public static VisualStyleElement Normal => normal ??= new VisualStyleElement(className, part, 1);

                private static VisualStyleElement hot;

                public static VisualStyleElement Hot => hot ??= new VisualStyleElement(className, part, 2);

                private static VisualStyleElement pressed;

                public static VisualStyleElement Pressed => pressed ??= new VisualStyleElement(className, part, 3);

                private static VisualStyleElement disabled;

                public static VisualStyleElement Disabled => disabled ??= new VisualStyleElement(className, part, 4);
            }

            public static class MdiHelpButton
            {
                private static readonly int part = 24;

                private static VisualStyleElement normal;

                public static VisualStyleElement Normal => normal ??= new VisualStyleElement(className, part, 1);

                private static VisualStyleElement hot;

                public static VisualStyleElement Hot => hot ??= new VisualStyleElement(className, part, 2);

                private static VisualStyleElement pressed;

                public static VisualStyleElement Pressed => pressed ??= new VisualStyleElement(className, part, 3);

                private static VisualStyleElement disabled;

                public static VisualStyleElement Disabled => disabled ??= new VisualStyleElement(className, part, 4);
            }

            public static class HorizontalScroll
            {
                private static readonly int part = 25;

                private static VisualStyleElement normal;

                public static VisualStyleElement Normal => normal ??= new VisualStyleElement(className, part, 1);

                private static VisualStyleElement hot;

                public static VisualStyleElement Hot => hot ??= new VisualStyleElement(className, part, 2);

                private static VisualStyleElement pressed;

                public static VisualStyleElement Pressed => pressed ??= new VisualStyleElement(className, part, 3);

                private static VisualStyleElement disabled;

                public static VisualStyleElement Disabled => disabled ??= new VisualStyleElement(className, part, 4);
            }

            public static class HorizontalThumb
            {
                private static readonly int part = 26;

                private static VisualStyleElement normal;

                public static VisualStyleElement Normal => normal ??= new VisualStyleElement(className, part, 1);

                private static VisualStyleElement hot;

                public static VisualStyleElement Hot => hot ??= new VisualStyleElement(className, part, 2);

                private static VisualStyleElement pressed;

                public static VisualStyleElement Pressed => pressed ??= new VisualStyleElement(className, part, 3);

                private static VisualStyleElement disabled;

                public static VisualStyleElement Disabled => disabled ??= new VisualStyleElement(className, part, 4);
            }

            public static class VerticalScroll
            {
                private static readonly int part = 27;

                private static VisualStyleElement normal;

                public static VisualStyleElement Normal => normal ??= new VisualStyleElement(className, part, 1);

                private static VisualStyleElement hot;

                public static VisualStyleElement Hot => hot ??= new VisualStyleElement(className, part, 2);

                private static VisualStyleElement pressed;

                public static VisualStyleElement Pressed => pressed ??= new VisualStyleElement(className, part, 3);

                private static VisualStyleElement disabled;

                public static VisualStyleElement Disabled => disabled ??= new VisualStyleElement(className, part, 4);
            }

            public static class VerticalThumb
            {
                private static readonly int part = 28;

                private static VisualStyleElement normal;

                public static VisualStyleElement Normal => normal ??= new VisualStyleElement(className, part, 1);

                private static VisualStyleElement hot;

                public static VisualStyleElement Hot => hot ??= new VisualStyleElement(className, part, 2);

                private static VisualStyleElement pressed;

                public static VisualStyleElement Pressed => pressed ??= new VisualStyleElement(className, part, 3);

                private static VisualStyleElement disabled;

                public static VisualStyleElement Disabled => disabled ??= new VisualStyleElement(className, part, 4);
            }

            public static class Dialog
            {
                private static readonly int part = 29;

                private static VisualStyleElement normal;

                public static VisualStyleElement Normal => normal ??= new VisualStyleElement(className, part, 0);
            }

            public static class CaptionSizingTemplate
            {
                private static readonly int part = 30;

                private static VisualStyleElement normal;

                public static VisualStyleElement Normal => normal ??= new VisualStyleElement(className, part, 0);
            }

            public static class SmallCaptionSizingTemplate
            {
                private static readonly int part = 31;

                private static VisualStyleElement normal;

                public static VisualStyleElement Normal => normal ??= new VisualStyleElement(className, part, 0);
            }

            public static class FrameLeftSizingTemplate
            {
                private static readonly int part = 32;

                private static VisualStyleElement normal;

                public static VisualStyleElement Normal => normal ??= new VisualStyleElement(className, part, 0);
            }

            public static class SmallFrameLeftSizingTemplate
            {
                private static readonly int part = 33;

                private static VisualStyleElement normal;

                public static VisualStyleElement Normal => normal ??= new VisualStyleElement(className, part, 0);
            }

            //Not used as compound word here
            public static class FrameRightSizingTemplate
            {
                private static readonly int part = 34;

                private static VisualStyleElement normal;

                public static VisualStyleElement Normal => normal ??= new VisualStyleElement(className, part, 0);
            }

            //Not used as compound word here
            public static class SmallFrameRightSizingTemplate
            {
                private static readonly int part = 35;

                private static VisualStyleElement normal;

                public static VisualStyleElement Normal => normal ??= new VisualStyleElement(className, part, 0);
            }

            public static class FrameBottomSizingTemplate
            {
                private static readonly int part = 36;

                private static VisualStyleElement normal;

                public static VisualStyleElement Normal => normal ??= new VisualStyleElement(className, part, 0);
            }

            public static class SmallFrameBottomSizingTemplate
            {
                private static readonly int part = 37;

                private static VisualStyleElement normal;

                public static VisualStyleElement Normal => normal ??= new VisualStyleElement(className, part, 0);
            }
        } // END WINDOW
    }
}
