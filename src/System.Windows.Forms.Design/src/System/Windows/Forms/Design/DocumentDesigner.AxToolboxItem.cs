// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing.Design;
using System.Globalization;
using System.Reflection;
using System.Runtime.Serialization;
using Microsoft.Win32;
using Windows.Win32.System.Com;

namespace System.Windows.Forms.Design;

public partial class DocumentDesigner
{
    private class AxToolboxItem : ToolboxItem
    {
        private string _clsid;
        private Type? _axctlType;
        private string _version = string.Empty;

        public AxToolboxItem(string clsid) : base(typeof(AxHost))
        {
            _clsid = clsid;
            Company = null; // we don't get any company info for ax controls.
            LoadVersionInfo();
        }

        // Since we don't call the base constructor here, which does call Deserialize which we
        // override, we should be okay.
        private AxToolboxItem(SerializationInfo info, StreamingContext context)
        {
            Deserialize(info, context);
        }

        /// <summary>
        ///  The Component Type is ".Net Component" -- unless otherwise specified by a derived toolboxitem
        /// </summary>
        public override string ComponentType => SR.Ax_Control;

        public override string Version => _version;

        private void LoadVersionInfo()
        {
            string controlKey = $"CLSID\\{_clsid}";
            using RegistryKey? key = Registry.ClassesRoot.OpenSubKey(controlKey);

            // Fail later- not for tooltip info.
            using RegistryKey? verKey = key?.OpenSubKey("Version");
            if (verKey is not null)
            {
                _version = (string)verKey.GetValue("")!;
            }
        }

        /// <summary>
        ///  Creates an instance of the ActiveX control.
        ///  Calls VS7 project system to generate the wrappers if they are needed.
        /// </summary>
        protected override IComponent[] CreateComponentsCore(IDesignerHost? host)
        {
            Debug.Assert(host is not null, "Designer host is null");

            // Get the DTE References object
            object? references = GetReferences(host);
            if (references is not null)
            {
                try
                {
                    TLIBATTR tlibAttr = GetTypeLibAttr();

                    object[] args =
                    [
                        $"{{{tlibAttr.guid}}}",
                        (int)tlibAttr.wMajorVerNum,
                        (int)tlibAttr.wMinorVerNum,
                        tlibAttr.lcid,
                        "",
                    ];
                    object? tlbRef = references.GetType().InvokeMember(
                        "AddActiveX",
                        BindingFlags.InvokeMethod | BindingFlags.Public | BindingFlags.Instance,
                        null,
                        references,
                        args,
                        CultureInfo.InvariantCulture);
                    Debug.Assert(tlbRef is not null, $"Null reference returned by AddActiveX (tlbimp) by the project system for: {_clsid}");

                    args[4] = "aximp";
                    object? axRef = references.GetType().InvokeMember(
                        "AddActiveX",
                        BindingFlags.InvokeMethod | BindingFlags.Public | BindingFlags.Instance,
                        null,
                        references,
                        args,
                        CultureInfo.InvariantCulture);
                    Debug.Assert(axRef is not null, $"Null reference returned by AddActiveX (aximp) by the project system for: {_clsid}");

                    _axctlType = GetAxTypeFromReference(axRef, host);
                }
                catch (TargetInvocationException tie)
                {
                    throw tie.InnerException!;
                }
            }

            if (_axctlType is null)
            {
                IUIService? uiSvc = host.GetService<IUIService>();
                if (uiSvc is null)
                {
                    RTLAwareMessageBox.Show(
                        null,
                        SR.AxImportFailed,
                        null,
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error,
                        MessageBoxDefaultButton.Button1,
                        0);
                }
                else
                {
                    uiSvc.ShowError(SR.AxImportFailed);
                }

                return [];
            }

            var comps = new IComponent[1];
            try
            {
                comps[0] = host.CreateComponent(_axctlType);
            }
            catch (Exception e)
            {
                Debug.Fail($"Could not create type: {e}");
                throw;
            }

            Debug.Assert(comps[0] is not null, "Could not create instance of ActiveX control wrappers!!!");
            return comps;
        }

