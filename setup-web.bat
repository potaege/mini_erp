@echo off
setlocal
set "ROOT=%~dp0"

where node >nul 2>nul
if errorlevel 1 (
    echo Node.js was not found. Install a supported Node.js version and try again.
    goto :failed
)

where npm >nul 2>nul
if errorlevel 1 (
    echo npm was not found. Reinstall Node.js with npm and try again.
    goto :failed
)

pushd "%ROOT%web" || goto :failed
set "PUSHED=1"

call npm install
if errorlevel 1 goto :failed

echo.
echo Web dependencies are ready. Starting the Angular development server...
call npm start
set "EXIT_CODE=%ERRORLEVEL%"
popd
pause
exit /b %EXIT_CODE%

:failed
set "EXIT_CODE=%ERRORLEVEL%"
if "%EXIT_CODE%"=="0" set "EXIT_CODE=1"
echo.
echo Web setup failed. Review the error above and try again.
if defined PUSHED popd
pause
exit /b %EXIT_CODE%
