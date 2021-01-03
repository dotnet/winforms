﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using System.Reflection;

namespace System.Windows.Forms
{
    public partial class Control
    {
        private class ControlVersionInfo
        {
            private string? _companyName;
            private string? _productName;
            private string? _productVersion;
            private FileVersionInfo? _versionInfo;
            private readonly Control _owner;

            internal ControlVersionInfo(Control owner)
            {
                _owner = owner;
            }

            /// <summary>
            ///  The company name associated with the component.
            /// </summary>
            internal string CompanyName
            {
                get
                {
                    if (_companyName is null)
                    {
                        object[] attrs = _owner.GetType().Module.Assembly.GetCustomAttributes(typeof(AssemblyCompanyAttribute), false);
                        if (attrs is not null && attrs.Length > 0)
                        {
                            _companyName = ((AssemblyCompanyAttribute)attrs[0]).Company;
                        }

                        if (_companyName is null || _companyName.Length == 0)
                        {
                            _companyName = GetFileVersionInfo().CompanyName;
                            if (_companyName is not null)
                            {
                                _companyName = _companyName.Trim();
                            }
                        }

                        if (_companyName is null || _companyName.Length == 0)
                        {
                            string? ns = _owner.GetType().Namespace;

                            if (ns is null)
                            {
                                ns = string.Empty;
                            }

                            int firstDot = ns.IndexOf('/');
                            if (firstDot != -1)
                            {
                                _companyName = ns.Substring(0, firstDot);
                            }
                            else
                            {
                                _companyName = ns;
                            }
                        }
                    }
                    return _companyName;
                }
            }

            /// <summary>
            ///  The product name associated with this component.
            /// </summary>
            internal string ProductName
            {
                get
                {
                    if (_productName is null)
                    {
                        object[] attrs = _owner.GetType().Module.Assembly.GetCustomAttributes(typeof(AssemblyProductAttribute), false);
                        if (attrs is not null && attrs.Length > 0)
                        {
                            _productName = ((AssemblyProductAttribute)attrs[0]).Product;
                        }

                        if (_productName is null || _productName.Length == 0)
                        {
                            _productName = GetFileVersionInfo().ProductName;
                            if (_productName is not null)
                            {
                                _productName = _productName.Trim();
                            }
                        }

                        if (_productName is null || _productName.Length == 0)
                        {
                            string? ns = _owner.GetType().Namespace;

                            if (ns is null)
                            {
                                ns = string.Empty;
                            }
                            int firstDot = ns.IndexOf('.');
                            if (firstDot != -1)
                            {
                                _productName = ns.Substring(firstDot + 1);
                            }
                            else
                            {
                                _productName = ns;
                            }
                        }
                    }

                    return _productName;
                }
            }

            /// <summary>
            ///  The product version associated with this component.
            /// </summary>
            internal string ProductVersion
            {
                get
                {
                    if (_productVersion is null)
                    {
                        // custom attribute
                        object[] attrs = _owner.GetType().Module.Assembly.GetCustomAttributes(typeof(AssemblyInformationalVersionAttribute), false);
                        if (attrs is not null && attrs.Length > 0)
                        {
                            _productVersion = ((AssemblyInformationalVersionAttribute)attrs[0]).InformationalVersion;
                        }

                        // win32 version info
                        if (_productVersion is null || _productVersion.Length == 0)
                        {
                            _productVersion = GetFileVersionInfo().ProductVersion;
                            if (_productVersion is not null)
                            {
                                _productVersion = _productVersion.Trim();
                            }
                        }

                        // fake it
                        if (_productVersion is null || _productVersion.Length == 0)
                        {
                            _productVersion = "1.0.0.0";
                        }
                    }
                    return _productVersion;
                }
            }

            /// <summary>
            ///  Retrieves the FileVersionInfo associated with the main module for
            ///  the component.
            /// </summary>
            private FileVersionInfo GetFileVersionInfo()
            {
                if (_versionInfo is null)
                {
                    string path = _owner.GetType().Module.FullyQualifiedName;

                    _versionInfo = FileVersionInfo.GetVersionInfo(path);
                }
                return _versionInfo;
            }
        }
    }
}
