// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.ComponentModel.Design.Serialization
{
    public abstract partial class BasicDesignerLoader
    {
        /// <summary>
        ///  A list of flags that indicate rules to apply when requesting 
        ///  that the designer reload itself.
        /// </summary>
        [Flags]
        protected enum ReloadOptions
        {
            /// <summary>
            ///  Peform the default behavior.
            /// </summary>
            Default = 0x00,

            /// <summary>
            ///  If this flag is set, any error encoutered during the
            ///  reload will automatically put the designer loader in
            ///  the modified state.
            /// </summary>
            ModifyOnError = 0x01,

            /// <summary>
            ///  If this flag is set, a reload will occur.  If the
            ///  flag is not set a reload will only occur if the
            ///  IsReloadNeeded method returns true.
            /// </summary>
            Force = 0x02,

            /// <summary>
            ///  If this flag is set, any pending changes in the
            ///  designer will be abandonded.  If this flag is not
            ///  set, designer changes will be flushed through the
            ///  designer loader before reloading the design surface.
            /// </summary>
            NoFlush = 0x04,
        }
    }
}

