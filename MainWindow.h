#ifndef MAINWINDOW_H
#define MAINWINDOW_H

#include <QMainWindow>
#include <QLabel>
#include <QProgressBar>
#include <QLCDNumber>
#include <QPushButton>  // Required for QPushButton
#include <QMenuBar>     // Required for QMenuBar
#include <QMenu>        // Required for QMenu
#include <QToolBar>     // Required for QToolBar
#include <QTextEdit>

class MainWindow : public QMainWindow {
    Q_OBJECT  // This is critical for QObject-based classes

public:
    MainWindow(QWidget *parent = nullptr);
    ~MainWindow();
    void updateStatus(const QString &status);
    void updateProgress(int value);
    void updateTemperature(double temperature);
    void updateVoltage(double voltage);

private:
    QTextEdit *messageDisplay;  // Text area to display messages
    void setupMenuBar();
    //void setupToolBar();
    void setupCentralWidget();
    void setupRightPanel();

private slots:
    void onRunClicked();  // Slot to handle button click
    void onStopClicked();
    void onStartTestClicked();
    void onTestBenchOptionSelected(int testBenchNumber, const QString &option); // Slots for handling cell selection in Test Benches
    //void onViewDBCMessage();

private:
    QPushButton *startButton;

    // Member variables for the display widgets
    QLabel *testBenchLabel;
    QLabel *cellNumberLabel;
    QLabel *testTypeLabel;
    QProgressBar *progressBar;
    QLCDNumber *temperatureDisplay;
    QLCDNumber *voltageDisplay;
};

#endif // MAINWINDOW_H
