# Тест логирования ILogger
# Запустите приложение перед выполнением этого скрипта: dotnet run

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Testing ILogger Functionality" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan

$baseUrl = "http://localhost:5000"

# 1. Тест логирования входа
Write-Host "`n[1/5] Testing login logging (LogInformation)..." -ForegroundColor Yellow
try {
    $loginResponse = Invoke-RestMethod -Uri "$baseUrl/auth/login" `
        -Method POST `
        -ContentType "application/json" `
        -Body '{"username":"testuser","password":"testpass"}'
    
    $token = $loginResponse.Token
    Write-Host "✅ Login successful - Check console for: 'Login attempt for user: testuser'" -ForegroundColor Green
    Write-Host "   Token: $($token.Substring(0,20))..." -ForegroundColor Gray
} catch {
    Write-Host "❌ Login failed: $_" -ForegroundColor Red
    exit 1
}

# 2. Тест логирования получения продуктов
Write-Host "`n[2/5] Testing product retrieval logging..." -ForegroundColor Yellow
try {
    $products = Invoke-RestMethod -Uri "$baseUrl/product"
    Write-Host "✅ Products retrieved: $($products.Count) items" -ForegroundColor Green
    Write-Host "   Check console for cache/DB access logs" -ForegroundColor Gray
} catch {
    Write-Host "⚠️  Error retrieving products: $_" -ForegroundColor Yellow
}

# 3. Тест логирования создания продукта
Write-Host "`n[3/5] Testing product creation logging (LogInformation)..." -ForegroundColor Yellow
$headers = @{
    "Authorization" = "Bearer $token"
    "Content-Type" = "application/json"
}
$product = @{
    name = "Logger Test Product"
    price = 99.99
    quantity = 10
} | ConvertTo-Json

try {
    $created = Invoke-RestMethod -Uri "$baseUrl/product" `
        -Method POST `
        -Headers $headers `
        -Body $product
    Write-Host "✅ Product created: ID=$($created.id), Name=$($created.name)" -ForegroundColor Green
    Write-Host "   Check console for: 'Product created successfully: $($created.id), Name: $($created.name)'" -ForegroundColor Gray
    $createdProductId = $created.id
} catch {
    Write-Host "❌ Product creation failed: $_" -ForegroundColor Red
    $createdProductId = $null
}

# 4. Тест логирования предупреждений (LogWarning)
Write-Host "`n[4/5] Testing warning logging (LogWarning)..." -ForegroundColor Yellow
try {
    $response = Invoke-WebRequest -Uri "$baseUrl/product/99999" -ErrorAction Stop
} catch {
    if ($_.Exception.Response.StatusCode -eq 404) {
        Write-Host "✅ 404 Not Found (expected)" -ForegroundColor Green
        Write-Host "   Check console for: 'Product not found: 99999'" -ForegroundColor Gray
    } else {
        Write-Host "⚠️  Unexpected error: $_" -ForegroundColor Yellow
    }
}

# 5. Тест логирования удаления
if ($createdProductId) {
    Write-Host "`n[5/5] Testing product deletion logging..." -ForegroundColor Yellow
    try {
        Invoke-RestMethod -Uri "$baseUrl/product/$createdProductId" `
            -Method DELETE `
            -Headers $headers | Out-Null
        Write-Host "✅ Product deleted: ID=$createdProductId" -ForegroundColor Green
        Write-Host "   Check console for: 'Product deleted successfully: $createdProductId'" -ForegroundColor Gray
    } catch {
        Write-Host "⚠️  Deletion failed: $_" -ForegroundColor Yellow
    }
} else {
    Write-Host "`n[5/5] Skipping deletion test (product not created)" -ForegroundColor Yellow
}

Write-Host "`n========================================" -ForegroundColor Cyan
Write-Host "✅ All tests completed!" -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "`nCheck the console output where you ran 'dotnet run'" -ForegroundColor Yellow
Write-Host "You should see logs with prefixes:" -ForegroundColor Yellow
Write-Host "  - info: (Information level)" -ForegroundColor White
Write-Host "  - warn: (Warning level)" -ForegroundColor White
Write-Host "  - fail: (Error level, if errors occur)" -ForegroundColor White

