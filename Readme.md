1. Add 'RabbitMQ.Client' package in the Application to use the RabbitMQ for communication
2. Install Rabbit MQ
    - https://www.rabbitmq.com/download.html
    - Use the Rabbit MQ Docker Image
        - # command to pull the image and run it
          # 15672:15672, then port for management console
          # 5672:5672, port use by AMQP protocol
                - Asynchronous Message Queue Protocol
                - Protocol usd by Rabbit MQ For Messaging
          # rabbitmq:3-management , image name


docker run -d --hostname my-rabbitap --name emessage-rabbit -p 15672:15672 -p 5672:5672 rabbitmq:3-management

# log or run the rabbitmq image 742 are first 3 digits of the image
docker logs -f 724

# browse the rabbit mq in the browser

http://localhost:15672
User Name is guest and password is guest
   