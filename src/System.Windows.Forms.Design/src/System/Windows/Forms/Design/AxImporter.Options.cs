// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Reflection;

namespace System.Windows.Forms.Design;

public partial class AxImporter
{
    public sealed class Options
    {
        /// <summary>
        ///  The flag that controls when we sign the generated assemblies.
        /// </summary>
        public bool delaySign;

        /// <summary>
        ///  Flag that controls whether we are to generate sources for the ActiveX control wrapper.
        /// </summary>
        public bool genSources;

        /// <summary>
        ///  Regardless of which version of the library is registered, if at all, use the
        ///  path supplied on the command line for AxImp to generate the Windows Forms wrappers.
        /// </summary>
        public bool ignoreRegisteredOcx;

        /// <summary>
        ///  The file containing the strong name key container for the generated assemblies.
        /// </summary>
        public string? keyContainer;

        /// <summary>
        ///  The file containing the strong name key for the generated assemblies.
        /// </summary>
        public string? keyFile;

        /// <summary>
        ///  The strong name used for the generated assemblies.
        /// </summary>
#pragma warning disable SYSLIB0017 // Type or member is obsolete, see https://github.com/dotnet/runtime/pull/50941
        public StrongNameKeyPair? keyPair;
#pragma warning restore SYSLIB0017

        /// <summary>
        ///  Flag that controls whether we should output errors in the MSBuild format.
        /// </summary>
        public bool msBuildErrors;

        /// <summary>
        ///  Flag that controls whether we should show the logo.
        /// </summary>
        public bool noLogo;

        /// <summary>
        ///  The output directory for all the generated assemblies.
        /// </summary>
        public string? outputDirectory;

        /// <summary>
        ///  The path-included filename of type library containing the definition of the ActiveX control.
        /// </summary>
        public string? outputName;

        /// <summary>
        ///  The flag that controls whether we try to overwrite existing generated assemblies.
        /// </summary>
        public bool overwriteRCW;

        /// <summary>
        ///  The public key used to sign the generated assemblies.
        /// </summary>
        public byte[]? publicKey;

        /// <summary>
        ///  The object that allows us to resolve types and references needed to generate assemblies.
        /// </summary>
        public IReferenceResolver? references;

        /// <summary>
        ///  Flag that controls the output generated.
        /// </summary>
        public bool silentMode;

        /// <summary>
        ///  Flag that controls the output generated.
        /// </summary>
        public bool verboseMode;
    }
}
