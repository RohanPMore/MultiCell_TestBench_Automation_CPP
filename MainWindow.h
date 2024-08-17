#ifndef MAINWINDOW_H
#define MAINWINDOW_H

#include <QMainWindow>
#include <QPushButton>  // Required for QPushButton
#include <QMenuBar>     // Required for QMenuBar
#include <QMenu>        // Required for QMenu
#include <QToolBar>     // Required for QToolBar

class MainWindow : public QMainWindow {
    Q_OBJECT  // This is critical for QObject-based classes

public:
    MainWindow(QWidget *parent = nullptr);
    ~MainWindow();

private:
    void setupMenuBar();
    void setupToolBar();
    void setupCentralWidget();

private slots:
    void onStartTestClicked();  // Slot to handle button click

private:
    QPushButton *startButton;
};

#endif // MAINWINDOW_H
