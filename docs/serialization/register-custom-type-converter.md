# Manually Registering a TypeConverter to a Class
Typically, a type converter is bound to a class by adding the [TypeConverterAttribute](https://learn.microsoft.com/en-us/dotnet/api/system.componentmodel.typeconverterattribute?view=net-7.0) to the class. 
However, there are some scenarios where it is not possible to add the attribute to the class e.g. user may not own the class, but there is no type converter registered to that class for serialization. This document outlines how to manually register a `TypeConverter` to a class for these scenarios.

## Prerequisite
Implement your own converter for a class. Ensure that your converter inherits from [TypeConverter](https://learn.microsoft.com/en-us/dotnet/api/system.componentmodel.typeconverter?view=net-7.0).

## Registering Your TypeConverter

### Background
`TypeDescriptor.GetConverter()` is typically called to get the `TypeConverter` for a class. This method will run through the `TypeDescriptor`'s list of `TypeDescriptionProvider`s and get the provider associated with the type/object. It will then call `TypeDescriptionProvider.GetTypeDescriptor()` to get an `ICustomTypeDescriptor` and call `ICustomTypeDescriptor.GetConverter()` to finally grab the `TypeConverter`. 
Thus to properly register your custom `TypeConverter`, we will need to override such method from `TypeDescriptionProvider` and implement `ICustomTypeDescriptor`.

### Inheriting CustomTypeDescriptor and TypeDescriptionProvider

#### CustomTypeDescriptor
The simplest way to implement `ICustomTypeDescriptor` is to inherit from `CustomTypeDescriptor`. This class already provides a simple default implementation of `ICustomTypeDescriptor` interface. All that is left to be done after inheriting `CustomTypeDescriptor` is to override `GetConverter()` to return your own `TypeConverter`.
```c#
private class MyCustomTypeConverterDescriptor : CustomTypeDescriptor
        {
            private static TypeConverter _myConverter;

            public MyCustomTypeConverterDescriptor(ICustomTypeDescriptor parent, TypeConverter converter) : base(parent)
                => _myConverter = converter;

            public override TypeConverter GetConverter() => _myConverter;
        }
```

#### TypeDescriptionProvider
Your inherited class will be added to the `TypeDescriptor`'s list of providers.
We will want to override `GetTypeDescriptor()` to return `MyCustomTypeConverterDescriptor`.
```c#
public class MyTypeDescriptionProvider : TypeDescriptionProvider
    {
        private TypeConverter _myConverter;

        public MyTypeDescriptionProvider(TypeDescriptionProvider parent, TypeConverter converter) : base(parent)
            => _myConverter = converter;

        public override ICustomTypeDescriptor GetTypeDescriptor(
            [DynamicallyAccessedMembers((DynamicallyAccessedMemberTypes)(-1))] Type objectType,
            object instance) => new MyCustomTypeConverterDescriptor(base.GetTypeDescriptor(objectType, instance), _myConverter);
    }
```
Note: The parent `TypeDescriptionProvider` to be passed to the constructor can be grabbed by calling `TypeDescriptor.GetProvider()` and passing in the object/type.

### Registration/Deregistration
Once you have `MyTypeDescriptionProvider` and `MyCustomTypeConverterDescriptor` as outlined above,
register your custom `TypeConverter` to a object/type by calling `TypeDescriptor.AddProvider()` with parameters `MyTypeDescriptionProvider`
and the object/type you want your custom converter to be associated with. At this point, calling `TypeDescriptor.GetConverter()` with the same object/type that was passed to `TypeDescriptor.AddProvider()` will return your custom `TypeConverter`! 
If you want this type converter to be temporarily registered to the object/type, do not forget to call `TypeDescriptor.RemoveProvider()` when finished.