#!/bin/bash

# Скрипт для создания API ключа через подключение к базе данных
# Использование: ./create-apikey.sh [name] [days_valid]

NAME=${1:-"Default API Key"}
DAYS_VALID=${2:-365}

# Генерируем безопасный API ключ (Base64-кодированный GUID)
API_KEY=$(python3 -c "import base64, uuid; key = base64.b64encode(uuid.uuid4().bytes).decode('utf-8'); print(key.replace('/', '_').replace('+', '-').rstrip('='))")

echo "Creating API Key..."
echo "Name: $NAME"
echo "Valid for: $DAYS_VALID days"
echo "API Key: $API_KEY"
echo ""

# Подключаемся к PostgreSQL через docker-compose
docker-compose exec -T postgres psql -U postgres -d wishlistdb <<EOF
INSERT INTO "ApiKeys" ("Id", "Key", "Name", "ExpiresAt", "IsActive", "CreatedAt")
VALUES (
    gen_random_uuid(),
    '$API_KEY',
    '$NAME',
    NOW() + INTERVAL '$DAYS_VALID days',
    true,
    NOW()
);
EOF

if [ $? -eq 0 ]; then
    echo ""
    echo "✅ API Key успешно создан!"
    echo ""
    echo "Используйте этот ключ для авторизации:"
    echo "  X-API-KEY: $API_KEY"
    echo ""
    echo "Пример использования:"
    echo "  curl -H \"X-API-KEY: $API_KEY\" http://localhost:5001/api/wishes"
else
    echo ""
    echo "❌ Ошибка при создании API ключа"
    exit 1
fi
