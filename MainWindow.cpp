#include "MainWindow.h"
#include <QVBoxLayout>
#include <QLabel>
#include <QMessageBox>  // For showing a message box

MainWindow::MainWindow(QWidget *parent) : QMainWindow(parent) {
    setupMenuBar();
    setupToolBar();
    setupCentralWidget();
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

    QMenu *helpMenu = menuBar()->addMenu("Help");
    QAction *aboutAction = new QAction("About", this);
    helpMenu->addAction(aboutAction);
}

void MainWindow::setupToolBar() {
    // Tool Bar Setup
    QToolBar *toolBar = addToolBar("Main Toolbar");
    QAction *runAction = new QAction("Run", this);
    QAction *stopAction = new QAction("Stop", this);
    
    toolBar->addAction(runAction);
    toolBar->addAction(stopAction);
}

void MainWindow::setupCentralWidget() {
    // Central Widget and Layout Setup
    QWidget *centralWidget = new QWidget(this);
    QVBoxLayout *layout = new QVBoxLayout(centralWidget);

    QLabel *welcomeLabel = new QLabel("Welcome to the Professional Test Bench", centralWidget);
    welcomeLabel->setAlignment(Qt::AlignCenter);
    layout->addWidget(welcomeLabel);

    QPushButton *startButton = new QPushButton("Start Test", centralWidget);
    layout->addWidget(startButton);

    // Connect the start button's clicked signal to the onStartTestClicked slot
    connect(startButton, &QPushButton::clicked, this, &MainWindow::onStartTestClicked);

    centralWidget->setLayout(layout);
    setCentralWidget(centralWidget);
}

void MainWindow::onStartTestClicked() {
    // Slot implementation for the Start button click
    QMessageBox::information(this, "Test Started", "The test has been started.");
}
