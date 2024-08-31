#ifndef TESTBENCHOPERATIONS_HPP
#define TESTBENCHOPERATIONS_HPP

#include "TestOperations.hpp"
#include "TestType.hpp"
#include <thread>
#include <mutex>
#include <stdexcept>

class TestBenchOperations {
public:
    TestBenchOperations(int testBenchNumber, int cellNumber, TestOperations& sharedOperations);
    
    void performTest(TestType testType);
    
    //bool isTestRunning() const; 

    //class TestRunningException : public std::runtime_error {
    //public:
        //TestRunningException(const std::string& message) : std::runtime_error(message) {}
    //};

private:
    int testBenchNumber_;
    int cellNumber_;
    TestOperations& operations_;  // Shared resource
    std::mutex threadMutex_;  // To ensure one test bench runs only one test at a time
   // bool testRunning_ = false;

    void performCCCVChargeCycle();
    void performCCDischargeCycle();
    void performRPTTest();
};

#endif // TESTBENCHOPERATIONS_HPP
