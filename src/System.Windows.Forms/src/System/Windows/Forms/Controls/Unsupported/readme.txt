History
WinForms had documented some APIs as obsolete in NETFX 2.0, we shipped them in .NET3.0, but then removed them completely in .NET3.1 - https://learn.microsoft.com/dotnet/core/compatibility/winforms#removed-controls.
The reason to remove these types was that replacement types have been implemented in .NET Framework 2.0 and we had stopped maintaining these classes. Meanwhile, they fell befind in support of the modern features and particularly they don't comply with the accessibility requirements.

Problem
Since then we received 2 requests to re-introduce the public surface in order to facilitate migration from the .NET Framework. Consider an application that supports extensibility plug ins compiled against the .NET Framework. .NET application can load .NET Framework assemblies only if .NET Framework types can be forwarded to the .NET implementations, even if those implementations do nothing.

Solution
We are re-adding the previously removed types as "empty" placeholder types to provide binary compatibility. These types can't be executed, as they should be replaced with the modern counterparts. The main advantage is that now the above mentioned plugin would load and would throw PlatformNotSupportedException when the problematic code is executed instead f the missing member exceptions at JIT time.

Description of changes:
* None of the re-added types can be constructed, all constructors throw new PlatformNotSupportedException(), default constructors are avoided by introducing private constructors if needed.
* All attributes that have been present on the .NET Framework types, are preserved, as they are accessible by code.
* All re-introduced types are decorated with [EditorBrowsable(EditorBrowsableState.Never)] attribute that hides them from the intellisense. Type names can be typed in by the developer and then intellisense would show the same members as it did for .NET Framework projects.
* All re-introduced types are decorated with the ObsoleteAttribute that results in a compile time warning. Types related to the same feature share a single deprecation warning Id.
* All re-introduced types are decorated with [Browsable(false)] attribute to not show custom control properties of these types in the property browser.
* All other attributes are removed from the type members because they are not accessible. Most of these attributes were consumed by the designer, and these types can't be instantiated on the designer surface, and thus are not visible to Property Browser or design time serialization.
Members inherited from the base classes (Control for example) are not re-introduced even if they had been overridden in the past because they are not required for binary compatibility. An exception is properties or events that were decorated with [EditorBrowsable(EditorBrowsableState.Never)] in .NET Framework, as we don't want to show in intellisense members that have not been shown in the past.
* Members that are re-added to the existing types (Form and Control) are returning a default or doing nothing.
* Public or protected methods or properties on the restored types throw a PlatformNotSupportedException for consistency.
* Public or protected fields return the default value.
* nullability is disabled for all re-added classes, structs and delegates for compatibility with the .NET Framework.

Use Case

.NET applications can reference .NET Framework 3rd party libraries that use these types. Code will JIT and if the unsupported code is executed, it will throw an exception that can be caught instead of JIT throwing a missing member exception. For example:

if (control.ContextMenu is not null)
{
    control.ShowContextMenu();
} 
else
{
    // show a ContextMenuStrip
}
try
{
    LegacyControl.ShowContextMenu();
}
catch (PlatformNotSupportedException)
{
    // create a new ContextMenuStrip
}