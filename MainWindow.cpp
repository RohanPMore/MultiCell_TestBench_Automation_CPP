#include "MainWindow.h"
#include <QVBoxLayout>
#include <QHBoxLayout>
#include <QLabel>
#include <QMessageBox>
#include <QInputDialog>
#include <QDockWidget>
#include <QProgressBar>
#include <QLCDNumber>
#include <QPushButton>

MainWindow::MainWindow(QWidget *parent) : QMainWindow(parent) {
    setupMenuBar();
    //setupToolBar();
    setupCentralWidget();
    setupRightPanel();
}

MainWindow::~MainWindow() {}

void MainWindow::setupMenuBar() {
    // Menu Bar Setup
    QMenu *fileMenu = menuBar()->addMenu("File");
    QAction *exitAction = new QAction("Exit", this);
    fileMenu->addAction(exitAction);
    connect(exitAction, &QAction::triggered, this, &QMainWindow::close);

    QMenu *editMenu = menuBar()->addMenu("Edit");
    QAction *preferencesAction = new QAction("Preferences", this);
    editMenu->addAction(preferencesAction);

    // Test Menu with Submenus for Test Benches and Options
    QMenu *testMenu = menuBar()->addMenu("Test");

    // Test Bench 1 Submenu
    QMenu *testBench1Menu = testMenu->addMenu("Test Bench 1");
    QAction *cccvCharge1 = new QAction("CCCV Charge Cycle", this);
    QAction *ccDischarge1 = new QAction("CC Discharge Cycle", this);
    QAction *rptTest1 = new QAction("RPT (Rapid Pulse Test)", this);
    testBench1Menu->addAction(cccvCharge1);
    testBench1Menu->addAction(ccDischarge1);
    testBench1Menu->addAction(rptTest1);

    connect(cccvCharge1, &QAction::triggered, this, [this]() { onTestBenchOptionSelected(1, "CCCV Charge Cycle"); });
    connect(ccDischarge1, &QAction::triggered, this, [this]() { onTestBenchOptionSelected(1, "CC Discharge Cycle"); });
    connect(rptTest1, &QAction::triggered, this, [this]() { onTestBenchOptionSelected(1, "RPT (Rapid Pulse Test)"); });

    // Test Bench 2 Submenu
    QMenu *testBench2Menu = testMenu->addMenu("Test Bench 2");
    QAction *cccvCharge2 = new QAction("CCCV Charge Cycle", this);
    QAction *ccDischarge2 = new QAction("CC Discharge Cycle", this);
    QAction *rptTest2 = new QAction("RPT (Rapid Pulse Test)", this);
    testBench2Menu->addAction(cccvCharge2);
    testBench2Menu->addAction(ccDischarge2);
    testBench2Menu->addAction(rptTest2);

    connect(cccvCharge2, &QAction::triggered, this, [this]() { onTestBenchOptionSelected(2, "CCCV Charge Cycle"); });
    connect(ccDischarge2, &QAction::triggered, this, [this]() { onTestBenchOptionSelected(2, "CC Discharge Cycle"); });
    connect(rptTest2, &QAction::triggered, this, [this]() { onTestBenchOptionSelected(2, "RPT (Rapid Pulse Test)"); });

    // Test Bench 3 Submenu
    QMenu *testBench3Menu = testMenu->addMenu("Test Bench 3");
    QAction *cccvCharge3 = new QAction("CCCV Charge Cycle", this);
    QAction *ccDischarge3 = new QAction("CC Discharge Cycle", this);
    QAction *rptTest3 = new QAction("RPT (Rapid Pulse Test)", this);
    testBench3Menu->addAction(cccvCharge3);
    testBench3Menu->addAction(ccDischarge3);
    testBench3Menu->addAction(rptTest3);

    connect(cccvCharge3, &QAction::triggered, this, [this]() { onTestBenchOptionSelected(3, "CCCV Charge Cycle"); });
    connect(ccDischarge3, &QAction::triggered, this, [this]() { onTestBenchOptionSelected(3, "CC Discharge Cycle"); });
    connect(rptTest3, &QAction::triggered, this, [this]() { onTestBenchOptionSelected(3, "RPT (Rapid Pulse Test)"); });

    QMenu *helpMenu = menuBar()->addMenu("Help");
    QAction *aboutAction = new QAction("About", this);
    helpMenu->addAction(aboutAction);

    // Add Start Test, Run, and Stop actions
    QMenu *actionMenu = menuBar()->addMenu("Actions");
    QAction *startTestAction = new QAction("Start Test", this);
    QAction *runAction = new QAction("Run", this);
    QAction *stopAction = new QAction("Stop", this);
    
    actionMenu->addAction(startTestAction);
    actionMenu->addAction(runAction);
    actionMenu->addAction(stopAction);

    connect(startTestAction, &QAction::triggered, this, &MainWindow::onStartTestClicked);
    connect(runAction, &QAction::triggered, this, &MainWindow::onRunClicked);
    connect(stopAction, &QAction::triggered, this, &MainWindow::onStopClicked);
}

void MainWindow::onRunClicked() {
    // Implement what happens when Run is clicked
    QMessageBox::information(this, "Run", "Run action triggered.");
}

