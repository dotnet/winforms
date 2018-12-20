// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms {

    using System.Diagnostics;
    using System;
    using System.Drawing;
    using System.Diagnostics.CodeAnalysis;
    using System.Windows.Forms.Internal;
    using System.Windows.Forms;
    using Microsoft.Win32;
    using System.Runtime.Serialization;
    using System.Runtime.Serialization.Formatters;
    using System.Security.Permissions;


    /// <include file='doc\OwnerDrawPropertyBag.uex' path='docs/doc[@for="OwnerDrawPropertyBag"]/*' />
    /// <devdoc>
    ///
    ///     Class used to pass new font/color information around for "partial" ownerdraw list/treeview items.
    /// </devdoc>
    /// <internalonly/>
    // 
    [SuppressMessage("Microsoft.Usage", "CA2240:ImplementISerializableCorrectly")]    
    [Serializable]
    public class OwnerDrawPropertyBag : MarshalByRefObject, ISerializable {
        Font font = null;
        Color foreColor = Color.Empty;
        Color backColor = Color.Empty;
        Control.FontHandleWrapper fontWrapper = null;
        private static object internalSyncObject = new object();
          /**
         * Constructor used in deserialization
         * Has to be protected because OwnerDrawPropertyBag is not sealed. FxCop Rule CA2229.
         */
        protected OwnerDrawPropertyBag(SerializationInfo info, StreamingContext context) {
            foreach (SerializationEntry entry in info) {
                if (entry.Name == "Font") {
                    font = (Font) entry.Value;
                }
                else if (entry.Name =="ForeColor") {
                    foreColor =(Color)entry.Value;
                }
                else if (entry.Name =="BackColor") {
                    backColor = (Color)entry.Value;
                }
            }
        }

        internal OwnerDrawPropertyBag(){
        }

        /// <include file='doc\OwnerDrawPropertyBag.uex' path='docs/doc[@for="OwnerDrawPropertyBag.Font"]/*' />
        public Font Font {
            get { 
                return font;
            }
            set {
                font = value;
            }
        }

        /// <include file='doc\OwnerDrawPropertyBag.uex' path='docs/doc[@for="OwnerDrawPropertyBag.ForeColor"]/*' />
        public Color ForeColor {
            get {
                return foreColor;
            }
            set {
                foreColor = value;
            }
        }

        /// <include file='doc\OwnerDrawPropertyBag.uex' path='docs/doc[@for="OwnerDrawPropertyBag.BackColor"]/*' />
        public Color BackColor {
            get {
                return backColor;
            }
            set {
                backColor = value;
            }
        }

        internal IntPtr FontHandle {
            get {
                if (fontWrapper == null) {
                    fontWrapper = new Control.FontHandleWrapper(Font);
                }
                return fontWrapper.Handle;
            }
        }

        /// <include file='doc\OwnerDrawPropertyBag.uex' path='docs/doc[@for="OwnerDrawPropertyBag.IsEmpty"]/*' />
        /// <devdoc>
        ///     Returns whether or not this property bag contains all default values (is empty)
        /// </devdoc>
        public virtual bool IsEmpty() {
            return (Font == null && foreColor.IsEmpty && backColor.IsEmpty);
        }

        /// <include file='doc\OwnerDrawPropertyBag.uex' path='docs/doc[@for="OwnerDrawPropertyBag.Copy"]/*' />
        /// <devdoc>
        ///     Copies the bag. Always returns a valid ODPB object
        /// </devdoc>
        public static OwnerDrawPropertyBag Copy(OwnerDrawPropertyBag value) {
            lock(internalSyncObject) {
                OwnerDrawPropertyBag ret = new OwnerDrawPropertyBag();
                if (value == null) return ret;
                ret.backColor = value.backColor;
                ret.foreColor = value.foreColor;
                ret.Font = value.font;
                return ret;
            }
        }

        /// <include file='doc\Cursor.uex' path='docs/doc[@for="Cursor.ISerializable.GetObjectData"]/*' />
        /// <devdoc>
        /// ISerializable private implementation
        /// </devdoc>
        /// <internalonly/>
        [SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.SerializationFormatter)]        
        void ISerializable.GetObjectData(SerializationInfo si, StreamingContext context) {
            si.AddValue("BackColor", BackColor);
            si.AddValue("ForeColor", ForeColor);
            si.AddValue("Font", Font);
        }

    }
}
