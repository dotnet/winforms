// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.ComponentModel.Design.Serialization;

public sealed partial class CodeDomComponentSerializationService
{
    private sealed partial class CodeDomSerializationStore
    {
        /// <summary>
        ///  We create one of these for each object we process.
        /// </summary>
        private class ObjectData
        {
            private bool _entireObject;
            private List<MemberData>? _members;

            /// <summary>
            ///  The object value we're serializing.
            /// </summary>
            internal readonly object _value;

            public ObjectData(string name, object value)
            {
                _value = value;
                _name = name;
            }

            /// <summary>
            ///  The name of the object we're serializing.
            /// </summary>
            internal readonly string _name;

            /// <summary>
            ///  If true, the entire object should be serialized. If false, only the members in the member list should be serialized.
            /// </summary>
            internal bool EntireObject
            {
                get => _entireObject;
                set
                {
                    if (value && _members is not null)
                    {
                        _members.Clear();
                    }

                    _entireObject = value;
                }
            }

            /// <summary>
            ///  If <see langword="true"/>, the object should be serialized such that during deserialization
            ///  to an existing object the object is reconstructed entirely.
            ///  If <see langword="false"/>, serialize normally
            /// </summary>
            internal bool Absolute { get; set; }

            /// <summary>
            ///  A list of MemberData objects representing specific members that should be serialized.
            /// </summary>
            internal IList<MemberData> Members => _members ??= [];
        }

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