void MainWindow::onStopClicked() {
    // Implement what happens when Stop is clicked
    QMessageBox::information(this, "Stop", "Stop action triggered.");
}

//void MainWindow::setupToolBar() {
    // Tool Bar Setup
    //QToolBar *toolBar = addToolBar("Main Toolbar");
    //QAction *runAction = new QAction("Run", this);
    //QAction *stopAction = new QAction("Stop", this);
    //QAction *startAction = new QAction("Start Test", this); // Added Start Test button

    //toolBar->addAction(startAction);
    //toolBar->addAction(runAction);
    //toolBar->addAction(stopAction);

    //connect(startAction, &QAction::triggered, this, &MainWindow::onStartTestClicked);
//}

void MainWindow::setupCentralWidget() {
    // Central Widget and Layout Setup
    QWidget *centralWidget = new QWidget(this);
    QVBoxLayout *layout = new QVBoxLayout(centralWidget);

    QLabel *welcomeLabel = new QLabel("Welcome to the MultiCell TestBench Automation", centralWidget);
    welcomeLabel->setAlignment(Qt::AlignCenter);
    layout->addWidget(welcomeLabel);

    centralWidget->setLayout(layout);
    setCentralWidget(centralWidget);
}

void MainWindow::setupRightPanel() {
    // Create the right panel widget
    QWidget *rightPanel = new QWidget(this);
    rightPanel->setWindowTitle("Standard Test");

    QVBoxLayout *rightPanelLayout = new QVBoxLayout(rightPanel);

    testBenchLabel = new QLabel("Test Bench: ", rightPanel);
    cellNumberLabel = new QLabel("Cell Number: ", rightPanel);
    testTypeLabel = new QLabel("Test Type: ", rightPanel);

    // Set smaller font size for labels
    QFont font = testBenchLabel->font();
    font.setPointSize(1);
    testBenchLabel->setFont(font);
    cellNumberLabel->setFont(font);
    testTypeLabel->setFont(font);

    progressBar = new QProgressBar(rightPanel);
    progressBar->setRange(0, 100);
    progressBar->setValue(0);  // Initial value

    temperatureDisplay = new QLCDNumber(rightPanel);
    temperatureDisplay->setDigitCount(5);
    temperatureDisplay->setStyleSheet("QLCDNumber { font-size: 4pt; }");
    temperatureDisplay->setFixedSize(100, 50);  // Set fixed size to avoid resizing

    voltageDisplay = new QLCDNumber(rightPanel);
    voltageDisplay->setDigitCount(5);
    voltageDisplay->setStyleSheet("QLCDNumber { font-size: 4pt; }");
    voltageDisplay->setFixedSize(100, 50);  // Set fixed size to avoid resizing

    rightPanelLayout->addWidget(testBenchLabel);
    rightPanelLayout->addWidget(cellNumberLabel);
    rightPanelLayout->addWidget(testTypeLabel);
    rightPanelLayout->addWidget(progressBar);
    rightPanelLayout->addWidget(new QLabel("Temperature:", rightPanel));
    rightPanelLayout->addWidget(temperatureDisplay);
    rightPanelLayout->addWidget(new QLabel("Voltage:", rightPanel));
    rightPanelLayout->addWidget(voltageDisplay);

    rightPanel->setLayout(rightPanelLayout);
    
    // Set the right panel as a dock widget on the top right
    QDockWidget *dockWidget = new QDockWidget("Standard Test", this);
    dockWidget->setWidget(rightPanel);
    dockWidget->setFeatures(QDockWidget::DockWidgetFloatable | QDockWidget::DockWidgetMovable);

    dockWidget->setFixedSize(400, 350);  // Adjust the size to your needs
    
    // Adjust the dock widget's size to auto-resize with the main window
    addDockWidget(Qt::RightDockWidgetArea, dockWidget);
}

void MainWindow::onStartTestClicked() {
    // Slot implementation for the Start button click
    QMessageBox::information(this, "Test Started", "The test has been started.");
}

// Slot for when a specific test bench option is selected
void MainWindow::onTestBenchOptionSelected(int testBenchNumber, const QString &option) {
    // Ask the user for the cell number using an input dialog
    bool ok;
    int cellNumber = QInputDialog::getInt(this, QString("Test Bench %1 - %2").arg(testBenchNumber).arg(option),
                                          "Enter Cell Number:", 1, 1, 50, 1, &ok);

    // If the user clicked OK and entered a valid number
    if (ok) {
        QString message = QString("Test Bench %1 - %2 selected for Cell %3.").arg(testBenchNumber).arg(option).arg(cellNumber);
        QMessageBox::information(this, QString("Test Bench %1").arg(testBenchNumber), message);

        // Update the display labels with the selected test bench, cell number, and test type
        testBenchLabel->setText(QString("Test Bench: %1").arg(testBenchNumber));
        cellNumberLabel->setText(QString("Cell Number: %1").arg(cellNumber));
        testTypeLabel->setText(QString("Test Type: %1").arg(option));

        // Example progress update
        progressBar->setValue(50);  // Set progress value (for example purposes)
        temperatureDisplay->display(25.0);  // Set a dummy temperature value
        voltageDisplay->display(3.7);  // Set a dummy voltage value
    }
}
