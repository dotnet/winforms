# System.Drawing.Common

The `System.Drawing.Common` package allows .NET Core and .NET 6+ applications to access GDI+ graphics functionality. 
This package is especially useful for porting .NET Framework applications that rely on the `System.Drawing` namespace.

## Getting Started

To get started with `System.Drawing.Common`, install it using the NuGet Package Manager, the .NET CLI, or by editing your project file directly.

**NOTE:** If you are developing a **WinForms** application, you **do not** need to install the `System.Drawing.Common` package separately (to this end, you use the `Sdk` attribute for the `Project` element like `<Project Sdk="Microsoft.NET.Sdk">` in the .csproj or the .vbproj file and then specify `<UseWindowsForms>true</UseWindowsForms>`). This package is then automatically included as part of the .NET SDK for WinForms Apps, which means you can start using the `System.Drawing` namespace right away in your WinForms projects.

## Usage

The following examples demonstrate some basic tasks you can accomplish with `System.Drawing.Common`.

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
- [System.Drawing.Common Namespace | Microsoft Docs](https://docs.microsoft.com/dotnet/api/system.drawing)
- [Drawing with System.Drawing.Common | Microsoft Learn](https://learn.microsoft.com/dotnet/core/drawing/)

## Feedback

- Open an issue on the [GitHub repository](https://github.com/dotnet/winforms/issues)
- Reach out on Twitter with the [hashtag #winforms](https://twitter.com/search?q=%23winforms)
- Join our Discord channel: [dotnet/Discord](https://discord.com/invite/dotnet)
