# WinForms Clipboard/DataObject updates for .NET 10

The **.NET 10** cycle introduces major updates to the clipboard and drag‑and‑drop stack in **System.Windows.Forms**. These changes were driven by the **removal of `BinaryFormatter`** from the runtime, which previously handled serialization of custom types for OLE data transfer. The new APIs provide explicit type‑safe methods and use **JSON** instead of binary serialization, while still allowing an opt‑in fallback to `BinaryFormatter` for legacy applications.

## Background

`BinaryFormatter` was used when a custom type was placed on the clipboard for a user defined data format or in a drag‑and‑drop operation. The `BinaryFormatter` is insecure and has been removed from the base runtime. WinForms has introduced new, safer APIs and deprecated older APIs. WinForms also now makes you explicitly enable clipboard and drag drop scenarios for `BinaryFormatter` so you don't get unexpected usage if you need to enable it for other scenarios.

### Best Practices

- Avoid enabling the `BinaryFormatter` for clipboard and drag and drop scenarios if at all possible.
- Use the safe built-in types (listed below) and `SetData` APIs.
- For custom types, use the new `SetDataAsJson<T>()` APIs or serialize manually as `string` or `byte[]` data.
- **Always** use `TryGetData<T>()` to get data (not `GetData()`).
- Read legacy data using the `NrbfDecoder`.

### Safe built‑in types

