// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Reflection;

namespace System.Windows.Forms;

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
        [UnconditionalSuppressMessage("SingleFile", "IL3002", Justification = "Single-file case is handled")]
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

                    if ((_companyName is null || _companyName.Length == 0) && !OwnerIsInMemoryAssembly)
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

                        ns ??= string.Empty;

                        int firstDot = ns.IndexOf('/');
                        if (firstDot != -1)
                        {
                            _companyName = ns[..firstDot];
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
        [UnconditionalSuppressMessage("SingleFile", "IL3002", Justification = "Single-file case is handled")]
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

                    if ((_productName is null || _productName.Length == 0) && !OwnerIsInMemoryAssembly)
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

                        ns ??= string.Empty;

                        int firstDot = ns.IndexOf('.');
                        if (firstDot != -1)
                        {
                            _productName = ns[(firstDot + 1)..];
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
        [UnconditionalSuppressMessage("SingleFile", "IL3002", Justification = "Single-file case is handled")]
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
                    if ((_productVersion is null || _productVersion.Length == 0) && !OwnerIsInMemoryAssembly)
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
        ///  Retrieves the FileVersionInfo associated with the main module for the component.
        /// </summary>
        [RequiresAssemblyFiles($"Throws if {nameof(_owner)} is an in-memory assembly. Check {nameof(OwnerIsInMemoryAssembly)} first")]
        private FileVersionInfo GetFileVersionInfo()
        {
            if (_versionInfo is null)
            {
                string path = _owner.GetType().Module.FullyQualifiedName;
                _versionInfo = FileVersionInfo.GetVersionInfo(path);
            }

            return _versionInfo;
        }

        // We're deliberately checking for the empty string scenario to see if this is single-file.
#pragma warning disable IL3000 // Avoid accessing Assembly file path when publishing as a single file
        private bool OwnerIsInMemoryAssembly => _owner.GetType().Assembly.Location.Length == 0;
#pragma warning restore IL3000
    }
}
