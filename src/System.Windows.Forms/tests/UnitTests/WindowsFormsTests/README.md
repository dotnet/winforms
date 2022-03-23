Supplementary native parts for WinForms unit tests
==================================================

This project is native DLL which provide unmanaged parts required to testing of WinForms components. Most interesting part of the tests would be COM objects which given to controls.
This project builds as native DLL, which can be used as reg-free COM host. That allow run tests and do not worry about polluting the developer/build machines.

Preferably each test case should have their separate COM objects. 

Manifests used in the application:

- `App.manifest` This file used during running test to provide information about this DLL. Developers do not need to modify that file, just run it inside activation context.
- `System_Windows_Forms_NativeTests.X.manifest` This is manifest where all reg-free COM objects registered.

## How to add new COM object

1. Declare COM object in `Contract.idl` file 
2. Implement COM object in C++.
3. Add CoClass defined in `Contract.idl` inside `System_Windows_Forms_NativeTests.X.manifest`
