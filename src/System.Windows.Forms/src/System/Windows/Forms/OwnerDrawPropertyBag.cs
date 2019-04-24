// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters;

namespace System.Windows.Forms
{
    /// <summary>
    /// Class used to pass new font/color information around for "partial" ownerdraw list/treeview items.
    /// </summary>
    [SuppressMessage("Microsoft.Usage", "CA2240:ImplementISerializableCorrectly")]
    [Serializable]
    public class OwnerDrawPropertyBag : MarshalByRefObject, ISerializable
    {
        private Control.FontHandleWrapper _fontWrapper = null;
        private static object s_internalSyncObject = new object();

        protected OwnerDrawPropertyBag(SerializationInfo info, StreamingContext context)
        {
            foreach (SerializationEntry entry in info)
            {
                if (entry.Name == "Font")
                {
                    Font = (Font)entry.Value;
                }
                else if (entry.Name == "ForeColor")
                {
                    ForeColor = (Color)entry.Value;
                }
                else if (entry.Name == "BackColor")
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

        internal IntPtr FontHandle
        {
            get
            {
                if (_fontWrapper == null)
                {
                    _fontWrapper = new Control.FontHandleWrapper(Font);
                }

                return _fontWrapper.Handle;
            }
        }

        /// <summary>
        /// Returns whether or not this property bag contains all default values (is empty)
        /// </summary>
        public virtual bool IsEmpty() => Font == null && ForeColor.IsEmpty && BackColor.IsEmpty;

        /// <summary>
        /// Copies the bag. Always returns a valid ODPB object
        /// </summary>
        public static OwnerDrawPropertyBag Copy(OwnerDrawPropertyBag value)
        {
            lock (s_internalSyncObject)
            {
                var result = new OwnerDrawPropertyBag();
                if (value == null)
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
            si.AddValue("BackColor", BackColor);
            si.AddValue("ForeColor", ForeColor);
            si.AddValue("Font", Font);
        }
    }
}
