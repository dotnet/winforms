// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

internal static partial class Interop
{
    internal static partial class Mso
    {
        /// <summary>
        ///  State IDs passed to <see cref="IMsoComponent.OnEnterState" /> and
        ///  <see cref="IMsoComponentManager.OnComponentEnterState" />
        ///
        ///  When the host or a component is notified through one of these methods that another
        ///  entity(component or host) is entering or exiting a state identified by one of these
        ///  state IDs, the host/component should take appropriate action.
        /// </summary>
        public enum msocstate : uint
        {
            /// <summary>
            ///  If app is entering modal state, host/component should disable
            ///  its toplevel windows, and reenable them when app exits this
            ///  state.  Also, when this state is entered or exited, host/component
            ///  should notify approprate inplace objects via
            ///  IOleInPlaceActiveObject::EnableModeless.
            /// </summary>
            Modal = 1,

            /// <summary>
            ///  If app is entering redraw fff state, host/component should disable
            ///  repainting of its windows, and reenable repainting when app exits
            ///  this state.
            /// </summary>
            RedrawOff = 2,

            /// <summary>
            ///  If app is entering warnings off state, host/component should disable
            ///  the presentation of any user warnings, and reenable this when
            ///  app exits this state.
            /// </summary>
            WarningsOff = 3,

            /// <summary>
            ///  Used to notify host/component when Recording is turned on or off.
            /// </summary>
            Recording = 4
        }
    }
}
