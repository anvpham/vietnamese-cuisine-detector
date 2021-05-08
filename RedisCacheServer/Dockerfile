FROM redis

COPY food-script.txt /redis-scripts/food-script.txt

COPY redis.conf /usr/local/etc/redis/redis.conf

CMD [ "redis-server", "/usr/local/etc/redis/redis.conf" ]
