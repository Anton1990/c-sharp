В репозитории два проекта для запуска в Docker.
Веб Api:
  Для сборки в Docker и запуска контейнера
    docker build -t dockerwebapi -f Dockerfile .
    docker run -ti --rm -p 8080:80 dockerwebapi
  Url для тестирования:
    http://localhost:9000/WeatherForecast
 Console App:
 Для сборки в Docker и запуска контейнера
   docker build -t dockerwebapi -f Dockerfile .
   docker run -i -t dockerwebapi:latest /bin/bash



