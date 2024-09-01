#ifndef TESTBENCHOPERATIONS_HPP
#define TESTBENCHOPERATIONS_HPP

#include "TestOperations.hpp"
#include "TestType.hpp"
#include <QObject> // 01.09 Updated
#include <thread>
#include <mutex>
#include <stdexcept>

// Forward declaration of MainWindow class
class MainWindow;
 
//class TestBenchOperations {
class TestBenchOperations : public QObject { // 01.09 Updated
    Q_OBJECT // 01.09 Updated

public:
    //TestBenchOperations(int testBenchNumber, int cellNumber, TestOperations& sharedOperations);
    TestBenchOperations(int testBenchNumber, int cellNumber, TestOperations& sharedOperations, MainWindow *mainWindow);
    
    void performTest(TestType testType);
    
    //bool isTestRunning() const; 

    //class TestRunningException : public std::runtime_error {
    //public:
        //TestRunningException(const std::string& message) : std::runtime_error(message) {}
    //};
    void performCCCVChargeCycle();
    void performCCDischargeCycle();
    void performRPTTest();

signals: // 01.09 Updated 
    void testStatusUpdated(const QString &status);
    void progressUpdated(int value);
    void temperatureUpdated(double temperature);
    void voltageUpdated(double voltage);

private:
    int testBenchNumber_;
    int cellNumber_;
    TestOperations& operations_;  // Shared resource
    std::mutex threadMutex_;  // To ensure one test bench runs only one test at a time
    MainWindow *mainWindow_; // Pointer to the main window for UI updates
   // bool testRunning_ = false;
};

#endif // TESTBENCHOPERATIONS_HPP
