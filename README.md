# LostFilm.tv torrent feed
Этот проект создан для автоматизации процесса загрузки новых эпизодов сериалов с сайта [https://www.lostfilm.tv](https://www.lostfilm.tv).

#### 1. Выбираем сериалы.
![Выбираем сериалы](https://github.com/lAnubisl/LostFilmTorrentsFeed/blob/master/LostFilmMonitoring.Web/wwwroot/images/f0.png?raw=true)
#### 2. Выбираем качество.
![Выбираем качество](https://github.com/lAnubisl/LostFilmTorrentsFeed/blob/master/LostFilmMonitoring.Web/wwwroot/images/f1.png?raw=true)
#### 3. Копируем персонализированную ссылку.
![Копируем персонализированную ссылку](https://github.com/lAnubisl/LostFilmTorrentsFeed/blob/master/LostFilmMonitoring.Web/wwwroot/images/f2.png?raw=true)
#### 4. Вставляем в Feeds вашего torrent клиента.
![Загрузка будет проходить автоматически](https://github.com/lAnubisl/LostFilmTorrentsFeed/blob/master/LostFilmMonitoring.Web/wwwroot/images/f3.png?raw=true)
## Установка
Если собирать из исходников лень - можете скачать готовые сборки отсюда: [https://disk.yandex.by/d/v-87A2al4mnXyg?w=1](https://disk.yandex.by/d/v-87A2al4mnXyg?w=1)
Скрипты для сборки проекта находятся в директории "build_and_run".
## Запуск
Для запуска нужно в переменные окружения вписать "BASEFEEDCOOKIE" и в качестве значения вставить значение cookie 'lf_session' с любого реального аккаунта LostFilm.tv Без этого значения планировщик не сможет ходить на сайт чтобы обновлять ссылки на торренты.
Если вы скачали готовый архив то внутри найдёте файл run.ps1. Откройте его, вставьте туда значение cookie в $Env:BASEFEEDCOOKIE = "" и запусииие скрипт.
