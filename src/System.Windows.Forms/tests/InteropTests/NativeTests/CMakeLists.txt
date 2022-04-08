cmake_minimum_required (VERSION 2.8.12)
project (NativeTests)
set(CMAKE_MACOSX_RPATH 1)
set(CMAKE_CXX_STANDARD 11)
set(CMAKE_CXX_STANDARD_REQUIRED ON)

# Compile IDL file using MIDL
set(IDL_SOURCE ${CMAKE_CURRENT_SOURCE_DIR}/Contract.idl)
get_filename_component(IDL_NAME ${IDL_SOURCE} NAME_WE)


FIND_PROGRAM( MIDL midl.exe )
set(IDL_OUTPUT_DIRECTORY ${CMAKE_CURRENT_BINARY_DIR}/Contract)
add_custom_command(
    OUTPUT ${IDL_OUTPUT_DIRECTORY}/${IDL_NAME}_i.c ${IDL_OUTPUT_DIRECTORY}/${IDL_NAME}.h
    COMMAND ${MIDL} ${MIDL_INCLUDE_DIRECTORIES}
        /h ${IDL_OUTPUT_DIRECTORY}/${IDL_NAME}.h ${MIDL_DEFINITIONS}
        /out ${IDL_OUTPUT_DIRECTORY}
        /tlb $<TARGET_FILE_DIR:${PROJECT_NAME}>/NativeTests.tlb
        ${IDL_SOURCE}
    DEPENDS ${IDL_SOURCE}
    COMMENT "Compiling ${IDL_SOURCE}")

include_directories(${IDL_OUTPUT_DIRECTORY})

# [[! Microsoft.Security.SystemsADM.10086 !]] - SQL required warnings
add_compile_options($<$<COMPILE_LANGUAGE:C,CXX>:/W3>) # warning level 3
add_compile_options($<$<COMPILE_LANGUAGE:C,CXX>:/WX>) # treat warnings as errors
add_compile_options($<$<COMPILE_LANGUAGE:C,CXX>:/we4018>) # 'expression' : signed/unsigned mismatch
add_compile_options($<$<COMPILE_LANGUAGE:C,CXX>:/we4055>) # 'conversion' : from data pointer 'type1' to function pointer 'type2'


add_library(NativeTests SHARED
    AccessibleObjectTests.cpp
    DllGetClassObject.cpp
    DispatchImpl.cpp
    RawErrorInfoUsageTest.cpp
    RichTextBoxTests.cpp
    StandardErrorInfoUsageTest.cpp
    WebBrowserSiteBaseInteropTests.cpp

    Exports.def
    ${IDL_OUTPUT_DIRECTORY}/${IDL_NAME}_i.c
)
file(GENERATE OUTPUT $<TARGET_FILE_DIR:${PROJECT_NAME}>/NativeTests.X.manifest INPUT ${CMAKE_CURRENT_SOURCE_DIR}/NativeTests.X.manifest)
file(GENERATE OUTPUT $<TARGET_FILE_DIR:${PROJECT_NAME}>/App.manifest INPUT ${CMAKE_CURRENT_SOURCE_DIR}/App.manifest)

install(TARGETS NativeTests)
install(FILES $<TARGET_FILE_DIR:${PROJECT_NAME}>/NativeTests.tlb TYPE BIN)
install(FILES $<TARGET_FILE_DIR:${PROJECT_NAME}>/NativeTests.X.manifest TYPE BIN)
install(FILES $<TARGET_FILE_DIR:${PROJECT_NAME}>/App.manifest TYPE BIN)
