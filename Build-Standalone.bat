@echo off
echo ================================================
echo NRUA Guest Manager - Creando Ejecutable
echo ================================================
echo.
echo Esto creara un archivo .exe que NO requiere .NET instalado
echo El proceso puede tardar unos minutos...
echo.

dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -p:PublishTrimmed=true

if %ERRORLEVEL% NEQ 0 (
    echo.
    echo ERROR: Fallo la creacion del ejecutable
    pause
    exit /b 1
)

echo.
echo ================================================
echo EXITO!
echo ================================================
echo.
echo El ejecutable se encuentra en:
echo bin\Release\net6.0-windows\win-x64\publish\NRUAGuestManager.exe
echo.
echo Puede copiar este archivo a cualquier PC con Windows 10/11
echo.
pause

REM Open the folder
explorer bin\Release\net6.0-windows\win-x64\publish
