-- Инициализация тестовых данных

-- Создание пользователей с разными ролями
-- Пароли: все "password" (SHA256 hash)
INSERT INTO users (username, email, password_hash, role, is_active, created_at)
VALUES 
    ('admin', 'admin@flowershop.com', 'XohImNooBHFR0OVvjcYpJ3NgPQ1qq73WKhHvch0VQtg=', 'Admin', true, CURRENT_TIMESTAMP),
    ('manager', 'manager@flowershop.com', 'XohImNooBHFR0OVvjcYpJ3NgPQ1qq73WKhHvch0VQtg=', 'Manager', true, CURRENT_TIMESTAMP),
    ('user', 'user@flowershop.com', 'XohImNooBHFR0OVvjcYpJ3NgPQ1qq73WKhHvch0VQtg=', 'User', true, CURRENT_TIMESTAMP)
ON CONFLICT (username) DO NOTHING;

-- Создание тестового API ключа
INSERT INTO api_keys (key, name, is_active, expires_at, created_at)
VALUES 
    ('test-api-key-12345', 'Test API Key', true, CURRENT_TIMESTAMP + INTERVAL '1 year', CURRENT_TIMESTAMP)
ON CONFLICT (key) DO NOTHING;

-- Создание тестовых категорий
INSERT INTO flower_categories (name, description, is_active, created_at)
VALUES 
    ('Roses', 'Beautiful roses in various colors', true, CURRENT_TIMESTAMP),
    ('Tulips', 'Spring tulips', true, CURRENT_TIMESTAMP),
    ('Lilies', 'Elegant lilies', true, CURRENT_TIMESTAMP)
ON CONFLICT DO NOTHING;

-- Создание тестовых цветов
INSERT INTO flowers (name, description, price, quantity, color, season, category_id, is_available, created_at)
VALUES 
    ('Red Rose', 'Classic red rose', 15.99, 50, 'Red', 'All', 1, true, CURRENT_TIMESTAMP),
    ('White Tulip', 'Pure white tulip', 8.99, 30, 'White', 'Spring', 2, true, CURRENT_TIMESTAMP),
    ('Pink Lily', 'Delicate pink lily', 12.99, 25, 'Pink', 'Summer', 3, true, CURRENT_TIMESTAMP)
ON CONFLICT DO NOTHING;

