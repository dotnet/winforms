// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.ComponentModel.Design.Serialization
{
    /// <summary>
    ///  The ComponentSerializationService supports "absolute" serialization,
    ///  where instead of just serializing values that differ from an object's
    // default values, all values are serialized in such a way as to be able
    ///  to reset values to their defaults for objects that may have already
    ///  been initialized. When a component serialization service wishes to
    ///  indicate this to CodeDomSerializer objects, it will place a
    ///  SerializeAbsoluteContext on the context stack. The member in this
    ///  context may be null, to indicate that all members are serialized, or
    ///  a member indicating that only a specific member is being serialized at
    ///  this time.
    /// </summary>
    public sealed class SerializeAbsoluteContext
    {
        /// <summary>
        ///  Creeates a new SerializeAbsoluteContext. Member can be null or
        ///  omitted to indicate this context should be used for all members.
        /// </summary>
        public SerializeAbsoluteContext()
        {
        }

        /// <summary>
        ///  Creeates a new SerializeAbsoluteContext. Member can be null or
        ///  omitted to indicate this context should be used for all members.
        /// </summary>
        public SerializeAbsoluteContext(MemberDescriptor member)
        {
            Member = member;
        }

        /// <summary>
        ///  This property returns the member this context is bound to. It may be null to indicate the context is bound to all members of an object.
        /// </summary>
        public MemberDescriptor Member { get; }

        /// <summary>
        ///  Returns true if the given member should be serialized in this context.
        /// </summary>
        public bool ShouldSerialize(MemberDescriptor member)
        {
            return Member == null || Member == member;
        }
    }
}
