# Omotemachi API

## О проекте
**Omotemachi** — это ASP.NET Core 8.0 API для взаимодействия с системой проекта Omotemachi (является ядром проекта), который включает в себя 2 Discord-бота, Веб-сайт и Graphics-API.  
Приложение разворачивается в контейнере Docker.  

**Стек технологий:**
- ASP.NET Core 8.0 (C#)
- PostgreSQL
- Docker (multi-stage build)
- GitHub Actions (CI/CD)
- Telegram Bot API (уведомления)

## CI/CD
Для автоматизации сборки используется **GitHub Actions**.  
[Pipeline](github/workflows/ci-cd.yml) выполняет следующие шаги:

1. Проверка кода из репозитория.  
2. Авторизация в Docker Hub.  
3. Сборка Docker-образа на основе `Dockerfile`.  
4. Публикация образа в Docker Hub (`{{username}}/omotemachi:latest`).  
5. Отправка уведомления в Telegram-бот.

### Secrets (GitHub → Settings → Secrets → Actions)
- `DOCKER_USERNAME` — логин Docker Hub  
- `DOCKER_PASSWORD` — токен Docker Hub  
- `TELEGRAM_TOKEN` — токен Telegram-бота  
- `TELEGRAM_CHAT_ID` — ID чата для уведомлений  

## Триггеры
CI/CD запускается автоматически:  
- при пуше в ветку `main`;  
- вручную через вкладку **Actions** → `Run workflow`.

## Результаты сборки
- Собранный Docker-образ заливается в Docker Hub для дальнейшей развёртки.  
- Telegram-бот присылает уведомления о статусе сборки (`успех` / `ошибка`).  

## Ручная сборка
Локально образ можно собрать и запустить так:
```bash
# Сборка
docker build -t {{username}}/omotemachi:latest .

# Запуск
docker run -e DatabaseConnection="{{database_connection}}" --rm -i -t -p 8080:8080 {{username}}/omotemachi
```
