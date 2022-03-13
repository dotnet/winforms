cmake_minimum_required (VERSION 2.8.12)
project (System_Windows_Forms_NativeTests)
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
        /tlb $<TARGET_FILE_DIR:${PROJECT_NAME}>/System_Windows_Forms_NativeTests.tlb
        ${IDL_SOURCE}
    DEPENDS ${IDL_SOURCE}
    COMMENT "Compiling ${IDL_SOURCE}")

include_directories(${IDL_OUTPUT_DIRECTORY})

add_library(System_Windows_Forms_NativeTests SHARED
    IErrorInfoObject.cpp
    DispatchImpl.cpp
    StandardErrorInfoUsageTest.cpp
    RawErrorInfoUsageTest.cpp
    ${IDL_OUTPUT_DIRECTORY}/${IDL_NAME}_i.c
)

file(GENERATE OUTPUT $<TARGET_FILE_DIR:${PROJECT_NAME}>/System_Windows_Forms_NativeTests.X.manifest INPUT ${CMAKE_CURRENT_SOURCE_DIR}/System_Windows_Forms_NativeTests.X.manifest)

install(TARGETS System_Windows_Forms_NativeTests)
install(FILES $<TARGET_FILE_DIR:${PROJECT_NAME}>/System_Windows_Forms_NativeTests.tlb TYPE BIN)
