// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    public struct BindingMemberInfo
    {
        private readonly string _dataList;
        private readonly string _dataField;

        public BindingMemberInfo(string dataMember)
        {
            if (dataMember == null)
            {
                dataMember = string.Empty;
            }

            int lastDot = dataMember.LastIndexOf('.');
            if (lastDot != -1)
            {
                _dataList = dataMember.Substring(0, lastDot);
                _dataField = dataMember.Substring(lastDot + 1);
            }
            else
            {
                _dataList = string.Empty;
                _dataField = dataMember;
            }
        }

        public string BindingPath => _dataList ?? string.Empty;

        public string BindingField => _dataField ?? string.Empty;

        public string BindingMember
        {
            get => BindingPath.Length > 0 ? BindingPath + "." + BindingField : BindingField;
        }

        public override bool Equals(object otherObject)
        {
            if (!(otherObject is BindingMemberInfo otherMember))
            {
                return false;
            }

            return string.Equals(BindingMember, otherMember.BindingMember, StringComparison.OrdinalIgnoreCase);
        }

        public static bool operator ==(BindingMemberInfo a, BindingMemberInfo b) => a.Equals(b);

        public static bool operator !=(BindingMemberInfo a, BindingMemberInfo b) => !a.Equals(b);

        public override int GetHashCode() => base.GetHashCode();
    }
}