Without `BinaryFormatter`, WinForms can still transfer certain intrinsic types using a built‑in manual serializer in the `BinaryFormatter` data format, **NRBF** ([.NET Remoting Binary Format](https://learn.microsoft.com/openspecs/windows_protocols/ms-nrbf/75b9fe09-be15-475f-85b8-ae7b7558cfe5)). Supported types include:

- **Primitive types**: `bool`, `byte`, `char`, `decimal`, `double`, `short`, `int`, `long`, `sbyte`, `ushort`, `uint`, `ulong`, `float`, `string`, `TimeSpan` and `DateTime`.
- **Arrays or `List<T>`** of these primitive types.
- **System.Drawing types**: `Bitmap`, `PointF`, `RectangleF`, `Point`, `Rectangle`, `SizeF`, `Size`, and `Color`.

When data is one of these types, WinForms transfers it without the user needing to implement any special logic. For custom types with [`DataFormats`](https://learn.microsoft.com/dotnet/api/system.windows.forms.dataformats) that aren't predefined, you should use JSON or your own serialization using raw string or byte data.

## New APIs in .NET 10

### `TryGetData<T>` – constrained deserialization of data

Several overloads of `TryGetData<T>` exist on **`Clipboard`**, **`DataObject`**, **`ITypedDataObject`**, **`DataObjectExtensions`** and the VB **`ClipboardProxy`**. These methods:

- Attempt to retrieve data in a given format and deserialize it to the requested type `T`.
- Return a boolean indicating success instead of throwing.
- When enabling `BinaryFormatter`, requires a **`resolver`** callback to restrict deserialization to known types.

Example using the **simplest overload** (no resolver).  This will **never** fall back to `BinaryFormatter`; it only succeeds if the data was serialized with `SetDataAsJson<T>`, one of the built‑in safe NRBF types (see above), _or_ if the data was set in-process without the `copy` flag (not the default):

```csharp
// Reading a Person previously stored with SetDataAsJson
if (Clipboard.TryGetData("MyApp.Person", out Person? person))
{
    Console.WriteLine($"Person: {person.Name} is {person.Age}");
}
else
{
    Console.WriteLine("No person data on clipboard or type mismatch");
}
```

A resolver overload can be used when you need to support legacy binary formats and have fully enabled the `BinaryFormatter`. See the "Enabling Full BinaryFormatter Support" below for details.

### `SetDataAsJson<T>` – write custom types without BinaryFormatter

`SetDataAsJson<T>` is a new overload on the **`Clipboard`**, **`DataObject`** and **VB `ClipboardProxy`** classes. It serializes *simple* `T` types to JSON using **`System.Text.Json`**. For example:

```csharp
public record Person(string Name, int Age);

// Serialize to JSON and put on the clipboard
Person person = new("Alisha", 34);
Clipboard.SetDataAsJson(person);
```
Internally `DataObject.SetDataAsJson<T>` writes the JSON to the underlying OLE data stream in the NRBF format in a way that it can be safely extracted without using the `BinaryFormatter`.


### `ITypedDataObject` interface – enabling typed data retrieval

Custom data objects used with drag & drop should implement the **`ITypedDataObject`** interface.  This interface declares the typed `TryGetData<T>` overloads. Implementing it signals to WinForms that your data object is capable of deserializing data into specific types; if it is not implemented, calls to the typed APIs throw `NotSupportedException`. The `DataObject` class already implements `ITypedDataObject`.

### Summary of new API signatures

| API (class) | Description | Remarks |
|---|---|---|
| `DataObject.SetDataAsJson<T>(string format, T data)` and `DataObject.SetDataAsJson<T>(T data)` | Serialize an object to JSON and store it under the specified format (or the type’s full name). | Uses `System.Text.Json`. |
| `Clipboard.SetDataAsJson<T>(string format, T data)` | Clears the clipboard and stores JSON data in a new `DataObject`. | Equivalent VB method: `ClipboardProxy.SetDataAsJson(Of T)`. |
| `DataObject.TryGetData<T>(string format, Func<TypeName,Type> resolver, bool autoConvert, out T data)` | Attempts to read data and deserialize it; `resolver` controls allowed types when falling back to binary deserialization; `autoConvert` indicates whether OLE conversion should be attempted. | Returns `true` when successful; does not throw. |
| `DataObject.TryGetData<T>(string format, bool autoConvert, out T data)` | Same as above without a resolver; does not fall back to binary serialization. | |
| `DataObject.TryGetData<T>(string format, out T data)` | Uses default `autoConvert=true`; no resolver. | |
| `DataObject.TryGetData<T>(out T data)` | Infers the format from the full name of `T`. | |
| `Clipboard.TryGetData<T>` overloads | Static methods that obtain the current clipboard data and delegate to the corresponding `DataObject` overloads. | Overloads include versions with and without resolver and autoConvert. |
| `ITypedDataObject` | Interface declaring typed `TryGetData` methods. | Implement on custom data objects used for drag & drop. |
| `DataObjectExtensions.TryGetData<T>` | Extension methods on `IDataObject` that call through to `ITypedDataObject`; throws when the underlying object does not implement `ITypedDataObject`. | Enables typed retrieval without explicitly casting. |
| `ClipboardProxy` (VB) new methods | Exposes `SetDataAsJson(Of T)`, `TryGetData(Of T)` and `TryGetData(Of T)(resolver)` in Visual Basic’s `My.Computer.Clipboard`. | VB apps can use typed clipboard APIs without referencing the `System.Windows.Forms` namespace directly. |

## Enabling full `BinaryFormatter` support (not recommended)

Although the default behaviour removes `BinaryFormatter`, a full fallback is still available **for legacy applications**.  To enable it you must do **all** of the following:

1. **Reference the `System.Runtime.Serialization.Formatters` package**:  Add a PackageReference to your project file:

   ```xml
   <ItemGroup>
     <PackageReference Include="System.Runtime.Serialization.Formatters" Version="10.0.0"/>
   </ItemGroup>
   ```

   This compatibility package is unsupported and dangerous; use it only as a last resort.

2. **Set the runtime switch to enable unsafe serialization**:  In your project file, set the property `EnableUnsafeBinaryFormatterSerialization` to `true`.  Without this switch, calls to binary serialization throw.

    ```xml
    <PropertyGroup>
      <EnableUnsafeBinaryFormatterSerialization>true</EnableUnsafeBinaryFormatterSerialization>
    </PropertyGroup>
    ```

3. **Set the WinForms‑specific switch**: WinForms adds an app‑context switch `Windows.ClipboardDragDrop.EnableUnsafeBinaryFormatterSerialization`. Without this switch, WinForms will not fall back to `BinaryFormatter` even if the general serialization switch is enabled. Set this flag in your `runtimeconfig.json` file:

    ```json
    {
        "configProperties": {
            "Windows.ClipboardDragDrop.EnableUnsafeBinaryFormatterSerialization": true
        }
    }
    ```

4. **Use the resolver overload to control which types are allowed**: Even with `BinaryFormatter` enabled, you must specify a resolver when calling `TryGetData<T>` so you only deserialize known safe types. The existing, deprecated, `GetData` APIs will continue to work, but provide no additional constraints. A sample type resolver follows:

    ```csharp
    static Type MyExactMatchResolver(TypeName typeName)
    {
        (Type type, TypeName typeName)[] allowedTypes =
        [
            (typeof(MyClass1), TypeName.Parse(typeof(MyClass1).AssemblyQualifiedName)),
            (typeof(MyClass2), TypeName.Parse(typeof(MyClass2).AssemblyQualifiedName))
        ];

        foreach (var (type, name) in allowedTypes)
        {
            // Check the type name
            if (name.FullName != typeName.FullName)
            {
                continue;
            }

            // You should also consider checking the `.AssemblyName` here as well.
            return type;
        }

        // Always throw to prevent the BinaryFormatter from implicitly loading assemblies.    
        throw new NotSupportedException($"Can't resolve {typeName.AssemblyQualifiedName}");
    }
    ```

Only after completing **all of the first three steps** above will WinForms behave as in older versions and automatically use `BinaryFormatter` for unknown types.

## Compatibility

### Predefined formats and primitive types

Types already defined in [`DataFormats`](https://learn.microsoft.com/dotnet/api/system.windows.forms.dataformats) and the primitive types described in the beginning of the document just work. This ensures compatibility with existing applications and common data exchange scenarios. 

### Reading JSON down‑level

Even though `SetDataAsJson<T>` is only available in .NET 10, you can round‑trip JSON data across processes and frameworks by storing the JSON yourself (as UTF-8 `byte[]` or `string`) if you're able to change all of the consumer code.

### Reading legacy NRBF / binary data in .NET 10

While .NET 10 removes `BinaryFormatter` by default, WinForms still includes safe support for the types mentioned above and provides mechanisms to read legacy binary payloads **without referencing `BinaryFormatter`**. The simplest way to read NRBF data is to use the `NrbfDecoder` `SerializationRecord`:

```csharp
if (dataObject.TryGetData("LegacyFormat", out SerializationRecord? record))
{
    // Process the serialization record
}
```

You can also retrieve raw data as a `MemoryStream` for custom processing.

## Links

- [BinaryFormatter migration guide](https://learn.microsoft.com/dotnet/standard/serialization/binaryformatter-migration-guide/)
- [Windows Forms migration guide for BinaryFormatter](https://learn.microsoft.com/dotnet/standard/serialization/binaryformatter-migration-guide/winforms-applications)
- [Windows Forms and Windows Presentation Foundation BinaryFormatter OLE guidance](https://learn.microsoft.com/dotnet/standard/serialization/binaryformatter-migration-guide)
- [System.Text.Json documentation](https://learn.microsoft.com/dotnet/standard/serialization/system-text-json/overview)
- [.NET Core runtime configuration settings](https://learn.microsoft.com/dotnet/core/runtime-config/)
- [Windows Forms Clipboard class](https://learn.microsoft.com/dotnet/api/system.windows.forms.clipboard)
- [DataObject class](https://learn.microsoft.com/dotnet/api/system.windows.forms.dataobject)
- [NRBF format specification](https://learn.microsoft.com/openspecs/windows_protocols/ms-nrbf/75b9fe09-be15-475f-85b8-ae7b7558cfe5)
