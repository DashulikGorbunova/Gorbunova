# Быстрый старт

## 1. Запуск инфраструктуры

```bash
# Запустить PostgreSQL и Redis
docker-compose up -d postgres redis

# Дождаться готовности сервисов (около 10 секунд)
```

## 2. Применение миграций БД

```bash
# Применить все миграции Liquibase
docker-compose --profile migrate run liquibase
```

## 3. Инициализация тестовых данных (опционально)

```bash
# Подключиться к PostgreSQL
docker exec -it appone-postgres psql -U appone_user -d appone_db

# Выполнить скрипт инициализации
\i /path/to/Scripts/InitTestData.sql
```

Или через docker:

```bash
docker exec -i appone-postgres psql -U appone_user -d appone_db < Scripts/InitTestData.sql
```

## 4. Запуск API

### Вариант 1: Через Docker

```bash
docker-compose up -d api
```

API будет доступно на `http://localhost:5000`

### Вариант 2: Локально

```bash
dotnet run
```

API будет доступно на `http://localhost:5000` (или порт из launchSettings.json)

## 5. Тестирование API

### Получить JWT токен

```bash
curl -X POST http://localhost:5000/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{"username":"admin","password":"password"}'
```

### Использовать токен

```bash
curl -X GET http://localhost:5000/api/flowers \
  -H "Authorization: Bearer YOUR_TOKEN_HERE"
```

### Использовать API Key

```bash
curl -X GET http://localhost:5000/api/flowers \
  -H "X-API-KEY: test-api-key-12345"
```

## 6. Swagger UI

Откройте в браузере: `http://localhost:5000/swagger`

## Тестовые учетные данные

После выполнения InitTestData.sql:

- **Admin**: username=`admin`, password=`password`
- **Manager**: username=`manager`, password=`password`
- **User**: username=`user`, password=`password`
- **API Key**: `test-api-key-12345`

## Проверка Health Check

```bash
curl http://localhost:5000/health
```

## Остановка

```bash
docker-compose down
```

Для удаления всех данных (volumes):

```bash
docker-compose down -v
```

