cmake_minimum_required(VERSION 3.10)

project(MultiCell-TestBench-Automation)

set(CMAKE_AUTOMOC ON)  # Ensure that moc is automatically run
set(CMAKE_AUTORCC ON)
set(CMAKE_AUTOUIC ON)


set(CMAKE_CXX_STANDARD 17)
set(CMAKE_CXX_STANDARD_REQUIRED ON)

find_package(Qt6 REQUIRED COMPONENTS Widgets)

add_executable(MultiCell-TestBench-Automation main.cpp MainWindow.cpp MainWindow.h)

target_link_libraries(MultiCell-TestBench-Automation Qt6::Widgets)
