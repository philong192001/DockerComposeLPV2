FROM rabbitmq
WORKDIR /home/
RUN rabbitmq-plugins enable rabbitmq_management