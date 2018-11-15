// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms {

    using System;
    using System.Globalization;
    
    /// <include file='doc\DataMemberInfo.uex' path='docs/doc[@for="BindingMemberInfo"]/*' />
    /// <devdoc>
    ///    <para>[To be supplied.]</para>
    /// </devdoc>
    public struct BindingMemberInfo {
        private string dataList;
        private string dataField;
            
        /// <include file='doc\DataMemberInfo.uex' path='docs/doc[@for="BindingMemberInfo.BindingMemberInfo"]/*' />
        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        public BindingMemberInfo(string dataMember) {
            if (dataMember == null)
                dataMember = "";
                    
            int lastDot = dataMember.LastIndexOf(".");
            if (lastDot != -1) {
                dataList = dataMember.Substring(0,lastDot);
                dataField = dataMember.Substring(lastDot+1);
            }
            else {
                dataList = "";
                dataField = dataMember;
            }
        }
            
        /// <include file='doc\DataMemberInfo.uex' path='docs/doc[@for="BindingMemberInfo.BindingPath"]/*' />
        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        public string BindingPath {
            get {
                return (dataList != null ? dataList : "");
            }
        }
            
        /// <include file='doc\DataMemberInfo.uex' path='docs/doc[@for="BindingMemberInfo.BindingField"]/*' />
        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        public string BindingField {
            get {
                return (dataField != null ? dataField : "");
            }
        }
            
        /// <include file='doc\DataMemberInfo.uex' path='docs/doc[@for="BindingMemberInfo.BindingMember"]/*' />
        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        public string BindingMember {
            get {
                return (BindingPath.Length > 0 ? BindingPath + "." + BindingField : BindingField);
            }
        }
            
        /// <include file='doc\DataMemberInfo.uex' path='docs/doc[@for="BindingMemberInfo.Equals"]/*' />
        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        public override bool Equals(object otherObject) {
            if (otherObject is BindingMemberInfo) {
                BindingMemberInfo otherMember = (BindingMemberInfo) otherObject;
                return (String.Equals(this.BindingMember, otherMember.BindingMember, StringComparison.OrdinalIgnoreCase));
            }
            return false;
        }

        public static bool operator ==(BindingMemberInfo a, BindingMemberInfo b) {            
            return a.Equals(b);
        }

        public static bool operator !=(BindingMemberInfo a, BindingMemberInfo b) {
            return !a.Equals(b);
        }
        
        /// <include file='doc\DataMemberInfo.uex' path='docs/doc[@for="BindingMemberInfo.GetHashCode"]/*' />
        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        public override int GetHashCode() {
            return base.GetHashCode();
        }
    }
}
            
