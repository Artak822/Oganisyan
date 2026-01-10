# WishList API

REST API сервис для управления списками желаний (WishList) на платформе ASP.NET Core Web API.

## Технологический стек

- ASP.NET Core 8.0 Web API
- PostgreSQL
- Redis
- Docker + docker-compose
- Liquibase
- Entity Framework Core
- Dapper
- FluentValidation
- Swagger (OpenAPI)
- JWT (Bearer authentication)
- API Key authentication
- Serilog
- Rate Limiting

## Архитектура проекта

```
/Controllers          - API контроллеры
/Auth                 - Аутентификация
/Services             - Бизнес-логика
/Interfaces           - Интерфейсы сервисов
/Repositories         - Доступ к данным
/Interfaces           - Интерфейсы репозиториев
/Data                 - Контекст БД
/Models               - Модели данных
/Entities             - Сущности БД
/DTO                  - Data Transfer Objects
/Middleware           - Middleware компоненты
/Validators           - Валидаторы FluentValidation
```

## Функциональность

### Сущности

- **User** - пользователи системы
- **Wish** - подарки/желания
- **WishRecommendation** - рекомендации подарков (many-to-many связь)
- **ApiKey** - API ключи для системных клиентов

### Роли пользователей

- **Admin** - полный доступ (Read, Create, Update, Delete)
- **Manager** - доступ на чтение, создание и обновление (Read, Create, Update)
- **User** - доступ на чтение и создание собственных записей (Read, Create для своих, Update/Delete только для своих)

### API Endpoints

#### Аутентификация
- `POST /api/auth/register` - регистрация нового пользователя
- `POST /api/auth/login` - вход и получение JWT токена

#### Подарки (Wishes)
- `GET /api/wishes` - получить список подарков с пагинацией и фильтрацией
- `GET /api/wishes/{id}` - получить подарок по ID
- `POST /api/wishes` - создать новый подарок
- `PUT /api/wishes/{id}` - обновить подарок
- `DELETE /api/wishes/{id}` - удалить подарок
- `GET /api/wishes/recommendations` - получить рекомендации для текущего пользователя

#### Health Check
- `GET /health` - проверка состояния API, PostgreSQL и Redis

## Запуск проекта

### Требования

- Docker и Docker Compose
- .NET 8.0 SDK (для локальной разработки)

### Запуск через Docker Compose

```bash
docker-compose up -d
```

Сервисы будут доступны:
- API: http://localhost:5001
- Swagger: http://localhost:5001/swagger
- PostgreSQL: localhost:5432
- Redis: localhost:6379

### Локальная разработка

1. Запустить PostgreSQL и Redis через Docker:
```bash
docker-compose up -d postgres redis liquibase
```

2. Запустить API:
```bash
cd src/WishList.API
dotnet run
```

## Особенности реализации

### Idempotency
Поддержка Idempotency-Key для POST запросов. Повторный запрос с тем же ключом не создаст дубликат.

### Кэширование (Redis)
- Кэширование GET запросов для подарков
- Автоматическая инвалидация кэша при изменениях

### Rate Limiting
Ограничение: 100 запросов в минуту на все endpoints.

### Логирование
Используется Serilog для логирования:
- Входящие HTTP-запросы
- Ошибки
- Бизнес-события

### Обработка ошибок
Глобальный middleware для обработки всех исключений с единым форматом ответа.

### Метрики Prometheus
Метрики доступны на `/metrics` endpoint.

## Тестирование

### Unit-тесты
```bash
cd tests/WishList.Tests
dotnet test
```

## Конфигурация

Настройки находятся в `appsettings.json`:
- ConnectionStrings - строки подключения к БД и Redis
- JwtSettings - настройки JWT токенов
- IpRateLimiting - настройки rate limiting
- Serilog - настройки логирования

## Swagger

Swagger UI доступен по адресу `/swagger` с полным описанием всех endpoints, DTO и возможных HTTP-кодов ответа.

## API Key

Для получения API-KEY запустите скрипт `./create-apikey.sh [name] [days-valid]`