// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.Drawing;
using System.Runtime.Serialization;
using static Interop;

namespace System.Windows.Forms
{
    /// <summary>
    ///  Class used to pass new font/color information around for "partial" ownerdraw list/treeview items.
    /// </summary>
    [Serializable] // This class is participating in resx serialization scenarios for listview/treeview items.
    public class OwnerDrawPropertyBag : MarshalByRefObject, ISerializable
    {
        private Control.FontHandleWrapper _fontWrapper;
        private static readonly object s_internalSyncObject = new object();

        protected OwnerDrawPropertyBag(SerializationInfo info, StreamingContext context)
        {
            foreach (SerializationEntry entry in info)
            {
                if (entry.Name == nameof(Font))
                {
                    Font = (Font)entry.Value;
                }
                else if (entry.Name == nameof(ForeColor))
                {
                    ForeColor = (Color)entry.Value;
                }
                else if (entry.Name == nameof(BackColor))
                {
                    BackColor = (Color)entry.Value;
                }
            }
        }

        internal OwnerDrawPropertyBag()
        {
        }

        public Font Font { get; set; }

        public Color ForeColor { get; set; }

        public Color BackColor { get; set; }

        internal Gdi32.HFONT FontHandle
        {
            get
            {
                if (_fontWrapper is null)
                {
                    _fontWrapper = new Control.FontHandleWrapper(Font);
                }

                return _fontWrapper.Handle;
            }
        }

        /// <summary>
        ///  Returns whether or not this property bag contains all default values (is empty)
        /// </summary>
        public virtual bool IsEmpty() => Font is null && ForeColor.IsEmpty && BackColor.IsEmpty;

        /// <summary>
        ///  Copies the bag. Always returns a valid ODPB object
        /// </summary>
        public static OwnerDrawPropertyBag Copy(OwnerDrawPropertyBag value)
        {
            lock (s_internalSyncObject)
            {
                var result = new OwnerDrawPropertyBag();
                if (value is null)
                {
                    return result;
                }

                result.BackColor = value.BackColor;
                result.ForeColor = value.ForeColor;
                result.Font = value.Font;
                return result;
            }
        }

        void ISerializable.GetObjectData(SerializationInfo si, StreamingContext context)
        {
            si.AddValue(nameof(BackColor), BackColor);
            si.AddValue(nameof(ForeColor), ForeColor);
            si.AddValue(nameof(Font), Font);
        }
    }
}
