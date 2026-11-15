cmake_minimum_required(VERSION 3.20)
project(fidget_native_smoke C)
enable_testing()

set(FIDGET_LIBRARY "" CACHE FILEPATH "Built shared library (import library on Windows)")
set(FIDGET_RUNTIME "" CACHE FILEPATH "Built DLL on Windows")
if(NOT EXISTS "${FIDGET_LIBRARY}")
    message(FATAL_ERROR "Set FIDGET_LIBRARY to the built native library")
endif()

add_executable(fidget_native_smoke smoke.c)
target_compile_features(fidget_native_smoke PRIVATE c_std_99)
target_include_directories(fidget_native_smoke PRIVATE ../include)
target_link_libraries(fidget_native_smoke PRIVATE "${FIDGET_LIBRARY}")
if(WIN32)
    if(NOT EXISTS "${FIDGET_RUNTIME}")
        message(FATAL_ERROR "Set FIDGET_RUNTIME to the built DLL")
    endif()
    add_custom_command(TARGET fidget_native_smoke POST_BUILD
        COMMAND ${CMAKE_COMMAND} -E copy_if_different
        "${FIDGET_RUNTIME}" "$<TARGET_FILE_DIR:fidget_native_smoke>")
else()
    target_link_libraries(fidget_native_smoke PRIVATE m)
endif()

add_test(NAME native_sphere COMMAND fidget_native_smoke)
