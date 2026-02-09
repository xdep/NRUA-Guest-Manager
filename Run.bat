@echo off
echo ================================================
echo NRUA Guest Manager - Iniciando...
echo ================================================
echo.

dotnet run

if %ERRORLEVEL% NEQ 0 (
    echo.
    echo ERROR: No se pudo ejecutar la aplicacion
    echo Asegurese de compilar primero con Build.bat
    echo.
    pause
)
