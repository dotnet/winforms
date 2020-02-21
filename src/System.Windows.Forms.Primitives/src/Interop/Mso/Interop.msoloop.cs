// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

internal partial class Interop
{
    internal partial class Mso
    {
        /// <summary>
        ///  Reasons for pushing a message loop as passed to <see cref="IMsoComponentManager.FPushMessageLoop" />.
        ///  The host should remain in message loop until <see cref="IMsoComponent.FContinueMessageLoop" />
        ///  returns <see cref="BOOL.FALSE" />
        /// </summary>
        public enum msoloop : uint
        {
            FocusWait    = 1,  // component is activating host
            DoEvents     = 2,  // component is asking host to process messages
            Debug        = 3,  // component has entered debug mode
            ModalForm    = 4,  // component is displaying a modal form
            ModalAlert   = 5,  // Different from ModalForm in the intention that
                               // this should act as much like a blocking call as
                               // as possible- app should do no idling in this case
                               // if alerts might come up in badly defined states

            // Unoffical msoloop messages
            DoEventsModal = unchecked((uint)(-2)),
            Main = unchecked((uint)(-1))
        }
    }
}
