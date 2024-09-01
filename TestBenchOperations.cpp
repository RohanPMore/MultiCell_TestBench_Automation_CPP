#include "TestBenchOperations.hpp"
#include "MainWindow.h"  // Include MainWindow header
#include <iostream>

//TestBenchOperations::TestBenchOperations(int testBenchNumber, int cellNumber, TestOperations& sharedOperations)
    //: testBenchNumber_(testBenchNumber), cellNumber_(cellNumber), operations_(sharedOperations) {}

TestBenchOperations::TestBenchOperations(int testBenchNumber, int cellNumber, TestOperations& sharedOperations, MainWindow *mainWindow)
    : testBenchNumber_(testBenchNumber), cellNumber_(cellNumber), operations_(sharedOperations), mainWindow_(mainWindow) {}

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
    //std::cout << "Starting CCCV Charge Cycle on Test Bench: " << testBenchNumber_ << ", Cell: " << cellNumber_ << std::endl;
    mainWindow_->updateStatus(QString("Starting CCCV Charge Cycle on Test Bench: %1, Cell: %2").arg(testBenchNumber_).arg(cellNumber_));

    operations_.subMethod1();
    mainWindow_->updateProgress(25);

    operations_.subMethod2();
    mainWindow_->updateProgress(50);

    operations_.subMethod3();
    mainWindow_->updateProgress(75);

    operations_.subMethod4();
    mainWindow_->updateProgress(100);

    //std::cout << "CCCV Charge Cycle completed on Test Bench: " << testBenchNumber_
              //<< ", Cell: " << cellNumber_ << std::endl;
    mainWindow_->updateStatus(QString("CCCV Charge Cycle completed on Test Bench: %1, Cell: %2").arg(testBenchNumber_).arg(cellNumber_));
}

void TestBenchOperations::performCCDischargeCycle() {
    //std::cout << "Starting CC Discharge Cycle on Test Bench: " << testBenchNumber_ << ", Cell: " << cellNumber_ << std::endl;
    mainWindow_->updateStatus(QString("Starting CC Discharge Cycle on Test Bench: %1, Cell: %2").arg(testBenchNumber_).arg(cellNumber_));

    operations_.subMethod1();
    mainWindow_->updateProgress(25);

    operations_.subMethod2();
    mainWindow_->updateProgress(50);

    operations_.subMethod3();
    mainWindow_->updateProgress(75);

    operations_.subMethod4();
    mainWindow_->updateProgress(100);

    //std::cout << "CC Discharge Cycle completed on Test Bench: " << testBenchNumber_
              //<< ", Cell: " << cellNumber_ << std::endl;

    mainWindow_->updateStatus(QString("CC Discharge Cycle completed on Test Bench: %1, Cell: %2").arg(testBenchNumber_).arg(cellNumber_));          
}

void TestBenchOperations::performRPTTest() {
    //std::cout << "Starting RPT Test on Test Bench: " << testBenchNumber_ << ", Cell: " << cellNumber_ << std::endl;
    mainWindow_->updateStatus(QString("Starting RPT Test on Test Bench: %1, Cell: %2").arg(testBenchNumber_).arg(cellNumber_));

    operations_.subMethod1();
    mainWindow_->updateProgress(25);

    operations_.subMethod2();
    mainWindow_->updateProgress(50);

    operations_.subMethod3();
    mainWindow_->updateProgress(75);

    operations_.subMethod4();
    mainWindow_->updateProgress(100);

    //std::cout << "RPT Test completed on Test Bench: " << testBenchNumber_
              //<< ", Cell: " << cellNumber_ << std::endl;

    mainWindow_->updateStatus(QString("RPT Test completed on Test Bench: %1, Cell: %2").arg(testBenchNumber_).arg(cellNumber_));
}