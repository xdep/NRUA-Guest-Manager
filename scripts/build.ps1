# NRUA Guest Manager - Build Script
# Usage: powershell -ExecutionPolicy Bypass -File scripts\build.ps1
# Prerequisites: .NET 8 SDK, InnoSetup 6 (for installer)

param(
    [switch]$SkipInstaller,
    [string]$Configuration = "Release"
)

$ErrorActionPreference = "Stop"
$ProjectRoot = Split-Path -Parent $PSScriptRoot
$PublishDir = Join-Path $ProjectRoot "publish"

Write-Host "================================================" -ForegroundColor Cyan
Write-Host "NRUA Guest Manager - Build Pipeline" -ForegroundColor Cyan
Write-Host "================================================" -ForegroundColor Cyan
Write-Host ""

# Step 1: Clean previous build
Write-Host "[1/4] Cleaning previous build..." -ForegroundColor Yellow
if (Test-Path $PublishDir) {
    Remove-Item -Recurse -Force $PublishDir
}

# Step 2: Publish self-contained exe
Write-Host "[2/4] Building self-contained executable..." -ForegroundColor Yellow
dotnet publish $ProjectRoot -c $Configuration -r win-x64 --self-contained true -o $PublishDir
if ($LASTEXITCODE -ne 0) {
    Write-Host "ERROR: Build failed!" -ForegroundColor Red
    exit 1
}

$exePath = Join-Path $PublishDir "NRUAGuestManager.exe"
if (Test-Path $exePath) {
    $size = (Get-Item $exePath).Length / 1MB
    Write-Host "  -> Executable: $exePath ($([math]::Round($size, 1)) MB)" -ForegroundColor Green
} else {
    Write-Host "ERROR: Executable not found at $exePath" -ForegroundColor Red
    exit 1
}

# Step 3: Build Key Generator (optional - for seller use)
Write-Host "[3/4] Building Key Generator tool..." -ForegroundColor Yellow
$keyGenProject = Join-Path $ProjectRoot "tools\KeyGenerator\KeyGenerator.csproj"
dotnet build $keyGenProject -c $Configuration
if ($LASTEXITCODE -ne 0) {
    Write-Host "WARNING: Key Generator build failed (non-critical)" -ForegroundColor Yellow
} else {
    Write-Host "  -> Key Generator built successfully" -ForegroundColor Green
}

# Step 4: Create installer
if (-not $SkipInstaller) {
    Write-Host "[4/4] Creating installer with InnoSetup..." -ForegroundColor Yellow

    $issFile = Join-Path $ProjectRoot "installer\setup.iss"
    $iscc = "C:\Program Files (x86)\Inno Setup 6\ISCC.exe"

    if (-not (Test-Path $iscc)) {
        # Try alternative location
        $iscc = "C:\Program Files\Inno Setup 6\ISCC.exe"
    }

    if (Test-Path $iscc) {
        & $iscc $issFile
        if ($LASTEXITCODE -ne 0) {
            Write-Host "ERROR: Installer creation failed!" -ForegroundColor Red
            exit 1
        }

        $installerDir = Join-Path $ProjectRoot "installer\Output"
        Write-Host "  -> Installer created in: $installerDir" -ForegroundColor Green
    } else {
        Write-Host "  -> InnoSetup not found. Skipping installer creation." -ForegroundColor Yellow
        Write-Host "     Install InnoSetup 6 from: https://jrsoftware.org/isinfo.php" -ForegroundColor Yellow
    }
} else {
    Write-Host "[4/4] Skipping installer (--SkipInstaller)" -ForegroundColor Yellow
}

Write-Host ""
Write-Host "================================================" -ForegroundColor Cyan
Write-Host "Build complete!" -ForegroundColor Green
Write-Host "================================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "Output:"
Write-Host "  Executable: $PublishDir\NRUAGuestManager.exe"
if (-not $SkipInstaller) {
    Write-Host "  Installer:  $ProjectRoot\installer\Output\"
}
Write-Host ""
Write-Host "To generate license keys:"
Write-Host "  dotnet run --project tools\KeyGenerator -- --count 10"
