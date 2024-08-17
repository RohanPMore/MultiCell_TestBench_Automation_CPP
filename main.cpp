#include <QApplication>
#include "MainWindow.h"

int main(int argc, char *argv[]) {
    QApplication app(argc, argv);

    MainWindow mainWindow;
    mainWindow.setWindowTitle("MultiCell-TestBench-Automation");
    mainWindow.resize(800, 600);  // Typical modern application size
    // Styling using Qt Stylesheets
    app.setStyleSheet(
        "QMainWindow { background-color: #2b2b2b; } "
        "QPushButton { "
        "background-color: #3498db; "
        "color: white; "
        "border: none; "
        "padding: 10px 20px; "
        "border-radius: 5px; "
        "font-size: 16px; "
        "} "
        "QPushButton:hover { background-color: #2980b9; } "
        "QMenuBar { background-color: #2b2b2b; color: white; } "
        "QMenuBar::item { background: transparent; } "
        "QMenuBar::item:selected { background: #3498db; } "
        "QToolBar { background-color: #2b2b2b; border: none; } "
        "QToolBar QToolButton { background-color: #3498db; color: white; } "
        "QToolBar QToolButton:hover { background-color: #2980b9; } "
        "QLabel { color: white; font-size: 24px; }"
    );
    mainWindow.show();

    return app.exec();
}
