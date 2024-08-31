#include "TestBenchOperations.hpp"
#include <iostream>

TestBenchOperations::TestBenchOperations(int testBenchNumber, int cellNumber, TestOperations& sharedOperations)
    : testBenchNumber_(testBenchNumber), cellNumber_(cellNumber), operations_(sharedOperations) {}

void TestBenchOperations::performTest(TestType testType) {
    std::lock_guard<std::mutex> lock(threadMutex_);  // Ensure only one test per test bench

    switch (testType) {
    case TestType::CCCV_ChargeCycle:
        performCCCVChargeCycle();
        break;
    case TestType::CC_DischargeCycle:
        performCCDischargeCycle();
        break;
    case TestType::RPT_Test:
        performRPTTest();
        break;
    }
}

void TestBenchOperations::performCCCVChargeCycle() {
    std::cout << "Starting CCCV Charge Cycle on Test Bench: " << testBenchNumber_ << ", Cell: " << cellNumber_ << std::endl;

    operations_.subMethod1();
    operations_.subMethod2();
    operations_.subMethod3();
    operations_.subMethod4();

    std::cout << "CCCV Charge Cycle completed on Test Bench: " << testBenchNumber_
              << ", Cell: " << cellNumber_ << std::endl;
}

void TestBenchOperations::performCCDischargeCycle() {
    std::cout << "Starting CC Discharge Cycle on Test Bench: " << testBenchNumber_ << ", Cell: " << cellNumber_ << std::endl;

    operations_.subMethod1();
    operations_.subMethod2();
    operations_.subMethod3();
    operations_.subMethod4();

    std::cout << "CC Discharge Cycle completed on Test Bench: " << testBenchNumber_
              << ", Cell: " << cellNumber_ << std::endl;
}

void TestBenchOperations::performRPTTest() {
    std::cout << "Starting RPT Test on Test Bench: " << testBenchNumber_ << ", Cell: " << cellNumber_ << std::endl;

    operations_.subMethod1();
    operations_.subMethod2();
    operations_.subMethod3();
    operations_.subMethod4();

    std::cout << "RPT Test completed on Test Bench: " << testBenchNumber_
              << ", Cell: " << cellNumber_ << std::endl;
}