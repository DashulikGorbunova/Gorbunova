# Руководство по использованию Health Check Endpoint

## Что такое `/health`?

`/health` - это endpoint для проверки состояния приложения и его зависимостей (база данных, Redis и т.д.). Используется для мониторинга и автоматического обнаружения проблем.

## Как использовать

### 1. Простой запрос

**GET запрос:**
```http
GET /health
```

**Пример через curl:**
```bash
curl http://localhost:5000/health
```

**Пример через PowerShell:**
```powershell
Invoke-WebRequest -Uri http://localhost:5000/health -Method GET
```

**Пример через браузер:**
Просто откройте в браузере: `http://localhost:5000/health`

### 2. Ответ при успехе (200 OK)

```json
{
  "status": "Healthy",
  "checks": [
    {
      "name": "postgresql",
      "status": "Healthy",
      "description": null,
      "duration": 15.5
    },
    {
      "name": "redis",
      "status": "Healthy",
      "description": null,
      "duration": 2.3
    }
  ]
}
```

### 3. Ответ при проблемах (503 Service Unavailable)

```json
{
  "status": "Unhealthy",
  "checks": [
    {
      "name": "postgresql",
      "status": "Unhealthy",
      "description": "Connection timeout",
      "duration": 5000.0
    },
    {
      "name": "redis",
      "status": "Healthy",
      "description": null,
      "duration": 2.1
    }
  ]
}
```

## Статусы

### `Healthy`
Все проверки прошли успешно. Приложение готово к работе.

### `Degraded`
Некоторые проверки не прошли, но приложение может работать с ограничениями.

### `Unhealthy`
Критические проверки не прошли. Приложение не готово к работе.

## Что проверяется

### 1. PostgreSQL (postgresql)
- Проверка подключения к базе данных
- Время ответа: обычно < 100ms

### 2. Redis (redis)
- Проверка подключения к кэшу
- Время ответа: обычно < 10ms

## Использование в разных сценариях

### Мониторинг

**Периодическая проверка:**
```bash
# Каждые 30 секунд
while true; do
  curl http://localhost:5000/health
  sleep 30
done
```

**С уведомлением:**
```bash
response=$(curl -s http://localhost:5000/health)
if echo "$response" | grep -q "Unhealthy"; then
  echo "ALERT: Application is unhealthy!"
  # Отправить уведомление
fi
```

### Kubernetes

В Kubernetes health check используется для:
- **Liveness Probe** - проверка, что приложение работает
- **Readiness Probe** - проверка, что приложение готово принимать трафик

**Пример конфигурации:**
```yaml
livenessProbe:
  httpGet:
    path: /health
    port: 5000
  initialDelaySeconds: 30
  periodSeconds: 10

readinessProbe:
  httpGet:
    path: /health
    port: 5000
  initialDelaySeconds: 5
  periodSeconds: 5
```

### Docker

**В docker-compose.yml:**
```yaml
services:
  app:
    healthcheck:
      test: ["CMD", "curl", "-f", "http://localhost:5000/health"]
      interval: 30s
      timeout: 10s
      retries: 3
      start_period: 40s
```

### Load Balancer

Load balancer может использовать `/health` для:
- Определения, какие серверы готовы принимать трафик
- Автоматического исключения нездоровых серверов
- Распределения нагрузки только на здоровые серверы

**Пример для Nginx:**
```nginx
upstream backend {
    server app1:5000;
    server app2:5000;
}

server {
    location /health {
        proxy_pass http://backend;
    }
}
```

### CI/CD Pipeline

**Проверка после деплоя:**
```yaml
# GitHub Actions пример
- name: Health Check
  run: |
    for i in {1..30}; do
      if curl -f http://localhost:5000/health; then
        echo "Health check passed"
        exit 0
      fi
      sleep 2
    done
    echo "Health check failed"
    exit 1
```

## Примеры использования

### 1. Через Swagger

1. Откройте Swagger UI
2. Найдите `GET /health`
3. Нажмите "Try it out"
4. Нажмите "Execute"
5. Посмотрите ответ

### 2. Через Postman

