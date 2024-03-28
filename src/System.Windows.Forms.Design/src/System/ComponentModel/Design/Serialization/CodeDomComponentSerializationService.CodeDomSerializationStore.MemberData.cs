// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.ComponentModel.Design.Serialization;

public sealed partial class CodeDomComponentSerializationService
{
    private sealed partial class CodeDomSerializationStore
    {
        /// <summary>
        ///  We create one of these for each specific member on an object.
        /// </summary>
        private class MemberData
        {
            /// <summary>
            ///  The member we're serializing.
            /// </summary>
            internal readonly MemberDescriptor _member;

            /// <summary>
            ///  True if we should try to serialize values that contain their defaults as well.
            /// </summary>
            internal readonly bool _absolute;

            /// <summary>
            ///  Creates a new member data ready to be serialized.
            /// </summary>
            internal MemberData(MemberDescriptor member, bool absolute)
            {
                _member = member;
                _absolute = absolute;
            }
        }
    }
}
