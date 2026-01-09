# Dapper - Микро-ORM для .NET

## Что такое Dapper?

**Dapper** - это легковесный микро-ORM (Object-Relational Mapping), созданный командой Stack Overflow. Он является расширением для `IDbConnection` и предоставляет простой способ выполнения SQL-запросов и маппинга результатов в объекты C#.

## Для чего используется Dapper?

### Основные преимущества:

1. **Производительность** ⚡
   - Один из самых быстрых ORM для .NET
   - Минимальные накладные расходы
   - Прямое выполнение SQL-запросов

2. **Простота** 🎯
   - Легковесный (один файл, ~1500 строк кода)
   - Не требует сложной конфигурации
   - Понятный и читаемый код

3. **Гибкость** 🔧
   - Полный контроль над SQL-запросами
   - Поддержка сложных запросов и хранимых процедур
   - Легко интегрируется с существующим кодом

4. **Безопасность** 🔒
   - Поддержка параметризованных запросов (защита от SQL-инъекций)
   - Использование `@param` синтаксиса

## Как добавлен Dapper в проект

### 1. Установка пакетов

```bash
dotnet add package Dapper
dotnet add package Npgsql
```

### 2. Создана фабрика подключений

**Файл:** `Data/IDbConnectionFactory.cs`

```csharp
public interface IDbConnectionFactory
{
    IDbConnection CreateConnection();
}

public class DbConnectionFactory : IDbConnectionFactory
{
    // Создает подключение к PostgreSQL
}
```

### 3. Репозитории переписаны на Dapper

**Пример:** `Repositories/ProductRepository.cs`

```csharp
public async Task<IEnumerable<Product>> GetAllAsync()
{
    using var connection = _connectionFactory.CreateConnection();
    const string sql = @"
        SELECT id AS Id, name AS Name, description AS Description, 
               price AS Price, quantity AS Quantity, 
               created_at AS CreatedAt, updated_at AS UpdatedAt 
        FROM products 
        ORDER BY created_at DESC";
    
    return await connection.QueryAsync<Product>(sql);
}
```

### 4. Регистрация в Program.cs

```csharp
// Dapper connection factory
builder.Services.AddSingleton<IDbConnectionFactory, DbConnectionFactory>();
```

## Основные методы Dapper

### QueryAsync<T> - для SELECT запросов
```csharp
var products = await connection.QueryAsync<Product>("SELECT * FROM products");
```

### QueryFirstOrDefaultAsync<T> - для одной записи
```csharp
var product = await connection.QueryFirstOrDefaultAsync<Product>(
    "SELECT * FROM products WHERE id = @Id", 
    new { Id = 1 });
```

### ExecuteAsync - для INSERT, UPDATE, DELETE
```csharp
await connection.ExecuteAsync(
    "INSERT INTO products (name, price) VALUES (@Name, @Price)",
    new { Name = "Product", Price = 100 });
```

### QuerySingleAsync<T> - для получения одного значения
```csharp
var id = await connection.QuerySingleAsync<int>(
    "INSERT INTO products (name) VALUES (@Name) RETURNING id",
    new { Name = "Product" });
```

## Отличия от Entity Framework Core

| Характеристика | Dapper | Entity Framework Core |
|---------------|--------|---------------------|
| **Производительность** | Очень высокая | Средняя-высокая |
| **Размер** | Минимальный | Большой |
| **SQL запросы** | Написание вручную | Генерация автоматически |
| **Миграции** | Вручную (Liquibase) | Автоматические |
| **Сложность** | Простая | Средняя-высокая |
| **Контроль** | Полный контроль | Абстракция |

## Когда использовать Dapper?

✅ **Используйте Dapper, когда:**
- Нужна максимальная производительность
- Простые CRUD операции
- Хотите полный контроль над SQL
- Работаете с существующей БД
- Не нужны сложные связи между сущностями

❌ **Не используйте Dapper, когда:**
- Нужны автоматические миграции
- Сложные связи между сущностями
- Хотите избежать написания SQL
- Нужен трекинг изменений

## Текущая реализация

В проекте Dapper используется для:
- ✅ `ProductRepository` - работа с продуктами
- ✅ `CategoryRepository` - работа с категориями

Все операции выполняются через параметризованные запросы для безопасности.

