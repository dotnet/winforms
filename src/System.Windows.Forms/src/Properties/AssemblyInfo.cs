// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.CompilerServices;

[assembly: System.Runtime.InteropServices.ComVisible(false)]

[assembly: InternalsVisibleTo($"System.Windows.Forms.Tests, PublicKey={PublicKeys.Ecma}")]
[assembly: InternalsVisibleTo($"System.Windows.Forms.TestUtilities, PublicKey={PublicKeys.Ecma}")]
[assembly: InternalsVisibleTo($"System.Windows.Forms.Primitives.TestUtilities, PublicKey={PublicKeys.Ecma}")]
[assembly: InternalsVisibleTo($"System.Windows.Forms.Interop.Tests, PublicKey={PublicKeys.Ecma}")]
[assembly: InternalsVisibleTo($"System.Windows.Forms.UI.IntegrationTests, PublicKey={PublicKeys.Ecma}")]
[assembly: InternalsVisibleTo($"ScratchProjectWithInternals, PublicKey={PublicKeys.Ecma}")]
[assembly: InternalsVisibleTo($"ComDisabled.Tests, PublicKey={PublicKeys.Ecma}")]

// This is needed in order to Moq internal interfaces for testing
[assembly: InternalsVisibleTo($"DynamicProxyGenAssembly2, PublicKey={PublicKeys.Moq}")]
