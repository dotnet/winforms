### History
WinForms had documented some NETFX 1.1 APIs as obsolete in NETFX 2.0, but didn't decorate them with the [Obsolete] attributes. We shipped them in .NET3.0, then removed them completely in .NET3.1 - https://learn.microsoft.com/dotnet/core/compatibility/winforms#removed-controls.
The reason to remove these types was that replacement types have been implemented in .NET Framework 2.0 and we had stopped maintaining these classes. Meanwhile, they fell befind in support of the modern features and particularly they don't comply with the accessibility requirements.

### Problem
Since then we received 2 requests to re-introduce the public surface in order to facilitate migration from the .NET Framework. Consider an application that supports extensibility plug ins compiled against the .NET Framework. .NET application can load .NET Framework assemblies if .NET Framework types can be forwarded to the .NET implementations, even if those implementations do nothing. Such plug-ins would load and work if they avoid the unsupported code. However, if the unsupported code is executed, the JIT throws a MissingMemberException that is hard to catch and handle.

### Solution
We are re-adding the previously removed types as "hollow" types to provide binary compatibility. This "implementation" is copied from the .NET Framework 4.8.1 reference assembly. These types can't be executed, as they should be replaced with the modern counterparts as users migrate their applications. The main advantage is that now the above mentioned plugin would load and would throw PlatformNotSupportedException when the problematic code is executed instead of the missing member exceptions at JIT time. And the developer has an option for conditionally avoid calling the hollow methods.
We should not modify code in this folder.

#### Description of changes:
* None of the re-added types can be constructed, all constructors throw new PlatformNotSupportedException(), default constructors are suppressed by introducing throwing public constructors if needed.
* All attributes that have been present on the .NET Framework types, are preserved, as they are accessible by reflection.
* All re-introduced types are decorated with [EditorBrowsable(EditorBrowsableState.Never)] attribute that hides them from the intellisense. Type names can be typed in by the developer manually and intellisense would show the same members as it did for .NET Framework projects.
* All re-introduced types are decorated with the ObsoleteAttribute that results in a compile time warning. All types share a single deprecation warning Id, to simplify suppression, but have different explanation messages.
* All re-introduced types are decorated with [Browsable(false)] attribute to not show custom control properties of these types in the property browser.
* All public members are re-introduced with their metadata, except for the private attributes.
* Members inherited from the base classes (Control, for example) are not re-introduced even if they had been overridden in the past because they are not required for binary compatibility, unless they are decorated with different attributes.
* Members that are re-added to the existing types (Form and Control) are returning defaults or doing nothing.
* Non-void public or protected instance members on the restored types `throw null`, not the `PlatformNotSupportedException` to save space.
* Void public or protected instance members do nothing.
* Public or protected fields return default values.
* Nullability is disabled for all re-added classes, structs and delegates for compatibility with the .NET Framework.

### Use Case
.NET applications can reference .NET Framework 3rd party libraries that use these types. Code will JIT and if the unsupported code is executed, it will throw an exception that can be caught instead of JIT throwing a missing member exception. For example:

```cs
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
```