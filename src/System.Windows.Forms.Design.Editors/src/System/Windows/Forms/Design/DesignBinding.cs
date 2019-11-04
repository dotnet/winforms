// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.Drawing.Design;
using System.Reflection.Metadata;

namespace System.Windows.Forms.Design
{
    internal class DesignBinding
    {

        private object dataSource;
        private string dataMember;

        public static DesignBinding Null = new DesignBinding(null, null);

        public DesignBinding(object dataSource, string dataMember)
        {
            this.dataSource = dataSource;
            this.dataMember = dataMember;
        }

        public bool IsNull
        {
            get
            {
                return (dataSource == null);
            }
        }

        public object DataSource
        {
            get
            {
                return dataSource;
            }
        }

        public string DataMember
        {
            get
            {
                return dataMember;
            }
        }

        public string DataField
        {
            get
            {
                if (String.IsNullOrEmpty(dataMember))
                {
                    return String.Empty;
                }

                int lastDot = dataMember.LastIndexOf(".");
                if (lastDot == -1)
                {
                    return dataMember;
                }
                else
                {
                    return dataMember.Substring(lastDot + 1);
                }
            }
        }

        public bool Equals(object dataSource, string dataMember)
        {
            return (dataSource == this.dataSource && String.Equals(dataMember, this.dataMember, StringComparison.OrdinalIgnoreCase));
        }
    }
}
