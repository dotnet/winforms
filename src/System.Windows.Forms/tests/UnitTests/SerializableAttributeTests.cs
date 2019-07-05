// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Versioning;
using Xunit;

namespace System.Windows.Forms.Tests
{
    public class SerializableAttributeTests
    {
        private static readonly string[] s_interestedAssemblies = new string[]
        {
            "System.Windows.Forms",
        };

        private static readonly string[] s_serializableWhiteList = new string[]
        {
            "System.Resources.ResXDataNode",
            "System.Resources.ResXFileRef",
            "System.Windows.Forms.Cursor",
            "System.Windows.Forms.ImageListStreamer",
            "System.Windows.Forms.ListViewGroup",
            "System.Windows.Forms.ListViewItem",
            "System.Windows.Forms.OwnerDrawPropertyBag",
            "System.Windows.Forms.Padding",
            "System.Windows.Forms.TableLayoutSettings",
            "System.Windows.Forms.TreeNode",
            "System.LocalAppContext+<>c",
            "System.Windows.Forms.AxHost+State",
            "System.Windows.Forms.ListViewItem+ListViewSubItem",
            "System.Windows.Forms.ListViewItem+SubItemStyle",

        };

        private const string DesiredFrameworkMoniker = "netcoreapp";

        private HashSet<string> _interestedAssemblies;
        private HashSet<string> _serializableWhiteList;

        public SerializableAttributeTests()
        {
            _interestedAssemblies = new HashSet<string>(interestedAssemblies, StringComparer.InvariantCultureIgnoreCase);
            _serializableWhiteList = new HashSet<string>(serializableWhiteList, StringComparer.InvariantCultureIgnoreCase);
        }

        [Fact]
        public void EnsureWhitelist()
        {
            foreach (AssemblyName assemblyName in Assembly.GetExecutingAssembly().GetReferencedAssemblies())
            {
                if (!_interestedAssemblies.Contains(assemblyName.Name))
                {
                    continue;
                }

                var referencedAssembly = Assembly.Load(assemblyName);

                var targetFramework = referencedAssembly.GetCustomAttributes(true)
                    .OfType<TargetFrameworkAttribute>().FirstOrDefault();

                if (!targetFramework.FrameworkDisplayName.Contains(DesiredFrameworkMoniker, StringComparison.InvariantCultureIgnoreCase) &&
                    !targetFramework.FrameworkName.Contains(DesiredFrameworkMoniker, StringComparison.InvariantCultureIgnoreCase))
                {
                    continue;
                }

                foreach (Type type in referencedAssembly.GetTypes())
                {
                    // ensure that type we expect to have [Serializable] do and that others do not.
                    if (_serializableWhiteList.Contains(type.FullName))
                    {
                        Assert.False(Attribute.GetCustomAttribute(type, typeof(SerializableAttribute)) == null,
                            string.Format("{0} must have Serializable Attribute", type.FullName));
                    }
                    else
                    {
                        Assert.True(Attribute.GetCustomAttribute(type, typeof(SerializableAttribute)) == null,
                            string.Format("{0} must not have Serializable Attribute", type.FullName));
                    }
                }
            }
        }
    }
}
