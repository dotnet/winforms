// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    public partial class Control
    {
        /// <summary>
        ///  Contains a single ambient property, including DISPID, name and value.
        /// </summary>
        private class AmbientProperty
        {
            private object _value;

            /// <summary>
            ///  Creates a new, empty ambient property.
            ///</summary>
            internal AmbientProperty(string name, int dispID)
            {
                Name = name;
                DispID = dispID;
                _value = null;
                Empty = true;
            }

            /// <summary>
            ///  The windows forms property name.
            /// </summary>
            internal string Name { get; }

            /// <summary>
            ///  The DispID for the property.
            /// </summary>
            internal int DispID { get; }

            /// <summary>
            ///  Returns true if this property has not been set.
            /// </summary>
            internal bool Empty { get; private set; }

            /// <summary>
            ///  The current value of the property.
            /// </summary>
            internal object Value
            {
                get
                {
                    return _value;
                }
                set
                {
                    _value = value;
                    Empty = false;
                }
            }

            /// <summary>
            ///  Resets the property.
            /// </summary>
            internal void ResetValue()
            {
                Empty = true;
                _value = null;
            }
        }
    }
}