1. Создайте новый запрос
2. Метод: `GET`
3. URL: `http://localhost:5000/health`
4. Отправьте запрос

### 3. Через PowerShell

```powershell
# Простой запрос
$response = Invoke-RestMethod -Uri "http://localhost:5000/health"
$response | ConvertTo-Json

# С проверкой статуса
$response = Invoke-RestMethod -Uri "http://localhost:5000/health"
if ($response.status -eq "Healthy") {
    Write-Host "Application is healthy" -ForegroundColor Green
} else {
    Write-Host "Application is unhealthy" -ForegroundColor Red
}
```

### 4. Через curl

```bash
# Простой запрос
curl http://localhost:5000/health

# С форматированием JSON
curl http://localhost:5000/health | jq

# С проверкой статуса
status=$(curl -s http://localhost:5000/health | jq -r '.status')
if [ "$status" = "Healthy" ]; then
    echo "OK"
else
    echo "FAIL"
fi
```

### 5. Через JavaScript (браузер)

```javascript
fetch('http://localhost:5000/health')
  .then(response => response.json())
  .then(data => {
    console.log('Status:', data.status);
    console.log('Checks:', data.checks);
  })
  .catch(error => console.error('Error:', error));
```

### 6. Через Python

```python
import requests

response = requests.get('http://localhost:5000/health')
data = response.json()

print(f"Status: {data['status']}")
for check in data['checks']:
    print(f"  {check['name']}: {check['status']} ({check['duration']}ms)")
```

## Интерпретация результатов

### Все проверки Healthy
```
✅ Приложение полностью работоспособно
✅ База данных доступна
✅ Redis доступен
✅ Можно принимать трафик
```

### Одна проверка Unhealthy
```
⚠️ Приложение частично работоспособно
❌ Одна из зависимостей недоступна
⚠️ Может работать с ограничениями
```

### Все проверки Unhealthy
```
❌ Приложение не работоспособно
❌ Критические зависимости недоступны
❌ Не следует принимать трафик
```

## Настройка проверок

Проверки настраиваются в `Program.cs`:

```csharp
builder.Services.AddHealthChecks()
    .AddNpgSql(
        connectionString: "...",
        name: "postgresql",
        tags: new[] { "db", "sql", "postgresql" })
    .AddRedis(
        redisConnectionString: "...",
        name: "redis",
        tags: new[] { "cache", "redis" });
```

## Мониторинг и алерты

### Настройка алертов

**Пример скрипта для мониторинга:**
```bash
#!/bin/bash
HEALTH_URL="http://localhost:5000/health"
STATUS=$(curl -s $HEALTH_URL | jq -r '.status')

if [ "$STATUS" != "Healthy" ]; then
    # Отправить уведомление
    echo "ALERT: Application health check failed!"
    # Можно отправить email, SMS, Slack и т.д.
fi
```

## Часто задаваемые вопросы

### Q: Как часто нужно проверять `/health`?
**A:** Зависит от требований. Обычно:
- Production: каждые 10-30 секунд
- Development: по необходимости

### Q: Можно ли использовать `/health` для проверки перед деплоем?
**A:** Да, это стандартная практика. Проверяйте health check после деплоя перед переключением трафика.

### Q: Что делать, если health check возвращает Unhealthy?
**A:** 
1. Проверьте логи приложения
2. Проверьте доступность базы данных
3. Проверьте доступность Redis
4. Проверьте конфигурацию подключений

### Q: Можно ли добавить свои проверки?
**A:** Да, можно добавить кастомные health checks:

```csharp
builder.Services.AddHealthChecks()
    .AddCheck<CustomHealthCheck>("custom", tags: new[] { "custom" });
```

## Резюме

1. **Использование:** `GET /health`
2. **Ответ:** JSON с статусом и деталями проверок
3. **Статусы:** Healthy, Degraded, Unhealthy
4. **Применение:** Мониторинг, Kubernetes, Load Balancer, CI/CD
5. **Проверяет:** PostgreSQL, Redis

Health check endpoint - это важный инструмент для обеспечения надежности приложения!

