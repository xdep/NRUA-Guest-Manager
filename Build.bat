@echo off
echo ================================================
echo NRUA Guest Manager - Compilador
echo ================================================
echo.

REM Check if dotnet is installed
where dotnet >nul 2>nul
if %ERRORLEVEL% NEQ 0 (
    echo ERROR: .NET no esta instalado
    echo.
    echo Por favor instale .NET 6.0 desde:
    echo https://dotnet.microsoft.com/download/dotnet/6.0
    echo.
    pause
    exit /b 1
)

echo [1/2] Compilando aplicacion...
dotnet build -c Release

if %ERRORLEVEL% NEQ 0 (
    echo.
    echo ERROR: Fallo la compilacion
    pause
    exit /b 1
)

echo.
echo [2/2] Compilacion exitosa!
echo.
echo Para ejecutar: Run.bat
echo Para crear ejecutable independiente: Build-Standalone.bat
echo.
pause
