# `Архив погоды`

#### <code> Испытательное задание </code>

[Тестовое задание.webm](https://github.com/user-attachments/assets/a5061672-ab8e-4c72-b016-2f15967bc766)

Демо: https://vpn.tail1a578.ts.net/
#####  Содержание

- [ Суть](#суть)
- [ Возможности](#возможности)
- [ Начало работы](#начало-работы)
    - [ Предварительные условия](#предварительные-условия)
    - [ Развёртывание](#развёртывание)
    - [ Установка](#установка)
    - [ Запуск](#запуск)
- [ Лицензия](#лицензия)

---

##  Суть

<code>  Это ASP.NET Core MVC приложение для загрузки и отображения архивов
погодных условий в городе Москве </code>

---

##  Возможности

- Загрузка архивов погодных условий в виде excel файлов
- Сохранение архивов погодных условий в базе данных
- Отображение архивов погодных условий в виде таблицы
- Фильтрация таблицы по годам, месяцам
- Постраничная навигация по таблице
---

##  Начало работы

###  Предварительные условия

Для развёртывания:
>   [**Docker**](https://www.docker.com/products/docker-desktop/)

Для установки без Docker:
>   [**.NET**](https://dotnet.microsoft.com/ru-ru/download/dotnet/8.0): `  
> version 8.0 `   
>   [**PostgreSQL**](https://www.postgresql.org/download/)  

## Развёртывание

Чтобы развёрнуть приложение, выполните команду:

```sh
❯ docker-compose up --build
```

###  Установка

Соберите проект из исходников:

1. Склонируйте репозиторий:
```sh
❯ git clone https://github.com/Leplik500/WeatherArchive
```

2. Перейдите в директорию проекта:
```sh
❯ cd WeatherArchive
```

3. Установите зависимости:

#### PostgreSQL
1. Запустите службу PostgreSQL.
2. Создайте базу данных и установите данные пользователя, как указано в ConnectionStrings в файле `appsettings.json`.

Установите пакеты dotnet и соберите проект:
```sh
❯ dotnet restore
❯ dotnet build
```

###  Запуск

Чтобы запустить приложение, выполните команду:

```sh
❯ dotnet run
```
---

##  Лицензия

Этот проект защищен лицензией [Mozilla Public License 2.0](https://choosealicense.com/licenses/mpl-2.0/). Для получения более подробной информации обратитесь к файлу [LICENSE](https://choosealicense.com/licenses/mpl-2.0/).


