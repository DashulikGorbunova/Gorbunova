# Настройка базы данных

## Создание таблиц

### Способ 1: Через Docker (рекомендуется)

Если контейнер PostgreSQL уже запущен:

```bash
# Создать таблицу categories
docker exec -i appone-postgres psql -U appone_user -d appone_db -c "CREATE TABLE IF NOT EXISTS categories (id SERIAL PRIMARY KEY, name VARCHAR(255) NOT NULL, description VARCHAR(1000), is_active BOOLEAN NOT NULL DEFAULT true, created_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP, updated_at TIMESTAMP);"

# Создать индексы
docker exec -i appone-postgres psql -U appone_user -d appone_db -c "CREATE INDEX IF NOT EXISTS idx_categories_name ON categories(name); CREATE INDEX IF NOT EXISTS idx_categories_is_active ON categories(is_active);"
```

### Способ 2: Через Liquibase

```bash
# Запустить миграции через Docker Compose
docker-compose --profile migrate up liquibase
```

### Способ 3: Выполнить SQL скрипт напрямую

```bash
# Если у вас установлен psql
psql -h localhost -p 15432 -U appone_user -d appone_db -f Scripts/CreateTables.sql
```

## Проверка таблиц

```bash
# Проверить существование таблицы categories
docker exec -i appone-postgres psql -U appone_user -d appone_db -c "\d categories"

# Проверить все таблицы
docker exec -i appone-postgres psql -U appone_user -d appone_db -c "\dt"
```

## Структура таблиц

### Таблица `products`
- `id` - SERIAL PRIMARY KEY
- `name` - VARCHAR(255) NOT NULL
- `description` - VARCHAR(1000)
- `price` - DECIMAL(18,2) NOT NULL
- `quantity` - INTEGER NOT NULL DEFAULT 0
- `created_at` - TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP
- `updated_at` - TIMESTAMP

### Таблица `categories`
- `id` - SERIAL PRIMARY KEY
- `name` - VARCHAR(255) NOT NULL
- `description` - VARCHAR(1000)
- `is_active` - BOOLEAN NOT NULL DEFAULT true
- `created_at` - TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP
- `updated_at` - TIMESTAMP

## Индексы

- `idx_products_name` - индекс по полю name в таблице products
- `idx_categories_name` - индекс по полю name в таблице categories
- `idx_categories_is_active` - индекс по полю is_active в таблице categories

