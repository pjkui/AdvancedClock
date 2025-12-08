@echo off
echo 测试单实例功能
echo.
echo 启动第一个实例...
start "" "bin\Debug\net8.0-windows\AdvancedClock.exe"
echo.
echo 等待3秒...
timeout /t 3 /nobreak >nul
echo.
echo 启动第二个实例（应该会杀掉第一个实例）...
start "" "bin\Debug\net8.0-windows\AdvancedClock.exe"
echo.
echo 等待3秒...
timeout /t 3 /nobreak >nul
echo.
echo 启动第三个实例（应该会杀掉第二个实例）...
start "" "bin\Debug\net8.0-windows\AdvancedClock.exe"
echo.
echo 测试完成！请检查任务管理器，应该只有一个 AdvancedClock 进程在运行。
pause