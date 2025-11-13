Write-Host "Checking Docker..." -ForegroundColor Cyan

# Check if Docker Desktop is running
$dockerRunning = $false
try {
    docker ps 2>&1 | Out-Null
    if ($LASTEXITCODE -eq 0) {
        $dockerRunning = $true
    }
} catch {
    $dockerRunning = $false
}

if (-not $dockerRunning) {
    Write-Host ""
    Write-Host "Docker Desktop is NOT running!" -ForegroundColor Red
    Write-Host ""
    Write-Host "Please follow these steps:" -ForegroundColor Yellow
    Write-Host "  1. Open Docker Desktop" -ForegroundColor White
    Write-Host "  2. Wait for Docker to start (icon turns green)" -ForegroundColor White
    Write-Host "  3. Run this script again" -ForegroundColor White
    Write-Host ""
    
    # Try to start Docker Desktop
    $dockerPath = "C:\Program Files\Docker\Docker\Docker Desktop.exe"
    if (Test-Path $dockerPath) {
        $response = Read-Host "Do you want me to start Docker Desktop? (Y/N)"
        if ($response -eq "Y" -or $response -eq "y") {
            Write-Host "Starting Docker Desktop..." -ForegroundColor Cyan
            Start-Process $dockerPath
            Write-Host "Please wait for Docker to start, then run this script again." -ForegroundColor Yellow
        }
    }
    exit 1
}

Write-Host ""
Write-Host "Docker is running!" -ForegroundColor Green
Write-Host ""
Write-Host "Starting services..." -ForegroundColor Cyan
docker-compose up -d

Write-Host ""
Write-Host "Services started!" -ForegroundColor Green
Write-Host ""
Write-Host "Access your application:" -ForegroundColor Cyan
Write-Host "  Frontend:  http://localhost" -ForegroundColor White
Write-Host "  Backend:   http://localhost:5201" -ForegroundColor White
Write-Host "  Swagger:   http://localhost:5201/swagger" -ForegroundColor White
