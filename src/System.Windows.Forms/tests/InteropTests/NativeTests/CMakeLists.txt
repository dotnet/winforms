cmake_minimum_required (VERSION 2.8.12)
project (NativeTests)
set(CMAKE_MACOSX_RPATH 1)
set(CMAKE_CXX_STANDARD 11)
set(CMAKE_CXX_STANDARD_REQUIRED ON)


# [[! Microsoft.Security.SystemsADM.10086 !]] - SQL required warnings
add_compile_options($<$<COMPILE_LANGUAGE:C,CXX>:/W3>) # warning level 3
add_compile_options($<$<COMPILE_LANGUAGE:C,CXX>:/WX>) # treat warnings as errors
add_compile_options($<$<COMPILE_LANGUAGE:C,CXX>:/we4018>) # 'expression' : signed/unsigned mismatch
add_compile_options($<$<COMPILE_LANGUAGE:C,CXX>:/we4055>) # 'conversion' : from data pointer 'type1' to function pointer 'type2'


add_library(NativeTests SHARED
    AccessibleObjectTests.cpp
    WebBrowserSiteBaseInteropTests.cpp
)
install(TARGETS NativeTests)
