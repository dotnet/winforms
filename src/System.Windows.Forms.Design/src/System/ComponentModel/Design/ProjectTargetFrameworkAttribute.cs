// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.ComponentModel.Design
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.Struct)]
    public sealed class ProjectTargetFrameworkAttribute : Attribute
    {
        private string _targetFrameworkMoniker;
        public ProjectTargetFrameworkAttribute(string targetFrameworkMoniker)
        {
            _targetFrameworkMoniker = targetFrameworkMoniker;
        }

        public string TargetFrameworkMoniker
        {
            get => _targetFrameworkMoniker;
        }
    }
}
