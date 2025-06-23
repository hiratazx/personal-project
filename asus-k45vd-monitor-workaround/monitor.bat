@echo off
:: Check for admin rights
>nul 2>&1 "%SYSTEMROOT%\system32\cacls.exe" "%SYSTEMROOT%\system32\config\system"
if %errorlevel% neq 0 (
    echo.
    echo This script requires administrator privileges.
    echo Restarting with elevated permissions...
    powershell -Command "Start-Process '%~f0' -Verb runAs"
    exit /b
)

echo Disabling Intel GPU...
powershell -Command "Get-PnpDevice -FriendlyName '*HD Graphics*' | Disable-PnpDevice -Confirm:$false"

timeout /t 2 >nul

echo Enabling Intel GPU...
powershell -Command "Get-PnpDevice -FriendlyName '*HD Graphics*' | Enable-PnpDevice -Confirm:$false"

echo Restarting Windows Explorer...
taskkill /f /im explorer.exe
start explorer.exe

echo Done.
pause
