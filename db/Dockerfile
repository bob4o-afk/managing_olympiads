FROM mysql:8.0

COPY OlympiadDB.sql /docker-entrypoint-initdb.d/
COPY Sample.sql /docker-entrypoint-initdb.d/Sample.sql


EXPOSE 3306
