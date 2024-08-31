#include "TestOperations.hpp"
#include <iostream>

void TestOperations::subMethod1() {
    std::lock_guard<std::mutex> lock(mutex_);
    std::cout << "Executing SubMethod 1 (This will take some time)" << std::endl;

    const unsigned long long countLimit = 10000000000ULL;
    unsigned long long counter = 0;

    while (counter < countLimit) {
        ++counter;
    }

    std::cout << "SubMethod 1 completed." << std::endl;
}

void TestOperations::subMethod2() {
    std::lock_guard<std::mutex> lock(mutex_);
    std::cout << "Executing SubMethod 2" << std::endl;

    const unsigned long long countLimit = 500000000ULL;
    unsigned long long counter = 0;

    while (counter < countLimit) {
        ++counter;
    }

    std::cout << "SubMethod 2 completed." << std::endl;
}

void TestOperations::subMethod3() {
    std::lock_guard<std::mutex> lock(mutex_);
    std::cout << "Executing SubMethod 3" << std::endl;

    const unsigned long long countLimit = 500000000ULL;
    unsigned long long counter = 0;

    while (counter < countLimit) {
        ++counter;
    }

    std::cout << "SubMethod 3 completed." << std::endl;
}

void TestOperations::subMethod4() {
    std::lock_guard<std::mutex> lock(mutex_);
    std::cout << "Executing SubMethod 4" << std::endl;

    const unsigned long long countLimit = 500000000ULL;
    unsigned long long counter = 0;

    while (counter < countLimit) {
        ++counter;
    }

    std::cout << "SubMethod 4 completed." << std::endl;
}
