$ErrorActionPreference = "Continue"

Write-Host "=== Testing Backend ===" -ForegroundColor Green

# Test 1: Health Check
Write-Host "`n[1] Health Check" -ForegroundColor Cyan
$health = Invoke-WebRequest -Uri "http://localhost:5000/api/health" -UseBasicParsing -ErrorAction Stop
Write-Host "✓ Status: $($health.StatusCode)" -ForegroundColor Green

# Test 2: Register
Write-Host "`n[2] Register User" -ForegroundColor Cyan
$registerJson = '{"email":"gabrielsotelo52@gmail.com","password":"pw123","fullName":"Gabriel Sotelo"}'
Write-Host "Payload: $registerJson"

try {
    $register = Invoke-WebRequest -Uri "http://localhost:5000/api/auth/register" `
        -Method POST `
        -ContentType "application/json" `
        -Body $registerJson `
        -UseBasicParsing -ErrorAction Stop
    
    Write-Host "✓ Status: $($register.StatusCode)" -ForegroundColor Green
    $regData = $register.Content | ConvertFrom-Json
    Write-Host "Response: $($register.Content)" -ForegroundColor Yellow
    $token = $regData.token
} catch {
    Write-Host "✗ Error: $($_.Exception.Message)" -ForegroundColor Red
    if ($_.Exception.Response) {
        Write-Host "Status Code: $($_.Exception.Response.StatusCode)" -ForegroundColor Red
    }
    exit 1
}

# Test 3: Login
Write-Host "`n[3] Login" -ForegroundColor Cyan
$loginJson = '{"email":"gabrielsotelo52@gmail.com","password":"pw123"}'
Write-Host "Payload: $loginJson"

try {
    $login = Invoke-WebRequest -Uri "http://localhost:5000/api/auth/login" `
        -Method POST `
        -ContentType "application/json" `
        -Body $loginJson `
        -UseBasicParsing -ErrorAction Stop
    
    Write-Host "✓ Status: $($login.StatusCode)" -ForegroundColor Green
    $loginData = $login.Content | ConvertFrom-Json
    Write-Host "Response: $($login.Content)" -ForegroundColor Yellow
    
    # Try to decode JWT
    Write-Host "`n[4] JWT Token Analysis" -ForegroundColor Cyan
    $tokenParts = $loginData.token.Split('.')
    if ($tokenParts.Length -eq 3) {
        try {
            $payload = [System.Text.Encoding]::UTF8.GetString([System.Convert]::FromBase64String($tokenParts[1]))
            Write-Host "✓ Token Payload: $payload" -ForegroundColor Yellow
        } catch {
            Write-Host "✗ Failed to decode token: $($_.Exception.Message)" -ForegroundColor Red
        }
    }
} catch {
    Write-Host "✗ Error: $($_.Exception.Message)" -ForegroundColor Red
    if ($_.Exception.Response) {
        Write-Host "Status Code: $($_.Exception.Response.StatusCode)" -ForegroundColor Red
    }
}

Write-Host "`n=== Test Complete ===" -ForegroundColor Green
