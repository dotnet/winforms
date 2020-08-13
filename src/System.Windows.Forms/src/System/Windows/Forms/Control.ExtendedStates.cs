// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    public partial class Control
    {
        [Flags]
        private protected enum ExtendedStates
        {
            /// <summary>
            ///  When we change RightToLeft, we need to change the scrollbar thumb. We can't do that until after the
            ///  control has been created, and all the items added back. This is because the system control won't know
            ///  the nMin and nMax of the scroll bar until the items are added. So in RightToLeftChanged, we set a
            ///  flag that indicates that we want to set the scroll position. In OnHandleCreated we check this flag,
            ///  and if set, we BeginInvoke. We have to BeginInvoke since we have to wait until the items are added.
            ///  We only want to do this when RightToLeft changes thus the flags HAVEINVOKED and SETSCROLLPOS.
            ///  Otherwise we would do this on each HandleCreated.
            /// </summary>
            HaveInvoked = 0x00000001,

            SetScrollPosition = 0x00000002,

            /// <summary>
            ///  Set when the control is listening to SystemEvents.UserPreferenceChanged.
            /// </summary>
            ListeningToUserPreferenceChanged = 0x00000004,

            /// <summary>
            ///  If set, the control will listen to SystemEvents.UserPreferenceChanged when TopLevel is true and handle is created.
            /// </summary>
            InterestedInUserPreferenceChanged = 0x00000008,

            /// <summary>
            ///  If set, the control DOES NOT necessarily take capture on MouseDown
            /// </summary>
            MaintainsOwnCaptureMode = 0x00000010,

            /// <summary>
            ///  Set to true by ContainerControl when this control is becoming its active control
            /// </summary>
            BecomingActiveControl = 0x00000020,

            /// <summary>
            ///  If set, the next time PerformLayout is called, cachedLayoutEventArg will be cleared.
            /// </summary>
            ClearLayoutArgs = 0x00000040,

            InputKey = 0x00000080,

            InputChar = 0x00000100,

            UiCues = 0x00000200,

            IsActiveX = 0x00000400,

            UserPreferredSizeCache = 0x00000800,

            TopMDIWindowClosing = 0x00001000,

            /// <summary>
            ///  If set, the control is being scaled currently
            /// </summary>
            CurrentlyBeingScaled = 0x00002000
        }
    }
}
