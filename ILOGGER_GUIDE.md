# Руководство по использованию ILogger

## Что такое ILogger?

`ILogger<T>` - это интерфейс для логирования в .NET, который является частью `Microsoft.Extensions.Logging`. Он предоставляет структурированное логирование с различными уровнями важности.

## Внедрение ILogger

### В конструкторе класса

```csharp
public class ProductService : IProductService
{
    private readonly ILogger<ProductService> _logger;

    public ProductService(
        IProductRepository productRepository, 
        IRedisCacheService cache, 
        ILogger<ProductService> logger)
    {
        _productRepository = productRepository;
        _cache = cache;
        _logger = logger;  // Внедрение через DI
    }
}
```

**Важно:** Используйте `ILogger<ClassName>` для правильной категоризации логов.

## Уровни логирования

### 1. LogTrace (0)
Самый детальный уровень. Используется для отладки.

```csharp
_logger.LogTrace("Processing item {ItemId}", itemId);
```

### 2. LogDebug (1)
Отладочная информация. Полезно при разработке.

```csharp
_logger.LogDebug("Cache hit for key: {CacheKey}", cacheKey);
```

### 3. LogInformation (2)
Информационные сообщения о нормальной работе приложения.

```csharp
_logger.LogInformation("Product created successfully: {ProductId}, Name: {ProductName}", 
    product.Id, product.Name);
```

### 4. LogWarning (3)
Предупреждения о потенциальных проблемах.

```csharp
_logger.LogWarning("Product not found: {ProductId}", id);
_logger.LogWarning("Login attempt with empty username or password");
```

### 5. LogError (4)
Ошибки и исключения, которые не останавливают работу приложения.

```csharp
_logger.LogError(ex, "Error getting product by id: {ProductId}", id);
_logger.LogError("Failed to connect to database");
```

### 6. LogCritical (5)
Критические ошибки, которые могут привести к остановке приложения.

```csharp
_logger.LogCritical(ex, "Database connection lost. Application may be unstable.");
```

## Структурированное логирование

### Использование параметров (рекомендуется)

✅ **Правильно:**
```csharp
_logger.LogError(ex, "Error getting product by id: {ProductId}", id);
_logger.LogInformation("User {Username} logged in at {LoginTime}", 
    username, DateTime.UtcNow);
```

❌ **Неправильно:**
```csharp
_logger.LogError($"Error getting product by id: {id}");  // Не используйте интерполяцию!
```

### Почему использовать параметры?

1. **Производительность** - строка форматируется только если уровень логирования включен
2. **Структурированные логи** - параметры можно фильтровать и анализировать
3. **Безопасность** - чувствительные данные не попадают в строки

## Логирование исключений

### С контекстом

```csharp
try
{
    var product = await _productRepository.GetByIdAsync(id);
    return product;
}
catch (Exception ex)
{
    _logger.LogError(ex, "Error getting product by id: {ProductId}", id);
    throw;  // Пробрасываем исключение дальше
}
```

### Без проброса исключения

```csharp
try
{
    await _cache.SetAsync(key, value);
}
catch (Exception ex)
{
    _logger.LogError(ex, "Error setting value in Redis cache for key: {CacheKey}", key);
    return default;  // Возвращаем значение по умолчанию
}
```

## Примеры использования в проекте

### В контроллерах

```csharp
[HttpGet("{id}")]
public async Task<ActionResult<Product>> Get(int id)
{
    try
    {
        var product = await _productService.GetByIdAsync(id);
        
        if (product == null)
        {
            _logger.LogWarning("Product not found: {ProductId}", id);
            return NotFound();
        }
        
        return Ok(product);
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error getting product: {ProductId}", id);
        return StatusCode(500, new { message = "An error occurred" });
    }
}
```

### В сервисах

```csharp
public async Task<Product> CreateAsync(Product product)
{
    try
    {
        product.CreatedAt = DateTime.UtcNow;
        await _productRepository.AddAsync(product);
        await _productRepository.SaveChangesAsync();
        
        _logger.LogInformation("Product created successfully: {ProductId}, Name: {ProductName}", 
            product.Id, product.Name);
        
        return product;
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error creating product: {ProductName}", product.Name);
        throw;
    }
}
```

