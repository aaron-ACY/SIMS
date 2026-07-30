# Script to verify password hashes in users.csv
# Run this from the project root after building

$projectPath = "SIMS-BackEnd\bin\Debug\net8.0\SIMS-API.dll"

if (-not (Test-Path $projectPath)) {
    Write-Host "Build the project first: dotnet build" -ForegroundColor Red
    exit 1
}

# Add reference to the built DLL
Add-Type -Path $projectPath
Add-Type -Path "SIMS-BackEnd\bin\Debug\net8.0\SIMS.Infrastructure.dll"

$hasher = New-Object SIMS.Infrastructure.Security.Pbkdf2PasswordHasher

# Test passwords
$tests = @(
    @{Username="admin"; Password="Admin123"; Hash="Pzxucb7pEKHHEE8+lPfhBn6VQCqF7J5OSetIRwDqJ34="; Salt="u42JjYeCBDSz8wXavgbRCw=="}
    @{Username="tranngoczit"; Password="Ngoczit123"; Hash="oQSxBes+2Wml++LVWYihffpb3VFLAjwp/NEQhk6G8kE="; Salt="gvxOjRDGORe4jv580grD7A=="}
    @{Username="okuzchan"; Password="davidokuz123"; Hash="t/9PdXiunUYf+LlTE8unL8H+0UA+EQEACPg4VqhjEok="; Salt="JQg8BTnQj0rrSoND1LY/fw=="}
)

Write-Host "`nVerifying passwords..." -ForegroundColor Cyan

foreach ($test in $tests) {
    $result = $hasher.VerifyPassword($test.Password, $test.Hash, $test.Salt)
    $status = if ($result) { "✓ MATCH" } else { "✗ FAIL" }
    $color = if ($result) { "Green" } else { "Red" }

    Write-Host "$status - $($test.Username) / $($test.Password)" -ForegroundColor $color
}
