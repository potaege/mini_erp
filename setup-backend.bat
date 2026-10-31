@echo off
chcp 65001 >nul
setlocal
set "ROOT=%~dp0"

where dotnet >nul 2>nul
if errorlevel 1 (
    echo ไม่พบ .NET SDK กรุณาติดตั้ง .NET 10 SDK ก่อน
    goto :failed
)

pushd "%ROOT%backend" || goto :failed
set "PUSHED=1"

echo ============================================================
echo สร้างตารางและข้อมูลตัวอย่าง แล้วเปิด Backend API
echo ฐานข้อมูลเป้าหมาย: mini_erp
echo ต้องรัน setup-db-connection.bat ตั้งค่าการเชื่อมต่อก่อน
echo ============================================================
echo.

powershell.exe -NoProfile -ExecutionPolicy Bypass -Command "$p=Join-Path $env:APPDATA 'Microsoft\UserSecrets\948bc9af-58d9-4a6a-9f0d-500cc196355c\secrets.json'; if (-not (Test-Path $p)) { Write-Error 'ยังไม่มีการตั้งค่าฐานข้อมูล กรุณารัน setup-db-connection.bat ก่อน'; exit 1 }; $s=Get-Content $p -Raw | ConvertFrom-Json; $connection=$s.'ConnectionStrings:DefaultConnection'; $signingKey=$s.'Jwt:SigningKey'; if ([string]::IsNullOrWhiteSpace($connection) -or [string]::IsNullOrWhiteSpace($signingKey)) { Write-Error 'ค่าการเชื่อมต่อหรือ JWT key ไม่ครบ กรุณารัน setup-db-connection.bat อีกครั้ง'; exit 1 }"
if errorlevel 1 goto :failed

dotnet tool restore
if errorlevel 1 goto :failed

dotnet restore
if errorlevel 1 goto :failed

dotnet tool run dotnet-ef -- database update
if errorlevel 1 goto :failed

echo.
echo สร้างตารางและข้อมูลตัวอย่างสำเร็จ กำลังเปิด Backend API...
dotnet run --launch-profile http
set "EXIT_CODE=%ERRORLEVEL%"
popd
pause
exit /b %EXIT_CODE%

:failed
set "EXIT_CODE=%ERRORLEVEL%"
if "%EXIT_CODE%"=="0" set "EXIT_CODE=1"
echo.
echo ทำงานไม่สำเร็จ ตรวจสอบข้อความผิดพลาดด้านบน
if defined PUSHED popd
pause
exit /b %EXIT_CODE%