### В репозиториях

```csharp
public async Task<Product?> GetByIdAsync(int id)
{
    try
    {
        using var connection = _connectionFactory.CreateConnection();
        const string sql = "SELECT * FROM products WHERE id = @Id";
        
        return await connection.QueryFirstOrDefaultAsync<Product>(sql, new { Id = id });
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error getting product by id from database: {ProductId}", id);
        throw;
    }
}
```

## Настройка уровней логирования

### В appsettings.json

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning",
      "Microsoft.EntityFrameworkCore": "Information",
      "WebApplication1": "Debug"  // Для вашего проекта
    }
  }
}
```

### Уровни по умолчанию:
- **Production**: `Warning` и выше
- **Development**: `Information` и выше
- **Debug**: `Debug` и выше

## Лучшие практики

### ✅ Делайте так:

1. **Используйте структурированное логирование**
   ```csharp
   _logger.LogInformation("User {UserId} created order {OrderId}", userId, orderId);
   ```

2. **Логируйте исключения с контекстом**
   ```csharp
   _logger.LogError(ex, "Error processing payment for order {OrderId}", orderId);
   ```

3. **Используйте правильный уровень**
   - `Information` - успешные операции
   - `Warning` - не найдено, неверные данные
   - `Error` - исключения
   - `Critical` - критические сбои

4. **Не логируйте чувствительные данные**
   ```csharp
   // ❌ Плохо
   _logger.LogInformation("User {Username} logged in with password {Password}", username, password);
   
   // ✅ Хорошо
   _logger.LogInformation("User {Username} logged in", username);
   ```

### ❌ Не делайте так:

1. **Не используйте интерполяцию строк**
   ```csharp
   // ❌ Плохо
   _logger.LogError($"Error: {ex.Message}");
   
   // ✅ Хорошо
   _logger.LogError(ex, "Error occurred");
   ```

2. **Не логируйте слишком много**
   ```csharp
   // ❌ Плохо - слишком детально
   _logger.LogDebug("Starting method");
   _logger.LogDebug("Parameter validated");
   _logger.LogDebug("Database query started");
   
   // ✅ Хорошо - только важное
   _logger.LogInformation("Processing order {OrderId}", orderId);
   ```

3. **Не игнорируйте исключения**
   ```csharp
   // ❌ Плохо
   catch (Exception ex)
   {
       // Ничего не делаем
   }
   
   // ✅ Хорошо
   catch (Exception ex)
   {
       _logger.LogError(ex, "Error occurred");
       throw;  // или обработать
   }
   ```

## Просмотр логов

### В консоли (Development)
Логи выводятся в консоль автоматически.

### В файл
Настройте провайдер файлового логирования в `Program.cs`:

```csharp
builder.Logging.AddFile("logs/app-{Date}.txt");
```

### В внешние системы
- **Serilog** - популярная библиотека для логирования
- **Application Insights** - для Azure
- **ELK Stack** - для больших приложений

## Примеры из проекта

### AuthController
```csharp
_logger.LogInformation("Login attempt for user: {Username}", request.Username);
_logger.LogWarning("Login attempt with empty username or password");
_logger.LogError(ex, "Error during login for user: {Username}", request?.Username);
```

### ProductService
```csharp
_logger.LogInformation("Product created successfully: {ProductId}, Name: {ProductName}", 
    product.Id, product.Name);
_logger.LogWarning("Product not found for update: {ProductId}", id);
_logger.LogError(ex, "Error creating product: {ProductName}", product.Name);
```

### RedisCacheService
```csharp
_logger.LogError(ex, "Error getting value from Redis cache for key: {CacheKey}", key);
_logger.LogError(ex, "Error setting value in Redis cache for key: {CacheKey}", key);
```

## Резюме

1. **Внедряйте** `ILogger<ClassName>` через конструктор
2. **Используйте** структурированное логирование с параметрами
3. **Логируйте** исключения с контекстом
4. **Выбирайте** правильный уровень логирования
5. **Избегайте** интерполяции строк и чувствительных данных

Логирование помогает отслеживать работу приложения, находить ошибки и анализировать производительность!