        /// <summary>
        /// <para>Loads the state of this 'AxToolboxItem'
        /// from the stream.</para>
        /// </summary>
        [MemberNotNull(nameof(_clsid))]
        protected override void Deserialize(SerializationInfo info, StreamingContext context)
        {
            base.Deserialize(info, context);
            _clsid = info.GetString("Clsid")!;
        }

        /// <summary>
        /// <para>Gets hold of the DTE Reference object and from there, opens the assembly of the
        /// ActiveX control we want to create. It then walks through all AxHost derived classes
        /// in that assembly, and returns the type that matches our control's CLSID.</para>
        /// </summary>
        private Type? GetAxTypeFromReference(object reference, IDesignerHost host)
        {
            string? path = (string?)reference.GetType().InvokeMember("Path", BindingFlags.GetProperty | BindingFlags.Public | BindingFlags.Instance, null, reference, null, CultureInfo.InvariantCulture);

            // Missing reference will show up as an empty string.
            //
            if (string.IsNullOrEmpty(path))
            {
                return null;
            }

            FileInfo file = new(path);
            string fullPath = file.FullName;

            ITypeResolutionService? trs = host.GetService<ITypeResolutionService>();
            Debug.Assert(trs is not null, "No type resolution service found.");

            Assembly? a = trs.GetAssembly(AssemblyName.GetAssemblyName(fullPath));
            Debug.Assert(a is not null, $"No assembly found at {fullPath}");

            return GetAxTypeFromAssembly(a);
        }

        /// <summary>
        /// <para>Walks through all AxHost derived classes in the given assembly,
        /// and returns the type that matches our control's CLSID.</para>
        /// </summary>
        private Type? GetAxTypeFromAssembly(Assembly a)
        {
            Type[] types = a.GetTypes();
            int len = types.Length;
            for (int i = 0; i < len; ++i)
            {
                Type t = types[i];
                if (!(typeof(AxHost).IsAssignableFrom(t)))
                {
                    continue;
                }

                object[] attrs = t.GetCustomAttributes(typeof(AxHost.ClsidAttribute), false);
                Debug.Assert(attrs is not null && attrs.Length == 1, $"Invalid number of GuidAttributes found on: {t.FullName}");

                AxHost.ClsidAttribute guid = (AxHost.ClsidAttribute)attrs[0];
                if (string.Equals(guid.Value, _clsid, StringComparison.OrdinalIgnoreCase))
                {
                    return t;
                }
            }

            return null;
        }

        /// <summary>
        /// <para>Gets the References collection object from the designer host. The steps are:
        ///  Get the ProjectItem from the IDesignerHost.
        ///  Get the Containing Project of the ProjectItem.
        ///  Get the VSProject of the Containing Project.
        ///  Get the References property of the VSProject.</para>
        /// </summary>
        private static object? GetReferences(IDesignerHost host)
        {
            Type? type = Type.GetType($"EnvDTE.ProjectItem, {AssemblyRef.EnvDTE}");

            if (type is null)
            {
                return null;
            }

            object? ext = host.GetService(type);
            if (ext is null)
                return null;

            string? name = ext.GetType().InvokeMember("Name", BindingFlags.GetProperty | BindingFlags.Public | BindingFlags.Instance, null, ext, null, CultureInfo.InvariantCulture)!.ToString();

            object? project = ext.GetType().InvokeMember("ContainingProject", BindingFlags.GetProperty | BindingFlags.Public | BindingFlags.Instance, null, ext, null, CultureInfo.InvariantCulture);
            Debug.Assert(project is not null, $"No DTE Project for the current project item: {name}");

            object? vsproject = project.GetType().InvokeMember("Object", BindingFlags.GetProperty | BindingFlags.Public | BindingFlags.Instance, null, project, null, CultureInfo.InvariantCulture);
            Debug.Assert(vsproject is not null, $"No VS Project for the current project item: {name}");

