# Как проверить работу ILogger

## Быстрая проверка

### 1. Настройте уровни логирования

Откройте `appsettings.json` и настройте логирование:

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning",
      "WebApplication1": "Debug"  // ← Добавьте эту строку для вашего проекта
    }
  }
}
```

### 2. Запустите приложение

```bash
dotnet run
```

### 3. Выполните действия, которые логируют

## Практические тесты

### Тест 1: Логирование успешных операций (LogInformation)

**Действие:** Создайте продукт через API

```bash
# 1. Получите токен
curl -X POST http://localhost:5000/auth/login \
  -H "Content-Type: application/json" \
  -d '{"username":"admin","password":"test"}'

# 2. Создайте продукт (замените YOUR_TOKEN на полученный токен)
curl -X POST http://localhost:5000/product \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer YOUR_TOKEN" \
  -d '{"name":"Test Product","price":99.99,"quantity":10}'
```

**Ожидаемый лог в консоли:**
```
info: WebApplication1.Services.ProductService[0]
      Product created successfully: 1, Name: Test Product
```

### Тест 2: Логирование ошибок (LogError)

**Действие:** Попробуйте получить несуществующий продукт

```bash
curl http://localhost:5000/product/99999
```

**Ожидаемый лог в консоли:**
```
warn: WebApplication1.Controllers.ProductController[0]
      Product not found: 99999
```

### Тест 3: Логирование предупреждений (LogWarning)

**Действие:** Попробуйте обновить несуществующий продукт

```bash
# 1. Получите токен
curl -X POST http://localhost:5000/auth/login \
  -H "Content-Type: application/json" \
  -d '{"username":"admin","password":"test"}'

# 2. Попробуйте обновить несуществующий продукт
curl -X PUT http://localhost:5000/product/99999 \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer YOUR_TOKEN" \
  -d '{"id":99999,"name":"Updated","price":50,"quantity":5}'
```

**Ожидаемый лог в консоли:**
```
warn: WebApplication1.Services.ProductService[0]
      Product not found for update: 99999
```

### Тест 4: Логирование авторизации (LogInformation)

**Действие:** Выполните вход

```bash
curl -X POST http://localhost:5000/auth/login \
  -H "Content-Type: application/json" \
  -d '{"username":"testuser","password":"testpass"}'
```

**Ожидаемый лог в консоли:**
```
info: WebApplication1.Controllers.AuthController[0]
      Login attempt for user: testuser
info: WebApplication1.Controllers.AuthController[0]
      Login successful for user: testuser
```

### Тест 5: Логирование ошибок базы данных (LogError)

**Действие:** Остановите PostgreSQL и попробуйте получить продукты

```bash
# Остановите PostgreSQL
docker stop appone-postgres

# Попробуйте получить продукты
curl http://localhost:5000/product
```

**Ожидаемый лог в консоли:**
```
fail: WebApplication1.Repositories.ProductRepository[0]
      Error getting all products from database
      System.Exception: Connection failed...
```

## Настройка уровней логирования для тестирования

### Для просмотра всех логов (включая Debug)

**appsettings.Development.json:**
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Debug",
      "Microsoft.AspNetCore": "Warning",
      "WebApplication1": "Debug"
    }
  }
}
```

### Для просмотра только важных логов

**appsettings.json:**
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning",
      "WebApplication1": "Information"
    }
  }
}
```

## Просмотр логов

### 1. В консоли (по умолчанию)

Логи автоматически выводятся в консоль при запуске через `dotnet run`.

**Формат:**
```
[Timestamp] [Level] [Category] [EventId]
      Message
```

**Пример:**
```
info: WebApplication1.Services.ProductService[0]
      Product created successfully: 1, Name: Test Product
```

### 2. Фильтрация логов в консоли

**Windows PowerShell:**
```powershell
# Запустите приложение и фильтруйте логи
dotnet run | Select-String "ProductService"
```

**Linux/Mac:**
```bash
dotnet run | grep "ProductService"
```

### 3. Сохранение логов в файл

**Windows PowerShell:**
```powershell
dotnet run 2>&1 | Tee-Object -FilePath "logs.txt"
```

**Linux/Mac:**
```bash
dotnet run 2>&1 | tee logs.txt
```

## Пошаговый тест всех уровней

### Шаг 1: Настройте максимальное логирование

**appsettings.Development.json:**
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Trace",
      "Microsoft.AspNetCore": "Warning",
      "WebApplication1": "Trace"
    }
  }
}
```

### Шаг 2: Выполните тестовый сценарий

```bash
# 1. Вход
curl -X POST http://localhost:5000/auth/login \
  -H "Content-Type: application/json" \
  -d '{"username":"admin","password":"test"}'

# 2. Получение всех продуктов
curl http://localhost:5000/product

# 3. Создание продукта
curl -X POST http://localhost:5000/product \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer YOUR_TOKEN" \
  -d '{"name":"Logger Test","price":100,"quantity":1}'

# 4. Получение продукта по ID
curl http://localhost:5000/product/1

# 5. Обновление продукта
curl -X PUT http://localhost:5000/product/1 \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer YOUR_TOKEN" \
  -d '{"id":1,"name":"Updated","price":200,"quantity":2}'

# 6. Удаление продукта
curl -X DELETE http://localhost:5000/product/1 \
  -H "Authorization: Bearer YOUR_TOKEN"
```

