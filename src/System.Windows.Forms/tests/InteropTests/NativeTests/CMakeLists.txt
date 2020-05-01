cmake_minimum_required (VERSION 2.6)
project (NativeTests)
set(CMAKE_MACOSX_RPATH 1)
set(CMAKE_CXX_STANDARD 11)
set(CMAKE_CXX_STANDARD_REQUIRED ON)

add_library(NativeTests SHARED
    WebBrowserSiteBaseInteropTests.cpp
)
install(TARGETS NativeTests)
