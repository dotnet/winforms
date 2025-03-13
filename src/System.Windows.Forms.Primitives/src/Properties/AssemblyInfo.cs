// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.CompilerServices;

[assembly: System.Runtime.InteropServices.ComVisible(false)]

[assembly: InternalsVisibleTo($"Microsoft.VisualBasic.Forms, PublicKey={PublicKeys.MicrosoftPublic}")]

[assembly: InternalsVisibleTo($"System.Windows.Forms, PublicKey={PublicKeys.Ecma}")]
[assembly: InternalsVisibleTo($"System.Windows.Forms.Design, PublicKey={PublicKeys.Ecma}")]
[assembly: InternalsVisibleTo($"System.Windows.Forms.Design.Editors, PublicKey={PublicKeys.Ecma}")]

[assembly: InternalsVisibleTo($"System.Windows.Forms.Design.Tests, PublicKey={PublicKeys.Ecma}")]
[assembly: InternalsVisibleTo($"System.Windows.Forms.Primitives.Tests, PublicKey={PublicKeys.Ecma}")]
[assembly: InternalsVisibleTo($"System.Windows.Forms.Primitives.TestUtilities, PublicKey={PublicKeys.Ecma}")]
[assembly: InternalsVisibleTo($"System.Windows.Forms.Tests, PublicKey={PublicKeys.MicrosoftShared}")]
[assembly: InternalsVisibleTo($"System.Windows.Forms.TestUtilities, PublicKey={PublicKeys.MicrosoftShared}")]
[assembly: InternalsVisibleTo($"System.Windows.Forms.UI.IntegrationTests, PublicKey={PublicKeys.MicrosoftShared}")]
[assembly: InternalsVisibleTo($"System.Windows.Forms.Interop.Tests, PublicKey={PublicKeys.MicrosoftShared}")]
[assembly: InternalsVisibleTo($"WinformsControlsTest, PublicKey={PublicKeys.MicrosoftShared}")]
[assembly: InternalsVisibleTo($"System.Windows.Forms.IntegrationTests.Common, PublicKey={PublicKeys.MicrosoftShared}")]
[assembly: InternalsVisibleTo($"NativeHost.ManagedControl, PublicKey={PublicKeys.MicrosoftShared}")]
[assembly: InternalsVisibleTo($"ScratchProjectWithInternals, PublicKey={PublicKeys.MicrosoftShared}")]
[assembly: InternalsVisibleTo($"ComDisabled.Tests, PublicKey={PublicKeys.MicrosoftShared}")]

// This is needed in order to Moq internal interfaces for testing
[assembly: InternalsVisibleTo($"DynamicProxyGenAssembly2, PublicKey={PublicKeys.Moq}")]
