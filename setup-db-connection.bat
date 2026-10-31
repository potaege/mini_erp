@echo off
chcp 65001 >nul
setlocal
set "ROOT=%~dp0"
set "DB_CONFIG=%ROOT%backend\db-connection.json"

where dotnet >nul 2>nul
if errorlevel 1 (
    echo ไม่พบ .NET SDK กรุณาติดตั้ง .NET 10 SDK ก่อน
    goto :failed
)

if not exist "%DB_CONFIG%" (
    copy "%ROOT%backend\db-connection.example.json" "%DB_CONFIG%" >nul
    if errorlevel 1 goto :failed
    echo เปิดไฟล์ตั้งค่าฐานข้อมูล กรุณากรอกข้อมูล MySQL แล้วบันทึกไฟล์
    start /wait "" notepad.exe "%DB_CONFIG%"
)

pushd "%ROOT%backend" || goto :failed
set "PUSHED=1"

powershell.exe -NoProfile -ExecutionPolicy Bypass -Command "$ErrorActionPreference='Stop'; $config=Get-Content $env:DB_CONFIG -Raw | ConvertFrom-Json; foreach ($field in @('server','port','database','userId')) { if ([string]::IsNullOrWhiteSpace([string]$config.$field)) { throw \"กรุณากรอกค่า '$field' ในไฟล์ backend\db-connection.json\" } }; $builder=[System.Data.Common.DbConnectionStringBuilder]::new(); $builder['Server']=[string]$config.server; $builder['Port']=[string]$config.port; $builder['Database']=[string]$config.database; $builder['User ID']=[string]$config.userId; $builder['Password']=[string]$config.password; dotnet user-secrets set 'ConnectionStrings:DefaultConnection' $builder.ConnectionString; if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }; $bytes=New-Object byte[] 32; $rng=[System.Security.Cryptography.RandomNumberGenerator]::Create(); $rng.GetBytes($bytes); $key=[Convert]::ToBase64String($bytes); dotnet user-secrets set 'Jwt:SigningKey' $key; if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }; Remove-Variable config, builder, bytes, key; $rng.Dispose()"
if errorlevel 1 goto :failed

echo.
echo บันทึกค่าจาก backend\db-connection.json แล้ว
popd
exit /b 0

:failed
set "EXIT_CODE=%ERRORLEVEL%"
if "%EXIT_CODE%"=="0" set "EXIT_CODE=1"
echo.
echo อ่านไฟล์ตั้งค่าไม่สำเร็จ ตรวจ backend\db-connection.json แล้วลองใหม่
if defined PUSHED popd
pause
exit /b %EXIT_CODE%