            object? references = vsproject.GetType().InvokeMember("References", BindingFlags.GetProperty | BindingFlags.Public | BindingFlags.Instance, null, vsproject, null, CultureInfo.InvariantCulture);
            Debug.Assert(references is not null, $"No References for the current project item: {name}");

            return references;
        }

        /// <summary>
        ///  Gets the TypeLibAttr corresponding to the TLB containing our ActiveX control.
        /// </summary>
        private unsafe TLIBATTR GetTypeLibAttr()
        {
            string controlKey = $"CLSID\\{_clsid}";
            using RegistryKey? key = Registry.ClassesRoot.OpenSubKey(controlKey)
                ?? throw new ArgumentException(string.Format(SR.AXNotRegistered, controlKey));

            // Load the typelib into memory.
            ITypeLib* pTLB = null;

            // Open the key for the TypeLib
            using RegistryKey? tlbKey = key.OpenSubKey("TypeLib");

            if (tlbKey is not null)
            {
                // Get the major and minor version numbers.
                using RegistryKey? verKey = key.OpenSubKey("Version");
                Debug.Assert(verKey is not null, $"No version registry key found for: {controlKey}");

                string ver = (string)verKey.GetValue("")!;
                int dot = ver.IndexOf('.');

                short majorVer;

                short minorVer;
                if (dot == -1)
                {
                    majorVer = short.Parse(ver, CultureInfo.InvariantCulture);
                    minorVer = 0;
                }
                else
                {
                    majorVer = short.Parse(ver.AsSpan(0, dot), CultureInfo.InvariantCulture);
                    minorVer = short.Parse(ver.AsSpan(dot + 1, ver.Length - dot - 1), CultureInfo.InvariantCulture);
                }

                Debug.Assert(majorVer > 0 && minorVer >= 0, $"No Major version number found for: {controlKey}");
                verKey.Close();

                object o = tlbKey.GetValue("")!;

                // Try to get the TypeLib's Guid.
                Guid tlbGuid = new((string)o);
                Debug.Assert(!tlbGuid.Equals(Guid.Empty), $"No valid Guid found for: {controlKey}");
                tlbKey.Close();

                HRESULT hr = PInvokeCore.LoadRegTypeLib(
                    tlbGuid,
                    (ushort)majorVer,
                    (ushort)minorVer,
                    (uint)Application.CurrentCulture.LCID,
                    &pTLB);
            }

            // Try to load the TLB directly from the InprocServer32.
            // If that fails, try to load the TLB based on the TypeLib guid key.
            if (pTLB is null)
            {
                using RegistryKey? inprocServerKey = key.OpenSubKey("InprocServer32");
                if (inprocServerKey is not null)
                {
                    string? inprocServer = (string?)inprocServerKey.GetValue(string.Empty);
                    Debug.Assert(inprocServer is not null, $"No valid InprocServer32 found for: {controlKey}");
                    HRESULT hr = PInvoke.LoadTypeLib(inprocServer, &pTLB);
                }
            }

            if (pTLB is not null)
            {
                try
                {
                    if (pTLB->GetLibAttr(out TLIBATTR* pTlibAttr).Failed)
                    {
                        throw new ArgumentException(string.Format(SR.AXNotRegistered, controlKey));
                    }
                    else
                    {
                        // Marshal the returned int as a TLibAttr structure
                        TLIBATTR typeLibraryAttributes = *pTlibAttr;
                        pTLB->ReleaseTLibAttr(pTlibAttr);

                        return typeLibraryAttributes;
                    }
                }
                finally
                {
                    pTLB->Release();
                }
            }
            else
            {
                throw new ArgumentException(string.Format(SR.AXNotRegistered, controlKey));
            }
        }

        /// <summary>
        ///  Saves the state of this <see cref="AxToolboxItem"/> to the specified serialization info.
        /// </summary>
        protected override void Serialize(SerializationInfo info, StreamingContext context)
        {
            base.Serialize(info, context);
            info.AddValue("Clsid", _clsid);
        }
    }
}
