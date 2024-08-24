# Distributed under the OSI-approved BSD 3-Clause License.  See accompanying
# file Copyright.txt or https://cmake.org/licensing for details.

cmake_minimum_required(VERSION 3.5)

if(EXISTS "N:/Programming_VS/Cpp/multicell-testbench-automation/MultiCell-TestBench-Automation/out/build/GCC 11.2.0 x86_64-w64-mingw32/_deps/libcanardbc-subbuild/libcanardbc-populate-prefix/src/libcanardbc-populate-stamp/libcanardbc-populate-gitclone-lastrun.txt" AND EXISTS "N:/Programming_VS/Cpp/multicell-testbench-automation/MultiCell-TestBench-Automation/out/build/GCC 11.2.0 x86_64-w64-mingw32/_deps/libcanardbc-subbuild/libcanardbc-populate-prefix/src/libcanardbc-populate-stamp/libcanardbc-populate-gitinfo.txt" AND
  "N:/Programming_VS/Cpp/multicell-testbench-automation/MultiCell-TestBench-Automation/out/build/GCC 11.2.0 x86_64-w64-mingw32/_deps/libcanardbc-subbuild/libcanardbc-populate-prefix/src/libcanardbc-populate-stamp/libcanardbc-populate-gitclone-lastrun.txt" IS_NEWER_THAN "N:/Programming_VS/Cpp/multicell-testbench-automation/MultiCell-TestBench-Automation/out/build/GCC 11.2.0 x86_64-w64-mingw32/_deps/libcanardbc-subbuild/libcanardbc-populate-prefix/src/libcanardbc-populate-stamp/libcanardbc-populate-gitinfo.txt")
  message(VERBOSE
    "Avoiding repeated git clone, stamp file is up to date: "
    "'N:/Programming_VS/Cpp/multicell-testbench-automation/MultiCell-TestBench-Automation/out/build/GCC 11.2.0 x86_64-w64-mingw32/_deps/libcanardbc-subbuild/libcanardbc-populate-prefix/src/libcanardbc-populate-stamp/libcanardbc-populate-gitclone-lastrun.txt'"
  )
  return()
endif()

# Even at VERBOSE level, we don't want to see the commands executed, but
# enabling them to be shown for DEBUG may be useful to help diagnose problems.
cmake_language(GET_MESSAGE_LOG_LEVEL active_log_level)
if(active_log_level MATCHES "DEBUG|TRACE")
  set(maybe_show_command "COMMAND_ECHO STDOUT")
else()
  set(maybe_show_command "")
endif()

execute_process(
  COMMAND ${CMAKE_COMMAND} -E rm -rf "N:/Programming_VS/Cpp/multicell-testbench-automation/MultiCell-TestBench-Automation/out/build/GCC 11.2.0 x86_64-w64-mingw32/_deps/libcanardbc-src"
  RESULT_VARIABLE error_code
  ${maybe_show_command}
)
if(error_code)
  message(FATAL_ERROR "Failed to remove directory: 'N:/Programming_VS/Cpp/multicell-testbench-automation/MultiCell-TestBench-Automation/out/build/GCC 11.2.0 x86_64-w64-mingw32/_deps/libcanardbc-src'")
endif()

# try the clone 3 times in case there is an odd git clone issue
set(error_code 1)
set(number_of_tries 0)
while(error_code AND number_of_tries LESS 3)
  execute_process(
    COMMAND "C:/Program Files/Git/cmd/git.exe"
            clone --no-checkout --config "advice.detachedHead=false" "https://github.com/Polyconseil/libcanardbc.git" "libcanardbc-src"
    WORKING_DIRECTORY "N:/Programming_VS/Cpp/multicell-testbench-automation/MultiCell-TestBench-Automation/out/build/GCC 11.2.0 x86_64-w64-mingw32/_deps"
    RESULT_VARIABLE error_code
    ${maybe_show_command}
  )
  math(EXPR number_of_tries "${number_of_tries} + 1")
endwhile()
if(number_of_tries GREATER 1)
  message(NOTICE "Had to git clone more than once: ${number_of_tries} times.")
endif()
if(error_code)
  message(FATAL_ERROR "Failed to clone repository: 'https://github.com/Polyconseil/libcanardbc.git'")
endif()

execute_process(
  COMMAND "C:/Program Files/Git/cmd/git.exe"
          checkout "libcanardbc-1.0.0" --
  WORKING_DIRECTORY "N:/Programming_VS/Cpp/multicell-testbench-automation/MultiCell-TestBench-Automation/out/build/GCC 11.2.0 x86_64-w64-mingw32/_deps/libcanardbc-src"
  RESULT_VARIABLE error_code
  ${maybe_show_command}
)
if(error_code)
  message(FATAL_ERROR "Failed to checkout tag: 'libcanardbc-1.0.0'")
endif()

set(init_submodules TRUE)
if(init_submodules)
  execute_process(
    COMMAND "C:/Program Files/Git/cmd/git.exe" 
            submodule update --recursive --init 
    WORKING_DIRECTORY "N:/Programming_VS/Cpp/multicell-testbench-automation/MultiCell-TestBench-Automation/out/build/GCC 11.2.0 x86_64-w64-mingw32/_deps/libcanardbc-src"
    RESULT_VARIABLE error_code
    ${maybe_show_command}
  )
endif()
if(error_code)
  message(FATAL_ERROR "Failed to update submodules in: 'N:/Programming_VS/Cpp/multicell-testbench-automation/MultiCell-TestBench-Automation/out/build/GCC 11.2.0 x86_64-w64-mingw32/_deps/libcanardbc-src'")
endif()

# Complete success, update the script-last-run stamp file:
#
execute_process(
  COMMAND ${CMAKE_COMMAND} -E copy "N:/Programming_VS/Cpp/multicell-testbench-automation/MultiCell-TestBench-Automation/out/build/GCC 11.2.0 x86_64-w64-mingw32/_deps/libcanardbc-subbuild/libcanardbc-populate-prefix/src/libcanardbc-populate-stamp/libcanardbc-populate-gitinfo.txt" "N:/Programming_VS/Cpp/multicell-testbench-automation/MultiCell-TestBench-Automation/out/build/GCC 11.2.0 x86_64-w64-mingw32/_deps/libcanardbc-subbuild/libcanardbc-populate-prefix/src/libcanardbc-populate-stamp/libcanardbc-populate-gitclone-lastrun.txt"
  RESULT_VARIABLE error_code
  ${maybe_show_command}
)
if(error_code)
  message(FATAL_ERROR "Failed to copy script-last-run stamp file: 'N:/Programming_VS/Cpp/multicell-testbench-automation/MultiCell-TestBench-Automation/out/build/GCC 11.2.0 x86_64-w64-mingw32/_deps/libcanardbc-subbuild/libcanardbc-populate-prefix/src/libcanardbc-populate-stamp/libcanardbc-populate-gitclone-lastrun.txt'")
endif()
