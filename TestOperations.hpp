#ifndef TESTOPERATIONS_HPP
#define TESTOPERATIONS_HPP

#include <mutex>

class TestOperations {
public:
    void subMethod1();
    void subMethod2();
    void subMethod3();
    void subMethod4();

private:
    std::mutex mutex_;
};

#endif // TESTOPERATIONS_HPP
