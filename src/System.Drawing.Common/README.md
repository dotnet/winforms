# System.Drawing.Common

The System.Drawing.Common package allows .NET Core and .NET 5/6/7/8+ applications to access GDI+ graphics functionality. This package is especially useful for porting .NET Framework applications that rely on the `System.Drawing` namespace.

## Getting Started

To get started with System.Drawing.Common, install it using the NuGet Package Manager, the .NET CLI, or by editing your project file directly.

**NOTE:** If you are developing a **WinForms** application, you **do not** need to install the `System.Drawing.Common` package separately. This package is automatically included as part of the WinForms SDK. You can start using the `System.Drawing` namespace right away in your WinForms projects.

### NuGet Package Manager
```
Install-Package System.Drawing.Common -Version [your_version_here]
```

### .NET CLI
```
dotnet add package System.Drawing.Common --version [your_version_here]
```

### Prerequisites

- .NET Core 2.0-3.1 (min. .NET Core 3.1 for WinForms/WPF Apps)
- .NET 5-.NET 8+
- .NET Standard 2.0 or higher

## Usage

The following examples demonstrate some basic tasks you can accomplish with System.Drawing.Common.

### Create a Simple Bitmap and Save it

#### C#
```csharp
using System.Drawing;

class Program
{
    static void Main()
    {
        using (Bitmap bitmap = new Bitmap(100, 100))
        {
            using (Graphics g = Graphics.FromImage(bitmap))
            {
                g.Clear(Color.Red);
            }
            bitmap.Save("output.bmp");
        }
    }
}
```

#### VB
```vb
Imports System.Drawing

Module Program
    Sub Main()
        Using bitmap As New Bitmap(100, 100)
            Using g As Graphics = Graphics.FromImage(bitmap)
                g.Clear(Color.Red)
            End Using
            bitmap.Save("output.bmp")
        End Using
    End Sub
End Module
```

## Additional Documentation

For more in-depth tutorials and API references, you can check the following resources:

- [NuGet Gallery | System.Drawing.Common](https://nuget.org/packages/System.Drawing.Common/)
- [System.Drawing.Common Namespace | Microsoft Docs](https://docs.microsoft.com/en-us/dotnet/api/system.drawing?view=net-5.0)
- [Drawing with System.Drawing.Common | Microsoft Learn](https://learn.microsoft.com/en-us/dotnet/core/drawing/)

## Contribution Bar
- [x] [We consider new features, new APIs and performance changes](../README.md#primary-bar)
- [x] [We consider PRs that target this library for new source code analyzers](../README.md#secondary-bars)
- [ ] [We don't accept refactoring changes due to new language features](../README.md#secondary-bars)

See the [Help Wanted](https://github.com/dotnet/runtime/issues?q=is%3Aissue+is%3Aopen+label%3Aarea-System.Drawing+label%3A%22help+wanted%22) issues.

## Feedback

For feedback, you can:

- Open an issue on the [GitHub repository](https://github.com/dotnet/runtime/issues)
- Reach out on Twitter with the hashtag #dotnet
- Join our Discord channel: [dotnet/Discord](https://discord.com/invite/dotnet)
