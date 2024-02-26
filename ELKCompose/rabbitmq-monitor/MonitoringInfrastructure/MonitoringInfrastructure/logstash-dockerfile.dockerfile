FROM logstash:7.10.1
WORKDIR /plugin-install
RUN logstash-plugin install --no-verify logstash-integration-rabbitmq