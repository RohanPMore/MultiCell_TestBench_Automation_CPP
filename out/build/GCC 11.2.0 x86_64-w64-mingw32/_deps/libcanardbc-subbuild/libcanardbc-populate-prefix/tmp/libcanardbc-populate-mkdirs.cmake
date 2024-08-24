# Distributed under the OSI-approved BSD 3-Clause License.  See accompanying
# file Copyright.txt or https://cmake.org/licensing for details.

cmake_minimum_required(VERSION 3.5)

# If CMAKE_DISABLE_SOURCE_CHANGES is set to true and the source directory is an
# existing directory in our source tree, calling file(MAKE_DIRECTORY) on it
# would cause a fatal error, even though it would be a no-op.
if(NOT EXISTS "N:/Programming_VS/Cpp/multicell-testbench-automation/MultiCell-TestBench-Automation/out/build/GCC 11.2.0 x86_64-w64-mingw32/_deps/libcanardbc-src")
  file(MAKE_DIRECTORY "N:/Programming_VS/Cpp/multicell-testbench-automation/MultiCell-TestBench-Automation/out/build/GCC 11.2.0 x86_64-w64-mingw32/_deps/libcanardbc-src")
endif()
file(MAKE_DIRECTORY
  "N:/Programming_VS/Cpp/multicell-testbench-automation/MultiCell-TestBench-Automation/out/build/GCC 11.2.0 x86_64-w64-mingw32/_deps/libcanardbc-build"
  "N:/Programming_VS/Cpp/multicell-testbench-automation/MultiCell-TestBench-Automation/out/build/GCC 11.2.0 x86_64-w64-mingw32/_deps/libcanardbc-subbuild/libcanardbc-populate-prefix"
  "N:/Programming_VS/Cpp/multicell-testbench-automation/MultiCell-TestBench-Automation/out/build/GCC 11.2.0 x86_64-w64-mingw32/_deps/libcanardbc-subbuild/libcanardbc-populate-prefix/tmp"
  "N:/Programming_VS/Cpp/multicell-testbench-automation/MultiCell-TestBench-Automation/out/build/GCC 11.2.0 x86_64-w64-mingw32/_deps/libcanardbc-subbuild/libcanardbc-populate-prefix/src/libcanardbc-populate-stamp"
  "N:/Programming_VS/Cpp/multicell-testbench-automation/MultiCell-TestBench-Automation/out/build/GCC 11.2.0 x86_64-w64-mingw32/_deps/libcanardbc-subbuild/libcanardbc-populate-prefix/src"
  "N:/Programming_VS/Cpp/multicell-testbench-automation/MultiCell-TestBench-Automation/out/build/GCC 11.2.0 x86_64-w64-mingw32/_deps/libcanardbc-subbuild/libcanardbc-populate-prefix/src/libcanardbc-populate-stamp"
)

set(configSubDirs )
foreach(subDir IN LISTS configSubDirs)
    file(MAKE_DIRECTORY "N:/Programming_VS/Cpp/multicell-testbench-automation/MultiCell-TestBench-Automation/out/build/GCC 11.2.0 x86_64-w64-mingw32/_deps/libcanardbc-subbuild/libcanardbc-populate-prefix/src/libcanardbc-populate-stamp/${subDir}")
endforeach()
if(cfgdir)
  file(MAKE_DIRECTORY "N:/Programming_VS/Cpp/multicell-testbench-automation/MultiCell-TestBench-Automation/out/build/GCC 11.2.0 x86_64-w64-mingw32/_deps/libcanardbc-subbuild/libcanardbc-populate-prefix/src/libcanardbc-populate-stamp${cfgdir}") # cfgdir has leading slash
endif()
