﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.CompilerServices;

[assembly: System.Runtime.InteropServices.ComVisible(false)]

[assembly: InternalsVisibleTo($"System.Windows.Forms, PublicKey={PublicKeys.Ecma}")]
[assembly: InternalsVisibleTo($"System.Windows.Forms.Design, PublicKey={PublicKeys.Ecma}")]
[assembly: InternalsVisibleTo($"System.Windows.Forms.Design.Editors, PublicKey={PublicKeys.Ecma}")]
[assembly: InternalsVisibleTo($"System.Windows.Forms.Primitives, PublicKey={PublicKeys.Ecma}")]
[assembly: InternalsVisibleTo($"System.Drawing.Common, PublicKey={PublicKeys.Open}")]

[assembly: InternalsVisibleTo($"System.Windows.Forms.Design.Tests, PublicKey={PublicKeys.Ecma}")]
[assembly: InternalsVisibleTo($"System.Windows.Forms.Primitives.Tests, PublicKey={PublicKeys.Ecma}")]
[assembly: InternalsVisibleTo($"System.Windows.Forms.Primitives.TestUtilities, PublicKey={PublicKeys.Ecma}")]
[assembly: InternalsVisibleTo($"System.Windows.Forms.Tests, PublicKey={PublicKeys.Ecma}")]
[assembly: InternalsVisibleTo($"System.Private.Windows.Core.Tests, PublicKey={PublicKeys.Ecma}")]
[assembly: InternalsVisibleTo($"BinaryFormatTests, PublicKey={PublicKeys.Ecma}")]
[assembly: InternalsVisibleTo($"System.Windows.Forms.TestUtilities, PublicKey={PublicKeys.Ecma}")]
[assembly: InternalsVisibleTo($"System.Windows.Forms.UI.IntegrationTests, PublicKey={PublicKeys.Ecma}")]
[assembly: InternalsVisibleTo($"System.Windows.Forms.Interop.Tests, PublicKey={PublicKeys.Ecma}")]
[assembly: InternalsVisibleTo($"System.Drawing.Common.Tests, PublicKey={PublicKeys.Open}")]
[assembly: InternalsVisibleTo($"WinformsControlsTest, PublicKey={PublicKeys.Ecma}")]
[assembly: InternalsVisibleTo($"System.Windows.Forms.IntegrationTests.Common, PublicKey={PublicKeys.Ecma}")]
[assembly: InternalsVisibleTo($"NativeHost.ManagedControl, PublicKey={PublicKeys.Ecma}")]
[assembly: InternalsVisibleTo($"ScratchProjectWithInternals, PublicKey={PublicKeys.Ecma}")]
[assembly: InternalsVisibleTo($"ComDisabled.Tests, PublicKey={PublicKeys.Ecma}")]

// This is needed in order to Moq internal interfaces for testing
[assembly: InternalsVisibleTo($"DynamicProxyGenAssembly2, PublicKey={PublicKeys.Moq}")]

// WPF assemblies
[assembly: InternalsVisibleTo($"PresentationBuildTasks, PublicKey={PublicKeys.MicrosoftShared}")]
[assembly: InternalsVisibleTo($"PresentationCore, PublicKey={PublicKeys.MicrosoftShared}")]
[assembly: InternalsVisibleTo($"PresentationFramework, PublicKey={PublicKeys.MicrosoftShared}")]
[assembly: InternalsVisibleTo($"PresentationUI, PublicKey={PublicKeys.MicrosoftShared}")]
[assembly: InternalsVisibleTo($"ReachFramework, PublicKey={PublicKeys.MicrosoftShared}")]
[assembly: InternalsVisibleTo($"System.Windows.Controls.Ribbon, PublicKey={PublicKeys.Ecma}")]
[assembly: InternalsVisibleTo($"System.Windows.Input.Manipulations, PublicKey={PublicKeys.Ecma}")]
[assembly: InternalsVisibleTo($"System.Windows.Presentation, PublicKey={PublicKeys.Ecma}")]
[assembly: InternalsVisibleTo($"System.Windows.Primitives, PublicKey={PublicKeys.MicrosoftShared}")]
[assembly: InternalsVisibleTo($"System.Xaml, PublicKey={PublicKeys.Ecma}")]
[assembly: InternalsVisibleTo($"UIAutomationClient, PublicKey={PublicKeys.MicrosoftShared}")]
[assembly: InternalsVisibleTo($"UIAutomationClientSideProviders, PublicKey={PublicKeys.Ecma}")]
[assembly: InternalsVisibleTo($"UIAutomationProvider, PublicKey={PublicKeys.MicrosoftShared}")]
[assembly: InternalsVisibleTo($"UIAutomationTypes, PublicKey={PublicKeys.MicrosoftShared}")]
[assembly: InternalsVisibleTo($"WindowsBase, PublicKey={PublicKeys.MicrosoftShared}")]
[assembly: InternalsVisibleTo($"WindowsFormsIntegration, PublicKey={PublicKeys.MicrosoftShared}")]