### Шаг 3: Проверьте логи в консоли

Вы должны увидеть логи на разных уровнях:
- `info:` - успешные операции
- `warn:` - предупреждения (не найдено)
- `fail:` - ошибки (если что-то пошло не так)

## Проверка конкретных компонентов

### Проверка ProductService

**Действие:** Создайте продукт

**Ожидаемые логи:**
```
info: WebApplication1.Services.ProductService[0]
      Product created successfully: {ProductId}, Name: {ProductName}
```

### Проверка ProductController

**Действие:** Получите несуществующий продукт

**Ожидаемые логи:**
```
warn: WebApplication1.Controllers.ProductController[0]
      Product not found: {ProductId}
```

### Проверка ProductRepository

**Действие:** Остановите БД и попробуйте получить продукты

**Ожидаемые логи:**
```
fail: WebApplication1.Repositories.ProductRepository[0]
      Error getting all products from database
      System.Exception: ...
```

### Проверка AuthController

**Действие:** Войдите с пустыми данными

**Ожидаемые логи:**
```
warn: WebApplication1.Controllers.AuthController[0]
      Login attempt with empty username or password
```

## Автоматизированный тест

Создайте файл `test-logging.ps1`:

```powershell
# Тест логирования
Write-Host "Testing ILogger..." -ForegroundColor Cyan

# 1. Получить токен
Write-Host "`n1. Testing login logging..." -ForegroundColor Yellow
$loginResponse = Invoke-RestMethod -Uri "http://localhost:5000/auth/login" `
    -Method POST `
    -ContentType "application/json" `
    -Body '{"username":"testuser","password":"testpass"}'
Write-Host "Token received: $($loginResponse.Token.Substring(0,20))..." -ForegroundColor Green

# 2. Получить продукты
Write-Host "`n2. Testing product retrieval logging..." -ForegroundColor Yellow
try {
    $products = Invoke-RestMethod -Uri "http://localhost:5000/product"
    Write-Host "Products retrieved: $($products.Count)" -ForegroundColor Green
} catch {
    Write-Host "Error: $_" -ForegroundColor Red
}

# 3. Создать продукт
Write-Host "`n3. Testing product creation logging..." -ForegroundColor Yellow
$headers = @{
    "Authorization" = "Bearer $($loginResponse.Token)"
    "Content-Type" = "application/json"
}
$product = @{
    name = "Logger Test Product"
    price = 99.99
    quantity = 10
} | ConvertTo-Json

try {
    $created = Invoke-RestMethod -Uri "http://localhost:5000/product" `
        -Method POST `
        -Headers $headers `
        -Body $product
    Write-Host "Product created: $($created.id)" -ForegroundColor Green
} catch {
    Write-Host "Error: $_" -ForegroundColor Red
}

# 4. Получить несуществующий продукт
Write-Host "`n4. Testing warning logging..." -ForegroundColor Yellow
try {
    $response = Invoke-WebRequest -Uri "http://localhost:5000/product/99999" -ErrorAction Stop
} catch {
    if ($_.Exception.Response.StatusCode -eq 404) {
        Write-Host "404 Not Found (expected) - Warning should be logged" -ForegroundColor Yellow
    }
}

Write-Host "`n✅ All tests completed! Check console output for logs." -ForegroundColor Green
```

**Запуск:**
```powershell
.\test-logging.ps1
```

## Проверка структурированного логирования

### Что проверить:

1. **Параметры логируются правильно:**
   ```csharp
   _logger.LogInformation("Product created: {ProductId}, Name: {ProductName}", id, name);
   ```
   В логах должно быть: `Product created: 1, Name: Test Product`

2. **Исключения логируются с контекстом:**
   ```csharp
   _logger.LogError(ex, "Error getting product: {ProductId}", id);
   ```
   В логах должно быть исключение + контекст

3. **Категория правильная:**
   Логи должны начинаться с правильной категории:
   - `WebApplication1.Services.ProductService`
   - `WebApplication1.Controllers.ProductController`
   - `WebApplication1.Repositories.ProductRepository`

## Чек-лист проверки

- [ ] Логи выводятся в консоль при запуске приложения
- [ ] LogInformation работает (успешные операции)
- [ ] LogWarning работает (не найдено, предупреждения)
- [ ] LogError работает (исключения)
- [ ] Параметры логируются правильно
- [ ] Исключения логируются с контекстом
- [ ] Категории логов правильные
- [ ] Уровни логирования настраиваются через appsettings.json

## Резюме

1. **Настройте** уровни логирования в `appsettings.json`
2. **Запустите** приложение через `dotnet run`
3. **Выполните** действия через API (Swagger, curl, Postman)
4. **Проверьте** логи в консоли
5. **Убедитесь**, что логи содержат нужную информацию

Если логи не появляются, проверьте:
- Уровень логирования в appsettings.json
- Правильность категории в ILogger<ClassName>
- Что действия действительно вызывают методы с логированием

