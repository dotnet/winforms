// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.CompilerServices;

[assembly: System.Runtime.InteropServices.ComVisible(false)]

// Expose internal types to System.Design for type forwarding.
[assembly: InternalsVisibleTo($"System.Design, PublicKey={PublicKeys.MicrosoftPublic}")]

[assembly: InternalsVisibleTo($"System.Windows.Forms.Design.Tests, PublicKey={PublicKeys.Ecma}")]
[assembly: InternalsVisibleTo($"System.Windows.Forms.UI.IntegrationTests, PublicKey={PublicKeys.MicrosoftShared}")]
[assembly: InternalsVisibleTo($"ScratchProjectWithInternals, PublicKey={PublicKeys.Ecma}")]
